using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Results.BlogResults;

namespace ZenBlog.Application.Features.Queries.BlogQueries;

public record GetCategoryPageQuery(Guid CategoryId, int Page, int PageSize) : IRequest<BaseResult<CategoryPageResult>>;