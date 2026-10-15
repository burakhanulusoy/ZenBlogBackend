using ZenBlog.Domain.Enums;

namespace ZenBlog.Application.Features.Results.ContactMessageResults;

public record GetContactMessagesByStatusQueryResult(Guid Id,
                                                    string Name,
                                                    string Email,
                                                    string Subject,
                                                    ContactMessageStatus ContactMessageStatus,
                                                    DateTime CreatedAt);