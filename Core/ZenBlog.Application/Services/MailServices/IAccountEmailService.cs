using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Services.MailServices;

// Hesap durumu değişince giden bütün mailler (tek yerden yönetilsin diye)
public interface IAccountEmailService
{
    Task SendClosureScheduledAsync(AppUser user, DateTime deadline);   // "Hesabın 30 gün sonra kapanacak"
    Task SendClosureReminderAsync(AppUser user, DateTime deadline);    // "Son 3 gün"
    Task SendBlockedAsync(AppUser user, string reason);                // "Hesabın bloke edildi" (3. adımda kullanılacak)
    Task SendUnblockedAsync(AppUser user);                             // "Hesabın tekrar açıldı" (3. adımda kullanılacak)
}