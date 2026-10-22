using MediatR;
using Microsoft.AspNetCore.Identity;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.UserCommands;
using ZenBlog.Application.Features.Results.UserResults;
using ZenBlog.Application.Services.MailServices;
using ZenBlog.Domain.Entities;
using ZenBlog.Domain.Enums;

namespace ZenBlog.Application.Features.Handlers.UserHandlers;

public class CloseMyAccountCommandHandler(UserManager<AppUser> _userManager,
                                          ICurrentUserService _currentUser,
                                          IAccountContentService _accountContentService,
                                          IAccountEmailService _accountEmailService)
    : IRequestHandler<CloseMyAccountCommand, BaseResult<CloseMyAccountResult>>
{
    public async Task<BaseResult<CloseMyAccountResult>> Handle(CloseMyAccountCommand request, CancellationToken cancellationToken)
    {
        // 1) KÝM? ? token'dan
        if (_currentUser.UserId is null)
            return BaseResult<CloseMyAccountResult>.Fail("Kullanýcý bilgisi alýnamadý. Lütfen tekrar giriþ yapýn.");

        var user = await _userManager.FindByIdAsync(_currentUser.UserId.Value.ToString());
        if (user is null)
            return BaseResult<CloseMyAccountResult>.Fail("Kullanýcý bulunamadý.");

        // 2) Admin hesabýný buradan kapatamaz (panelin sahipsiz kalmamasý için)
        if (await _userManager.IsInRoleAsync(user, Roles.Admin))
            return BaseResult<CloseMyAccountResult>.Fail("Yönetici hesaplarý bu yolla kapatýlamaz.");

        // 3) ÞÝFRE ONAYI (profil güncellemedeki kurallarýn aynýsý)
        if (!await _userManager.HasPasswordAsync(user))
            return BaseResult<CloseMyAccountResult>.Fail("NoPassword",
                "Hesabýnda henüz þifre yok. Önce 'Þifremi unuttum' ile bir þifre belirle.");

        if (await _userManager.IsLockedOutAsync(user))
            return BaseResult<CloseMyAccountResult>.Fail(
                "Çok fazla hatalý deneme yapýldý. Hesabýn geçici olarak kilitlendi, lütfen daha sonra tekrar dene.");

        if (!await _userManager.CheckPasswordAsync(user, request.CurrentPassword!))
        {
            await _userManager.AccessFailedAsync(user);
            return BaseResult<CloseMyAccountResult>.Fail("CurrentPassword", "Þifre hatalý.");
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        // 4) Durumu "Kapatýlýyor" yap
        var now = DateTime.Now;
        user.AccountStatus = AccountStatus.PendingDeletion;
        user.DeletionRequestedAt = now;
        user.DeletionReminderSentAt = null;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BaseResult<CloseMyAccountResult>.Fail(result.Errors);

        // 5) Ýçerikleri gizle (blog, yorum, alt yorum ? IsDeleted = 1)
        await _accountContentService.HideContentAsync(user.Id, cancellationToken);

        // 6) Bilgilendirme maili. Mail gönderilemese bile hesap kapatma iþlemi geçerli kalsýn.
        var deadline = now.AddDays(AccountRules.DeletionGraceDays);
        try
        {
            await _accountEmailService.SendClosureScheduledAsync(user, deadline);
        }
        catch
        {
            // Mail sunucusu geçici hata verdiyse iþlemi bozmayalým
        }

        // Not: Bu noktadan sonra kiþinin elindeki token geçersiz (OnTokenValidated hesabý aktif görmüyor);
        // frontend kiþiyi çýkýþa yönlendirecek.
        return BaseResult<CloseMyAccountResult>.Success(new CloseMyAccountResult(deadline));
    }
}