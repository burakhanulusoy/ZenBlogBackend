using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Results.UserResults;

namespace ZenBlog.Application.Features.Queries.UserQueries;

// Admin: tek kullanýcýnýn detayý + bloke geçmiþi
public record GetUserByIdQuery(Guid Id) : IRequest<BaseResult<GetUserByIdQueryResult>>;