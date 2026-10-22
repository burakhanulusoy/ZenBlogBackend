namespace ZenBlog.Application.Features.Results.UserResults;

// Frontend'deki "Hesabýn þu tarihte kapanacak" ekraný için
public record CloseMyAccountResult(DateTime DeletionDeadline);