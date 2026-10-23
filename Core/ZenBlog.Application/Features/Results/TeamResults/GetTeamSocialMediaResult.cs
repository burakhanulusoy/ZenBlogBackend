namespace ZenBlog.Application.Features.Results.TeamResults;

// Üyenin bir sosyal hesabý (alan adlarý entity ile ayný)
public record GetTeamSocialMediaResult(Guid Id,
                                       string SocialMediaName,
                                       string Icon,
                                       string Url);