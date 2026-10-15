using AutoMapper;
using ZenBlog.Application.Features.Commands.CommentCommands;
using ZenBlog.Application.Features.Results.CommentResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Mappings.CommentMappings;

public class CommentMapping : Profile
{
    public CommentMapping()
    {
        CreateMap<CreateCommentCommand, Comment>();

        // Alt yorumlar da eskiden yeniye sýralý gelsin
        CreateMap<Comment, GetCommentsByBlogIdQueryResult>()
            .ForCtorParam(nameof(GetCommentsByBlogIdQueryResult.SubComments),
                          opt => opt.MapFrom(src => src.SubComments.OrderBy(s => s.CreatedAt)));

        CreateMap<SubComment, CommentSubCommentResult>();

        // BlogMainTitle entity'de doðrudan yok; yorumun baðlý olduðu blogun baþlýðýndan alýyoruz
        CreateMap<Comment, GetCommentsByUserIdQueryResult>()
            .ForCtorParam(nameof(GetCommentsByUserIdQueryResult.BlogMainTitle),
                          opt => opt.MapFrom(src => src.Blog.MainTitle));

    }
}