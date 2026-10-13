using FluentValidation;
using ZenBlog.Application.Features.Commands.UserCommands;

namespace ZenBlog.Application.Validators.UserValidators
{
    public class CreateUserValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.Name)
             .NotEmpty().WithMessage("Ad boþ olamaz.")
             .MaximumLength(50).WithMessage("Ad en fazla 50 karakter olabilir.");

            RuleFor(x => x.Surname)
                .NotEmpty().WithMessage("Soyad boþ olamaz.")
                .MaximumLength(50).WithMessage("Soyad en fazla 50 karakter olabilir.");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Kullanýcý adý boþ olamaz.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-posta boþ olamaz.")
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

            // Boþ þifre gelirse Identity düzgün hata yerine exception fýrlatýr, o yüzden burada.
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Þifre boþ olamaz.");

            // Identity'de "þifre tekrarý" kavramý yok, bu kontrol bize kalýyor.
            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Þifreler eþleþmiyor.");
        }
    }
}