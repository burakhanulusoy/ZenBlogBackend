using MediatR;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.CategoryCommands;

public record CreateCategoryCommand(string Name):IRequest<BaseResult<object>>;
