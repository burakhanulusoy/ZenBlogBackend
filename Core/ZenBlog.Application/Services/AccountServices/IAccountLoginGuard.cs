using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Services.AccountServices;

// Girişte hesabın durumuna bakmanın sonucu
public enum AccountLoginState
{
    Allowed,       // aktif hesap → giriş serbest
    Reactivated,   // 30 gün dolmadan geri döndü → kapatma iptal edildi, içerikler geri açıldı
    Blocked,       // admin bloke etmiş → giriş yok
    Closed         // 30 gün dolmuş → hesap kapandı, giriş yok
}

// Normal giriş ve Google ile giriş AYNI kuralı uygulasın diye ortak servis
public interface IAccountLoginGuard
{
    Task<AccountLoginState> CheckAsync(AppUser user, CancellationToken cancellationToken);
}