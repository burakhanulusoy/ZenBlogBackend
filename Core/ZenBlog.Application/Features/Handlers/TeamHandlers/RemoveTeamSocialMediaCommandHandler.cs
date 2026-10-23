using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.TeamCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.TeamHandlers;

public class RemoveTeamSocialMediaCommandHandler(IRepository<TeamSocialMedia> _repository,
                                                 IUnitOfWork _unitOfWork)
    : IRequestHandler<RemoveTeamSocialMediaCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(RemoveTeamSocialMediaCommand request, CancellationToken cancellationToken)
    {
        var social = await _repository.GetByIdAsync(request.Id);
        if (social is null)
            return BaseResult<object>.Fail("Bu Id'ye sahip sosyal hesap bulunamadý.");

        _repository.Delete(social);
        var result = await _unitOfWork.SaveChangesAsync();

        return result
            ? BaseResult<object>.Success()
            : BaseResult<object>.Fail("Sosyal hesap silinirken bir sorun oluþtu.");
    }
}