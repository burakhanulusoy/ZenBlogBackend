using AutoMapper;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Queries.BlogQueries;
using ZenBlog.Application.Features.Results.BlogResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.BlogHandlers
{
    public class GetBlogsByCategoryIdQueryHandler(IRepository<Blog> _repository,
                                                  IMapper _mapper) : IRequestHandler<GetBlogsByCategoryIdQuery, BaseResult<List<GetBlogsByCategoryIdQueryResult>>>
    {
        public async Task<BaseResult<List<GetBlogsByCategoryIdQueryResult>>> Handle(GetBlogsByCategoryIdQuery request, CancellationToken cancellationToken)
        {
            var blogs = await _repository.GetAllFilterAsync(x => x.CategoryId == request.CategoryId);

            //if (blogs is null)
            //    return BaseResult<List<GetBlogsByCategoryIdQueryResult>>.Fail("Bu kategori id sahip blog bulunamadý.");
            //grek yok cunku tolist() null deðil boþ liste döndiürü

            var mappedBlog = _mapper.Map<List<GetBlogsByCategoryIdQueryResult>>(blogs);

            return BaseResult<List<GetBlogsByCategoryIdQueryResult>>.Success(mappedBlog);

        }
    }
}
