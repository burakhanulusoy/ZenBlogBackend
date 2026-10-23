using FluentValidation;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Features.Commands.TeamCommands;

namespace ZenBlog.Application.Validators.TeamValidators;

public class UpdateTeamSocialMediaValidator : AbstractValidator<UpdateTeamSocialMediaCommand>
{
    public UpdateTeamSocialMediaValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id boþ olamaz.");

        RuleFor(x => x.SocialMediaName)
            .Must(TeamSocialPlatforms.IsValid).WithMessage("Listeden geçerli bir platform seç.");

        RuleFor(x => x.Url)
            .NotEmpty().WithMessage("Adres boþ olamaz.")
            .MaximumLength(500).WithMessage("Adres en fazla 500 karakter olabilir.")
            .Must(BeValidUrl).WithMessage("Geçerli bir adres gir (https://...).");
    }

    private static bool BeValidUrl(string? url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri)
        && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
}