using MediatR;
using Microsoft.AspNetCore.Identity;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Application.Features.Queries.UserQueries;
using ZenBlog.Application.Features.Results.UserResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.UserHandlers;

public class GetMyProfileQueryHandler(UserManager<AppUser> _userManager,
                                      ICurrentUserService _currentUser)
    : IRequestHandler<GetMyProfileQuery, BaseResult<GetMyProfileQueryResult>>
{
    public async Task<BaseResult<GetMyProfileQueryResult>> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        // Kim olduðunu token söylüyor; istekten Id almýyoruz
        if (_currentUser.UserId is null)
            return BaseResult<GetMyProfileQueryResult>.Fail("Kullanýcý bilgisi alýnamadý. Lütfen tekrar giriþ yapýn.");

        var user = await _userManager.FindByIdAsync(_currentUser.UserId.Value.ToString());
        if (user is null)
            return BaseResult<GetMyProfileQueryResult>.Fail("Kullanýcý bulunamadý.");

        var roles = await _userManager.GetRolesAsync(user);
        var hasPassword = await _userManager.HasPasswordAsync(user);

        var result = new GetMyProfileQueryResult(
            user.Id, user.Name, user.Surname, user.UserName, user.Email, user.PhoneNumber,
            user.ImageUrl, user.Location, user.Description,
            user.InstagramUrl, user.TwitterUrl, user.LinkedlnUrl, user.FacebookUrl,
            hasPassword,
            !roles.Contains(Roles.Admin),   // CanCloseAccount: Admin hesabýný kapatamaz
            roles.ToList());

        return BaseResult<GetMyProfileQueryResult>.Success(result);
    }
}