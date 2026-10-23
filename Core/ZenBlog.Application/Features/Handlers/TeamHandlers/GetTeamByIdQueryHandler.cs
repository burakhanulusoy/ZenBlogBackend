using AutoMapper;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Queries.TeamQueries;
using ZenBlog.Application.Features.Results.TeamResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.TeamHandlers;

public class GetTeamByIdQueryHandler(IRepository<Team> _repository,
                                     IMapper _mapper)
    : IRequestHandler<GetTeamByIdQuery, BaseResult<GetTeamByIdQueryResult>>
{
    public async Task<BaseResult<GetTeamByIdQueryResult>> Handle(GetTeamByIdQuery request, CancellationToken cancellationToken)
    {
        var team = await _repository.GetByIdAsync(request.Id);
        if (team is null)
            return BaseResult<GetTeamByIdQueryResult>.Fail("Bu Id'ye sahip ekip üyesi bulunamadý.");

        // TeamSocialMedias lazy loading ile, eþleme sýrasýnda yüklenir
        return BaseResult<GetTeamByIdQueryResult>.Success(_mapper.Map<GetTeamByIdQueryResult>(team));
    }
}