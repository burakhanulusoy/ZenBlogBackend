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

public class BlockUserCommandHandler(UserManager<AppUser> _userManager,
                                     IRepository<UserBlock> _blockRepository,
                                     IAccountContentService _accountContentService,
                                     IAccountEmailService _accountEmailService,
                                     IFileService _fileService,
                                     ICurrentUserService _currentUser,
                                     ILogger<BlockUserCommandHandler> _logger)   // DEÐÝÞTÝ: logu yazan sýnýf
    : IRequestHandler<BlockUserCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(BlockUserCommand request, CancellationToken cancellationToken)
    {
        // 1) Bloke eden admin -> token'dan
        if (_currentUser.UserId is null)
            return BaseResult<object>.Fail("Kullanýcý bilgisi alýnamadý. Lütfen tekrar giriþ yapýn.");

        var adminId = _currentUser.UserId.Value;

        if (request.UserId == adminId)
            return BaseResult<object>.Fail("Kendini bloke edemezsin.");

        // 2) Bloke edilecek kullanýcý
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
            return BaseResult<object>.Fail("Kullanýcý bulunamadý.");

        if (await _userManager.IsInRoleAsync(user, Roles.Admin))
            return BaseResult<object>.Fail("Yönetici hesaplarý bloke edilemez. Önce kullanýcýnýn Admin rolünü kaldýr.");

        if (user.AccountStatus == AccountStatus.Blocked)
            return BaseResult<object>.Fail("Bu kullanýcý zaten bloke edilmiþ.");

        // 3) Kanýt dosyasýný yükle
        string evidenceUrl;
        await using (var stream = request.Evidence!.OpenReadStream())
        {
            evidenceUrl = await _fileService.UploadAsync(stream, request.Evidence.FileName, ImageFolders.Blocks, cancellationToken);
        }

        var now = DateTime.Now;

        // 4) Bloke kaydý + kullanýcýnýn yeni durumu -> TEK SaveChanges ile
        //    CreateAsync kaydý sadece DbContext'e EKLER (henüz veritabanýna yazmaz).
        //    UserManager.UpdateAsync da ayný DbContext'i kullanýr ve sonunda SaveChanges çaðýrýr;
        //    o anda bekleyen bloke kaydý da birlikte yazýlýr -> ya ikisi birden ya hiçbiri.
        await _blockRepository.CreateAsync(new UserBlock
        {
            UserId = user.Id,
            BlockedByAdminId = adminId,
            Reason = request.Reason!.Trim(),
            EvidenceUrl = evidenceUrl,
            BlockedAt = now
        });

        // Hesap kapatma sürecindeyse o süreç de sona erer (bloke kaldýrýlýrsa doðrudan Aktif olur)
        user.AccountStatus = AccountStatus.Blocked;
        user.DeletionRequestedAt = null;
        user.DeletionReminderSentAt = null;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            await _fileService.DeleteAsync(evidenceUrl, cancellationToken);   // yüklenen kanýtý geri al
            return BaseResult<object>.Fail(result.Errors);
        }

        // 5) Ýçerikleri gizle (zaten gizliyse tekrar dokunmaz: !IsDeleted koþulu var)
        await _accountContentService.HideContentAsync(user.Id, cancellationToken);

        // LOG: Gerekçe metnini loglamýyoruz (kiþisel bilgi içerebilir); o zaten UserBlock tablosunda.
        // DEÐÝÞTÝ: alan adý {TargetUserId} (diðer olaylarla ayný ad -> "bu kiþinin bütün olaylarý" aramasý çalýþsýn)
        _logger.LogWarning("{EventType}: Kullanýcý bloke edildi. Hedef: {TargetUserId}",
                           LogEvents.UserBlocked, user.Id);

        // 6) Bilgilendirme maili (gönderilemese bile bloke geçerli)
        //    Gönderilemezse hata AccountEmailService içinde EmailFailed olarak loglanýr.
        try
        {
            await _accountEmailService.SendBlockedAsync(user, request.Reason!.Trim());
        }
        catch
        {
            // mail sunucusu hatasý iþlemi bozmasýn
        }

        return BaseResult<object>.Success();
    }
}