using MediatR;
using Microsoft.AspNetCore.Mvc;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Features.Commands.WriterApplicationCommands;
using ZenBlog.Application.Features.Queries.WriterApplicationQueries;
using ZenBlog.Application.Features.Results.WriterApplicationResults;

namespace ZenBlog.API.Endpoints.WriterApplication;

// =====================================================================
// YAZARLIK BAŞVURULARI (sadece Admin) → /api/writer-applications/...
// Kullanıcının kendi başvuru uçları UserEndpoints'te (/api/users/me/writer-application)
// =====================================================================
public static class WriterApplicationEndpoints
{
    public static void RegisterWriterApplicationEndpoints(this IEndpointRouteBuilder app)
    {
        var applications = app.MapGroup("/writer-applications")
                              .WithTags("Writer Applications")
                              .RequireAuthorization(Policies.AdminOnly);

        // GET /api/writer-applications → bütün başvurular
        applications.MapGet(string.Empty, async (IMediator mediator) =>
        {
            var response = await mediator.Send(new GetWriterApplicationsQuery());
            return Results.Ok(response);
        })
        .Produces<BaseResult<List<WriterApplicationListItemResult>>>(StatusCodes.Status200OK);

        // GET /api/writer-applications/pending-count → menüdeki rozet
        applications.MapGet("pending-count", async (IMediator mediator) =>
        {
            var response = await mediator.Send(new GetPendingWriterApplicationCountQuery());
            return Results.Ok(response);
        })
        .Produces<BaseResult<int>>(StatusCodes.Status200OK);

        // GET /api/writer-applications/{id} → detay (açılınca "okundu" olur)
        applications.MapGet("{id:guid}", async (IMediator mediator, Guid id) =>
        {
            var response = await mediator.Send(new GetWriterApplicationByIdQuery(id));
            return response.IsSuccessful ? Results.Ok(response) : Results.NotFound(response);
        })
        .Produces<BaseResult<WriterApplicationDetailResult>>(StatusCodes.Status200OK)
        .Produces<BaseResult<WriterApplicationDetailResult>>(StatusCodes.Status404NotFound);

        // POST /api/writer-applications/{id}/approve
        applications.MapPost("{id:guid}/approve", async (IMediator mediator, Guid id) =>
        {
            var response = await mediator.Send(new ApproveWriterApplicationCommand(id));
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
        .Produces<BaseResult<object>>(StatusCodes.Status200OK)
        .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);

        // POST /api/writer-applications/{id}/reject   Body: { "reason": "..." }
        applications.MapPost("{id:guid}/reject", async (Guid id, [FromBody] RejectWriterApplicationCommand command, IMediator mediator) =>
        {
            var response = await mediator.Send(command with { Id = id });
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
        .Produces<BaseResult<object>>(StatusCodes.Status200OK)
        .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);
    }
}