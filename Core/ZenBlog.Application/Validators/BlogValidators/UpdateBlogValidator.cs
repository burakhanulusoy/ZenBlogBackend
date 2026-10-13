using FluentValidation;
using ZenBlog.Application.Features.Commands.BlogCommands;
using ZenBlog.Application.Validators.FileValidators;

namespace ZenBlog.Application.Validators.BlogValidators;

public class UpdateBlogValidator : AbstractValidator<UpdateBlogCommand>
{
    public UpdateBlogValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Blog Id boþ olamaz.");

        // Baþlýklar
        RuleFor(x => x.MainTitle)
            .NotEmpty().WithMessage("Ana baþlýk boþ olamaz.")
            .MaximumLength(150).WithMessage("Ana baþlýk en fazla 150 karakter olabilir.");

        RuleFor(x => x.Title1)
            .NotEmpty().WithMessage("1. baþlýk boþ olamaz.")
            .MaximumLength(150).WithMessage("1. baþlýk en fazla 150 karakter olabilir.");

        RuleFor(x => x.Title2)
            .NotEmpty().WithMessage("2. baþlýk boþ olamaz.")
            .MaximumLength(150).WithMessage("2. baþlýk en fazla 150 karakter olabilir.");

        RuleFor(x => x.Title3)
            .NotEmpty().WithMessage("3. baþlýk boþ olamaz.")
            .MaximumLength(150).WithMessage("3. baþlýk en fazla 150 karakter olabilir.");

        // Açýklamalar
        RuleFor(x => x.MainDescription).NotEmpty().WithMessage("Ana açýklama boþ olamaz.");
        RuleFor(x => x.Description1).NotEmpty().WithMessage("1. açýklama boþ olamaz.");
        RuleFor(x => x.Description2).NotEmpty().WithMessage("2. açýklama boþ olamaz.");
        RuleFor(x => x.Description3).NotEmpty().WithMessage("3. açýklama boþ olamaz.");
        RuleFor(x => x.Description4).NotEmpty().WithMessage("4. açýklama boþ olamaz.");

        // Görseller: seçilmediyse sorun yok (eskisi kalýr), seçildiyse kurallara uymalý
        RuleFor(x => x.CoverImage)
            .SetValidator(new ImageFileValidator()!)
            .When(x => x.CoverImage is not null);

        RuleFor(x => x.BlogImage1)
            .SetValidator(new ImageFileValidator()!)
            .When(x => x.BlogImage1 is not null);

        RuleFor(x => x.BlogImage2)
            .SetValidator(new ImageFileValidator()!)
            .When(x => x.BlogImage2 is not null);

        // Ýliþki
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Kategori seçilmelidir.");
    }
}