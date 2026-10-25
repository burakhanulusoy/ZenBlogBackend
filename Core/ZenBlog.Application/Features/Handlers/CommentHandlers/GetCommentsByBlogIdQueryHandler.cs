using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Queries.CommentQueries;
using ZenBlog.Application.Features.Results.CommentResults;

namespace ZenBlog.Application.Features.Handlers.CommentHandlers
{
    // DEÐÝÞTÝ: AutoMapper + lazy loading yerine projeksiyonlu okuma servisi
    public class GetCommentsByBlogIdQueryHandler(ICommentReadService _commentReadService)
        : IRequestHandler<GetCommentsByBlogIdQuery, BaseResult<List<GetCommentsByBlogIdQueryResult>>>
    {
        public async Task<BaseResult<List<GetCommentsByBlogIdQueryResult>>> Handle(GetCommentsByBlogIdQuery request, CancellationToken cancellationToken)
        {
            var comments = await _commentReadService.GetByBlogIdAsync(request.BlogId, cancellationToken);
            return BaseResult<List<GetCommentsByBlogIdQueryResult>>.Success(comments);
        }
    }
}