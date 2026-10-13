using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Infrastructure.Services;
using ZenBlog.Infrastructure.Services.Azure;
using ZenBlog.Infrastructure.Services.EmailService;

namespace ZenBlog.Infrastructure.Extensions;

public static class ServiceRegistrations
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
                                                               IConfiguration configuration)
    {
        // E-posta
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.AddScoped<IEmailService, EmailService>();

        // Dosya depolama: Azure Blob Storage
        services.Configure<AzureBlobStorageSettings>(configuration.GetSection("AzureBlobStorage"));

        // Azure'a baðlanan ana istemci; thread-safe ve oluþturmasý maliyetli, o yüzden tek bir tane (Singleton)
        services.AddSingleton(_ => new BlobServiceClient(configuration["AzureBlobStorage:ConnectionString"]));

        services.AddScoped<IFileService, AzureBlobFileService>();
        // Yerel diske dönmek istersen: services.AddScoped<IFileService, LocalFileService>();

        return services;
    }
}