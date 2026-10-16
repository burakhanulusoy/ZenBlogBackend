using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Queries.RoleQueries;
using ZenBlog.Application.Features.Results.RoleResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.RoleHandlers;

public class GetRolesQueryHandler(RoleManager<AppRole> _roleManager,
                                 IMapper _mapper) : IRequestHandler<GetRolesQuery, BaseResult<List<GetRolesQueryResult>>>
{
    public async Task<BaseResult<List<GetRolesQueryResult>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {

        var roles =_roleManager.Roles.ToList();

        var mappedRoles =_mapper.Map<List<GetRolesQueryResult>>(roles);

        return BaseResult<List<GetRolesQueryResult>>.Success(mappedRoles);


    }
}
