using Elastic.Ingest.Elasticsearch;                  // BootstrapMethod
using Elastic.Ingest.Elasticsearch.DataStreams;      // DataStreamName
using Elastic.Serilog.Sinks;                         // WriteTo.Elasticsearch
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;                                // LogEventLevel
using System.Security.Claims;                        // ClaimTypes
using System.Threading.RateLimiting;
using ZenBlog.API.CustomMiddlewares;
using ZenBlog.API.Registrations;
using ZenBlog.Application.Base;
using ZenBlog.Application.Extensions;
using ZenBlog.Infrastructure.Extensions;
using ZenBlog.Persistence.Extensions;

// =====================================================================
// 0) SERILOG'UN KENDÝ HATALARI
// Serilog'un iç sorunlarý (ör. Elasticsearch'e baðlanamadý) normalde sessizce yutulur.
// Bu satýr onlarý konsola yazar; "loglar neden Kibana'ya gitmiyor?" sorusunun cevabý burada çýkar.
// =====================================================================
Serilog.Debugging.SelfLog.Enable(Console.Error);

// =====================================================================
// 1) BAÞLANGIÇ LOGGER'I
// Uygulama ayaða kalkarken (ayarlar okunmadan, servisler kurulmadan önce) bir hata olursa
// bile konsola yazýlabilsin diye geçici, basit bir logger. Asýl logger aþaðýda kurulunca
// bunun yerini alýr.
// =====================================================================
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

// Bütün uygulama try içinde: baþlarken çökerse bile hatayý loglayabilelim
try
{
    Log.Information("ZenBlog API baþlatýlýyor...");

    var builder = WebApplication.CreateBuilder(args);

    // =================================================================
    // 2) ASIL LOGGER (Serilog)
    // .NET'in kendi loglama sistemini Serilog'a baðlar. Kodda ILogger<T> ile yazýlan
    // her þey (ve .NET'in kendi loglarý) artýk Serilog'dan geçer.
    // =================================================================
    builder.Host.UseSerilog((context, services, config) =>
    {
        config
            // appsettings / secrets'taki "Serilog" bölümünden seviyeleri oku
            .ReadFrom.Configuration(context.Configuration)
            // DI'daki servislere eriþebilsin (ileride gerekirse)
            .ReadFrom.Services(services)

            // ZENGÝNLEÞTÝRÝCÝLER: her loga otomatik eklenen bilgiler
            .Enrich.FromLogContext()                                // isteðe özel bilgiler (TraceId vb.)
            .Enrich.WithMachineName()                               // hangi makine / kapsayýcý
            .Enrich.WithProperty("Application", "ZenBlog.API")     // hangi uygulama

            // HEDEF 1: konsol (geliþtirirken anlýk görmek için)
            .WriteTo.Console();

        // HEDEF 2: Elasticsearch (kalýcý saklama + Kibana'da arama)
        // Adres ayarlarda YOKSA çökme; sadece konsola yazmaya devam et.
        var elasticUrl = context.Configuration["Elasticsearch:Url"];
        if (string.IsNullOrWhiteSpace(elasticUrl))
            return;

        config.WriteTo.Elasticsearch(
            new[] { new Uri(elasticUrl) },
            options =>
            {
                // Loglar "logs-zenblog-development" adlý VERÝ AKIÞINA (data stream) gider.
                // Data stream: Elasticsearch'te zamana göre büyüyen veriler (log gibi) için
                // özel yapý. Kibana'da "logs-zenblog-*" diye bulacaðýz.
                options.DataStream = new DataStreamName(
                    "logs",                                                         // tür
                    "zenblog",                                                      // uygulama
                    context.HostingEnvironment.EnvironmentName.ToLowerInvariant()); // ortam

                // Þablonlarý oluþturmayý dener; baþaramazsa API ÇÖKMESÝN.
                // Kural: loglama sistemi asla uygulamayý durdurmamalý.
                options.BootstrapMethod = BootstrapMethod.Silent;
            });
    });

    // SIRA ÖNEMLÝ: Persistence (AddIdentity) önce, Infrastructure (JWT) sonra;
    // böylece JWT ayarlarý Identity'nin çerez varsayýlanlarýnýn üzerine yazýlýr.
    builder.Services.AddPersitenceServices(builder.Configuration)
                    .AddApplicationServices(builder.Configuration)
                    .AddInfrastructureServices(builder.Configuration);

    builder.Services.AddOpenApi();

    // Angular (localhost:4200) API'ye istek atabilsin
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AngularClient", policy =>
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials());
    });

    builder.Services.AddRateLimiter(options =>
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        options.OnRejected = async (context, cancellationToken) =>
        {
            // YENÝ: Hýz sýnýrý aþýldý -> spam ya da kaba kuvvet saldýrýsý belirtisi olabilir.
            // Middleware/servis dýþýnda olduðumuz için logger'ý DI'dan isimle oluþturuyoruz.
            var logger = context.HttpContext.RequestServices
                                .GetRequiredService<ILoggerFactory>()
                                .CreateLogger("RateLimiting");

            logger.LogWarning("{EventType}: Hýz sýnýrý aþýldý. Uç: {RequestPath}",
                              ZenBlog.Application.Constants.LogEvents.RateLimited,
                              context.HttpContext.Request.Path.Value);

            await context.HttpContext.Response.WriteAsJsonAsync(
                BaseResult<object>.Fail("Çok fazla deneme yaptýnýz. Lütfen biraz bekleyip tekrar deneyin."),
                cancellationToken);
        };

        // Doðrulama kodu: IP baþýna dakikada 5
        options.AddPolicy("email-verification", httpContext =>
            RateLimitPartition.GetFixedWindowLimiter(
                httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                _ => new FixedWindowRateLimiterOptions { PermitLimit = 5, Window = TimeSpan.FromMinutes(1) }));

        // Login: IP baþýna dakikada 5 (brute force / credential stuffing'e karþý)
        options.AddPolicy("login", httpContext =>
            RateLimitPartition.GetFixedWindowLimiter(
                httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                _ => new FixedWindowRateLimiterOptions { PermitLimit = 5, Window = TimeSpan.FromMinutes(1) }));

        // Ýletiþim formu: IP baþýna 10 dakikada 3
        options.AddPolicy("contact-message", httpContext =>
            RateLimitPartition.GetFixedWindowLimiter(
                httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                _ => new FixedWindowRateLimiterOptions { PermitLimit = 3, Window = TimeSpan.FromMinutes(10) }));
    });

    var app = builder.Build();

    // Hata middleware'i EN BAÞTA: sonraki her adýmýn hatasýný yakalayabilsin
    app.UseMiddleware<CustomExceptionHandlingMiddleware>();

    // =================================================================
    // 3) ÝSTEK LOGLAMASI
    // Her HTTP isteði bittiðinde TEK satýr log: yöntem, adres, durum kodu, süre, kullanýcý.
    // Hata middleware'inden SONRA gelsin ki hatalý isteklerin 500 durum kodunu da görebilsin.
    // =================================================================
    app.UseSerilogRequestLogging(options =>
    {
        // Log satýrýnýn biçimi. {...} içindekiler metin deðil, aranabilir ALANLAR olarak saklanýr.
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} -> {StatusCode} ({Elapsed:0} ms)";

        // Seviyeyi duruma göre seç:
        //   hata ya da 5xx            -> Error       (sunucu tarafýnda bir þey bozuldu)
        //   4xx ya da 1 sn'den yavaþ  -> Warning     (dikkat edilmeli)
        //   diðerleri                 -> Information
        options.GetLevel = (http, elapsedMs, exception) =>
            exception is not null || http.Response.StatusCode >= 500 ? LogEventLevel.Error
            : http.Response.StatusCode >= 400 || elapsedMs > 1000 ? LogEventLevel.Warning
            : LogEventLevel.Information;

        // Her isteðe KÝMÝN yaptýðýný ekle: sadece kullanýcý Id'si.
        // E-posta, IP gibi kiþisel veriler LOGLANMAZ (KVKK).
        options.EnrichDiagnosticContext = (diagnostic, http) =>
        {
            diagnostic.Set("UserId", http.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonim");
        };
    });

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
       
    }

    app.UseHttpsRedirection();
    app.UseCors("AngularClient");
    app.UseRateLimiter();
    app.UseAuthentication();   // 1) Token'ý oku ve doðrula: "bu kiþi KÝM?"
    app.UseAuthorization();    // 2) Kurallara bak: "bu kiþi buna YETKÝLÝ mi?"

    // Varsayýlan olarak her þey korumalý; herkese açýk endpoint'ler AllowAnonymous ile açýlýr.
    // ("Güvenli varsayýlan" yaklaþýmý: yeni bir endpoint eklemeyi unutsan bile korumasýz kalmaz.)
    app.MapGroup("/api")
       .RequireAuthorization()
       .RegisterEndpoints();

    app.Run();
}
// Uygulama baþlarken çökerse (ör. veritabanýna baðlanamadý) bunu Fatal olarak logla.
// HostAbortedException: EF Core migration araçlarý uygulamayý BÝLEREK durdurur; hata sayýlmasýn.
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "ZenBlog API baþlatýlamadý");
}
finally
{
    // Serilog loglarý performans için toplu halde, arka planda gönderir.
    // Uygulama kapanýrken bekleyen loglarýn hepsi gönderilsin (yoksa son loglar kaybolabilir).
    Log.CloseAndFlush();
}