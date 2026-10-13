using MediatR;
using Microsoft.AspNetCore.Http;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.BlogCommands;

public record CreateBlogCommand : IRequest<BaseResult<object>>
{
    public string? MainTitle { get; init; }
    public IFormFile? CoverImage { get; init; }
    public IFormFile? BlogImage1 { get; init; }
    public string? Title1 { get; init; }
    public string? Description1 { get; init; }
    public string? MainDescription { get; init; }
    public string? Description2 { get; init; }
    public string? Title2 { get; init; }
    public string? Description3 { get; init; }
    public IFormFile? BlogImage2 { get; init; }
    public string? Title3 { get; init; }
    public string? Description4 { get; init; }
    public Guid? CategoryId { get; init; }
    public Guid? AppUserId { get; init; }
}