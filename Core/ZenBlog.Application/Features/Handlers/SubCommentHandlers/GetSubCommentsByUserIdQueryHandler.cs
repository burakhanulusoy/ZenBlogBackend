using AutoMapper;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Results.SubCommentResults;
using ZenBlog.Domain.Entities;

public class GetSubCommentsByUserIdQueryHandler(IRepository<SubComment> _repository,
                                                IMapper _mapper)
    : IRequestHandler<GetSubCommentsByUserIdQuery, BaseResult<List<GetSubCommentsByUserIdQueryResult>>>
{
    public async Task<BaseResult<List<GetSubCommentsByUserIdQueryResult>>> Handle(GetSubCommentsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var subComments = await _repository.GetAllFilterAsync(x => x.AppUserId == request.UserId);

        // Kullanýcýnýn son aktivitesi en üstte: yeniden eskiye
        var result = _mapper.Map<List<GetSubCommentsByUserIdQueryResult>>(subComments.OrderByDescending(x => x.CreatedAt));

        return BaseResult<List<GetSubCommentsByUserIdQueryResult>>.Success(result);
    }
}