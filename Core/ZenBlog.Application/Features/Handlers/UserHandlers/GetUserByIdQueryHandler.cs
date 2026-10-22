using MediatR;
using Microsoft.AspNetCore.Identity;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Queries.UserQueries;
using ZenBlog.Application.Features.Results.UserResults;
using ZenBlog.Domain.Entities;
using ZenBlog.Domain.Enums;

namespace ZenBlog.Application.Features.Handlers.UserHandlers;

public class GetUserByIdQueryHandler(UserManager<AppUser> _userManager,
                                     IRepository<Blog> _blogRepository,
                                     IRepository<Comment> _commentRepository,
                                     IRepository<UserBlock> _blockRepository)
    : IRequestHandler<GetUserByIdQuery, BaseResult<GetUserByIdQueryResult>>
{
    public async Task<BaseResult<GetUserByIdQueryResult>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString());
        if (user is null)
            return BaseResult<GetUserByIdQueryResult>.Fail("Kullanýcý bulunamadý.");

        var roles = await _userManager.GetRolesAsync(user);

        // Yayýndaki bloglar ve yorumlar (gizlenenler sayýlmaz)
        var blogs = await _blogRepository.GetAllFilterAsync(b => b.AppUserId == user.Id);
        var comments = await _commentRepository.GetAllFilterAsync(c => c.AppUserId == user.Id);

        // En yeni 5 blog (sadece basit alanlar)
        var recentBlogs = blogs
            .OrderByDescending(b => b.CreatedAt)
            .Take(5)
            .Select(b => new UserRecentBlogResult(b.Id, b.MainTitle, b.CoverImage, b.CreatedAt))
            .ToList();

        // BLOKE GEÇMÝÞÝ
        var blocks = (await _blockRepository.GetAllFilterAsync(b => b.UserId == user.Id))
            .OrderByDescending(b => b.BlockedAt)
            .ToList();

        // Kayýtlardaki admin Id'lerini ada çevir (tek sorgu)
        var adminIds = blocks.Select(b => b.BlockedByAdminId)
            .Concat(blocks.Where(b => b.UnblockedByAdminId.HasValue).Select(b => b.UnblockedByAdminId!.Value))
            .Distinct()
            .ToList();

        var adminNames = _userManager.Users
            .Where(u => adminIds.Contains(u.Id))
            .Select(u => new { u.Id, FullName = u.Name + " " + u.Surname })
            .ToList()
            .ToDictionary(a => a.Id, a => a.FullName);

        string NameOf(Guid? id) =>
            id.HasValue && adminNames.TryGetValue(id.Value, out var name) ? name : "Bilinmeyen yönetici";

        var blockHistory = blocks.Select(b => new UserBlockHistoryResult(
                b.Id, b.Reason, b.EvidenceUrl, b.BlockedAt,
                b.BlockedByAdminId, NameOf(b.BlockedByAdminId),
                b.UnblockedAt,
                b.UnblockedByAdminId.HasValue ? NameOf(b.UnblockedByAdminId) : null,
                b.UnblockReason))
            .ToList();

        var result = new GetUserByIdQueryResult(
            user.Id, user.Name, user.Surname, user.UserName, user.Email, user.PhoneNumber,
            user.ImageUrl, user.Location, user.Description,
            user.InstagramUrl, user.TwitterUrl, user.LinkedlnUrl, user.FacebookUrl,
            user.EmailConfirmed, roles.ToList(),
            user.AccountStatus.ToString(),
            user.DeletionRequestedAt,
            user.AccountStatus == AccountStatus.PendingDeletion
                ? user.DeletionRequestedAt?.AddDays(AccountRules.DeletionGraceDays)
                : null,
            blogs.Count, comments.Count, recentBlogs, blockHistory);

        return BaseResult<GetUserByIdQueryResult>.Success(result);
    }
}