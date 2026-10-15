using MediatR;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.CommentCommands;

public record RemoveCommentCommand(Guid Id) : IRequest<BaseResult<object>>;