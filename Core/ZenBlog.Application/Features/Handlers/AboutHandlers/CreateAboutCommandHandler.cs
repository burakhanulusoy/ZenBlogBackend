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

public class CreateAboutCommandHandler(IRepository<About> _repository,
                                       IUnitOfWork _unitOfWork,
                                       IMapper _mapper,
                                       IFileService _fileService)
    : IRequestHandler<CreateAboutCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(CreateAboutCommand request, CancellationToken cancellationToken)
    {
        var about = _mapper.Map<About>(request);

        // Resimleri yükle, adreslerini yaz
        about.Image1 = await UploadAsync(request.Image1!, cancellationToken);
        about.Image2 = await UploadAsync(request.Image2!, cancellationToken);
        about.Image3 = await UploadAsync(request.Image3!, cancellationToken);

        await _repository.CreateAsync(about);
        var result = await _unitOfWork.SaveChangesAsync();

        if (!result)
        {
            // Kayýt baþarýsýz ? yüklenen resimler boþuna yer kaplamasýn
            await _fileService.DeleteAsync(about.Image1, cancellationToken);
            await _fileService.DeleteAsync(about.Image2, cancellationToken);
            await _fileService.DeleteAsync(about.Image3, cancellationToken);

            return BaseResult<object>.Fail("Hakkýmýzda bilgisi oluþturulurken bir sorun oluþtu.");
        }

        return BaseResult<object>.Success();
    }

    private async Task<string> UploadAsync(IFormFile file, CancellationToken cancellationToken)
    {
        await using var stream = file.OpenReadStream();
        return await _fileService.UploadAsync(stream, file.FileName, ImageFolders.Abouts, cancellationToken);
    }
}