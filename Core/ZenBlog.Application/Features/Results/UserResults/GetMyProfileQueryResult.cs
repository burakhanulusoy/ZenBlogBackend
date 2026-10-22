namespace ZenBlog.Application.Features.Results.UserResults;

// Profilim sayfasý
public record GetMyProfileQueryResult(Guid Id,
                                      string Name,
                                      string Surname,
                                      string? UserName,
                                      string? Email,
                                      string? PhoneNumber,
                                      string? ImageUrl,
                                      string? Location,
                                      string? Description,
                                      string? InstagramUrl,
                                      string? TwitterUrl,
                                      string? LinkedlnUrl,
                                      string? FacebookUrl,
                                      bool HasPassword,        // Google ile gelenlerde false olabilir ? þifre onayý yapýlamaz
                                      bool CanCloseAccount,    // Admin deðilse true ? "Hesabýmý kapat" görünsün
                                      List<string> Roles);