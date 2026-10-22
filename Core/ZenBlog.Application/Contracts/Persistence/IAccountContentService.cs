namespace ZenBlog.Application.Contracts.Persistence;

// Bir kullanıcının içeriklerini (blog, yorum, alt yorum) topluca gizler / geri açar.
// Hem "Hesabımı kapat" hem "Bloke et / kaldır" bunu kullanıyor.
public interface IAccountContentService
{
    // Gizle: IsDeleted = 1 + HiddenByAccount = 1
    //   - kişinin blogları
    //   - kişinin yorumları + başkalarının onun bloglarına yazdığı yorumlar
    //   - kişinin alt yorumları + gizlenen yorumların altındaki alt yorumlar
    Task HideContentAsync(Guid userId, CancellationToken cancellationToken);

    // Geri aç: sadece HiddenByAccount = 1 olanlar → IsDeleted = 0
    // ÖNEMLİ: çağırmadan ÖNCE kullanıcının durumu Active yapılmış olmalı.
    // (Başka bir kullanıcının da hesabı kapalıysa onun yorumu geri gelmesin diye
    //  "yorumun sahibi aktif mi" kontrolü yapılıyor.)
    Task RestoreContentAsync(Guid userId, CancellationToken cancellationToken);
}