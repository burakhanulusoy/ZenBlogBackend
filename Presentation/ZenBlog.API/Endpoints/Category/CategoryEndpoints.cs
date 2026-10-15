using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Commands.CategoryCommands;
using ZenBlog.Application.Features.Queries.CategoryQueries;
using ZenBlog.Application.Features.Results.CategoryResults;

namespace ZenBlog.API.Endpoints.Category;

public static class CategoryEndpoints
{
    public static void RegisterCategoryEndpoints(this  IEndpointRouteBuilder app)
    {
        var categories = app.MapGroup("/categories").WithTags("Categories");

        categories.MapGet(string.Empty, async (IMediator mediator) =>
        {
            var response = await mediator.Send(new GetCategoriesQuery());

            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);

        });


        categories.MapPost(string.Empty, async (IMediator mediator, CreateCategoryCommand command) =>
        {
            var response = await mediator.Send(command);
            return response.IsSuccessful ? Results.Ok() : Results.BadRequest(response);
        });


        categories.MapGet("{id}", async (Guid id, IMediator mediator) =>
        {
            var response = await mediator.Send(new GetCategoryByIdQuery(id));
            return response.IsSuccessful ? Results.Ok(response) :  Results.BadRequest(response);
        });

        categories.MapPut(string.Empty, async (IMediator mediator, UpdateCategoryCommand command) =>
        {
            var response = await mediator.Send(command);
            return response.IsSuccessful ? Results.Ok() :Results.BadRequest(response);
        });

        categories.MapDelete("{id}", async (IMediator mediator, Guid id) =>
        {
            var response = await mediator.Send(new RemoveCategoryCommand(id));

            return response.IsSuccessful ? Results.Ok() : Results.BadRequest(response);

        });

        categories.MapGet("with-blogs", async (IMediator mediator) =>
        {
            var response = await mediator.Send(new GetCategoriesWithBlogsQuery());
            return Results.Ok(response);
        })
        .Produces<BaseResult<List<GetCategoriesWithBlogsQueryResult>>>(StatusCodes.Status200OK);


    }




}
