using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Results.WriterApplicationResults;

namespace ZenBlog.Application.Features.Queries.WriterApplicationQueries;

// Admin: bütün baþvurular
public record GetWriterApplicationsQuery : IRequest<BaseResult<List<WriterApplicationListItemResult>>>;