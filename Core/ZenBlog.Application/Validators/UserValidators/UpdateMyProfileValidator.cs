using FluentValidation;
using ZenBlog.Application.Features.Commands.UserCommands;
using ZenBlog.Application.Validators.FileValidators;

namespace ZenBlog.Application.Validators.UserValidators;

public class UpdateMyProfileValidator : AbstractValidator<UpdateMyProfileCommand>
{
    public UpdateMyProfileValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ad boþ olamaz.")
            .MaximumLength(50).WithMessage("Ad en fazla 50 karakter olabilir.");

        RuleFor(x => x.Surname)
            .NotEmpty().WithMessage("Soyad boþ olamaz.")
            .MaximumLength(50).WithMessage("Soyad en fazla 50 karakter olabilir.");

        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Kullanýcý adý boþ olamaz.")
            .MaximumLength(50).WithMessage("Kullanýcý adý en fazla 50 karakter olabilir.");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("Telefon en fazla 20 karakter olabilir.");

        RuleFor(x => x.Location)
            .MaximumLength(100).WithMessage("Konum en fazla 100 karakter olabilir.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Hakkýmda en fazla 500 karakter olabilir.");

        // Sosyal medya: boþsa sorun yok, doluysa geçerli bir http(s) adresi olmalý
        RuleFor(x => x.InstagramUrl).Must(BeValidUrl).WithMessage("Geçerli bir adres gir (https://...).");
        RuleFor(x => x.TwitterUrl).Must(BeValidUrl).WithMessage("Geçerli bir adres gir (https://...).");
        RuleFor(x => x.LinkedlnUrl).Must(BeValidUrl).WithMessage("Geçerli bir adres gir (https://...).");
        RuleFor(x => x.FacebookUrl).Must(BeValidUrl).WithMessage("Geçerli bir adres gir (https://...).");

        // Fotoðraf seçildiyse: en fazla 5 MB, jpg/jpeg/png/webp (blog resimleriyle ayný kural)
        RuleFor(x => x.Image)
            .SetValidator(new ImageFileValidator()!)
            .When(x => x.Image is not null);

        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Deðiþiklikleri onaylamak için þifreni gir.");
    }

    private static bool BeValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return true;

        return Uri.TryCreate(url, UriKind.Absolute, out var uri)
               && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}