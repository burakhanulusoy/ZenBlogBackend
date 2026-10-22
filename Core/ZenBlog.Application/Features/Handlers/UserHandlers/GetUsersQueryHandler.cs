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

public class GetUsersQueryHandler(UserManager<AppUser> _userManager,
                                  RoleManager<AppRole> _roleManager,
                                  IRepository<UserBlock> _blockRepository)
    : IRequestHandler<GetUsersQuery, BaseResult<List<GetUserListQueryResult>>>
{
    public async Task<BaseResult<List<GetUserListQueryResult>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        // 1) Kullanýcý Id ? rolleri sözlüðü (C#: Dictionary)
        //    Her kullanýcý için ayrý GetRolesAsync çaðýrsaydýk N kullanýcýda N sorgu olurdu;
        //    rol baþýna bir sorgu (3 rol = 3 sorgu) çok daha hafif.
        var userRoles = new Dictionary<Guid, List<string>>();

        var roleNames = _roleManager.Roles.Select(r => r.Name!).ToList();
        foreach (var roleName in roleNames)
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
            foreach (var u in usersInRole)
            {
                if (!userRoles.TryGetValue(u.Id, out var list))
                {
                    list = new List<string>();
                    userRoles[u.Id] = list;
                }
                list.Add(roleName);
            }
        }

        // 2) Her kullanýcýnýn EN SON bloke tarihi (kullanýcý Id ? tarih)
        var blocks = await _blockRepository.GetAllAsync();
        var lastBlockedAt = blocks
            .GroupBy(b => b.UserId)
            .ToDictionary(g => g.Key, g => g.Max(b => b.BlockedAt));

        // 3) Kullanýcýlar + yayýndaki blog sayýlarý tek sorguda
        //    u.Blogs.Count: SQL'de alt sorguya çevrilir; soft delete filtresi burada da geçerli
        var users = _userManager.Users
            .OrderBy(u => u.Name).ThenBy(u => u.Surname)
            .Select(u => new
            {
                u.Id,
                u.Name,
                u.Surname,
                u.UserName,
                u.Email,
                u.ImageUrl,
                u.EmailConfirmed,
                u.AccountStatus,
                u.DeletionRequestedAt,
                BlogCount = u.Blogs.Count
            })
            .ToList();

        // 4) Hepsini birleþtir
        var result = users.Select(u => new GetUserListQueryResult(
                u.Id, u.Name, u.Surname, u.UserName, u.Email, u.ImageUrl, u.EmailConfirmed,
                userRoles.TryGetValue(u.Id, out var roles) ? roles : new List<string>(),
                u.AccountStatus.ToString(),
                u.AccountStatus == AccountStatus.PendingDeletion
                    ? u.DeletionRequestedAt?.AddDays(AccountRules.DeletionGraceDays)
                    : null,
                u.BlogCount,
                lastBlockedAt.TryGetValue(u.Id, out var blockedAt) ? blockedAt : null))
            .ToList();

        return BaseResult<List<GetUserListQueryResult>>.Success(result);
    }
}