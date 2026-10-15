using MediatR;
using Microsoft.AspNetCore.Http;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.AboutCommands;

public record CreateAboutCommand : IRequest<BaseResult<object>>
{
    public string? MainTitle { get; init; }
    public string? Description { get; init; }
    public string? Title1 { get; init; }
    public string? Title2 { get; init; }
    public string? Title3 { get; init; }
    public IFormFile? Image1 { get; init; }
    public IFormFile? Image2 { get; init; }
    public IFormFile? Image3 { get; init; }
}