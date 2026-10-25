using System.Text.Json;
using ZenBlog.Application.Features.Results.LogResults;

namespace ZenBlog.Application.Contracts.Infrastructure;

// Log arama filtresi
public record LogSearchFilter(DateTime From,
                              DateTime To,
                              string[]? Levels,
                              string? EventType,
                              string? Search,
                              Guid? UserId,          // işlemi yapan YA DA etkilenen bu kişi olan loglar
                              string? TraceId,
                              bool OnlyEvents,       // sadece önemli olaylar (EventType olanlar)
                              int Page,
                              int PageSize);

// =====================================================================
// LOG SORGULARI (Elasticsearch)
// Application katmanı Elasticsearch'i bilmiyor; sadece bu arayüzü biliyor.
// Elasticsearch'e ulaşılamazsa metotlar null döner (sayfa "ulaşılamıyor" gösterir, API çökmez).
// =====================================================================
public interface ILogQueryService
{
    Task<LogSearchResult?> SearchAsync(LogSearchFilter filter, CancellationToken cancellationToken);

    Task<LogSummaryResult?> GetSummaryAsync(DateTime from, DateTime to, string interval, CancellationToken cancellationToken);

    // Bulunamazsa ya da index izinli değilse null
    Task<JsonElement?> GetByIdAsync(string index, string id, CancellationToken cancellationToken);
}