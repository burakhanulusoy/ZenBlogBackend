namespace ZenBlog.Domain.Entities;

// Bir blogun bir ziyaretçi tarafýndan BÝR GÜNDE okunmasý (günlük tekil okunma).
// BaseEntity'den türemiyor: silinme / güncellenme kavramý yok, sadece kayýt.
public class BlogView
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid BlogId { get; set; }
    public virtual Blog Blog { get; set; }

    // Ziyaretçinin özetlenmiþ (SHA-256) kimliði. IP ya da kullanýcý Id'si OLDUÐU GÝBÝ saklanmaz.
    public string VisitorKey { get; set; }

    public DateOnly ViewDate { get; set; }   // tekillik ve grafikler için gün
    public DateTime CreatedAt { get; set; }
}