using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Results.CategoryResults;

namespace ZenBlog.Application.Features.Queries.CategoryQueries;

public record GetCategoriesWithBlogsQuery : IRequest<BaseResult<List<GetCategoriesWithBlogsQueryResult>>>;