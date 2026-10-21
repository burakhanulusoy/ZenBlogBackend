namespace ZenBlog.Application.Constants;

// Yetki kurallarýnýn (policy) adlarý
public static class Policies
{
    public const string AdminOnly = "AdminOnly";          // sadece Admin
    public const string WriterOrAdmin = "WriterOrAdmin";  // Writer veya Admin
}