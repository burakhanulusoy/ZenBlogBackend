using MediatR;

namespace ZenBlog.Application.Features.Commands.CategoryCommands;

public record UpdateCategoryCommand(string Name) : IRequest;
