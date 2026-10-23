using FluentValidation;
using ZenBlog.Application.Features.Commands.TeamCommands;
using ZenBlog.Application.Validators.FileValidators;

namespace ZenBlog.Application.Validators.TeamValidators;

public class CreateTeamValidator : AbstractValidator<CreateTeamCommand>
{
    public CreateTeamValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ad boþ olamaz.")
            .MaximumLength(50).WithMessage("Ad en fazla 50 karakter olabilir.");

        RuleFor(x => x.Surname)
            .NotEmpty().WithMessage("Soyad boþ olamaz.")
            .MaximumLength(50).WithMessage("Soyad en fazla 50 karakter olabilir.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Unvan boþ olamaz.")
            .MaximumLength(100).WithMessage("Unvan en fazla 100 karakter olabilir.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Açýklama boþ olamaz.")
            .MaximumLength(1000).WithMessage("Açýklama en fazla 1000 karakter olabilir.");

        RuleFor(x => x.Image)
            .NotNull().WithMessage("Fotoðraf yüklemek zorunludur.")
            .SetValidator(new ImageFileValidator()!);
    }
}