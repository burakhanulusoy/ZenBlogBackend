using FluentValidation;
using ZenBlog.Application.Features.Commands.SubCommentCommands;

namespace ZenBlog.Application.Validators.SubCommentValidators;

public class CreateSubCommentValidator : AbstractValidator<CreateSubCommentCommand>
{
    public CreateSubCommentValidator()
    {
        RuleFor(x => x.Body)
            .NotEmpty().WithMessage("Cevap boþ olamaz.")
            .MaximumLength(1000).WithMessage("Cevap en fazla 1000 karakter olabilir.");

        RuleFor(x => x.CommentId)
            .NotEmpty().WithMessage("Yorum bilgisi boþ olamaz.");

        RuleFor(x => x.AppUserId)
            .NotEmpty().WithMessage("Kullanýcý bilgisi boþ olamaz.");
    }
}