using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Features.Queries.TeamQueries;
using ZenBlog.Application.Features.Results.TeamResults;

namespace ZenBlog.Application.Features.Handlers.TeamHandlers;

public class GetTeamSocialPlatformsQueryHandler
    : IRequestHandler<GetTeamSocialPlatformsQuery, BaseResult<List<GetTeamSocialPlatformsQueryResult>>>
{
    public Task<BaseResult<List<GetTeamSocialPlatformsQueryResult>>> Handle(GetTeamSocialPlatformsQuery request, CancellationToken cancellationToken)
    {
        var result = TeamSocialPlatforms.Icons
            .Select(p => new GetTeamSocialPlatformsQueryResult(p.Key, p.Value))
            .ToList();

        // Veritabanýna gitmediði için hazýr bir Task döndürüyoruz
        return Task.FromResult(BaseResult<List<GetTeamSocialPlatformsQueryResult>>.Success(result));
    }
}