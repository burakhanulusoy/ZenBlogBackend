using FluentValidation;
using ZenBlog.Application.Features.Commands.ContactCommands;

namespace ZenBlog.Application.Validators.ContactValidators;

public class UpdateContactValidator : AbstractValidator<UpdateContactCommand>
{
    public UpdateContactValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Ýletiþim bilgisi Id boþ olamaz.");

        RuleFor(x => x.MapLocationUrl)
            .NotEmpty().WithMessage("Harita konum adresi boþ olamaz.")
            .MaximumLength(2000).WithMessage("Harita konum adresi en fazla 2000 karakter olabilir.")
            .Must(BeAValidUrl).WithMessage("Geçerli bir harita adresi giriniz (http veya https ile baþlamalý).");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Adres boþ olamaz.")
            .MaximumLength(250).WithMessage("Adres en fazla 250 karakter olabilir.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Telefon numarasý boþ olamaz.")
            .Matches(@"^\+?[0-9\s\-()]{10,20}$").WithMessage("Geçerli bir telefon numarasý giriniz.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta boþ olamaz.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");
    }

    private static bool BeAValidUrl(string? url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uri)
               && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}