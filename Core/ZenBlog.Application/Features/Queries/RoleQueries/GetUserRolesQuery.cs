using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Results.RoleResults;

namespace ZenBlog.Application.Features.Queries.RoleQueries;

public record GetUserRolesQuery(Guid UserId) : IRequest<BaseResult<GetUserRolesQueryResult>>;