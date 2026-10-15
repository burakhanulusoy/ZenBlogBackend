using ZenBlog.Application.Features.Results.UserResults;

namespace ZenBlog.Application.Features.Results.CommentResults;

// Bir yorum: yazarý ve altýndaki cevaplarla birlikte
public record GetCommentsByBlogIdQueryResult(Guid Id,
                                             string Body,
                                             DateTime CreatedAt,
                                             GetUsersQueryResult AppUser,
                                             List<CommentSubCommentResult> SubComments);
