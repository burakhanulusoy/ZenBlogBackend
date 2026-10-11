using ZenBlog.Domain.Entities.Common;

namespace ZenBlog.Domain.Entities; //file scoped

public class Contact:BaseEntity
{
    public string MapLocationUrl { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }


}
