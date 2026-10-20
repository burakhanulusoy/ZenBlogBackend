using MediatR;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.UserCommands;

public record ForgotPasswordCommand(string? Email) : IRequest<BaseResult<object>>;