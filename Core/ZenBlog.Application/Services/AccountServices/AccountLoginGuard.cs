using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;   // YENÝ
using ZenBlog.Application.Constants;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Domain.Entities;
using ZenBlog.Domain.Enums;

namespace ZenBlog.Application.Services.AccountServices;

public class AccountLoginGuard(UserManager<AppUser> _userManager,
                               IAccountContentService _accountContentService,
                               ILogger<AccountLoginGuard> _logger) : IAccountLoginGuard   // YENÝ
{
    public async Task<AccountLoginState> CheckAsync(AppUser user, CancellationToken cancellationToken)
    {
        switch (user.AccountStatus)
        {
            case AccountStatus.Active:
                return AccountLoginState.Allowed;

            case AccountStatus.Blocked:
                return AccountLoginState.Blocked;

            case AccountStatus.PendingDeletion:
                // 30 gün doldu mu? Zamanlayýcýya gerek yok: giriþ anýnda tarihe bakmak yeterli.
                var deadline = user.DeletionRequestedAt?.AddDays(AccountRules.DeletionGraceDays);
                if (deadline is null || DateTime.Now >= deadline)
                    return AccountLoginState.Closed;

                // Süre dolmadan geri döndü -> kapatmayý iptal et
                user.AccountStatus = AccountStatus.Active;
                user.DeletionRequestedAt = null;
                user.DeletionReminderSentAt = null;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                    throw new InvalidOperationException("Hesap yeniden etkinleþtirilemedi.");

                // Önce durum Active yapýldý, SONRA içerikler geri açýlýyor (sýra önemli, bkz. arayüz)
                await _accountContentService.RestoreContentAsync(user.Id, cancellationToken);

                // LOG: YENÝ
                _logger.LogInformation("{EventType}: Kullanýcý geri döndü, hesap kapatma iptal edildi. Kullanýcý: {TargetUserId}",
                                       LogEvents.AccountReactivated, user.Id);

                return AccountLoginState.Reactivated;

            default:
                return AccountLoginState.Blocked;
        }
    }
}