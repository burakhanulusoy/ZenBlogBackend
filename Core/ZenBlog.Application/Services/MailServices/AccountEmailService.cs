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
    // Rol adlarýnýn Türkçesi (özel roller olduðu gibi yazýlýr)
    private static readonly Dictionary<string, string> RoleLabels = new()
    {
        [Roles.Admin] = "Yönetici",
        [Roles.Writer] = "Yazar",
        [Roles.User] = "Okur"
    };
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
    // YENÝ: rol deðiþikliði maili
    public Task SendRolesChangedAsync(AppUser user, List<string> addedRoles, List<string> removedRoles)
    {
        string Label(string role) => WebUtility.HtmlEncode(RoleLabels.TryGetValue(role, out var label) ? label : role);

        var added = addedRoles.Count == 0 ? "" :
            $"<p><strong>Verilen rol:</strong> {string.Join(", ", addedRoles.Select(Label))}</p>";
        var removed = removedRoles.Count == 0 ? "" :
            $"<p><strong>Alýnan rol:</strong> {string.Join(", ", removedRoles.Select(Label))}</p>";

        // Yazar yapýldýysa ona özel bir cümle
        var writerNote = addedRoles.Contains(Roles.Writer)
            ? "<p>Tebrikler! Artýk ZenBlog'da yazý yayýnlayabilirsin. Yazar paneline giriþ yaptýktan sonra ulaþabilirsin.</p>"
            : "";

        var body = $"""
            <p>Hesabýndaki yetkiler bir yönetici tarafýndan güncellendi.</p>
            {added}
            {removed}
            {writerNote}
            <p>Güvenliðin için açýk oturumlarýn kapatýldý. Yeni yetkilerinle devam etmek için tekrar giriþ yapman yeterli.</p>
            """;

        return SendAsync(user, "ZenBlog - Rollerin güncellendi", "Rollerin güncellendi", body);
    }



    // YENÝ: YAZARLIK BAÞVURUSU MAÝLLERÝ
    public Task SendWriterApplicationReceivedAsync(AppUser user)
    {
        var body = """
            <p>Yazarlýk baþvurunu aldýk, teþekkür ederiz.</p>
            <p>Ekibimiz baþvurunu inceleyecek ve sonucu sana e-posta ile bildireceðiz.
               Baþvurunun durumunu panelindeki "Yazar ol" sayfasýndan da takip edebilirsin.</p>
            """;

        return SendAsync(user, "ZenBlog - Yazarlýk baþvurun alýndý", "Baþvurun alýndý", body);
    }

    public Task SendWriterApplicationApprovedAsync(AppUser user)
    {
        var body = """
            <p><strong>Tebrikler!</strong> Yazarlýk baþvurun onaylandý, artýk bir ZenBlog yazarýsýn.</p>
            <p>Güvenliðin için açýk oturumlarýn kapatýldý. Tekrar giriþ yaptýðýnda doðrudan
               yazar paneline yönlendirileceksin ve ilk yazýný yayýnlayabileceksin.</p>
            <p>Seni aramýzda görmekten mutluluk duyuyoruz.</p>
            """;

        return SendAsync(user, "ZenBlog - Artýk yazarsýn!", "Baþvurun onaylandý", body);
    }

    public Task SendWriterApplicationRejectedAsync(AppUser user, string reason, DateTime reapplyAt)
    {
        var body = $"""
            <p>Yazarlýk baþvurunu dikkatle inceledik; ancak bu kez olumlu sonuçlandýramadýk.</p>
            <p><strong>Deðerlendirme notu:</strong></p>
            <blockquote style="margin:0 0 16px;padding:12px 16px;border-left:4px solid #d97706;background:#fffbeb;color:#78350f;">
                {WebUtility.HtmlEncode(reason)}
            </blockquote>
            <p><strong>{FormatDate(reapplyAt)}</strong> tarihinden sonra yeniden baþvurabilirsin.
               Bu süreyi okumak, yorum yapmak ve kendini geliþtirmek için deðerlendirebilirsin.</p>
            """;

        return SendAsync(user, "ZenBlog - Yazarlýk baþvurun hakkýnda", "Baþvurun deðerlendirildi", body);
    }





    private static string FormatDate(DateTime date) => date.ToString("d MMMM yyyy", Turkish);
}