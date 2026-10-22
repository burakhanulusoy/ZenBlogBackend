using MediatR;
using Microsoft.AspNetCore.Identity;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Queries.WriterApplicationQueries;
using ZenBlog.Application.Features.Results.WriterApplicationResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.WriterApplicationHandlers;

public class GetWriterApplicationsQueryHandler(UserManager<AppUser> _userManager,
                                               IRepository<WriterApplication> _applicationRepository)
    : IRequestHandler<GetWriterApplicationsQuery, BaseResult<List<WriterApplicationListItemResult>>>
{
    public async Task<BaseResult<List<WriterApplicationListItemResult>>> Handle(GetWriterApplicationsQuery request, CancellationToken cancellationToken)
    {
        var applications = await _applicationRepository.GetAllAsync();

        // Baþvuranlarýn bilgilerini tek sorguda al (kullanýcý Id ? bilgi)
        var userIds = applications.Select(a => a.UserId).Distinct().ToList();
        var users = _userManager.Users
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new { u.Id, u.Name, u.Surname, u.UserName, u.Email, u.ImageUrl })
            .ToList()
            .ToDictionary(u => u.Id);

        var result = applications
            .OrderByDescending(a => a.SubmittedAt)
            .Select(a =>
            {
                users.TryGetValue(a.UserId, out var u);
                return new WriterApplicationListItemResult(
                    a.Id, a.UserId,
                    u is null ? "Bilinmeyen kullanýcý" : $"{u.Name} {u.Surname}".Trim(),
                    u?.UserName, u?.Email, u?.ImageUrl,
                    a.Topics, a.Status.ToString(), a.SubmittedAt, a.ReviewedAt);
            })
            .ToList();

        return BaseResult<List<WriterApplicationListItemResult>>.Success(result);
    }
}