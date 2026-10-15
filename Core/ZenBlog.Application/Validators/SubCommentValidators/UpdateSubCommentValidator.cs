using FluentValidation;

namespace ZenBlog.Application.Validators.SubCommentValidators
{
    public class UpdateSubCommentValidator : AbstractValidator<UpdateSubCommentCommand>
    {
        public UpdateSubCommentValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Cevap Id boþ olamaz.");

            RuleFor(x => x.Body)
                .NotEmpty().WithMessage("Cevap boþ olamaz.")
                .MaximumLength(1000).WithMessage("Cevap en fazla 1000 karakter olabilir.");
        }
    }
}
