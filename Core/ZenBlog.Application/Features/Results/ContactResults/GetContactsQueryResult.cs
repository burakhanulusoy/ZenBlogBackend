namespace ZenBlog.Application.Features.Results.ContactResults;
    public record GetContactsQueryResult(Guid Id,
                                        string MapLocationUrl,
                                        string Address,
                                        string PhoneNumber,
                                        string Email);
