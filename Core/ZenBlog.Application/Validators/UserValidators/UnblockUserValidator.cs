using FluentValidation;
using ZenBlog.Application.Features.Commands.UserCommands;

namespace ZenBlog.Application.Validators.UserValidators;

public class UnblockUserValidator : AbstractValidator<UnblockUserCommand>
{
    public UnblockUserValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Kullanýcý seçilmedi.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Blokeyi kaldýrma gerekçesi zorunludur.")
            .MinimumLength(10).WithMessage("Gerekçe en az 10 karakter olmalý.")
            .MaximumLength(1000).WithMessage("Gerekçe en fazla 1000 karakter olabilir.");
    }
}