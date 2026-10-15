using MediatR;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.CommentCommands;

public record UpdateCommentCommand(Guid? Id, string? Body) : IRequest<BaseResult<object>>;