using FluentValidation;
using ZenBlog.Application.Features.Commands.WriterApplicationCommands;

namespace ZenBlog.Application.Validators.WriterApplicationValidators;

public class RejectWriterApplicationValidator : AbstractValidator<RejectWriterApplicationCommand>
{
    public RejectWriterApplicationValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Baþvuru seçilmedi.");

        // Gerekçe kiþiye mail ile gidecek ? yapýcý ve açýklayýcý olsun
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Ret gerekçesi zorunludur.")
            .MinimumLength(20).WithMessage("Gerekçe en az 20 karakter olmalý.")
            .MaximumLength(1000).WithMessage("Gerekçe en fazla 1000 karakter olabilir.");
    }
}