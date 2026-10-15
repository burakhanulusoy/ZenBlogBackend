using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Results.SubCommentResults;

public record GetSubCommentByIdQuery(Guid Id)
    : IRequest<BaseResult<GetSubCommentByIdQueryResult>>;