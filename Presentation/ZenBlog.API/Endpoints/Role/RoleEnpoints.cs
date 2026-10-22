using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Features.Commands.RoleCommands;
using ZenBlog.Application.Features.Queries.RoleQueries;
using ZenBlog.Application.Features.Results.RoleResults;

namespace ZenBlog.API.Endpoints.Role;

public static class RoleEndpoints
{
    public static void RegisterRoleEndpoints(this IEndpointRouteBuilder app)
    {
        var roles = app.MapGroup("/roles").WithTags("Roles").RequireAuthorization(Policies.AdminOnly);

        // GET /api/roles
        roles.MapGet(string.Empty, async (IMediator mediator) =>
        {
            var response = await mediator.Send(new GetRolesQuery());
            return Results.Ok(response);
        })
        .Produces<BaseResult<List<GetRolesQueryResult>>>(StatusCodes.Status200OK);

        // GET /api/roles/{id}
        roles.MapGet("{id:guid}", async (IMediator mediator, Guid id) =>
        {
            var response = await mediator.Send(new GetRoleByIdQuery(id));
            return response.IsSuccessful ? Results.Ok(response) : Results.NotFound(response);
        })
        .Produces<BaseResult<GetRoleByIdQueryResult>>(StatusCodes.Status200OK)
        .Produces<BaseResult<GetRoleByIdQueryResult>>(StatusCodes.Status404NotFound);

        // GET /api/roles/{id}/users → rol ve o role sahip kullanıcılar
        roles.MapGet("withUsers/{roleId:guid}", async (IMediator mediator, Guid roleId) =>
        {
            var response = await mediator.Send(new GetRoleByRoleIdWithUserQuery(roleId));
            return response.IsSuccessful ? Results.Ok(response) : Results.NotFound(response);
        })
        .Produces<BaseResult<GetRoleByRoleIdWithUsersQueryResult>>(StatusCodes.Status200OK)
        .Produces<BaseResult<GetRoleByRoleIdWithUsersQueryResult>>(StatusCodes.Status404NotFound);

        // GET /api/roles/user/{userId} → rol atama ekranı (kutucuklar)
        roles.MapGet("user/{userId:guid}", async (IMediator mediator, Guid userId) =>
        {
            var response = await mediator.Send(new GetUserRolesQuery(userId));
            return response.IsSuccessful ? Results.Ok(response) : Results.NotFound(response);
        })
        .Produces<BaseResult<GetUserRolesQueryResult>>(StatusCodes.Status200OK)
        .Produces<BaseResult<GetUserRolesQueryResult>>(StatusCodes.Status404NotFound);

        // POST /api/roles
        roles.MapPost(string.Empty, async (IMediator mediator, CreateRoleCommand command) =>
        {
            var response = await mediator.Send(command);
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
        .Produces<BaseResult<object>>(StatusCodes.Status200OK)
        .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);

        // POST /api/roles/assign → kullanıcının rollerini güncelle
        roles.MapPost("assign", async (IMediator mediator, AssignRoleCommand command) =>
        {
            var response = await mediator.Send(command);
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
        .Produces<BaseResult<object>>(StatusCodes.Status200OK)
        .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);

        // PUT /api/roles
        roles.MapPut(string.Empty, async (IMediator mediator, UpdateRoleCommand command) =>
        {
            var response = await mediator.Send(command);
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
        .Produces<BaseResult<object>>(StatusCodes.Status200OK)
        .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);

        // DELETE /api/roles/{id}
        roles.MapDelete("{id:guid}", async (IMediator mediator, Guid id) =>
        {
            var response = await mediator.Send(new RemoveRoleCommand(id));
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
        .Produces<BaseResult<object>>(StatusCodes.Status200OK)
        .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);




    }
}