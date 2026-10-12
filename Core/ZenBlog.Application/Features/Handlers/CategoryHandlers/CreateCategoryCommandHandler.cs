using AutoMapper;
using FluentValidation;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.CategoryCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.CategoryHandlers
{
    public class CreateCategoryCommandHandler(IRepository<Category> _repository,
                                              IMapper _mapper,
                                              IUnitOfWork _unitOfWork
                                              ) : IRequestHandler<CreateCategoryCommand, BaseResult<object>>
    {
        public async Task<BaseResult<object>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
           

            var mappedCategory = _mapper.Map<Category>(request);

            await _repository.CreateAsync(mappedCategory);

            var result = await _unitOfWork.SaveChangesAsync();

            return result ? BaseResult<object>.Success() : BaseResult<object>.Fail("Kategori oluþturulurken bir sorun oluþtu");

        }
    }
}
