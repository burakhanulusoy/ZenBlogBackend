using FluentValidation;
using ZenBlog.Application.Features.Commands.AboutCommands;
using ZenBlog.Application.Validators.FileValidators;

namespace ZenBlog.Application.Validators.AboutValidators;

public class UpdateAboutValidator : AbstractValidator<UpdateAboutCommand>
{
    public UpdateAboutValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Hakkýmýzda Id boþ olamaz.");

        RuleFor(x => x.MainTitle)
            .NotEmpty().WithMessage("Ana baþlýk boþ olamaz.")
            .MaximumLength(150).WithMessage("Ana baþlýk en fazla 150 karakter olabilir.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Açýklama boþ olamaz.");

        RuleFor(x => x.Title1)
            .NotEmpty().WithMessage("1. baþlýk boþ olamaz.")
            .MaximumLength(150).WithMessage("1. baþlýk en fazla 150 karakter olabilir.");

        RuleFor(x => x.Title2)
            .NotEmpty().WithMessage("2. baþlýk boþ olamaz.")
            .MaximumLength(150).WithMessage("2. baþlýk en fazla 150 karakter olabilir.");

        RuleFor(x => x.Title3)
            .NotEmpty().WithMessage("3. baþlýk boþ olamaz.")
            .MaximumLength(150).WithMessage("3. baþlýk en fazla 150 karakter olabilir.");

        // Güncellemede resimler zorunlu deðil; seçildiyse kurallara uymalý
        RuleFor(x => x.Image1).SetValidator(new ImageFileValidator()!).When(x => x.Image1 is not null);
        RuleFor(x => x.Image2).SetValidator(new ImageFileValidator()!).When(x => x.Image2 is not null);
        RuleFor(x => x.Image3).SetValidator(new ImageFileValidator()!).When(x => x.Image3 is not null);
    }
}