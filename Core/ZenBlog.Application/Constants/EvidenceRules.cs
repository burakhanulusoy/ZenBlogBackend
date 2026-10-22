namespace ZenBlog.Application.Constants;

// Bloke kanýt dosyasý kurallarý (resim kurallarýndan farklý: PDF de olabilir, daha büyük olabilir)
public static class EvidenceRules
{
    public const long MaxFileSize = 10 * 1024 * 1024;   // 10 MB
    public static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp", ".pdf"];
}