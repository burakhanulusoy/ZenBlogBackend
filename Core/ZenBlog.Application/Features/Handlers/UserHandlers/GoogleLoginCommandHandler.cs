using System.Text.RegularExpressions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Application.Features.Commands.UserCommands;
using ZenBlog.Application.Features.Results.UserResults;
using ZenBlog.Application.Services.AccountServices;   // YENÝ
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.UserHandlers;

public class GoogleLoginCommandHandler(UserManager<AppUser> _userManager,
                                       IGoogleAuthService _googleAuthService,
                                       IJwtService _jwtService,
                                       IAccountLoginGuard _accountLoginGuard)   // YENÝ
    : IRequestHandler<GoogleLoginCommand, BaseResult<LoginCommandResult>>
{
    private const string GoogleProvider = "Google";

    public async Task<BaseResult<LoginCommandResult>> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
    {
        // 1) Google token'ý gerçekten Google'dan mý, bizim uygulamamýz için mi?
        var googleUser = await _googleAuthService.ValidateIdTokenAsync(request.IdToken!);
        if (googleUser is null)
            return BaseResult<LoginCommandResult>.Fail("Google ile giriþ doðrulanamadý. Lütfen tekrar deneyin.");

        if (!googleUser.EmailVerified)
            return BaseResult<LoginCommandResult>.Fail("Google hesabýnýzýn e-posta adresi doðrulanmamýþ.");

        // YENÝ: kapatma iptal edildi mi? (cevapta frontend'e bildirmek için)
        var accountState = AccountLoginState.Allowed;

        // 2) Bu e-postaya ait kullanýcý var mý?
        var user = await _userManager.FindByEmailAsync(googleUser.Email);

        if (user is null)
        {
            // 3a) YOK ? yeni kullanýcý oluþtur
            user = new AppUser
            {
                UserName = await GenerateUniqueUserNameAsync(googleUser.Email),
                Email = googleUser.Email,
                Name = googleUser.FirstName ?? string.Empty,
                Surname = googleUser.LastName ?? string.Empty,
                ImageUrl = googleUser.PictureUrl,
                EmailConfirmed = true   // Google bu e-postayý zaten doðruladý; kod göndermeye gerek yok
            };

            // Þifresiz oluþturuyoruz: bu kullanýcý Google ile giriþ yapacak
            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
                return BaseResult<LoginCommandResult>.Fail(createResult.Errors);

            // Normal kayýttaki gibi varsayýlan rol
            await _userManager.AddToRoleAsync(user, Roles.User);
        }
        else
        {
            // 3b) VAR ? hesap kilitli mi?
            if (await _userManager.IsLockedOutAsync(user))
                return BaseResult<LoginCommandResult>.Fail(
                    "Hesabýnýz geçici olarak kilitlendi, lütfen daha sonra tekrar deneyin.");

            // YENÝ: HESAP DURUMU (normal giriþle ayný kural)
            // Bloke / kapanmýþ hesaba Google ile de girilemez; ayný e-postayla yeni hesap da açýlamaz
            // çünkü kullanýcý satýrý duruyor (FindByEmailAsync onu buluyor).
            accountState = await _accountLoginGuard.CheckAsync(user, cancellationToken);

            if (accountState == AccountLoginState.Blocked)
                return BaseResult<LoginCommandResult>.Fail("AccountBlocked",
                    "Hesabýnýz bloke edildi. Ayrýntýlar e-posta adresinize gönderildi.");

            if (accountState == AccountLoginState.Closed)
                return BaseResult<LoginCommandResult>.Fail("AccountClosed",
                    "Bu hesap kapatýldý. Bu e-posta adresiyle tekrar giriþ yapýlamaz.");

            // GÜVENLÝK: Hesap var ama e-postasý hiç doðrulanmamýþsa, o hesabý ve þifresini
            // e-postanýn sahibi olmayan biri açmýþ olabilir (baþkasýnýn e-postasýyla kayýt olup býrakmýþ).
            // Google, e-postanýn gerçek sahibinin bu kiþi olduðunu kanýtladý; eski þifreyi kaldýrýyoruz ki
            // hesabý açan kiþi o þifreyle bir daha giremesin.
            if (!user.EmailConfirmed)
            {
                if (await _userManager.HasPasswordAsync(user))
                    await _userManager.RemovePasswordAsync(user);

                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
            }
        }

        // 4) Hesabý Google hesabýyla iliþkilendir (AspNetUserLogins tablosu).
        //    Ýleride "bu kullanýcý Google ile mi giriþ yapýyor?" sorusunun cevabý buradan gelir.
        var logins = await _userManager.GetLoginsAsync(user);
        if (!logins.Any(l => l.LoginProvider == GoogleProvider))
            await _userManager.AddLoginAsync(user, new UserLoginInfo(GoogleProvider, googleUser.Subject, GoogleProvider));

        await _userManager.ResetAccessFailedCountAsync(user);

        // 5) Normal login'deki gibi BÝZÝM JWT'mizi üret
        var token = await _jwtService.GenerateTokenAsync(user);

        // YENÝ: kapatma iptal edildiyse frontend'e haber ver
        if (accountState == AccountLoginState.Reactivated)
            token = token with { AccountReactivated = true };

        return BaseResult<LoginCommandResult>.Success(token);
    }

    // E-postanýn @ öncesinden kullanýcý adý üretir; alýnmýþsa sonuna rakam ekler.
    // ali.veli@gmail.com ? "ali.veli", alýnmýþsa "ali.veli4821"
    private async Task<string> GenerateUniqueUserNameAsync(string email)
    {
        // Identity'nin kabul ettiði karakterler dýþýndakileri temizle
        var baseName = Regex.Replace(email.Split('@')[0], @"[^a-zA-Z0-9._-]", "");
        if (string.IsNullOrEmpty(baseName))
            baseName = "user";

        var userName = baseName;
        while (await _userManager.FindByNameAsync(userName) is not null)
            userName = $"{baseName}{Random.Shared.Next(1000, 9999)}";

        return userName;
    }
}