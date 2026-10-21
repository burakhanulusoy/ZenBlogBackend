namespace ZenBlog.Application.Contracts.Infrastructure;

// "Bu Turnstile token'ı geçerli mi?" sorusunu Cloudflare'e soran servis
public interface ITurnstileService
{
    // token: frontend'in Turnstile'dan aldığı değer
    // remoteIp: kullanıcının IP adresi (Cloudflare ek kontrol için kullanıyor, isteğe bağlı)
    Task<bool> ValidateAsync(string? token, string? remoteIp);
}