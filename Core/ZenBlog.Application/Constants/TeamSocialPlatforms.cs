namespace ZenBlog.Application.Constants;

// Ekip üyesinin seçebileceði sosyal platformlar ve ikonlarý.
// Ýkon istekten DEÐÝL buradan gelir ? kimse rastgele bir ikon gönderemez.
// Yeni platform eklemek için bu listeye bir satýr eklemek yeterli.
// Deðerler PrimeIcons sýnýflarý (frontend <i class="pi pi-linkedin"> ile çiziyor).
public static class TeamSocialPlatforms
{
    public static readonly IReadOnlyDictionary<string, string> Icons = new Dictionary<string, string>
    {
        ["Instagram"] = "pi pi-instagram",
        ["X"] = "pi pi-twitter",
        ["LinkedIn"] = "pi pi-linkedin",
        ["GitHub"] = "pi pi-github",
        ["Facebook"] = "pi pi-facebook",
        ["YouTube"] = "pi pi-youtube",
        ["TikTok"] = "pi pi-tiktok",
        ["Website"] = "pi pi-globe"
    };

    public static bool IsValid(string? platform) =>
        platform is not null && Icons.ContainsKey(platform);
}