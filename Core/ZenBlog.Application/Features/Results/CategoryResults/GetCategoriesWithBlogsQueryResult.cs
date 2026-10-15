using ZenBlog.Application.Features.Results.UserResults;

namespace ZenBlog.Application.Features.Results.CategoryResults;

// Ana sayfadaki bir kategori bölümü
public record GetCategoriesWithBlogsQueryResult(Guid Id,
                                                string Name,
                                                List<CategoryBlogItemResult> Blogs);
