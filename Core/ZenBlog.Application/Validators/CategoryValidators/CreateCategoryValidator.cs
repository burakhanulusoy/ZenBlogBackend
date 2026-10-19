using FluentValidation;
using ZenBlog.Application.Features.Commands.CategoryCommands;

namespace ZenBlog.Application.Validators.CategoryValidators
{
    public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Kategori adý boþ olamaz.")
                .MaximumLength(50).WithMessage("Kategori adý en fazla 50 karakter olabilir.");







        }
    }
}