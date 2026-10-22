using MediatR;
using Microsoft.AspNetCore.Identity;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.WriterApplicationCommands;
using ZenBlog.Application.Services.AccountServices;
using ZenBlog.Application.Services.MailServices;
using ZenBlog.Domain.Entities;
using ZenBlog.Domain.Enums;

namespace ZenBlog.Application.Features.Handlers.WriterApplicationHandlers;

public class SubmitWriterApplicationCommandHandler(UserManager<AppUser> _userManager,
                                                   IRepository<WriterApplication> _applicationRepository,
                                                   IRepository<Category> _categoryRepository,
                                                   IUnitOfWork _unitOfWork,
                                                   ICurrentUserService _currentUser,
                                                   IAccountEmailService _accountEmailService)
    : IRequestHandler<SubmitWriterApplicationCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(SubmitWriterApplicationCommand request, CancellationToken cancellationToken)
    {
        // 1) KÝM? ? token'dan
        if (_currentUser.UserId is null)
            return BaseResult<object>.Fail("Kullanýcý bilgisi alýnamadý. Lütfen tekrar giriþ yapýn.");

        var user = await _userManager.FindByIdAsync(_currentUser.UserId.Value.ToString());
        if (user is null)
            return BaseResult<object>.Fail("Kullanýcý bulunamadý.");

        // 2) Baþvurabilir mi? (baþvuru sayfasýndaki kuralýn aynýsý)
        var roles = await _userManager.GetRolesAsync(user);
        var isWriter = roles.Contains(Roles.Writer) || roles.Contains(Roles.Admin);

        var latest = (await _applicationRepository.GetAllFilterAsync(a => a.UserId == user.Id))
            .OrderByDescending(a => a.SubmittedAt)
            .FirstOrDefault();

        var now = DateTime.Now;
        var eligibility = WriterApplicationEligibility.Check(isWriter, latest, now);
        if (!eligibility.CanApply)
            return BaseResult<object>.Fail(eligibility.Reason!);

        // 3) Seçilen kategoriler gerçekten var mý? ? adlarýný yazý olarak sakla
        var categoryIds = request.CategoryIds.Distinct().ToList();
        var categories = await _categoryRepository.GetAllFilterAsync(c => categoryIds.Contains(c.Id));
        if (categories.Count != categoryIds.Count)
            return BaseResult<object>.Fail("CategoryIds", "Seçilen konulardan bazýlarý bulunamadý.");

        var topics = string.Join(", ", categories.OrderBy(c => c.Name).Select(c => c.Name));

        // 4) Kaydet
        await _applicationRepository.CreateAsync(new WriterApplication
        {
            UserId = user.Id,
            Topics = topics,
            Motivation = request.Motivation!.Trim(),
            Experience = Clean(request.Experience),
            SampleLinks = Clean(request.SampleLinks),
            Status = WriterApplicationStatus.Pending,
            SubmittedAt = now
        });
        await _unitOfWork.SaveChangesAsync();

        // 5) "Baþvurun alýndý" maili
        try
        {
            await _accountEmailService.SendWriterApplicationReceivedAsync(user);
        }
        catch
        {
            // mail sunucusu hatasý iþlemi bozmasýn
        }

        return BaseResult<object>.Success("Baþvurun alýndý.");
    }

    private static string? Clean(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}