using FluentValidation;
using ZenBlog.Application.Features.Commands.UserCommands;
using ZenBlog.Application.Validators.FileValidators;

namespace ZenBlog.Application.Validators.UserValidators;

public class BlockUserValidator : AbstractValidator<BlockUserCommand>
{
    public BlockUserValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Kullanıcı seçilmedi.");

        // Gerekçe kullanıcıya mail ile gidecek → anlaşılır ve yeterince açıklayıcı olsun
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Bloke gerekçesi zorunludur.")
            .MinimumLength(20).WithMessage("Gerekçe en az 20 karakter olmalı; neden bloke edildiğini açıkça yaz.")
            .MaximumLength(1000).WithMessage("Gerekçe en fazla 1000 karakter olabilir.");

        RuleFor(x => x.Evidence)
            .NotNull().WithMessage("Kanıt dosyası yüklemek zorunludur.")
            .SetValidator(new EvidenceFileValidator()!);
    }
}