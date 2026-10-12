using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.UserCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.UserHandlers
{
    public class CreateUserCommandHandler(UserManager<AppUser> _userManager,
                                          IMapper _mapper) : IRequestHandler<CreateUserCommand, BaseResult<object>>
    {
        public async Task<BaseResult<object>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
           
            if(request.Password != request.ConfirmPassword)
            {
                return BaseResult<object>.Fail("Þifreleriniz birbiriyle uyuþmuyor");
            }
            
            var user = _mapper.Map<AppUser>(request);

            var result = await _userManager.CreateAsync(user,request.Password);

            if(!result.Succeeded)
            {
                return BaseResult<object>.Fail(result.Errors);
            }

            return BaseResult<object>.Success(new { Message = "Created sUCCESS" });


        }
    }
}
