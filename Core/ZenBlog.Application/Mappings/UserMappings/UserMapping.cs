using AutoMapper;
using ZenBlog.Application.Features.Commands.UserCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Mappings.UserMappings
{
    public class UserMapping :Profile
    {
        public UserMapping()
        {
            CreateMap<CreateUserCommand, AppUser>();
        }
    }
}
