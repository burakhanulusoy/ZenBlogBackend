using Microsoft.EntityFrameworkCore;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Results.HomeResults;
using ZenBlog.Persistence.Context;

namespace ZenBlog.Persistence.Concrete;

// =====================================================================
// ANA SAYFA SORGULARI
// Hepsi PROJEKSİYON (Select) ile yazıldı:
//   - EF bunu tek SQL'e çevirir (JOIN + alt sorgular)
//   - sadece gereken sütunlar gelir
//   - lazy loading hiç devreye girmez → N+1 sorgu sorunu yok
// Soft delete filtresi burada da otomatik: gizli bloglar ve silinmiş yorumlar gelmez/sayılmaz.
// =====================================================================
public class HomeReadService(AppDbContext _context) : IHomeReadService
{
    private const int SliderCount = 3;
    private const int LatestCount = 7;
    private const int TrendingCount = 5;
    private const int BlogsPerCategory = 5;

    public async Task<HomePageResult> GetHomePageAsync(CancellationToken cancellationToken)
    {
        // 1) SON 10 BLOG → ilk 3 slider, sonraki 7 üst bölüm
        var newest = await _context.Blogs
            .AsNoTracking()
            .OrderByDescending(b => b.CreatedAt)
            .Take(SliderCount + LatestCount)
            .Select(b => new HomeBlogResult(
                b.Id, b.MainTitle, b.MainDescription, b.CoverImage, b.CreatedAt,
                b.CategoryId, b.Category.Name,
                b.AppUserId, b.AppUser.Name + " " + b.AppUser.Surname, b.AppUser.ImageUrl,
                b.Comments.Count + b.Comments.SelectMany(c => c.SubComments).Count()))
            .ToListAsync(cancellationToken);

        var slider = newest.Take(SliderCount).ToList();

        // Yeterince blog varsa slider'dakiler tekrar etmesin; azsa son 7'yi göster (sayfa boş kalmasın)
        var latest = newest.Count > SliderCount + LatestCount - 1
            ? newest.Skip(SliderCount).Take(LatestCount).ToList()
            : newest.Take(LatestCount).ToList();

        // 2) TRENDING → yorum + alt yorum toplamı en yüksek 5 blog (eşitse en yeni önce)
        var trending = await _context.Blogs
            .AsNoTracking()
            .OrderByDescending(b => b.Comments.Count + b.Comments.SelectMany(c => c.SubComments).Count())
            .ThenByDescending(b => b.CreatedAt)
            .Take(TrendingCount)
            .Select(b => new HomeBlogResult(
                b.Id, b.MainTitle, b.MainDescription, b.CoverImage, b.CreatedAt,
                b.CategoryId, b.Category.Name,
                b.AppUserId, b.AppUser.Name + " " + b.AppUser.Surname, b.AppUser.ImageUrl,
                b.Comments.Count + b.Comments.SelectMany(c => c.SubComments).Count()))
            .ToListAsync(cancellationToken);

        // 3) KATEGORİ BÖLÜMLERİ → bloğu olan her kategori + son 5 blogu, TEK sorguda
        //    EF, c.Blogs.OrderBy...Take(5) ifadesini SQL'de ROW_NUMBER() OVER (PARTITION BY ...) yapar
        var sections = await _context.Categories
            .AsNoTracking()
            .Where(c => c.Blogs.Any())
            .OrderBy(c => c.Name)
            .Select(c => new HomeCategorySectionResult(
                c.Id,
                c.Name,
                c.Blogs
                    .OrderByDescending(b => b.CreatedAt)
                    .Take(BlogsPerCategory)
                    .Select(b => new HomeBlogResult(
                        b.Id, b.MainTitle, b.MainDescription, b.CoverImage, b.CreatedAt,
                        b.CategoryId, c.Name,
                        b.AppUserId, b.AppUser.Name + " " + b.AppUser.Surname, b.AppUser.ImageUrl,
                        b.Comments.Count + b.Comments.SelectMany(cm => cm.SubComments).Count()))
                    .ToList()))
            .ToListAsync(cancellationToken);

        return new HomePageResult(slider, latest, trending, sections);
    }
}