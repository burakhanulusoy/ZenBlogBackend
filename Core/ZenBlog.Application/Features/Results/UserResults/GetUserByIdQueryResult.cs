namespace ZenBlog.Application.Features.Results.UserResults;

// Admin kullanýcý detayý
public record GetUserByIdQueryResult(Guid Id,
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
                                     bool EmailConfirmed,
                                     List<string> Roles,
                                     string Status,
                                     DateTime? DeletionRequestedAt,
                                     DateTime? DeletionDeadline,
                                     int BlogCount,
                                     int CommentCount,
                                     List<UserRecentBlogResult> RecentBlogs,
                                     List<UserBlockHistoryResult> BlockHistory,
                                     List<UserRoleChangeResult> RoleHistory);   // YENÝ

// Son bloglar listesi
public record UserRecentBlogResult(Guid Id,
                                   string MainTitle,
                                   string CoverImage,
                                   DateTime CreatedAt);

// Bloke geçmiþindeki her kayýt (en yeni en üstte)
public record UserBlockHistoryResult(Guid Id,
                                     string Reason,
                                     string EvidenceUrl,
                                     DateTime BlockedAt,
                                     Guid BlockedByAdminId,
                                     string BlockedByAdminName,
                                     DateTime? UnblockedAt,
                                     string? UnblockedByAdminName,
                                     string? UnblockReason);

// YENÝ: Rol geçmiþindeki her kayýt (en yeni en üstte)
public record UserRoleChangeResult(Guid Id,
                                   string RoleName,
                                   bool IsAdded,
                                   DateTime ChangedAt,
                                   string ChangedByAdminName);