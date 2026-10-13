using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using ZenBlog.Application.Contracts.Infrastructure;

namespace ZenBlog.Infrastructure.Services.EmailService
{
    public class EmailService(IOptions<EmailSettings> options) : IEmailService
    {
        private readonly EmailSettings _settings = options.Value;

        public async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            // 1) E-postayý hazýrla: kimden, kime, konu, içerik
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = htmlBody };

            // 2) Gmail'e baðlan ? giriþ yap ? gönder ? baðlantýyý kapat
            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_settings.SenderEmail, _settings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
