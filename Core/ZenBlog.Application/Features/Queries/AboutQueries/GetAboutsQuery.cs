using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Results.AboutResults;

namespace ZenBlog.Application.Features.Queries.AboutQueries;

public record GetAboutsQuery : IRequest<BaseResult<List<GetAboutsQueryResult>>>;