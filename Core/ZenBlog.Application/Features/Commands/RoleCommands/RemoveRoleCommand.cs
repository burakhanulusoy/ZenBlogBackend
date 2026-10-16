using MediatR;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.RoleCommands;

public record RemoveRoleCommand(Guid Id):IRequest<BaseResult<object>>;
