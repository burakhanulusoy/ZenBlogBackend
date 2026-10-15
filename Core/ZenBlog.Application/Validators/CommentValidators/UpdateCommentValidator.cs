using FluentValidation;
using ZenBlog.Application.Features.Commands.CommentCommands;

namespace ZenBlog.Application.Validators.CommentValidators;

public class UpdateCommentValidator : AbstractValidator<UpdateCommentCommand>
{
    public UpdateCommentValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Yorum Id boþ olamaz.");

        RuleFor(x => x.Body)
            .NotEmpty().WithMessage("Yorum boþ olamaz.")
            .MaximumLength(1000).WithMessage("Yorum en fazla 1000 karakter olabilir.");
    }
}