using ZenBlog.Application.Features.Results.HomeResults;

namespace ZenBlog.Application.Features.Results.BlogResults;

// Kategori sayfasý: kategori bilgisi + sayfalanmýþ bloglar
// Blog kartlarý ana sayfadakiyle ayný biçimde (HomeBlogResult)
public record CategoryPageResult(Guid CategoryId,
                                 string CategoryName,
                                 int TotalCount,    // kategorideki toplam yazý
                                 int Page,          // þu anki sayfa
                                 int PageSize,
                                 int TotalPages,
                                 List<HomeBlogResult> Blogs);