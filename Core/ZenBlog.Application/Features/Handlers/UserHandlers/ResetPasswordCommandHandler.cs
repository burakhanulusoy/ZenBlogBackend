using MediatR;
using Microsoft.AspNetCore.Identity;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Commands.UserCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.UserHandlers;

public class ResetPasswordCommandHandler(UserManager<AppUser> _userManager)
    : IRequestHandler<ResetPasswordCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        // Kullanýcý yoksa da kod yanlýþsa da AYNI mesaj
        const string invalidCodeMessage = "Kod geçersiz veya süresi dolmuþ. Lütfen yeni kod isteyin.";

        var user = await _userManager.FindByEmailAsync(request.Email!);

        if (user is null)
            return BaseResult<object>.Fail("Code", invalidCodeMessage);

        // Identity kodu kontrol eder; doðruysa þifreyi deðiþtirir.
        // Kullanýcýnýn daha önce þifresi olmasa da (Google ile gelen) çalýþýr ? ilk þifresini oluþturmuþ olur.
        var result = await _userManager.ResetPasswordAsync(user, request.Code!, request.NewPassword!);

        if (!result.Succeeded)
        {
            // Kod hatalýysa Identity "InvalidToken" kodu döner ? kendi anlaþýlýr mesajýmýz
            if (result.Errors.Any(e => e.Code == "InvalidToken"))
                return BaseResult<object>.Fail("Code", invalidCodeMessage);

            // Diðer hatalar þifre kurallarýyla ilgili (PasswordTooShort vb.) ? CustomErrorDescriber'ýn Türkçe mesajlarý
            return BaseResult<object>.Fail(result.Errors);
        }

        // Kod e-postaya gittiðine göre kullanýcý bu e-postanýn sahibi; doðrulanmamýþsa doðrulanmýþ say
        if (!user.EmailConfirmed)
        {
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);
        }

        // Þifresini sýfýrlayan kullanýcýnýn kilidi ve hatalý deneme sayacý da sýfýrlansýn
        await _userManager.SetLockoutEndDateAsync(user, null);
        await _userManager.ResetAccessFailedCountAsync(user);

        return BaseResult<object>.Success(new { Message = "Þifren baþarýyla güncellendi." });
    }
}