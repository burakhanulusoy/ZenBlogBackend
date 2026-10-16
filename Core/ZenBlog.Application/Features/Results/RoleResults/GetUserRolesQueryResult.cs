namespace ZenBlog.Application.Features.Results.RoleResults;

// Rol atama ekraný: kullanýcý bilgisi + tüm roller (kutucuklar)
public record GetUserRolesQueryResult(Guid UserId,
                                      string FullName,
                                      List<UserRoleItemResult> Roles);

// Tek bir kutucuk: rol ve kullanýcýda olup olmadýðý
public record UserRoleItemResult(Guid RoleId,
                                 string RoleName,
                                 bool RoleExist);