using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Domain.Enums;

namespace ZenBlog.Application.Features.Commands.ContactMessageCommands;

public record UpdateContactMessageCommand(Guid Id,ContactMessageStatus? Status):IRequest<BaseResult<object>>;
