using MediatR;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.RoleCommands;

// Kullanıcının rollerini TOPLU belirle: RoleIds = olması gereken bütün roller
// (listede olmayanlar alınır, olmayan eklenir)
// YENİ: AdminPassword → Admin rolü VERİLİYORSA işlemi yapan adminin şifresi zorunlu
public record AssignRoleCommand(Guid UserId, List<Guid> RoleIds, string? AdminPassword) : IRequest<BaseResult<object>>;