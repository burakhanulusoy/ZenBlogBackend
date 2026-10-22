using AutoMapper;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Queries.SocialMediaQueries;
using ZenBlog.Application.Features.Results.SocialMediaResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.SocialMediaHandlers
{
    public class GetSocialMediasQueryHandler(IRepository<SocialMedia> _repository,
                                             IMapper _mapper) : IRequestHandler<GetSocialMediasQuery, BaseResult<List<GetSocialMediasQueryResult>>>
    {
        public async Task<BaseResult<List<GetSocialMediasQueryResult>>> Handle(GetSocialMediasQuery request, CancellationToken cancellationToken)
        {

            var socialMedias = await _repository.GetAllAsync();
            
            var mappedSocialMedias =_mapper.Map<List<GetSocialMediasQueryResult>>(socialMedias);

            return BaseResult<List<GetSocialMediasQueryResult>>.Success(mappedSocialMedias);



        }
    }
}
