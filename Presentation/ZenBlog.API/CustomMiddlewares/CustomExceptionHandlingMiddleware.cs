using FluentValidation;
using System.Diagnostics;
using ZenBlog.Application.Base;

namespace ZenBlog.API.CustomMiddlewares
{
    // =====================================================================
    // HATA YAKALAMA MIDDLEWARE'İ
    // Her HTTP isteği bu middleware'den geçer. Zincirin herhangi bir yerinde
    // (Endpoint -> MediatR -> Behavior -> Handler) exception fırlarsa burada yakalanır.
    //
    //   ValidationException -> 400, alan hataları (LoggingBehavior zaten loglar; burada tekrar loglanmaz)
    //   İstek iptal edildi  -> kullanıcı sayfadan ayrıldı; hata değil
    //   Diğer her şey       -> 500; hatanın TAMAMI loglanır, kullanıcıya sadece genel mesaj + hata kodu
    // =====================================================================
    public class CustomExceptionHandlingMiddleware(RequestDelegate next,
                                                   ILogger<CustomExceptionHandlingMiddleware> logger)   // YENİ
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (ValidationException ex)
            {
                // ValidationBehavior'ın fırlattığı hata: alan hatalarını BaseResult biçiminde dön
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";

                var response = new BaseResult<object>
                {
                    Errors = ex.Errors
                        .GroupBy(e => e.PropertyName)
                        .Select(g => new Error
                        {
                            PropertyName = g.Key,
                            ErrorMessage = g.First().ErrorMessage
                        }).ToList()
                };

                await context.Response.WriteAsJsonAsync(response);
            }
            catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
            {
                // YENİ: Kullanıcı istek bitmeden sayfayı kapattı / başka sayfaya geçti.
                // Bu bir hata değil; Error olarak loglarsak Kibana sahte hatalarla dolar.
                logger.LogInformation("İstek kullanıcı tarafından iptal edildi: {RequestMethod} {RequestPath}",
                                      context.Request.Method, context.Request.Path);
            }
            catch (Exception ex)
            {
                // YENİ: Hata kodu = isteğin TraceId'si. Kullanıcı bu kodu verirse Kibana'da tam olarak bulunur.
                // DEĞİŞTİ: Hata kodu = isteğin İZLEME KİMLİĞİ (trace.id).
                // Serilog her loga bu kimliği "trace.id" olarak ekliyor; böylece kullanıcının söylediği
                // hata koduyla o isteğin BÜTÜN logları (HTTP, komut, hata) tek aramada bulunur.
                var traceId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;

                // YENİ: Hatanın TAMAMI loga (mesaj + yığın izi + hangi istek).
                // İlk parametre "ex": Serilog yığın izini ayrı bir alan olarak saklar.
                logger.LogError(ex, "Beklenmeyen hata: {RequestMethod} {RequestPath} (Hata kodu: {TraceId})",
                                context.Request.Method, context.Request.Path, traceId);

                // Cevap zaten yazılmaya başlandıysa (nadir) başlıkları değiştiremeyiz; sadece logla
                if (context.Response.HasStarted)
                    return;

                // DEĞİŞTİ: Kullanıcıya teknik ayrıntı (ex.Message) GÖNDERİLMEZ (bilgi ifşası açığı).
                // Genel mesaj + hata kodu, BaseResult biçiminde (frontend her yerde bu biçimi okuyor).
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(BaseResult<object>.Fail(
                    "ServerError",
                    $"Beklenmeyen bir hata oluştu. Lütfen daha sonra tekrar deneyin. (Hata kodu: {traceId})"));
            }
        }
    }
}