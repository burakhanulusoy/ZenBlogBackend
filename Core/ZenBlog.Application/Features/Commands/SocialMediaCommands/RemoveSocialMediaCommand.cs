using MediatR;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.SocialMediaCommands;

public record RemoveSocialMediaCommand(Guid Id):IRequest<BaseResult<object>>;