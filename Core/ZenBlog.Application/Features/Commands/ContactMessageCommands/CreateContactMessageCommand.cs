using MediatR;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.ContactMessageCommands;

public record CreateContactMessageCommand(string? Name,
                                   string? Email,
                                   string? Subject,
                                   string? MessageBody):IRequest<BaseResult<object>>;
