using AutoMapper;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.TeamCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.TeamHandlers;

public class UpdateTeamSocialMediaCommandHandler(IRepository<TeamSocialMedia> _repository,
                                                 IUnitOfWork _unitOfWork,
                                                 IMapper _mapper)
    : IRequestHandler<UpdateTeamSocialMediaCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(UpdateTeamSocialMediaCommand request, CancellationToken cancellationToken)
    {
        var social = await _repository.GetByIdAsync(request.Id);
        if (social is null)
            return BaseResult<object>.Fail("Bu Id'ye sahip sosyal hesap bulunamadý.");

        // Platform deðiþiyorsa: ayný üyede o platform zaten var mý?
        if (social.SocialMediaName != request.SocialMediaName)
        {
            var existing = await _repository.GetAllFilterAsync(
                s => s.TeamId == social.TeamId && s.SocialMediaName == request.SocialMediaName && s.Id != social.Id);
            if (existing.Count > 0)
                return BaseResult<object>.Fail("SocialMediaName", $"Bu üyenin zaten bir {request.SocialMediaName} hesabý var.");
        }

        // SocialMediaName ve Url kopyalanýr (Id, TeamId, Icon eþlemede Ignore edildi)
        _mapper.Map(request, social);
        social.Icon = TeamSocialPlatforms.Icons[request.SocialMediaName!];

        _repository.Update(social);
        var result = await _unitOfWork.SaveChangesAsync();

        return result
            ? BaseResult<object>.Success()
            : BaseResult<object>.Fail("Sosyal hesap güncellenirken bir sorun oluþtu.");
    }
}