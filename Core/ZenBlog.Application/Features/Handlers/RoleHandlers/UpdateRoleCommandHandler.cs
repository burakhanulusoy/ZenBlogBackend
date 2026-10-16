using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Commands.RoleCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.RoleHandlers
{
    public class UpdateRoleCommandHandler(RoleManager<AppRole> _roleManager,
                                          IMapper _mapper) : IRequestHandler<UpdateRoleCommand, BaseResult<object>>
    {
        public async Task<BaseResult<object>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(request.Id.ToString());

            if (role is null)
                return BaseResult<object>.Fail("Bu Id'ye sahip rol bulunamadý.");

            role.Name=request.Name;

            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
                return BaseResult<object>.Fail(result.Errors);

            return BaseResult<object>.Success("Rol baþarýlý bir þekilde güncellendi.");




        }
    }
}
