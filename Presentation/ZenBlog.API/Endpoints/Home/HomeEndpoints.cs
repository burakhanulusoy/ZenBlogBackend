using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Queries.HomeQueries;
using ZenBlog.Application.Features.Results.HomeResults;

namespace ZenBlog.API.Endpoints.Home;

// ANA SAYFA → /api/home   (herkese açık; tek istekte bütün ana sayfa verisi)
public static class HomeEndpoints
{
    public static void RegisterHomeEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/home", async (IMediator mediator) =>
        {
            var response = await mediator.Send(new GetHomePageQuery());
            return Results.Ok(response);
        })
        .WithTags("Home")
        .AllowAnonymous()
        .Produces<BaseResult<HomePageResult>>(StatusCodes.Status200OK);
    }
}