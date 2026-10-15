using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.BlogCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.BlogHandlers;

public class CreateBlogCommandHandler(IRepository<Blog> _repository,
                                      IRepository<Category> _categoryRepository,
                                      IUnitOfWork _unitOfWork,
                                      IMapper _mapper,
                                      IFileService _fileService)
    : IRequestHandler<CreateBlogCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(CreateBlogCommand request, CancellationToken cancellationToken)
    {
        // Kategori gerçekten var mý? Resim yüklemeden ÖNCE kontrol ediyoruz;
        // yanlýþsa Azure'a hiç dosya gitmiyor.
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId!.Value);

        if (category is null)
            return BaseResult<object>.Fail("Seçilen kategori bulunamadý.");

        var blog = _mapper.Map<Blog>(request);

        // Resimleri yükle, adreslerini blog'a yaz
        blog.CoverImage = await UploadAsync(request.CoverImage!, cancellationToken);
        blog.BlogImage1 = await UploadAsync(request.BlogImage1!, cancellationToken);
        blog.BlogImage2 = await UploadAsync(request.BlogImage2!, cancellationToken);

        await _repository.CreateAsync(blog);
        var result = await _unitOfWork.SaveChangesAsync();

        if (!result)
        {
            // Kayýt baþarýsýz ? az önce yüklenen resimler boþuna yer kaplamasýn
            await _fileService.DeleteAsync(blog.CoverImage, cancellationToken);
            await _fileService.DeleteAsync(blog.BlogImage1, cancellationToken);
            await _fileService.DeleteAsync(blog.BlogImage2, cancellationToken);

            return BaseResult<object>.Fail("Blog oluþturulurken bir sorun oluþtu.");
        }

        return BaseResult<object>.Success();
    }

    private async Task<string> UploadAsync(IFormFile file, CancellationToken cancellationToken)
    {
        await using var stream = file.OpenReadStream();
        return await _fileService.UploadAsync(stream, file.FileName, ImageFolders.Blogs, cancellationToken);
    }
}