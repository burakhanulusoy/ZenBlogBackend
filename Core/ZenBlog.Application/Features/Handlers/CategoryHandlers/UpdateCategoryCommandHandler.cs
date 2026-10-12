using AutoMapper;
using FluentValidation;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.CategoryCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.CategoryHandlers
{
    public class UpdateCategoryCommandHandler(IRepository<Category> _repository,
                                              IMapper _mapper,
                                              IUnitOfWork _unitOfWork) : IRequestHandler<UpdateCategoryCommand, BaseResult<object>>
    {
        public async Task<BaseResult<object>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            
         

            var category = await _repository.GetByIdAsync(request.Id);

            if(category is null)
            {
                return BaseResult<object>.Fail("Bu id sahip kategori bulunamdý");
            }

            var mappedCategory = _mapper.Map(request,category);

            _repository.Update(mappedCategory);

            var result = await _unitOfWork.SaveChangesAsync();

            return result ? BaseResult<object>.Success() : BaseResult<object>.Fail("Güncelleme yapýlýrken bir sorun oluþtu");





        }
    }
}
