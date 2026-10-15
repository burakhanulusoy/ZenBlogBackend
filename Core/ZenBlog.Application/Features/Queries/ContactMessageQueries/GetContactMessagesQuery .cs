using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Results.ContactMessageResults;

namespace ZenBlog.Application.Features.Queries.ContactMessageQueries;

public record GetContactMessagesQuery : IRequest<BaseResult<List<GetContactMessagesQueryResult>>>;