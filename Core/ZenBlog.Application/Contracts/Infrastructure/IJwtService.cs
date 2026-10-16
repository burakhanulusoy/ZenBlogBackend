using ZenBlog.Application.Features.Results.UserResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Contracts.Infrastructure;

public interface IJwtService
{
    // Kullanıcıyı doğrudan alıyoruz; servisin onu tekrar veritabanından çekmesine gerek kalmıyor
    Task<LoginCommandResult> GenerateTokenAsync(AppUser user);
}