using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Results.UserResults;

namespace ZenBlog.Application.Features.Queries.UserQueries;

// Admin: sistemdeki bütün kullanýcýlar (aktif, kapatýlýyor, bloke)
public record GetUsersQuery : IRequest<BaseResult<List<GetUserListQueryResult>>>;