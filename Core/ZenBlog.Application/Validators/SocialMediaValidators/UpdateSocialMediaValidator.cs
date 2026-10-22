using FluentValidation;
using ZenBlog.Application.Features.Commands.SocialMediaCommands;
using ZenBlog.Application.Validators.FileValidators;

namespace ZenBlog.Application.Validators.SocialMediaValidators;

public class UpdateSocialMediaValidator : AbstractValidator<UpdateSocialMediaCommand>
{
    public UpdateSocialMediaValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id boþ olamaz.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Baþlýk boþ olamaz.")
            .MaximumLength(50).WithMessage("Baþlýk en fazla 50 karakter olabilir.");

        RuleFor(x => x.Url)
            .NotEmpty().WithMessage("Adres boþ olamaz.")
            .MaximumLength(500).WithMessage("Adres en fazla 500 karakter olabilir.")
            .Must(BeValidUrl).WithMessage("Geçerli bir adres gir (https://...).");

        // Güncellemede ikon isteðe baðlý: seçildiyse kurallara uymalý, seçilmediyse eskisi kalýr
        RuleFor(x => x.Icon)
            .SetValidator(new ImageFileValidator()!)
            .When(x => x.Icon is not null);
    }

    private static bool BeValidUrl(string? url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri)
        && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
}