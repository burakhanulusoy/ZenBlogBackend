using AutoMapper;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Queries.AboutQueries;
using ZenBlog.Application.Features.Results.AboutResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.AboutHandlers;

public class GetAboutsQueryHandler(IRepository<About> _repository,
                                   IMapper _mapper)
    : IRequestHandler<GetAboutsQuery, BaseResult<List<GetAboutsQueryResult>>>
{
    public async Task<BaseResult<List<GetAboutsQueryResult>>> Handle(GetAboutsQuery request, CancellationToken cancellationToken)
    {
        var abouts = await _repository.GetAllAsync();

        var result = _mapper.Map<List<GetAboutsQueryResult>>(abouts.OrderByDescending(x => x.CreatedAt));

        return BaseResult<List<GetAboutsQueryResult>>.Success(result);
    }
}