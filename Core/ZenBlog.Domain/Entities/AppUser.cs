using Microsoft.AspNetCore.Identity;

namespace ZenBlog.Domain.Entities
{
    public class AppUser:IdentityUser<Guid>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? ImageUrl { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public string? InstagramUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? LinkedlnUrl { get; set; }
        public string? FacebookUrl { get; set; }


        public virtual IList<Comment> Comments { get; set; }
        public virtual IList<SubComment> SubComments { get; set; }
        public virtual IList<Blog> Blogs { get; set; }


    }
}
