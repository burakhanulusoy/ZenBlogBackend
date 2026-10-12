using MediatR;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.CategoryCommands;

public record UpdateCategoryCommand(Guid Id,string Name) : IRequest<BaseResult<object>>;
