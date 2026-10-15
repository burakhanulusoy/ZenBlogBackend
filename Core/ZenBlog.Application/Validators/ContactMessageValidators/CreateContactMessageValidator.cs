using FluentValidation;
using ZenBlog.Application.Features.Commands.ContactMessageCommands;

namespace ZenBlog.Application.Validators.ContactMessageValidators;

public class CreateContactMessageValidator : AbstractValidator<CreateContactMessageCommand>
{
    public CreateContactMessageValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ad soyad boþ olamaz.")
            .MaximumLength(100).WithMessage("Ad soyad en fazla 100 karakter olabilir.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta boþ olamaz.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

        RuleFor(x => x.Subject)
            .NotEmpty().WithMessage("Konu boþ olamaz.")
            .MaximumLength(150).WithMessage("Konu en fazla 150 karakter olabilir.");

        RuleFor(x => x.MessageBody)
            .NotEmpty().WithMessage("Mesaj boþ olamaz.")
            .MinimumLength(10).WithMessage("Mesaj en az 10 karakter olmalýdýr.")
            .MaximumLength(2000).WithMessage("Mesaj en fazla 2000 karakter olabilir.");
    }
}