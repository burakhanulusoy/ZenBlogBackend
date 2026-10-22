using Microsoft.EntityFrameworkCore;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Domain.Enums;
using ZenBlog.Persistence.Context;

namespace ZenBlog.Persistence.Concrete;

// Kullanýcýnýn içeriklerini topluca gizler / geri açar.
//
// ExecuteUpdateAsync: kayýtlarý belleðe çekmeden doðrudan SQL "UPDATE ... SET ... WHERE ..." çalýþtýrýr.
// Yüzlerce yorum olsa bile tek sorgu. SaveChanges'e uðramadýðý için UpdatedAt'i kendimiz yazýyoruz.
//
// IgnoreQueryFilters: "IsDeleted = false" otomatik filtresini kapatýr. Geri açarken zaten gizli
// (IsDeleted = 1) kayýtlarý bulmamýz gerekiyor; gizlerken de iliþkili tablolarýn filtresi araya girmesin.
public class AccountContentService(AppDbContext _context) : IAccountContentService
{
    public async Task HideContentAsync(Guid userId, CancellationToken cancellationToken)
    {
        var now = DateTime.Now;

        // Hepsi ya birlikte olsun ya hiç: biri hata verirse diðerleri de geri alýnýr
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        // 1) Alt yorumlar: kiþinin yazdýklarý + kiþinin yorumlarýna gelenler + kiþinin bloglarýndaki yorumlara gelenler
        await _context.SubComments.IgnoreQueryFilters()
            .Where(s => !s.IsDeleted &&
                        (s.AppUserId == userId ||
                         s.Comment.AppUserId == userId ||
                         s.Comment.Blog.AppUserId == userId))
            .ExecuteUpdateAsync(set => set
                .SetProperty(s => s.IsDeleted, true)
                .SetProperty(s => s.HiddenByAccount, true)
                .SetProperty(s => s.UpdatedAt, now), cancellationToken);

        // 2) Yorumlar: kiþinin yazdýklarý + baþkalarýnýn onun bloglarýna yazdýklarý
        await _context.Comments.IgnoreQueryFilters()
            .Where(c => !c.IsDeleted &&
                        (c.AppUserId == userId ||
                         c.Blog.AppUserId == userId))
            .ExecuteUpdateAsync(set => set
                .SetProperty(c => c.IsDeleted, true)
                .SetProperty(c => c.HiddenByAccount, true)
                .SetProperty(c => c.UpdatedAt, now), cancellationToken);

        // 3) Bloglar
        await _context.Blogs.IgnoreQueryFilters()
            .Where(b => !b.IsDeleted && b.AppUserId == userId)
            .ExecuteUpdateAsync(set => set
                .SetProperty(b => b.IsDeleted, true)
                .SetProperty(b => b.HiddenByAccount, true)
                .SetProperty(b => b.UpdatedAt, now), cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }

    public async Task RestoreContentAsync(Guid userId, CancellationToken cancellationToken)
    {
        var now = DateTime.Now;
        var active = AccountStatus.Active;

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        // 1) Bloglar (sadece hesap iþlemiyle gizlenenler)
        await _context.Blogs.IgnoreQueryFilters()
            .Where(b => b.HiddenByAccount && b.AppUserId == userId)
            .ExecuteUpdateAsync(set => set
                .SetProperty(b => b.IsDeleted, false)
                .SetProperty(b => b.HiddenByAccount, false)
                .SetProperty(b => b.UpdatedAt, now), cancellationToken);

        // 2) Yorumlar: yorumu yazan da, blogun sahibi de AKTÝF olmalý.
        //    Örn. A'nýn bloguna B yorum yazmýþ; B de bloke ise A geri dönünce B'nin yorumu gizli kalýr.
        await _context.Comments.IgnoreQueryFilters()
            .Where(c => c.HiddenByAccount &&
                        (c.AppUserId == userId || c.Blog.AppUserId == userId) &&
                        c.AppUser.AccountStatus == active &&
                        c.Blog.AppUser.AccountStatus == active)
            .ExecuteUpdateAsync(set => set
                .SetProperty(c => c.IsDeleted, false)
                .SetProperty(c => c.HiddenByAccount, false)
                .SetProperty(c => c.UpdatedAt, now), cancellationToken);

        // 3) Alt yorumlar: yazaný, baðlý olduðu yorumun sahibi ve blogun sahibi AKTÝF olmalý
        await _context.SubComments.IgnoreQueryFilters()
            .Where(s => s.HiddenByAccount &&
                        (s.AppUserId == userId ||
                         s.Comment.AppUserId == userId ||
                         s.Comment.Blog.AppUserId == userId) &&
                        s.AppUser.AccountStatus == active &&
                        s.Comment.AppUser.AccountStatus == active &&
                        s.Comment.Blog.AppUser.AccountStatus == active)
            .ExecuteUpdateAsync(set => set
                .SetProperty(s => s.IsDeleted, false)
                .SetProperty(s => s.HiddenByAccount, false)
                .SetProperty(s => s.UpdatedAt, now), cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }
}