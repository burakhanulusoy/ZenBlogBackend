using MediatR;
using Microsoft.AspNetCore.Http;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.TeamCommands;

public record UpdateTeamCommand : IRequest<BaseResult<object>>
{
    public Guid? Id { get; init; }
    public string? Name { get; init; }
    public string? Surname { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
    public IFormFile? Image { get; init; }   // seçilmezse eski fotoðraf kalýr
}