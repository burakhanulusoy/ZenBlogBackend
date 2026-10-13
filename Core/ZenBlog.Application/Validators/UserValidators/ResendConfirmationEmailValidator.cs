using FluentValidation;
using ZenBlog.Application.Features.Commands.UserCommands;

namespace ZenBlog.Application.Validators.UserValidators;

public class ResendConfirmationEmailValidator : AbstractValidator<ResendConfirmationEmailCommand>
{
    public ResendConfirmationEmailValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta boþ olamaz.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");
    }
}