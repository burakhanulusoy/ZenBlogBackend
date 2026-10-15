using AutoMapper;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Queries.ContactMessageQueries;
using ZenBlog.Application.Features.Results.ContactMessageResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.ContactMessageHandlers;

public class GetContactMessagesQueryHandler(IRepository<ContactMessage> _repository,
                                            IMapper _mapper)
    : IRequestHandler<GetContactMessagesQuery, BaseResult<List<GetContactMessagesQueryResult>>>
{
    public async Task<BaseResult<List<GetContactMessagesQueryResult>>> Handle(GetContactMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await _repository.GetAllAsync();

        // Gelen kutusu gibi: en yeni mesaj en üstte
        var result = _mapper.Map<List<GetContactMessagesQueryResult>>(messages.OrderByDescending(x => x.CreatedAt));

        return BaseResult<List<GetContactMessagesQueryResult>>.Success(result);
    }
}