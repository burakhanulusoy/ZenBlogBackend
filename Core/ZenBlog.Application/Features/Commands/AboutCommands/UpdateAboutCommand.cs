using MediatR;
using Microsoft.AspNetCore.Http;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.AboutCommands;

public record UpdateAboutCommand : IRequest<BaseResult<object>>
{
    public Guid? Id { get; init; }
    public string? MainTitle { get; init; }
    public string? Description { get; init; }
    public string? Title1 { get; init; }
    public string? Title2 { get; init; }
    public string? Title3 { get; init; }
    public IFormFile? Image1 { get; init; }   // seçilmezse eski resim kalýr
    public IFormFile? Image2 { get; init; }
    public IFormFile? Image3 { get; init; }
}