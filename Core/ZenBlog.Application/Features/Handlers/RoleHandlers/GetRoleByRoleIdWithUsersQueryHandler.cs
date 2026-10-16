using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Queries.RoleQueries;
using ZenBlog.Application.Features.Results.RoleResults;
using ZenBlog.Application.Features.Results.UserResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.RoleHandlers;

public class GetRoleByRoleIdWithUsersQueryHandler(RoleManager<AppRole> _roleManager,
                                                  UserManager<AppUser> _userManager,
                                                  IMapper _mapper)
    : IRequestHandler<GetRoleByRoleIdWithUserQuery, BaseResult<GetRoleByRoleIdWithUsersQueryResult>>
{
    public async Task<BaseResult<GetRoleByRoleIdWithUsersQueryResult>> Handle(GetRoleByRoleIdWithUserQuery request, CancellationToken cancellationToken)
    {
        var role = await _roleManager.FindByIdAsync(request.RoleId.ToString());

        if (role is null)
            return BaseResult<GetRoleByRoleIdWithUsersQueryResult>.Fail("Bu Id'ye sahip rol bulunamadý.");

        // 1) Bu role sahip kullanýcýlarý Identity'den çek (mapper bunu yapamaz)
        var users = await _userManager.GetUsersInRoleAsync(role.Name!);

        // 2) Kullanýcýlarý result'a çevirmek için mapper
        var mappedUsers = _mapper.Map<List<GetUsersQueryResult>>(users);

        // 3) Rol + kullanýcýlar ? sonucu kendimiz oluþturuyoruz
        var result = new GetRoleByRoleIdWithUsersQueryResult(role.Id, role.Name!, mappedUsers);

        return BaseResult<GetRoleByRoleIdWithUsersQueryResult>.Success(result);
    }
}