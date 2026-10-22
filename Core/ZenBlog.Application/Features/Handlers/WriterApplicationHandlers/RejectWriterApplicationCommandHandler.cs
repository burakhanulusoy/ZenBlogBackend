using MediatR;
using Microsoft.AspNetCore.Identity;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.WriterApplicationCommands;
using ZenBlog.Application.Services.MailServices;
using ZenBlog.Domain.Entities;
using ZenBlog.Domain.Enums;

namespace ZenBlog.Application.Features.Handlers.WriterApplicationHandlers;

public class RejectWriterApplicationCommandHandler(UserManager<AppUser> _userManager,
                                                   IRepository<WriterApplication> _applicationRepository,
                                                   IUnitOfWork _unitOfWork,
                                                   ICurrentUserService _currentUser,
                                                   IAccountEmailService _accountEmailService)
    : IRequestHandler<RejectWriterApplicationCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(RejectWriterApplicationCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return BaseResult<object>.Fail("Kullanýcý bilgisi alýnamadý. Lütfen tekrar giriþ yapýn.");

        var application = await _applicationRepository.GetByIdAsync(request.Id);
        if (application is null)
            return BaseResult<object>.Fail("Baþvuru bulunamadý.");

        if (application.Status is WriterApplicationStatus.Approved or WriterApplicationStatus.Rejected)
            return BaseResult<object>.Fail("Bu baþvuru zaten sonuçlandýrýlmýþ.");

        var now = DateTime.Now;

        application.Status = WriterApplicationStatus.Rejected;
        application.RejectReason = request.Reason!.Trim();
        application.DecidedAt = now;
        application.DecidedByAdminId = _currentUser.UserId.Value;
        application.ReviewedAt ??= now;

        _applicationRepository.Update(application);
        await _unitOfWork.SaveChangesAsync();

        // Bilgilendirme maili: gerekçe + ne zaman tekrar baþvurabileceði
        var user = await _userManager.FindByIdAsync(application.UserId.ToString());
        if (user is not null)
        {
            try
            {
                await _accountEmailService.SendWriterApplicationRejectedAsync(
                    user, application.RejectReason, now.AddDays(WriterApplicationRules.ReapplyDays));
            }
            catch
            {
                // mail sunucusu hatasý iþlemi bozmasýn
            }
        }

        return BaseResult<object>.Success("Baþvuru reddedildi.");
    }
}