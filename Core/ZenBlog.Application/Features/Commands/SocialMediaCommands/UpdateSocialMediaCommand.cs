using MediatR;
using Microsoft.AspNetCore.Http;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.SocialMediaCommands;

public record UpdateSocialMediaCommand(Guid Id,
                                      IFormFile? Icon,
                                      string? Title,
                                      string? Url):IRequest<BaseResult<object>>;