using MediatR;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.UserCommands;

public record ResendConfirmationEmailCommand(string Email) : IRequest<BaseResult<object>>;