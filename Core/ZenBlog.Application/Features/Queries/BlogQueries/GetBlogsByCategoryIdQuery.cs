using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Results.BlogResults;

namespace ZenBlog.Application.Features.Queries.BlogQueries;

public record GetBlogsByCategoryIdQuery(Guid CategoryId):IRequest<BaseResult<List<GetBlogsByCategoryIdQueryResult>>>;