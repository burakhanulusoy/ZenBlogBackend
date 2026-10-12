using ZenBlog.Domain.Entities.Common;

namespace ZenBlog.Domain.Entities
{
    public class TeamSocialMedia:BaseEntity
    {
        public string SocialMediaName { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        public virtual Team Team { get; set; }
        public Guid TeamId { get; set; }

    }
}
