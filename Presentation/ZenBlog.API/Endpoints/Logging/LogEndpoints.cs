using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Features.Queries.LogQueries;
using ZenBlog.Application.Features.Results.LogResults;

namespace ZenBlog.API.Endpoints.Log;

// =====================================================================
// LOG KAYITLARI → /api/logs   (SADECE ADMIN: loglarda kullanıcı Id'leri ve hata ayrıntıları var)
// Elasticsearch'e ulaşılamazsa 503 döner; API'nin geri kalanı etkilenmez.
// =====================================================================
public static class LogEndpoints
{
    public static void RegisterLogEndpoints(this IEndpointRouteBuilder app)
    {
        var logs = app.MapGroup("/logs")
                      .WithTags("Logs")
                      .RequireAuthorization(Policies.AdminOnly);

        // GET /api/logs?period=24h&levels=Error,Warning&eventType=LoginFailed&search=...&userId=...&traceId=...&onlyEvents=true&page=1&pageSize=25
        logs.MapGet(string.Empty, async (IMediator mediator,
                                         string? period, string? levels, string? eventType, string? search,
                                         Guid? userId, string? traceId, bool? onlyEvents, int? page, int? pageSize) =>
        {
            var response = await mediator.Send(new GetLogsQuery(period, levels, eventType, search, userId, traceId,
                                                                onlyEvents ?? false, page ?? 1, pageSize ?? 25));
            return ToHttpResult(response);
        })
        .Produces<BaseResult<LogSearchResult>>(StatusCodes.Status200OK);

        // GET /api/logs/summary?period=24h → kartlar ve grafikler
        logs.MapGet("summary", async (IMediator mediator, string? period) =>
        {
            var response = await mediator.Send(new GetLogSummaryQuery(period));
            return ToHttpResult(response);
        })
        .Produces<BaseResult<LogSummaryResult>>(StatusCodes.Status200OK);

        // GET /api/logs/detail?index=...&id=... → tek logun tamamı
        logs.MapGet("detail", async (IMediator mediator, string index, string id) =>
        {
            var response = await mediator.Send(new GetLogDetailQuery(index, id));
            return response.IsSuccessful ? Results.Ok(response) : ToHttpResult(response);
        })
        .Produces<BaseResult<LogDetailResult>>(StatusCodes.Status200OK);
    }

    // Elasticsearch'e ulaşılamadıysa 503 (Service Unavailable), diğer hatalar 400
    private static IResult ToHttpResult<T>(BaseResult<T> response)
    {
        if (response.IsSuccessful)
            return Results.Ok(response);

        return response.Errors.Any(e => e.PropertyName == "LogsUnavailable")
            ? Results.Json(response, statusCode: StatusCodes.Status503ServiceUnavailable)
            : Results.BadRequest(response);
    }
}