namespace ZenBlog.Application.Features.Results.UserResults;

// Frontend token'ý saklar; ExpiresAt ile ne zaman yeniden giriþ gerektiðini bilir
public record LoginCommandResult(string Token, DateTime ExpiresAt);