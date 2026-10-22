using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.SocialMediaCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.SocialMediaHandlers
{
    public class CreateSocialMediaCommandHandler(IRepository<SocialMedia> _repository,
                                                 IMapper _mapper,
                                                 IUnitOfWork _unitOfWork,
                                                 IFileService _fileService) : IRequestHandler<CreateSocialMediaCommand, BaseResult<object>>
    {
        public async Task<BaseResult<object>> Handle(CreateSocialMediaCommand request, CancellationToken cancellationToken)
        {
           
            var mappedSocialMedia = _mapper.Map<SocialMedia>(request);

            mappedSocialMedia.Icon = await UploadAsync(request.Icon!, cancellationToken);

            await _repository.CreateAsync(mappedSocialMedia);

            var result = await _unitOfWork.SaveChangesAsync();

            if(!result)
            {
                await _fileService.DeleteAsync(mappedSocialMedia.Icon, cancellationToken);
                return BaseResult<object>.Fail("Sosyal Medya yüklerken bir hata oluþtu");
            }

            return BaseResult<object>.Success();


        }

        private async Task<string> UploadAsync(IFormFile file, CancellationToken cancellationToken)
        {
            await using var stream = file.OpenReadStream();
            return await _fileService.UploadAsync(stream, file.FileName, ImageFolders.SocialMedias, cancellationToken);   // DEÐÝÞTÝ
        }

    }
}
