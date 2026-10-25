using Microsoft.AspNetCore.Identity;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.LogHandlers;

// Loglardaki kullanýcý Id'lerini isimlere çevirir (TEK SQL sorgusuyla).
// Loglarda kiþisel veri yok (sadece Id); isim sadece admin ekranýnda gösterilir.
internal static class LogUserNames
{
    public static Dictionary<string, string> Resolve(UserManager<AppUser> userManager, IEnumerable<string?> ids)
    {
        var guids = ids
            .Where(id => Guid.TryParse(id, out _))
            .Select(id => Guid.Parse(id!))
            .Distinct()
            .ToList();

        if (guids.Count == 0)
            return new Dictionary<string, string>();

        return userManager.Users
            .Where(u => guids.Contains(u.Id))
            .Select(u => new { u.Id, FullName = u.Name + " " + u.Surname })
            .ToList()
            .ToDictionary(u => u.Id.ToString(), u => u.FullName);
    }
}