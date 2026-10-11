using ZenBlog.Domain.Entities.Common;

namespace ZenBlog.Domain.Entities
{
    public class About : BaseEntity
    {
        public string MainTitle { get; set; }
        public string Description { get; set; }
        public string Title1 { get; set; }
        public string Title2 { get; set; }
        public string Title3 { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string Image3 { get; set; }

    }
}
