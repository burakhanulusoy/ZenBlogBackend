using MediatR;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Queries.WriterApplicationQueries;

// Admin: menüdeki rozet için okunmamýþ (Beklemede) baþvuru sayýsý
public record GetPendingWriterApplicationCountQuery : IRequest<BaseResult<int>>;