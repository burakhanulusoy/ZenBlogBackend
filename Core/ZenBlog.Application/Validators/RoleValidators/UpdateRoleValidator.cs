using FluentValidation;
using ZenBlog.Application.Features.Commands.RoleCommands;

namespace ZenBlog.Application.Validators.RoleValidators;

public class UpdateRoleValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Rol Id boþ olamaz.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Rol adý boþ olamaz.")
            .Length(2, 50).WithMessage("Rol adý 2 ile 50 karakter arasýnda olmalýdýr.")
            .Matches("^[a-zA-Z]+$").WithMessage("Rol adý sadece Ýngilizce harflerden oluþmalýdýr (örn. Admin, Writer).");
    }
}