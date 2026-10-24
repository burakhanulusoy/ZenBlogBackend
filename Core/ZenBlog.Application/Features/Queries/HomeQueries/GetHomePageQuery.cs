using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Results.HomeResults;

namespace ZenBlog.Application.Features.Queries.HomeQueries;

public record GetHomePageQuery : IRequest<BaseResult<HomePageResult>>;