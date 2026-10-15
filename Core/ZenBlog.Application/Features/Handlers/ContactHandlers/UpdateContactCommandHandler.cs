using AutoMapper;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.ContactCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.ContactHandlers;

public class UpdateContactCommandHandler(IRepository<Contact> _repository,
                                  IUnitOfWork _unitOfWork,
                                  IMapper _mapper) : IRequestHandler<UpdateContactCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(UpdateContactCommand request, CancellationToken cancellationToken)
    {
        var contact = await _repository.GetByIdAsync(request.Id);

        if (contact is null)
            return BaseResult<object>.Fail("Bu Id'ye sahip iletiþim bilgisi bulunamadý.");

        // request'teki yeni deðerleri mevcut contact'ýn üzerine yaz
        _mapper.Map(request, contact);

        _repository.Update(contact);
        var result = await _unitOfWork.SaveChangesAsync();

        return result
            ? BaseResult<object>.Success()
            : BaseResult<object>.Fail("Ýletiþim bilgisi güncellenirken bir sorun oluþtu.");
    }
}
