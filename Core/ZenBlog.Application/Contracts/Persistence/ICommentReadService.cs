using ZenBlog.Application.Features.Results.CommentResults;

namespace ZenBlog.Application.Contracts.Persistence;

// Blog detayındaki yorumları okuyan servis (projeksiyon → tek SQL, lazy loading yok)
public interface ICommentReadService
{
    Task<List<GetCommentsByBlogIdQueryResult>> GetByBlogIdAsync(Guid blogId, CancellationToken cancellationToken);
}