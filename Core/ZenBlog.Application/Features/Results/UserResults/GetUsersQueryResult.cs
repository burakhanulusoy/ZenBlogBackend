namespace ZenBlog.Application.Features.Results.UserResults;

public record GetUsersQueryResult(Guid Id,
                                string Name,
                                string Surname,
                                string? ImageUrl);
