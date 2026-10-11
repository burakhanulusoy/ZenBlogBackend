using ZenBlog.Domain.Entities.Common;
using ZenBlog.Domain.Enums;

namespace ZenBlog.Domain.Entities;

public class ContactMessage : BaseEntity
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Subject { get; set; }
    public string MessageBody { get; set; }
    public ContactMessageStatus  ContactMessageStatus { get; set; }
}
