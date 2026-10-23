namespace ZenBlog.Application.Features.Results.TeamResults;

// TeamSocialMedias adý entity ile ayný ? AutoMapper listeyi kendiliðinden eþler
public record GetTeamsQueryResult(Guid Id,
                                  string Name,
                                  string Surname,
                                  string Title,
                                  string Description,
                                  string Image,
                                  DateTime CreatedAt,
                                  List<GetTeamSocialMediaResult> TeamSocialMedias);