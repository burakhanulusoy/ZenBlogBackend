using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Features.Commands.ContactCommands;
using ZenBlog.Application.Features.Queries.ContactQueries;
using ZenBlog.Application.Features.Results.ContactResults;

namespace ZenBlog.API.Endpoints.Contact
{
    public static class ContactEndpoints
    {

        public  static void RegisterContactEndpoints(this IEndpointRouteBuilder app)
        {
            
            var contacts = app.MapGroup("/contacts").WithTags("Contacts");


            contacts.MapGet(string.Empty, async (IMediator mediator) =>
            {
                var response = await mediator.Send(new GetContactsQuery());
                return Results.Ok(response);
            })
            .AllowAnonymous()

   .Produces<BaseResult<List<GetContactsQueryResult>>>(StatusCodes.Status200OK);

            contacts.MapGet("{id:guid}", async (IMediator mediator, Guid id) =>
            {
                var response = await mediator.Send(new GetContactByIdQuery(id));
                return response.IsSuccessful ? Results.Ok(response) : Results.NotFound(response);
            })
            .AllowAnonymous()

            .Produces<BaseResult<GetContactByIdQueryResult>>(StatusCodes.Status200OK)
            .Produces<BaseResult<GetContactByIdQueryResult>>(StatusCodes.Status404NotFound);

            contacts.MapPost(string.Empty, async (IMediator mediator, CreateContactCommand command) =>
            {
                var response = await mediator.Send(command);
                return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
            })
                            .RequireAuthorization(Policies.AdminOnly)   // YENİ: sadece Admin
            .Produces(StatusCodes.Status200OK)
            .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);

            contacts.MapPut(string.Empty, async (IMediator mediator, UpdateContactCommand command) =>
            {
                var response = await mediator.Send(command);
                return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
            })
                            .RequireAuthorization(Policies.AdminOnly)   // YENİ: sadece Admin
            .Produces(StatusCodes.Status200OK)
            .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);

            contacts.MapDelete("{id:guid}", async (IMediator mediator, Guid id) =>
            {
                var response = await mediator.Send(new RemoveContactCommand(id));
                return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
            })
                            .RequireAuthorization(Policies.AdminOnly)   // YENİ: sadece Admin
            .Produces(StatusCodes.Status200OK)
            .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);

        }



    }
}
