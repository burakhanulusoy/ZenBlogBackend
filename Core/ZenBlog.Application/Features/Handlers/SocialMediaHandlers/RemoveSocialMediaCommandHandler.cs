using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.SocialMediaCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.SocialMediaHandlers
{
    public class RemoveSocialMediaCommandHandler(IRepository<SocialMedia> _repository,
                                                 IUnitOfWork _unitOfWork) : IRequestHandler<RemoveSocialMediaCommand, BaseResult<object>>
    {
        public async Task<BaseResult<object>> Handle(RemoveSocialMediaCommand request, CancellationToken cancellationToken)
        {

            var socialMedia = await _repository.GetByIdAsync(request.Id);

            if (socialMedia is null)
                return BaseResult<object>.Fail("Bu Id'ye sahip sosyal medya hesabýnýz bulunamadý");

            _repository.Delete(socialMedia);

            var result = await _unitOfWork.SaveChangesAsync();


            return result
                ? BaseResult<object>.Success()
                : BaseResult<object>.Fail("Sosyal Medya hesabýnýz silinirken bir sorun oluþtu.");



        }
    }
}
