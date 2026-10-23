using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Results.TeamResults;

namespace ZenBlog.Application.Features.Queries.TeamQueries;

// Seçilebilir platformlar (frontend açılır listesi bunu kullanır → tek kaynak backend)
public record GetTeamSocialPlatformsQuery : IRequest<BaseResult<List<GetTeamSocialPlatformsQueryResult>>>;