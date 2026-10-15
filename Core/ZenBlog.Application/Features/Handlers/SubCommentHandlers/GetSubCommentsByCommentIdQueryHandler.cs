using AutoMapper;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Queries.SubCommentQueries;
using ZenBlog.Application.Features.Results.SubCommentResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.SubCommentHandlers;

public class GetSubCommentsByCommentIdQueryHandler(IRepository<SubComment> _repository,
                                                   IMapper _mapper)
    : IRequestHandler<GetSubCommentsByCommentIdQuery, BaseResult<List<GetSubCommentsByCommentIdQueryResult>>>
{
    public async Task<BaseResult<List<GetSubCommentsByCommentIdQueryResult>>> Handle(GetSubCommentsByCommentIdQuery request, CancellationToken cancellationToken)
    {
        var subComments = await _repository.GetAllFilterAsync(x => x.CommentId == request.CommentId);

        // Cevaplar sohbet sýrasýyla: eskiden yeniye
        var result = _mapper.Map<List<GetSubCommentsByCommentIdQueryResult>>(subComments.OrderBy(x => x.CreatedAt));

        return BaseResult<List<GetSubCommentsByCommentIdQueryResult>>.Success(result);
    }
}