namespace ZenBlog.Application.Constants;

// Hesap kapatma kurallarý (tek yerden deðiþtirilsin diye)
public static class AccountRules
{
    public const int DeletionGraceDays = 30;   // "Hesabýmý kapat" sonrasý geri dönme süresi
    public const int ReminderDaysBefore = 3;   // süre bitmeden kaç gün önce hatýrlatma maili
}