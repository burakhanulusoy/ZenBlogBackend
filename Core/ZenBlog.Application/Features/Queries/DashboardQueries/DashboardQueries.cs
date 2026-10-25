using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Features.Results.DashboardResults;

namespace ZenBlog.Application.Features.Queries.DashboardQueries;

public record GetAdminDashboardQuery(string? Period) : IRequest<BaseResult<AdminDashboardResult>>;

// Yazar: kim olduðu token'dan
public record GetWriterDashboardQuery(string? Period) : IRequest<BaseResult<WriterDashboardResult>>;