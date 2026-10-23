using ZenBlog.Domain.Entities.Common;

namespace ZenBlog.Domain.Entities
{
    // Ekip üyesi (sitede Hakkýmýzda sayfasýnda görünür)
    public class Team : BaseEntity
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Title { get; set; }         
        public string Description { get; set; } 
        public string Image { get; set; }

        public virtual IList<TeamSocialMedia> TeamSocialMedias { get; set; }
    }
}