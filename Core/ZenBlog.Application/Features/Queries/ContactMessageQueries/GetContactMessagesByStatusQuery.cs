using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Results.ContactMessageResults;
using ZenBlog.Domain.Enums;

public record GetContactMessagesByStatusQuery(ContactMessageStatus Status)
    : IRequest<BaseResult<List<GetContactMessagesByStatusQueryResult>>>;