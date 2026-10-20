using MediatR;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.UserCommands;

public record ResetPasswordCommand(string? Email,
                                   string? Code,
                                   string? NewPassword,
                                   string? ConfirmPassword) : IRequest<BaseResult<object>>;