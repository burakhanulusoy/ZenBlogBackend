using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.ContactMessageCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.ContactMessageHandlers
{
    public class RemoveContactMessageCommandHandler(IRepository<ContactMessage> _repository,
                                                    IUnitOfWork _unitOfWork) : IRequestHandler<RemoveContactMessageCommand, BaseResult<object>>
    {
        public async Task<BaseResult<object>> Handle(RemoveContactMessageCommand request, CancellationToken cancellationToken)
        {
            var contactMessage = await _repository.GetByIdAsync(request.Id);

            if (contactMessage == null)
                return BaseResult<object>.Fail("Bu ýd sahip mesaj bulunmamaktadýr");


            _repository.Delete(contactMessage);

            var result = await _unitOfWork.SaveChangesAsync();

            return result ? BaseResult<object>.Success() : BaseResult<object>.Fail("Mesaj silinirken bir hata oluþtu");


        }
    }
}
