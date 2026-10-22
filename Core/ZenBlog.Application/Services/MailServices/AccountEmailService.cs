using System.Globalization;
using System.Net;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Services.MailServices;

public class AccountEmailService(IEmailService _emailService) : IAccountEmailService
{
    // Tarihler Türkçe yazýlsýn: "21 Kasým 2026"
    private static readonly CultureInfo Turkish = new("tr-TR");

    public Task SendClosureScheduledAsync(AppUser user, DateTime deadline)
    {
        var body = $"""
            <p>Hesabýný kapatma isteðini aldýk.</p>
            <p>Hesabýn <strong>{FormatDate(deadline)}</strong> tarihinde kalýcý olarak kapanacak.
               O zamana kadar bloglarýn, yorumlarýn ve profilin sitede görünmeyecek.</p>
            <p><strong>Fikrini deðiþtirirsen:</strong> bu tarihe kadar giriþ yapman yeterli.
               Kapatma iþlemi otomatik olarak iptal edilir ve her þey eski haline döner.</p>
            <p>Süre dolduktan sonra bu e-posta adresi ve kullanýcý adýyla tekrar kayýt olunamaz.</p>
            <p style="color:#8a5300;">Bu iþlemi sen yapmadýysan hemen giriþ yap ve þifreni deðiþtir.</p>
            """;

        return SendAsync(user, "ZenBlog - Hesabýn kapatýlýyor", "Hesabýn kapatýlýyor", body);
    }

    public Task SendClosureReminderAsync(AppUser user, DateTime deadline)
    {
        var body = $"""
            <p>Hesabýnýn kalýcý olarak kapanmasýna <strong>{AccountRules.ReminderDaysBefore} gün</strong> kaldý.</p>
            <p>Kapanýþ tarihi: <strong>{FormatDate(deadline)}</strong></p>
            <p>Hesabýný kullanmaya devam etmek istiyorsan bu tarihten önce giriþ yapman yeterli;
               bloglarýn ve yorumlarýn geri gelir.</p>
            <p>Bir þey yapmazsan hesabýn kapanacak ve bu e-posta adresiyle tekrar kayýt olunamayacak.</p>
            """;

        return SendAsync(user, "ZenBlog - Hesabýnýn kapanmasýna 3 gün kaldý", "Son 3 gün", body);
    }

    public Task SendBlockedAsync(AppUser user, string reason)
    {
        // HtmlEncode: admin'in yazdýðý gerekçe mail içinde HTML olarak çalýþmasýn (güvenlik)
        var body = $"""
            <p>ZenBlog hesabýn, topluluk kurallarýna aykýrý davranýþ nedeniyle <strong>bloke edildi</strong>.</p>
            <p><strong>Gerekçe:</strong></p>
            <blockquote style="margin:0 0 16px;padding:12px 16px;border-left:4px solid #dc2626;background:#fef2f2;color:#7f1d1d;">
                {WebUtility.HtmlEncode(reason)}
            </blockquote>
            <p>Bloglarýn ve yorumlarýn sitede artýk görünmüyor.</p>
            <p><strong>{WebUtility.HtmlEncode(user.Email)}</strong> e-posta adresi ve
               <strong>@{WebUtility.HtmlEncode(user.UserName)}</strong> kullanýcý adýyla tekrar kayýt olamazsýn.</p>
            <p>Bir yanlýþlýk olduðunu düþünüyorsan bizimle iletiþime geçebilirsin.</p>
            """;

        return SendAsync(user, "ZenBlog - Hesabýn bloke edildi", "Hesabýn bloke edildi", body);
    }

    public Task SendUnblockedAsync(AppUser user)
    {
        var body = """
            <p>Hesabýndaki bloke kaldýrýldý. Tekrar giriþ yapabilirsin.</p>
            <p>Bloglarýn ve yorumlarýn yeniden yayýnda.</p>
            <p>ZenBlog'a tekrar hoþ geldin.</p>
            """;

        return SendAsync(user, "ZenBlog - Hesabýn tekrar açýldý", "Hesabýn tekrar açýldý", body);
    }

    // ---------------------------------------------------------------
    // ORTAK MAÝL ÇERÇEVESÝ: bütün mailler ayný görünsün
    // (Mail programlarý CSS dosyasý okumaz; stiller satýr içi yazýlýr)
    // ---------------------------------------------------------------
    private Task SendAsync(AppUser user, string subject, string heading, string content)
    {
        var html = $"""
            <div style="font-family:Arial,Helvetica,sans-serif;background:#f5f6f3;padding:24px;">
              <div style="max-width:560px;margin:0 auto;background:#ffffff;border:1px solid #e4e7e1;border-radius:12px;overflow:hidden;">
                <div style="background:#065f46;color:#ffffff;padding:18px 24px;font-size:18px;font-weight:bold;">ZenBlog</div>
                <div style="padding:24px;color:#1c2420;font-size:15px;line-height:1.6;">
                  <h2 style="margin:0 0 12px;font-size:20px;">{heading}</h2>
                  <p>Merhaba {WebUtility.HtmlEncode(user.Name)},</p>
                  {content}
                </div>
                <div style="padding:14px 24px;border-top:1px solid #e4e7e1;color:#7a847e;font-size:12px;">
                  Bu e-posta ZenBlog tarafýndan otomatik gönderildi.
                </div>
              </div>
            </div>
            """;

        return _emailService.SendEmailAsync(user.Email!, subject, html);
    }

    private static string FormatDate(DateTime date) => date.ToString("d MMMM yyyy", Turkish);
}