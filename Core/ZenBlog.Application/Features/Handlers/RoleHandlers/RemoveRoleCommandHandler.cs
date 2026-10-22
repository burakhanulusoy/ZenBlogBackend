using MediatR;
using Microsoft.AspNetCore.Identity;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Features.Commands.RoleCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.RoleHandlers;

public class RemoveRoleCommandHandler(RoleManager<AppRole> _roleManager,
                                      UserManager<AppUser> _userManager)
    : IRequestHandler<RemoveRoleCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(RemoveRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleManager.FindByIdAsync(request.Id.ToString());
        if (role is null)
            return BaseResult<object>.Fail("Bu Id'ye sahip rol bulunamadý.");

        // DEÐÝÞTÝ: Sadece Admin deðil, üç sistem rolü de korunuyor
        if (Roles.All.Contains(role.Name))
            return BaseResult<object>.Fail($"'{role.Name}' bir sistem rolüdür, silinemez.");

        // YENÝ: Ýçinde kullanýcý olan rol silinmesin (kimse habersiz rol kaybetmesin)
        var users = await _userManager.GetUsersInRoleAsync(role.Name!);
        if (users.Count > 0)
            return BaseResult<object>.Fail(
                $"Bu rolde {users.Count} kullanýcý var. Silmeden önce kullanýcýlarý bu rolden çýkar.");

        var result = await _roleManager.DeleteAsync(role);
        if (!result.Succeeded)
            return BaseResult<object>.Fail(result.Errors);

        return BaseResult<object>.Success("Rol baþarýlý bir þekilde silindi.");
    }
}