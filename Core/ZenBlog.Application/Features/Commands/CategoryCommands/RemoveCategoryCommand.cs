using MediatR;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.CategoryCommands;

public record RemoveCategoryCommand(Guid Id):IRequest<BaseResult<object>>;
