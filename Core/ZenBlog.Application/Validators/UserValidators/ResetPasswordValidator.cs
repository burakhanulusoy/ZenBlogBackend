using FluentValidation;
using ZenBlog.Application.Features.Commands.UserCommands;

namespace ZenBlog.Application.Validators.UserValidators;

public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta boþ olamaz.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Kod boþ olamaz.")
            .Matches(@"^\d{6}$").WithMessage("Kod 6 haneli bir sayý olmalýdýr.");

        // Þifre kurallarýný (uzunluk, büyük harf vb.) Identity kontrol ediyor; burada sadece boþ olmasýn
        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Yeni þifre boþ olamaz.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.NewPassword).WithMessage("Þifreler eþleþmiyor.");
    }
}