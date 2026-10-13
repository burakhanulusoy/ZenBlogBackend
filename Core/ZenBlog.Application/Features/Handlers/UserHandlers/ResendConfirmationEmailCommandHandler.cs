using MediatR;
using Microsoft.AspNetCore.Identity;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Commands.UserCommands;
using ZenBlog.Application.Services;
using ZenBlog.Application.Services.MailServices;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.UserHandlers;

public class ResendConfirmationEmailCommandHandler(UserManager<AppUser> _userManager,
                                                   IEmailConfirmationService _emailConfirmationService)
    : IRequestHandler<ResendConfirmationEmailCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(ResendConfirmationEmailCommand request, CancellationToken cancellationToken)
    {
        // Güvenlik: e-posta kayýtlý olsa da olmasa da HEP AYNI mesajý dönüyoruz.
        // Farklý mesaj dönersek biri deneme yaparak hangi e-postalarýn sistemde olduðunu öðrenebilir.
        const string message = "E-posta adresiniz kayýtlý ve doðrulanmamýþsa yeni baðlantý gönderildi.";

        var user = await _userManager.FindByEmailAsync(request.Email);

        // Sadece kullanýcý varsa VE henüz doðrulanmamýþsa e-posta gönder
        if (user is not null && !user.EmailConfirmed)
            await _emailConfirmationService.SendConfirmationEmailAsync(user);

        return BaseResult<object>.Success(new { Message = message });
    }
}