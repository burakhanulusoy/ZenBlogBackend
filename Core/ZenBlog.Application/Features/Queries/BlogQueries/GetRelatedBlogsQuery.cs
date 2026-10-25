using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Results.HomeResults;

namespace ZenBlog.Application.Features.Queries.BlogQueries;

public record GetRelatedBlogsQuery(Guid BlogId, int Count) : IRequest<BaseResult<List<HomeBlogResult>>>;