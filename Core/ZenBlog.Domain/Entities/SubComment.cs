using ZenBlog.Domain.Entities.Common;

namespace ZenBlog.Domain.Entities
{
    public class SubComment:BaseEntity
    {

        public virtual Comment Comment { get; set; }
        public Guid CommentId { get; set; }
        public string Body { get; set; }

        public virtual AppUser AppUser { get; set; }
        public Guid AppUserId { get; set; }


        // YENÝ: Bu kayýt, sahibinin hesap iþlemi (kapatma / bloke) yüzünden mi gizlendi?
        // Geri açarken SADECE bunlar geri gelir; kiþinin kendi sildiði bir blog geri dönmez.
        public bool HiddenByAccount { get; set; }

    }
}
