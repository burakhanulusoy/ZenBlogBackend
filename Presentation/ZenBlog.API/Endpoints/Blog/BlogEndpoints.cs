using MediatR;
using Microsoft.AspNetCore.Mvc;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Application.Features.Commands.BlogCommands;
using ZenBlog.Application.Features.Queries.BlogQueries;
using ZenBlog.Application.Features.Results.BlogResults;

namespace ZenBlog.API.Endpoints.Blog
{
    public static class BlogEndpoints
    {
        public static void RegisterBlogEndpoints(this IEndpointRouteBuilder app)
        {
            //tümünü alma endpointisin
            var blogs = app.MapGroup("/blogs").WithTags("Blogs");//.RequireAuthorization();

            // GET /api/blogs → tüm bloglar
            blogs.MapGet(string.Empty, async (IMediator mediator) =>
            {
                var response = await mediator.Send(new GetBlogsQuery());
                return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
            })
            .Produces<BaseResult<List<GetBlogsQueryResult>>>(StatusCodes.Status200OK)
            .Produces<BaseResult<List<GetBlogsQueryResult>>>(StatusCodes.Status400BadRequest);
            // .RequireAuthorization();
            // bunu yaptıktan sonra 401 unothorizated hatası alır cunku arıtık korumalı bura
            // giriş yapmış tokenı olmayanlar ıcın hatalı lakın gırııs yapıca token lusacak zaten onun ıcın hata vereycek ılerde bırde admın role gore olacak



            // GET /api/blogs/{id} → tek blog, bulunamazsa 404
            blogs.MapGet("{id}", async (IMediator mediator, Guid id) =>
            {
                var response = await mediator.Send(new GetBlogByIdQuery(id));
                return response.IsSuccessful ? Results.Ok(response) : Results.NotFound(response);
            })
            .Produces<BaseResult<GetBlogByIdQueryResult>>(StatusCodes.Status200OK)
            .Produces<BaseResult<GetBlogByIdQueryResult>>(StatusCodes.Status404NotFound);

            // POST /api/blogs → yeni blog (form-data: yazı alanları + 3 resim)
            blogs.MapPost(string.Empty, async ([FromForm] CreateBlogCommand command, IMediator mediator) =>
            {
                var response = await mediator.Send(command);
                return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
            })
// DisableAntiforgery:
// .NET 8+ minimal API, form-data alan endpoint'lerde (dosya yükleme, [FromForm])
// CSRF saldırısına karşı otomatik olarak "antiforgery token" bekler.
// CSRF: Kullanıcı sitemize giriş yapmışken başka bir sitenin, tarayıcının
// otomatik gönderdiği çerezi (cookie) kullanarak onun adına istek atmasıdır.
// Bizim API'miz form sayfası üretmediği için bu token'ı kimseye vermiyor;
// koruma açık kalırsa her istek "token yok" diye 400 ile reddedilir.
// JWT, çerezle değil isteğin header'ına elle eklenerek gönderildiği için
// CSRF riski zaten oluşmaz; bu yüzden JWT kullanan API'lerde kapatılması standarttır.
.DisableAntiforgery()
            .DisableAntiforgery()
            .Produces(StatusCodes.Status200OK)
            .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);




            // PUT /api/blogs → güncelleme (form-data: seçilmeyen resimler eski haliyle kalır)
            blogs.MapPut(string.Empty, async ([FromForm] UpdateBlogCommand command, IMediator mediator) =>
            {
                var response = await mediator.Send(command);
                return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
            })
            .DisableAntiforgery()
            .Produces(StatusCodes.Status200OK)
            .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);

            // DELETE /api/blogs/{id}
            blogs.MapDelete("{id}", async (IMediator mediator, Guid id) =>
            {
                var response = await mediator.Send(new RemoveBlogCommand(id));
                return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
            })
            .Produces(StatusCodes.Status200OK)
            .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);


            //getblogsbycategoryId
            blogs.MapGet("byCategoryId/{categoryId}", async (IMediator mediator , Guid categoryId) =>
            {
                var response = await mediator.Send(new GetBlogsByCategoryIdQuery(categoryId));

                return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
            });

            //getblogsbyuserId
            blogs.MapGet("byUserId/{userId}", async (IMediator mediator, Guid userId) =>
            {
                var response = await mediator.Send(new GetBlogsByUserIdQuery(userId));

                return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
            });


            // GET /api/blogs/my → giriş yapan yazarın kendi blogları
            // Kullanıcı Id'si istekten DEĞİL token'dan; kimse başkasının listesini "kendi listesi" gibi çekemez
            blogs.MapGet("my", async (IMediator mediator, ICurrentUserService currentUser) =>
            {
                var response = await mediator.Send(new GetBlogsByUserIdQuery(currentUser.UserId!.Value));
                return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
            })
            .RequireAuthorization(Policies.WriterOrAdmin);

        }
    }
}