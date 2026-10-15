using AutoMapper;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Results.ContactMessageResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.ContactMessageHandlers
{
    public class GetContactMessageByIdQueryHandler(IRepository<ContactMessage> _repository,
                                                IMapper _mapper)
     : IRequestHandler<GetContactMessageByIdQuery, BaseResult<GetContactMessageByIdQueryResult>>
    {
        public async Task<BaseResult<GetContactMessageByIdQueryResult>> Handle(GetContactMessageByIdQuery request, CancellationToken cancellationToken)
        {
            var message = await _repository.GetByIdAsync(request.Id);

            if (message is null)
                return BaseResult<GetContactMessageByIdQueryResult>.Fail("Bu Id'ye sahip mesaj bulunamadý.");

            return BaseResult<GetContactMessageByIdQueryResult>.Success(_mapper.Map<GetContactMessageByIdQueryResult>(message));
        }
    }
}
