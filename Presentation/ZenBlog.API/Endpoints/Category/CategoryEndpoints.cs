using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;   // YENİ: Policies
using ZenBlog.Application.Features.Commands.CategoryCommands;
using ZenBlog.Application.Features.Queries.BlogQueries;
using ZenBlog.Application.Features.Queries.CategoryQueries;
using ZenBlog.Application.Features.Results.BlogResults;
using ZenBlog.Application.Features.Results.CategoryResults;

namespace ZenBlog.API.Endpoints.Category;

// =====================================================================
// KATEGORİLER → /api/categories
//   Okuma (GET)       → HERKESE AÇIK (sitedeki menü ve kategori sayfaları)
//   Ekle/güncelle/sil → sadece Admin
// =====================================================================
public static class CategoryEndpoints
{
    public static void RegisterCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var categories = app.MapGroup("/categories").WithTags("Categories");

        // ---------------------------------------------------------------
        // OKUMA (herkese açık)
        // ---------------------------------------------------------------

        // GET /api/categories → bütün kategoriler
        categories.MapGet(string.Empty, async (IMediator mediator) =>
        {
            var response = await mediator.Send(new GetCategoriesQuery());
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
        .AllowAnonymous()   // YENİ
        .Produces<BaseResult<List<GetCategoriesQueryResult>>>(StatusCodes.Status200OK);

        // GET /api/categories/with-blogs → blog sayılarıyla kategoriler
        // ÖNEMLİ: "with-blogs" satırı "{id:guid}" satırının üstünde ya da altında olabilir;
        // :guid kısıtı sayesinde "with-blogs" bir Id sanılmaz.
        categories.MapGet("with-blogs", async (IMediator mediator) =>
        {
            var response = await mediator.Send(new GetCategoriesWithBlogsQuery());
            return Results.Ok(response);
        })
        .AllowAnonymous()   // YENİ
        .Produces<BaseResult<List<GetCategoriesWithBlogsQueryResult>>>(StatusCodes.Status200OK);

        // GET /api/categories/{id} → tek kategori
        categories.MapGet("{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var response = await mediator.Send(new GetCategoryByIdQuery(id));
            return response.IsSuccessful ? Results.Ok(response) : Results.NotFound(response);
        })
        .AllowAnonymous()   // YENİ
        .Produces<BaseResult<GetCategoryByIdQueryResult>>(StatusCodes.Status200OK)
        .Produces<BaseResult<GetCategoryByIdQueryResult>>(StatusCodes.Status404NotFound);

        // ---------------------------------------------------------------
        // YAZMA (sadece Admin)
        // ---------------------------------------------------------------

        // POST /api/categories   Body: { "name": "..." }
        categories.MapPost(string.Empty, async (IMediator mediator, CreateCategoryCommand command) =>
        {
            var response = await mediator.Send(command);
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
        .RequireAuthorization(Policies.AdminOnly)   // YENİ
        .Produces<BaseResult<object>>(StatusCodes.Status200OK)
        .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);

        // PUT /api/categories   Body: { "id": "...", "name": "..." }
        categories.MapPut(string.Empty, async (IMediator mediator, UpdateCategoryCommand command) =>
        {
            var response = await mediator.Send(command);
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
        .RequireAuthorization(Policies.AdminOnly)   // YENİ
        .Produces<BaseResult<object>>(StatusCodes.Status200OK)
        .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);

        // DELETE /api/categories/{id}
        categories.MapDelete("{id:guid}", async (IMediator mediator, Guid id) =>
        {
            var response = await mediator.Send(new RemoveCategoryCommand(id));
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
        .RequireAuthorization(Policies.AdminOnly)   // YENİ
        .Produces<BaseResult<object>>(StatusCodes.Status200OK)
        .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);


        // YENİ: GET /api/categories/{id}/blogs?page=1&pageSize=9 → kategori sayfası (herkese açık)
        categories.MapGet("{id:guid}/blogs", async (IMediator mediator, Guid id, int? page, int? pageSize) =>
        {
            var response = await mediator.Send(new GetCategoryPageQuery(id, page ?? 1, pageSize ?? 9));
            return response.IsSuccessful ? Results.Ok(response) : Results.NotFound(response);
        })
        .AllowAnonymous()
        .Produces<BaseResult<CategoryPageResult>>(StatusCodes.Status200OK)
        .Produces<BaseResult<CategoryPageResult>>(StatusCodes.Status404NotFound);
    }
}