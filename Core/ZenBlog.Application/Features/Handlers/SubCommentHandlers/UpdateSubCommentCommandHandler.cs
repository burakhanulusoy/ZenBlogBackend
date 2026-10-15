using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.SubCommentHandlers
{
    public class UpdateSubCommentCommandHandler(IRepository<SubComment> _repository,
                                               IUnitOfWork _unitOfWork)
      : IRequestHandler<UpdateSubCommentCommand, BaseResult<object>>
    {
        public async Task<BaseResult<object>> Handle(UpdateSubCommentCommand request, CancellationToken cancellationToken)
        {
            var subComment = await _repository.GetByIdAsync(request.Id!.Value);

            if (subComment is null)
                return BaseResult<object>.Fail("Bu Id'ye sahip cevap bulunamadý.");

            subComment.Body = request.Body!;

            _repository.Update(subComment);
            var result = await _unitOfWork.SaveChangesAsync();

            return result
                ? BaseResult<object>.Success()
                : BaseResult<object>.Fail("Cevap güncellenirken bir sorun oluþtu.");
        }
    }
}   
