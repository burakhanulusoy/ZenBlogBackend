namespace ZenBlog.Application.Features.Results.UserResults;

// Admin kullanýcý listesindeki her satýr
// Status: "Active" | "PendingDeletion" | "Blocked" (frontend okunaklý olsun diye yazý olarak)
public record GetUserListQueryResult(Guid Id,
                                     string Name,
                                     string Surname,
                                     string? UserName,
                                     string? Email,
                                     string? ImageUrl,
                                     bool EmailConfirmed,
                                     List<string> Roles,
                                     string Status,
                                     DateTime? DeletionDeadline,   // sadece "Kapatýlýyor" ise dolu
                                     int BlogCount,                // yayýndaki blog sayýsý
                                     DateTime? LastBlockedAt);     // en son ne zaman bloke edildi