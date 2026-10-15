using ZenBlog.Application.Features.Results.UserResults;

namespace ZenBlog.Application.Features.Results.SubCommentResults;

public record GetSubCommentsByCommentIdQueryResult(Guid Id,
                                                   string Body,
                                                   DateTime CreatedAt,
                                                   GetUsersQueryResult AppUser);