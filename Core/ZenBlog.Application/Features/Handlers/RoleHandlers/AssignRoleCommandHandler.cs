using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.RoleCommands;
using ZenBlog.Application.Services.MailServices;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.RoleHandlers;

public class AssignRoleCommandHandler(UserManager<AppUser> _userManager,
                                      RoleManager<AppRole> _roleManager,
                                      IRepository<RoleChangeLog> _roleLogRepository,
                                      ICurrentUserService _currentUser,
                                      IAccountEmailService _accountEmailService,
                                      ILogger<AssignRoleCommandHandler> _logger)   // DEÐÝÞTÝ: logu yazan sýnýf
    : IRequestHandler<AssignRoleCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        // Ýþlemi yapan admin -> token'dan
        if (_currentUser.UserId is null)
            return BaseResult<object>.Fail("Kullanýcý bilgisi alýnamadý. Lütfen tekrar giriþ yapýn.");

        var adminId = _currentUser.UserId.Value;

        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
            return BaseResult<object>.Fail("Kullanýcý bulunamadý.");

        // 1) Ýstenen roller gerçekten var mý?
        var requestedIds = request.RoleIds.Distinct().ToList();

        var selectedRoleNames = _roleManager.Roles
            .Where(r => requestedIds.Contains(r.Id))
            .Select(r => r.Name!)
            .ToList();

        if (selectedRoleNames.Count != requestedIds.Count)
            return BaseResult<object>.Fail("Gönderilen rollerden bazýlarý bulunamadý.");

        // 2) YENÝ: User rolü herkeste kalýr (temel rol; listede yoksa biz ekliyoruz)
        if (!selectedRoleNames.Contains(Roles.User))
            selectedRoleNames.Add(Roles.User);

        // 3) Ne eklenecek, ne alýnacak? (LINQ Except: birinde olup diðerinde olmayanlar)
        var currentRoles = await _userManager.GetRolesAsync(user);
        var rolesToAdd = selectedRoleNames.Except(currentRoles).ToList();
        var rolesToRemove = currentRoles.Except(selectedRoleNames).ToList();

        if (rolesToAdd.Count == 0 && rolesToRemove.Count == 0)
            return BaseResult<object>.Success("Rollerde deðiþiklik yapýlmadý.");

        // 4) YENÝ: ADMÝN ROLÜNÜ ALMA KURALLARI
        if (rolesToRemove.Contains(Roles.Admin))
        {
            if (user.Id == adminId)
                return BaseResult<object>.Fail("Kendi yönetici rolünü kaldýramazsýn.");

            var admins = await _userManager.GetUsersInRoleAsync(Roles.Admin);
            if (admins.Count <= 1)
                return BaseResult<object>.Fail("Sistemde en az bir yönetici kalmalý.");
        }

        // 5) YENÝ: ADMÝN ROLÜ VERÝLÝYORSA -> iþlemi yapan adminin þifresiyle onay
        if (rolesToAdd.Contains(Roles.Admin))
        {
            var admin = await _userManager.FindByIdAsync(adminId.ToString());
            if (admin is null || !await _userManager.HasPasswordAsync(admin))
                return BaseResult<object>.Fail("AdminPassword",
                    "Yönetici yetkisi vermek için hesabýnda bir þifre olmalý.");

            if (await _userManager.IsLockedOutAsync(admin))
                return BaseResult<object>.Fail("AdminPassword",
                    "Çok fazla hatalý deneme yapýldý. Lütfen daha sonra tekrar dene.");

            if (string.IsNullOrWhiteSpace(request.AdminPassword))
                return BaseResult<object>.Fail("AdminPassword", "Yönetici yetkisi vermek için þifreni gir.");

            if (!await _userManager.CheckPasswordAsync(admin, request.AdminPassword))
            {
                await _userManager.AccessFailedAsync(admin);
                return BaseResult<object>.Fail("AdminPassword", "Þifre hatalý.");
            }

            await _userManager.ResetAccessFailedCountAsync(admin);
        }

        // 6) Rolleri uygula
        if (rolesToAdd.Count > 0)
        {
            var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
            if (!addResult.Succeeded)
                return BaseResult<object>.Fail(addResult.Errors);
        }

        if (rolesToRemove.Count > 0)
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            if (!removeResult.Succeeded)
                return BaseResult<object>.Fail(removeResult.Errors);
        }

        // 7) YENÝ: GEÇMÝÞ KAYITLARI
        //    CreateAsync kayýtlarý sadece DbContext'e ekler; bir sonraki adýmdaki
        //    UpdateSecurityStampAsync, SaveChanges çaðýrdýðý için hepsi birlikte yazýlýr.
        var now = DateTime.Now;
        foreach (var role in rolesToAdd)
        {
            await _roleLogRepository.CreateAsync(new RoleChangeLog
            {
                UserId = user.Id,
                ChangedByAdminId = adminId,
                RoleName = role,
                IsAdded = true,
                ChangedAt = now
            });
        }
        foreach (var role in rolesToRemove)
        {
            await _roleLogRepository.CreateAsync(new RoleChangeLog
            {
                UserId = user.Id,
                ChangedByAdminId = adminId,
                RoleName = role,
                IsAdded = false,
                ChangedAt = now
            });
        }

        // 8) YENÝ: Hesap damgasýný yenile -> kiþinin elindeki eski token geçersiz -> oturumu düþer
        var stampResult = await _userManager.UpdateSecurityStampAsync(user);
        if (!stampResult.Succeeded)
            return BaseResult<object>.Fail(stampResult.Errors);

        // LOG: Yetki deðiþikliði her zaman izlenmeli -> Warning seviyesinde.
        // Eklenen ve alýnan roller AYRI alanlarda; iþlemi yapan admin (UserId) LoggingBehavior'dan otomatik gelir.
        _logger.LogWarning("{EventType}: Roller deðiþti. Hedef: {TargetUserId}, Eklenen: {AddedRoles}, Alýnan: {RemovedRoles}",
                           LogEvents.RoleChanged, user.Id,
                           string.Join(",", rolesToAdd), string.Join(",", rolesToRemove));

        // 9) YENÝ: Bilgilendirme maili (gönderilemese bile rol deðiþikliði geçerli)
        //    Gönderilemezse hata AccountEmailService içinde EmailFailed olarak loglanýr.
        try
        {
            await _accountEmailService.SendRolesChangedAsync(user, rolesToAdd, rolesToRemove);
        }
        catch
        {
            // mail sunucusu hatasý iþlemi bozmasýn
        }

        return BaseResult<object>.Success("Kullanýcýnýn rolleri güncellendi.");
    }
}