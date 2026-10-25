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

        // DEÐÝÞTÝ: GetCommentsByBlogIdQueryResult ve CommentSubCommentResult eþlemeleri kaldýrýldý
        // (blog yorumlarý artýk CommentReadService'te projeksiyonla oluþturuluyor).
        // Kalsaydý ForCtorParam artýk olmayan bir alaný aradýðý için uygulama açýlýrken hata verirdi.

        CreateMap<Comment, GetCommentsByUserIdQueryResult>()
            .ForCtorParam(nameof(GetCommentsByUserIdQueryResult.BlogMainTitle),
                          opt => opt.MapFrom(src => src.Blog.MainTitle));

        CreateMap<Comment, GetCommentByIdQueryResult>();
    }
}