using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Results.BlogResults;

namespace ZenBlog.Application.Features.Queries.BlogQueries;

public record GetBlogsQuery:IRequest<BaseResult<List<GetBlogsQueryResult>>>;
