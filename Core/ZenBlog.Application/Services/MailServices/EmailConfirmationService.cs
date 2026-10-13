using Microsoft.AspNetCore.Identity;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Services.MailServices;

public class EmailConfirmationService(UserManager<AppUser> _userManager,
                                      IEmailService _emailService) : IEmailConfirmationService
{
    public async Task SendConfirmationEmailAsync(AppUser user)
    {
        // Identity 6 haneli bir kod üretir (Adým 1'deki ayar sayesinde).
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var body = $"""
        <h3>Merhaba {user.Name},</h3>
        <p>ZenBlog hesabýný doðrulamak için aþaðýdaki kodu kullan:</p>
        <h2 style="letter-spacing: 6px;">{code}</h2>
        <p>Bu kod birkaç dakika içinde geçerliliðini yitirir.</p>
        <p>Bu iþlemi sen yapmadýysan bu e-postayý görmezden gelebilirsin.</p>
        """;

        await _emailService.SendEmailAsync(user.Email!, "ZenBlog - Doðrulama Kodun", body);
    }
}