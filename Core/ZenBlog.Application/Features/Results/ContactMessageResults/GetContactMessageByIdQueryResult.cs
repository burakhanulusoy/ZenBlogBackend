using ZenBlog.Domain.Enums;

namespace ZenBlog.Application.Features.Results.ContactMessageResults;

// Mesaj detayý: metin dahil her þey
public record GetContactMessageByIdQueryResult(Guid Id,
                                               string Name,
                                               string Email,
                                               string Subject,
                                               string MessageBody,
                                               ContactMessageStatus ContactMessageStatus,
                                               DateTime CreatedAt);