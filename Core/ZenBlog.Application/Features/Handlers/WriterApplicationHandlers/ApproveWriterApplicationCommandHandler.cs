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

public class ApproveWriterApplicationCommandHandler(UserManager<AppUser> _userManager,
                                                    IRepository<WriterApplication> _applicationRepository,
                                                    IRepository<RoleChangeLog> _roleLogRepository,
                                                    ICurrentUserService _currentUser,
                                                    IAccountEmailService _accountEmailService)
    : IRequestHandler<ApproveWriterApplicationCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(ApproveWriterApplicationCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return BaseResult<object>.Fail("Kullanýcý bilgisi alýnamadý. Lütfen tekrar giriþ yapýn.");

        var adminId = _currentUser.UserId.Value;

        var application = await _applicationRepository.GetByIdAsync(request.Id);
        if (application is null)
            return BaseResult<object>.Fail("Baþvuru bulunamadý.");

        if (application.Status is WriterApplicationStatus.Approved or WriterApplicationStatus.Rejected)
            return BaseResult<object>.Fail("Bu baþvuru zaten sonuçlandýrýlmýþ.");

        var user = await _userManager.FindByIdAsync(application.UserId.ToString());
        if (user is null)
            return BaseResult<object>.Fail("Baþvuran kullanýcý bulunamadý.");

        if (user.AccountStatus != AccountStatus.Active)
            return BaseResult<object>.Fail("Kullanýcýnýn hesabý aktif deðil; baþvuru onaylanamaz.");

        var now = DateTime.Now;

        // 1) Baþvuruyu sonuçlandýr
        application.Status = WriterApplicationStatus.Approved;
        application.DecidedAt = now;
        application.DecidedByAdminId = adminId;
        application.ReviewedAt ??= now;   // ??=: boþsa doldur
        _applicationRepository.Update(application);

        // 2) Writer rolünü ver + rol geçmiþine yaz
        if (!await _userManager.IsInRoleAsync(user, Roles.Writer))
        {
            var addResult = await _userManager.AddToRoleAsync(user, Roles.Writer);
            if (!addResult.Succeeded)
                return BaseResult<object>.Fail(addResult.Errors);

            await _roleLogRepository.CreateAsync(new RoleChangeLog
            {
                UserId = user.Id,
                ChangedByAdminId = adminId,
                RoleName = Roles.Writer,
                IsAdded = true,
                ChangedAt = now
            });
        }

        // 3) Damgayý yenile ? kiþinin oturumu düþer, yeniden girince yazar paneline gider.
        //    UpdateSecurityStampAsync içinde SaveChanges var ? baþvuru ve rol geçmiþi de birlikte yazýlýr.
        var stampResult = await _userManager.UpdateSecurityStampAsync(user);
        if (!stampResult.Succeeded)
            return BaseResult<object>.Fail(stampResult.Errors);

        // 4) Tebrik maili
        try
        {
            await _accountEmailService.SendWriterApplicationApprovedAsync(user);
        }
        catch
        {
            // mail sunucusu hatasý iþlemi bozmasýn
        }

        return BaseResult<object>.Success("Baþvuru onaylandý, kullanýcý artýk yazar.");
    }
}