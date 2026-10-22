using MediatR;
using Microsoft.AspNetCore.Mvc;
using ZenBlog.API.Filters;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Features.Commands.UserCommands;
using ZenBlog.Application.Features.Queries.UserQueries;
using ZenBlog.Application.Features.Results.UserResults;

namespace ZenBlog.API.Endpoints.User;

public static class UserEndpoints
{
    public static void RegisterUserEndpoints(this IEndpointRouteBuilder app)
    {
        var users = app.MapGroup("/users").WithTags("Users").AllowAnonymous();

        // Kayıt ol → POST /api/users/register
        users.MapPost("register", async (IMediator mediator, CreateUserCommand command) =>
        {
            var response = await mediator.Send(command);
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
            .AddEndpointFilter<TurnstileValidationFilter>();   // : bot koruması

        // Kodu doğrula → POST /api/users/confirm-email   Body: { "email": "...", "code": "482913" }
        users.MapPost("confirm-email", async (IMediator mediator, ConfirmEmailCommand command) =>
        {
            var response = await mediator.Send(command);
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
        .RequireRateLimiting("email-verification");   // dakikada en fazla 5 deneme

        // Kodu tekrar gönder → POST /api/users/resend-confirmation   Body: { "email": "..." }
        users.MapPost("resend-confirmation", async (IMediator mediator, ResendConfirmationEmailCommand command) =>
        {
            var response = await mediator.Send(command);
            return Results.Ok(response);
        })
        .RequireRateLimiting("email-verification");   // e-posta spam'ini engeller


        // POST /api/users/login → { "email": "...", "password": "..." }
        users.MapPost("login", async (IMediator mediator, LoginCommand command) =>
        {
            var response = await mediator.Send(command);
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
        .RequireRateLimiting("login")
        .Produces<BaseResult<LoginCommandResult>>(StatusCodes.Status200OK)
        .Produces<BaseResult<LoginCommandResult>>(StatusCodes.Status400BadRequest)
        .AddEndpointFilter<TurnstileValidationFilter>();   // YENİ: bot koruması

        // POST /api/users/google-login   Body: { "idToken": "..." }
        users.MapPost("google-login", async (IMediator mediator, GoogleLoginCommand command) =>
        {
            var response = await mediator.Send(command);
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
        .RequireRateLimiting("login")
        .Produces<BaseResult<LoginCommandResult>>(StatusCodes.Status200OK)
        .Produces<BaseResult<LoginCommandResult>>(StatusCodes.Status400BadRequest);


        // POST /api/users/forgot-password   Body: { "email": "..." }
        users.MapPost("forgot-password", async (IMediator mediator, ForgotPasswordCommand command) =>
        {
            var response = await mediator.Send(command);
            return Results.Ok(response);   // güvenlik gereği her zaman aynı cevap
        })
        .RequireRateLimiting("email-verification");   // e-posta spam'ini engeller

        // POST /api/users/reset-password   Body: { "email", "code", "newPassword", "confirmPassword" }
        users.MapPost("reset-password", async (IMediator mediator, ResetPasswordCommand command) =>
        {
            var response = await mediator.Send(command);
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
        .RequireRateLimiting("email-verification");   // kod tahminini (brute force) engeller


        // =================================================================
        // YENİ: HESABIM (giriş yapmış herkes) → /api/users/me/...
        // NOT: Yukarıdaki "users" grubu AllowAnonymous. Bir uçta AllowAnonymous varsa
        // RequireAuthorization çalışmaz (AllowAnonymous her zaman kazanır).
        // Bu yüzden yetki isteyen uçlar için AYRI bir grup açıyoruz; bu grup
        // Program.cs'teki /api grubunun RequireAuthorization()'ını devralıyor (giriş şart).
        // 4. adımda Profilim uçları (GET / PUT) da bu gruba eklenecek.
        // =================================================================
        var me = app.MapGroup("/users/me").WithTags("My Account");

        // POST /api/users/me/close   Body: { "currentPassword": "..." }
        // Hesabı 30 günlük kapanış sürecine alır (Admin hariç)
        me.MapPost("close", async (IMediator mediator, CloseMyAccountCommand command) =>
        {
            var response = await mediator.Send(command);
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
        .Produces<BaseResult<CloseMyAccountResult>>(StatusCodes.Status200OK)
        .Produces<BaseResult<CloseMyAccountResult>>(StatusCodes.Status400BadRequest);

        // GET /api/users/me → kendi profilim (kim olduğum token'dan)
        me.MapGet(string.Empty, async (IMediator mediator) =>
        {
            var response = await mediator.Send(new GetMyProfileQuery());
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
        .Produces<BaseResult<GetMyProfileQueryResult>>(StatusCodes.Status200OK);

        // PUT /api/users/me (form-data) → kendi profilimi güncelle, cevapta yeni token
        me.MapPut(string.Empty, async ([FromForm] UpdateMyProfileCommand command, IMediator mediator) =>
        {
            var response = await mediator.Send(command);
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
        .DisableAntiforgery()
        .Produces<BaseResult<LoginCommandResult>>(StatusCodes.Status200OK)
        .Produces<BaseResult<LoginCommandResult>>(StatusCodes.Status400BadRequest);


        // =================================================================
        // YENİ: KULLANICI YÖNETİMİ (sadece Admin) → /api/users/...
        // {id:guid}: sadece Guid biçimindeki adresler eşleşir → "/users/me" buraya karışmaz
        // =================================================================
        var admin = app.MapGroup("/users").WithTags("User Management").RequireAuthorization(Policies.AdminOnly);

        // GET /api/users → bütün kullanıcılar
        admin.MapGet(string.Empty, async (IMediator mediator) =>
        {
            var response = await mediator.Send(new GetUsersQuery());
            return Results.Ok(response);
        })
        .Produces<BaseResult<List<GetUserListQueryResult>>>(StatusCodes.Status200OK);

        // GET /api/users/{id} → kullanıcı detayı + bloke geçmişi
        admin.MapGet("{id:guid}", async (IMediator mediator, Guid id) =>
        {
            var response = await mediator.Send(new GetUserByIdQuery(id));
            return response.IsSuccessful ? Results.Ok(response) : Results.NotFound(response);
        })
        .Produces<BaseResult<GetUserByIdQueryResult>>(StatusCodes.Status200OK)
        .Produces<BaseResult<GetUserByIdQueryResult>>(StatusCodes.Status404NotFound);

        // POST /api/users/{id}/block   (form-data: Reason, Evidence)
        // "with": record'un kopyası, sadece UserId adresteki değerle değişmiş haliyle
        admin.MapPost("{id:guid}/block", async (Guid id, [FromForm] BlockUserCommand command, IMediator mediator) =>
        {
            var response = await mediator.Send(command with { UserId = id });
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
        .DisableAntiforgery()
        .Produces<BaseResult<object>>(StatusCodes.Status200OK)
        .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);

        // POST /api/users/{id}/unblock   Body: { "reason": "..." }
        admin.MapPost("{id:guid}/unblock", async (Guid id, [FromBody] UnblockUserCommand command, IMediator mediator) =>
        {
            var response = await mediator.Send(command with { UserId = id });
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        })
        .Produces<BaseResult<object>>(StatusCodes.Status200OK)
        .Produces<BaseResult<object>>(StatusCodes.Status400BadRequest);


    }
}