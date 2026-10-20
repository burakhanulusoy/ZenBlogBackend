using FluentValidation;
using ZenBlog.Application.Features.Commands.UserCommands;

namespace ZenBlog.Application.Validators.UserValidators;

public class GoogleLoginValidator : AbstractValidator<GoogleLoginCommand>
{
    public GoogleLoginValidator()
    {
        RuleFor(x => x.IdToken)
            .NotEmpty().WithMessage("Google kimlik bilgisi alýnamadý.");
    }
}