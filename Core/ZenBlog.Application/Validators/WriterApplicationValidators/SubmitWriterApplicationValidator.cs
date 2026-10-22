using FluentValidation;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Features.Commands.WriterApplicationCommands;

namespace ZenBlog.Application.Validators.WriterApplicationValidators;

public class SubmitWriterApplicationValidator : AbstractValidator<SubmitWriterApplicationCommand>
{
    public SubmitWriterApplicationValidator()
    {
        RuleFor(x => x.CategoryIds)
            .NotEmpty().WithMessage("Yazmak istediðin en az bir konu seç.")
            .Must(ids => ids.Distinct().Count() <= WriterApplicationRules.MaxTopics)
            .WithMessage($"En fazla {WriterApplicationRules.MaxTopics} konu seçebilirsin.");

        RuleFor(x => x.Motivation)
            .NotEmpty().WithMessage("Neden yazar olmak istediðini yaz.")
            .MinimumLength(WriterApplicationRules.MinMotivationLength)
            .WithMessage($"Biraz daha anlat: en az {WriterApplicationRules.MinMotivationLength} karakter.")
            .MaximumLength(2000).WithMessage("En fazla 2000 karakter yazabilirsin.");

        RuleFor(x => x.Experience)
            .MaximumLength(1000).WithMessage("En fazla 1000 karakter yazabilirsin.");

        RuleFor(x => x.SampleLinks)
            .MaximumLength(1000).WithMessage("En fazla 1000 karakter yazabilirsin.");

        RuleFor(x => x.AcceptedRules)
            .Equal(true).WithMessage("Baþvurmak için yazarlýk kurallarýný kabul etmelisin.");
    }
}