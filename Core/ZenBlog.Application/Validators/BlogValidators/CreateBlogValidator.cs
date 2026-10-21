using FluentValidation;
using ZenBlog.Application.Features.Commands.BlogCommands;
using ZenBlog.Application.Validators.FileValidators;   // YENÝ: ImageFileValidator için

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
        // DEÐÝÞTÝ: Önce "seçilmiþ mi?" (NotEmpty), sonra "kurallara uyuyor mu?" (ImageFileValidator:
        // en fazla 5 MB, sadece .jpg/.jpeg/.png/.webp). Update'teki kurallarýn aynýsý.
        // SetValidator, dosya null ise kendiliðinden atlanýr; o durumda sadece NotEmpty mesajý döner.
        // Not: Alt validator'ýn hatalarý "CoverImage.Length" / "CoverImage.FileName" adýyla döner;
        // frontend "CoverImage" ile baþlayan hatalarý o alanýn altýnda gösteriyor.
        RuleFor(x => x.CoverImage)
            .NotEmpty().WithMessage("Kapak görseli boþ olamaz.")
            .SetValidator(new ImageFileValidator()!);           // YENÝ

        RuleFor(x => x.BlogImage1)
            .NotEmpty().WithMessage("1. blog görseli boþ olamaz.")
            .SetValidator(new ImageFileValidator()!);           // YENÝ

        RuleFor(x => x.BlogImage2)
            .NotEmpty().WithMessage("2. blog görseli boþ olamaz.")
            .SetValidator(new ImageFileValidator()!);           // YENÝ

        // Ýliþkiler
        // Guid? için NotEmpty hem null'ý hem de 00000000-... (Guid.Empty) deðerini yakalar.
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Kategori seçilmelidir.");

        // Yazar (AppUserId) istekten gelmiyor; handler ICurrentUserService ile token'dan alýyor.
    }
}