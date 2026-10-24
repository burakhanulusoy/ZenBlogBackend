using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Commands.SubCommentCommands;
using ZenBlog.Application.Features.Queries.SubCommentQueries;
using ZenBlog.Application.Features.Results.SubCommentResults;

namespace ZenBlog.API.Endpoints.SubComment;

public static class SubCommentEndpoints
{
    public static void RegisterSubCommentEndpoints(this IEndpointRouteBuilder app)
    {
        var subComments = app.MapGroup("/subcomments").WithTags("SubComments");

        subComments.MapGet("byCommentId/{commentId:guid}", async (IMediator mediator, Guid commentId) =>
        {
            var response = await mediator.Send(new GetSubCommentsByCommentIdQuery(commentId));
            return Results.Ok(response);
        })
            .AllowAnonymous()

        .Produces<BaseResult<List<GetSubCommentsByCommentIdQueryResult>>>(StatusCodes.Status200OK);

        subComments.MapGet("byUserId/{userId:guid}", async (IMediator mediator, Guid userId) =>
        {
            var response = await mediator.Send(new GetSubCommentsByUserIdQuery(userId));
            return Results.Ok(response);
        })
        .Produces<BaseResult<List<GetSubCommentsByUserIdQueryResult>>>(StatusCodes.Status200OK);

        subComments.MapGet("{id:guid}", async (IMediator mediator, Guid id) =>
        {
            var response = await mediator.Send(new GetSubCommentByIdQuery(id));
            return response.IsSuccessful ? Results.Ok(response) : Results.NotFound(response);
        })
        .Produces<BaseResult<GetSubCommentByIdQueryResult>>(StatusCodes.Status200OK)
        .Produces<BaseResult<GetSubCommentByIdQueryResult>>(StatusCodes.Status404NotFound);

        subComments.MapPost(string.Empty, async (IMediator mediator, CreateSubCommentCommand command) =>
        {
            var response = await mediator.Send(command);
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
        .Produces(StatusCodes.Status200OK)
        .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);

        subComments.MapPut(string.Empty, async (IMediator mediator, UpdateSubCommentCommand command) =>
        {
            var response = await mediator.Send(command);
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
        .Produces(StatusCodes.Status200OK)
        .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);

        subComments.MapDelete("{id:guid}", async (IMediator mediator, Guid id) =>
        {
            var response = await mediator.Send(new RemoveSubCommentCommand(id));
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
        .Produces(StatusCodes.Status200OK)
        .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);
    }
}