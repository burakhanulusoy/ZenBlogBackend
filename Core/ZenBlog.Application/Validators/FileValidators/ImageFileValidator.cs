using FluentValidation;
using Microsoft.AspNetCore.Http;
using ZenBlog.Application.Constants;

namespace ZenBlog.Application.Validators.FileValidators;

// Tek bir resim dosyasýný (IFormFile) doðrular: boþ mu, çok mu büyük, uzantýsý uygun mu?
// Blog create ve update validator'larýnda SetValidator ile kullanýlýyor.
public class ImageFileValidator : AbstractValidator<IFormFile>
{
    public ImageFileValidator()
    {
        RuleFor(x => x.Length)
            .GreaterThan(0).WithMessage("Dosya boþ olamaz.")
            .LessThanOrEqualTo(ImageRules.MaxFileSize).WithMessage("Dosya boyutu en fazla 5 MB olabilir.");

        RuleFor(x => x.FileName)
            .Must(name => ImageRules.AllowedExtensions.Contains(Path.GetExtension(name).ToLowerInvariant()))
            .WithMessage($"Sadece {string.Join(", ", ImageRules.AllowedExtensions)} dosyalarý yüklenebilir.");
    }
}