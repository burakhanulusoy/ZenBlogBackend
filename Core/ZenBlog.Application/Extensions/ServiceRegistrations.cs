using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZenBlog.Application.Assembly;
using ZenBlog.Application.Behaviors;
using ZenBlog.Application.Services.AccountServices;
using ZenBlog.Application.Services.MailServices;

namespace ZenBlog.Application.Extensions
{
    public static class ServiceRegistrations
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,IConfiguration configuration)
        {

            //MEDÝATr  and behavior
            services.AddMediatR(options =>
            {
                options.RegisterServicesFromAssemblies(typeof(ApplicationAssembly).Assembly);

                // BEHAVIOR SIRASI = SOÐANIN KATMANLARI (ilk eklenen en DIÞTA)
                // LoggingBehavior en dýþta: doðrulama + handler'ýn toplam süresini ölçsün
                // ve doðrulama hatalarýný da görsün.
                options.AddOpenBehavior(typeof(LoggingBehavior<,>));      // YENÝ
                options.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            //AutoMapper
            services.AddAutoMapper(typeof(ApplicationAssembly).Assembly);
            //(Assembly.GetExecutingAssembly());  de olabilir bu  assembly ýcýnde caýýscak dýye

            //FlunetValidation

            //FluentValidation
            services.AddValidatorsFromAssembly(typeof(ApplicationAssembly).Assembly);


            services.AddScoped<IEmailConfirmationService,EmailConfirmationService>();

            services.AddScoped<IAccountLoginGuard, AccountLoginGuard>();   // YENÝ: giriþte hesap durumu kontrolü

            services.AddScoped<IAccountEmailService, AccountEmailService>();   // YENÝ: hesap durumu mailleri

            return services;
        }
    }
}