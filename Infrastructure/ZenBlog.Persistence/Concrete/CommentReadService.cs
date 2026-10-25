using Microsoft.EntityFrameworkCore;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Results.CommentResults;
using ZenBlog.Persistence.Context;

namespace ZenBlog.Persistence.Concrete;

// =====================================================================
// BLOG YORUMLARI (projeksiyon)
// Yorumlar + yazarları + yanıtları + yanıtların yazarları → TEK SQL sorgusu.
// Eski yöntemde (AsNoTracking + AutoMapper) her yorumun yazarı ayrı yüklenmeye çalışılıyordu.
// Soft delete filtresi burada da geçerli: silinmiş ya da hesabı gizlenmiş kişilerin yorumları gelmez.
// =====================================================================
public class CommentReadService(AppDbContext _context) : ICommentReadService
{
    public async Task<List<GetCommentsByBlogIdQueryResult>> GetByBlogIdAsync(Guid blogId, CancellationToken cancellationToken)
    {
        return await _context.Comments
            .AsNoTracking()
            .Where(c => c.BlogId == blogId)
            .OrderBy(c => c.CreatedAt)   // eskiden yeniye (sohbet sırası)
            .Select(c => new GetCommentsByBlogIdQueryResult(
                c.Id,
                c.Body,
                c.CreatedAt,
                c.UpdatedAt,
                c.AppUserId,
                c.AppUser.Name + " " + c.AppUser.Surname,
                c.AppUser.ImageUrl,
                c.SubComments
                    .OrderBy(s => s.CreatedAt)
                    .Select(s => new CommentReplyResult(
                        s.Id,
                        s.Body,
                        s.CreatedAt,
                        s.UpdatedAt,
                        s.AppUserId,
                        s.AppUser.Name + " " + s.AppUser.Surname,
                        s.AppUser.ImageUrl))
                    .ToList()))
            .ToListAsync(cancellationToken);
    }
}