using ZenBlog.Domain.Enums;

namespace ZenBlog.Application.Features.Results.ContactMessageResults;

// Gelen kutusu listesi: mesaj metni yok, sadece özet bilgiler
public record GetContactMessagesQueryResult(Guid Id,
                                            string Name,
                                            string Email,
                                            string Subject,
                                            ContactMessageStatus ContactMessageStatus,
                                            DateTime CreatedAt);