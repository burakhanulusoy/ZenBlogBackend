using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.CommentCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.CommentHandlers
{
    public class UpdateCommentCommandHandler(IRepository<Comment> _repository,
                                             IUnitOfWork _unitOfWork,
                                             ICurrentUserService _currentUser)   // YENÝ
        : IRequestHandler<UpdateCommentCommand, BaseResult<object>>
    {
        public async Task<BaseResult<object>> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is null)
                return BaseResult<object>.Fail("Kullanýcý bilgisi alýnamadý. Lütfen tekrar giriþ yapýn.");

            var comment = await _repository.GetByIdAsync(request.Id!.Value);
            if (comment is null)
                return BaseResult<object>.Fail("Bu Id'ye sahip yorum bulunamadý.");

            // YENÝ: Yorumu SADECE sahibi düzenleyebilir (baþkasýnýn sözünü deðiþtirmek olmaz; admin dahil)
            if (comment.AppUserId != _currentUser.UserId.Value)
                return BaseResult<object>.Fail("Sadece kendi yorumunu düzenleyebilirsin.");

            comment.Body = request.Body!.Trim();
            _repository.Update(comment);

            var result = await _unitOfWork.SaveChangesAsync();
            return result
                ? BaseResult<object>.Success()
                : BaseResult<object>.Fail("Yorum güncellenirken bir sorun oluþtu.");
        }
    }
}