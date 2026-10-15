using AutoMapper;
using ZenBlog.Application.Features.Commands.ContactCommands;
using ZenBlog.Application.Features.Results.ContactResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Mappings.ContactMappings
{
    public class ContactMapping : Profile
    {
        public ContactMapping()
        {
            CreateMap<Contact, GetContactByIdQueryResult>();
            CreateMap<Contact, GetContactsQueryResult>();
            CreateMap<CreateContactCommand, Contact>();

            // Güncellemede Id deðiþmemeli
            CreateMap<UpdateContactCommand, Contact>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
