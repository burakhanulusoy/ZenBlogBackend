using FluentValidation;
using ZenBlog.Application.Features.Commands.UserCommands;

namespace ZenBlog.Application.Validators.UserValidators;

public class ConfirmEmailValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta boþ olamaz.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Doðrulama kodu boþ olamaz.")
            .Matches(@"^\d{6}$").WithMessage("Doðrulama kodu 6 haneli bir sayý olmalýdýr.");
    }
}