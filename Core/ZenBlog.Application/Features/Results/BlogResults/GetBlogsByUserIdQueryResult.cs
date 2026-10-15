using ZenBlog.Application.Features.Results.CategoryResults;
using ZenBlog.Application.Features.Results.UserResults;

namespace ZenBlog.Application.Features.Results.BlogResults;

public record GetBlogsByUserIdQueryResult(Guid Id,
                              string MainTitle,
                              string CoverImage,
                              string MainDescription,
                              Guid CategoryId,
                              GetCategoriesQueryResult Category,
                              Guid AppUserId,
                              GetUsersQueryResult AppUser,
                              DateTime CreatedAt);
