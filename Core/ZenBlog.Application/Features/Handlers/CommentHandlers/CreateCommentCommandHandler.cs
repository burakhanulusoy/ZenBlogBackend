using AutoMapper;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.CommentCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.CommentHandlers;

public class CreateCommentCommandHandler(IRepository<Comment> _repository,
                                         IRepository<Blog> _blogRepository,
                                         IUnitOfWork _unitOfWork,
                                         IMapper _mapper,
                                         ICurrentUserService _currentUser)
    : IRequestHandler<CreateCommentCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return BaseResult<object>.Fail("Kullanýcý bilgisi alýnamadý. Lütfen tekrar giriþ yapýn.");

        // Blog gerçekten var mý? (Soft delete sayesinde silinmiþ bir bloga da yorum yapýlamaz.)
        var blog = await _blogRepository.GetByIdAsync(request.BlogId!.Value);

        if (blog is null)
            return BaseResult<object>.Fail("Yorum yapýlmak istenen blog bulunamadý.");

        var comment = _mapper.Map<Comment>(request);

        // Yorumun sahibi token'dan
        comment.AppUserId = _currentUser.UserId.Value;

        await _repository.CreateAsync(comment);
        var result = await _unitOfWork.SaveChangesAsync();

        return result
            ? BaseResult<object>.Success()
            : BaseResult<object>.Fail("Yorum eklenirken bir sorun oluþtu.");
    }
}