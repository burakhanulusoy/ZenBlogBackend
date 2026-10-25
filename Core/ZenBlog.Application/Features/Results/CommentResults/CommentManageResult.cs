namespace ZenBlog.Application.Features.Results.CommentResults;

// Panel yorum sayfalarý için ORTAK biçim (yorum ya da yanýt)
public record CommentManageResult(Guid Id,
                                  bool IsReply,              // true: yanýt (alt yorum)
                                  string Body,
                                  DateTime CreatedAt,
                                  DateTime? UpdatedAt,
                                  Guid AuthorId,
                                  string AuthorName,
                                  string? AuthorImage,
                                  Guid BlogId,
                                  string BlogTitle,
                                  Guid BlogAuthorId,         // silme yetkisi için (blog yazarý silebilir)
                                  string? ReplyToName,       // yanýtsa: kime yanýt verildi
                                  int ReplyCount);           // yorumsa: kaç yanýtý var