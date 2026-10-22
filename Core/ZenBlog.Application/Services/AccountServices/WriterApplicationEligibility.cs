using ZenBlog.Application.Constants;
using ZenBlog.Domain.Entities;
using ZenBlog.Domain.Enums;

namespace ZenBlog.Application.Services.AccountServices;

// "Bu kişi şu an yazarlık başvurusu yapabilir mi?"
// Hem başvuru sayfası (GetMy...) hem başvuru gönderme (Submit...) AYNI kuralı kullansın diye tek yer.
// static: durum tutmuyor, sadece hesap yapıyor → DI'a kaydetmeye gerek yok.
public static class WriterApplicationEligibility
{
    public record Result(bool CanApply, string? Reason, DateTime? CanApplyAfter);

    // isWriter: Writer ya da Admin rolü var mı
    // latest: kişinin en son başvurusu (yoksa null)
    public static Result Check(bool isWriter, WriterApplication? latest, DateTime now)
    {
        if (isWriter)
            return new Result(false, "Zaten yazarsın.", null);

        if (latest is null)
            return new Result(true, null, null);

        if (latest.Status is WriterApplicationStatus.Pending or WriterApplicationStatus.Reviewed)
            return new Result(false, "Değerlendirilmeyi bekleyen bir başvurun var.", null);

        if (latest.Status == WriterApplicationStatus.Rejected && latest.DecidedAt.HasValue)
        {
            var reapplyAt = latest.DecidedAt.Value.AddDays(WriterApplicationRules.ReapplyDays);
            if (now < reapplyAt)
                return new Result(false,
                    $"Son başvurun reddedildi. {reapplyAt:dd.MM.yyyy} tarihinden sonra tekrar başvurabilirsin.",
                    reapplyAt);
        }

        // Onaylanmış ama sonradan yazarlığı alınmış kişi tekrar başvurabilir
        return new Result(true, null, null);
    }
}