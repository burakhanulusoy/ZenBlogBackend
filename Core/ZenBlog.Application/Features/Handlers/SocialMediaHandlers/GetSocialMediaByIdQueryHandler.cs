using AutoMapper;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Queries.SocialMediaQueries;
using ZenBlog.Application.Features.Results.SocialMediaResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.SocialMediaHandlers
{
    public class GetSocialMediaByIdQueryHandler(IRepository<SocialMedia> _repository,
                                                IMapper _mapper) : IRequestHandler<GetSocialMediaByIdQuery, BaseResult<GetSocialMediaByIdQueryResult>>
    {
        public async Task<BaseResult<GetSocialMediaByIdQueryResult>> Handle(GetSocialMediaByIdQuery request, CancellationToken cancellationToken)
        {
            var socialMedia = await _repository.GetByIdAsync(request.Id);

            if (socialMedia is null)
                return BaseResult<GetSocialMediaByIdQueryResult>.Fail("Bu Id'ye sahip sosyal medya hesabý bulunamadý");

            var mappedSocialMedia =_mapper.Map<GetSocialMediaByIdQueryResult>(socialMedia);

            return BaseResult<GetSocialMediaByIdQueryResult>.Success(mappedSocialMedia);
        }
    }
}
