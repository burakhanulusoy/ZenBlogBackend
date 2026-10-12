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

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Þifre boþ olamaz.");

            // Identity þifre tekrarýný kontrol ETMEZ, bu yüzden burada kalmalý.
            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Þifreler eþleþmiyor.");
        }
    }
}