using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using ZenBlog.Application.Contracts.Infrastructure;

namespace ZenBlog.Infrastructure.Services.Azure
{
    public class AzureBlobFileService : IFileService
    {
        private readonly BlobContainerClient _container;

        // Uzantýdan dosya türünü (image/jpeg gibi) bulmak için ASP.NET Core'un hazýr aracý
        private static readonly FileExtensionContentTypeProvider ContentTypeProvider = new();

        // YENÝ: Tarayýcý dosyayý 1 yýl saklasýn (her ziyarette baþtan indirmesin).
        // "immutable": dosya adý Guid olduðu için ayný adresteki içerik HÝÇ deðiþmez;
        // resim deðiþince zaten yeni bir adres (yeni Guid) oluþuyor. Bu yüzden uzun süre saklamak güvenli.
        private const string CacheControl = "public, max-age=31536000, immutable";

        public AzureBlobFileService(BlobServiceClient blobServiceClient,
                                    IOptions<AzureBlobStorageSettings> options)
        {
            // "images" kapsayýcýsýna baðlanan istemci
            _container = blobServiceClient.GetBlobContainerClient(options.Value.ContainerName);
        }

        public async Task<string> UploadAsync(Stream fileStream, string fileName, string folder, CancellationToken cancellationToken)
        {
            // 1) Dosya adýný oluþtur: blogs/2026/10/{guid}.jpg
            //    wwwroot'taki klasör düzeninin aynýsý; Azure "/" iþaretlerini klasör gibi gösterir.
            var now = DateTime.UtcNow;
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            var blobName = $"{folder}/{now:yyyy}/{now:MM}/{Guid.NewGuid()}{extension}";

            // 2) Dosya türünü belirle. Bunu yapmazsak Azure dosyayý "bilinmeyen tür" olarak saklar
            //    ve tarayýcý resmi göstermek yerine ÝNDÝRMEYE çalýþýr.
            if (!ContentTypeProvider.TryGetContentType(fileName, out var contentType))
                contentType = "application/octet-stream";

            // 3) Yükle (DEÐÝÞTÝ: CacheControl eklendi)
            var blobClient = _container.GetBlobClient(blobName);
            await blobClient.UploadAsync(fileStream,
                new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = contentType,
                        CacheControl = CacheControl   // YENÝ: tarayýcý önbelleði
                    }
                },
                cancellationToken);

            // 4) Tam adresi dön: https://zenblogstorage2026.blob.core.windows.net/images/blogs/2026/10/....jpg
            return blobClient.Uri.ToString();
        }

        public async Task DeleteAsync(string? url, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(url))
                return;

            // Güvenlik: sadece bizim kapsayýcýmýzdaki dosyalar silinebilir.
            // Eski "/images/..." adresleri ya da baþka sitelerin adresleri bu kontrolde elenir.
            var containerUrl = _container.Uri.ToString().TrimEnd('/') + "/";
            if (!url.StartsWith(containerUrl, StringComparison.OrdinalIgnoreCase))
                return;

            // Adresten dosya adýný çýkar: "blogs/2026/10/....jpg"
            var blobName = Uri.UnescapeDataString(url[containerUrl.Length..]);

            // Dosya yoksa hata vermez, sessizce geçer
            await _container.GetBlobClient(blobName).DeleteIfExistsAsync(cancellationToken: cancellationToken);
        }
    }
}