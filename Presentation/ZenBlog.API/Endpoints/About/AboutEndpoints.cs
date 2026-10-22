using MediatR;
using Microsoft.AspNetCore.Mvc;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Features.Commands.AboutCommands;
using ZenBlog.Application.Features.Queries.AboutQueries;
using ZenBlog.Application.Features.Results.AboutResults;

namespace ZenBlog.API.Endpoints.About;

public static class AboutEndpoints
{
    public static void RegisterAboutEndpoints(this IEndpointRouteBuilder app)
    {
        var abouts = app.MapGroup("/abouts").WithTags("Abouts");

        // GET /api/abouts
        abouts.MapGet(string.Empty, async (IMediator mediator) =>
        {
            var response = await mediator.Send(new GetAboutsQuery());
            return Results.Ok(response);
        })
        .Produces<BaseResult<List<GetAboutsQueryResult>>>(StatusCodes.Status200OK);

        // GET /api/abouts/{id}
        abouts.MapGet("{id:guid}", async (IMediator mediator, Guid id) =>
        {
            var response = await mediator.Send(new GetAboutByIdQuery(id));
            return response.IsSuccessful ? Results.Ok(response) : Results.NotFound(response);
        })
        .Produces<BaseResult<GetAboutByIdQueryResult>>(StatusCodes.Status200OK)
        .Produces<BaseResult<GetAboutByIdQueryResult>>(StatusCodes.Status404NotFound);

        // POST /api/abouts (form-data: metinler + 3 resim)
        abouts.MapPost(string.Empty, async ([FromForm] CreateAboutCommand command, IMediator mediator) =>
        {
            var response = await mediator.Send(command);
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
        .RequireAuthorization(Policies.AdminOnly)   // YENİ: sadece Admin ekler/günceller/siler
        .DisableAntiforgery() // Form-data endpoint'i; API olduğumuz ve JWT kullanacağımız için CSRF token'ı gerekmiyor
        .Produces(StatusCodes.Status200OK)
        .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);

        // PUT /api/abouts (form-data: seçilmeyen resimler eski haliyle kalır)
        abouts.MapPut(string.Empty, async ([FromForm] UpdateAboutCommand command, IMediator mediator) =>
        {
            var response = await mediator.Send(command);
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
                    .RequireAuthorization(Policies.AdminOnly)   // YENİ: sadece Admin ekler/günceller/siler
        .DisableAntiforgery()
        .Produces(StatusCodes.Status200OK)
        .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);

        // DELETE /api/abouts/{id}
        abouts.MapDelete("{id:guid}", async (IMediator mediator, Guid id) =>
        {
            var response = await mediator.Send(new RemoveAboutCommand(id));
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
                    .RequireAuthorization(Policies.AdminOnly)   // YENİ: sadece Admin ekler/günceller/siler
        .Produces(StatusCodes.Status200OK)
        .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);
    }
}