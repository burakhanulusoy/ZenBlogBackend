namespace ZenBlog.Application.Features.Results.DashboardResults;

// ---------------------------------------------------------------
// ORTAK PARÇALAR
// ---------------------------------------------------------------

// Bir ölçü: toplam + seçilen dönem + ondan önceki ayný uzunluktaki dönem (deðiþim yüzdesi için)
public record StatValue(int Total, int InPeriod, int PreviousPeriod);

// Grafikteki bir nokta (gün ya da ay)
public record TrendPoint(string Key,      // "2026-10-24" (sýralama / benzersizlik)
                         string Label,    // "24 Eki" ya da "Eki 26" (eksende görünen)
                         int Views,
                         int Blogs,
                         int Comments,
                         int Users);

// Ad + sayý (kategori daðýlýmý, rol daðýlýmý)
public record NameCount(string Name, int Count);

// Listelerdeki blog
public record DashboardBlogItem(Guid Id,
                                string Title,
                                string CoverImage,
                                DateTime CreatedAt,
                                int Views,
                                int Comments,
                                string AuthorName);

// Listelerdeki yorum
public record DashboardCommentItem(Guid Id,
                                   bool IsReply,
                                   string Body,
                                   DateTime CreatedAt,
                                   string AuthorName,
                                   string? AuthorImage,
                                   Guid BlogId,
                                   string BlogTitle);

// ---------------------------------------------------------------
// ADMÝN
// ---------------------------------------------------------------
public record AdminPendingResult(int WriterApplications,   // açýk (bekleyen + incelenen) baþvurular
                                 int UnreadMessages,
                                 int PendingDeletions,      // kapanma sürecindeki hesaplar
                                 int BlockedUsers);

public record DashboardAuthorItem(Guid Id, string Name, string? ImageUrl, int Blogs, int Views);

public record DashboardUserItem(Guid Id, string Name, string? UserName, string? ImageUrl, DateTime CreatedAt);

public record AdminDashboardResult(string Period,
                                   StatValue Blogs,
                                   StatValue Users,
                                   StatValue Comments,
                                   StatValue Views,
                                   AdminPendingResult Pending,
                                   List<TrendPoint> Trend,
                                   List<NameCount> Categories,
                                   List<NameCount> Roles,
                                   List<DashboardBlogItem> TopBlogs,
                                   List<DashboardAuthorItem> TopAuthors,
                                   List<DashboardCommentItem> RecentComments,
                                   List<DashboardUserItem> NewUsers);

// ---------------------------------------------------------------
// YAZAR
// ---------------------------------------------------------------
public record WriterDashboardResult(string Period,
                                    StatValue Blogs,
                                    StatValue Views,
                                    StatValue Comments,          // bloglarýma BAÞKALARININ yazdýklarý
                                    List<TrendPoint> Trend,
                                    List<DashboardBlogItem> TopBlogs,
                                    List<DashboardCommentItem> RecentComments,
                                    List<NameCount> Categories,
                                    DateTime? LastPublishedAt);