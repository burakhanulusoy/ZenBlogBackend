using MediatR;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.UserCommands;

public record CreateUserCommand(string Name,
                                string Surname,
                                string Email,
                                string Password,
                                string ConfirmPassword,
                                string PhoneNumber,
                                string UserName
                                ) : IRequest<BaseResult<object>>;
