using AutoMapper;
using ZenBlog.Application.Features.Commands.RoleCommands;
using ZenBlog.Application.Features.Results.RoleResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Mappings.RoleMappings;

public class RoleMapping : Profile
{
    public RoleMapping()
    {
        CreateMap<AppRole, GetRolesQueryResult>();
        CreateMap<AppRole, GetRoleByIdQueryResult>();

        CreateMap<CreateRoleCommand, AppRole>();

    }
}