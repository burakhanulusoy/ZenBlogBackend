using MediatR;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.TeamCommands;

public record RemoveTeamCommand(Guid Id) : IRequest<BaseResult<object>>;