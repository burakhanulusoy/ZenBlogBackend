using FluentValidation;
using ZenBlog.Application.Features.Commands.ContactCommands;

namespace ZenBlog.Application.Validators.ContactValidators;

public class UpdateContactValidator : AbstractValidator<UpdateContactCommand>
{
    public UpdateContactValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Ýletiþim bilgisi Id boþ olamaz.");

        // DEÐÝÞTÝ: Sadece Google Haritalar "yerleþtir" adresi kabul ediliyor.
        // Bu adres sitede <iframe> içinde gösteriliyor; rastgele bir adres kabul edilirse
        // sitemizin içine baþka bir sitenin sayfasý yerleþtirilebilir.
        RuleFor(x => x.MapLocationUrl)
            .NotEmpty().WithMessage("Harita adresi boþ olamaz.")
            .MaximumLength(2000).WithMessage("Harita adresi en fazla 2000 karakter olabilir.")
            .Must(BeGoogleMapsEmbed).WithMessage("Google Haritalar'daki 'Haritayý yerleþtir' adresini gir (https://www.google.com/maps/embed?... ile baþlamalý).");
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

    private static bool BeGoogleMapsEmbed(string? url) =>
      url is not null && url.StartsWith("https://www.google.com/maps/embed", StringComparison.OrdinalIgnoreCase);
}