using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.BlogCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.BlogHandlers;

public class RecordBlogViewCommandHandler(IRepository<Blog> _blogRepository,
                                          IBlogViewService _blogViewService,
                                          ICurrentUserService _currentUser)
    : IRequestHandler<RecordBlogViewCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(RecordBlogViewCommand request, CancellationToken cancellationToken)
    {
        var blog = await _blogRepository.GetByIdAsync(request.BlogId);
        if (blog is null || blog.IsDeleted)
            return BaseResult<object>.Fail("Blog bulunamadý.");

        // Yazarýn kendi blogunu okumasý sayýlmaz
        if (_currentUser.UserId == blog.AppUserId)
            return BaseResult<object>.Success();

        await _blogViewService.RecordAsync(blog.Id, request.VisitorKey, cancellationToken);
        return BaseResult<object>.Success();
    }
}