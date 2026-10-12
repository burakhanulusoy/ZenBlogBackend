using FluentValidation;
using MediatR;

namespace ZenBlog.Application.Behaviors
{
    // Generic yazýldýðý için TÜM request'ler için çalýþýr.
    // TRequest: gelen istek (örn. CreateCategoryCommand)
    // TResponse: handler'ýn döneceði tip (örn. BaseResult<object>)
    //
    // IEnumerable<IValidator<TRequest>>: DI, bu request için kayýtlý bütün validator'larý
    // liste halinde verir. CreateCategoryCommand için 1 tane gelir,
    // GetCategoriesQuery gibi validator'ý olmayan istekler için boþ liste gelir.
    public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
        : IPipelineBehavior<TRequest, TResponse> where TRequest : class
    {
        public async Task<TResponse> Handle(TRequest request,
                                            RequestHandlerDelegate<TResponse> next,
                                            CancellationToken cancellationToken)
        {
            // Bu request'in validator'ý yoksa (örn. Get sorgularý) hiçbir þey yapmadan geç.
            if (validators.Any())
            {
                // FluentValidation'ýn doðrulama yaparken kullandýðý baðlam nesnesi.
                var context = new ValidationContext<TRequest>(request);

                // Birden fazla validator varsa hepsini ayný anda (paralel) çalýþtýr.
                var validationResults = await Task.WhenAll(
                    validators.Select(v => v.ValidateAsync(context, cancellationToken)));

                // Tüm validator'larýn hatalarýný tek bir listede topla.
                var failures = validationResults
                    .SelectMany(r => r.Errors)
                    .ToList();

                // Hata varsa handler'a HÝÇ gitme, exception fýrlat.
                // Bu exception'ý middleware yakalayýp 400 BadRequest'e çevirecek.
                if (failures.Any())
                {
                    throw new ValidationException(failures);
                }
            }

            // Doðrulama geçti ? sýradaki adýma (baþka behavior ya da handler) devam et.
            return await next(cancellationToken);
        }
    }
}