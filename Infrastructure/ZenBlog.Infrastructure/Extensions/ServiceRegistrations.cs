using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Infrastructure.Services.Azure;
using ZenBlog.Infrastructure.Services.CurrentUser;
using ZenBlog.Infrastructure.Services.EmailService;
using ZenBlog.Infrastructure.Services.GoogleAuth;
using ZenBlog.Infrastructure.Services.Jwt;
using ZenBlog.Infrastructure.Services.Turnstile;

namespace ZenBlog.Infrastructure.Extensions;

public static class ServiceRegistrations
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
                                                               IConfiguration configuration)
    {
        // E-posta
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.AddScoped<IEmailService, EmailService>();

        // Dosya depolama: Azure Blob Storage
        services.Configure<AzureBlobStorageSettings>(configuration.GetSection("AzureBlobStorage"));
        services.AddSingleton(_ => new BlobServiceClient(configuration["AzureBlobStorage:ConnectionString"]));
        services.AddScoped<IFileService, AzureBlobFileService>();

        // ---------------------------------------------------------------
        // JWT
        // ---------------------------------------------------------------
        // Token ÜRETÝMÝ için: JwtService IOptions<JwtTokenOptions> ile ayarlarý okur
        services.Configure<JwtTokenOptions>(configuration.GetSection(nameof(JwtTokenOptions)));
        services.AddScoped<IJwtService, JwtService>();

        // Token DOÐRULAMASI için ayarlarý burada da okuyoruz.
        // Ayar yoksa uygulama açýlýrken net bir hatayla dursun; ilk istekte gizemli hatalar vermesin.
        var jwtOptions = configuration.GetSection(nameof(JwtTokenOptions)).Get<JwtTokenOptions>()
            ?? throw new InvalidOperationException("JwtTokenOptions ayarlarý bulunamadý (secrets.json'u kontrol edin).");

        // AddIdentity (Persistence'ta) varsayýlan olarak ÇEREZ (cookie) tabanlý giriþ kurar.
        // Burada varsayýlanlarý JWT'ye çeviriyoruz. Persistence kaydý Program.cs'te bundan ÖNCE
        // çaðrýldýðý için bizim ayarlarýmýz sonra gelip geçerli oluyor; sýra önemli.
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(opt =>
        {
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,               // token bizim API'miz tarafýndan mý yayýnlanmýþ?
                ValidateAudience = true,             // token bizim frontend için mi üretilmiþ?
                ValidateIssuerSigningKey = true,     // imza bizim Key'imizle mi atýlmýþ?
                ValidateLifetime = true,             // süresi dolmuþ mu?
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                ClockSkew = TimeSpan.Zero            // süre dolunca 5 dk tolerans yok, anýnda geçersiz
            };

            // 401 ve 403 cevaplarýný da diðer hatalar gibi BaseResult formatýnda dön
            opt.Events = new JwtBearerEvents
            {
                // Token yok, geçersiz ya da süresi dolmuþ ? 401
                OnChallenge = async context =>
                {
                    context.HandleResponse();   // ASP.NET Core'un varsayýlan boþ cevabýný engelle
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(
                        BaseResult<object>.Fail("Bu iþlem için giriþ yapmalýsýnýz."));
                },

                // Token geçerli ama rol yetmiyor (örn. User, Admin endpoint'ine girmeye çalýþtý) ? 403
                OnForbidden = async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsJsonAsync(
                        BaseResult<object>.Fail("Bu iþlem için yetkiniz yok."));
                }
            };
        });


        // Yetkilendirme kurallarý (policy). Ýsim verilmiþ bir kural, endpoint'lerde adýyla kullanýlýr.  
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole(Roles.Admin));
        });


        // HttpContext'e servislerden eriþebilmek için
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();


        // Google ile giriþ: token doðrulama
        services.Configure<GoogleAuthSettings>(configuration.GetSection("GoogleAuth"));
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();


        // Cloudflare Turnstile: bot korumasý
        services.Configure<TurnstileSettings>(configuration.GetSection("Turnstile"));

        // AddHttpClient: TurnstileService'e hazýr, doðru yönetilen bir HttpClient verir
        services.AddHttpClient<ITurnstileService, TurnstileService>();

        return services;
    }
}