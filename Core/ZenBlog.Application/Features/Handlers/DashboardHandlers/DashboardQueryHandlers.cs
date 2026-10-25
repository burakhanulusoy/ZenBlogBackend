using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Queries.DashboardQueries;
using ZenBlog.Application.Features.Results.DashboardResults;

namespace ZenBlog.Application.Features.Handlers.DashboardHandlers;

public class GetAdminDashboardQueryHandler(IDashboardReadService _dashboardReadService)
    : IRequestHandler<GetAdminDashboardQuery, BaseResult<AdminDashboardResult>>
{
    public async Task<BaseResult<AdminDashboardResult>> Handle(GetAdminDashboardQuery request, CancellationToken cancellationToken)
    {
        var period = DashboardPeriods.Normalize(request.Period);
        var result = await _dashboardReadService.GetAdminAsync(period, cancellationToken);
        return BaseResult<AdminDashboardResult>.Success(result);
    }
}

public class GetWriterDashboardQueryHandler(IDashboardReadService _dashboardReadService,
                                            ICurrentUserService _currentUser)
    : IRequestHandler<GetWriterDashboardQuery, BaseResult<WriterDashboardResult>>
{
    public async Task<BaseResult<WriterDashboardResult>> Handle(GetWriterDashboardQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return BaseResult<WriterDashboardResult>.Fail("Kullanýcý bilgisi alýnamadý.");

        var period = DashboardPeriods.Normalize(request.Period);
        var result = await _dashboardReadService.GetWriterAsync(_currentUser.UserId.Value, period, cancellationToken);
        return BaseResult<WriterDashboardResult>.Success(result);
    }
}