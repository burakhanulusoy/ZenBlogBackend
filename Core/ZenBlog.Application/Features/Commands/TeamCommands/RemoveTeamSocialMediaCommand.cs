using MediatR;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.TeamCommands;

public record RemoveTeamSocialMediaCommand(Guid Id) : IRequest<BaseResult<object>>;