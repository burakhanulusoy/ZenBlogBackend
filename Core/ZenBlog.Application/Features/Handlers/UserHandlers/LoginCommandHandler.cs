using MediatR;
using Microsoft.AspNetCore.Identity;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Application.Features.Commands.UserCommands;
using ZenBlog.Application.Features.Results.UserResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.UserHandlers;

public class LoginCommandHandler(UserManager<AppUser> _userManager,
                                 IJwtService _jwtService)
    : IRequestHandler<LoginCommand, BaseResult<LoginCommandResult>>
{
    // Kullanýcý yoksa da þifre yanlýþsa da AYNI mesaj:
    // farklý mesajlar saldýrgana hangi e-postalarýn kayýtlý olduðunu söyler.
    private const string InvalidCredentialsMessage = "E-posta veya þifre hatalý.";

    public async Task<BaseResult<LoginCommandResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // 1) Kullanýcýyý bul
        var user = await _userManager.FindByEmailAsync(request.Email!);

        if (user is null)
            return BaseResult<LoginCommandResult>.Fail(InvalidCredentialsMessage);

        // 2) Hesap kilitli mi? (çok fazla hatalý deneme yapýldýysa)
        if (await _userManager.IsLockedOutAsync(user))
            return BaseResult<LoginCommandResult>.Fail(
                "Çok fazla hatalý deneme yapýldý. Hesabýnýz geçici olarak kilitlendi, lütfen daha sonra tekrar deneyin.");

        // 3) Þifre doðru mu?
        if (!await _userManager.CheckPasswordAsync(user, request.Password!))
        {
            // CheckPasswordAsync hatalý denemeleri SAYMAZ; sayacý biz artýrýyoruz.
            // Sayaç Identity ayarlarýndaki MaxFailedAccessAttempts'e (5) ulaþýnca
            // hesap DefaultLockoutTimeSpan kadar otomatik kilitlenir.
            await _userManager.AccessFailedAsync(user);

            return BaseResult<LoginCommandResult>.Fail(InvalidCredentialsMessage);
        }

        // 4) E-posta doðrulanmýþ mý?
        //    Bu kontrolü þifreden SONRA yapýyoruz: þifreyi bilmeyen biri,
        //    bu mesaj sayesinde hesabýn var olduðunu öðrenemesin.
        if (!user.EmailConfirmed)
            return BaseResult<LoginCommandResult>.Fail(
                "E-posta adresiniz doðrulanmamýþ. Lütfen e-postanýza gelen kodu girin veya yeni kod isteyin.");

        // 5) Baþarýlý giriþ ? hatalý deneme sayacýný sýfýrla
        await _userManager.ResetAccessFailedCountAsync(user);

        // 6) Token üret
        var token = await _jwtService.GenerateTokenAsync(user);

        return BaseResult<LoginCommandResult>.Success(token);
    }
}