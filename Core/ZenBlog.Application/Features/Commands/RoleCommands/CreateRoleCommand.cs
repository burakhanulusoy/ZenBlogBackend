using MediatR;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.RoleCommands;

    public record CreateRoleCommand(string Name) : IRequest<BaseResult<object>>;
