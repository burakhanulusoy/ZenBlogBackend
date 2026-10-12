using AutoMapper;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Queries.CategoryQueries;
using ZenBlog.Application.Features.Results.CategoryResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.CategoryHandlers
{
    public class GetCategoryByIdQueryResultHandler(IRepository<Category> _repository,
                                                   IMapper _mapper) : IRequestHandler<GetCategoryByIdQuery, BaseResult<GetCategoryByIdQueryResult>>
    {
        public async Task<BaseResult<GetCategoryByIdQueryResult>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            
            var category = await _repository.GetByIdAsync(request.Id);

            if (category is null)
            {
                return BaseResult<GetCategoryByIdQueryResult>.Fail("Category Not Found.");
            }

            var mappedCategory = _mapper.Map<GetCategoryByIdQueryResult>(category);

            return BaseResult<GetCategoryByIdQueryResult>.Success(mappedCategory);

        }
    }
}
