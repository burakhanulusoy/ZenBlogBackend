using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZenBlog.Application.Assembly;

namespace ZenBlog.Application.Extensions
{
    public static class ServiceRegistrations
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddMediatR(options =>
            {
                options.RegisterServicesFromAssemblies(typeof(ApplicationAssembly).Assembly);
            });

            return services;
        }
    }
}