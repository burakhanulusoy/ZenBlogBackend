namespace ZenBlog.Application.Features.Results.SocialMediaResults;

public record GetSocialMediaByIdQueryResult(Guid Id,
                                            string Icon,
                                            string  Title,
                                            string Url);
