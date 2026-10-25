using Microsoft.EntityFrameworkCore;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Domain.Entities;
using ZenBlog.Persistence.Context;

namespace ZenBlog.Persistence.Concrete;

// =====================================================================
// OKUNMA SAYACI (günlük tekil)
// 1) Bu ziyaretçi bugün bu blogu okudu mu? → okuduysa sayma
// 2) Okumadıysa kaydet + bloğun toplamını 1 artır
// Aynı anda iki istek gelirse benzersiz index ikincisini reddeder (DbUpdateException) → sessizce geç.
// =====================================================================
public class BlogViewService(AppDbContext _context) : IBlogViewService
{
    public async Task<bool> RecordAsync(Guid blogId, string visitorKey, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);

        var alreadyViewed = await _context.BlogViews.AnyAsync(
            v => v.BlogId == blogId && v.VisitorKey == visitorKey && v.ViewDate == today,
            cancellationToken);

        if (alreadyViewed)
            return false;

        _context.BlogViews.Add(new BlogView
        {
            BlogId = blogId,
            VisitorKey = visitorKey,
            ViewDate = today,
            CreatedAt = DateTime.Now
        });

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return false;   // aynı anda gelen ikinci istek: zaten sayıldı
        }

        // Toplamı tek SQL ile artır: UPDATE Blogs SET ViewCount = ViewCount + 1 WHERE Id = ...
        // (önce okuyup sonra yazmak yerine; aynı anda gelen okunmalar birbirini ezmez)
        await _context.Blogs
            .Where(b => b.Id == blogId)
            .ExecuteUpdateAsync(set => set.SetProperty(b => b.ViewCount, b => b.ViewCount + 1), cancellationToken);

        return true;
    }
}