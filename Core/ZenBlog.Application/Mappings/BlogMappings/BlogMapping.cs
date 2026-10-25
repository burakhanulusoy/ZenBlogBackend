using AutoMapper;
using ZenBlog.Application.Features.Commands.BlogCommands;
using ZenBlog.Application.Features.Results.BlogResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Mappings.BlogMappings
{
    public class BlogMapping : Profile
    {
        public BlogMapping()
        {
            // Okuma: entity ? result
            CreateMap<Blog, GetBlogsQueryResult>();
            CreateMap<Blog, GetBlogByIdQueryResult>();
            CreateMap<Blog, GetBlogsByCategoryIdQueryResult>();
            CreateMap<Blog, GetBlogsByUserIdQueryResult>();

            // Oluþturma: command ? entity
            // Resimler IFormFile, entity'de string; onlarý handler yükleyip dolduruyor.
            CreateMap<CreateBlogCommand, Blog>()
                .ForMember(dest => dest.CoverImage, opt => opt.Ignore())
                .ForMember(dest => dest.BlogImage1, opt => opt.Ignore())
                .ForMember(dest => dest.BlogImage2, opt => opt.Ignore());

            // Güncelleme: command ? mevcut entity
            // Id deðiþmemeli; resimler seçilmezse eski adresler korunmalý.
            CreateMap<UpdateBlogCommand, Blog>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CoverImage, opt => opt.Ignore())
                .ForMember(dest => dest.BlogImage1, opt => opt.Ignore())
                .ForMember(dest => dest.BlogImage2, opt => opt.Ignore())
                .ForMember(dest => dest.AppUserId, opt => opt.Ignore());
        }
    }
}