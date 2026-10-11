using ZenBlog.Domain.Entities.Common;

namespace ZenBlog.Domain.Entities;

public class Category :BaseEntity
{
    public string Name { get; set; }

    public IList<Blog> Blogs { get; set; }


}
