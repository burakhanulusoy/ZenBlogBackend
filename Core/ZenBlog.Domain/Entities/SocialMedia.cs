using ZenBlog.Domain.Entities.Common;

namespace ZenBlog.Domain.Entities;

public class SocialMedia : BaseEntity
{
    public string Icon { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
}
