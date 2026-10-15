namespace ZenBlog.Application.Features.Results.SubCommentResults;

public record GetSubCommentByIdQueryResult(Guid Id,
                                           string Body,
                                           Guid CommentId,
                                           Guid AppUserId,
                                           DateTime CreatedAt);