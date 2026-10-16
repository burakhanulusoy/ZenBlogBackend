using FluentValidation;
using ZenBlog.Application.Features.Commands.CommentCommands;

namespace ZenBlog.Application.Validators.CommentValidators;

public class CreateCommentValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentValidator()
    {
        RuleFor(x => x.Body)
            .NotEmpty().WithMessage("Yorum boþ olamaz.")
            .MaximumLength(1000).WithMessage("Yorum en fazla 1000 karakter olabilir.");

        RuleFor(x => x.BlogId)
            .NotEmpty().WithMessage("Blog bilgisi boþ olamaz.");

        //RuleFor(x => x.AppUserId)
        //    .NotEmpty().WithMessage("Kullanýcý bilgisi boþ olamaz.");
    }
}