using MediatR;
using Microsoft.AspNetCore.Identity;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Application.Features.Queries.LogQueries;
using ZenBlog.Application.Features.Results.LogResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.LogHandlers;

// Elasticsearch'e ulaþýlamadýðýnda dönen hata kodu (endpoint bunu 503'e çevirir)
internal static class LogErrors
{
    public const string Unavailable = "LogsUnavailable";
    public const string UnavailableMessage = "Log sistemine þu an ulaþýlamýyor. Elasticsearch'in çalýþtýðýndan emin olun.";
}

// ---------------------------------------------------------------
// LÝSTE
// ---------------------------------------------------------------
public class GetLogsQueryHandler(ILogQueryService _logQueryService,
                                 UserManager<AppUser> _userManager)
    : IRequestHandler<GetLogsQuery, BaseResult<LogSearchResult>>
{
    public async Task<BaseResult<LogSearchResult>> Handle(GetLogsQuery request, CancellationToken cancellationToken)
    {
        var (from, to, _) = LogPeriods.Resolve(request.Period);

        // Sayfa ayarlarýný güvenli aralýða çek.
        // Elasticsearch varsayýlan olarak ilk 10.000 kayýttan ötesine sayfalamaya izin vermez.
        var pageSize = Math.Clamp(request.PageSize, 10, 100);
        var maxPage = 10_000 / pageSize;
        var page = Math.Clamp(request.Page, 1, maxPage);

        var levels = request.Levels?
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var filter = new LogSearchFilter(from, to, levels, request.EventType, request.Search,
                                         request.UserId, request.TraceId, request.OnlyEvents, page, pageSize);

        var result = await _logQueryService.SearchAsync(filter, cancellationToken);
        if (result is null)
            return BaseResult<LogSearchResult>.Fail(LogErrors.Unavailable, LogErrors.UnavailableMessage);

        // Id -> isim (sayfadaki bütün Id'ler TEK sorguda)
        var names = LogUserNames.Resolve(_userManager,
            result.Items.SelectMany(i => new[] { i.UserId, i.TargetUserId }));

        var items = result.Items
            .Select(i => i with
            {
                UserName = i.UserId is not null && names.TryGetValue(i.UserId, out var u) ? u : null,
                TargetUserName = i.TargetUserId is not null && names.TryGetValue(i.TargetUserId, out var t) ? t : null
            })
            .ToList();

        return BaseResult<LogSearchResult>.Success(result with { Items = items });
    }
}

// ---------------------------------------------------------------
// ÖZET
// ---------------------------------------------------------------
public class GetLogSummaryQueryHandler(ILogQueryService _logQueryService)
    : IRequestHandler<GetLogSummaryQuery, BaseResult<LogSummaryResult>>
{
    public async Task<BaseResult<LogSummaryResult>> Handle(GetLogSummaryQuery request, CancellationToken cancellationToken)
    {
        var (from, to, interval) = LogPeriods.Resolve(request.Period);

        var result = await _logQueryService.GetSummaryAsync(from, to, interval, cancellationToken);
        return result is null
            ? BaseResult<LogSummaryResult>.Fail(LogErrors.Unavailable, LogErrors.UnavailableMessage)
            : BaseResult<LogSummaryResult>.Success(result);
    }
}

// ---------------------------------------------------------------
// DETAY
// ---------------------------------------------------------------
public class GetLogDetailQueryHandler(ILogQueryService _logQueryService,
                                      UserManager<AppUser> _userManager)
    : IRequestHandler<GetLogDetailQuery, BaseResult<LogDetailResult>>
{
    public async Task<BaseResult<LogDetailResult>> Handle(GetLogDetailQuery request, CancellationToken cancellationToken)
    {
        var source = await _logQueryService.GetByIdAsync(request.Index, request.Id, cancellationToken);
        if (source is null)
            return BaseResult<LogDetailResult>.Fail("Log kaydý bulunamadý.");

        // Logun içindeki bütün Guid görünümlü deðerleri bul ve isimlere çevir
        var ids = new List<string?>();
        CollectStrings(source.Value, ids);
        var names = LogUserNames.Resolve(_userManager, ids);

        return BaseResult<LogDetailResult>.Success(new LogDetailResult(request.Id, request.Index, source.Value, names));
    }

    private static void CollectStrings(System.Text.Json.JsonElement element, List<string?> list)
    {
        switch (element.ValueKind)
        {
            case System.Text.Json.JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                    CollectStrings(property.Value, list);
                break;
            case System.Text.Json.JsonValueKind.Array:
                foreach (var item in element.EnumerateArray())
                    CollectStrings(item, list);
                break;
            case System.Text.Json.JsonValueKind.String:
                list.Add(element.GetString());
                break;
        }
    }
}