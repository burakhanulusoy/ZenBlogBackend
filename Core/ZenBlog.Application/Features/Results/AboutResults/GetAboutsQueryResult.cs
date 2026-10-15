namespace ZenBlog.Application.Features.Results.AboutResults;

public record GetAboutsQueryResult(Guid Id,
                                   string MainTitle,
                                   string Description,
                                   string Title1,
                                   string Title2,
                                   string Title3,
                                   string Image1,
                                   string Image2,
                                   string Image3,
                                   DateTime CreatedAt);