using FluentValidation;
using Microsoft.AspNetCore.Http;
using ZenBlog.Application.Constants;

namespace ZenBlog.Application.Validators.FileValidators;

// Kanýt dosyasý: resim ya da PDF, en fazla 10 MB
public class EvidenceFileValidator : AbstractValidator<IFormFile>
{
    public EvidenceFileValidator()
    {
        RuleFor(x => x.Length)
            .GreaterThan(0).WithMessage("Dosya boþ olamaz.")
            .LessThanOrEqualTo(EvidenceRules.MaxFileSize).WithMessage("Kanýt dosyasý en fazla 10 MB olabilir.");

        RuleFor(x => x.FileName)
            .Must(name => EvidenceRules.AllowedExtensions.Contains(Path.GetExtension(name).ToLowerInvariant()))
            .WithMessage($"Sadece {string.Join(", ", EvidenceRules.AllowedExtensions)} dosyalarý yüklenebilir.");
    }
}