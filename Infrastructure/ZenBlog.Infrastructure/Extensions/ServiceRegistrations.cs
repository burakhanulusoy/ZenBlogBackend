using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Domain.Entities;
using ZenBlog.Domain.Enums;
using ZenBlog.Infrastructure.Services.Azure;
using ZenBlog.Infrastructure.Services.BackgroundJobs;
using ZenBlog.Infrastructure.Services.CurrentUser;
using ZenBlog.Infrastructure.Services.EmailService;
using ZenBlog.Infrastructure.Services.GoogleAuth;
using ZenBlog.Infrastructure.Services.Jwt;
using ZenBlog.Infrastructure.Services.Logging;
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
        // Token ÜRETİMİ için: JwtService IOptions<JwtTokenOptions> ile ayarları okur
        services.Configure<JwtTokenOptions>(configuration.GetSection(nameof(JwtTokenOptions)));
        services.AddScoped<IJwtService, JwtService>();

        // Token DOĞRULAMASI için ayarları burada da okuyoruz.
        // Ayar yoksa uygulama açılırken net bir hatayla dursun; ilk istekte gizemli hatalar vermesin.
        var jwtOptions = configuration.GetSection(nameof(JwtTokenOptions)).Get<JwtTokenOptions>()
            ?? throw new InvalidOperationException("JwtTokenOptions ayarları bulunamadı (secrets.json'u kontrol edin).");

        // AddIdentity (Persistence'ta) varsayılan olarak ÇEREZ (cookie) tabanlı giriş kurar.
        // Burada varsayılanları JWT'ye çeviriyoruz. Persistence kaydı Program.cs'te bundan ÖNCE
        // çağrıldığı için bizim ayarlarımız sonra gelip geçerli oluyor; sıra önemli.
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
                ValidateIssuer = true,               // token bizim API'miz tarafından mı yayınlanmış?
                ValidateAudience = true,             // token bizim frontend için mi üretilmiş?
                ValidateIssuerSigningKey = true,     // imza bizim Key'imizle mi atılmış?
                ValidateLifetime = true,             // süresi dolmuş mu?
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                ClockSkew = TimeSpan.Zero            // süre dolunca 5 dk tolerans yok, anında geçersiz
            };

            // 401 ve 403 cevaplarını da diğer hatalar gibi BaseResult formatında dön
            opt.Events = new JwtBearerEvents
            {

                // YENİ: Token imza ve süre olarak geçerli; ama sahibi hâlâ AKTİF mi?
                // Bloke edilen ya da "Hesabımı kapat" diyen kişinin elindeki token süresi dolmasa da
                // bir sonraki istekte reddedilir → 401 → frontend çıkış yaptırır.
                // (Her istekte veritabanına bir hafif sorgu atar; bu proje için sorun değil.)
                // Token imza ve süre olarak geçerli; ama:
                //   1) sahibi hâlâ AKTİF mi? (bloke / hesap kapatma)
                //   2) DEĞİŞTİ: token'daki damga güncel mi? (rol değiştiyse ya da şifre sıfırlandıysa eskidir)
                // İkisinden biri tutmazsa token reddedilir → 401 → frontend çıkış yaptırır.
                OnTokenValidated = async context =>
                {
                    var userManager = context.HttpContext.RequestServices
                        .GetRequiredService<UserManager<AppUser>>();

                    var userId = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier)
                                 ?? context.Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub);

                    var user = userId is null ? null : await userManager.FindByIdAsync(userId);

                    if (user is null || user.AccountStatus != AccountStatus.Active)
                    {
                        context.Fail("Hesap aktif değil.");
                        return;
                    }

                    var tokenStamp = context.Principal?.FindFirstValue("stamp");
                    if (tokenStamp != user.SecurityStamp)
                        context.Fail("Oturum bilgileri değişti, lütfen tekrar giriş yapın.");
                },

                // Token yok, geçersiz ya da süresi dolmuş ? 401
                OnChallenge = async context =>
                {
                    context.HandleResponse();   // ASP.NET Core'un varsayılan boş cevabını engelle
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(
                        BaseResult<object>.Fail("Bu işlem için giriş yapmalısınız."));
                },

                // Token geçerli ama rol yetmiyor (örn. User, Admin endpoint'ine girmeye çalıştı) ? 403
                OnForbidden = async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsJsonAsync(
                        BaseResult<object>.Fail("Bu işlem için yetkiniz yok."));
                }
            };
        });


        // Yetkilendirme kuralları (policy). İsim verilmiş bir kural, endpoint'lerde adıyla kullanılır.  
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.AdminOnly, policy => policy.RequireRole(Roles.Admin));

            // RequireRole birden fazla rol alırsa "bunlardan BİRİ yeterli" demek
            options.AddPolicy(Policies.WriterOrAdmin, policy => policy.RequireRole(Roles.Writer, Roles.Admin));
        });


        // HttpContext'e servislerden erişebilmek için
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();


        // Google ile giriş: token doğrulama
        services.Configure<GoogleAuthSettings>(configuration.GetSection("GoogleAuth"));
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();


        // Cloudflare Turnstile: bot koruması
        services.Configure<TurnstileSettings>(configuration.GetSection("Turnstile"));

        // AddHttpClient: TurnstileService'e hazır, doğru yönetilen bir HttpClient verir
        services.AddHttpClient<ITurnstileService, TurnstileService>();

        // YENİ: Log sorguları (Elasticsearch). Adres Serilog'un kullandığı ayarın aynısı.
        // AddHttpClient: HttpClient'ı doğru yönetir (bağlantı havuzu; her istekte yeni bağlantı açmaz).
        services.AddHttpClient<ILogQueryService, ElasticLogQueryService>(client =>
        {
            var url = configuration["Elasticsearch:Url"] ?? "http://localhost:9200";
            client.BaseAddress = new Uri(url.TrimEnd('/') + "/");
            client.Timeout = TimeSpan.FromSeconds(10);   // Elasticsearch yanıt vermezse sayfa sonsuza kadar beklemesin
        });

        // YENİ: Arka plan görevi: hesap kapanmadan 3 gün önce hatırlatma maili
        services.AddHostedService<AccountDeletionReminderService>();
        return services;
    }
}