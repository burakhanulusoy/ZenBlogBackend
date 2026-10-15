using ZenBlog.Application.Features.Results.UserResults;

namespace ZenBlog.Application.Features.Results.CategoryResults;


// Kategori bölümünün içindeki bir blog kartý
public record CategoryBlogItemResult(Guid Id,
                                     string MainTitle,
                                     string CoverImage,
                                     string MainDescription,
                                     DateTime CreatedAt,
                                     GetUsersQueryResult AppUser);
