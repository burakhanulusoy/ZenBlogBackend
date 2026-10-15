using FluentValidation;
using ZenBlog.Application.Features.Commands.ContactMessageCommands;

namespace ZenBlog.Application.Validators.ContactMessageValidators;

public class UpdateContactMessageValidator : AbstractValidator<UpdateContactMessageCommand>
{
    public UpdateContactMessageValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Mesaj Id boþ olamaz.");

        RuleFor(x => x.Status)
            .NotNull().WithMessage("Mesaj durumu seçilmelidir.")
            .IsInEnum().WithMessage("Geçersiz mesaj durumu.");
    }
}