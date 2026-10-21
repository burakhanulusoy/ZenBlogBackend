using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using ZenBlog.Application.Contracts.Infrastructure;

namespace ZenBlog.Infrastructure.Services.Turnstile;

// HttpClient'ý DI veriyor (AddHttpClient ile kaydediyoruz; doðru HttpClient kullanýmý budur,
// her istekte "new HttpClient()" yapmak baðlantýlarý tüketir)
public class TurnstileService(HttpClient _httpClient,
                              IOptions<TurnstileSettings> options) : ITurnstileService
{
    private const string VerifyUrl = "https://challenges.cloudflare.com/turnstile/v0/siteverify";

    private readonly TurnstileSettings _settings = options.Value;

    public async Task<bool> ValidateAsync(string? token, string? remoteIp)
    {
        // Token hiç gelmediyse Cloudflare'e sormaya gerek yok
        if (string.IsNullOrWhiteSpace(token))
            return false;

        // Cloudflare form formatýnda istiyor: secret + response (+ remoteip)
        var form = new Dictionary<string, string>
        {
            ["secret"] = _settings.SecretKey,
            ["response"] = token
        };

        if (!string.IsNullOrEmpty(remoteIp))
            form["remoteip"] = remoteIp;

        try
        {
            using var response = await _httpClient.PostAsync(VerifyUrl, new FormUrlEncodedContent(form));

            if (!response.IsSuccessStatusCode)
                return false;

            var result = await response.Content.ReadFromJsonAsync<TurnstileVerifyResponse>();
            return result?.Success == true;
        }
        catch (HttpRequestException)
        {
            // Cloudflare'e ulaþýlamadý ? güvenli tarafta kalýp REDDEDÝYORUZ.
            // (Tersini seçseydik, Cloudflare eriþilemezken botlar da geçebilirdi.)
            return false;
        }
    }

    // Cloudflare'in cevabýndan sadece ihtiyacýmýz olan alan
    private record TurnstileVerifyResponse([property: JsonPropertyName("success")] bool Success);
}