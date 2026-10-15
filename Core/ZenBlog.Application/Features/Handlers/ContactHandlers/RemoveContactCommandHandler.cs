using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.ContactCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.ContactHandlers;

public class RemoveContactCommandHandler(IRepository<Contact> _repository,
                                  IUnitOfWork _unitOfWork) : IRequestHandler<RemoveContactCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(RemoveContactCommand request, CancellationToken cancellationToken)
    {

        var contact = await _repository.GetByIdAsync(request.Id);

        if (contact is null)
            return BaseResult<object>.Fail("Bu Id'ye sahip iletiþim bulunamadý");


        _repository.Delete(contact);

        var result = await _unitOfWork.SaveChangesAsync();

        return result ? BaseResult<object>.Success() : BaseResult<object>.Fail("Ýletiþim silinirken bir sorun oluþtu");




    }
}
