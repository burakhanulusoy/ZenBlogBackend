using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Results.SubCommentResults;

namespace ZenBlog.Application.Features.Queries.SubCommentQueries;

public record GetSubCommentsByCommentIdQuery(Guid CommentId)
    : IRequest<BaseResult<List<GetSubCommentsByCommentIdQueryResult>>>;