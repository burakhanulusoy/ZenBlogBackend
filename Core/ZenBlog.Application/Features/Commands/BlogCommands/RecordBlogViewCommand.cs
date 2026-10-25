using MediatR;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.BlogCommands;

// VisitorKey endpoint'te oluþturulur (IP ve tarayýcý bilgisi HttpContext'te; Application katmaný bilmez)
public record RecordBlogViewCommand(Guid BlogId, string VisitorKey) : IRequest<BaseResult<object>>;