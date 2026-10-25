using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Application.Features.Commands.UserCommands;
using ZenBlog.Application.Features.Results.UserResults;
using ZenBlog.Application.Services.AccountServices;   // YENÝ
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.UserHandlers;

public class LoginCommandHandler(UserManager<AppUser> _userManager,
                                 IJwtService _jwtService,
                                 IAccountLoginGuard _accountLoginGuard,
                                 ILogger<LoginCommandHandler> _logger)   // DEÐÝÞTÝ: logu yazan sýnýf
    : IRequestHandler<LoginCommand, BaseResult<LoginCommandResult>>
{
    private const string InvalidCredentialsMessage = "E-posta veya þifre hatalý.";

    // LOG: Baþarýsýz giriþlerin ortak satýrý. {Reason}: neden baþarýsýz oldu
    // (dashboard'da "baþarýsýz giriþlerin sebeplere göre daðýlýmý" grafiði bu alandan çizilir).
    private void LogLoginFailed(string reason, Guid? userId) =>
        _logger.LogWarning("{EventType}: Giriþ baþarýsýz. Sebep: {Reason}, Kullanýcý: {TargetUserId}",
                           LogEvents.LoginFailed, reason, userId?.ToString() ?? "yok");

    public async Task<BaseResult<LoginCommandResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email!);
        if (user is null)
        {
            // E-postayý LOGLAMIYORUZ (kiþisel veri); sadece "kayýtlý olmayan bir hesaba deneme" bilgisi
            LogLoginFailed("UnknownEmail", null);
            return BaseResult<LoginCommandResult>.Fail(InvalidCredentialsMessage);
        }

        if (await _userManager.IsLockedOutAsync(user))
        {
            // Hesap ZATEN kilitliyken tekrar denendi
            LogLoginFailed("LockedOut", user.Id);
            return BaseResult<LoginCommandResult>.Fail(
                "Çok fazla hatalý deneme yapýldý. Hesabýnýz geçici olarak kilitlendi, lütfen daha sonra tekrar deneyin.");
        }

        if (!await _userManager.CheckPasswordAsync(user, request.Password!))
        {
            await _userManager.AccessFailedAsync(user);
            LogLoginFailed("WrongPassword", user.Id);

            // YENÝ: Bu hatalý deneme hesabý KÝLÝTLEDÝ MÝ? (ör. 5. hatalý deneme)
            // Kilitlenme aný ayrý bir olay: kaba kuvvet saldýrýsý belirtisi olabilir.
            if (await _userManager.IsLockedOutAsync(user))
            {
                _logger.LogWarning("{EventType}: Çok fazla hatalý deneme, hesap kilitlendi. Kullanýcý: {TargetUserId}",
                                   LogEvents.AccountLocked, user.Id);
            }

            return BaseResult<LoginCommandResult>.Fail(InvalidCredentialsMessage);
        }

        // YENÝ: HESAP DURUMU
        // Þifre kontrolünden SONRA bakýyoruz: þifreyi bilmeyen biri "bu e-posta bloke mi"
        // bilgisini öðrenemesin.
        var accountState = await _accountLoginGuard.CheckAsync(user, cancellationToken);

        if (accountState == AccountLoginState.Blocked)
        {
            LogLoginFailed("AccountBlocked", user.Id);
            return BaseResult<LoginCommandResult>.Fail("AccountBlocked",
                "Hesabýnýz bloke edildi. Ayrýntýlar e-posta adresinize gönderildi.");
        }

        if (accountState == AccountLoginState.Closed)
        {
            LogLoginFailed("AccountClosed", user.Id);
            return BaseResult<LoginCommandResult>.Fail("AccountClosed",
                "Bu hesap kapatýldý. Bu e-posta adresiyle tekrar giriþ yapýlamaz.");
        }

        if (!user.EmailConfirmed)
        {
            LogLoginFailed("EmailNotConfirmed", user.Id);
            return BaseResult<LoginCommandResult>.Fail(
                "EmailNotConfirmed",
                "E-posta adresiniz doðrulanmamýþ. Lütfen e-postanýza gelen kodu girin.");
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        var token = await _jwtService.GenerateTokenAsync(user);

        // YENÝ: kapatma iptal edildiyse frontend'e haber ver
        // "with": record'un kopyasýný, sadece bu alan deðiþmiþ haliyle üretir
        if (accountState == AccountLoginState.Reactivated)
            token = token with { AccountReactivated = true };

        _logger.LogInformation("{EventType}: Kullanýcý giriþ yaptý. Kullanýcý: {TargetUserId}",
                               LogEvents.LoginSucceeded, user.Id);

        return BaseResult<LoginCommandResult>.Success(token);
    }
}