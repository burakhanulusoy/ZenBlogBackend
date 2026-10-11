using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Persistence.Concrete;
using ZenBlog.Persistence.Context;
using ZenBlog.Persistence.Interceptors;

namespace ZenBlog.Persistence.Extensions
{
    public static class ServiceRegistrations
    {

        public static IServiceCollection AddPersitenceServices(this IServiceCollection services,IConfiguration _configuration)
        {
            //Database & Interceptor

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(_configuration.GetConnectionString("MSSQL_Connection"));
                options.AddInterceptors(new AddDbContextInterceptor());
            });

            //SCOPED

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IRepository<>),typeof(GenericRepository<>));





            return services;
        }




    }
}
