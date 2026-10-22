using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Results.WriterApplicationResults;

namespace ZenBlog.Application.Features.Queries.WriterApplicationQueries;

// Kullanýcý: baþvuru sayfasý için durum (baþvurabilir mi, son baþvurusu ne durumda)
public record GetMyWriterApplicationQuery : IRequest<BaseResult<MyWriterApplicationResult>>;