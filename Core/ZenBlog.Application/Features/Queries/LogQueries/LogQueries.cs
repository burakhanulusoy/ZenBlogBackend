using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Results.LogResults;

namespace ZenBlog.Application.Features.Queries.LogQueries;

public record GetLogsQuery(string? Period,
                           string? Levels,       // virgüllü: "Error,Warning"
                           string? EventType,
                           string? Search,
                           Guid? UserId,
                           string? TraceId,
                           bool OnlyEvents,
                           int Page,
                           int PageSize) : IRequest<BaseResult<LogSearchResult>>;

public record GetLogSummaryQuery(string? Period) : IRequest<BaseResult<LogSummaryResult>>;

public record GetLogDetailQuery(string Index, string Id) : IRequest<BaseResult<LogDetailResult>>;