using MediatR;
using Microsoft.AspNetCore.Mvc;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Features.Commands.TeamCommands;
using ZenBlog.Application.Features.Queries.TeamQueries;
using ZenBlog.Application.Features.Results.TeamResults;

namespace ZenBlog.API.Endpoints.Team;

// EKİP → /api/teams/...   (okuma ileride ziyaretçi sitesine açılacak; yazma SADECE Admin)
public static class TeamEndpoints
{
    public static void RegisterTeamEndpoints(this IEndpointRouteBuilder app)
    {
        var teams = app.MapGroup("/teams").WithTags("Teams");

        // ---------- ÜYELER ----------
        teams.MapGet(string.Empty, async (IMediator mediator) =>
        {
            var response = await mediator.Send(new GetTeamsQuery());
            return Results.Ok(response);
        })
            .AllowAnonymous()

        .Produces<BaseResult<List<GetTeamsQueryResult>>>(StatusCodes.Status200OK);

        // Seçilebilir platformlar
        teams.MapGet("platforms", async (IMediator mediator) =>
        {
            var response = await mediator.Send(new GetTeamSocialPlatformsQuery());
            return Results.Ok(response);
        })
            .AllowAnonymous()

        .Produces<BaseResult<List<GetTeamSocialPlatformsQueryResult>>>(StatusCodes.Status200OK);

        teams.MapGet("{id:guid}", async (IMediator mediator, Guid id) =>
        {
            var response = await mediator.Send(new GetTeamByIdQuery(id));
            return response.IsSuccessful ? Results.Ok(response) : Results.NotFound(response);
        })
            .AllowAnonymous()

        .Produces<BaseResult<GetTeamByIdQueryResult>>(StatusCodes.Status200OK)
        .Produces<BaseResult<GetTeamByIdQueryResult>>(StatusCodes.Status404NotFound);

        teams.MapPost(string.Empty, async ([FromForm] CreateTeamCommand command, IMediator mediator) =>
        {
            var response = await mediator.Send(command);
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
        .DisableAntiforgery()
        .RequireAuthorization(Policies.AdminOnly)
        .Produces<BaseResult<object>>(StatusCodes.Status200OK)
        .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);

        teams.MapPut(string.Empty, async ([FromForm] UpdateTeamCommand command, IMediator mediator) =>
        {
            var response = await mediator.Send(command);
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
        .DisableAntiforgery()
        .RequireAuthorization(Policies.AdminOnly)
        .Produces<BaseResult<object>>(StatusCodes.Status200OK)
        .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);

        teams.MapDelete("{id:guid}", async (IMediator mediator, Guid id) =>
        {
            var response = await mediator.Send(new RemoveTeamCommand(id));
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
        .RequireAuthorization(Policies.AdminOnly)
        .Produces<BaseResult<object>>(StatusCodes.Status200OK)
        .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);

        // ---------- ÜYENİN SOSYAL HESAPLARI ----------

        // POST /api/teams/{teamId}/social-medias   Body: { "socialMediaName": "LinkedIn", "url": "https://..." }
        teams.MapPost("{teamId:guid}/social-medias", async (Guid teamId, [FromBody] CreateTeamSocialMediaCommand command, IMediator mediator) =>
        {
            var response = await mediator.Send(command with { TeamId = teamId });
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
        .RequireAuthorization(Policies.AdminOnly)
        .Produces<BaseResult<object>>(StatusCodes.Status200OK)
        .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);

        // PUT /api/teams/social-medias   Body: { "id": "...", "socialMediaName": "...", "url": "..." }
        teams.MapPut("social-medias", async (IMediator mediator, UpdateTeamSocialMediaCommand command) =>
        {
            var response = await mediator.Send(command);
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
        .RequireAuthorization(Policies.AdminOnly)
        .Produces<BaseResult<object>>(StatusCodes.Status200OK)
        .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);

        teams.MapDelete("social-medias/{id:guid}", async (IMediator mediator, Guid id) =>
        {
            var response = await mediator.Send(new RemoveTeamSocialMediaCommand(id));
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
        .RequireAuthorization(Policies.AdminOnly)
        .Produces<BaseResult<object>>(StatusCodes.Status200OK)
        .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);
    }
}