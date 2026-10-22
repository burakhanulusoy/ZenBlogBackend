using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Application.Features.Results.UserResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Infrastructure.Services.Jwt;

public class JwtService(UserManager<AppUser> _userManager,
                        IOptions<JwtTokenOptions> options) : IJwtService
{
    private readonly JwtTokenOptions _options = options.Value;

    public async Task<LoginCommandResult> GenerateTokenAsync(AppUser user)
    {
        var now = DateTime.UtcNow;
        var expiresAt = now.AddMinutes(_options.ExpireInMinutes);

        // 1) CLAIM'LER: token'ýn taþýyacaðý bilgiler.
        //    Herkes okuyabildiði için sadece gerekli ve gizli olmayan bilgiler konur.
        var claims = new List<Claim>
        {
            // sub (subject): token kimin için. Kullanýcý Id'si.
            // ASP.NET Core bunu otomatik olarak ClaimTypes.NameIdentifier'a çevirir;
            // ileride "giriþ yapan kullanýcý kim?" sorusunu buradan cevaplayacaðýz.
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),

            new(JwtRegisteredClaimNames.Email, user.Email!),

            // jti: her token'a benzersiz bir kimlik. Ýleride token iptali/izleme için kullanýlabilir.
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            // Frontend'in "Hoþ geldin Ali Yýlmaz" yazabilmesi için
            new("fullName", $"{user.Name} {user.Surname}")
        };

        // YENÝ: Profil fotoðrafý (varsa). Frontend sað üstteki avatarda gösteriyor.
        // Profil güncellenince yeni token üretildiði için fotoðraf deðiþince bu da güncellenir.
        if (!string.IsNullOrWhiteSpace(user.ImageUrl))
            claims.Add(new Claim("imageUrl", user.ImageUrl));

        // YENÝ: Hesap damgasý (SecurityStamp). Rol deðiþince, þifre sýfýrlanýnca vb. Identity bunu yeniler.
        // Her istekte token'daki damga veritabanýndakiyle karþýlaþtýrýlýr; farklýysa token reddedilir
        // ? kiþinin oturumu düþer, yeniden girince güncel rolleriyle yeni token alýr.
        claims.Add(new Claim("stamp", user.SecurityStamp ?? string.Empty));

        // 2) ROLLER: her rol ayrý bir claim olarak eklenir.
        //    RequireRole("Admin") gibi kurallar bu claim'lere bakarak karar verir.
        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        // 3) ÝMZA: Key ile HS256 algoritmasýyla imzalanýr.
        //    Payload'ý deðiþtiren biri geçerli imza üretemez, çünkü Key'i bilmez.
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 4) TOKEN'I OLUÞTUR
        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now,              // bu andan önce geçersiz
            expires: expiresAt,          // bu andan sonra geçersiz
            signingCredentials: credentials);

        // 5) Token nesnesini "eyJhbGciOi..." þeklindeki metne çevir
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new LoginCommandResult(tokenString, expiresAt);
    }
}