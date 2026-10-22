namespace ZenBlog.Application.Features.Results.RoleResults;

// DEÐÝÞTÝ: UserCount (bu rolde kaç kiþi var) + IsSystem (Admin/Writer/User ? silinemez, adý deðiþmez)
public record GetRolesQueryResult(Guid Id,
                                  string Name,
                                  int UserCount,
                                  bool IsSystem);