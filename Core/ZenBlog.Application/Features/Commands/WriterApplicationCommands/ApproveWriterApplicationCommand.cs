using MediatR;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.WriterApplicationCommands;

// Admin: başvuruyu onayla → Writer rolü verilir
public record ApproveWriterApplicationCommand(Guid Id) : IRequest<BaseResult<object>>;