using AutoMapper;
using ZenBlog.Application.Features.Commands.UserCommands;
using ZenBlog.Application.Features.Results.UserResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Mappings.UserMappings
{
    public class UserMapping :Profile
    {
        public UserMapping()
        {
            CreateMap<CreateUserCommand, AppUser>().ReverseMap();
            CreateMap<AppUser,GetUsersQueryResult>().ReverseMap();


        }
    }
}
