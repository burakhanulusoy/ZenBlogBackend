using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Results.TeamResults;

namespace ZenBlog.Application.Features.Queries.TeamQueries;

public record GetTeamsQuery : IRequest<BaseResult<List<GetTeamsQueryResult>>>;