using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.CommentCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.CommentHandlers;

public class RemoveCommentCommandHandler(IRepository<Comment> _repository,
                                         IRepository<SubComment> _subCommentRepository,   // YENİ
                                         IUnitOfWork _unitOfWork,
                                         ICurrentUserService _currentUser)                // YENİ
    : IRequestHandler<RemoveCommentCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(RemoveCommentCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return BaseResult<object>.Fail("Kullanıcı bilgisi alınamadı. Lütfen tekrar giriş yapın.");

        var userId = _currentUser.UserId.Value;

        var comment = await _repository.GetByIdAsync(request.Id);
        if (comment is null)
            return BaseResult<object>.Fail("Bu Id'ye sahip yorum bulunamadı.");

        // YENİ: Kim silebilir? Yorumun sahibi, blogun yazarı ya da Admin.
        // comment.Blog: GetByIdAsync takip edilen nesne döndürdüğü için lazy loading burada çalışır (tek ek sorgu).
        var canDelete = comment.AppUserId == userId
                     || comment.Blog.AppUserId == userId
                     || _currentUser.IsInRole(Roles.Admin);

        if (!canDelete)
            return BaseResult<object>.Fail("Bu yorumu silme yetkin yok.");

        // YENİ: Yanıtları da gizle (yoksa sahipsiz kalırlar)
        var replies = await _subCommentRepository.GetAllFilterAsync(s => s.CommentId == comment.Id);
        foreach (var reply in replies)
            _subCommentRepository.Delete(reply);

        _repository.Delete(comment);

        // Hepsi tek SaveChanges → ya birlikte silinir ya hiçbiri
        var result = await _unitOfWork.SaveChangesAsync();
        return result
            ? BaseResult<object>.Success()
            : BaseResult<object>.Fail("Yorum silinirken bir hata oluştu.");
    }
}