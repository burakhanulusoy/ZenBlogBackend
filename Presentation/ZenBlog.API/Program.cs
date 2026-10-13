using Scalar.AspNetCore;
using System.Threading.RateLimiting;
using ZenBlog.API.CustomMiddlewares;
using ZenBlog.API.Registrations;
using ZenBlog.Application.Base;
using ZenBlog.Application.Extensions;
using ZenBlog.Infrastructure.Extensions;
using ZenBlog.Persistence.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();



builder.Services.AddPersitenceServices(builder.Configuration)
                .AddApplicationServices(builder.Configuration)
                .AddInfrastructureServices(builder.Configuration);




// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddRateLimiter(options =>
{
    // "email-verification" adýnda bir kural tanýmlýyoruz:
    // her IP adresi 1 dakika içinde en fazla 5 istek atabilir.
    options.AddPolicy("email-verification", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1)
            }));

    // Sýnýr aþýlýnca 429 dön ve cevabý BaseResult formatýnda ver;
    // böylece Angular her hatayý ayný yapýda okuyabilir.
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, cancellationToken) =>
    {
        await context.HttpContext.Response.WriteAsJsonAsync(
            BaseResult<object>.Fail("Çok fazla deneme yaptýnýz. Lütfen bir dakika bekleyip tekrar deneyin."),
            cancellationToken);
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseRateLimiter();

app.UseMiddleware<CustomExceptionHandlingMiddleware>();

app.UseHttpsRedirection();


app.UseAuthorization();

app.MapControllers();

app.MapGroup("/api")
    .RegisterEndpoints();

app.Run();
