using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Results.CommentResults;
using ZenBlog.Domain.Entities;
using ZenBlog.Persistence.Context;

namespace ZenBlog.Persistence.Concrete;

// =====================================================================
// YORUM OKUMA SORGULARI (projeksiyon)
// Yorumlar ve yanıtlar iki ayrı tabloda; panel sayfaları için ikisini
// ORTAK biçimde (CommentManageResult) çekip birleştiriyoruz.
// =====================================================================
public class CommentReadService(AppDbContext _context) : ICommentReadService
{
    // ---------------------------------------------------------------
    // BLOG DETAYI: yorumlar + yanıtlar (değişmedi)
    // ---------------------------------------------------------------
    public async Task<List<GetCommentsByBlogIdQueryResult>> GetByBlogIdAsync(Guid blogId, CancellationToken cancellationToken)
    {
        return await _context.Comments
            .AsNoTracking()
            .Where(c => c.BlogId == blogId)
            .OrderBy(c => c.CreatedAt)
            .Select(c => new GetCommentsByBlogIdQueryResult(
                c.Id, c.Body, c.CreatedAt, c.UpdatedAt, c.AppUserId,
                c.AppUser.Name + " " + c.AppUser.Surname, c.AppUser.ImageUrl,
                c.SubComments
                    .OrderBy(s => s.CreatedAt)
                    .Select(s => new CommentReplyResult(
                        s.Id, s.Body, s.CreatedAt, s.UpdatedAt, s.AppUserId,
                        s.AppUser.Name + " " + s.AppUser.Surname, s.AppUser.ImageUrl))
                    .ToList()))
            .ToListAsync(cancellationToken);
    }

    // ---------------------------------------------------------------
    // YENİ: PANEL SAYFALARI
    // ---------------------------------------------------------------
    public Task<List<CommentManageResult>> GetByAuthorAsync(Guid userId, CancellationToken cancellationToken) =>
        MergeAsync(c => c.AppUserId == userId,
                   s => s.AppUserId == userId,
                   take: null, cancellationToken);

    // Kendi bloguna kendi yazdığı yorumlar hariç (onlar "Yorumlarım"da)
    public Task<List<CommentManageResult>> GetOnBlogsOfAsync(Guid blogAuthorId, CancellationToken cancellationToken) =>
        MergeAsync(c => c.Blog.AppUserId == blogAuthorId && c.AppUserId != blogAuthorId,
                   s => s.Comment.Blog.AppUserId == blogAuthorId && s.AppUserId != blogAuthorId,
                   take: null, cancellationToken);

    public Task<List<CommentManageResult>> GetAllAsync(int take, CancellationToken cancellationToken) =>
        MergeAsync(c => true, s => true, take, cancellationToken);

    // Yorumları ve yanıtları aynı biçimde çek, birleştir, en yeniden eskiye sırala
    private async Task<List<CommentManageResult>> MergeAsync(Expression<Func<Comment, bool>> commentFilter,
                                                             Expression<Func<SubComment, bool>> replyFilter,
                                                             int? take,
                                                             CancellationToken cancellationToken)
    {
        var commentsQuery = _context.Comments
            .AsNoTracking()
            .Where(commentFilter)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CommentManageResult(
                c.Id, false, c.Body, c.CreatedAt, c.UpdatedAt,
                c.AppUserId, c.AppUser.Name + " " + c.AppUser.Surname, c.AppUser.ImageUrl,
                c.BlogId, c.Blog.MainTitle, c.Blog.AppUserId,
                null,
                c.SubComments.Count));

        var repliesQuery = _context.SubComments
            .AsNoTracking()
            .Where(replyFilter)
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new CommentManageResult(
                s.Id, true, s.Body, s.CreatedAt, s.UpdatedAt,
                s.AppUserId, s.AppUser.Name + " " + s.AppUser.Surname, s.AppUser.ImageUrl,
                s.Comment.BlogId, s.Comment.Blog.MainTitle, s.Comment.Blog.AppUserId,
                s.Comment.AppUser.Name + " " + s.Comment.AppUser.Surname,
                0));

        // Sınır varsa her iki sorgu da veritabanında sınırlanır (gereksiz satır gelmesin)
        if (take.HasValue)
        {
            commentsQuery = commentsQuery.Take(take.Value);
            repliesQuery = repliesQuery.Take(take.Value);
        }

        var comments = await commentsQuery.ToListAsync(cancellationToken);
        var replies = await repliesQuery.ToListAsync(cancellationToken);

        var merged = comments.Concat(replies).OrderByDescending(x => x.CreatedAt);
        return (take.HasValue ? merged.Take(take.Value) : merged).ToList();
    }
}