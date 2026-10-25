using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.SubCommentCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.SubCommentHandlers;

public class UpdateSubCommentCommandHandler(IRepository<SubComment> _repository,
                                            IUnitOfWork _unitOfWork,
                                            ICurrentUserService _currentUser)
    : IRequestHandler<UpdateSubCommentCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(UpdateSubCommentCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return BaseResult<object>.Fail("Kullanýcý bilgisi alýnamadý. Lütfen tekrar giriþ yapýn.");

        var subComment = await _repository.GetByIdAsync(request.Id!.Value);
        if (subComment is null)
            return BaseResult<object>.Fail("Bu Id'ye sahip yanýt bulunamadý.");

        // Yanýtý SADECE sahibi düzenleyebilir
        if (subComment.AppUserId != _currentUser.UserId.Value)
            return BaseResult<object>.Fail("Sadece kendi yanýtýný düzenleyebilirsin.");

        subComment.Body = request.Body!.Trim();
        _repository.Update(subComment);

        var result = await _unitOfWork.SaveChangesAsync();
        return result
            ? BaseResult<object>.Success()
            : BaseResult<object>.Fail("Yanýt güncellenirken bir sorun oluþtu.");
    }
}