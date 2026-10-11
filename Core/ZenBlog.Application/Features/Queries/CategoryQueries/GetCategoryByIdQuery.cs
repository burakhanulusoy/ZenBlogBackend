using MediatR;
using ZenBlog.Application.Features.Results.CategoryResults;

namespace ZenBlog.Application.Features.Queries.CategoryQueries;

public record GetCategoryByIdQuery(Guid Id) : IRequest<GetCategoryByIdQueryResult>;