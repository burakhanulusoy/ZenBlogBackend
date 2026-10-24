namespace ZenBlog.Application.Features.Results.HomeResults;

// Ana sayfanýn tamamý tek cevapta
public record HomePageResult(List<HomeBlogResult> Slider,                        // son 3 blog
                             List<HomeBlogResult> Latest,                        // sonraki 7 blog
                             List<HomeBlogResult> Trending,                      // en çok yorum alan 5 blog
                             List<HomeCategorySectionResult> CategorySections);  // her kategorinin son 5 blogu

// Kartlarda gösterilen blog bilgisi (sadece gereken alanlar; e-posta gibi kiþisel bilgi YOK)
public record HomeBlogResult(Guid Id,
                             string MainTitle,
                             string MainDescription,
                             string CoverImage,
                             DateTime CreatedAt,
                             Guid CategoryId,
                             string CategoryName,
                             Guid AuthorId,
                             string AuthorName,
                             string? AuthorImage,
                             int CommentCount);   // yorum + alt yorum

// Bir kategori bölümü
public record HomeCategorySectionResult(Guid CategoryId,
                                        string CategoryName,
                                        List<HomeBlogResult> Blogs);