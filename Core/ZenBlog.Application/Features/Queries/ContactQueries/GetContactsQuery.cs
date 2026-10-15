using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Results.ContactResults;

namespace ZenBlog.Application.Features.Queries.ContactQueries;

public record GetContactsQuery:IRequest<BaseResult<List<GetContactsQueryResult>>>;
