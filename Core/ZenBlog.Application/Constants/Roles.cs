namespace ZenBlog.Application.Constants;

// Sistemdeki rollerin TEK listesi. Kodda rol adý gerektiðinde hep buradan kullanýlýr.
public static class Roles
{
    public const string Admin = "Admin";
    public const string Writer = "Writer";
    public const string User = "User";

    public static readonly string[] All = [Admin, Writer, User];
}