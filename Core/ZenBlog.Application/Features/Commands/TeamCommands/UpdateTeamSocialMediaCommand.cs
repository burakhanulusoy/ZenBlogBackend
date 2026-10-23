using MediatR;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.TeamCommands;

public record UpdateTeamSocialMediaCommand(Guid Id,
                                           string? SocialMediaName,
                                           string? Url) : IRequest<BaseResult<object>>;