using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Results.SubCommentResults;

public record GetSubCommentsByUserIdQuery(Guid UserId)
    : IRequest<BaseResult<List<GetSubCommentsByUserIdQueryResult>>>;