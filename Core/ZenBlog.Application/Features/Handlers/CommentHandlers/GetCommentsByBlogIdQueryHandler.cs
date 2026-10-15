using AutoMapper;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Queries.CommentQueries;
using ZenBlog.Application.Features.Results.CommentResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.CommentHandlers
{
    public class GetCommentsByBlogIdQueryHandler(IRepository<Comment> _repository,
                                                 IMapper _mapper) : IRequestHandler<GetCommentsByBlogIdQuery, BaseResult<List<GetCommentsByBlogIdQueryResult>>>
    {
        public async Task<BaseResult<List<GetCommentsByBlogIdQueryResult>>> Handle(GetCommentsByBlogIdQuery request, CancellationToken cancellationToken)
        {
          
            var comments = await _repository.GetAllFilterAsync(x=>x.BlogId == request.BlogId);

            var newComments = comments.OrderBy(x => x.CreatedAt);

            var mappedComments = _mapper.Map<List<GetCommentsByBlogIdQueryResult>>(newComments);

            return BaseResult<List<GetCommentsByBlogIdQueryResult>>.Success(mappedComments);
        }
    }
}
