using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Results.ContactMessageResults;

public record GetContactMessageByIdQuery(Guid Id) : IRequest<BaseResult<GetContactMessageByIdQueryResult>>;