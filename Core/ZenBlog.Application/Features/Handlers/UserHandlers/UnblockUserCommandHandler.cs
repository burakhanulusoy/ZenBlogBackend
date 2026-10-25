using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.UserCommands;
using ZenBlog.Application.Services.MailServices;
using ZenBlog.Domain.Entities;
using ZenBlog.Domain.Enums;

namespace ZenBlog.Application.Features.Handlers.UserHandlers;

public class UnblockUserCommandHandler(UserManager<AppUser> _userManager,
                                       IRepository<UserBlock> _blockRepository,
                                       IAccountContentService _accountContentService,
                                       IAccountEmailService _accountEmailService,
                                       ICurrentUserService _currentUser,
                                       ILogger<UnblockUserCommandHandler> _logger)   // DEÐÝÞTÝ: logu yazan sýnýf
    : IRequestHandler<UnblockUserCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(UnblockUserCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return BaseResult<object>.Fail("Kullanýcý bilgisi alýnamadý. Lütfen tekrar giriþ yapýn.");

        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
            return BaseResult<object>.Fail("Kullanýcý bulunamadý.");

        if (user.AccountStatus != AccountStatus.Blocked)
            return BaseResult<object>.Fail("Bu kullanýcý bloke deðil.");

        // 1) Açýk (kaldýrýlmamýþ) en son bloke kaydý
        var activeBlock = (await _blockRepository.GetAllFilterAsync(b => b.UserId == user.Id && b.UnblockedAt == null))
            .OrderByDescending(b => b.BlockedAt)
            .FirstOrDefault();

        // Kayýt kapanýr ama SÝLÝNMEZ -> geçmiþte "kim, ne zaman, neden kaldýrdý" görünür
        if (activeBlock is not null)
        {
            activeBlock.UnblockedAt = DateTime.Now;
            activeBlock.UnblockedByAdminId = _currentUser.UserId.Value;
            activeBlock.UnblockReason = request.Reason!.Trim();
            _blockRepository.Update(activeBlock);   // DbContext'e "deðiþti" diye iþaretler
        }

        // 2) Kullanýcýyý aktif yap -> UpdateAsync'in SaveChanges'i bloke kaydýný da birlikte yazar
        user.AccountStatus = AccountStatus.Active;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BaseResult<object>.Fail(result.Errors);

        // 3) Ýçerikleri geri aç (durum Active yapýldýktan SONRA; sýra önemli)
        await _accountContentService.RestoreContentAsync(user.Id, cancellationToken);

        // LOG: DEÐÝÞTÝ: alan adý {TargetUserId}
        _logger.LogInformation("{EventType}: Bloke kaldýrýldý. Hedef: {TargetUserId}",
                               LogEvents.UserUnblocked, user.Id);

        // 4) Bilgilendirme maili
        //    Gönderilemezse hata AccountEmailService içinde EmailFailed olarak loglanýr.
        try
        {
            await _accountEmailService.SendUnblockedAsync(user);
        }
        catch
        {
            // mail sunucusu hatasý iþlemi bozmasýn
        }

        return BaseResult<object>.Success();
    }
}