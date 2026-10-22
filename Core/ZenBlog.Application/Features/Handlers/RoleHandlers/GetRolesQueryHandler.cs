using MediatR;
using Microsoft.AspNetCore.Identity;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Features.Queries.RoleQueries;
using ZenBlog.Application.Features.Results.RoleResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.RoleHandlers;

// DEÐÝÞTÝ: AutoMapper yerine elle oluþturuyoruz (kullanýcý sayýsý ve sistem rolü bilgisi eklendi)
public class GetRolesQueryHandler(RoleManager<AppRole> _roleManager,
                                  UserManager<AppUser> _userManager)
    : IRequestHandler<GetRolesQuery, BaseResult<List<GetRolesQueryResult>>>
{
    public async Task<BaseResult<List<GetRolesQueryResult>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = _roleManager.Roles.OrderBy(r => r.Name).ToList();

        var result = new List<GetRolesQueryResult>();
        foreach (var role in roles)
        {
            var users = await _userManager.GetUsersInRoleAsync(role.Name!);
            result.Add(new GetRolesQueryResult(
                role.Id,
                role.Name!,
                users.Count,
                Roles.All.Contains(role.Name)));
        }

        return BaseResult<List<GetRolesQueryResult>>.Success(result);
    }
}