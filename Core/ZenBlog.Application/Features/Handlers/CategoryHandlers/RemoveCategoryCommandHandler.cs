using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.CategoryCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.CategoryHandlers
{
    public class RemoveCategoryCommandHandler(IRepository<Category> _repository,
                                              IUnitOfWork _unitOfWork) : IRequestHandler<RemoveCategoryCommand, BaseResult<object>>
    {
        public async Task<BaseResult<object>> Handle(RemoveCategoryCommand request, CancellationToken cancellationToken)
        {
            
            var category = await _repository.GetByIdAsync(request.Id);

            if(category is null)
            {
                return BaseResult<object>.Fail("Bu id sahip kategori bulunamadý.");
            }

            _repository.Delete(category);

            var result = await _unitOfWork.SaveChangesAsync();

            return result ? BaseResult<object>.Success() : BaseResult<object>.Fail("Bir sorun oluþtu");





        }
    }
}
