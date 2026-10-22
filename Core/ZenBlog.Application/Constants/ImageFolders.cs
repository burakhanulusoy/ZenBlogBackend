namespace ZenBlog.Application.Constants;

// Görsel yüklenebilecek klasörlerin TEK listesi.
// Yeni klasör lazým olunca sadece buraya bir satýr eklemen yeterli.
public static class ImageFolders
{
    public const string Blogs = "blogs";
    public const string Users = "users";
    public const string Categories = "categories";
    public const string Teams = "teams";
    public const string Abouts = "abouts";
    public const string Blocks = "blocks";   //blog gerekçeli 

    public static readonly string[] All = [Blogs, Users, Categories, Teams, Abouts,Blocks];
}