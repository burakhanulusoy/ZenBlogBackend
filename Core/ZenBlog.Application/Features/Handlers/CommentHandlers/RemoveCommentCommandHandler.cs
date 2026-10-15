using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.CommentCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.CommentHandlers;

public class RemoveCommentCommandHandler(IRepository<Comment> _repository,
                                         IUnitOfWork _unitOfWork) : IRequestHandler<RemoveCommentCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(RemoveCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await _repository.GetByIdAsync(request.Id);

        if (comment is null)
            return BaseResult<object>.Fail("Bu Id'ye sahip yorum bulunamadý.");

        _repository.Delete(comment);

        var result = await _unitOfWork.SaveChangesAsync();

        return result ? BaseResult<object>.Success() : BaseResult<object>.Fail("Yorum silinirken bir hata oluþtu.");




    }
}
