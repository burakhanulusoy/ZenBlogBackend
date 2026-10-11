using ZenBlog.Domain.Entities.Common;

namespace ZenBlog.Domain.Entities
{
    public class Team:BaseEntity
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Descriptiron { get; set; }
        public string Image { get; set; }
        public IList<TeamSocialMedia> TeamSocialMedias { get; set; }
    }
}
