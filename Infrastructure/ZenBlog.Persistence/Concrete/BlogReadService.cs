using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Results.BlogResults;
using ZenBlog.Application.Features.Results.HomeResults;
using ZenBlog.Domain.Entities;
using ZenBlog.Persistence.Context;

namespace ZenBlog.Persistence.Concrete;

// =====================================================================
// SİTE: BLOG OKUMA SORGULARI (kategori sayfası, ilgili yazılar)
// Hepsi projeksiyonla: EF sadece gereken sütunları tek SQL'de getirir.
// Sayfalama veritabanında yapılır (Skip/Take → SQL'de OFFSET/FETCH):
// 200 yazılı bir kategoride bile sadece o sayfadaki 9 yazı gelir.
// =====================================================================
public class BlogReadService(AppDbContext _context) : IBlogReadService
{
    private const int MaxPageSize = 30;

    // Blog → kart dönüşümü. Expression olarak tanımlandığı için EF bunu SQL'e çevirebiliyor
    // (normal bir C# metodu olsaydı EF içine bakamaz, bütün satırları belleğe çekerdi).
    private static readonly Expression<Func<Blog, HomeBlogResult>> ToCard = b => new HomeBlogResult(
        b.Id, b.MainTitle, b.MainDescription, b.CoverImage, b.CreatedAt,
        b.CategoryId, b.Category.Name,
        b.AppUserId, b.AppUser.Name + " " + b.AppUser.Surname, b.AppUser.ImageUrl,
        b.Comments.Count + b.Comments.SelectMany(c => c.SubComments).Count());

    public async Task<CategoryPageResult?> GetCategoryPageAsync(Guid categoryId, int page, int pageSize, CancellationToken cancellationToken)
    {
        // 1) Kategori var mı? (silinmiş kategori de "yok" sayılır, soft delete filtresi)
        var category = await _context.Categories
            .AsNoTracking()
            .Where(c => c.Id == categoryId)
            .Select(c => new { c.Id, c.Name })
            .FirstOrDefaultAsync(cancellationToken);

        if (category is null)
            return null;

        // 2) Sayfa ayarlarını güvenli aralığa çek (biri ?pageSize=100000 gönderemesin)
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

        var query = _context.Blogs.AsNoTracking().Where(b => b.CategoryId == categoryId);

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
        page = Math.Clamp(page, 1, totalPages);

        // 3) Sadece istenen sayfa
        var blogs = await query
            .OrderByDescending(b => b.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ToCard)
            .ToListAsync(cancellationToken);

        return new CategoryPageResult(category.Id, category.Name, totalCount, page, pageSize, totalPages, blogs);
    }

    public async Task<List<HomeBlogResult>> GetRelatedAsync(Guid blogId, int count, CancellationToken cancellationToken)
    {
        count = Math.Clamp(count, 1, 12);

        // Blogun kategorisi (blog yoksa boş liste)
        var categoryId = await _context.Blogs
            .AsNoTracking()
            .Where(b => b.Id == blogId)
            .Select(b => (Guid?)b.CategoryId)
            .FirstOrDefaultAsync(cancellationToken);

        if (categoryId is null)
            return new List<HomeBlogResult>();

        // 1) Aynı kategoriden, bu blog hariç, en yeniler
        var related = await _context.Blogs
            .AsNoTracking()
            .Where(b => b.CategoryId == categoryId && b.Id != blogId)
            .OrderByDescending(b => b.CreatedAt)
            .Take(count)
            .Select(ToCard)
            .ToListAsync(cancellationToken);

        // 2) Yetmezse en yeni yazılarla tamamla (okuyucu boş bölüm görmesin)
        if (related.Count < count)
        {
            var excludedIds = related.Select(r => r.Id).Append(blogId).ToList();

            var more = await _context.Blogs
                .AsNoTracking()
                .Where(b => !excludedIds.Contains(b.Id))
                .OrderByDescending(b => b.CreatedAt)
                .Take(count - related.Count)
                .Select(ToCard)
                .ToListAsync(cancellationToken);

            related.AddRange(more);
        }

        return related;
    }
}