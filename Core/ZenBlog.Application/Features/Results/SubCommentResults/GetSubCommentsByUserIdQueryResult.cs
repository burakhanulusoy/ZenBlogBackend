namespace ZenBlog.Application.Features.Results.SubCommentResults;

public record GetSubCommentsByUserIdQueryResult(Guid Id,
                                                string Body,
                                                DateTime CreatedAt,
                                                Guid CommentId,
                                                string CommentBody);