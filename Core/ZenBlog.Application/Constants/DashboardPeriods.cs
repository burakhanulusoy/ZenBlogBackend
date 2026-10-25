namespace ZenBlog.Application.Constants;

// Dashboard dönemleri
public static class DashboardPeriods
{
    public const string Week = "7d";
    public const string Month = "30d";
    public const string Year = "12m";

    // Tanýnmayan deðer ? 30 gün
    public static string Normalize(string? period) =>
        period is Week or Month or Year ? period : Month;
}