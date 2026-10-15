using AutoMapper;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Queries.CommentQueries;
using ZenBlog.Application.Features.Results.CommentResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.CommentHandlers;

public class GetCommentsByUserIdQueryHandler(IRepository<Comment> _repository,
                                             IMapper _mapper)
    : IRequestHandler<GetCommentsByUserIdQuery, BaseResult<List<GetCommentsByUserIdQueryResult>>>
{
    public async Task<BaseResult<List<GetCommentsByUserIdQueryResult>>> Handle(GetCommentsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var comments = await _repository.GetAllFilterAsync(x => x.AppUserId == request.UserId);

        // Kullanýcýnýn en son yaptýðý yorum en üstte
        var orderedComments = comments.OrderByDescending(x => x.CreatedAt);

        var result = _mapper.Map<List<GetCommentsByUserIdQueryResult>>(orderedComments);

        return BaseResult<List<GetCommentsByUserIdQueryResult>>.Success(result);
    }
}