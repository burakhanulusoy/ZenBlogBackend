using MediatR;
using Microsoft.AspNetCore.Identity;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Application.Features.Commands.UserCommands;
using ZenBlog.Application.Features.Results.UserResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.UserHandlers;

public class UpdateMyProfileCommandHandler(UserManager<AppUser> _userManager,
                                           ICurrentUserService _currentUser,
                                           IFileService _fileService,
                                           IJwtService _jwtService)
    : IRequestHandler<UpdateMyProfileCommand, BaseResult<LoginCommandResult>>
{
    public async Task<BaseResult<LoginCommandResult>> Handle(UpdateMyProfileCommand request, CancellationToken cancellationToken)
    {
        // 1) KÝM? ? token'dan (istekte Id yok, kimse baþkasýný güncelleyemez)
        if (_currentUser.UserId is null)
            return BaseResult<LoginCommandResult>.Fail("Kullanýcý bilgisi alýnamadý. Lütfen tekrar giriþ yapýn.");

        var user = await _userManager.FindByIdAsync(_currentUser.UserId.Value.ToString());
        if (user is null)
            return BaseResult<LoginCommandResult>.Fail("Kullanýcý bulunamadý.");

        // 2) ÞÝFRE ONAYI
        //    Google ile kayýt olanlarýn þifresi olmayabilir ? önce þifre belirlemeleri gerekir
        if (!await _userManager.HasPasswordAsync(user))
            return BaseResult<LoginCommandResult>.Fail("NoPassword",
                "Hesabýnda henüz þifre yok. Önce 'Þifremi unuttum' ile bir þifre belirle.");

        if (await _userManager.IsLockedOutAsync(user))
            return BaseResult<LoginCommandResult>.Fail(
                "Çok fazla hatalý deneme yapýldý. Hesabýn geçici olarak kilitlendi, lütfen daha sonra tekrar dene.");

        if (!await _userManager.CheckPasswordAsync(user, request.CurrentPassword!))
        {
            await _userManager.AccessFailedAsync(user);   // hatalý deneme sayacý (5'te kilit)
            return BaseResult<LoginCommandResult>.Fail("CurrentPassword", "Þifre hatalý.");
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        // 3) FOTOÐRAF
        var oldImage = user.ImageUrl;
        string? uploadedImage = null;

        if (request.Image is not null)
        {
            await using var stream = request.Image.OpenReadStream();
            uploadedImage = await _fileService.UploadAsync(stream, request.Image.FileName, ImageFolders.Users, cancellationToken);
            user.ImageUrl = uploadedImage;
        }
        else if (request.RemoveImage)
        {
            user.ImageUrl = null;
        }

        // 4) BÝLGÝLER (boþ býrakýlan isteðe baðlý alanlar null olarak kaydedilir)
        user.Name = request.Name!.Trim();
        user.Surname = request.Surname!.Trim();
        user.UserName = request.UserName!.Trim();
        user.PhoneNumber = Clean(request.PhoneNumber);
        user.Location = Clean(request.Location);
        user.Description = Clean(request.Description);
        user.InstagramUrl = Clean(request.InstagramUrl);
        user.TwitterUrl = Clean(request.TwitterUrl);
        user.LinkedlnUrl = Clean(request.LinkedlnUrl);
        user.FacebookUrl = Clean(request.FacebookUrl);

        // UpdateAsync: kullanýcý adý kurallarýný ve "bu kullanýcý adý alýnmýþ mý" kontrolünü de yapar
        // (bloke ya da kapatýlmýþ hesaplarýn kullanýcý adlarý da hâlâ veritabanýnda olduðu için onlar da alýnamaz)
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            // Kaydedilemediyse az önce yüklenen fotoðrafý geri sil
            if (uploadedImage is not null)
                await _fileService.DeleteAsync(uploadedImage, cancellationToken);

            return BaseResult<LoginCommandResult>.Fail(result.Errors);   // örn. DuplicateUserName (Türkçe)
        }

        // 5) Fotoðraf deðiþtiyse ya da kaldýrýldýysa eskisini Azure'dan sil
        //    (Google'dan gelen dýþ adresler FileService içinde zaten atlanýyor)
        if (oldImage != user.ImageUrl)
            await _fileService.DeleteAsync(oldImage, cancellationToken);

        // 6) Yeni token: ad soyad deðiþtiyse token'daki fullName de güncellensin
        var token = await _jwtService.GenerateTokenAsync(user);
        return BaseResult<LoginCommandResult>.Success(token);
    }

    // "  " ya da "" ? null, diðerleri kýrpýlmýþ hali
    private static string? Clean(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}