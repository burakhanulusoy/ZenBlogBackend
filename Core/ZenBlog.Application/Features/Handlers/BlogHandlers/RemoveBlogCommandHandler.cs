using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.BlogCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.BlogHandlers;

public class RemoveBlogCommandHandler(IRepository<Blog> _repository,
                                      IUnitOfWork _unitOfWork,
                                      ICurrentUserService _currentUser)
    : IRequestHandler<RemoveBlogCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(RemoveBlogCommand request, CancellationToken cancellationToken)
    {
        var blog = await _repository.GetByIdAsync(request.Id);

        if (blog is null)
            return BaseResult<object>.Fail("Bu Id'ye sahip blog bulunamadý.");

        // SAHÝPLÝK: sahibi ya da Admin silebilir
        if (_currentUser.UserId is null)
            return BaseResult<object>.Fail("Kullanýcý bilgisi alýnamadý. Lütfen tekrar giriþ yapýn.");

        var isOwner = blog.AppUserId == _currentUser.UserId.Value;
        if (!isOwner && !_currentUser.IsInRole(Roles.Admin))
            return BaseResult<object>.Fail("Bu blogu silme yetkin yok.");

        // Interceptor bunu soft delete'e çevirir (IsDeleted = true).
        // Blog geri getirilebileceði için görselleri SÝLMÝYORUZ.
        _repository.Delete(blog);

        var result = await _unitOfWork.SaveChangesAsync();

        return result
            ? BaseResult<object>.Success()
            : BaseResult<object>.Fail("Blog silinirken bir sorun oluþtu.");
    }
}