namespace ZenBlog.Infrastructure.Services.Jwt;

// secrets.json'daki "JwtTokenOptions" bölümü bu sýnýfa doldurulur.
public class JwtTokenOptions
{
    // Token'ý yayýnlayan taraf (API). Doðrulamada token'daki deðerle karþýlaþtýrýlýr.
    public string Issuer { get; set; } = string.Empty;

    // Token'ýn üretildiði taraf (frontend). Doðrulamada karþýlaþtýrýlýr.
    public string Audience { get; set; } = string.Empty;

    // Ýmza anahtarý. HS256 için en az 32 karakter. Gizli tutulmalý.
    public string Key { get; set; } = string.Empty;

    // Token kaç dakika geçerli olacak
    public int ExpireInMinutes { get; set; }
}