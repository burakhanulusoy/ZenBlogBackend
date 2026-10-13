using MediatR;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.BlogCommands;

public record RemoveBlogCommand(Guid Id) : IRequest<BaseResult<object>>;