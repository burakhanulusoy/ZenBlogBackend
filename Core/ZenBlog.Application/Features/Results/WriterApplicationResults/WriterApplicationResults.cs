namespace ZenBlog.Application.Features.Results.WriterApplicationResults;

// Kullanýcýnýn baþvuru sayfasý
public record MyWriterApplicationResult(bool IsWriter,
                                        bool CanApply,
                                        string? CannotApplyReason,
                                        DateTime? CanApplyAfter,
                                        WriterApplicationSummaryResult? Latest);

// Kullanýcýnýn kendi son baþvurusu (özet)
public record WriterApplicationSummaryResult(Guid Id,
                                             string Status,
                                             string Topics,
                                             DateTime SubmittedAt,
                                             DateTime? DecidedAt,
                                             string? RejectReason);

// Admin listesi
// Status: "Pending" | "Reviewed" | "Approved" | "Rejected"
public record WriterApplicationListItemResult(Guid Id,
                                              Guid UserId,
                                              string FullName,
                                              string? UserName,
                                              string? Email,
                                              string? ImageUrl,
                                              string Topics,
                                              string Status,
                                              DateTime SubmittedAt,
                                              DateTime? ReviewedAt);

// Admin detayý
public record WriterApplicationDetailResult(Guid Id,
                                            Guid UserId,
                                            string FullName,
                                            string? UserName,
                                            string? Email,
                                            string? ImageUrl,
                                            string AccountStatus,
                                            string Topics,
                                            string Motivation,
                                            string? Experience,
                                            string? SampleLinks,
                                            string Status,
                                            DateTime SubmittedAt,
                                            DateTime? ReviewedAt,
                                            DateTime? DecidedAt,
                                            string? DecidedByAdminName,
                                            string? RejectReason,
                                            int PreviousApplicationCount);   // bu kiþinin önceki baþvuru sayýsý