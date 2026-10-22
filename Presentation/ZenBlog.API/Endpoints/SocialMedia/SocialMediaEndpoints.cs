using MediatR;
using Microsoft.AspNetCore.Mvc;                     // YENİ: [FromForm]
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;                // YENİ: Policies
using ZenBlog.Application.Features.Commands.SocialMediaCommands;
using ZenBlog.Application.Features.Queries.SocialMediaQueries;
using ZenBlog.Application.Features.Results.SocialMediaResults;

namespace ZenBlog.API.Endpoints.SocialMedia
{
    public static class SocialMediaEndpoints
    {
        public static void RegisterSocialMediaEndpoint(this IEndpointRouteBuilder app)
        {
            // DEĞİŞTİ: adres diğerleri gibi küçük harf + çoğul
            var socialMedias = app.MapGroup("/social-medias").WithTags("SocialMedia");

            // GET /api/social-medias
            // (İleride ziyaretçi sitesi için herkese açılacak → TODO listesindeki "endpoint izinleri")
            socialMedias.MapGet(string.Empty, async (IMediator mediator) =>
            {
                var response = await mediator.Send(new GetSocialMediasQuery());
                return Results.Ok(response);
            })
            .Produces<BaseResult<List<GetSocialMediasQueryResult>>>(StatusCodes.Status200OK);

            // GET /api/social-medias/{id}
            // DEĞİŞTİ: {id:guid} + bulunamazsa NotFound
            socialMedias.MapGet("{id:guid}", async (IMediator mediator, Guid id) =>
            {
                var response = await mediator.Send(new GetSocialMediaByIdQuery(id));
                return response.IsSuccessful ? Results.Ok(response) : Results.NotFound(response);
            })
            .Produces<BaseResult<GetSocialMediaByIdQueryResult>>(StatusCodes.Status200OK)
            .Produces<BaseResult<GetSocialMediaByIdQueryResult>>(StatusCodes.Status404NotFound);

            // POST /api/social-medias  (form-data: Title, Url, Icon)
            // DEĞİŞTİ: [FromForm] (dosya JSON'la gelmez) + DisableAntiforgery + sadece Admin
            socialMedias.MapPost(string.Empty, async ([FromForm] CreateSocialMediaCommand command, IMediator mediator) =>
            {
                var response = await mediator.Send(command);
                return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
            })
            .DisableAntiforgery()
            .RequireAuthorization(Policies.AdminOnly)
            .Produces<BaseResult<object>>(StatusCodes.Status200OK)
            .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);

            // PUT /api/social-medias  (form-data: Id, Title, Url, Icon?)
            socialMedias.MapPut(string.Empty, async ([FromForm] UpdateSocialMediaCommand command, IMediator mediator) =>
            {
                var response = await mediator.Send(command);
                return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
            })
            .DisableAntiforgery()
            .RequireAuthorization(Policies.AdminOnly)
            .Produces<BaseResult<object>>(StatusCodes.Status200OK)
            .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);

            // DELETE /api/social-medias/{id}
            socialMedias.MapDelete("{id:guid}", async (IMediator mediator, Guid id) =>
            {
                var response = await mediator.Send(new RemoveSocialMediaCommand(id));
                return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
            })
            .RequireAuthorization(Policies.AdminOnly)
            .Produces<BaseResult<object>>(StatusCodes.Status200OK)
            .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);
        }
    }
}