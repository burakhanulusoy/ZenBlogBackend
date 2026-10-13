using MediatR;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.UserCommands;

public record ConfirmEmailCommand(string Email, string Code) : IRequest<BaseResult<object>>;