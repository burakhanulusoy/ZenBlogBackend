using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Services.MailServices;
using ZenBlog.Domain.Entities;
using ZenBlog.Domain.Enums;

namespace ZenBlog.Infrastructure.Services.BackgroundJobs;

// ARKA PLAN GÖREVÝ: "Hesabýnýn kapanmasýna 3 gün kaldý" maili.
// API çalýþtýðý sürece kendi kendine döner: açýlýþta bir kez, sonra 6 saatte bir.
//
// BackgroundService: ASP.NET Core'un hazýr "arka planda sürekli çalýþan iþ" sýnýfý.
// Uygulamayla birlikte baþlar, uygulama kapanýnca durur.
public class AccountDeletionReminderService(IServiceScopeFactory _scopeFactory) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromHours(6);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // PeriodicTimer: belirli aralýklarla "týk" atan zamanlayýcý
        using var timer = new PeriodicTimer(Interval);

        try
        {
            do
            {
                try
                {
                    await SendRemindersAsync();
                }
                catch (Exception)
                {
                    // Bir turda hata olursa (mail sunucusu, veritabaný...) görev çökmesin;
                    // 6 saat sonra tekrar dener. Hata dýþarý kaçarsa bütün API kapanabilir.
                }
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }
        catch (OperationCanceledException)
        {
            // Uygulama kapanýyor ? sessizce çýk
        }
    }

    private async Task SendRemindersAsync()
    {
        // Arka plan görevi tek bir HTTP isteðine baðlý deðil; UserManager gibi "scoped" servisleri
        // kullanmak için kendi kapsamýmýzý (scope) açýyoruz. (using bitince servisler temizlenir)
        using var scope = _scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var accountEmailService = scope.ServiceProvider.GetRequiredService<IAccountEmailService>();

        var now = DateTime.Now;

        // Kapatma isteði bu tarihten ÖNCE yapýldýysa son 3 güne girmiþ demektir (30 - 3 = 27 gün önce)
        var reminderThreshold = now.AddDays(-(AccountRules.DeletionGraceDays - AccountRules.ReminderDaysBefore));

        // Bu tarihten önce yapýldýysa 30 gün dolmuþ, hatýrlatmanýn anlamý kalmamýþ
        var expiredThreshold = now.AddDays(-AccountRules.DeletionGraceDays);

        var users = userManager.Users
            .Where(u => u.AccountStatus == AccountStatus.PendingDeletion
                     && u.DeletionReminderSentAt == null
                     && u.DeletionRequestedAt != null
                     && u.DeletionRequestedAt <= reminderThreshold
                     && u.DeletionRequestedAt > expiredThreshold)
            .ToList();

        foreach (var user in users)
        {
            var deadline = user.DeletionRequestedAt!.Value.AddDays(AccountRules.DeletionGraceDays);

            await accountEmailService.SendClosureReminderAsync(user, deadline);

            // Gönderildi diye iþaretle ? bir sonraki turda ayný kiþiye tekrar gitmesin
            user.DeletionReminderSentAt = now;
            await userManager.UpdateAsync(user);
        }
    }
}