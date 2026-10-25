namespace ZenBlog.Application.Base;

// BaseResult<T>'nin generic olmayan "yüzü".
// LoggingBehavior gibi generic kodlar, T ne olursa olsun (BaseResult<object>, BaseResult<List<...>>)
// sonucun başarılı olup olmadığına bu arayüz üzerinden bakabilsin diye.
public interface IBaseResult
{
    bool IsSuccessful { get; }
    IReadOnlyList<Error> Errors { get; }
}