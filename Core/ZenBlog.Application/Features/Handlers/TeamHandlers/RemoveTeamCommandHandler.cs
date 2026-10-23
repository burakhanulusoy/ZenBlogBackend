using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.TeamCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.TeamHandlers;

public class RemoveTeamCommandHandler(IRepository<Team> _repository,
                                      IRepository<TeamSocialMedia> _socialRepository,
                                      IUnitOfWork _unitOfWork)
    : IRequestHandler<RemoveTeamCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(RemoveTeamCommand request, CancellationToken cancellationToken)
    {
        var team = await _repository.GetByIdAsync(request.Id);
        if (team is null)
            return BaseResult<object>.Fail("Bu Id'ye sahip ekip üyesi bulunamadý.");

        // Üyenin sosyal hesaplarý da gizlensin (soft delete); yoksa sahipsiz kalýrlar
        var socials = await _socialRepository.GetAllFilterAsync(s => s.TeamId == team.Id);
        foreach (var social in socials)
            _socialRepository.Delete(social);

        _repository.Delete(team);

        // Hepsi tek SaveChanges ? ya birlikte silinir ya hiçbiri
        var result = await _unitOfWork.SaveChangesAsync();

        return result
            ? BaseResult<object>.Success()
            : BaseResult<object>.Fail("Ekip üyesi silinirken bir sorun oluþtu.");
    }
}