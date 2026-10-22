using MediatR;
using Microsoft.AspNetCore.Identity;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Features.Commands.RoleCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.RoleHandlers
{
    public class UpdateRoleCommandHandler(RoleManager<AppRole> _roleManager)
        : IRequestHandler<UpdateRoleCommand, BaseResult<object>>
    {
        public async Task<BaseResult<object>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(request.Id.ToString());
            if (role is null)
                return BaseResult<object>.Fail("Bu Id'ye sahip rol bulunamadý.");

            // YENÝ: Sistem rollerinin adý kodda geçiyor (policy, guard, yönlendirme);
            // deðiþirse paneller çalýþmaz.
            if (Roles.All.Contains(role.Name))
                return BaseResult<object>.Fail($"'{role.Name}' bir sistem rolüdür, adý deðiþtirilemez.");

            // YENÝ: Özel bir rolün adý sistem rolü adýna çevrilemesin
            if (Roles.All.Contains(request.Name, StringComparer.OrdinalIgnoreCase))
                return BaseResult<object>.Fail("Name", "Bu ad bir sistem rolüne ait, kullanýlamaz.");

            role.Name = request.Name;

            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded)
                return BaseResult<object>.Fail(result.Errors);

            return BaseResult<object>.Success("Rol baþarýlý bir þekilde güncellendi.");
        }
    }
}