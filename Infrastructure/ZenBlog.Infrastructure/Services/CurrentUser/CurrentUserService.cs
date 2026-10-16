using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ZenBlog.Application.Contracts.Infrastructure;

namespace ZenBlog.Infrastructure.Services.CurrentUser;

public class CurrentUserService(IHttpContextAccessor _httpContextAccessor) : ICurrentUserService
{
    // Token doðrulandýktan sonra içindeki claim'ler buraya konur
    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            // Token'a "sub" claim'iyle kullanýcý Id'sini koymuþtuk.
            // ASP.NET Core bunu okurken ClaimTypes.NameIdentifier'a çeviriyor; ikisine de bakýyoruz.
            var id = User?.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? User?.FindFirstValue(JwtRegisteredClaimNames.Sub);

            return Guid.TryParse(id, out var userId) ? userId : null;
        }
    }

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public bool IsInRole(string role) => User?.IsInRole(role) ?? false;
}