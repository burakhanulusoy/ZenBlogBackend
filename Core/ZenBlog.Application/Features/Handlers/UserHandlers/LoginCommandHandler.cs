using MediatR;
using Microsoft.AspNetCore.Identity;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Application.Features.Commands.UserCommands;
using ZenBlog.Application.Features.Results.UserResults;
using ZenBlog.Application.Services.AccountServices;   // YENÝ
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.UserHandlers;

public class LoginCommandHandler(UserManager<AppUser> _userManager,
                                 IJwtService _jwtService,
                                 IAccountLoginGuard _accountLoginGuard)   // YENÝ
    : IRequestHandler<LoginCommand, BaseResult<LoginCommandResult>>
{
    private const string InvalidCredentialsMessage = "E-posta veya þifre hatalý.";

    public async Task<BaseResult<LoginCommandResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email!);
        if (user is null)
            return BaseResult<LoginCommandResult>.Fail(InvalidCredentialsMessage);

        if (await _userManager.IsLockedOutAsync(user))
            return BaseResult<LoginCommandResult>.Fail(
                "Çok fazla hatalý deneme yapýldý. Hesabýnýz geçici olarak kilitlendi, lütfen daha sonra tekrar deneyin.");

        if (!await _userManager.CheckPasswordAsync(user, request.Password!))
        {
            await _userManager.AccessFailedAsync(user);
            return BaseResult<LoginCommandResult>.Fail(InvalidCredentialsMessage);
        }

        // YENÝ: HESAP DURUMU
        // Þifre kontrolünden SONRA bakýyoruz: þifreyi bilmeyen biri "bu e-posta bloke mi"
        // bilgisini öðrenemesin.
        var accountState = await _accountLoginGuard.CheckAsync(user, cancellationToken);

        if (accountState == AccountLoginState.Blocked)
            return BaseResult<LoginCommandResult>.Fail("AccountBlocked",
                "Hesabýnýz bloke edildi. Ayrýntýlar e-posta adresinize gönderildi.");

        if (accountState == AccountLoginState.Closed)
            return BaseResult<LoginCommandResult>.Fail("AccountClosed",
                "Bu hesap kapatýldý. Bu e-posta adresiyle tekrar giriþ yapýlamaz.");

        if (!user.EmailConfirmed)
            return BaseResult<LoginCommandResult>.Fail(
                "EmailNotConfirmed",
                "E-posta adresiniz doðrulanmamýþ. Lütfen e-postanýza gelen kodu girin.");

        await _userManager.ResetAccessFailedCountAsync(user);

        var token = await _jwtService.GenerateTokenAsync(user);

        // YENÝ: kapatma iptal edildiyse frontend'e haber ver
        // "with": record'un kopyasýný, sadece bu alan deðiþmiþ haliyle üretir
        if (accountState == AccountLoginState.Reactivated)
            token = token with { AccountReactivated = true };

        return BaseResult<LoginCommandResult>.Success(token);
    }
}