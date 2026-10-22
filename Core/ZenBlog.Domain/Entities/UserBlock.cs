using ZenBlog.Domain.Entities.Common;

namespace ZenBlog.Domain.Entities;

// Bir bloke kaydý. Bloke kaldýrýlýnca kayýt SÝLÝNMEZ; Unblocked... alanlarý doldurulur.
// Böylece "kim, ne zaman, neden bloke etti / kaldýrdý" geçmiþi hep kalýr.
public class UserBlock : BaseEntity
{
    // Bloke edilen kullanýcý
    public Guid UserId { get; set; }
    public virtual AppUser User { get; set; }

    // Bloke eden admin
    public Guid BlockedByAdminId { get; set; }
    public virtual AppUser BlockedByAdmin { get; set; }

    public string Reason { get; set; }          // gerekçe (zorunlu)
    public string EvidenceUrl { get; set; }     // kanýt dosyasýnýn adresi (zorunlu)
    public DateTime BlockedAt { get; set; }

    // Bloke kaldýrýldýysa doldurulur
    public DateTime? UnblockedAt { get; set; }
    public Guid? UnblockedByAdminId { get; set; }
    public virtual AppUser? UnblockedByAdmin { get; set; }
    public string? UnblockReason { get; set; }
}