using AutoMapper;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.TeamCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.TeamHandlers;

public class CreateTeamSocialMediaCommandHandler(IRepository<Team> _teamRepository,
                                                 IRepository<TeamSocialMedia> _repository,
                                                 IUnitOfWork _unitOfWork,
                                                 IMapper _mapper)
    : IRequestHandler<CreateTeamSocialMediaCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(CreateTeamSocialMediaCommand request, CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetByIdAsync(request.TeamId);
        if (team is null)
            return BaseResult<object>.Fail("Bu Id'ye sahip ekip üyesi bulunamadý.");

        // Ayný platform bir üyede iki kez olmasýn
        var existing = await _repository.GetAllFilterAsync(
            s => s.TeamId == request.TeamId && s.SocialMediaName == request.SocialMediaName);
        if (existing.Count > 0)
            return BaseResult<object>.Fail("SocialMediaName", $"Bu üyenin zaten bir {request.SocialMediaName} hesabý var.");

        var social = _mapper.Map<TeamSocialMedia>(request);
        social.Icon = TeamSocialPlatforms.Icons[request.SocialMediaName!];   // ikon OTOMATÝK

        await _repository.CreateAsync(social);
        var result = await _unitOfWork.SaveChangesAsync();

        return result
            ? BaseResult<object>.Success()
            : BaseResult<object>.Fail("Sosyal hesap eklenirken bir sorun oluþtu.");
    }
}