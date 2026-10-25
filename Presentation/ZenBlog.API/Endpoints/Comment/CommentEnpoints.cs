using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Commands.CommentCommands;
using ZenBlog.Application.Features.Queries.CommentQueries;
using ZenBlog.Application.Features.Results.CommentResults;

namespace ZenBlog.API.Endpoints.Comment
{
    // =====================================================================
    // YORUMLAR → /api/comments
    //   Okuma (blogun yorumları) → herkese açık
    //   Yaz / düzenle / sil      → giriş şart (/api grubundan gelir); sahiplik handler'da
    // =====================================================================
    public static class CommentEnpoints
    {
        public static void RegisterCommentEndpoint(this IEndpointRouteBuilder app)
        {
            var comments = app.MapGroup("/comments").WithTags("Comments");

            // GET /api/comments/byBlogId/{blogId} → yorumlar + yanıtlar (herkese açık)
            comments.MapGet("byBlogId/{blogId:guid}", async (IMediator mediator, Guid blogId) =>
            {
                var response = await mediator.Send(new GetCommentsByBlogIdQuery(blogId));
                return Results.Ok(response);
            })
            .AllowAnonymous()
            .Produces<BaseResult<List<GetCommentsByBlogIdQueryResult>>>(StatusCodes.Status200OK);

            // GET /api/comments/{id}
            comments.MapGet("{id:guid}", async (IMediator mediator, Guid id) =>
            {
                var response = await mediator.Send(new GetCommentByIdQuery(id));
                return response.IsSuccessful ? Results.Ok(response) : Results.NotFound(response);
            })
            .Produces<BaseResult<GetCommentByIdQueryResult>>(StatusCodes.Status200OK)
            .Produces<BaseResult<GetCommentByIdQueryResult>>(StatusCodes.Status404NotFound);

            // GET /api/comments/byUserId/{userId}
            comments.MapGet("byUserId/{userId:guid}", async (IMediator mediator, Guid userId) =>
            {
                var response = await mediator.Send(new GetCommentsByUserIdQuery(userId));
                return Results.Ok(response);
            })
            .Produces<BaseResult<List<GetCommentsByUserIdQueryResult>>>(StatusCodes.Status200OK);

            // POST /api/comments   Body: { "body": "...", "blogId": "..." }  (yazan token'dan)
            comments.MapPost(string.Empty, async (IMediator mediator, CreateCommentCommand command) =>
            {
                var response = await mediator.Send(command);
                return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
            })
            .Produces<BaseResult<object>>(StatusCodes.Status200OK)
            .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);

            // PUT /api/comments   Body: { "id": "...", "body": "..." }  (sadece sahibi)
            comments.MapPut(string.Empty, async (IMediator mediator, UpdateCommentCommand command) =>
            {
                var response = await mediator.Send(command);
                return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
            })
            .Produces<BaseResult<object>>(StatusCodes.Status200OK)
            .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);

            // DELETE /api/comments/{id}  (sahibi, blogun yazarı ya da Admin)
            comments.MapDelete("{id:guid}", async (IMediator mediator, Guid id) =>
            {
                var response = await mediator.Send(new RemoveCommentCommand(id));
                return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
            })
            .Produces<BaseResult<object>>(StatusCodes.Status200OK)
            .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);
        }
    }
}