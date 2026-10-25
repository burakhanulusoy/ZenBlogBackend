using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.SubCommentCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.SubCommentHandlers;

public class RemoveSubCommentCommandHandler(IRepository<SubComment> _repository,
                                            IUnitOfWork _unitOfWork,
                                            ICurrentUserService _currentUser)
    : IRequestHandler<RemoveSubCommentCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(RemoveSubCommentCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return BaseResult<object>.Fail("Kullanıcı bilgisi alınamadı. Lütfen tekrar giriş yapın.");

        var userId = _currentUser.UserId.Value;

        var subComment = await _repository.GetByIdAsync(request.Id);
        if (subComment is null)
            return BaseResult<object>.Fail("Bu Id'ye sahip yanıt bulunamadı.");

        // Yanıtın sahibi, blogun yazarı ya da Admin silebilir
        // (subComment.Comment.Blog: takip edilen nesne → lazy loading çalışır)
        var canDelete = subComment.AppUserId == userId
                     || subComment.Comment.Blog.AppUserId == userId
                     || _currentUser.IsInRole(Roles.Admin);

        if (!canDelete)
            return BaseResult<object>.Fail("Bu yanıtı silme yetkin yok.");

        _repository.Delete(subComment);

        var result = await _unitOfWork.SaveChangesAsync();
        return result
            ? BaseResult<object>.Success()
            : BaseResult<object>.Fail("Yanıt silinirken bir hata oluştu.");
    }
}