using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.AboutCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.AboutHandlers;

public class RemoveAboutCommandHandler(IRepository<About> _repository,
                                       IUnitOfWork _unitOfWork)
    : IRequestHandler<RemoveAboutCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(RemoveAboutCommand request, CancellationToken cancellationToken)
    {
        var about = await _repository.GetByIdAsync(request.Id);

        if (about is null)
            return BaseResult<object>.Fail("Bu Id'ye sahip hakkýmýzda bilgisi bulunamadý.");

        // Interceptor soft delete'e çevirir; kayýt geri getirilebileceði için resimleri silmiyoruz
        _repository.Delete(about);
        var result = await _unitOfWork.SaveChangesAsync();

        return result
            ? BaseResult<object>.Success()
            : BaseResult<object>.Fail("Hakkýmýzda bilgisi silinirken bir sorun oluþtu.");
    }
}