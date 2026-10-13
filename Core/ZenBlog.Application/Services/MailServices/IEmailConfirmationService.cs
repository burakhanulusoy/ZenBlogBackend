using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Services.MailServices
{
    public interface IEmailConfirmationService
    {
        Task SendConfirmationEmailAsync(AppUser user);
    }
}
