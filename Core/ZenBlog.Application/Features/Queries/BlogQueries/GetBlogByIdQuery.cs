using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Results.BlogResults;

namespace ZenBlog.Application.Features.Queries.BlogQueries;

public record GetBlogByIdQuery(Guid id) :IRequest<BaseResult<GetBlogByIdQueryResult>>;
