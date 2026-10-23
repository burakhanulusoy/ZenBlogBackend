using AutoMapper;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Queries.TeamQueries;
using ZenBlog.Application.Features.Results.TeamResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.TeamHandlers;

public class GetTeamsQueryHandler(IRepository<Team> _repository,
                                  IMapper _mapper)
    : IRequestHandler<GetTeamsQuery, BaseResult<List<GetTeamsQueryResult>>>
{
    public async Task<BaseResult<List<GetTeamsQueryResult>>> Handle(GetTeamsQuery request, CancellationToken cancellationToken)
    {
        // Include: sosyal hesaplarý da tek sorguda getir (silinmiþler otomatik filtrelenir)
        var teams = await _repository.GetAllAsync(t => t.TeamSocialMedias);

        // Sitede eklenme sýrasýyla görünür
        var result = _mapper.Map<List<GetTeamsQueryResult>>(teams.OrderBy(t => t.CreatedAt));

        return BaseResult<List<GetTeamsQueryResult>>.Success(result);
    }
}