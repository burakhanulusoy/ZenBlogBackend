namespace ZenBlog.Application.Features.Results.UserResults;

// Frontend token'ý saklar; ExpiresAt ile ne zaman yeniden giriþ gerektiðini bilir.
// YENÝ: AccountReactivated ? true ise frontend "Tekrar hoþ geldin, hesap kapatma iþlemin iptal edildi" der.
// Varsayýlan deðeri false olduðu için JwtService'teki new LoginCommandResult(token, expiresAt) aynen çalýþýr.
public record LoginCommandResult(string Token, DateTime ExpiresAt, bool AccountReactivated = false);