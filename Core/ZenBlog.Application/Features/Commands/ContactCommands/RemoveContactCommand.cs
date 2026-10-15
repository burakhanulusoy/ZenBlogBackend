using MediatR;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.ContactCommands;

public record RemoveContactCommand(Guid Id):IRequest<BaseResult<object>>;