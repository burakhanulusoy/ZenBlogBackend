namespace ZenBlog.Infrastructure.Services.Turnstile;

// secrets.json'daki "Turnstile" bölümü
public class TurnstileSettings
{
    public string SecretKey { get; set; } = string.Empty;
}