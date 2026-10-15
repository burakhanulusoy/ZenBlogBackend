using AutoMapper;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.ContactCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.ContactHandlers
{
    public class CreateContactCommandHandler(IRepository<Contact> _repository,
                                             IUnitOfWork unitOfWork,
                                             IMapper mapper) : IRequestHandler<CreateContactCommand, BaseResult<object>>
    {
        public async Task<BaseResult<object>> Handle(CreateContactCommand request, CancellationToken cancellationToken)
        {
            
            var mappedContact = mapper.Map<Contact>(request);

           await _repository.CreateAsync(mappedContact);

            var result = await unitOfWork.SaveChangesAsync();

            return result ? BaseResult<object>.Success() : BaseResult<object>.Fail("Ýletiþim bilgisi oluþturulurken bir sorun oluþtu");




        }
    }
}
