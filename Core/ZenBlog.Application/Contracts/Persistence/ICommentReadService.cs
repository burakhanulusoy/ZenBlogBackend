using ZenBlog.Application.Features.Results.CommentResults;

namespace ZenBlog.Application.Contracts.Persistence;

// Yorum okuma sorguları (projeksiyon → tek SQL, lazy loading yok)
public interface ICommentReadService
{
    // Blog detayı: yorumlar + yanıtlar
    Task<List<GetCommentsByBlogIdQueryResult>> GetByBlogIdAsync(Guid blogId, CancellationToken cancellationToken);

    // YENİ: Kişinin kendi yorum ve yanıtları
    Task<List<CommentManageResult>> GetByAuthorAsync(Guid userId, CancellationToken cancellationToken);

    // YENİ: Kişinin bloglarına BAŞKALARININ yazdıkları
    Task<List<CommentManageResult>> GetOnBlogsOfAsync(Guid blogAuthorId, CancellationToken cancellationToken);

    // YENİ: Bütün yorumlar (admin, en yeni "take" kadar)
    Task<List<CommentManageResult>> GetAllAsync(int take, CancellationToken cancellationToken);
}