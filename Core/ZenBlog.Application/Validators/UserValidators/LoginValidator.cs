using FluentValidation;
using ZenBlog.Application.Features.Commands.UserCommands;

namespace ZenBlog.Application.Validators.UserValidators;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta boþ olamaz.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

        // Þifre kurallarýný (uzunluk vb.) burada kontrol ETMÝYORUZ:
        // login'de amaç kural denetlemek deðil, girilen þifrenin doðru olup olmadýðýna bakmak.
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Þifre boþ olamaz.");
    }
}