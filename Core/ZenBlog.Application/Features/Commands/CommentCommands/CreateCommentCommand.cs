using MediatR;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.CommentCommands;

public record CreateCommentCommand(string? Body,
                                  // Guid? AppUserId,
                                   Guid? BlogId
                                   ):IRequest<BaseResult<object>>;
