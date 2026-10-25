using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Results.CommentResults;

namespace ZenBlog.Application.Features.Queries.CommentQueries;

// Benim yorum ve yanýtlarým (kim olduðum token'dan)
public record GetMyCommentsQuery : IRequest<BaseResult<List<CommentManageResult>>>;

// Bloglarýma baþkalarýnýn yazdýklarý
public record GetCommentsOnMyBlogsQuery : IRequest<BaseResult<List<CommentManageResult>>>;

// Bütün yorumlar (admin)
public record GetAllCommentsQuery(int Take = 500) : IRequest<BaseResult<List<CommentManageResult>>>;