using ZenBlog.Application.Features.Results.UserResults;

namespace ZenBlog.Application.Features.Results.RoleResults;

public record GetRoleByRoleIdWithUsersQueryResult(Guid Id,
                                                  string Name,
                                                  List<GetUsersQueryResult> AppUsers);