using AutoMapper;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Queries.BlogQueries;
using ZenBlog.Application.Features.Results.BlogResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.BlogHandlers
{
    public class GetBlogByIdQueryHandler(IRepository<Blog> _repository,
                                         IMapper mapper) : IRequestHandler<GetBlogByIdQuery, BaseResult<GetBlogByIdQueryResult>>
    {
        public async Task<BaseResult<GetBlogByIdQueryResult>> Handle(GetBlogByIdQuery request, CancellationToken cancellationToken)
        {

            var blog = await _repository.GetByIdAsync(request.id);

            if(blog is null)
            {
                return BaseResult<GetBlogByIdQueryResult>.Fail("Blog Not Found");
            }

            var mappedBlog=mapper.Map<GetBlogByIdQueryResult>(blog);

            return BaseResult<GetBlogByIdQueryResult>.Success(mappedBlog);





        }
    }
}
