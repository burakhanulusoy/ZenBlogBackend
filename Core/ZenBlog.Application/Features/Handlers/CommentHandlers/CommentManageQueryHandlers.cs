using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Queries.CommentQueries;
using ZenBlog.Application.Features.Results.CommentResults;

namespace ZenBlog.Application.Features.Handlers.CommentHandlers;

public class GetMyCommentsQueryHandler(ICommentReadService _commentReadService,
                                       ICurrentUserService _currentUser)
    : IRequestHandler<GetMyCommentsQuery, BaseResult<List<CommentManageResult>>>
{
    public async Task<BaseResult<List<CommentManageResult>>> Handle(GetMyCommentsQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return BaseResult<List<CommentManageResult>>.Fail("Kullanýcý bilgisi alýnamadý.");

        var result = await _commentReadService.GetByAuthorAsync(_currentUser.UserId.Value, cancellationToken);
        return BaseResult<List<CommentManageResult>>.Success(result);
    }
}

public class GetCommentsOnMyBlogsQueryHandler(ICommentReadService _commentReadService,
                                              ICurrentUserService _currentUser)
    : IRequestHandler<GetCommentsOnMyBlogsQuery, BaseResult<List<CommentManageResult>>>
{
    public async Task<BaseResult<List<CommentManageResult>>> Handle(GetCommentsOnMyBlogsQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return BaseResult<List<CommentManageResult>>.Fail("Kullanýcý bilgisi alýnamadý.");

        var result = await _commentReadService.GetOnBlogsOfAsync(_currentUser.UserId.Value, cancellationToken);
        return BaseResult<List<CommentManageResult>>.Success(result);
    }
}

public class GetAllCommentsQueryHandler(ICommentReadService _commentReadService)
    : IRequestHandler<GetAllCommentsQuery, BaseResult<List<CommentManageResult>>>
{
    public async Task<BaseResult<List<CommentManageResult>>> Handle(GetAllCommentsQuery request, CancellationToken cancellationToken)
    {
        var take = Math.Clamp(request.Take, 1, 1000);
        var result = await _commentReadService.GetAllAsync(take, cancellationToken);
        return BaseResult<List<CommentManageResult>>.Success(result);
    }
}