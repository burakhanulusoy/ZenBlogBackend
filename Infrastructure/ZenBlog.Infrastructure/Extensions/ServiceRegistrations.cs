using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Infrastructure.Services;
using ZenBlog.Infrastructure.Services.EmailService;

namespace ZenBlog.Infrastructure.Extensions;

public static class ServiceRegistrations
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
                                                               IConfiguration configuration)
    {
        // secrets.json'daki "EmailSettings" bölümünü EmailSettings sýnýfýna doldur
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

        // Biri IEmailService isterse ona EmailService ver
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}