using ZenBlog.Domain.Entities.Common;

namespace ZenBlog.Domain.Entities;

public class Blog:BaseEntity
{


    public string MainTitle  { get; set; }
    public string CoverImage { get; set; }
    public string BlogImage1 { get; set; }
    public string Title1 {  get; set; }
    public string Description1 { get; set; }
    public string MainDescription { get; set; }
    public string Description2 { get; set; }
    public string Title2 { get; set; }
    public string Description3 { get; set; }
    public string BlogImage2 { get; set; }
    public string Title3 { get; set; }
    public string Description4 { get; set; }

    public virtual Category Category { get; set; }
    public Guid CategoryId { get; set; }


    public virtual AppUser AppUser { get; set; }
    public Guid AppUserId { get; set; }


    public virtual IList<Comment> Comments { get; set; }


    // YENÝ: Bu kayýt, sahibinin hesap iþlemi (kapatma / bloke) yüzünden mi gizlendi?
    // Geri açarken SADECE bunlar geri gelir; kiþinin kendi sildiði bir blog geri dönmez.
    public bool HiddenByAccount { get; set; }

}
