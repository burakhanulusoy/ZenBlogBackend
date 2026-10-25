using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Application.Features.Results.DashboardResults;
using ZenBlog.Application.Features.Results.LogResults;

namespace ZenBlog.Infrastructure.Services.Logging;

// =====================================================================
// ELASTICSEARCH LOG SORGULARI
// Elasticsearch bir REST API: JSON bir sorgu (Query DSL) gönderip JSON cevap alýyoruz.
//   POST /logs-zenblog-*/_search   { "query": {...}, "aggs": {...} }
// Hazýr .NET kütüphanesi yerine HttpClient + Query DSL: sürüm uyumsuzluðu yok, sorgular
// Kibana -> Dev Tools'a kopyalanýp aynen denenebilir.
//
// Elasticsearch'e ulaþýlamazsa metotlar null döner (sayfa "ulaþýlamýyor" gösterir, API ÇÖKMEZ).
// =====================================================================
public class ElasticLogQueryService(HttpClient _http,
                                    ILogger<ElasticLogQueryService> _logger) : ILogQueryService
{
    // Serilog'un yazdýðý veri akýþý: logs-zenblog-development, logs-zenblog-production...
    private const string IndexPattern = "logs-zenblog-*";

    // Türkiye saati: grafik çubuklarý gece yarýsýna göre hizalansýn
    private const string TimeZone = "+03:00";

    // Listede lazým olan alanlar (logun TAMAMI deðil: daha az veri, daha hýzlý)
    private static readonly string[] ListFields =
    {
        "@timestamp", "log.level", "message", "labels.*", "metadata.*",
        "http.response.status_code", "url.path", "event.duration", "error.type", "trace.id"
    };

    // =================================================================
    // 1) LÝSTE
    // =================================================================
    public async Task<LogSearchResult?> SearchAsync(LogSearchFilter f, CancellationToken ct)
    {
        // "filter": puanlama yapmadan eler (hýzlý, önbelleðe alýnýr). Kesin koþullar buraya.
        var filters = new JsonArray { Range(f.From, f.To) };

        if (f.Levels is { Length: > 0 })
            filters.Add(new JsonObject { ["terms"] = new JsonObject { ["log.level"] = ToArray(f.Levels) } });

        if (!string.IsNullOrWhiteSpace(f.EventType))
            filters.Add(Term("labels.EventType", f.EventType));

        if (f.OnlyEvents)
            filters.Add(new JsonObject { ["exists"] = new JsonObject { ["field"] = "labels.EventType" } });

        if (!string.IsNullOrWhiteSpace(f.TraceId))
            filters.Add(Term("trace.id", f.TraceId));

        // Kullanýcý: iþlemi YAPAN ya da iþlemden ETKÝLENEN bu kiþi olsun ("should" = VEYA)
        if (f.UserId is Guid userId)
        {
            filters.Add(new JsonObject
            {
                ["bool"] = new JsonObject
                {
                    ["should"] = new JsonArray
                    {
                        Term("labels.UserId", userId.ToString()),
                        Term("labels.TargetUserId", userId.ToString())
                    },
                    ["minimum_should_match"] = 1
                }
            });
        }

        var boolQuery = new JsonObject { ["filter"] = filters };

        // Metin aramasý: mesajda geçen kelimeler (hepsi geçmeli: "and")
        if (!string.IsNullOrWhiteSpace(f.Search))
        {
            boolQuery["must"] = new JsonArray
            {
                new JsonObject
                {
                    ["simple_query_string"] = new JsonObject
                    {
                        ["query"] = f.Search,
                        ["fields"] = ToArray("message"),
                        ["default_operator"] = "and"
                    }
                }
            };
        }

        var body = new JsonObject
        {
            ["from"] = (f.Page - 1) * f.PageSize,
            ["size"] = f.PageSize,
            ["track_total_hits"] = true,   // toplam sayýyý 10.000'de kesmeden hesapla
            ["sort"] = new JsonArray { new JsonObject { ["@timestamp"] = "desc" } },
            ["query"] = new JsonObject { ["bool"] = boolQuery },
            ["_source"] = ToArray(ListFields)
        };

        using var doc = await SearchRawAsync(body, ct);
        if (doc is null)
            return null;

        var hits = doc.RootElement.GetProperty("hits");
        var total = hits.GetProperty("total").GetProperty("value").GetInt32();
        var items = hits.GetProperty("hits").EnumerateArray().Select(ToEntry).ToList();

        return new LogSearchResult(total, f.Page, f.PageSize, items);
    }

    // =================================================================
    // 2) ÖZET: tek istekte bütün sayýlar ve grafikler ("aggregations")
    // size = 0: log satýrý istemiyoruz, sadece hesaplamalar
    // =================================================================
    public async Task<LogSummaryResult?> GetSummaryAsync(DateTime from, DateTime to, string interval, CancellationToken ct)
    {
        var body = new JsonObject
        {
            ["size"] = 0,
            ["track_total_hits"] = true,
            ["query"] = Range(from, to),
            ["aggs"] = new JsonObject
            {
                // Seviyelere göre sayým
                ["levels"] = Terms("log.level", 10),

                // Olay türlerine göre sayým
                ["events"] = Terms("labels.EventType", 30),

                // HTTP istekleri: sayý, ortalama süre, 4xx ve 5xx
                ["requests"] = new JsonObject
                {
                    ["filter"] = new JsonObject { ["exists"] = new JsonObject { ["field"] = "http.response.status_code" } },
                    ["aggs"] = new JsonObject
                    {
                        ["avg_duration"] = new JsonObject { ["avg"] = new JsonObject { ["field"] = "event.duration" } },
                        ["server_errors"] = RangeFilter("http.response.status_code", 500, null),
                        ["client_errors"] = RangeFilter("http.response.status_code", 400, 499)
                    }
                },

                // Zaman grafiði: her çubukta seviyelere göre sayým. Boþ aralýklar da 0 olarak gelsin.
                ["timeline"] = new JsonObject
                {
                    ["date_histogram"] = new JsonObject
                    {
                        ["field"] = "@timestamp",
                        ["fixed_interval"] = interval,
                        ["time_zone"] = TimeZone,
                        ["min_doc_count"] = 0,
                        ["extended_bounds"] = new JsonObject { ["min"] = Iso(from), ["max"] = Iso(to) }
                    },
                    ["aggs"] = new JsonObject { ["levels"] = Terms("log.level", 5) }
                },

                // En yavaþ 5 uç nokta (ortalama süreye göre)
                ["slowest"] = new JsonObject
                {
                    ["terms"] = new JsonObject
                    {
                        ["field"] = "url.path",
                        ["size"] = 5,
                        ["order"] = new JsonObject { ["avg_duration"] = "desc" }
                    },
                    ["aggs"] = new JsonObject
                    {
                        ["avg_duration"] = new JsonObject { ["avg"] = new JsonObject { ["field"] = "event.duration" } }
                    }
                }
            }
        };

        using var doc = await SearchRawAsync(body, ct);
        if (doc is null)
            return null;

        var root = doc.RootElement;
        var total = root.GetProperty("hits").GetProperty("total").GetProperty("value").GetInt32();
        var aggs = root.GetProperty("aggregations");

        var levels = Buckets(aggs, "levels");
        var events = Buckets(aggs, "events");

        var requests = aggs.GetProperty("requests");
        var avgNs = NullableDouble(requests.GetProperty("avg_duration").GetProperty("value"));

        var timeline = aggs.GetProperty("timeline").GetProperty("buckets").EnumerateArray()
            .Select(b =>
            {
                var byLevel = Buckets(b, "levels");
                return new LogTimelinePoint(
                    DateTimeOffset.FromUnixTimeMilliseconds(b.GetProperty("key").GetInt64()).LocalDateTime,
                    CountOf(byLevel, "Information"),
                    CountOf(byLevel, "Warning"),
                    CountOf(byLevel, "Error") + CountOf(byLevel, "Fatal"));
            })
            .ToList();

        var slowest = aggs.GetProperty("slowest").GetProperty("buckets").EnumerateArray()
            .Select(b => new SlowEndpointResult(
                b.GetProperty("key").GetString() ?? "",
                Math.Round((NullableDouble(b.GetProperty("avg_duration").GetProperty("value")) ?? 0) / 1_000_000d, 1),
                b.GetProperty("doc_count").GetInt32()))
            .ToList();

        return new LogSummaryResult(
            total,
            CountOf(levels, "Error") + CountOf(levels, "Fatal"),
            CountOf(levels, "Warning"),
            requests.GetProperty("doc_count").GetInt32(),
            requests.GetProperty("server_errors").GetProperty("doc_count").GetInt32(),
            requests.GetProperty("client_errors").GetProperty("doc_count").GetInt32(),
            avgNs is null ? null : Math.Round(avgNs.Value / 1_000_000d, 1),   // nanosaniye -> ms
            CountOf(events, "LoginFailed"),
            events,
            timeline,
            slowest);
    }

    // =================================================================
    // 3) DETAY: tek logun TAMAMI
    // =================================================================
    public async Task<JsonElement?> GetByIdAsync(string index, string id, CancellationToken ct)
    {
        // GÜVENLÝK: sadece ZenBlog log index'leri okunabilir.
        // (Data stream'in arka plandaki index adý ".ds-logs-zenblog-..." biçimindedir.)
        // Bu kontrol olmasa biri index adýný deðiþtirip Elasticsearch'teki BAÞKA verileri okuyabilirdi.
        if (!index.StartsWith(".ds-logs-zenblog-", StringComparison.Ordinal) &&
            !index.StartsWith("logs-zenblog-", StringComparison.Ordinal))
            return null;

        try
        {
            using var response = await _http.GetAsync(
                $"{Uri.EscapeDataString(index)}/_doc/{Uri.EscapeDataString(id)}", ct);

            if (!response.IsSuccessStatusCode)
                return null;

            await using var stream = await response.Content.ReadAsStreamAsync(ct);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);

            if (!doc.RootElement.TryGetProperty("_source", out var source))
                return null;

            // Clone: JsonDocument kapanýnca (using) veri kaybolmasýn
            return source.Clone();
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            _logger.LogWarning(ex, "Log detayý okunamadý: Elasticsearch'e ulaþýlamadý");
            return null;
        }
    }

    // =================================================================
    // YARDIMCILAR
    // =================================================================

    // Sorguyu gönder; hata olursa null (ÇÖKME YOK)
    private async Task<JsonDocument?> SearchRawAsync(JsonObject body, CancellationToken ct)
    {
        try
        {
            using var content = new StringContent(body.ToJsonString(), Encoding.UTF8, "application/json");
            using var response = await _http.PostAsync($"{IndexPattern}/_search?ignore_unavailable=true", content, ct);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(ct);
                _logger.LogWarning("Log sorgusu baþarýsýz: {StatusCode} {ErrorBody}",
                                   (int)response.StatusCode, error.Length > 500 ? error[..500] : error);
                return null;
            }

            await using var stream = await response.Content.ReadAsStreamAsync(ct);
            return await JsonDocument.ParseAsync(stream, cancellationToken: ct);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            _logger.LogWarning(ex, "Log sorgusu yapýlamadý: Elasticsearch'e ulaþýlamadý");
            return null;
        }
    }

    // Elasticsearch satýrýný listedeki tek log satýrýna çevir
    private static LogEntryResult ToEntry(JsonElement hit)
    {
        var src = hit.GetProperty("_source");
        var durationNs = GetNumber(src, "event.duration");

        return new LogEntryResult(
            hit.GetProperty("_id").GetString()!,
            hit.GetProperty("_index").GetString()!,
            GetDate(src, "@timestamp") ?? DateTime.MinValue,
            GetString(src, "log.level") ?? "Information",
            CleanMessage(GetString(src, "message")),
            GetString(src, "labels.EventType"),
            GetString(src, "labels.UserId"),
            null,
            GetString(src, "labels.TargetUserId"),
            null,
            GetNumber(src, "http.response.status_code") is double code ? (int)code : null,
            GetString(src, "url.path"),
            durationNs is null ? null : Math.Round(durationNs.Value / 1_000_000d, 1),
            GetString(src, "error.type"),
            GetString(src, "trace.id"),
            GetString(src, "labels.RequestName"));
    }

    // Serilog metin deðerlerini mesajda týrnak içinde yazar: HTTP "GET" "/api/home" -> okunaklý hale getir
    private static string CleanMessage(string? message) =>
        (message ?? string.Empty).Replace("\"", string.Empty);

    // "log.level" gibi bir yolu bul. Elasticsearch alanlarý iç içe ({"log":{"level":..}})
    // ya da noktalý düz anahtar ({"labels.EventType":..}) olarak saklayabilir; ikisini de dener.
    private static JsonElement? Find(JsonElement element, string path)
    {
        if (element.ValueKind != JsonValueKind.Object)
            return null;

        if (element.TryGetProperty(path, out var direct))
            return direct;

        var dot = path.IndexOf('.');
        if (dot < 0)
            return null;

        return element.TryGetProperty(path[..dot], out var child)
            ? Find(child, path[(dot + 1)..])
            : null;
    }

    private static string? GetString(JsonElement src, string path) => Find(src, path) switch
    {
        { ValueKind: JsonValueKind.String } e => e.GetString(),
        { ValueKind: JsonValueKind.Number } e => e.GetRawText(),
        _ => null
    };

    private static double? GetNumber(JsonElement src, string path) => Find(src, path) switch
    {
        { ValueKind: JsonValueKind.Number } e => e.GetDouble(),
        { ValueKind: JsonValueKind.String } e when double.TryParse(e.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var d) => d,
        _ => null
    };

    private static DateTime? GetDate(JsonElement src, string path) =>
        GetString(src, path) is string s && DateTimeOffset.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d)
            ? d.LocalDateTime
            : null;

    private static double? NullableDouble(JsonElement e) =>
        e.ValueKind == JsonValueKind.Number ? e.GetDouble() : null;

    // Aggregation sonucunu ad+sayý listesine çevir
    private static List<NameCount> Buckets(JsonElement parent, string name) =>
        parent.GetProperty(name).GetProperty("buckets").EnumerateArray()
            .Select(b => new NameCount(b.GetProperty("key").GetString() ?? "", b.GetProperty("doc_count").GetInt32()))
            .ToList();

    private static int CountOf(List<NameCount> list, string name) =>
        list.FirstOrDefault(x => x.Name == name)?.Count ?? 0;

    // --- Query DSL parçalarý ---
    private static string Iso(DateTime date) => date.ToUniversalTime().ToString("o", CultureInfo.InvariantCulture);

    private static JsonObject Range(DateTime from, DateTime to) => new()
    {
        ["range"] = new JsonObject { ["@timestamp"] = new JsonObject { ["gte"] = Iso(from), ["lte"] = Iso(to) } }
    };

    private static JsonObject Term(string field, string value) => new()
    {
        ["term"] = new JsonObject { [field] = value }
    };

    private static JsonObject Terms(string field, int size) => new()
    {
        ["terms"] = new JsonObject { ["field"] = field, ["size"] = size }
    };

    private static JsonObject RangeFilter(string field, int? gte, int? lte)
    {
        var range = new JsonObject();
        if (gte is not null) range["gte"] = gte;
        if (lte is not null) range["lte"] = lte;
        return new JsonObject { ["filter"] = new JsonObject { ["range"] = new JsonObject { [field] = range } } };
    }

    private static JsonArray ToArray(params string[] values) =>
        new(values.Select(v => (JsonNode?)JsonValue.Create(v)).ToArray());
}