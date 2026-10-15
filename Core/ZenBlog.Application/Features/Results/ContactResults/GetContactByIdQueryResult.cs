namespace ZenBlog.Application.Features.Results.ContactResults;

public record GetContactByIdQueryResult(Guid Id,
                                        string MapLocationUrl,
                                        string Address,
                                        string PhoneNumber,
                                        string Email);
