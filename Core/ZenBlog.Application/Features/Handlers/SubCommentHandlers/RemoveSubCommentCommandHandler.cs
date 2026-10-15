using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.SubCommentHandlers
{
    public class RemoveSubCommentCommandHandler(IRepository<SubComment> _repository,
                                             IUnitOfWork _unitOfWork)
     : IRequestHandler<RemoveSubCommentCommand, BaseResult<object>>
    {
        public async Task<BaseResult<object>> Handle(RemoveSubCommentCommand request, CancellationToken cancellationToken)
        {
            var subComment = await _repository.GetByIdAsync(request.Id);

            if (subComment is null)
                return BaseResult<object>.Fail("Bu Id'ye sahip cevap bulunamadý.");

            _repository.Delete(subComment);
            var result = await _unitOfWork.SaveChangesAsync();

            return result
                ? BaseResult<object>.Success()
                : BaseResult<object>.Fail("Cevap silinirken bir sorun oluþtu.");
        }
    }
}
