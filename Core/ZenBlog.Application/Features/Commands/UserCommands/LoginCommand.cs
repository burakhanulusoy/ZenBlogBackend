using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Results.UserResults;

namespace ZenBlog.Application.Features.Commands.UserCommands;

public record LoginCommand(string? Email, string? Password) : IRequest<BaseResult<LoginCommandResult>>;