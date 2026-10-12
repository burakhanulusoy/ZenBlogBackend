using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZenBlog.Application.Assembly;
using ZenBlog.Application.Behaviors;

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


                //behavior
                options.AddOpenBehavior(typeof(ValidationBehavior<,>));


            });

            //AutoMapper
            services.AddAutoMapper(typeof(ApplicationAssembly).Assembly);
            //(Assembly.GetExecutingAssembly());  de olabilir bu  assembly ýcýnde caýýscak dýye

            //FlunetValidation

            //FluentValidation
            services.AddValidatorsFromAssembly(typeof(ApplicationAssembly).Assembly);

            return services;
        }
    }
}