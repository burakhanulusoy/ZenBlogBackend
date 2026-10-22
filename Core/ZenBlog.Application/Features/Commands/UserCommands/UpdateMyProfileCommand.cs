using MediatR;
using Microsoft.AspNetCore.Http;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Results.UserResults;

namespace ZenBlog.Application.Features.Commands.UserCommands;

// Giriþ yapan kiþi KENDÝ profilini günceller (form-data: fotoðraf olabileceði için).
// Id ve Email YOK: kimin güncellendiði token'dan bulunuyor, e-posta deðiþtirilemiyor.
// Cevap: yeni JWT (ad soyad deðiþince token'daki fullName de güncel olsun diye).
public record UpdateMyProfileCommand : IRequest<BaseResult<LoginCommandResult>>
{
    public string? Name { get; init; }
    public string? Surname { get; init; }
    public string? UserName { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Location { get; init; }
    public string? Description { get; init; }
    public string? InstagramUrl { get; init; }
    public string? TwitterUrl { get; init; }
    public string? LinkedlnUrl { get; init; }      // entity'deki yazýmla ayný
    public string? FacebookUrl { get; init; }

    public IFormFile? Image { get; init; }         // yeni profil fotoðrafý (isteðe baðlý)
    public bool RemoveImage { get; init; }         // true ? mevcut fotoðrafý kaldýr

    public string? CurrentPassword { get; init; }  // deðiþiklikleri onaylamak için mevcut þifre
}