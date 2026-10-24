using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Queries.HomeQueries;
using ZenBlog.Application.Features.Results.HomeResults;

namespace ZenBlog.Application.Features.Handlers.HomeHandlers;

public class GetHomePageQueryHandler(IHomeReadService _homeReadService)
    : IRequestHandler<GetHomePageQuery, BaseResult<HomePageResult>>
{
    public async Task<BaseResult<HomePageResult>> Handle(GetHomePageQuery request, CancellationToken cancellationToken)
    {
        var result = await _homeReadService.GetHomePageAsync(cancellationToken);
        return BaseResult<HomePageResult>.Success(result);
    }
}