using FluentValidation;
using ZenBlog.Application.Features.Commands.UserCommands;

namespace ZenBlog.Application.Validators.UserValidators;

public class CloseMyAccountValidator : AbstractValidator<CloseMyAccountCommand>
{
    public CloseMyAccountValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Hesabýný kapatmak için þifreni gir.");
    }
}