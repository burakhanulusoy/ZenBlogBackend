using MediatR;
using Microsoft.AspNetCore.Identity;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Queries.WriterApplicationQueries;
using ZenBlog.Application.Features.Results.WriterApplicationResults;
using ZenBlog.Domain.Entities;
using ZenBlog.Domain.Enums;

namespace ZenBlog.Application.Features.Handlers.WriterApplicationHandlers;

public class GetWriterApplicationByIdQueryHandler(UserManager<AppUser> _userManager,
                                                  IRepository<WriterApplication> _applicationRepository,
                                                  IUnitOfWork _unitOfWork)
    : IRequestHandler<GetWriterApplicationByIdQuery, BaseResult<WriterApplicationDetailResult>>
{
    public async Task<BaseResult<WriterApplicationDetailResult>> Handle(GetWriterApplicationByIdQuery request, CancellationToken cancellationToken)
    {
        var application = await _applicationRepository.GetByIdAsync(request.Id);
        if (application is null)
            return BaseResult<WriterApplicationDetailResult>.Fail("Başvuru bulunamadı.");

        // AÇILDI → "okundu" (sadece ilk açılışta; e-posta kutusundaki gibi)
        if (application.Status == WriterApplicationStatus.Pending)
        {
            application.Status = WriterApplicationStatus.Reviewed;
            application.ReviewedAt = DateTime.Now;
            _applicationRepository.Update(application);
            await _unitOfWork.SaveChangesAsync();
        }

        var user = await _userManager.FindByIdAsync(application.UserId.ToString());

        string? decidedByName = null;
        if (application.DecidedByAdminId.HasValue)
        {
            var admin = await _userManager.FindByIdAsync(application.DecidedByAdminId.Value.ToString());
            decidedByName = admin is null ? "Bilinmeyen yönetici" : $"{admin.Name} {admin.Surname}".Trim();
        }

        // Bu kişinin önceki başvuruları (bu hariç)
        var previousCount = (await _applicationRepository.GetAllFilterAsync(
            a => a.UserId == application.UserId && a.Id != application.Id)).Count;

        var result = new WriterApplicationDetailResult(
            application.Id, application.UserId,
            user is null ? "Bilinmeyen kullanıcı" : $"{user.Name} {user.Surname}".Trim(),
            user?.UserName, user?.Email, user?.ImageUrl,
            user?.AccountStatus.ToString() ?? "Active",
            application.Topics, application.Motivation, application.Experience, application.SampleLinks,
            application.Status.ToString(), application.SubmittedAt, application.ReviewedAt,
            application.DecidedAt, decidedByName, application.RejectReason,
            previousCount);

        return BaseResult<WriterApplicationDetailResult>.Success(result);
    }
}