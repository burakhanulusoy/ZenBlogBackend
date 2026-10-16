using MediatR;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.RoleCommands;

public record UpdateRoleCommand(Guid Id,string Name):IRequest<BaseResult<object>>;
