using AutoMapper;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Queries.BlogQueries;
using ZenBlog.Application.Features.Results.BlogResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.BlogHandlers
{
    public class GetBlogsByUserIdQueryHandler(IRepository<Blog> _repository,
                                              IMapper _mapper) : IRequestHandler<GetBlogsByUserIdQuery, BaseResult<List<GetBlogsByUserIdQueryResult>>>
    {
        public async Task<BaseResult<List<GetBlogsByUserIdQueryResult>>> Handle(GetBlogsByUserIdQuery request, CancellationToken cancellationToken)
        {
            var blogs = await _repository.GetAllFilterAsync(x => x.AppUserId == request.UserId);

            if (blogs is null)
                return BaseResult<List<GetBlogsByUserIdQueryResult>>.Fail("Bu userId sahip blog bulunamadý.");

            var mappedBlog = _mapper.Map<List<GetBlogsByUserIdQueryResult>>(blogs);

            return BaseResult<List<GetBlogsByUserIdQueryResult>>.Success(mappedBlog);
        }
    }
}
