using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Results.WriterApplicationResults;

namespace ZenBlog.Application.Features.Queries.WriterApplicationQueries;

// Admin: baþvuru detayý. AÇILINCA "okundu" (Beklemede ? Ýncelendi) iþaretlenir.
public record GetWriterApplicationByIdQuery(Guid Id) : IRequest<BaseResult<WriterApplicationDetailResult>>;