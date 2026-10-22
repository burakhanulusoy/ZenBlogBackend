using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Results.UserResults;

namespace ZenBlog.Application.Features.Commands.UserCommands;

// "Hesabýmý kapat": Id yok, kim olduðu token'dan bulunuyor. Sadece þifre onayý isteniyor.
public record CloseMyAccountCommand(string? CurrentPassword) : IRequest<BaseResult<CloseMyAccountResult>>;