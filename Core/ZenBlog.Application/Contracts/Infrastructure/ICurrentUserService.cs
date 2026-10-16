namespace ZenBlog.Application.Contracts.Infrastructure;

// Şu an istek yapan (giriş yapmış) kullanıcının bilgileri.
// Bilgiler istekten değil, imzalı JWT token'dan okunur; kullanıcı bunları değiştiremez.
public interface ICurrentUserService
{
    Guid? UserId { get; }           // giriş yapılmamışsa null
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
}