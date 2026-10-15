using AutoMapper;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Results.ContactMessageResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.ContactMessageHandlers
{
    public class GetContactMessagesByStatusQueryHandler(IRepository<ContactMessage> _repository,
                                                      IMapper _mapper)
      : IRequestHandler<GetContactMessagesByStatusQuery, BaseResult<List<GetContactMessagesByStatusQueryResult>>>
    {
        public async Task<BaseResult<List<GetContactMessagesByStatusQueryResult>>> Handle(GetContactMessagesByStatusQuery request, CancellationToken cancellationToken)
        {
            var messages = await _repository.GetAllFilterAsync(x => x.ContactMessageStatus == request.Status);

            var result = _mapper.Map<List<GetContactMessagesByStatusQueryResult>>(messages.OrderByDescending(x => x.CreatedAt));

            return BaseResult<List<GetContactMessagesByStatusQueryResult>>.Success(result);
        }
    }
}
