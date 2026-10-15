using MediatR;
using ZenBlog.Application.Base;

public record RemoveSubCommentCommand(Guid Id) : IRequest<BaseResult<object>>;