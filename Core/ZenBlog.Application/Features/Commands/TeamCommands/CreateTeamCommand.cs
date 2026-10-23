using MediatR;
using Microsoft.AspNetCore.Http;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.TeamCommands;

public record CreateTeamCommand : IRequest<BaseResult<object>>
{
    public string? Name { get; init; }
    public string? Surname { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
    public IFormFile? Image { get; init; }
}