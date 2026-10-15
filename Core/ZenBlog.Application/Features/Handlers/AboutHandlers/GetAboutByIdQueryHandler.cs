using AutoMapper;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Queries.AboutQueries;
using ZenBlog.Application.Features.Results.AboutResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.AboutHandlers
{
    public class GetAboutByIdQueryHandler(IRepository<About> _repository,
                                      IMapper _mapper)
    : IRequestHandler<GetAboutByIdQuery, BaseResult<GetAboutByIdQueryResult>>
    {
        public async Task<BaseResult<GetAboutByIdQueryResult>> Handle(GetAboutByIdQuery request, CancellationToken cancellationToken)
        {
            var about = await _repository.GetByIdAsync(request.Id);

            if (about is null)
                return BaseResult<GetAboutByIdQueryResult>.Fail("Bu Id'ye sahip hakkýmýzda bilgisi bulunamadý.");

            return BaseResult<GetAboutByIdQueryResult>.Success(_mapper.Map<GetAboutByIdQueryResult>(about));
        }
    }
}
