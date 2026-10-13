namespace ZenBlog.Application.Contracts.Infrastructure;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string htmlBody);
}