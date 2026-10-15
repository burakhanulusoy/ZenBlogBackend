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

public class UpdateBlogCommandHandler(IRepository<Blog> _repository,
                                      IUnitOfWork _unitOfWork,
                                      IRepository<Category> _categoryRepository,
                                      IMapper _mapper,
                                      IFileService _fileService)
    : IRequestHandler<UpdateBlogCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(UpdateBlogCommand request, CancellationToken cancellationToken)
    {
        var blog = await _repository.GetByIdAsync(request.Id);

        if (blog is null)
            return BaseResult<object>.Fail("Bu Id'ye sahip blog bulunamadý.");

        // Kategori gerçekten var mý? (resim yüklemeden önce)
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId!.Value);

        if (category is null)
            return BaseResult<object>.Fail("Seçilen kategori bulunamadý.");

        _mapper.Map(request, blog);

        var oldUrls = new List<string>();   // Kayýt BAÞARILI olursa silinecek eski resimler
        var newUrls = new List<string>();   // Kayýt BAÞARISIZ olursa silinecek yeni resimler

        // Sadece yeni resim seçilen alanlar deðiþir
        if (request.CoverImage is not null)
        {
            oldUrls.Add(blog.CoverImage);
            blog.CoverImage = await UploadAsync(request.CoverImage, cancellationToken);
            newUrls.Add(blog.CoverImage);
        }

        if (request.BlogImage1 is not null)
        {
            oldUrls.Add(blog.BlogImage1);
            blog.BlogImage1 = await UploadAsync(request.BlogImage1, cancellationToken);
            newUrls.Add(blog.BlogImage1);
        }

        if (request.BlogImage2 is not null)
        {
            oldUrls.Add(blog.BlogImage2);
            blog.BlogImage2 = await UploadAsync(request.BlogImage2, cancellationToken);
            newUrls.Add(blog.BlogImage2);
        }

        _repository.Update(blog);
        var result = await _unitOfWork.SaveChangesAsync();

        if (!result)
        {
            // Kayýt baþarýsýz ? veritabaný deðiþmedi, blog ESKÝ resimleri gösteriyor.
            // Sadece yeni yüklenenleri siliyoruz.
            foreach (var url in newUrls)
                await _fileService.DeleteAsync(url, cancellationToken);

            return BaseResult<object>.Fail("Blog güncellenirken bir sorun oluþtu.");
        }

        // Kayýt baþarýlý ? artýk kullanýlmayan eski resimleri sil
        foreach (var url in oldUrls)
            await _fileService.DeleteAsync(url, cancellationToken);

        return BaseResult<object>.Success();
    }

    private async Task<string> UploadAsync(IFormFile file, CancellationToken cancellationToken)
    {
        await using var stream = file.OpenReadStream();
        return await _fileService.UploadAsync(stream, file.FileName, ImageFolders.Blogs, cancellationToken);
    }
}