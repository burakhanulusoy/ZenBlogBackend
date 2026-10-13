using FluentValidation;
using ZenBlog.Application.Features.Commands.BlogCommands;

namespace ZenBlog.Application.Validators.BlogValidators;

public class CreateBlogValidator : AbstractValidator<CreateBlogCommand>
{
    public CreateBlogValidator()
    {
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
        RuleFor(x => x.MainDescription)
            .NotEmpty().WithMessage("Ana açýklama boþ olamaz.");

        RuleFor(x => x.Description1)
            .NotEmpty().WithMessage("1. açýklama boþ olamaz.");

        RuleFor(x => x.Description2)
            .NotEmpty().WithMessage("2. açýklama boþ olamaz.");

        RuleFor(x => x.Description3)
            .NotEmpty().WithMessage("3. açýklama boþ olamaz.");

        RuleFor(x => x.Description4)
            .NotEmpty().WithMessage("4. açýklama boþ olamaz.");

        // Görseller
        RuleFor(x => x.CoverImage)
            .NotEmpty().WithMessage("Kapak görseli boþ olamaz.");

        RuleFor(x => x.BlogImage1)
            .NotEmpty().WithMessage("1. blog görseli boþ olamaz.");

        RuleFor(x => x.BlogImage2)
            .NotEmpty().WithMessage("2. blog görseli boþ olamaz.");

        // Ýliþkiler
        // Guid? için NotEmpty hem null'ý hem de 00000000-... (Guid.Empty) deðerini yakalar.
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Kategori seçilmelidir.");

        RuleFor(x => x.AppUserId)
            .NotEmpty().WithMessage("Yazar bilgisi boþ olamaz.");
    }
}