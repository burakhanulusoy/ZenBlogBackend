using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Domain.Entities;
using ZenBlog.Persistence.Concrete;
using ZenBlog.Persistence.Context;
using ZenBlog.Persistence.IdentityValidations;
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
                options.AddInterceptors(new AuditDbContextInterceptor());

                //lazy loading
                options.UseLazyLoadingProxies();

            });

            //SCOPED

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IRepository<>),typeof(GenericRepository<>));

            //Identity
            //identity settings
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;//en az 1 farklý karakter
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); // Hesap kilitlendiðinde 15 dakika kapalý kalýr.
                options.Lockout.MaxFailedAccessAttempts = 5; // 5 kez yanlýþ girerse kilitlenir.
                options.Lockout.AllowedForNewUsers = true; // Yeni kullanýcýlar için de geçerli olsun.

            }).AddEntityFrameworkStores<AppDbContext>()
            .AddErrorDescriber<CustomErrorDescriber>();



            return services;
        }




    }
}
