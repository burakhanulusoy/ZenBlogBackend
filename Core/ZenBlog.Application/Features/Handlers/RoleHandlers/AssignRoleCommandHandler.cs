using MediatR;
using Microsoft.AspNetCore.Identity;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Commands.RoleCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.RoleHandlers;

// Admin ekranda iþaretlediði rollerin Id'lerini gönderir.
// Handler kullanýcýnýn rollerini tam olarak bu listeye eþitler:
// eksikleri ekler, fazlalarý çýkarýr, ortak olanlara dokunmaz.
//
// Aþaðýdaki örneklerde veritabanýnda þu roller olduðunu varsayýyoruz:
//   Id: 111 ? Admin
//   Id: 222 ? Writer
//   Id: 333 ? User
public class AssignRoleCommandHandler(UserManager<AppUser> _userManager,
                                      RoleManager<AppRole> _roleManager)
    : IRequestHandler<AssignRoleCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        // ---------------------------------------------------------------
        // 1) KULLANICIYI BUL
        // ---------------------------------------------------------------
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
            return BaseResult<object>.Fail("Kullanýcý bulunamadý.");

        // ---------------------------------------------------------------
        // 2) TEKRAR EDEN ID'LERÝ AT (Distinct)
        // ---------------------------------------------------------------
        // Gönderilen: [222, 222] ? requestedIds: [222]
        // Bu adým olmasaydý aþaðýdaki sayý kontrolü yanlýþ alarm verirdi:
        //   gönderilen 2 Id, veritabanýnda bulunan 1 rol ? 2 ? 1 ? "rol bulunamadý" hatasý
        //   (oysa Id geçerliydi, sadece iki kez yazýlmýþtý)
        var requestedIds = request.RoleIds.Distinct().ToList();

        // ---------------------------------------------------------------
        // 3) ID'LERÝ ROL ADLARINA ÇEVÝR
        // ---------------------------------------------------------------
        // Identity rol eklerken/çýkarýrken Id deðil AD istiyor (AddToRolesAsync(user, ["Admin"]) gibi).
        // Adlarý istemciden deðil veritabanýndan aldýðýmýz için Identity'ye her zaman
        // gerçekten var olan, doðru yazýlmýþ rol adlarý gidiyor.
        //
        // Where: veritabanýndaki her role (r) bakýp "Id'si gönderilen listede var mý?" diye soruyor.
        // Örnek: requestedIds = [222, 333]
        //   111 Admin  ? listede yok ? alýnmaz
        //   222 Writer ? listede var ? alýnýr
        //   333 User   ? listede var ? alýnýr
        // Select: alýnan rollerden sadece ADLARINI al ? ["Writer", "User"]
        var selectedRoleNames = _roleManager.Roles
            .Where(r => requestedIds.Contains(r.Id))
            .Select(r => r.Name!)
            .ToList();

        // ---------------------------------------------------------------
        // 4) GÖNDERÝLEN ID'LERÝN HEPSÝ GERÇEK BÝR ROLE MÝ AÝT?
        // ---------------------------------------------------------------
        // Bunu sayýlarý karþýlaþtýrarak anlýyoruz:
        //
        // Doðru istek: requestedIds = [222, 333]
        //   bulunan roller: ["Writer", "User"] ? 2 = 2 ? her Id gerçek bir role ait ?
        //
        // Hatalý istek: requestedIds = [222, 999]   (999 diye bir rol YOK)
        //   999 veritabanýnda olmadýðý için Where onunla eþleþen satýr bulamaz
        //   bulunan roller: ["Writer"] ? 2 ? 1 ? bir Id karþýlýksýz kaldý ?
        //
        // Bu kontrol olmasaydý 999 sessizce görmezden gelinir, sadece Writer atanýr ve
        // "roller güncellendi" mesajý dönerdi; admin istediði rolü atadýðýný sanýrdý.
        if (selectedRoleNames.Count != requestedIds.Count)
            return BaseResult<object>.Fail("Gönderilen rollerden bazýlarý bulunamadý.");

        // ---------------------------------------------------------------
        // 5) MEVCUT ROLLER ÝLE ÝSTENEN ROLLERÝ KARÞILAÞTIR
        // ---------------------------------------------------------------
        // Kullanýcýnýn þu anki rolleri ? örn. ["Admin", "User"]
        var currentRoles = await _userManager.GetRolesAsync(user);

        // Except: ilk listede olup ikinci listede OLMAYANLARI verir.
        // Örnek: Ýstenen = [Writer, User], Mevcut = [Admin, User]
        //
        // rolesToAdd    = Ýstenen.Except(Mevcut) ? [Writer]  (istenmiþ ama kullanýcýda yok ? EKLE)
        // rolesToRemove = Mevcut.Except(Ýstenen) ? [Admin]   (kullanýcýda var ama istenmiyor ? ÇIKAR)
        // "User" iki listede de var ? ne eklenir ne çýkarýlýr, dokunulmaz
        var rolesToAdd = selectedRoleNames.Except(currentRoles).ToList();
        var rolesToRemove = currentRoles.Except(selectedRoleNames).ToList();

        // ---------------------------------------------------------------
        // 6) EKSÝK ROLLERÝ EKLE
        // ---------------------------------------------------------------
        // Identity hata durumunda exception deðil IdentityResult döner;
        // bu yüzden sonucu MUTLAKA kontrol ediyoruz, yoksa hata sessizce yutulur.
        if (rolesToAdd.Count > 0)
        {
            var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
            if (!addResult.Succeeded)
                return BaseResult<object>.Fail(addResult.Errors);
        }

        // ---------------------------------------------------------------
        // 7) FAZLA ROLLERÝ ÇIKAR
        // ---------------------------------------------------------------
        if (rolesToRemove.Count > 0)
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            if (!removeResult.Succeeded)
                return BaseResult<object>.Fail(removeResult.Errors);
        }

        return BaseResult<object>.Success("Kullanýcýnýn rolleri atandý.");
    }
}