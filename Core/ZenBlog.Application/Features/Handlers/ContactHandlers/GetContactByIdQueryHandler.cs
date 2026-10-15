using AutoMapper;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Queries.ContactQueries;
using ZenBlog.Application.Features.Results.ContactResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.ContactHandlers
{
    public class GetContactByIdQueryHandler(IRepository<Contact> _repository,
                                            IMapper _mapper) : IRequestHandler<GetContactByIdQuery, BaseResult<GetContactByIdQueryResult>>
    {
        public async Task<BaseResult<GetContactByIdQueryResult>> Handle(GetContactByIdQuery request, CancellationToken cancellationToken)
        {
            
            var contact = await _repository.GetByIdAsync(request.Id);

            if (contact is null)
                return BaseResult<GetContactByIdQueryResult>.Fail("Bu Id'ye sahip iletiþim bilgisi bulunamadý.");


            var mappedContact = _mapper.Map<GetContactByIdQueryResult>(contact);

            return BaseResult<GetContactByIdQueryResult>.Success(mappedContact);





        }
    }
}
