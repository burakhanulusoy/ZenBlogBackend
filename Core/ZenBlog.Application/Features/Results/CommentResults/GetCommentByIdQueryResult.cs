namespace ZenBlog.Application.Features.Results.CommentResults;

public record GetCommentByIdQueryResult(Guid Id,
                                        string Body,
                                        Guid BlogId,
                                        Guid AppUserId,
                                        DateTime CreatedAt);