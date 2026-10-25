using System.Text.Json;
using ZenBlog.Application.Features.Results.DashboardResults;

namespace ZenBlog.Application.Features.Results.LogResults;

// Listedeki tek log satýrý
public record LogEntryResult(string Id,               // Elasticsearch belge Id'si (detay için)
                             string Index,            // hangi index'te (detay için)
                             DateTime Timestamp,
                             string Level,            // Information / Warning / Error / Fatal
                             string Message,
                             string? EventType,       // LoginFailed, UserBlocked... (önemli olaysa)
                             string? UserId,          // iþlemi yapan
                             string? UserName,        // (backend Id'den isme çevirir)
                             string? TargetUserId,    // iþlemden etkilenen
                             string? TargetUserName,
                             int? StatusCode,         // HTTP istek logu ise
                             string? Path,
                             double? DurationMs,
                             string? ErrorType,       // hata logu ise: exception türü
                             string? TraceId,         // ayný isteðin bütün loglarýný baðlar
                             string? RequestName);    // MediatR komut/sorgu adý

// Sayfalý liste
public record LogSearchResult(int Total, int Page, int PageSize, List<LogEntryResult> Items);

// Zaman grafiðindeki nokta
public record LogTimelinePoint(DateTime Time, int Information, int Warning, int Error);

// En yavaþ uç nokta
public record SlowEndpointResult(string Path, double AvgMs, int Count);

// Özet (kartlar + grafikler)
public record LogSummaryResult(int Total,
                               int Errors,
                               int Warnings,
                               int Requests,
                               int ServerErrors,     // 5xx
                               int ClientErrors,     // 4xx
                               double? AvgDurationMs,
                               int FailedLogins,
                               List<NameCount> Events,
                               List<LogTimelinePoint> Timeline,
                               List<SlowEndpointResult> Slowest);

// Detay: logun TAMAMI (Elasticsearch'teki ham hali) + içindeki Id'lerin isimleri
public record LogDetailResult(string Id,
                              string Index,
                              JsonElement Source,
                              Dictionary<string, string> UserNames);