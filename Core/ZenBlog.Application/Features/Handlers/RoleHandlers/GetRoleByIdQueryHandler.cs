using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Queries.RoleQueries;
using ZenBlog.Application.Features.Results.RoleResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.RoleHandlers;

public class GetRoleByIdQueryHandler(RoleManager<AppRole> _roleManager,
                                     IMapper _mapper) : IRequestHandler<GetRoleByIdQuery, BaseResult<GetRoleByIdQueryResult>>
{
    public async Task<BaseResult<GetRoleByIdQueryResult>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {

        var role = await _roleManager.FindByIdAsync(request.Id.ToString());

        if (role is null)
            return BaseResult<GetRoleByIdQueryResult>.Fail("Bu Id'ye sahip rol bulunamadý.");

        var mappedRole = _mapper.Map<GetRoleByIdQueryResult>(role);

        return BaseResult<GetRoleByIdQueryResult>.Success(mappedRole);



    }
}
