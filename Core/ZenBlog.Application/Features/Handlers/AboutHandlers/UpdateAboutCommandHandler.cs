using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.AboutCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.AboutHandlers;

public class UpdateAboutCommandHandler(IRepository<About> _repository,
                                       IUnitOfWork _unitOfWork,
                                       IMapper _mapper,
                                       IFileService _fileService)
    : IRequestHandler<UpdateAboutCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(UpdateAboutCommand request, CancellationToken cancellationToken)
    {
        var about = await _repository.GetByIdAsync(request.Id!.Value);

        if (about is null)
            return BaseResult<object>.Fail("Bu Id'ye sahip hakkýmýzda bilgisi bulunamadý.");

        // Çektiðimiz nesnenin üzerine yaz (yeni nesne oluþturmuyoruz)
        _mapper.Map(request, about);

        var oldUrls = new List<string>();   // Kayýt BAÞARILI olursa silinecek eski resimler
        var newUrls = new List<string>();   // Kayýt BAÞARISIZ olursa silinecek yeni resimler

        if (request.Image1 is not null)
        {
            oldUrls.Add(about.Image1);
            about.Image1 = await UploadAsync(request.Image1, cancellationToken);
            newUrls.Add(about.Image1);
        }

        if (request.Image2 is not null)
        {
            oldUrls.Add(about.Image2);
            about.Image2 = await UploadAsync(request.Image2, cancellationToken);
            newUrls.Add(about.Image2);
        }

        if (request.Image3 is not null)
        {
            oldUrls.Add(about.Image3);
            about.Image3 = await UploadAsync(request.Image3, cancellationToken);
            newUrls.Add(about.Image3);
        }

        _repository.Update(about);
        var result = await _unitOfWork.SaveChangesAsync();

        if (!result)
        {
            // Kayýt baþarýsýz ? eski resimler duruyor, sadece yeni yüklenenleri sil
            foreach (var url in newUrls)
                await _fileService.DeleteAsync(url, cancellationToken);

            return BaseResult<object>.Fail("Hakkýmýzda bilgisi güncellenirken bir sorun oluþtu.");
        }

        // Kayýt baþarýlý ? artýk kullanýlmayan eski resimleri sil
        foreach (var url in oldUrls)
            await _fileService.DeleteAsync(url, cancellationToken);

        return BaseResult<object>.Success();
    }

    private async Task<string> UploadAsync(IFormFile file, CancellationToken cancellationToken)
    {
        await using var stream = file.OpenReadStream();
        return await _fileService.UploadAsync(stream, file.FileName, ImageFolders.Abouts, cancellationToken);
    }
}