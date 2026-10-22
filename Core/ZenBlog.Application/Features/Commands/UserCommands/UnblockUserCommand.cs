using MediatR;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.UserCommands;

// Admin: blokeyi kaldýr. Body: { "reason": "..." }  (UserId adresten doldurulur)
public record UnblockUserCommand(Guid UserId, string? Reason) : IRequest<BaseResult<object>>;