using System.Diagnostics;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Infrastructure;

namespace ZenBlog.Application.Behaviors;

// =====================================================================
// LOGLAMA BEHAVIOR'I
// Her MediatR isteði (komut ya da sorgu) bu behavior'dan geçer:
//   Ýstek -> [LoggingBehavior -> [ValidationBehavior -> [Handler]]] -> Cevap
// En DIÞTA olduðu için doðrulama + handler'ýn TOPLAM süresini ölçer ve doðrulama hatalarýný da görür.
//
// Ne yazar?
//   Komut baþarýlý          -> Information  ("CreateBlogCommand tamamlandý (240 ms)")
//   Sorgu baþarýlý          -> Debug        (yazýlmaz; istek logu zaten var, gürültü olmasýn)
//   Baþarýsýz (BaseResult)  -> Warning      ("LoginCommand baþarýsýz: E-posta ya da þifre hatalý.")
//   Doðrulama hatasý        -> Warning
//   500 ms'den yavaþ        -> Warning
// Ne YAZMAZ?
//   Ýsteðin ÝÇERÝÐÝ (þifre, e-posta gibi kiþisel veriler olabilir). Sadece adý ve sonucu.
// Beklenmeyen hatalar (exception) burada deðil, hata middleware'inde loglanýr (bir hata bir kez loglansýn).
//
// NOT: Serilog'a doðrudan deðil, .NET'in ILogger arayüzüne yazýyoruz.
// Application katmaný Serilog'u bilmiyor; yarýn loglama aracý deðiþse bu dosya deðiþmez.
// =====================================================================
public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger,
                                                  ICurrentUserService currentUser)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : class
{
    private const int SlowThresholdMs = 500;

    public async Task<TResponse> Handle(TRequest request,
                                        RequestHandlerDelegate<TResponse> next,
                                        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;   // örn. "BlockUserCommand"
        var isQuery = requestName.EndsWith("Query");

        // SCOPE: bu blok içinde yazýlan BÜTÜN loglara (handler'ýn içindekiler dahil)
        // RequestName ve UserId alanlarý otomatik eklenir. Kibana'da "bu komutun bütün loglarý" diye aranabilir.
        using var scope = logger.BeginScope(new Dictionary<string, object>
        {
            ["RequestName"] = requestName,
            ["UserId"] = currentUser.UserId?.ToString() ?? "anonim"
        });

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next(cancellationToken);
            stopwatch.Stop();

            var elapsedMs = stopwatch.ElapsedMilliseconds;

            // 1) BAÞARISIZ SONUÇ (handler BaseResult.Fail döndü)
            if (response is IBaseResult { IsSuccessful: false } result)
            {
                var errorMessage = string.Join(" | ", result.Errors.Select(e => e.ErrorMessage));
                logger.LogWarning("{RequestName} baþarýsýz: {ErrorMessage} ({ElapsedMs} ms)",
                                  requestName, errorMessage, elapsedMs);
            }
            // 2) YAVAÞ ÇALIÞTI
            else if (elapsedMs > SlowThresholdMs)
            {
                logger.LogWarning("{RequestName} yavaþ çalýþtý ({ElapsedMs} ms)", requestName, elapsedMs);
            }
            // 3) BAÞARILI: komutlar Information, sorgular Debug (gürültü olmasýn)
            else
            {
                logger.Log(isQuery ? LogLevel.Debug : LogLevel.Information,
                           "{RequestName} tamamlandý ({ElapsedMs} ms)", requestName, elapsedMs);
            }

            return response;
        }
        catch (ValidationException ex)
        {
            // 4) DOÐRULAMA HATASI: ValidationBehavior fýrlattý; middleware 400'e çevirecek ama LOGLAMAYACAK.
            //    Hangi alanlarýn hatalý olduðunu yazýyoruz (deðerleri DEÐÝL: deðerde þifre olabilir).
            stopwatch.Stop();
            var fields = string.Join(", ", ex.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}").Distinct());
            logger.LogWarning("{RequestName} doðrulama hatasý: {ValidationErrors} ({ElapsedMs} ms)",
                              requestName, fields, stopwatch.ElapsedMilliseconds);
            throw;   // hatayý yut-MA, middleware'e iletmeye devam et
        }
    }
}