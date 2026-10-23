using MediatR;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.TeamCommands;

// Alan adlarý entity ile ayný (SocialMediaName, Url, TeamId) ? AutoMapper kendiliðinden eþler
// TeamId adresten (/teams/{teamId}/social-medias) doldurulur.
public record CreateTeamSocialMediaCommand(Guid TeamId,
                                           string? SocialMediaName,
                                           string? Url) : IRequest<BaseResult<object>>;