using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Commands.ContactMessageCommands;
using ZenBlog.Application.Features.Queries.ContactMessageQueries;
using ZenBlog.Application.Features.Results.ContactMessageResults;
using ZenBlog.Domain.Enums;

namespace ZenBlog.API.Endpoints.ContactMessage;

public static class ContactMessageEndpoints
{
    public static void RegisterContactMessageEndpoints(this IEndpointRouteBuilder app)
    {
        var contactMessages = app.MapGroup("/contactmessages").WithTags("ContactMessages");

        // GET /api/contactmessages → tüm mesajlar (gelen kutusu)
        contactMessages.MapGet(string.Empty, async (IMediator mediator) =>
        {
            var response = await mediator.Send(new GetContactMessagesQuery());
            return Results.Ok(response);
        })
        .Produces<BaseResult<List<GetContactMessagesQueryResult>>>(StatusCodes.Status200OK);

        // GET /api/contactmessages/{id} → mesaj detayı
        contactMessages.MapGet("{id:guid}", async (IMediator mediator, Guid id) =>
        {
            var response = await mediator.Send(new GetContactMessageByIdQuery(id));
            return response.IsSuccessful ? Results.Ok(response) : Results.NotFound(response);
        })
        .Produces<BaseResult<GetContactMessageByIdQueryResult>>(StatusCodes.Status200OK)
        .Produces<BaseResult<GetContactMessageByIdQueryResult>>(StatusCodes.Status404NotFound);

        // GET /api/contactmessages/byStatus/Pending → duruma göre filtre
        contactMessages.MapGet("byStatus/{status}", async (IMediator mediator, ContactMessageStatus status) =>
        {
            var response = await mediator.Send(new GetContactMessagesByStatusQuery(status));
            return Results.Ok(response);
        })
        .Produces<BaseResult<List<GetContactMessagesByStatusQueryResult>>>(StatusCodes.Status200OK);

        // POST /api/contactmessages → iletişim formundan yeni mesaj (herkese açık)
        contactMessages.MapPost(string.Empty, async (IMediator mediator, CreateContactMessageCommand command) =>
        {
            var response = await mediator.Send(command);
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
 .RequireRateLimiting("contact-message")
 .Produces(StatusCodes.Status200OK)
 .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);

        // PUT /api/contactmessages → sadece durum değiştirme
        contactMessages.MapPut(string.Empty, async (IMediator mediator, UpdateContactMessageCommand command) =>
        {
            var response = await mediator.Send(command);
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
        .Produces(StatusCodes.Status200OK)
        .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);

        // DELETE /api/contactmessages/{id}
        contactMessages.MapDelete("{id:guid}", async (IMediator mediator, Guid id) =>
        {
            var response = await mediator.Send(new RemoveContactMessageCommand(id));
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
        .Produces(StatusCodes.Status200OK)
        .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);
    }
}