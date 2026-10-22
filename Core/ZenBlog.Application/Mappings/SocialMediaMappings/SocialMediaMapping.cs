using AutoMapper;
using ZenBlog.Application.Features.Commands.SocialMediaCommands;
using ZenBlog.Application.Features.Results.SocialMediaResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Mappings.SocialMediaMappings
{
    public class SocialMediaMapping : Profile
    {
        public SocialMediaMapping()
        {
            // DEÐÝÞTÝ: Icon yok sayýlýyor.
            // Komuttaki Icon bir DOSYA (IFormFile), entity'deki Icon bir ADRES (string).
            // Eþlenirse güncellemede ikon null'a döner; ikonu handler'da elle yönetiyoruz.
            // ReverseMap kaldýrýldý: entity ? komut yönüne hiç ihtiyaç yok.
            CreateMap<CreateSocialMediaCommand, SocialMedia>()
                .ForMember(dest => dest.Icon, opt => opt.Ignore());

            CreateMap<UpdateSocialMediaCommand, SocialMedia>()
                .ForMember(dest => dest.Icon, opt => opt.Ignore());

            CreateMap<SocialMedia, GetSocialMediaByIdQueryResult>();
            CreateMap<SocialMedia, GetSocialMediasQueryResult>();
        }
    }
}