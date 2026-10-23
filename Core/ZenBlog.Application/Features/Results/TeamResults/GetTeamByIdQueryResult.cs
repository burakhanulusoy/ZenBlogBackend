namespace ZenBlog.Application.Features.Results.TeamResults;

public record GetTeamByIdQueryResult(Guid Id,
                                     string Name,
                                     string Surname,
                                     string Title,
                                     string Description,
                                     string Image,
                                     DateTime CreatedAt,
                                     List<GetTeamSocialMediaResult> TeamSocialMedias);