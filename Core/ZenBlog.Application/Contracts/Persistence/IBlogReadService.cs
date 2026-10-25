using ZenBlog.Application.Features.Results.BlogResults;
using ZenBlog.Application.Features.Results.HomeResults;

namespace ZenBlog.Application.Contracts.Persistence;

// Ziyaretçi sitesi için blog okuma sorguları (projeksiyon → lazy loading yok, N+1 yok)
public interface IBlogReadService
{
    // Kategori bulunamazsa null
    Task<CategoryPageResult?> GetCategoryPageAsync(Guid categoryId, int page, int pageSize, CancellationToken cancellationToken);

    // Aynı kategoriden ilgili yazılar (yetmezse en yenilerle tamamlanır)
    Task<List<HomeBlogResult>> GetRelatedAsync(Guid blogId, int count, CancellationToken cancellationToken);
}