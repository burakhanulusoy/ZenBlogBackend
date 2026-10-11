using ZenBlog.Domain.Entities.Common;

namespace ZenBlog.Domain.Entities;

public class Blog:BaseEntity
{


    public string MainTitle  { get; set; }
    public string CoverImage { get; set; }
    public string   BlogImage { get; set; }
    public string   Description1 { get; set; }
    public string   MainDescription { get; set; }
    public string Description2 { get; set; }
    public string Title3 { get; set; }
    public string Description3 { get; set; }
    public string Title4 { get; set; }
    public string Description4 { get; set; }

    public Category Category { get; set; }
    public Guid CategoryId { get; set; }


}
