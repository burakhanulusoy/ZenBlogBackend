using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Queries.WriterApplicationQueries;
using ZenBlog.Domain.Entities;
using ZenBlog.Domain.Enums;

namespace ZenBlog.Application.Features.Handlers.WriterApplicationHandlers;

public class GetPendingWriterApplicationCountQueryHandler(IRepository<WriterApplication> _applicationRepository)
    : IRequestHandler<GetPendingWriterApplicationCountQuery, BaseResult<int>>
{
    public async Task<BaseResult<int>> Handle(GetPendingWriterApplicationCountQuery request, CancellationToken cancellationToken)
    {
        var pending = await _applicationRepository.GetAllFilterAsync(a => a.Status == WriterApplicationStatus.Pending);
        return BaseResult<int>.Success(pending.Count);
    }
}