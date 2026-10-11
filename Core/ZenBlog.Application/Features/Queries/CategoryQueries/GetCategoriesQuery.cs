using MediatR;
using ZenBlog.Application.Features.Results.CategoryResults;

namespace ZenBlog.Application.Features.Queries.CategoryQueries;

public record GetCategoriesQuery:IRequest<List<GetCategoriesQueryResult>>;