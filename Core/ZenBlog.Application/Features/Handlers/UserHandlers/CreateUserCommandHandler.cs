using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Commands.UserCommands;
using ZenBlog.Application.Services.MailServices;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.UserHandlers;

public class CreateUserCommandHandler(UserManager<AppUser> _userManager,
                                      IMapper _mapper,
                                      IEmailConfirmationService _emailConfirmationService)
    : IRequestHandler<CreateUserCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // 1) Gelen bilgileri AppUser nesnesine çevir
        var user = _mapper.Map<AppUser>(request);

        // 2) Kullanýcýyý oluþtur.
        //    Þifre kurallarý, ayný e-posta/kullanýcý adý kontrolü burada Identity tarafýndan yapýlýr.
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return BaseResult<object>.Fail(result.Errors);  // CustomErrorDescriber sayesinde Türkçe

        // 3) Doðrulama e-postasýný gönder
        await _emailConfirmationService.SendConfirmationEmailAsync(user);

        return BaseResult<object>.Success(new { Message = "Kayýt baþarýlý. Lütfen e-postanýzý doðrulayýn." });
    }
}