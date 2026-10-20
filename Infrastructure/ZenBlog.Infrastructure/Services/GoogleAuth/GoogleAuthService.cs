using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using ZenBlog.Application.Contracts.Infrastructure;

namespace ZenBlog.Infrastructure.Services.GoogleAuth;

public class GoogleAuthService(IOptions<GoogleAuthSettings> options) : IGoogleAuthService
{
    private readonly GoogleAuthSettings _settings = options.Value;

    public async Task<GoogleUserInfo?> ValidateIdTokenAsync(string idToken)
    {
        try
        {
            // Google'ýn kütüphanesi þunlarý kontrol ediyor:
            //  - Token Google'ýn gizli anahtarýyla imzalanmýþ mý? (sahte deðil mi?)
            //  - Süresi dolmuþ mu?
            //  - Audience: token BÝZÝM Client ID'miz için mi üretilmiþ?
            //    (Baþka bir sitenin Google token'ýyla bizim sitemize girilemesin diye kritik)
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _settings.ClientId }
                });

            return new GoogleUserInfo(payload.Subject,
                                      payload.Email,
                                      payload.EmailVerified,
                                      payload.GivenName,
                                      payload.FamilyName,
                                      payload.Picture);
        }
        catch (InvalidJwtException)
        {
            // Ýmza, süre ya da audience tutmuyor ? geçersiz token
            return null;
        }
    }
}