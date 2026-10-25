using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Queries.BlogQueries;
using ZenBlog.Application.Features.Results.BlogResults;

namespace ZenBlog.Application.Features.Handlers.BlogHandlers;

public class GetCategoryPageQueryHandler(IBlogReadService _blogReadService)
    : IRequestHandler<GetCategoryPageQuery, BaseResult<CategoryPageResult>>
{
    public async Task<BaseResult<CategoryPageResult>> Handle(GetCategoryPageQuery request, CancellationToken cancellationToken)
    {
        var result = await _blogReadService.GetCategoryPageAsync(request.CategoryId, request.Page, request.PageSize, cancellationToken);

        return result is null
            ? BaseResult<CategoryPageResult>.Fail("Kategori bulunamadý.")
            : BaseResult<CategoryPageResult>.Success(result);
    }
}