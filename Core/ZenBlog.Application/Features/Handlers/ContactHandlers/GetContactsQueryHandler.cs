using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Queries.ContactQueries;
using ZenBlog.Application.Features.Results.ContactResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.ContactHandlers
{
    public class GetContactsQueryHandler(IRepository<Contact> _repository,
                                         IMapper _mapper) : IRequestHandler<GetContactsQuery, BaseResult<List<GetContactsQueryResult>>>
    {
        public async Task<BaseResult<List<GetContactsQueryResult>>> Handle(GetContactsQuery request, CancellationToken cancellationToken)
        {
            var contacts = await _repository.GetAllAsync();

            // En yeni eklenen en üstte
            var mappedContacts = _mapper.Map<List<GetContactsQueryResult>>(contacts.OrderByDescending(x => x.CreatedAt));

            return BaseResult<List<GetContactsQueryResult>>.Success(mappedContacts);
        }
    }
}
