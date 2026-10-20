using MediatR;
using Microsoft.AspNetCore.Identity;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Application.Features.Commands.UserCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.UserHandlers;

public class ForgotPasswordCommandHandler(UserManager<AppUser> _userManager,
                                          IEmailService _emailService)
    : IRequestHandler<ForgotPasswordCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        // Güvenlik: e-posta kayýtlý olsa da olmasa da AYNI mesaj (kullanýcý tespitini önlemek için;
        // resend-confirmation'daki yaklaþýmýn aynýsý)
        const string message = "E-posta adresin kayýtlýysa þifre sýfýrlama kodu gönderildi.";

        var user = await _userManager.FindByEmailAsync(request.Email!);

        if (user is not null)
        {
            // Adým 1'deki ayar sayesinde 6 haneli kod üretilir
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            var body = $"""
                <h3>Merhaba {user.Name},</h3>
                <p>ZenBlog hesabýn için þifre sýfýrlama talebi aldýk. Yeni þifreni belirlemek için aþaðýdaki kodu kullan:</p>
                <h2 style="letter-spacing: 6px;">{code}</h2>
                <p>Bu kod birkaç dakika içinde geçerliliðini yitirir.</p>
                <p>Bu talebi sen yapmadýysan bu e-postayý görmezden gelebilirsin; þifren deðiþmeyecek.</p>
                """;

            await _emailService.SendEmailAsync(user.Email!, "ZenBlog - Þifre Sýfýrlama Kodun", body);
        }

        return BaseResult<object>.Success(new { Message = message });
    }
}