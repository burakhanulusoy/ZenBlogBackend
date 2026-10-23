using AutoMapper;
using ZenBlog.Application.Features.Commands.TeamCommands;
using ZenBlog.Application.Features.Results.TeamResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Mappings.TeamMappings;

public class TeamMapping : Profile
{
    public TeamMapping()
    {
        // ENTITY ? SONUÇ
        CreateMap<Team, GetTeamsQueryResult>();
        CreateMap<Team, GetTeamByIdQueryResult>();
        CreateMap<TeamSocialMedia, GetTeamSocialMediaResult>();

        // KOMUT ? ENTITY
        // Image: komutta DOSYA (IFormFile), entity'de ADRES (string) ? eþlenmesin, handler yönetiyor
        CreateMap<CreateTeamCommand, Team>()
            .ForMember(dest => dest.Image, opt => opt.Ignore());

        // Id: var olan entity'nin Id'si deðiþmesin
        CreateMap<UpdateTeamCommand, Team>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Image, opt => opt.Ignore());

        // Icon: platformdan otomatik belirleniyor (handler'da)
        CreateMap<CreateTeamSocialMediaCommand, TeamSocialMedia>()
            .ForMember(dest => dest.Icon, opt => opt.Ignore());

        // Güncellemede Id ve TeamId deðiþmesin (hesap baþka üyeye taþýnamaz)
        CreateMap<UpdateTeamSocialMediaCommand, TeamSocialMedia>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TeamId, opt => opt.Ignore())
            .ForMember(dest => dest.Icon, opt => opt.Ignore());
    }
}