namespace ZenBlog.Application.Contracts.Infrastructure;

public interface IFileService
{
    // Dosyayı kaydeder ve erişim adresini döner.
    Task<string> UploadAsync(Stream fileStream, string fileName, string folder, CancellationToken cancellationToken);

    // Adresi verilen dosyayı siler. Dosya yoksa veya adres bize ait değilse hiçbir şey yapmaz.
    Task DeleteAsync(string? url, CancellationToken cancellationToken);
}