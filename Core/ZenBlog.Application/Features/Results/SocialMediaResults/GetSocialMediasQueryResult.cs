namespace ZenBlog.Application.Features.Results.SocialMediaResults;

public record GetSocialMediasQueryResult(Guid Id,
                                            string Icon,
                                            string Title,
                                            string Url);
