using AutoMapper;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.ContactMessageCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.ContactMessageHandlers
{
    public class UpdateContactMessageCommandHandler(IRepository<ContactMessage> _repository,
                                                    IMapper _mapper,
                                                    IUnitOfWork _unitOfWork) : IRequestHandler<UpdateContactMessageCommand, BaseResult<object>>
    {
        public async Task<BaseResult<object>> Handle(UpdateContactMessageCommand request, CancellationToken cancellationToken)
        {
            
            var contactMessage = await _repository.GetByIdAsync(request.Id);

            if (contactMessage == null)
                return BaseResult<object>.Fail("Bu ýd sahip mesaj bulunmamaktadýr");

            contactMessage.ContactMessageStatus = request.Status!.Value;

          //  _mapper.Map(request, contactMessage);

            _repository.Update(contactMessage);

            var result = await _unitOfWork.SaveChangesAsync();

            return result ? BaseResult<object>.Success() : BaseResult<object>.Fail("Mesaj güncellenirken bir hata oluþtu");



        }
    }
}
