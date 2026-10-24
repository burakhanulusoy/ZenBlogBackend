using ZenBlog.Application.Features.Results.HomeResults;

namespace ZenBlog.Application.Contracts.Persistence;

// Ana sayfa verisini okuyan servis.
// Generic repository yerine özel servis: sorgular projeksiyonla (Select) yazılıyor →
// tek SQL, lazy loading yok, N+1 yok.
public interface IHomeReadService
{
    Task<HomePageResult> GetHomePageAsync(CancellationToken cancellationToken);
}