using FluentValidation;
using ZenBlog.Application.Features.Commands.RoleCommands;

namespace ZenBlog.Application.Validators.RoleValidators;

public class AssignRoleValidator : AbstractValidator<AssignRoleCommand>
{
    public AssignRoleValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Kullanýcý Id boþ olamaz.");

        // Liste gönderilmeli; boþ liste olabilir (kullanýcýnýn bütün rolleri kaldýrýlýr)
        RuleFor(x => x.RoleIds)
            .NotNull().WithMessage("Rol listesi gönderilmelidir.");

        RuleForEach(x => x.RoleIds)
            .NotEmpty().WithMessage("Rol Id boþ olamaz.");
    }
}