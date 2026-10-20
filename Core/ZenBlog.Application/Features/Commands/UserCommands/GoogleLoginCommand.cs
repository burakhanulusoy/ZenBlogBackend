using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Results.UserResults;

namespace ZenBlog.Application.Features.Commands.UserCommands;

// Frontend'in Google'dan aldýðý kimlik token'ý (ID token)
public record GoogleLoginCommand(string? IdToken) : IRequest<BaseResult<LoginCommandResult>>;