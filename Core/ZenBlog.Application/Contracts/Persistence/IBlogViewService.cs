namespace ZenBlog.Application.Contracts.Persistence;

public interface IBlogViewService
{
    // Sayıldıysa true, bugün zaten sayılmışsa false
    Task<bool> RecordAsync(Guid blogId, string visitorKey, CancellationToken cancellationToken);
}