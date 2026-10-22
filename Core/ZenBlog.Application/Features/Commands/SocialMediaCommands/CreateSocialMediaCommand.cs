using MediatR;
using Microsoft.AspNetCore.Http;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.SocialMediaCommands;

public record CreateSocialMediaCommand(string? Title,
                                       string? Url,
                                       IFormFile? Icon):IRequest<BaseResult<object>>;
