using MediatR;
using Microsoft.AspNetCore.Routing;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Commands.CommentCommands;
using ZenBlog.Application.Features.Queries.CommentQueries;
using ZenBlog.Application.Features.Results.CommentResults;

namespace ZenBlog.API.Endpoints.Comment
{
    public static class CommentEnpoints
    {

        public static  void RegisterCommentEndpoint(this IEndpointRouteBuilder app)
        {

            var comments = app.MapGroup("/comments").WithTags("Comments");


            comments.MapGet("byBlogId/{BlogId}", async (IMediator mediator,Guid blogId) =>
            {

                var response = await mediator.Send(new GetCommentsByBlogIdQuery(blogId));

                return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);

            });


            comments.MapPost(string.Empty, async (IMediator mediator , CreateCommentCommand command) =>
            {
                var response = await mediator.Send(command);

                return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);

            });


            comments.MapDelete("{id}", async (IMediator mediator, Guid Id) =>
            {
                var response = await mediator.Send(new RemoveCommentCommand(Id));
                return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
            });


            comments.MapPut(string.Empty, async (IMediator mediator,UpdateCommentCommand command) =>
            {
                var response = await mediator.Send(command);

                return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);

            });

            // GET /api/comments/byUserId/{userId} → bir kullanıcının yaptığı yorumlar
            comments.MapGet("byUserId/{userId:guid}", async (IMediator mediator, Guid userId) =>
            {
                var response = await mediator.Send(new GetCommentsByUserIdQuery(userId));
                return Results.Ok(response);
            })
            .Produces<BaseResult<List<GetCommentsByUserIdQueryResult>>>(StatusCodes.Status200OK);


            // GET /api/comments/{id} → tek yorum (düzenleme formu için)
            comments.MapGet("{id:guid}", async (IMediator mediator, Guid id) =>
            {
                var response = await mediator.Send(new GetCommentByIdQuery(id));
                return response.IsSuccessful ? Results.Ok(response) : Results.NotFound(response);
            })
            .Produces<BaseResult<GetCommentByIdQueryResult>>(StatusCodes.Status200OK)
            .Produces<BaseResult<GetCommentByIdQueryResult>>(StatusCodes.Status404NotFound);


        }


    }
}
