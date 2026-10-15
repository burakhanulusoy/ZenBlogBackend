namespace ZenBlog.Application.Features.Results.CommentResults;

public record GetCommentsByUserIdQueryResult(Guid Id,
                                             string Body,
                                             DateTime CreatedAt,
                                             Guid BlogId,
                                             string BlogMainTitle);