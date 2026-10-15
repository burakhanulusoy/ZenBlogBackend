using ZenBlog.Domain.Entities.Common;

namespace ZenBlog.Domain.Entities;

public class Comment:BaseEntity
{
    public string Body { get; set; }
    public virtual IList<SubComment> SubComments { get; set; }
    public virtual AppUser AppUser { get; set; }
    public Guid AppUserId { get; set; }
    public virtual Blog Blog { get; set; }
    public Guid BlogId { get; set; }


}
