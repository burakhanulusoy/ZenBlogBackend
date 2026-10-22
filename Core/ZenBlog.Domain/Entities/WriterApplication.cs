using ZenBlog.Domain.Entities.Common;
using ZenBlog.Domain.Enums;

namespace ZenBlog.Domain.Entities;

// "Yazar olmak istiyorum" baþvurusu. Kayýtlar silinmez; kiþinin baþvuru geçmiþi kalýr.
public class WriterApplication : BaseEntity
{
    // Baþvuran
    public Guid UserId { get; set; }
    public virtual AppUser User { get; set; }

    // Ýçerik
    public string Topics { get; set; }          // seçilen kategorilerin ADLARI ("Yazýlým, Teknoloji")
    public string Motivation { get; set; }      // neden yazar olmak istiyor (zorunlu)
    public string? Experience { get; set; }     // yazý deneyimi (isteðe baðlý)
    public string? SampleLinks { get; set; }    // örnek çalýþma baðlantýlarý (isteðe baðlý)

    // Süreç
    public WriterApplicationStatus Status { get; set; } = WriterApplicationStatus.Pending;
    public DateTime SubmittedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }   // admin ilk açtýðýnda (okundu)

    // Karar
    public DateTime? DecidedAt { get; set; }
    public Guid? DecidedByAdminId { get; set; }
    public virtual AppUser? DecidedByAdmin { get; set; }
    public string? RejectReason { get; set; }
}