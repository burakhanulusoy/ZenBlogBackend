using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Infrastructure;

namespace ZenBlog.API.Filters;

// Takıldığı endpoint'te, handler'dan ÖNCE Turnstile token'ını doğrular.
// Geçersizse isteği 400 ile durdurur; handler hiç çalışmaz.
public class TurnstileValidationFilter(ITurnstileService _turnstileService) : IEndpointFilter
{
    public const string HeaderName = "X-Turnstile-Token";

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var httpContext = context.HttpContext;

        var token = httpContext.Request.Headers[HeaderName].ToString();
        var ip = httpContext.Connection.RemoteIpAddress?.ToString();

        if (!await _turnstileService.ValidateAsync(token, ip))
        {
            // "Turnstile" kodu: frontend bu hatayı tanıyıp kutucuğu sıfırlayabilsin
            return Results.BadRequest(BaseResult<object>.Fail(
                "Turnstile",
                "Güvenlik doğrulaması başarısız oldu. Lütfen kutucuğu tekrar işaretleyip deneyin."));
        }

        // Token geçerli → isteğe devam (sıradaki adım: handler)
        return await next(context);
    }
}