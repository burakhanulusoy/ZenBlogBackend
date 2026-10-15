using AutoMapper;
using ZenBlog.Application.Features.Commands.SubCommentCommands;
using ZenBlog.Application.Features.Results.SubCommentResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Mappings.SubCommentMappings;

public class SubCommentMapping : Profile
{
    public SubCommentMapping()
    {
        CreateMap<CreateSubCommentCommand, SubComment>();

        CreateMap<SubComment, GetSubCommentsByCommentIdQueryResult>();
        CreateMap<SubComment, GetSubCommentByIdQueryResult>();

        // CommentBody entity'de doðrudan yok; cevap verilen yorumun metninden alýyoruz.
        // Yorum sonradan silindiyse query filter yüzünden null gelir, o yüzden kontrol ediyoruz.
        CreateMap<SubComment, GetSubCommentsByUserIdQueryResult>()
            .ForCtorParam(nameof(GetSubCommentsByUserIdQueryResult.CommentBody),
                          opt => opt.MapFrom(src => src.Comment != null ? src.Comment.Body : "Silinmiþ yorum"));
    }
}