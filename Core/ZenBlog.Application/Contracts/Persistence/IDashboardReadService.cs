using ZenBlog.Application.Features.Results.DashboardResults;

namespace ZenBlog.Application.Contracts.Persistence;

public interface IDashboardReadService
{
    Task<AdminDashboardResult> GetAdminAsync(string period, CancellationToken cancellationToken);
    Task<WriterDashboardResult> GetWriterAsync(Guid writerId, string period, CancellationToken cancellationToken);
}