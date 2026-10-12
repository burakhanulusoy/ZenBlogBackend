using FluentValidation;
using ZenBlog.Application.Base;

namespace ZenBlog.API.CustomMiddlewares
{
    // Her HTTP isteği bu middleware'den geçer.
    // next: pipeline'daki bir sonraki adım (en sonunda endpoint'e ulaşır).
    public class CustomExceptionHandlingMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // İsteği normal şekilde ilerlet. Endpoint → MediatR → Behavior → Handler
                // zincirinin herhangi bir yerinde exception fırlarsa aşağıda yakalanır.
                await next(context);
            }
            catch (ValidationException ex)
            {
                // ValidationBehavior'ın fırlattığı hata buraya düşer.
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";

                // Hataları BaseResult formatına çevir; böylece frontend
                // başarılı ve başarısız cevaplarda AYNI yapıyı görür.
                var response = new BaseResult<object>
                {
                    Errors = ex.Errors
                        .GroupBy(e => e.PropertyName)      // Aynı alana ait hataları grupla
                        .Select(g => new Error
                        {
                            PropertyName = g.Key,
                            ErrorMessage = g.First().ErrorMessage  // Her alan için ilk hatayı göster
                        }).ToList()
                };

                await context.Response.WriteAsJsonAsync(response);
            }
            catch (Exception ex)
            {
                // Beklenmeyen tüm hatalar (DB bağlantısı koptu vs.) 500 olarak döner.
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new { ErrorMessage = ex.Message });
            }
        }
    }
}