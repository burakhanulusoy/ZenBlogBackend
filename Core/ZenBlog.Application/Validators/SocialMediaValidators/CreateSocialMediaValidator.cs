using FluentValidation;
using ZenBlog.Application.Features.Commands.SocialMediaCommands;
using ZenBlog.Application.Validators.FileValidators;

namespace ZenBlog.Application.Validators.SocialMediaValidators;

public class CreateSocialMediaValidator : AbstractValidator<CreateSocialMediaCommand>
{
    public CreateSocialMediaValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Baþlýk boþ olamaz.")
            .MaximumLength(50).WithMessage("Baþlýk en fazla 50 karakter olabilir.");

        RuleFor(x => x.Url)
            .NotEmpty().WithMessage("Adres boþ olamaz.")
            .MaximumLength(500).WithMessage("Adres en fazla 500 karakter olabilir.")
            .Must(BeValidUrl).WithMessage("Geçerli bir adres gir (https://...).");

        // Önce "seçildi mi", sonra "kurallara uyuyor mu" (en fazla 5 MB, jpg/jpeg/png/webp)
        RuleFor(x => x.Icon)
            .NotNull().WithMessage("Ýkon yüklemek zorunludur.")
            .SetValidator(new ImageFileValidator()!);
    }

    private static bool BeValidUrl(string? url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri)
        && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
}