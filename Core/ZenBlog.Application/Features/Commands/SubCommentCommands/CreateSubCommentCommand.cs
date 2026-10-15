using MediatR;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.SubCommentCommands;

public record CreateSubCommentCommand(string? Body,
                                      Guid? CommentId,
                                      Guid? AppUserId) : IRequest<BaseResult<object>>;