using AutoMapper;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Queries.CategoryQueries;
using ZenBlog.Application.Features.Results.CategoryResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.CategoryHandlers;

public class GetCategoriesWithBlogsQueryHandler(IRepository<Category> _categoryRepository,
                                                IRepository<Blog> _blogRepository,
                                                IMapper _mapper)
    : IRequestHandler<GetCategoriesWithBlogsQuery, BaseResult<List<GetCategoriesWithBlogsQueryResult>>>
{
    // Her kategori bölümünde gösterilecek en fazla blog sayýsý
    private const int BlogCountPerCategory = 10;

    public async Task<BaseResult<List<GetCategoriesWithBlogsQueryResult>>> Handle(GetCategoriesWithBlogsQuery request, CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetAllAsync();

        var result = new List<GetCategoriesWithBlogsQueryResult>();

        foreach (var category in categories)
        {
            // Bu kategorinin sadece son 10 blogu (veritabanýnda TOP 10 olarak çalýþýr)
            var blogs = await _blogRepository.GetLatestAsync(x => x.CategoryId == category.Id,
                                                             BlogCountPerCategory);

            // Hiç blogu olmayan kategoriler ana sayfada boþ bölüm olarak görünmesin
            if (blogs.Count == 0)
                continue;

            // AppUser map'lenirken lazy loading ile yüklenir
            var mappedBlogs = _mapper.Map<List<CategoryBlogItemResult>>(blogs);

            result.Add(new GetCategoriesWithBlogsQueryResult(category.Id, category.Name, mappedBlogs));
        }

        return BaseResult<List<GetCategoriesWithBlogsQueryResult>>.Success(result);
    }
}