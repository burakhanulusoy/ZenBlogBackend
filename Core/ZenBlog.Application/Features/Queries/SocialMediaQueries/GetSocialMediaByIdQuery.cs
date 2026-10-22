using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Results.SocialMediaResults;

namespace ZenBlog.Application.Features.Queries.SocialMediaQueries;

public record GetSocialMediaByIdQuery(Guid Id):IRequest<BaseResult<GetSocialMediaByIdQueryResult>>;
