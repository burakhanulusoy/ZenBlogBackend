namespace ZenBlog.Application.Constants;

// =====================================================================
// ÖNEMLÝ OLAY TÜRLERÝ (loglarda "EventType" alaný)
// Sabit adlar: Kibana'da EventType : "LoginFailed" diye kesin arama ve
// "olay türüne göre" grafik yapýlabilsin; mesaj metni deðiþse bile arama bozulmasýn.
// =====================================================================
public static class LogEvents
{
    // Giriþ / güvenlik
    public const string LoginSucceeded = "LoginSucceeded";
    public const string LoginFailed = "LoginFailed";
    public const string AccountLocked = "AccountLocked";
    public const string TurnstileFailed = "TurnstileFailed";
    public const string RateLimited = "RateLimited";

    // Yönetim (admin iþlemleri)
    public const string UserBlocked = "UserBlocked";
    public const string UserUnblocked = "UserUnblocked";
    public const string RoleChanged = "RoleChanged";
    public const string WriterApplicationApproved = "WriterApplicationApproved";
    public const string WriterApplicationRejected = "WriterApplicationRejected";

    // Hesap yaþam döngüsü
    public const string AccountClosed = "AccountClosed";
    public const string AccountReactivated = "AccountReactivated";

    // Altyapý
    public const string EmailFailed = "EmailFailed";
    public const string ReminderJobCompleted = "ReminderJobCompleted";
    public const string ReminderJobFailed = "ReminderJobFailed";
}