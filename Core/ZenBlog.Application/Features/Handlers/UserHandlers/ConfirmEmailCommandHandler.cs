using MediatR;
using Microsoft.AspNetCore.Identity;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Commands.UserCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.UserHandlers;

public class ConfirmEmailCommandHandler(UserManager<AppUser> _userManager)
    : IRequestHandler<ConfirmEmailCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        // Kullanýcý bulunamazsa da, kod yanlýþsa da AYNI mesajý dönüyoruz.
        // "Kullanýcý bulunamadý" desek biri bu endpoint'ten kayýtlý e-postalarý öðrenebilir.
        const string invalidMessage = "Kod geçersiz veya süresi dolmuþ. Lütfen yeni kod isteyin.";

        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
            return BaseResult<object>.Fail(invalidMessage);

        if (user.EmailConfirmed)
            return BaseResult<object>.Success(new { Message = "E-posta adresiniz zaten doðrulanmýþ." });

        // Identity kodu kontrol eder; doðruysa EmailConfirmed'ý kendisi true yapar.
        var result = await _userManager.ConfirmEmailAsync(user, request.Code);

        return result.Succeeded
            ? BaseResult<object>.Success(new { Message = "E-posta adresiniz baþarýyla doðrulandý." })
            : BaseResult<object>.Fail(invalidMessage);
    }
}