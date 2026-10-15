using AutoMapper;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Results.SubCommentResults;
using ZenBlog.Domain.Entities;

public class GetSubCommentByIdQueryHandler(IRepository<SubComment> _repository,
                                           IMapper _mapper)
    : IRequestHandler<GetSubCommentByIdQuery, BaseResult<GetSubCommentByIdQueryResult>>
{
    public async Task<BaseResult<GetSubCommentByIdQueryResult>> Handle(GetSubCommentByIdQuery request, CancellationToken cancellationToken)
    {
        var subComment = await _repository.GetByIdAsync(request.Id);

        if (subComment is null)
            return BaseResult<GetSubCommentByIdQueryResult>.Fail("Bu Id'ye sahip cevap bulunamadý.");

        return BaseResult<GetSubCommentByIdQueryResult>.Success(_mapper.Map<GetSubCommentByIdQueryResult>(subComment));
    }
}