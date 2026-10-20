namespace ZenBlog.Application.Contracts.Infrastructure;

// Google token'ından okunan, bizi ilgilendiren bilgiler
public record GoogleUserInfo(string Subject,        // Google'daki benzersiz kullanıcı Id'si
                             string Email,
                             bool EmailVerified,     // Google bu e-postayı doğrulamış mı?
                             string? FirstName,
                             string? LastName,
                             string? PictureUrl);

// Application "bu Google token'ı geçerli mi?" diye sorar;
// nasıl doğrulandığını Infrastructure bilir (e-posta ve dosya servislerindeki yapının aynısı).
public interface IGoogleAuthService
{
    // Token geçerliyse kullanıcı bilgilerini, değilse null döner
    Task<GoogleUserInfo?> ValidateIdTokenAsync(string idToken);
}