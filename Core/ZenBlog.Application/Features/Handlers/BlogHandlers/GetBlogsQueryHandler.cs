using AutoMapper;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Queries.BlogQueries;
using ZenBlog.Application.Features.Results.BlogResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.BlogHandlers
{
    public class GetBlogsQueryHandler(IRepository<Blog> _repository,
                                       IMapper _mapper) : IRequestHandler<GetBlogsQuery, BaseResult<List<GetBlogsQueryResult>>>
    {
        public async Task<BaseResult<List<GetBlogsQueryResult>>> Handle(GetBlogsQuery request, CancellationToken cancellationToken)
        {
            var blogs = await _repository.GetAllAsync();

            var mappedBlogs=_mapper.Map<List<GetBlogsQueryResult>>(blogs);

            return BaseResult<List<GetBlogsQueryResult>>.Success(mappedBlogs);
        }
    }
}
