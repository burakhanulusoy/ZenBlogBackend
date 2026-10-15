using AutoMapper;
using ZenBlog.Application.Features.Commands.ContactMessageCommands;
using ZenBlog.Application.Features.Results.ContactMessageResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Mappings.ContactMessageMappings;

public class ContactMessageMapping : Profile
{
    public ContactMessageMapping()
    {
        CreateMap<CreateContactMessageCommand, ContactMessage>();

        CreateMap<ContactMessage, GetContactMessagesQueryResult>();
        CreateMap<ContactMessage, GetContactMessageByIdQueryResult>();
        CreateMap<ContactMessage, GetContactMessagesByStatusQueryResult>();
    }
}