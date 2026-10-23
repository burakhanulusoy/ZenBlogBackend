using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Results.TeamResults;

namespace ZenBlog.Application.Features.Queries.TeamQueries;

public record GetTeamByIdQuery(Guid Id) : IRequest<BaseResult<GetTeamByIdQueryResult>>;