using MediatR;
using ZenBlog.Application.Base;

public record UpdateSubCommentCommand(Guid? Id, string? Body) : IRequest<BaseResult<object>>;