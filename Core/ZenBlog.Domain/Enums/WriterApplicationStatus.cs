namespace ZenBlog.Domain.Enums
{
    // Yazarlýk baþvurusunun durumu
    public enum WriterApplicationStatus
    {
        Pending = 0,    // Beklemede: gönderildi, admin henüz açmadý (okunmadý)
        Reviewed = 1,   // Ýncelendi: admin açtý (okundu), karar bekleniyor
        Approved = 2,   // Onaylandý: Writer rolü verildi
        Rejected = 3    // Reddedildi: gerekçeyle
    }
}