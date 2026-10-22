namespace ZenBlog.Domain.Enums
{
    // Hesabýn durumu (veritabanýnda sayý olarak tutulur)
    // Active = 0: migration'da mevcut kullanýcýlarýn sütunu 0 ile dolar ? hepsi otomatik "Aktif" baþlar
    public enum AccountStatus
    {
        Active = 0,            // Aktif: normal kullaným
        PendingDeletion = 1,   // Kapatýlýyor: kiþi "Hesabýmý kapat" dedi, 30 günlük bekleme süresinde
        Blocked = 2            // Bloke: admin bloke etti
    }
}