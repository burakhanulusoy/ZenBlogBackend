using FluentValidation;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Features.Commands.RoleCommands;

namespace ZenBlog.Application.Validators.RoleValidators;

public class CreateRoleValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Rol adý boþ olamaz.")
            .Length(2, 50).WithMessage("Rol adý 2 ile 50 karakter arasýnda olmalýdýr.")
            .Matches("^[a-zA-Z]+$").WithMessage("Rol adý sadece Ýngilizce harflerden oluþmalýdýr (örn. Editor).")
            // YENÝ: "admin", "WRITER" gibi sistem rolü adlarý farklý yazýmla da açýlamasýn
            .Must(name => !Roles.All.Contains(name, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Bu ad bir sistem rolüne ait, kullanýlamaz.");
    }
}