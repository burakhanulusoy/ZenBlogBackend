using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Features.Queries.DashboardQueries;
using ZenBlog.Application.Features.Results.DashboardResults;

namespace ZenBlog.API.Endpoints.Dashboard;

// =====================================================================
// DASHBOARD → /api/dashboard/...?period=7d|30d|12m
// Her panelin genel bakışı TEK istekte gelir.
// =====================================================================
public static class DashboardEndpoints
{
    public static void RegisterDashboardEndpoints(this IEndpointRouteBuilder app)
    {
        var dashboard = app.MapGroup("/dashboard").WithTags("Dashboard");

        // GET /api/dashboard/admin?period=30d
        dashboard.MapGet("admin", async (IMediator mediator, string? period) =>
        {
            var response = await mediator.Send(new GetAdminDashboardQuery(period));
            return Results.Ok(response);
        })
        .RequireAuthorization(Policies.AdminOnly)
        .Produces<BaseResult<AdminDashboardResult>>(StatusCodes.Status200OK);

        // GET /api/dashboard/writer?period=30d  (giriş yapan yazarın kendi istatistikleri)
        dashboard.MapGet("writer", async (IMediator mediator, string? period) =>
        {
            var response = await mediator.Send(new GetWriterDashboardQuery(period));
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
        .RequireAuthorization(Policies.WriterOrAdmin)
        .Produces<BaseResult<WriterDashboardResult>>(StatusCodes.Status200OK);
    }
}