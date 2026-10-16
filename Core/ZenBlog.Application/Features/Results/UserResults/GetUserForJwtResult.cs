namespace ZenBlog.Application.Features.Results.UserResults;

public record GetUserForJwtResult(Guid Id,
                                 string Name,
                                 string Surname,
                                 string? ImageUrl,
                                 string Email,
                                 string UserName);
