using ZenBlog.Domain.Entities.Common;

namespace ZenBlog.Domain.Entities;

// Rol deðiþikliði geçmiþi: kim, kime, hangi rolü verdi ya da aldý, ne zaman.
// Kayýtlar hiç silinmez (denetim izi / audit log).
public class RoleChangeLog : BaseEntity
{
    // Rolü deðiþen kullanýcý
    public Guid UserId { get; set; }
    public virtual AppUser User { get; set; }

    // Deðiþikliði yapan admin
    public Guid ChangedByAdminId { get; set; }
    public virtual AppUser ChangedByAdmin { get; set; }

    public string RoleName { get; set; }
    public bool IsAdded { get; set; }        // true: rol verildi, false: rol alýndý
    public DateTime ChangedAt { get; set; }
}