using Scalar.AspNetCore;
using System.Threading.RateLimiting;
using ZenBlog.API.CustomMiddlewares;
using ZenBlog.API.Registrations;
using ZenBlog.Application.Base;
using ZenBlog.Application.Extensions;
using ZenBlog.Infrastructure.Extensions;
using ZenBlog.Persistence.Extensions;

var builder = WebApplication.CreateBuilder(args);

// SIRA ÖNEMLÝ: Persistence (AddIdentity) önce, Infrastructure (JWT) sonra;
// böylece JWT ayarlarý Identity'nin çerez varsayýlanlarýnýn üzerine yazýlýr.
builder.Services.AddPersitenceServices(builder.Configuration)
                .AddApplicationServices(builder.Configuration)
                .AddInfrastructureServices(builder.Configuration);

builder.Services.AddOpenApi();

// Angular (localhost:4200) API'ye istek atabilsin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularClient", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, cancellationToken) =>
    {
        await context.HttpContext.Response.WriteAsJsonAsync(
            BaseResult<object>.Fail("Çok fazla deneme yaptýnýz. Lütfen biraz bekleyip tekrar deneyin."),
            cancellationToken);
    };

    // Doðrulama kodu: IP baþýna dakikada 5
    options.AddPolicy("email-verification", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions { PermitLimit = 5, Window = TimeSpan.FromMinutes(1) }));

    // Login: IP baþýna dakikada 5 (brute force / credential stuffing'e karþý)
    options.AddPolicy("login", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions { PermitLimit = 5, Window = TimeSpan.FromMinutes(1) }));

    // Ýletiþim formu: IP baþýna 10 dakikada 3
    options.AddPolicy("contact-message", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions { PermitLimit = 3, Window = TimeSpan.FromMinutes(10) }));
});

var app = builder.Build();

// Hata middleware'i EN BAÞTA: sonraki her adýmýn hatasýný yakalayabilsin
app.UseMiddleware<CustomExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseCors("AngularClient");
app.UseRateLimiter();
app.UseAuthentication();   // 1) Token'ý oku ve doðrula: "bu kiþi KÝM?"
app.UseAuthorization();    // 2) Kurallara bak: "bu kiþi buna YETKÝLÝ mi?"

// Varsayýlan olarak her þey korumalý; herkese açýk endpoint'ler AllowAnonymous ile açýlýr.
// ("Güvenli varsayýlan" yaklaþýmý: yeni bir endpoint eklemeyi unutsan bile korumasýz kalmaz.)
app.MapGroup("/api")
   .RequireAuthorization("AdminOnly")
   .RegisterEndpoints();

app.Run();