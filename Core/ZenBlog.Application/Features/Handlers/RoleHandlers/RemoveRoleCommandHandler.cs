using MediatR;
using Microsoft.AspNetCore.Identity;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Commands.RoleCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.RoleHandlers;

public class RemoveRoleCommandHandler(RoleManager<AppRole> _roleManager)
    : IRequestHandler<RemoveRoleCommand, BaseResult<object>>
{
    // Sistemin çalýþmasý için gerekli roller silinemez (kod bu adlara göre yetki veriyor)
    private static readonly string[] ProtectedRoles = ["Admin"];

    public async Task<BaseResult<object>> Handle(RemoveRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleManager.FindByIdAsync(request.Id.ToString());

        if (role is null)
            return BaseResult<object>.Fail("Bu Id'ye sahip rol bulunamadý.");

        if (ProtectedRoles.Contains(role.Name))
            return BaseResult<object>.Fail($"'{role.Name}' rolü sistem rolüdür, silinemez.");

        var result = await _roleManager.DeleteAsync(role);

        if (!result.Succeeded)
            return BaseResult<object>.Fail(result.Errors);

        return BaseResult<object>.Success("Rol baþarýlý bir þekilde silindi.");
    }
}