using MediatR;
using Microsoft.AspNetCore.Identity;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Queries.RoleQueries;
using ZenBlog.Application.Features.Results.RoleResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.RoleHandlers;

// Rol atama ekranýný doldurur: sistemdeki her rolü listeler ve
// kullanýcýnýn hangilerine sahip olduðunu iþaretler (kutucuklar).
//
// Mapper kullanmýyoruz: RoleExist alaný AppRole'de yok, kullanýcýya göre HESAPLANIYOR.
// Veri birden fazla kaynaktan birleþtiði için sonucu elle oluþturuyoruz.
public class GetUserRolesQueryHandler(UserManager<AppUser> _userManager,
                                      RoleManager<AppRole> _roleManager)
    : IRequestHandler<GetUserRolesQuery, BaseResult<GetUserRolesQueryResult>>
{
    public async Task<BaseResult<GetUserRolesQueryResult>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
            return BaseResult<GetUserRolesQueryResult>.Fail("Kullanýcý bulunamadý.");

        // Kullanýcýnýn sahip olduðu rollerin adlarý ? örn. ["Admin", "User"]
        var userRoles = await _userManager.GetRolesAsync(user);

        var roles = _roleManager.Roles
     .OrderBy(r => r.Name)                          // her rolü adýna göre sýrala
     .Select(r => new UserRoleItemResult(           // her rolü bir kutucuk satýrýna dönüþtür
         r.Id,
         r.Name!,
         userRoles.Contains(r.Name!)))              // kullanýcýda bu rol var mý?
     .ToList();                                     // sorguyu çalýþtýr, sonucu listeye çevir

        var result = new GetUserRolesQueryResult(user.Id, $"{user.Name} {user.Surname}", roles);

        return BaseResult<GetUserRolesQueryResult>.Success(result);
    }
}