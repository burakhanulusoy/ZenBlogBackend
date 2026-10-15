using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Results.CommentResults;

namespace ZenBlog.Application.Features.Queries.CommentQueries;

public record GetCommentsByUserIdQuery(Guid UserId)
    : IRequest<BaseResult<List<GetCommentsByUserIdQueryResult>>>;