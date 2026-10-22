using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Results.UserResults;

namespace ZenBlog.Application.Features.Queries.UserQueries;

// Giriþ yapan kiþinin kendi profili. Id YOK: handler token'dan alýyor.
public record GetMyProfileQuery : IRequest<BaseResult<GetMyProfileQueryResult>>;