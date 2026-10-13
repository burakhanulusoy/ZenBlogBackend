using MediatR;
using ZenBlog.Application.Features.Commands.UserCommands;

namespace ZenBlog.API.Endpoints.User;

public static class UserEndpoints
{
    public static void RegisterUserEndpoints(this IEndpointRouteBuilder app)
    {
        var users = app.MapGroup("/users").WithTags("Users");

        // Kayıt ol → POST /api/users/register
        users.MapPost("register", async (IMediator mediator, CreateUserCommand command) =>
        {
            var response = await mediator.Send(command);
            return response.IsSuccessful ? Results.Ok(response) : Results.BadRequest(response);
        });

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
    }
}