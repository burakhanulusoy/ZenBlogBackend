using MediatR;
using Microsoft.AspNetCore.Identity;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Queries.WriterApplicationQueries;
using ZenBlog.Application.Features.Results.WriterApplicationResults;
using ZenBlog.Application.Services.AccountServices;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.WriterApplicationHandlers;

public class GetMyWriterApplicationQueryHandler(UserManager<AppUser> _userManager,
                                                IRepository<WriterApplication> _applicationRepository,
                                                ICurrentUserService _currentUser)
    : IRequestHandler<GetMyWriterApplicationQuery, BaseResult<MyWriterApplicationResult>>
{
    public async Task<BaseResult<MyWriterApplicationResult>> Handle(GetMyWriterApplicationQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return BaseResult<MyWriterApplicationResult>.Fail("Kullanýcý bilgisi alýnamadý. Lütfen tekrar giriþ yapýn.");

        var user = await _userManager.FindByIdAsync(_currentUser.UserId.Value.ToString());
        if (user is null)
            return BaseResult<MyWriterApplicationResult>.Fail("Kullanýcý bulunamadý.");

        var roles = await _userManager.GetRolesAsync(user);
        var isWriter = roles.Contains(Roles.Writer) || roles.Contains(Roles.Admin);

        // En son baþvuru
        var latest = (await _applicationRepository.GetAllFilterAsync(a => a.UserId == user.Id))
            .OrderByDescending(a => a.SubmittedAt)
            .FirstOrDefault();

        var eligibility = WriterApplicationEligibility.Check(isWriter, latest, DateTime.Now);

        var summary = latest is null ? null : new WriterApplicationSummaryResult(
            latest.Id, latest.Status.ToString(), latest.Topics,
            latest.SubmittedAt, latest.DecidedAt, latest.RejectReason);

        return BaseResult<MyWriterApplicationResult>.Success(new MyWriterApplicationResult(
            isWriter, eligibility.CanApply, eligibility.Reason, eligibility.CanApplyAfter, summary));
    }
}