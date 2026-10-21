using MediatR;
using ZenBlog.API.Filters;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Commands.UserCommands;
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

    }
}