using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Queries.BlogQueries;
using ZenBlog.Application.Features.Results.HomeResults;

namespace ZenBlog.Application.Features.Handlers.BlogHandlers;

public class GetRelatedBlogsQueryHandler(IBlogReadService _blogReadService)
    : IRequestHandler<GetRelatedBlogsQuery, BaseResult<List<HomeBlogResult>>>
{
    public async Task<BaseResult<List<HomeBlogResult>>> Handle(GetRelatedBlogsQuery request, CancellationToken cancellationToken)
    {
        var result = await _blogReadService.GetRelatedAsync(request.BlogId, request.Count, cancellationToken);
        return BaseResult<List<HomeBlogResult>>.Success(result);
    }
}