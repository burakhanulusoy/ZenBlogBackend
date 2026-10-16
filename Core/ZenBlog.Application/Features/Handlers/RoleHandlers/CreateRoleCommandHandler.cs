using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Commands.RoleCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.RoleHandlers;

public class CreateRoleCommandHandler(RoleManager<AppRole> _roleManager,
                                      IMapper _mapper) : IRequestHandler<CreateRoleCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
       
        var role = _mapper.Map<AppRole>(request);

        var result = await _roleManager.CreateAsync(role);

        if (!result.Succeeded)
            return BaseResult<object>.Fail(result.Errors);

        return BaseResult<object>.Success("Rol baþarýlý bir þekilde oluþturuldu.");



    }
}
