using MediatR;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.AboutCommands;

public record RemoveAboutCommand(Guid Id) : IRequest<BaseResult<object>>;