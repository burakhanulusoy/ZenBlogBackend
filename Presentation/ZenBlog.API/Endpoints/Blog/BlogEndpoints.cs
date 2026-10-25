using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Application.Features.Commands.BlogCommands;
using ZenBlog.Application.Features.Queries.BlogQueries;
using ZenBlog.Application.Features.Results.BlogResults;
using ZenBlog.Application.Features.Results.HomeResults;

namespace ZenBlog.API.Endpoints.Blog
{
    // =====================================================================
    // BLOGLAR → /api/blogs
    //   Okuma (GET)       → HERKESE AÇIK (ziyaretçi sitesi)
    //   Ekle/güncelle/sil → sadece Writer ya da Admin
    //   my                → giriş yapan yazarın kendi blogları
    // Program.cs'teki /api grubu varsayılan olarak "giriş şart" diyor;
    // AllowAnonymous sadece eklendiği uçta bu kuralı kaldırıyor.
    // =====================================================================
    public static class BlogEndpoints
    {
        public static void RegisterBlogEndpoints(this IEndpointRouteBuilder app)
        {
            var blogs = app.MapGroup("/blogs").WithTags("Blogs");

            // ---------------------------------------------------------------
            // OKUMA (herkese açık)
            // ---------------------------------------------------------------

            // GET /api/blogs → tüm bloglar
            blogs.MapGet(string.Empty, async (IMediator mediator) =>
            {
                var response = await mediator.Send(new GetBlogsQuery());
                return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
            })
            .AllowAnonymous()   // YENİ
            .Produces<BaseResult<List<GetBlogsQueryResult>>>(StatusCodes.Status200OK)
            .Produces<BaseResult<List<GetBlogsQueryResult>>>(StatusCodes.Status400BadRequest);

            // GET /api/blogs/{id} → tek blog, bulunamazsa 404
            blogs.MapGet("{id:guid}", async (IMediator mediator, Guid id) =>
            {
                var response = await mediator.Send(new GetBlogByIdQuery(id));
                return response.IsSuccessful ? Results.Ok(response) : Results.NotFound(response);
            })
            .AllowAnonymous()   // YENİ
            .Produces<BaseResult<GetBlogByIdQueryResult>>(StatusCodes.Status200OK)
            .Produces<BaseResult<GetBlogByIdQueryResult>>(StatusCodes.Status404NotFound);

            // GET /api/blogs/byCategoryId/{categoryId} → bir kategorinin blogları
            blogs.MapGet("byCategoryId/{categoryId:guid}", async (IMediator mediator, Guid categoryId) =>
            {
                var response = await mediator.Send(new GetBlogsByCategoryIdQuery(categoryId));
                return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
            })
            .AllowAnonymous()   // YENİ
            .Produces<BaseResult<List<GetBlogsByCategoryIdQueryResult>>>(StatusCodes.Status200OK);

            // GET /api/blogs/byUserId/{userId} → bir yazarın blogları (yazar sayfası için)
            blogs.MapGet("byUserId/{userId:guid}", async (IMediator mediator, Guid userId) =>
            {
                var response = await mediator.Send(new GetBlogsByUserIdQuery(userId));
                return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
            })
            .AllowAnonymous()   // YENİ
            .Produces<BaseResult<List<GetBlogsByUserIdQueryResult>>>(StatusCodes.Status200OK);

            // GET /api/blogs/my → giriş yapan yazarın kendi blogları
            // Kullanıcı Id'si istekten DEĞİL token'dan; kimse başkasının listesini "kendi listesi" gibi çekemez
            blogs.MapGet("my", async (IMediator mediator, ICurrentUserService currentUser) =>
            {
                var response = await mediator.Send(new GetBlogsByUserIdQuery(currentUser.UserId!.Value));
                return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
            })
            .RequireAuthorization(Policies.WriterOrAdmin)
            .Produces<BaseResult<List<GetBlogsByUserIdQueryResult>>>(StatusCodes.Status200OK);

            // ---------------------------------------------------------------
            // YAZMA (sadece Writer ya da Admin)
            // ---------------------------------------------------------------

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
            .RequireAuthorization(Policies.WriterOrAdmin)   // YENİ: User blog ekleyemez
            .Produces(StatusCodes.Status200OK)
            .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);

            // PUT /api/blogs → güncelleme (form-data: seçilmeyen resimler eski haliyle kalır)
            blogs.MapPut(string.Empty, async ([FromForm] UpdateBlogCommand command, IMediator mediator) =>
            {
                var response = await mediator.Send(command);
                return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
            })
            .DisableAntiforgery()
            .RequireAuthorization(Policies.WriterOrAdmin)   // YENİ
            .Produces(StatusCodes.Status200OK)
            .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);

            // DELETE /api/blogs/{id}
            blogs.MapDelete("{id:guid}", async (IMediator mediator, Guid id) =>
            {
                var response = await mediator.Send(new RemoveBlogCommand(id));
                return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
            })
            .RequireAuthorization(Policies.WriterOrAdmin)   // YENİ
            .Produces(StatusCodes.Status200OK)
            .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);

            // NOT (TODO): "Sadece kendi blogunu güncelleyip silebilir" (sahiplik) kontrolü
            // handler'larda ayrıca yapılacak. Şu an Writer başkasının blogunun Id'sini bilirse değiştirebilir.


            // YENİ: GET /api/blogs/{id}/related?count=3 → ilgili yazılar (herkese açık)
            blogs.MapGet("{id:guid}/related", async (IMediator mediator, Guid id, int? count) =>
            {
                var response = await mediator.Send(new GetRelatedBlogsQuery(id, count ?? 3));
                return Results.Ok(response);
            })
            .AllowAnonymous()
            .Produces<BaseResult<List<HomeBlogResult>>>(StatusCodes.Status200OK);

            // YENİ: POST /api/blogs/{id}/view → okunmayı say (günlük tekil, herkese açık)
            // Ziyaretçi kimliği: giriş yapmışsa kullanıcı Id'si, değilse IP + tarayıcı bilgisi.
            // İkisi de OLDUĞU GİBİ saklanmaz; SHA-256 ile özetlenir (geri çevrilemez).
            blogs.MapPost("{id:guid}/view", async (Guid id, HttpContext http, IMediator mediator, ICurrentUserService currentUser) =>
            {
                var userAgent = http.Request.Headers.UserAgent.ToString();

                // Arama motoru botları sayılmasın
                if (IsBot(userAgent))
                    return Results.NoContent();

                var rawKey = currentUser.UserId is Guid userId
                    ? $"user:{userId}"
                    : $"anon:{http.Connection.RemoteIpAddress}|{userAgent}";

                var visitorKey = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawKey)));

                await mediator.Send(new RecordBlogViewCommand(id, visitorKey));
                return Results.NoContent();   // 204: cevap gövdesi yok, sadece "tamam"
            })
            .AllowAnonymous()
            .Produces(StatusCodes.Status204NoContent);

        }


        // Basit bot kontrolü: bilinen tarayıcı-dışı istemciler
        private static bool IsBot(string userAgent) =>
            string.IsNullOrWhiteSpace(userAgent) ||
            new[] { "bot", "crawler", "spider", "slurp", "facebookexternalhit", "preview" }
                .Any(word => userAgent.Contains(word, StringComparison.OrdinalIgnoreCase));

    }
}