using MediatR;
using ZenBlog.API.Filters;                          // YENİ: TurnstileValidationFilter
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;                // YENİ: Policies
using ZenBlog.Application.Features.Commands.ContactMessageCommands;
using ZenBlog.Application.Features.Queries.ContactMessageQueries;
using ZenBlog.Application.Features.Results.ContactMessageResults;
using ZenBlog.Domain.Enums;

namespace ZenBlog.API.Endpoints.ContactMessage;

// =====================================================================
// İLETİŞİM MESAJLARI → /api/contactmessages
//   POST            → ziyaretçi mesaj gönderir: HERKESE AÇIK + Turnstile + hız sınırı
//   GET/PUT/DELETE  → gelen kutusu: SADECE Admin (kişisel veri içeriyor)
// =====================================================================
public static class ContactMessageEndpoints
{
    public static void RegisterContactMessageEndpoints(this IEndpointRouteBuilder app)
    {
        var contactMessages = app.MapGroup("/contactmessages").WithTags("ContactMessages");

        // GET /api/contactmessages → bütün mesajlar (en yeni en üstte)
        contactMessages.MapGet(string.Empty, async (IMediator mediator) =>
        {
            var response = await mediator.Send(new GetContactMessagesQuery());
            return Results.Ok(response);
        })
        .RequireAuthorization(Policies.AdminOnly)   // YENİ
        .Produces<BaseResult<List<GetContactMessagesQueryResult>>>(StatusCodes.Status200OK);

        // GET /api/contactmessages/{id} → açılınca "Okundu" olur
        contactMessages.MapGet("{id:guid}", async (IMediator mediator, Guid id) =>
        {
            var response = await mediator.Send(new GetContactMessageByIdQuery(id));
            return response.IsSuccessful ? Results.Ok(response) : Results.NotFound(response);
        })
        .RequireAuthorization(Policies.AdminOnly)   // YENİ
        .Produces<BaseResult<GetContactMessageByIdQueryResult>>(StatusCodes.Status200OK)
        .Produces<BaseResult<GetContactMessageByIdQueryResult>>(StatusCodes.Status404NotFound);

        // GET /api/contactmessages/byStatus/{status} → örn. byStatus/Pending (menü rozeti için)
        contactMessages.MapGet("byStatus/{status}", async (IMediator mediator, ContactMessageStatus status) =>
        {
            var response = await mediator.Send(new GetContactMessagesByStatusQuery(status));
            return Results.Ok(response);
        })
        .RequireAuthorization(Policies.AdminOnly)   // YENİ
        .Produces<BaseResult<List<GetContactMessagesByStatusQueryResult>>>(StatusCodes.Status200OK);

        // POST /api/contactmessages → ziyaretçinin iletişim formu
        // AllowAnonymous: Program.cs'teki /api grubunun "giriş şart" kuralını bu uç için kaldırır
        contactMessages.MapPost(string.Empty, async (IMediator mediator, CreateContactMessageCommand command) =>
        {
            var response = await mediator.Send(command);
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
        .AllowAnonymous()                                     // YENİ: giriş gerekmez
        .AddEndpointFilter<TurnstileValidationFilter>()       // YENİ: bot / spam koruması
        .RequireRateLimiting("contact-message")
        .Produces(StatusCodes.Status200OK)
        .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);

        // PUT /api/contactmessages → durum değiştir   Body: { "id": "...", "status": 4 }
        contactMessages.MapPut(string.Empty, async (IMediator mediator, UpdateContactMessageCommand command) =>
        {
            var response = await mediator.Send(command);
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
        .RequireAuthorization(Policies.AdminOnly)   // YENİ
        .Produces(StatusCodes.Status200OK)
        .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);

        // DELETE /api/contactmessages/{id}
        contactMessages.MapDelete("{id:guid}", async (IMediator mediator, Guid id) =>
        {
            var response = await mediator.Send(new RemoveContactMessageCommand(id));
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
        .RequireAuthorization(Policies.AdminOnly)   // YENİ
        .Produces(StatusCodes.Status200OK)
        .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);
    }
}