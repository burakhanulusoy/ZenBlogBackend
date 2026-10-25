namespace ZenBlog.Application.Constants;

// Log sayfasýndaki dönemler: zaman aralýðý + grafik çubuðu geniþliði
public static class LogPeriods
{
    public static (DateTime From, DateTime To, string Interval) Resolve(string? period)
    {
        var now = DateTime.Now;
        return period switch
        {
            "1h" => (now.AddHours(-1), now, "1m"),     // 60 çubuk (dakikalýk)
            "7d" => (now.AddDays(-7), now, "6h"),      // 28 çubuk
            "30d" => (now.AddDays(-30), now, "1d"),    // 30 çubuk (günlük)
            _ => (now.AddHours(-24), now, "1h")        // varsayýlan: 24 saat, 24 çubuk
        };
    }
}