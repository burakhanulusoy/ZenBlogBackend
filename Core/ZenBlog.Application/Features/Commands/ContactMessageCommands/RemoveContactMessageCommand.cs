using MediatR;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.ContactMessageCommands;

public record RemoveContactMessageCommand(Guid Id):IRequest<BaseResult<object>>;
