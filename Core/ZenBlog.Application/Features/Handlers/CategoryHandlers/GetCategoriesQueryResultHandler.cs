using AutoMapper;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Queries.CategoryQueries;
using ZenBlog.Application.Features.Results.CategoryResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.CategoryHandlers
{
    public class GetCategoriesQueryResultHandler(IRepository<Category> _repository,
                                                 IMapper _mapper) : IRequestHandler<GetCategoriesQuery, BaseResult<List<GetCategoriesQueryResult>>>
    {
        public async Task<BaseResult<List<GetCategoriesQueryResult>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _repository.GetAllAsync();
            
            var mappedCategories = _mapper.Map<List<GetCategoriesQueryResult>>(categories);

            return BaseResult<List<GetCategoriesQueryResult>>.Success(mappedCategories);
        }
    }
}
