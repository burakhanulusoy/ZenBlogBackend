using ZenBlog.Application.Features.Results.CategoryResults;
using ZenBlog.Application.Features.Results.UserResults;

namespace ZenBlog.Application.Features.Results.BlogResults;

public record GetBlogByIdQueryResult(Guid Id,
                                     string MainTitle,
                                     string CoverImage,
                                     string BlogImage1,
                                     string Title1,
                                     string Description1,
                                     string MainDescription,
                                     string Description2,
                                     string Title2,
                                     string Description3,
                                     string BlogImage2,
                                     string Title3,
                                     string Description4,
                                     Guid CategoryId,
                                     GetCategoriesQueryResult Category,
                                     Guid AppUserId,
                                     GetUsersQueryResult AppUser,
                                     DateTime CreatedAt
                                     );