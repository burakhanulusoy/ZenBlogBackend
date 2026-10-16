using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Results.RoleResults;

namespace ZenBlog.Application.Features.Queries.RoleQueries;

public record GetRoleByRoleIdWithUserQuery(Guid RoleId):IRequest<BaseResult<GetRoleByRoleIdWithUsersQueryResult>>;
