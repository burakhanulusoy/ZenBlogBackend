using AutoMapper;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Queries.ContactMessageQueries;
using ZenBlog.Application.Features.Results.ContactMessageResults;
using ZenBlog.Domain.Entities;
using ZenBlog.Domain.Enums;

namespace ZenBlog.Application.Features.Handlers.ContactMessageHandlers
{
    public class GetContactMessageByIdQueryHandler(IRepository<ContactMessage> _repository,
                                                   IUnitOfWork _unitOfWork,   // YENÝ
                                                   IMapper _mapper)
        : IRequestHandler<GetContactMessageByIdQuery, BaseResult<GetContactMessageByIdQueryResult>>
    {
        public async Task<BaseResult<GetContactMessageByIdQueryResult>> Handle(GetContactMessageByIdQuery request, CancellationToken cancellationToken)
        {
            var message = await _repository.GetByIdAsync(request.Id);
            if (message is null)
                return BaseResult<GetContactMessageByIdQueryResult>.Fail("Bu Id'ye sahip mesaj bulunamadý.");

            // YENÝ: Admin mesajý açtý ? "Bekliyor" ise otomatik "Okundu"
            // (e-posta programlarýndaki gibi; GetByIdAsync takip edilen nesne döndürdüðü için doðrudan güncellenebiliyor)
            if (message.ContactMessageStatus == ContactMessageStatus.Pending)
            {
                message.ContactMessageStatus = ContactMessageStatus.Read;
                _repository.Update(message);
                await _unitOfWork.SaveChangesAsync();
            }

            return BaseResult<GetContactMessageByIdQueryResult>.Success(_mapper.Map<GetContactMessageByIdQueryResult>(message));
        }
    }
}