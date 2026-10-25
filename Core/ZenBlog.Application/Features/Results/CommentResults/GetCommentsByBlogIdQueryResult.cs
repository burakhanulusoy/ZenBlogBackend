namespace ZenBlog.Application.Features.Results.CommentResults;

// DEÐÝÞTÝ: Blog detayýndaki yorum (yazar bilgisi + yanýtlar).
// Kiþisel bilgi (e-posta vb.) YOK; sadece ad ve fotoðraf.
public record GetCommentsByBlogIdQueryResult(Guid Id,
                                             string Body,
                                             DateTime CreatedAt,
                                             DateTime? UpdatedAt,   // "(düzenlendi)" bilgisi için
                                             Guid AuthorId,
                                             string AuthorName,
                                             string? AuthorImage,
                                             List<CommentReplyResult> Replies);

// Yorumun altýndaki yanýt (alt yorum)
public record CommentReplyResult(Guid Id,
                                 string Body,
                                 DateTime CreatedAt,
                                 DateTime? UpdatedAt,
                                 Guid AuthorId,
                                 string AuthorName,
                                 string? AuthorImage);