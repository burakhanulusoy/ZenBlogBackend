using AutoMapper;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Queries.CommentQueries;
using ZenBlog.Application.Features.Results.CommentResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.CommentHandlers;

public class GetCommentByIdQueryHandler(IRepository<Comment> _repository,
                                        IMapper _mapper)
    : IRequestHandler<GetCommentByIdQuery, BaseResult<GetCommentByIdQueryResult>>
{
    public async Task<BaseResult<GetCommentByIdQueryResult>> Handle(GetCommentByIdQuery request, CancellationToken cancellationToken)
    {
        var comment = await _repository.GetByIdAsync(request.Id);

        if (comment is null)
            return BaseResult<GetCommentByIdQueryResult>.Fail("Bu Id'ye sahip yorum bulunamadý.");

        var result = _mapper.Map<GetCommentByIdQueryResult>(comment);

        return BaseResult<GetCommentByIdQueryResult>.Success(result);
    }
}