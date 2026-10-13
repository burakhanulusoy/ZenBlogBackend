namespace ZenBlog.Infrastructure.Services.Azure;

// secrets.json'daki "AzureBlobStorage" bölümü bu sýnýfa doldurulur.
public class AzureBlobStorageSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
}