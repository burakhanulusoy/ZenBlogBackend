using MediatR;
using Microsoft.AspNetCore.Http;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.UserCommands;

// Admin: kullanýcýyý bloke et (form-data: kanýt dosyasý var)
// UserId adresten (/users/{id}/block) gelip endpoint'te doldurulur; bloke eden admin token'dan.
public record BlockUserCommand : IRequest<BaseResult<object>>
{
    public Guid UserId { get; init; }
    public string? Reason { get; init; }       // gerekçe (zorunlu)
    public IFormFile? Evidence { get; init; }  // kanýt dosyasý (zorunlu)
}