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
    public class UpdateSocialMediaCommandHandler(IRepository<SocialMedia> _repository,
                                                 IUnitOfWork _unitOfWork,
                                                 IMapper _mapper,
                                                 IFileService _fileService) : IRequestHandler<UpdateSocialMediaCommand, BaseResult<object>>
    {
        public async Task<BaseResult<object>> Handle(UpdateSocialMediaCommand request, CancellationToken cancellationToken)
        {

            var socialMedia = await _repository.GetByIdAsync(request.Id);

            if (socialMedia is null)
            {
                return BaseResult<object>.Fail("Bu Id'ye sahip sosyal medya hesabýnýz bulunamadý.");
            }

            _mapper.Map(request, socialMedia);

            var oldUrls = new List<string>();   // Kayýt BAÞARILI olursa silinecek eski resimler
            var newUrls = new List<string>();   // Kayýt BAÞARISIZ olursa silinecek yeni resimler


            if (request.Icon is not null)
            {
                oldUrls.Add(socialMedia.Icon);
                socialMedia.Icon = await UploadAsync(request.Icon, cancellationToken);
                newUrls.Add(socialMedia.Icon);
            }

            _repository.Update(socialMedia);
            var result = await _unitOfWork.SaveChangesAsync();


            if(!result)
            {
                foreach (var image in newUrls)
                    await _fileService.DeleteAsync(image, cancellationToken);

                return BaseResult<object>.Fail("Sosyal Medya hesabýnýz güncellenirken bir sorun oluþtu");
            }

            //kayýt baþarýlý

            foreach(var image in oldUrls)
                await _fileService.DeleteAsync(image,cancellationToken);

            return BaseResult<object>.Success();



        }



        private async Task<string> UploadAsync(IFormFile file, CancellationToken cancellationToken)
        {
            await using var stream = file.OpenReadStream();
            return await _fileService.UploadAsync(stream, file.FileName, ImageFolders.SocialMedias, cancellationToken);   // DEÐÝÞTÝ
        }

    }
}
