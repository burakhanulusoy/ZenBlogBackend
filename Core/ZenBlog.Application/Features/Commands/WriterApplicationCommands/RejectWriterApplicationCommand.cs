using MediatR;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.WriterApplicationCommands;

// Admin: baþvuruyu reddet. Body: { "reason": "..." } (Id adresten doldurulur)
public record RejectWriterApplicationCommand(Guid Id, string? Reason) : IRequest<BaseResult<object>>;