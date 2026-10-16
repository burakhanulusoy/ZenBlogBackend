using MediatR;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.RoleCommands;

// Kullanýcýnýn OLMASI GEREKEN rollerin tam listesi (iþaretli kutucuklar)
public record AssignRoleCommand(Guid UserId, List<Guid> RoleIds) : IRequest<BaseResult<object>>;