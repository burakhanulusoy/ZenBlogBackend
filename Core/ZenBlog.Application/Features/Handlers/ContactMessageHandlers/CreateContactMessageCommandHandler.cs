using AutoMapper;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.ContactMessageCommands;
using ZenBlog.Domain.Entities;
using ZenBlog.Domain.Enums;

namespace ZenBlog.Application.Features.Handlers.ContactMessageHandlers
{
    public class CreateContactMessageCommandHandler(IRepository<ContactMessage> _repository,
                                                    IUnitOfWork _unitOfWork,
                                                    IMapper _mapper) : IRequestHandler<CreateContactMessageCommand, BaseResult<object>>
    { 
        public async Task<BaseResult<object>> Handle(CreateContactMessageCommand request, CancellationToken cancellationToken)
        {
            
            var mappedContactMessage = _mapper.Map<ContactMessage>(request);

            mappedContactMessage.ContactMessageStatus = ContactMessageStatus.Pending;

            await _repository.CreateAsync(mappedContactMessage);

            var result = await _unitOfWork.SaveChangesAsync();

            return result ? BaseResult<object>.Success() : BaseResult<object>.Fail("Mesaj oluþturulýken bir sorun oluþtu");




        }
    }
}
