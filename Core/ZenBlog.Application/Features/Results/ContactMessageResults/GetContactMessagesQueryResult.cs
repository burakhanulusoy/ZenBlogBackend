using ZenBlog.Domain.Enums;

namespace ZenBlog.Application.Features.Results.ContactMessageResults;

// DEÐÝÞTÝ: MessageBody eklendi (gelen kutusunda önizleme için)
public record GetContactMessagesQueryResult(Guid Id,
                                            string Name,
                                            string Email,
                                            string Subject,
                                            string MessageBody,
                                            ContactMessageStatus ContactMessageStatus,
                                            DateTime CreatedAt);