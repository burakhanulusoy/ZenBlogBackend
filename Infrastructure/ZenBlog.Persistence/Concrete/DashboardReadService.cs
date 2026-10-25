using System.Globalization;
using Microsoft.EntityFrameworkCore;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Results.DashboardResults;
using ZenBlog.Domain.Entities;
using ZenBlog.Domain.Enums;
using ZenBlog.Persistence.Context;

namespace ZenBlog.Persistence.Concrete;

// =====================================================================
// DASHBOARD SORGULARI
// Her ölçü için veritabanýnda GÜN GÜN gruplanmýþ sayým yapýlýr (GROUP BY tarih).
// 12 ayda bile en fazla ~730 satýrlýk özet gelir; binlerce kayýt belleðe çekilmez.
// Grafik noktalarý, dönem toplamý ve önceki dönem bu özetten bellekte hesaplanýr.
// =====================================================================
public class DashboardReadService(AppDbContext _context) : IDashboardReadService
{
    private static readonly CultureInfo Turkish = new("tr-TR");
    private const int ListSize = 5;

    // Seçilen dönem: baþlangýç, önceki dönemin baþlangýcý, aylýk mý, grafik noktalarý
    private sealed record PeriodRange(DateTime Start, DateTime PreviousStart, bool Monthly, List<DateTime> Buckets);

    // Bir ölçünün gün gün sayýmlarý + tüm zamanlarýn toplamý
    private sealed record Series(Dictionary<DateOnly, int> Daily, int Total);

    // =================================================================
    // ADMÝN
    // =================================================================
    public async Task<AdminDashboardResult> GetAdminAsync(string period, CancellationToken ct)
    {
        var range = GetRange(period);
        var from = range.PreviousStart;   // önceki dönemi de kapsasýn

        // 1) ÖLÇÜLER (gün gün + toplam)
        var blogs = new Series(
            await DailyAsync(_context.Blogs.Select(b => b.CreatedAt), from, ct),
            await _context.Blogs.CountAsync(ct));

        var users = new Series(
            await DailyAsync(_context.Users.Select(u => u.CreatedAt), from, ct),
            await _context.Users.CountAsync(ct));

        var comments = new Series(
            Merge(await DailyAsync(_context.Comments.Select(c => c.CreatedAt), from, ct),
                  await DailyAsync(_context.SubComments.Select(s => s.CreatedAt), from, ct)),
            await _context.Comments.CountAsync(ct) + await _context.SubComments.CountAsync(ct));

        var views = new Series(
            await ViewsDailyAsync(_context.BlogViews, DateOnly.FromDateTime(from), ct),
            await _context.Blogs.SumAsync(b => b.ViewCount, ct));

        // 2) SENÝ BEKLEYENLER
        var pending = new AdminPendingResult(
            await _context.WriterApplications.CountAsync(
                a => a.Status == WriterApplicationStatus.Pending || a.Status == WriterApplicationStatus.Reviewed, ct),
            await _context.ContactMessages.CountAsync(m => m.ContactMessageStatus == ContactMessageStatus.Pending, ct),
            await _context.Users.CountAsync(u => u.AccountStatus == AccountStatus.PendingDeletion, ct),
            await _context.Users.CountAsync(u => u.AccountStatus == AccountStatus.Blocked, ct));

        // 3) DAÐILIMLAR
        var categories = await _context.Categories
            .AsNoTracking()
            .Where(c => c.Blogs.Any())
            .OrderByDescending(c => c.Blogs.Count)
            .Select(c => new NameCount(c.Name, c.Blogs.Count))
            .ToListAsync(ct);

        // Rol baþýna kullanýcý sayýsý (AspNetUserRoles + AspNetRoles)
        var roles = await (from userRole in _context.UserRoles
                           join role in _context.Roles on userRole.RoleId equals role.Id
                           group userRole by role.Name into g
                           select new NameCount(g.Key!, g.Count()))
                          .ToListAsync(ct);

        // 4) LÝSTELER
        var topBlogs = await TopBlogsAsync(_context.Blogs, ct);

        var topAuthors = await _context.Users
            .AsNoTracking()
            .Where(u => u.Blogs.Any())
            .OrderByDescending(u => u.Blogs.Count)
            .ThenByDescending(u => u.Blogs.Sum(b => b.ViewCount))
            .Take(ListSize)
            .Select(u => new DashboardAuthorItem(
                u.Id, u.Name + " " + u.Surname, u.ImageUrl,
                u.Blogs.Count, u.Blogs.Sum(b => b.ViewCount)))
            .ToListAsync(ct);

        var recentComments = await RecentCommentsAsync(blogAuthorId: null, ct);

        var newUsers = await _context.Users
            .AsNoTracking()
            .OrderByDescending(u => u.CreatedAt)
            .Take(ListSize)
            .Select(u => new DashboardUserItem(u.Id, u.Name + " " + u.Surname, u.UserName, u.ImageUrl, u.CreatedAt))
            .ToListAsync(ct);

        return new AdminDashboardResult(
            period,
            ToStat(blogs, range), ToStat(users, range), ToStat(comments, range), ToStat(views, range),
            pending,
            BuildTrend(range, views, blogs, comments, users),
            categories, roles, topBlogs, topAuthors, recentComments, newUsers);
    }

    // =================================================================
    // YAZAR
    // =================================================================
    public async Task<WriterDashboardResult> GetWriterAsync(Guid writerId, string period, CancellationToken ct)
    {
        var range = GetRange(period);
        var from = range.PreviousStart;

        var myBlogs = _context.Blogs.Where(b => b.AppUserId == writerId);

        var blogs = new Series(
            await DailyAsync(myBlogs.Select(b => b.CreatedAt), from, ct),
            await myBlogs.CountAsync(ct));

        // Bloglarýma BAÞKALARININ yazdýðý yorum ve yanýtlar
        var commentsOnMine = _context.Comments.Where(c => c.Blog.AppUserId == writerId && c.AppUserId != writerId);
        var repliesOnMine = _context.SubComments.Where(s => s.Comment.Blog.AppUserId == writerId && s.AppUserId != writerId);

        var comments = new Series(
            Merge(await DailyAsync(commentsOnMine.Select(c => c.CreatedAt), from, ct),
                  await DailyAsync(repliesOnMine.Select(s => s.CreatedAt), from, ct)),
            await commentsOnMine.CountAsync(ct) + await repliesOnMine.CountAsync(ct));

        var views = new Series(
            await ViewsDailyAsync(_context.BlogViews.Where(v => v.Blog.AppUserId == writerId), DateOnly.FromDateTime(from), ct),
            await myBlogs.SumAsync(b => b.ViewCount, ct));

        var topBlogs = await TopBlogsAsync(myBlogs, ct);
        var recentComments = await RecentCommentsAsync(writerId, ct);

        var categories = await myBlogs
            .GroupBy(b => b.Category.Name)
            .Select(g => new NameCount(g.Key, g.Count()))
            .ToListAsync(ct);

        var lastPublishedAt = await myBlogs
            .Select(b => (DateTime?)b.CreatedAt)
            .MaxAsync(ct);

        return new WriterDashboardResult(
            period,
            ToStat(blogs, range), ToStat(views, range), ToStat(comments, range),
            BuildTrend(range, views, blogs, comments, users: null),
            topBlogs, recentComments,
            categories.OrderByDescending(c => c.Count).ToList(),
            lastPublishedAt);
    }

    // =================================================================
    // YARDIMCILAR
    // =================================================================

    // Dönem aralýðý ve grafik noktalarý
    private static PeriodRange GetRange(string period)
    {
        var today = DateTime.Today;

        switch (period)
        {
            case DashboardPeriods.Week:
                {
                    var start = today.AddDays(-6);   // bugün dahil 7 gün
                    return new PeriodRange(start, start.AddDays(-7), false,
                        Enumerable.Range(0, 7).Select(i => start.AddDays(i)).ToList());
                }
            case DashboardPeriods.Year:
                {
                    var start = new DateTime(today.Year, today.Month, 1).AddMonths(-11);   // bu ay dahil 12 ay
                    return new PeriodRange(start, start.AddMonths(-12), true,
                        Enumerable.Range(0, 12).Select(i => start.AddMonths(i)).ToList());
                }
            default:
                {
                    var start = today.AddDays(-29);   // bugün dahil 30 gün
                    return new PeriodRange(start, start.AddDays(-30), false,
                        Enumerable.Range(0, 30).Select(i => start.AddDays(i)).ToList());
                }
        }
    }

    // Tarih listesini veritabanýnda GÜN GÜN say: SELECT CAST(tarih AS date), COUNT(*) ... GROUP BY ...
    private static async Task<Dictionary<DateOnly, int>> DailyAsync(IQueryable<DateTime> dates, DateTime from, CancellationToken ct)
    {
        var rows = await dates
            .Where(d => d >= from)
            .GroupBy(d => d.Date)
            .Select(g => new { Day = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        return rows.ToDictionary(r => DateOnly.FromDateTime(r.Day), r => r.Count);
    }

    // Okunmalar zaten gün olarak tutuluyor (ViewDate)
    private static async Task<Dictionary<DateOnly, int>> ViewsDailyAsync(IQueryable<BlogView> views, DateOnly from, CancellationToken ct)
    {
        var rows = await views
            .Where(v => v.ViewDate >= from)
            .GroupBy(v => v.ViewDate)
            .Select(g => new { Day = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        return rows.ToDictionary(r => r.Day, r => r.Count);
    }

    // Ýki günlük sayýmý topla (yorumlar + yanýtlar)
    private static Dictionary<DateOnly, int> Merge(Dictionary<DateOnly, int> a, Dictionary<DateOnly, int> b)
    {
        var result = new Dictionary<DateOnly, int>(a);
        foreach (var (day, count) in b)
            result[day] = result.GetValueOrDefault(day) + count;
        return result;
    }

    // Toplam + bu dönem + önceki dönem
    private static StatValue ToStat(Series series, PeriodRange range)
    {
        var start = DateOnly.FromDateTime(range.Start);
        var inPeriod = series.Daily.Where(x => x.Key >= start).Sum(x => x.Value);
        var previous = series.Daily.Where(x => x.Key < start).Sum(x => x.Value);
        return new StatValue(series.Total, inPeriod, previous);
    }

    // Grafik noktalarý: günlük ya da aylýk; boþ günler/aylar 0
    private static List<TrendPoint> BuildTrend(PeriodRange range, Series views, Series blogs, Series comments, Series? users)
    {
        int Bucket(Dictionary<DateOnly, int> daily, DateTime bucket) => range.Monthly
            ? daily.Where(x => x.Key.Year == bucket.Year && x.Key.Month == bucket.Month).Sum(x => x.Value)
            : daily.GetValueOrDefault(DateOnly.FromDateTime(bucket));

        return range.Buckets.Select(b => new TrendPoint(
                b.ToString("yyyy-MM-dd"),
                range.Monthly ? b.ToString("MMM yy", Turkish) : b.ToString("d MMM", Turkish),
                Bucket(views.Daily, b),
                Bucket(blogs.Daily, b),
                Bucket(comments.Daily, b),
                users is null ? 0 : Bucket(users.Daily, b)))
            .ToList();
    }

    // En çok okunan bloglar (eþitse en yeni)
    private static Task<List<DashboardBlogItem>> TopBlogsAsync(IQueryable<Blog> source, CancellationToken ct) =>
        source
            .AsNoTracking()
            .OrderByDescending(b => b.ViewCount)
            .ThenByDescending(b => b.CreatedAt)
            .Take(ListSize)
            .Select(b => new DashboardBlogItem(
                b.Id, b.MainTitle, b.CoverImage, b.CreatedAt, b.ViewCount,
                b.Comments.Count + b.Comments.SelectMany(c => c.SubComments).Count(),
                b.AppUser.Name + " " + b.AppUser.Surname))
            .ToListAsync(ct);

    // Son yorumlar (yorum + yanýt karýþýk). blogAuthorId verilirse: o yazarýn bloglarýna BAÞKALARININ yazdýklarý
    private async Task<List<DashboardCommentItem>> RecentCommentsAsync(Guid? blogAuthorId, CancellationToken ct)
    {
        var commentQuery = _context.Comments.AsNoTracking();
        var replyQuery = _context.SubComments.AsNoTracking();

        if (blogAuthorId is Guid authorId)
        {
            commentQuery = commentQuery.Where(c => c.Blog.AppUserId == authorId && c.AppUserId != authorId);
            replyQuery = replyQuery.Where(s => s.Comment.Blog.AppUserId == authorId && s.AppUserId != authorId);
        }

        var commentItems = await commentQuery
            .OrderByDescending(c => c.CreatedAt)
            .Take(ListSize)
            .Select(c => new DashboardCommentItem(
                c.Id, false, c.Body, c.CreatedAt,
                c.AppUser.Name + " " + c.AppUser.Surname, c.AppUser.ImageUrl,
                c.BlogId, c.Blog.MainTitle))
            .ToListAsync(ct);

        var replyItems = await replyQuery
            .OrderByDescending(s => s.CreatedAt)
            .Take(ListSize)
            .Select(s => new DashboardCommentItem(
                s.Id, true, s.Body, s.CreatedAt,
                s.AppUser.Name + " " + s.AppUser.Surname, s.AppUser.ImageUrl,
                s.Comment.BlogId, s.Comment.Blog.MainTitle))
            .ToListAsync(ct);

        return commentItems.Concat(replyItems)
            .OrderByDescending(x => x.CreatedAt)
            .Take(ListSize)
            .ToList();
    }
}