using FluentValidation;
using ZenBlog.Application.Features.Commands.AboutCommands;
using ZenBlog.Application.Validators.FileValidators;

namespace ZenBlog.Application.Validators.AboutValidators;

public class CreateAboutValidator : AbstractValidator<CreateAboutCommand>
{
    public CreateAboutValidator()
    {
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

        // Oluþtururken üç resim de zorunlu
        RuleFor(x => x.Image1)
            .NotNull().WithMessage("1. görsel seçilmelidir.")
            .SetValidator(new ImageFileValidator()!);

        RuleFor(x => x.Image2)
            .NotNull().WithMessage("2. görsel seçilmelidir.")
            .SetValidator(new ImageFileValidator()!);

        RuleFor(x => x.Image3)
            .NotNull().WithMessage("3. görsel seçilmelidir.")
            .SetValidator(new ImageFileValidator()!);
    }
}