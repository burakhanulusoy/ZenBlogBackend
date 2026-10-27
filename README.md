<img width="1892" height="1010" alt="Ekran görüntüsü 2026-10-27 171000" src="https://github.com/user-attachments/assets/ec7da7b0-3d6e-4765-bc94-bacf5a332bdb" /><div align="center">

# 🌿 ZenBlog

### Çok Rollü, Güvenlik Odaklı Blog Platformu

![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Minimal API](https://img.shields.io/badge/ASP.NET_Core-Minimal_API-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Angular 20](https://img.shields.io/badge/Angular-20-DD0031?style=for-the-badge&logo=angular&logoColor=white)
![SQL Server](https://img.shields.io/badge/Database-SQL_Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)

![MediatR](https://img.shields.io/badge/Pattern-MediatR_%26_CQRS-7C3AED?style=for-the-badge)
![JWT](https://img.shields.io/badge/Auth-JWT_%2B_Identity-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white)
![Azure Blob](https://img.shields.io/badge/Storage-Azure_Blob-0078D4?style=for-the-badge&logo=microsoftazure&logoColor=white)
![PrimeNG](https://img.shields.io/badge/UI-PrimeNG_20-DD0031?style=for-the-badge)

![Serilog](https://img.shields.io/badge/Logging-Serilog-1E293B?style=for-the-badge)
![Elasticsearch](https://img.shields.io/badge/Search-Elasticsearch_9-005571?style=for-the-badge&logo=elasticsearch&logoColor=white)
![Kibana](https://img.shields.io/badge/Kibana-9-E8478B?style=for-the-badge&logo=kibana&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?style=for-the-badge&logo=docker&logoColor=white)

<p align="center">
  <strong>Okurdan yazara, yazardan yöneticiye: rol bazlı paneller, güvenli hesap yaşam döngüsü ve gözlemlenebilir bir altyapı.</strong>
</p>
<p align="center">
  <i>.NET 9 Minimal API · Clean Architecture · CQRS · JWT + SecurityStamp · Rate Limiting · Cloudflare Turnstile<br>
  Serilog + Elasticsearch + Kibana · Angular 20 + PrimeNG ile üç panel ve ziyaretçi sitesi</i>
</p>

<p align="center">
  <a href="https://github.com/KULLANICI_ADIN/ZenBlogBackend"><b>⚙️ Backend (bu depo)</b></a>
  &nbsp;·&nbsp;
  <a href="https://github.com/KULLANICI_ADIN/ZenBlogFrontend"><b>🎨 Frontend deposu</b></a>
</p>

</div>

---

**ZenBlog**, okurların yazıları okuyup yorum yaptığı, yazarların içerik ürettiği ve yöneticilerin bütün sistemi denetlediği **çok rollü bir blog platformudur**. Proje iki parçadan oluşur: **.NET 9 Minimal API** ile yazılmış, Clean Architecture prensiplerine göre katmanlandırılmış bir backend ve **Angular 20 + PrimeNG** ile geliştirilmiş, üç ayrı panel ve bir ziyaretçi sitesi içeren bir frontend.

Projenin odağında **güvenlik ve hesap yönetimi** vardır. Rol değiştiğinde ya da hesap bloke edildiğinde kişinin oturumu **anında** düşer. Hesap kapatma 30 günlük, geri alınabilir bir süreçtir. Bloke işlemleri gerekçe ve kanıt dosyasıyla kayıt altına alınır. Yazarlık bir başvuru ve değerlendirme sürecinden geçer. İçerik sahipliği backend'de, her istekte denetlenir.

Altyapı tarafında **Serilog ile yapılandırılmış loglama**, MediatR **pipeline behavior**'ları, **Elasticsearch'e ECS formatında** gönderilen loglar, Kibana dashboard'u ve yönetim panelinde **Elasticsearch Query DSL** ile çalışan bir log ekranı bulunur.

---

## 🌟 Temel Modüller ve Yetenekler

### 🔐 1. Kimlik Doğrulama, JWT ve Anında Oturum Geçersizleştirme

Kimlik işlemleri **ASP.NET Core Identity** (Guid anahtarlı `AppUser` / `AppRole`) ile yürütülür. API durumsuzdur (stateless) ve **JWT Bearer** ile korunur.

- **JWT üretimi:** Token **HS256** ile imzalanır. İçinde `sub`, `email`, `jti`, `fullName`, `imageUrl`, roller ve **`stamp`** (SecurityStamp) claim'leri bulunur.
- **Sıkı doğrulama:** Issuer, audience, imza ve süre doğrulanır; `ClockSkew = TimeSpan.Zero` ile süresi dolan token tolerans olmadan reddedilir.
- **Anında oturum düşürme (SecurityStamp):** `OnTokenValidated` olayında **her istekte** kullanıcının hesap durumu (`Active` mi?) ve token'daki damganın veritabanındaki `SecurityStamp` ile eşleşip eşleşmediği kontrol edilir. Rol değişikliği, bloke, hesap kapatma ve yazarlık onayı gibi işlemler damgayı yeniler; böylece kişinin elindeki eski token, süresi dolmasını beklemeden **bir sonraki istekte** geçersiz olur.
- **Standart hata cevapları:** `OnChallenge` (401) ve `OnForbidden` (403) olayları, diğer bütün cevaplar gibi `BaseResult` biçiminde JSON döndürür.
- **E-posta doğrulama:** Kayıttan sonra Identity'nin token sağlayıcısıyla **6 haneli kod** üretilir ve MailKit ile gönderilir. Doğrulanmamış hesap, şifre doğru olsa bile giriş yapamaz.
- **Şifre sıfırlama:** Yine **6 haneli kod** ile; `ResetPasswordAsync` üzerinden.
- **Google ile giriş:** Frontend'den gelen Google **ID token'ı backend'de doğrulanır**. Hesap yoksa oluşturulur (benzersiz kullanıcı adı üretilir), varsa `AspNetUserLogins` ile ilişkilendirilir. **Güvenlik önlemi:** E-postası hiç doğrulanmamış eski bir hesap Google ile açılırsa, o hesabı başka biri açmış olabileceği için eski şifresi kaldırılır.
- **Identity kuralları:** En az 8 karakterli şifre; 5 hatalı denemede 10 dakikalık hesap kilidi; Identity hata mesajları Türkçeleştirilmiş (`CustomErrorDescriber`).
- **Kullanıcı keşfine karşı:** Giriş hatalarında tek tip mesaj ("E-posta veya şifre hatalı") döner; hesabın bloke ya da kapalı olduğu bilgisi **ancak şifre doğrulandıktan sonra** verilir.

### 🛡️ 2. Kötüye Kullanıma Karşı Koruma ve Yetkilendirme

- **Güvenli varsayılan (secure by default):** Bütün uçlar `/api` grubu altında `RequireAuthorization()` ile **varsayılan olarak korumalıdır**. Herkese açık olması gereken uçlar tek tek `AllowAnonymous()` ile açılır; yeni bir uç eklemeyi unutmak onu korumasız bırakmaz.
- **Policy tabanlı yetki:** `AdminOnly` ve `WriterOrAdmin` politikaları.
- **Rate Limiting (`Microsoft.AspNetCore.RateLimiting`):** IP başına **sabit pencere (fixed window)**: giriş 5 istek/dk, doğrulama kodu 5 istek/dk, iletişim formu 3 istek/10 dk. Sınır aşılınca `429` ve `BaseResult` mesajı döner; olay loglanır.
- **Cloudflare Turnstile:** Kayıt, giriş ve iletişim formu uçlarında bir **`IEndpointFilter`** olarak çalışır; `X-Turnstile-Token` başlığı handler'dan önce Cloudflare'e doğrulatılır.
- **Sahiplik kontrolleri (handler seviyesinde):**
  - Blog: Güncellemeyi sadece sahibi; silmeyi sahibi ya da Admin yapabilir.
  - Yorum ve yanıt: Düzenlemeyi sadece sahibi; silmeyi sahibi, **blogun yazarı** ya da Admin yapabilir.
- **Bilgi sızıntısı önleme:** Beklenmeyen hatalarda kullanıcıya teknik ayrıntı değil, genel bir mesaj ve **hata kodu** döner.
- **Diğer önlemler:**
  - Google Haritalar adresi sadece `https://www.google.com/maps/embed` önekiyle kabul edilir (iframe XSS koruması).
  - Maillerde kullanıcı girdileri `HtmlEncode` ile kodlanır.
  - Frontend'de giriş sonrası `returnUrl` yönlendirmesi sadece site içi adreslere izin verir (open redirect koruması).

### ♻️ 3. Hesap Yaşam Döngüsü: Kapatma, Bloke ve Geri Dönüş

Hesap durumu `AccountStatus` ile yönetilir: `Active`, `PendingDeletion`, `Blocked`.

- **Hesabımı kapat:** Şifre onayıyla başlar ve **30 günlük** bir süreç açılır. Kullanıcının blogları, yorumları ve yanıtları `HiddenByAccount` işaretiyle topluca gizlenir.
- **Hatırlatma:** Bir **`BackgroundService`** 6 saatte bir çalışır; süreye **3 gün kalan** kullanıcılara hatırlatma maili gönderir (her döngüde yeni bir DI scope açar).
- **Geri dönüş:** 30 gün dolmadan giriş yapan kullanıcının kapatma işlemi otomatik iptal edilir, içerikleri geri açılır, frontend'de "Tekrar hoş geldin" bildirimi gösterilir. Süre dolduysa hesap kalıcı olarak kapanır.
- **Bloke:**
  - Admin, en az 20 karakterlik **gerekçe**, **kanıt dosyası** (en fazla 10 MB; jpg, png, webp, pdf) ve onay kutusu ile bloke eder.
  - Kullanıcıya mail gider, içerikleri gizlenir, oturumu düşer.
  - Kayıt `UserBlock` tablosunda saklanır; bloke kaldırılırken de gerekçe istenir ve geçmiş silinmez.
- **Kurallar:** Admin hesabını bu yolla kapatamaz; başka bir admin bloke edilemez; sistemde **en az bir yönetici** kalmak zorundadır.

### 👑 4. Rol Yönetimi ve Yazarlık Başvurusu

- **Rol atama:**
  - `User` rolü temel roldür ve alınamaz.
  - Admin kendi yönetici rolünü kaldıramaz.
  - **Admin rolü verilirken işlemi yapan yöneticinin şifresi** istenir (hatalı denemeler kilitlemeye dahildir).
- **Rol geçmişi:** Her değişiklik `RoleChangeLog` tablosuna "kim, kime, ne zaman, hangi rol" bilgisiyle yazılır; kullanıcıya bilgilendirme maili gider ve oturumu düşer.
- **Yazarlık başvurusu:** Okur; 5 konu seçimi, en az 50 karakterlik motivasyon, deneyim, örnek bağlantılar ve **8 maddelik yazar taahhütnamesi** ile başvurur.
- **Değerlendirme:**
  - Başvuru durumları: `Pending` → `Reviewed` → `Approved` / `Rejected`. Admin başvuruyu açınca "okundu" olur.
  - Onaylanırsa `Writer` rolü verilir, tebrik maili gider, oturum düşer; yeniden girişte kişi doğrudan yazar paneline yönlenir.
  - Reddedilirse gerekçe mail ile iletilir ve **30 gün** sonra yeniden başvurulabilir.

### 📝 5. Blog, Yorumlar ve Okunma Sayacı

- **Blog yönetimi:** Kapak dahil üç görsel ve bölümlü içerik; `multipart/form-data` ile yüklenir. Görseller **Azure Blob Storage**'a `klasör/yıl/ay/guid.uzantı` düzeniyle kaydedilir ve tarayıcı önbelleği için `Cache-Control: public, max-age=31536000, immutable` başlığı eklenir.
- **Yorum ve yanıtlar:** Tek seviyeli yanıt yapısı; bir yanıta yanıt verildiğinde aynı yorumun altına "@Ad" ile eklenir. Düzenlenen yorumlar "(düzenlendi)" olarak işaretlenir. Bir yorum silinince yanıtları da birlikte gizlenir.
- **Panel yorum sayfaları:** "Yorumlarım", "Bloglarıma gelen yorumlar" ve (admin için) "Tüm yorumlar"; yorumlar ve yanıtlar ortak bir biçimde birleştirilir.
- **Günlük tekil okunma sayacı:**
  - Aynı ziyaretçi, aynı blogu aynı gün içinde kaç kez açarsa açsın **bir kez** sayılır; bu, `(BlogId, VisitorKey, Gün)` üzerindeki **benzersiz index** ile veritabanı seviyesinde garanti edilir.
  - Ziyaretçi kimliği (giriş yapmışsa kullanıcı Id'si, değilse IP + tarayıcı bilgisi) **SHA-256 ile özetlenerek** saklanır; IP adresi veritabanına yazılmaz.
  - Botlar ve yazarın kendi blogunu açması sayılmaz.
  - Toplam, `ExecuteUpdateAsync` ile tek bir atomik SQL güncellemesiyle artırılır (`UPDATE ... SET ViewCount = ViewCount + 1`).

### 🌐 6. Ziyaretçi Sitesi

- **Ana sayfa:** Slider (son 3), son yazılar (7), "Trending" (en çok yorum alan 5) ve her kategori için son 5 yazı. Hepsi **tek istekte** (`GET /api/home`) gelir.
- **Blog detayı:** Okuma ilerleme çubuğu, tahmini okuma süresi, yazar kutusu, paylaşım bağlantıları ve **ilgili yazılar** (aynı kategoriden; yetmezse en yeni yazılarla tamamlanır).
- **Kategori sayfası:** Veritabanında sayfalama (`Skip/Take`); sayfa numarası adreste tutulur, yenilenince ve paylaşılınca aynı sayfa açılır.
- **Hakkımızda, Ekip ve İletişim:**
  - Hakkımızda'da en son eklenen içerik yayında olur; eski sürümler saklanır.
  - Ekip üyelerinin sosyal hesapları sabit bir platform listesinden seçilir.
  - İletişim formu Turnstile ve hız sınırıyla korunur.

### 📊 7. Paneller ve Dashboard'lar

- **Admin genel bakış:**
  - Dönem seçici (7 gün / 30 gün / 12 ay) ve **bir önceki aynı uzunluktaki döneme göre değişim**.
  - Okunma, blog, yorum ve yeni üye trendi (çift eksenli grafik).
  - "Seni bekleyenler": başvurular, okunmamış mesajlar, kapanan ve bloke hesaplar.
  - Kategori ve rol dağılımı, en çok okunanlar, en üretken yazarlar, son yorumlar ve yeni üyeler.
- **Yazar genel bakış:** Piksel (8-bit) tasarım; yazar seviyesi sistemi (Çaylak → Kalem → Usta → Efsane), okunma ve yorum grafiği, en çok okunan yazılar, gelen yorumlar.
- **İstatistik hesaplaması:** Her ölçü veritabanında **gün gün gruplanarak** (`GROUP BY`) sayılır; 12 aylık dönemde bile yalnızca küçük bir özet belleğe alınır, grafik noktaları ve dönem karşılaştırmaları bu özetten hesaplanır.
- **Site içeriği yönetimi:** Hakkımızda, Ekip, Sosyal medya, İletişim bilgileri ve **Gelen mesajlar**. Gelen mesajlar e-posta programı düzeninde; açılınca okundu sayılır ve menüde okunmamış rozeti gösterilir.

### 🔭 8. Loglama ve Gözlemlenebilirlik

- **Serilog ile yapılandırılmış loglama:** Loglar düz metin değil, aranabilir alanlar olarak (`{UserId}`, `{StatusCode}`...) yazılır; hem konsola hem **Elasticsearch**'e (ECS formatında, `logs-zenblog-{ortam}` data stream'ine) gönderilir.
- **Loglama asla uygulamayı çökertmez:** Başlangıç logger'ı ve `try/catch/finally` + `CloseAndFlush`; `BootstrapMethod.Silent`; Serilog'un iç hataları için `SelfLog`; Elasticsearch adresi yoksa sadece konsola yazılır.
- **İstek loglaması:** Her HTTP isteği tek satır. 5xx ve hatalar `Error`, 4xx ve 1 saniyeden yavaş istekler `Warning`. İsteği yapan kullanıcının yalnızca Id'si eklenir.
- **`LoggingBehavior` (MediatR):** Her komutun adı, süresi ve sonucu; doğrulama hataları; 500 ms'den yavaş işlemler. `BeginScope` ile `RequestName` ve `UserId`, handler içinde yazılan bütün loglara otomatik eklenir.
- **Önemli olaylar (`EventType`):** Başarılı ve başarısız giriş (sebebiyle birlikte), hesap kilitlenmesi, Turnstile hatası, hız sınırı, bloke ve bloke kaldırma, rol değişikliği, yazarlık kararı, hesap kapatma ve geri dönüş, **e-posta gönderim hataları** ve hatırlatma görevinin sonucu.
- **Global hata yakalama:** Beklenmeyen hatalar yığın iziyle loglanır. Kullanıcıya dönen **hata kodu, isteğin `trace.id`'si** ile aynıdır; o kodla, isteğin bütün logları tek aramada bulunur.
- **Kişisel veri:** Şifre, token, e-posta ve IP adresi **loglanmaz**; kişiler yalnızca Id ile temsil edilir.
- **Kibana:** Discover ve bir log dashboard'u (istek ve hata grafiği, en yavaş uçlar, önemli olaylar, başarısız giriş sebepleri).
- **Admin "Log kayıtları" ekranı:** Backend, Elasticsearch'e `HttpClient` ile doğrudan **Query DSL** sorguları ve **aggregation**'lar gönderir. Ekranda:
  - Özet kartlar, zaman grafiği, olay dağılımı ve en yavaş uçlar.
  - Filtreler: seviye, olay türü, metin, kullanıcı ve tek bir isteğe ait loglar.
  - Sayfalı liste ve logun bütün alanlarını, varsa yığın izini gösteren bir detay paneli. Loglardaki Id'ler tek bir SQL sorgusuyla isimlere çevrilir.
  - Elasticsearch kapalıysa uç `503` döner ve sitenin geri kalanı etkilenmez. Detay ucunda sadece ZenBlog log index'lerinin okunmasına izin verilir.

---

## 🏗️ Mimari ve Tasarım Yaklaşımı

Backend, **Clean Architecture** prensiplerine göre beş projeye ayrılmıştır: `Domain`, `Application`, `Persistence`, `Infrastructure` ve `API`. Bağımlılıklar içe doğrudur; Application katmanı veritabanını, Azure'u ya da Elasticsearch'ü değil, yalnızca arayüzleri bilir.

| Yapı / Konsept | Projedeki Kullanımı |
| :--- | :--- |
| **API** | .NET 9, ASP.NET Core **Minimal API**; uç noktalar `MapGroup` ile modül modül gruplanır |
| **Mimari** | Clean Architecture: Domain · Application · Persistence · Infrastructure · API |
| **Komut ve Sorgu Ayrımı** | **MediatR** ile CQRS: Commands, Queries, Handlers, Results |
| **Pipeline Behavior'lar** | `LoggingBehavior` (en dışta) → `ValidationBehavior` → Handler |
| **Doğrulama** | **FluentValidation**; hatalar middleware'de alan bazlı `400` cevabına çevrilir |
| **Nesne Eşleme** | **AutoMapper**; dosya alanları eşlemeden hariç tutulur |
| **Veri Erişimi** | **EF Core** + **SQL Server**; Generic Repository ve Unit of Work |
| **Okuma Servisleri** | Yoğun okuma ekranları için **projeksiyonla** (`Select`) çalışan servisler: `HomeReadService`, `BlogReadService`, `CommentReadService`, `DashboardReadService`. N+1 ve lazy loading yok |
| **Kayıt Takibi ve Soft Delete** | `AuditDbContextInterceptor` ile zaman damgaları; silme işlemi `IsDeleted`'a çevrilir; global sorgu filtresi |
| **Standart Cevap** | Bütün uçlar `BaseResult<T>` (Data, Errors, IsSuccessful) döndürür; generic olmayan `IBaseResult` arayüzü |
| **Kimlik ve Yetki** | ASP.NET Core Identity (Guid) · JWT Bearer · Policy · SecurityStamp |
| **Arka Plan İşleri** | `BackgroundService` (hesap kapatma hatırlatması) |
| **Dosya Depolama** | Azure Blob Storage (`IFileService`) |
| **E-posta** | MailKit (`IEmailService`) ve ortak HTML mail şablonu (`AccountEmailService`) |
| **Koruma** | Rate Limiting · Cloudflare Turnstile (`IEndpointFilter`) |
| **Loglama** | Serilog · Elastic.Serilog.Sinks (ECS) · Elasticsearch 9 · Kibana 9 |
| **API Dokümantasyonu** | OpenAPI + **Scalar** |
| **Geliştirme Ortamı** | Docker Compose (Elasticsearch + Kibana) |

### ⚡ Minimal API ve Güvenli Varsayılan

Controller yerine **Minimal API** kullanılır. Her modülün uçları kendi dosyasında bir uzantı metodu olarak tanımlanır (`RegisterBlogEndpoints`, `RegisterCommentEndpoint`...) ve `/api` grubuna bağlanır. Grup seviyesinde `RequireAuthorization()` uygulandığı için herkese açık uçlar bilinçli olarak `AllowAnonymous()` ile işaretlenir. Uçlar `Produces<BaseResult<T>>` ile belgelenir ve Scalar'da görüntülenir.

### 🧅 MediatR Pipeline Behavior'ları

Her istek, handler'a ulaşmadan önce bir "soğan" katmanından geçer:

1. **`LoggingBehavior`:** Süreyi ölçer; `BaseResult` başarısızsa hatayı, işlem yavaşsa uyarıyı yazar; doğrulama hatalarını loglar.
2. **`ValidationBehavior`:** İstek için kayıtlı bütün FluentValidation doğrulayıcılarını paralel çalıştırır; hata varsa handler'a hiç gitmeden `ValidationException` fırlatır.
3. **Handler:** İş kuralları ve sahiplik kontrolleri.

Logging en dışta olduğu için doğrulama dahil toplam süreyi ölçer.

### 🔎 Projeksiyonla Okuma ve N+1 Önleme

Repository'nin `AsNoTracking` ile döndüğü nesnelerde lazy loading çalışmadığı ve gereksiz sorgulara yol açtığı için, yoğun ekranlar ayrı **okuma servisleriyle** yazılmıştır. Örneğin ana sayfa verisinin tamamı birkaç projeksiyon sorgusuyla gelir; kategori başına son 5 blog, EF Core'un `ROW_NUMBER() OVER (PARTITION BY ...)` çevirisiyle tek sorguda alınır. Blog kartı dönüşümü bir `Expression<Func<Blog, ...>>` olarak tanımlanır; böylece EF Core bu dönüşümü SQL'e çevirebilir.

---

## 🔄 Mimari ve İş Akışları

Diyagramlar süreçleri kısa akışlar halinde gösterir. Hareketli bağlantılar işlem yönünü vurgular.

<!-- Uyumluluk: Bu diyagramlar Mermaid edge ID ve animation özelliklerini kullanır. GitHub'daki Mermaid sürümü desteklemezse "a1@" gibi önekleri ve animation satırlarını kaldırarak statik görünüm kullanılabilir. -->

### 01 · Katman Mimarisi

```mermaid
flowchart TB
    subgraph CLIENT["İSTEMCİ"]
        UI("Angular 20<br/>Ziyaretçi sitesi · Admin · Yazar · Okur")
    end
    subgraph API["ZenBlog.API"]
        EP("Minimal API Uçları<br/>MapGroup · Policy · Filters")
        MW("Middleware<br/>Hata yakalama · Rate Limiter · JWT")
    end
    subgraph APP["ZenBlog.Application"]
        MED("MediatR<br/>Logging → Validation → Handler")
        CON("Arayüzler<br/>IRepository · IFileService · ILogQueryService")
    end
    subgraph DATA["ZenBlog.Persistence"]
        REPO("Repository · Unit of Work<br/>Okuma servisleri (projeksiyon)")
        DB[("SQL Server")]
    end
    subgraph INFRA["ZenBlog.Infrastructure"]
        EXT("JWT · MailKit · Azure Blob<br/>Google · Turnstile · Elasticsearch")
    end
    DOMAIN("ZenBlog.Domain<br/>Entity'ler · Enum'lar")

    UI a1@--> MW
    MW a2@--> EP
    EP a3@--> MED
    MED a4@--> CON
    CON a5@-.-> REPO
    CON a6@-.-> EXT
    REPO a7@--> DB
    MED a8@-.-> DOMAIN

    class UI entry;
    class EP,MW,MED action;
    class CON,DOMAIN decision;
    class REPO,DB data;
    class EXT success;
    style CLIENT fill:#FAFBFC,stroke:#D8DFE7,color:#334155
    style API fill:#FAFBFC,stroke:#D8DFE7,color:#334155
    style APP fill:#FAFBFC,stroke:#D8DFE7,color:#334155
    style DATA fill:#FAFBFC,stroke:#D8DFE7,color:#334155
    style INFRA fill:#FAFBFC,stroke:#D8DFE7,color:#334155

    classDef entry fill:#F1F5F9,stroke:#64748B,color:#0F172A,stroke-width:1.5px;
    classDef action fill:#FFF7ED,stroke:#C48451,color:#633C24,stroke-width:1.5px;
    classDef data fill:#EFF6FF,stroke:#7095B8,color:#173854,stroke-width:1.5px;
    classDef decision fill:#FEF3C7,stroke:#C79A35,color:#713F12,stroke-width:1.5px;
    classDef success fill:#ECFDF5,stroke:#57A68A,color:#14533E,stroke-width:1.5px;
    classDef failure fill:#FFF1F2,stroke:#CD7E89,color:#831843,stroke-width:1.5px;
    linkStyle default stroke:#8B939F,stroke-width:1.5px;

    a1@{ animation: slow }
    a2@{ animation: slow }
    a3@{ animation: slow }
    a4@{ animation: slow }
    a5@{ animation: slow }
    a6@{ animation: slow }
    a7@{ animation: slow }
    a8@{ animation: slow }
```

Kesikli oklar "arayüz üzerinden" bağımlılığı gösterir: Application katmanı somut sınıfları değil, arayüzleri bilir. Uygulamalar Persistence ve Infrastructure'da yazılır ve DI ile bağlanır.

### 02 · Bir İsteğin Yolculuğu

```mermaid
flowchart TB
    subgraph GATE["KAPIDA"]
        A("İstek<br/>Angular → /api/...")
        B("Hata Middleware'i<br/>en dışta · her hatayı yakalar")
        C("Rate Limiter<br/>IP başına sabit pencere")
        D("JWT Doğrulama<br/>OnTokenValidated: durum + stamp")
        A b1@--> B
        B b2@--> C
        C b3@--> D
    end
    subgraph ENDPOINT["UÇ NOKTA"]
        E("Policy<br/>AdminOnly · WriterOrAdmin")
        F("Turnstile Filtresi<br/>sadece işaretli uçlarda")
        E b5@--> F
    end
    subgraph PIPELINE["MEDIATR"]
        G("LoggingBehavior<br/>süre · sonuç · scope")
        H("ValidationBehavior<br/>FluentValidation")
        I("Handler<br/>iş kuralları · sahiplik")
        G b6@--> H
        H b7@--> I
    end
    J[("SQL Server")]
    K("BaseResult&lt;T&gt;")
    D b4@--> E
    F b8@--> G
    I b9@--> J
    I b10@--> K

    class A entry;
    class B,C,D,E,F action;
    class G,H decision;
    class I,K success;
    class J data;
    style GATE fill:#FAFBFC,stroke:#D8DFE7,color:#334155
    style ENDPOINT fill:#FAFBFC,stroke:#D8DFE7,color:#334155
    style PIPELINE fill:#FAFBFC,stroke:#D8DFE7,color:#334155

    classDef entry fill:#F1F5F9,stroke:#64748B,color:#0F172A,stroke-width:1.5px;
    classDef action fill:#FFF7ED,stroke:#C48451,color:#633C24,stroke-width:1.5px;
    classDef data fill:#EFF6FF,stroke:#7095B8,color:#173854,stroke-width:1.5px;
    classDef decision fill:#FEF3C7,stroke:#C79A35,color:#713F12,stroke-width:1.5px;
    classDef success fill:#ECFDF5,stroke:#57A68A,color:#14533E,stroke-width:1.5px;
    classDef failure fill:#FFF1F2,stroke:#CD7E89,color:#831843,stroke-width:1.5px;
    linkStyle default stroke:#8B939F,stroke-width:1.5px;

    b1@{ animation: slow }
    b2@{ animation: slow }
    b3@{ animation: slow }
    b4@{ animation: slow }
    b5@{ animation: slow }
    b6@{ animation: slow }
    b7@{ animation: slow }
    b8@{ animation: slow }
    b9@{ animation: slow }
    b10@{ animation: slow }
```

Doğrulama hatası `400` ve alan bazlı hatalarla; yetki hatası `401` ya da `403` ile; hız sınırı `429` ile; beklenmeyen hata ise `500`, genel bir mesaj ve hata koduyla döner. Hepsi aynı `BaseResult` biçimindedir.


---

## 👥 Kullanıcı Rolleri ve Yetkiler

| Rol | Yetki ve İşlevler |
| :--- | :--- |
| **Ziyaretçi** | Yazıları okuma, kategori, Hakkımızda ve İletişim sayfaları; iletişim formu; kayıt ve giriş. |
| **User (Okur)** | Yorum ve yanıt yazma, kendi yorumlarını düzenleme ve silme; profil yönetimi; hesabını kapatma; yazarlık başvurusu. |
| **Writer (Yazar)** | Okurun bütün yetkileri + blog yazma, kendi bloglarını düzenleme ve silme, bloglarına gelen yorumları yönetme, yazar dashboard'u. |
| **Admin (Yönetici)** | Bütün bloglar ve yorumlar, kategoriler, kullanıcılar (bloke, rol), yazarlık başvuruları, site içeriği, gelen mesajlar, admin dashboard'u ve log kayıtları. |

Sahiplik kuralları rol kontrolünden ayrı, handler seviyesinde uygulanır: Bir yazar başka bir yazarın blogunu düzenleyemez; bir yönetici de başkasının blogunu ya da yorumunu **düzenleyemez**, yalnızca kaldırabilir.

---

## 🔌 Uç Noktalara Genel Bakış

Bütün uçlar `/api` altındadır ve `BaseResult<T>` döndürür. Ayrıntılı liste, geliştirme ortamında Scalar arayüzünden (`/scalar`) görülebilir.

| Modül | Örnek uçlar | Erişim |
| :--- | :--- | :--- |
| **Hesap** | `POST users/register` · `login` · `google-login` · `confirm-email` · `forgot-password` · `reset-password` | Herkese açık (Turnstile + rate limit) |
| **Profil** | `GET/PUT users/me` · `POST users/me/close` · `users/me/writer-application` | Giriş yapmış |
| **Kullanıcı yönetimi** | `GET users` · `POST users/{id}/block` · `unblock` · `POST roles/assign` | Admin |
| **Yazarlık** | `GET writer-applications` · `POST {id}/approve` · `reject` | Admin |
| **Blog** | `GET blogs/{id}` · `{id}/related` · `POST {id}/view` · `GET blogs/my` · `POST/PUT/DELETE blogs` | Okuma açık · yazma Writer/Admin |
| **Site** | `GET home` · `GET categories/{id}/blogs` · `abouts` · `teams` · `contacts` · `social-medias` | Herkese açık |
| **Yorumlar** | `GET comments/byBlogId/{id}` · `mine` · `on-my-blogs` · `all` · `subcomments` | Rol ve sahipliğe göre |
| **İletişim mesajları** | `POST contactmessages` (açık) · `GET/PUT/DELETE` (Admin) | Karışık |
| **Dashboard** | `GET dashboard/admin` · `dashboard/writer` `?period=7d\|30d\|12m` | Admin · Writer/Admin |
| **Loglar** | `GET logs` · `logs/summary` · `logs/detail` | Admin |

---

## 🎨 Frontend Öne Çıkanlar

Frontend ayrı bir depodadır: **[ZenBlogFrontend](https://github.com/KULLANICI_ADIN/ZenBlogFrontend)**.

- **Angular 20 (NgModule yapısı) + PrimeNG 20**; dört yerleşim: ziyaretçi sitesi, Admin, Yazar ve Okur panelleri.
- **Rol bazlı yönlendirme:** Rotalarda `data: { roles }` ile çalışan `AuthGuard`; yetkisiz erişimde 403 sayfası.
- **Token interceptor:** Her isteğe JWT ekler; `@auth0/angular-jwt` ile süre kontrolü ve rol okuma.
- **Tasarım:**
  - Ziyaretçi sitesinin stilleri `.zb-site` kapsamına alınmıştır; paneller etkilenmez.
  - Yazar ve okur panellerinde `:host-context(.zb-pixel)` ile ortak sayfalar **piksel (8-bit) tasarıma** bürünür.
- **Performans kararları:**
  - Ortak site verisi `shareReplay(1)` ile önbelleğe alınır.
  - Swiper ve otomatik yenileme gibi işler `NgZone.runOutsideAngular` ile çalıştırılır.
  - Listelerde `trackBy` kullanılır; şablonda yeni dizi üreten getter'lardan kaçınılır.
  - Aramalarda `debounceTime` kullanılır.
- **Grafikler:** Chart.js (PrimeNG `p-chart`) ile dashboard ve log grafikleri.

---

## 🔗 Entegrasyonlar

| Entegrasyon | Uygulamaya Katkısı |
| :--- | :--- |
| **Azure Blob Storage** | Blog, profil, ekip, Hakkımızda ve kanıt dosyalarının saklanması; uzun süreli tarayıcı önbelleği. |
| **MailKit (Gmail SMTP)** | Doğrulama kodu, şifre sıfırlama kodu, hesap kapatma, hatırlatma, bloke, rol değişikliği ve yazarlık mailleri. |
| **Google Identity** | Google ile giriş; ID token backend'de doğrulanır. |
| **Cloudflare Turnstile** | Kayıt, giriş ve iletişim formunda bot koruması. |
| **Elasticsearch + Kibana** | Yapılandırılmış logların saklanması, aranması ve görselleştirilmesi. |
| **Google Maps Embed** | İletişim sayfasında konum haritası. |

---

## 📂 Proje Organizasyonu

<details>
<summary><b>Klasör yapısını görmek için tıklayın</b></summary>

```text
ZenBlogBackend/
├── Core/
│   ├── ZenBlog.Domain/              # Entity'ler (Blog, Comment, AppUser, UserBlock, RoleChangeLog,
│   │                                #  WriterApplication, BlogView...) ve enum'lar
│   └── ZenBlog.Application/
│       ├── Base/                    # BaseResult<T>, IBaseResult
│       ├── Behaviors/               # LoggingBehavior, ValidationBehavior
│       ├── Constants/               # Roller, policy'ler, LogEvents, dönemler, hesap kuralları
│       ├── Contracts/               # Persistence ve Infrastructure arayüzleri
│       ├── Features/                # Commands · Queries · Handlers · Results (CQRS)
│       ├── Mappings/                # AutoMapper profilleri
│       ├── Services/                # Hesap içeriği, giriş kontrolü, mail şablonları, arka plan işi
│       └── Validators/              # FluentValidation kuralları
├── Infrastructure/
│   ├── ZenBlog.Persistence/
│   │   ├── Context/                 # AppDbContext (Identity + global sorgu filtreleri)
│   │   ├── Concrete/                # Generic Repository, UnitOfWork, okuma servisleri
│   │   ├── Interceptors/            # AuditDbContextInterceptor (zaman damgası, soft delete)
│   │   ├── IdentityValidations/     # Türkçe Identity hata mesajları
│   │   └── Migrations/
│   └── ZenBlog.Infrastructure/
│       └── Services/                # Jwt, Azure Blob, Email (MailKit), GoogleAuth, Turnstile,
│                                    #  CurrentUser, Logging (Elasticsearch sorguları)
├── Presentation/
│   └── ZenBlog.API/
│       ├── Endpoints/               # Modül modül Minimal API uçları
│       ├── CustomMiddlewares/       # Global hata yakalama
│       ├── Filters/                 # TurnstileValidationFilter
│       ├── Registrations/           # Uç kayıtları
│       └── Program.cs               # Serilog, rate limiting, CORS, kimlik doğrulama, pipeline
└── docker-compose.yml               # Elasticsearch + Kibana (geliştirme)
```

</details>

---

## 📸 Ekran Görüntüleri

Ekran görüntüleri, ziyaretçi deneyiminden yönetim panellerine uzanan süreçlere göre gruplandırılmıştır. İlgili bölümü açarak görselleri inceleyebilirsiniz.

#tüm sayfa
<img width="1763" height="6154" alt="Ekran görüntüsü_27-10-2026_164558_localhost" src="https://github.com/user-attachments/assets/31e21495-c553-45e4-a320-33fbff077615" />

#ana sayfa 
<img width="1903" height="1013" alt="Ekran görüntüsü 2026-10-27 164252" src="https://github.com/user-attachments/assets/b6220c77-8653-4b7c-88be-bc23964db6a5" />
<img width="1905" height="1011" alt="Ekran görüntüsü 2026-10-27 164309" src="https://github.com/user-attachments/assets/e8a19328-3f05-44ab-9061-7a70aafe6565" />
<img width="1901" height="1007" alt="Ekran görüntüsü 2026-10-27 164333" src="https://github.com/user-attachments/assets/0031e3d8-df66-4472-84c5-84e8396a8ff5" />
<img width="1898" height="1000" alt="Ekran görüntüsü 2026-10-27 164349" src="https://github.com/user-attachments/assets/92f5e9c9-cb94-4658-9db7-e4bcfa2f08fb" />
<img width="1916" height="1010" alt="Ekran görüntüsü 2026-10-27 164405" src="https://github.com/user-attachments/assets/b56c83a2-3b8c-422c-83cc-cc85431e4b15" />
<img width="1907" height="1012" alt="Ekran görüntüsü 2026-10-27 164413" src="https://github.com/user-attachments/assets/7eebae2d-cca6-413d-99be-7f11b078e293" />

#Blog Detayı
<img width="1896" height="1000" alt="Ekran görüntüsü 2026-10-27 202329" src="https://github.com/user-attachments/assets/f1eb9fd5-9d0d-4136-8919-75a8c174e1d5" />
<img width="1910" height="1012" alt="Ekran görüntüsü 2026-10-27 202339" src="https://github.com/user-attachments/assets/b06d06de-a742-4b34-a454-39ab64eb9adf" />
<img width="1910" height="1010" alt="Ekran görüntüsü 2026-10-27 202344" src="https://github.com/user-attachments/assets/01599a1e-07f6-49c1-b268-9941f8fd68d7" />
<img width="1911" height="1015" alt="Ekran görüntüsü 2026-10-27 202351" src="https://github.com/user-attachments/assets/1ded8277-4dd1-4c26-97ff-360c535c3091" />
<img width="1912" height="1010" alt="Ekran görüntüsü 2026-10-27 202400" src="https://github.com/user-attachments/assets/7992d7d1-96d7-4d5b-9d31-ce1d428b6c20" />

#bloga yorum yapmak için giriş yapmalıyız & blog yorum işlemleri
<img width="1911" height="1010" alt="Ekran görüntüsü 2026-10-27 202409" src="https://github.com/user-attachments/assets/375aeb58-d2ca-4b0e-a024-51e5a23428e3" />
<img width="1912" height="1015" alt="Ekran görüntüsü 2026-10-27 202448" src="https://github.com/user-attachments/assets/650f0c09-7f0f-4bec-85e3-917ad0572448" />
<img width="1916" height="1015" alt="Ekran görüntüsü 2026-10-27 202505" src="https://github.com/user-attachments/assets/ddc77617-8833-4f3b-984f-cc43718576aa" />


#hakkımzda & ekibimiz
<img width="1906" height="1011" alt="Ekran görüntüsü 2026-10-27 164619" src="https://github.com/user-attachments/assets/623d87c5-c0f9-49f1-b192-7797b11661f6" />
<img width="1911" height="1016" alt="Ekran görüntüsü 2026-10-27 164627" src="https://github.com/user-attachments/assets/b4b829cd-e96d-4264-b648-b86b1c664439" />
<img width="1917" height="1018" alt="Ekran görüntüsü 2026-10-27 164637" src="https://github.com/user-attachments/assets/9f5adb41-f1f1-4449-a380-c76cec83dffc" />

#kategorilere göre bloglar
<img width="1911" height="1010" alt="Ekran görüntüsü 2026-10-27 164704" src="https://github.com/user-attachments/assets/c26160ea-c307-486a-9be2-10eda0d0c96e" />
<img width="1888" height="1012" alt="Ekran görüntüsü 2026-10-27 164654" src="https://github.com/user-attachments/assets/32fab2b0-2647-4130-a914-512596ae0aa8" />

#iletişim
<img width="1912" height="1012" alt="Ekran görüntüsü 2026-10-27 164719" src="https://github.com/user-attachments/assets/bc0c6ab0-c41d-4220-9700-bbddeca40069" />
<img width="1911" height="1013" alt="Ekran görüntüsü 2026-10-27 164728" src="https://github.com/user-attachments/assets/c794b334-baa3-41a3-8ccd-1474800732d6" />
rate limit 5 den fazla atınca 30dk ban yersin sistemdende ddos saldırısan guvenlık
<img width="1912" height="1016" alt="Ekran görüntüsü 2026-10-27 164805" src="https://github.com/user-attachments/assets/499a422d-5444-4f53-9c43-f6d8a786add1" />
<img width="1892" height="1010" alt="Ekran görüntüsü 2026-10-27 171000" src="https://github.com/user-attachments/assets/4ce9450f-dfee-4297-8a12-e801ab763ee3" />

#login & regşster sayfaları ve hataları claudaflare entegrasyonu onyalamdan giriş yapılmıyor rate limit var 5 den fazla şifre yanlıssa 30 dk sistemden ban yersim
<img width="1903" height="1010" alt="Ekran görüntüsü 2026-10-27 165043" src="https://github.com/user-attachments/assets/8d187173-d8c1-48d1-b09f-703b9ab5f5ca" />
<img width="1903" height="1008" alt="Ekran görüntüsü 2026-10-27 165206" src="https://github.com/user-attachments/assets/f9c498e1-03f0-4fc4-b80e-bd9467632c8d" />
<img width="1912" height="1013" alt="Ekran görüntüsü 2026-10-27 165400" src="https://github.com/user-attachments/assets/4b24b0fe-4270-45ca-b22b-7e82464cd4f6" />
<img width="1905" height="1012" alt="Ekran görüntüsü 2026-10-27 170748" src="https://github.com/user-attachments/assets/103ad1b9-4a62-4912-bc4b-a9a40ee88b5a" />
<img width="692" height="250" alt="Ekran görüntüsü 2026-10-27 170808" src="https://github.com/user-attachments/assets/0f51cec0-a3c9-448d-a76f-c25de0a79f67" />
<img width="1906" height="1006" alt="Ekran görüntüsü 2026-10-27 170822" src="https://github.com/user-attachments/assets/618a4dbc-524b-4131-8cff-9b7a27af8407" />
<img width="1911" height="1011" alt="Ekran görüntüsü 2026-10-27 170834" src="https://github.com/user-attachments/assets/5b4c124b-656c-4ee0-9aca-47acc8b17e98" />
<img width="1907" height="1016" alt="Ekran görüntüsü 2026-10-27 170848" src="https://github.com/user-attachments/assets/94a34ab1-c458-42e3-8b15-818cb3dda919" />
<img width="1910" height="1011" alt="Ekran görüntüsü 2026-10-27 170918" src="https://github.com/user-attachments/assets/46aa74b0-fb10-446a-8715-d8a0114fc747" />

#admin paneli 
#dashboard
<img width="1907" height="1012" alt="Ekran görüntüsü 2026-10-27 171227" src="https://github.com/user-attachments/assets/cc30b581-931c-43a7-a920-78e4285f4aa3" />
<img width="1905" height="1012" alt="Ekran görüntüsü 2026-10-27 171237" src="https://github.com/user-attachments/assets/d7eca0c3-cbf0-4a77-b5e9-a7d0564cbc52" />
<img width="1906" height="1003" alt="Ekran görüntüsü 2026-10-27 171248" src="https://github.com/user-attachments/assets/fb188536-1fa0-43d7-a968-4f3f019f3a62" />
<img width="1912" height="1005" alt="Ekran görüntüsü 2026-10-27 171258" src="https://github.com/user-attachments/assets/3615e6da-87b0-4615-91a0-6d3d4d2c1876" />
<img width="1901" height="1007" alt="Ekran görüntüsü 2026-10-27 171304" src="https://github.com/user-attachments/assets/f24c749a-6fe6-45ba-b362-5a7eda5fe70b" />

#bloglar
<img width="1905" height="1006" alt="Ekran görüntüsü 2026-10-27 171321" src="https://github.com/user-attachments/assets/2b652546-2e9c-4586-a48e-938a2ef4e6c7" />
<img width="1897" height="996" alt="Ekran görüntüsü 2026-10-27 171408" src="https://github.com/user-attachments/assets/69cafe79-3d51-40d5-93c1-6b767a963fe6" />

#blog detayı ve yazar bilgileri
<img width="1905" height="1010" alt="Ekran görüntüsü 2026-10-27 171337" src="https://github.com/user-attachments/assets/af44232d-0549-4dbf-896d-e7b9f49f1bed" />
 <img width="1911" height="1007" alt="Ekran görüntüsü 2026-10-27 171353" src="https://github.com/user-attachments/assets/45d017e2-5230-4da1-a041-2c468a281a1a" />

#kategoriler
<img width="1903" height="1010" alt="Ekran görüntüsü 2026-10-27 171415" src="https://github.com/user-attachments/assets/eee03633-796c-4cbc-943e-73545cf4bce9" />

#tüm kullancıılar
<img width="1916" height="1005" alt="Ekran görüntüsü 2026-10-27 171450" src="https://github.com/user-attachments/assets/edce5e58-48c9-40af-a138-fbf117677dd1" />
<img width="1917" height="1015" alt="Ekran görüntüsü 2026-10-27 171455" src="https://github.com/user-attachments/assets/36d23fc4-8367-4016-8db3-086133923631" />

#kullanıcı detayı bloke etme rol verme işlemleri
<img width="1895" height="1006" alt="Ekran görüntüsü 2026-10-27 171508" src="https://github.com/user-attachments/assets/795d98a5-ff1b-425d-a271-5f2041f0fdca" />
<img width="1902" height="1002" alt="Ekran görüntüsü 2026-10-27 171516" src="https://github.com/user-attachments/assets/c5888397-0e3f-42c3-ba57-a56840517b27" />
<img width="1911" height="1006" alt="Ekran görüntüsü 2026-10-27 171535" src="https://github.com/user-attachments/assets/ae98ad57-bdf2-4e58-a082-c404bbad7e71" />
<img width="1916" height="1017" alt="Ekran görüntüsü 2026-10-27 171540" src="https://github.com/user-attachments/assets/adabb869-9ab3-422f-971c-66bcfcecf1f2" />
<img width="1911" height="1012" alt="Ekran görüntüsü 2026-10-27 171556" src="https://github.com/user-attachments/assets/e49a5c5d-624d-49cb-90e9-87c853b2b272" />
bloke sepeb girme ve bloke aedenı sısteme kaydeder
<img width="1911" height="1006" alt="Ekran görüntüsü 2026-10-27 171603" src="https://github.com/user-attachments/assets/deefcd06-5c9b-4845-90bf-4409f6ae847a" />
<img width="1915" height="1003" alt="Ekran görüntüsü 2026-10-27 171647" src="https://github.com/user-attachments/assets/2781d058-3735-497e-9393-70f96533eedf" />
<img width="1902" height="1013" alt="Ekran görüntüsü 2026-10-27 171704" src="https://github.com/user-attachments/assets/e9976ce9-86bc-4387-821e-4037be39bb4e" />
blok kaldırma
<img width="1907" height="1015" alt="Ekran görüntüsü 2026-10-27 171712" src="https://github.com/user-attachments/assets/68e38e02-8107-4b54-8c12-de0235c0b042" />
ozel filterleerle kullanıcları gorme
<img width="1893" height="768" alt="Ekran görüntüsü 2026-10-27 171805" src="https://github.com/user-attachments/assets/bbf4df28-5287-44a4-b881-041e55463c5c" />
bloke işlem mailleri
<img width="677" height="536" alt="Ekran görüntüsü 2026-10-27 172042" src="https://github.com/user-attachments/assets/cd1fb9c3-e77a-4eef-8558-dacd3eab5b5a" />
<img width="675" height="332" alt="Ekran görüntüsü 2026-10-27 172149" src="https://github.com/user-attachments/assets/c05d3a77-d539-41f1-a04f-c8dfe51947c1" />

#youmlar
<img width="1915" height="1013" alt="Ekran görüntüsü 2026-10-27 171816" src="https://github.com/user-attachments/assets/067b5945-22ef-42d5-958a-9e36df1e9d48" />

#hesabını kapatmak isteyen kullancı
<img width="1906" height="1006" alt="Ekran görüntüsü 2026-10-27 172222" src="https://github.com/user-attachments/assets/7fe9c1a1-7642-4e09-a003-b41acc526e5c" />

#Log kayıtalrı kubernates 

<img width="1907" height="1008" alt="Ekran görüntüsü 2026-10-27 172237" src="https://github.com/user-attachments/assets/393564f6-9645-42f0-8704-1b8279f1ec4b" />
<img width="1905" height="1013" alt="Ekran görüntüsü 2026-10-27 172243" src="https://github.com/user-attachments/assets/4655483c-7221-417c-a223-812c7a781bed" />
<img width="1900" height="1012" alt="Ekran görüntüsü 2026-10-27 172253" src="https://github.com/user-attachments/assets/4dc591b2-5ca8-405f-9ce0-f4d02bc25c67" />
<img width="1876" height="997" alt="Ekran görüntüsü 2026-10-27 172500" src="https://github.com/user-attachments/assets/a4504d9b-24af-4426-a217-3015e30efeaf" />

log detayı 
<img width="1895" height="1011" alt="Ekran görüntüsü 2026-10-27 172313" src="https://github.com/user-attachments/assets/e3f5d38a-93a3-42da-a7b4-21719d395406" />
<img width="1901" height="1002" alt="Ekran görüntüsü 2026-10-27 172327" src="https://github.com/user-attachments/assets/8ca5a6ba-06c0-456e-85ee-5a4ac30e4e47" />
<img width="1900" height="1012" alt="Ekran görüntüsü 2026-10-27 172341" src="https://github.com/user-attachments/assets/46e689ad-725c-498c-838d-fa4975234ef4" />
<img width="1898" height="928" alt="Ekran görüntüsü 2026-10-27 172357" src="https://github.com/user-attachments/assets/bbfee1a3-9a06-4eb5-b8e4-c1cb1f5b101a" />
<img width="1906" height="1000" alt="Ekran görüntüsü 2026-10-27 172412" src="https://github.com/user-attachments/assets/f6d48250-bc10-4d23-8636-3c5d3133f199" />
<img width="1892" height="991" alt="Ekran görüntüsü 2026-10-27 172423" src="https://github.com/user-attachments/assets/2e363cdc-5844-4e58-a72e-4e88a4da70c0" />


#roller
<img width="1907" height="1012" alt="Ekran görüntüsü 2026-10-27 172509" src="https://github.com/user-attachments/assets/bb5ec705-0603-4b98-ae66-ec0beee4fcaf" />

#yazarlık başvuları
<img width="1905" height="715" alt="Ekran görüntüsü 2026-10-27 172748" src="https://github.com/user-attachments/assets/e5840add-57f4-4c13-861d-604c5c336eb8" />
<img width="1898" height="620" alt="Ekran görüntüsü 2026-10-27 172629" src="https://github.com/user-attachments/assets/a817a55f-bb93-4f72-ae08-e763d59d8d11" />
<img width="1912" height="1017" alt="Ekran görüntüsü 2026-10-27 172636" src="https://github.com/user-attachments/assets/3702efcd-f08c-4fdd-8fa9-09c67db40a62" />
<img width="1902" height="1002" alt="Ekran görüntüsü 2026-10-27 172643" src="https://github.com/user-attachments/assets/8bf223f7-dd5c-4f7c-b629-a5076abd01f4" />
<img width="1912" height="1013" alt="Ekran görüntüsü 2026-10-27 172702" src="https://github.com/user-attachments/assets/8fde3ce4-b5f9-4ad8-85be-c79300f0b152" />
<img width="1906" height="1016" alt="Ekran görüntüsü 2026-10-27 172713" src="https://github.com/user-attachments/assets/a77244a5-ae33-4bef-a390-0fd4aee842d3" />
<img width="1897" height="1015" alt="Ekran görüntüsü 2026-10-27 172724" src="https://github.com/user-attachments/assets/d22e3a63-0c03-4b46-b090-2936527ca64c" />
<img width="1902" height="1013" alt="Ekran görüntüsü 2026-10-27 172732" src="https://github.com/user-attachments/assets/d1aca37d-0845-4928-a1c5-52a9b1974f54" />


#iletişimden gelen mesajlar
<img width="1905" height="1005" alt="Ekran görüntüsü 2026-10-27 172807" src="https://github.com/user-attachments/assets/b89e7865-6176-47d2-9bba-2ac553184ec5" />
<img width="1907" height="1011" alt="Ekran görüntüsü 2026-10-27 172820" src="https://github.com/user-attachments/assets/9de2ca22-82ff-40b7-8285-4ae03d5fcd63" />

#diğer sayfa işlemelri 
<img width="1915" height="1011" alt="Ekran görüntüsü 2026-10-27 172842" src="https://github.com/user-attachments/assets/75688a35-0bdc-4806-ad18-9e458ea4bf5f" />
<img width="1917" height="1011" alt="Ekran görüntüsü 2026-10-27 172829" src="https://github.com/user-attachments/assets/ae3faa83-91f3-462e-ace4-ee472de1816b" />
<img width="1902" height="998" alt="Ekran görüntüsü 2026-10-27 172848" src="https://github.com/user-attachments/assets/97a59e68-27ee-4b39-aa36-1f79ceba73ca" />
<img width="1911" height="1008" alt="Ekran görüntüsü 2026-10-27 172853" src="https://github.com/user-attachments/assets/05430570-137e-4564-9399-738eff21dc2a" />
<img width="1907" height="1010" alt="Ekran görüntüsü 2026-10-27 172902" src="https://github.com/user-attachments/assets/fceddd4c-387a-4788-856f-39acaf368a22" />


#admin de girene özel
<img width="1910" height="998" alt="Ekran görüntüsü 2026-10-27 172920" src="https://github.com/user-attachments/assets/2ce607fe-8c21-46f6-ac1a-39710488f9cc" />
<img width="1916" height="1011" alt="Ekran görüntüsü 2026-10-27 172927" src="https://github.com/user-attachments/assets/a7d56405-448b-46b7-9add-f6c8c1d17224" />
<img width="1896" height="1008" alt="Ekran görüntüsü 2026-10-27 172937" src="https://github.com/user-attachments/assets/3459a908-2dab-4bd7-9123-507558eb5c72" />
<img width="1905" height="1012" alt="Ekran görüntüsü 2026-10-27 172949" src="https://github.com/user-attachments/assets/5ba9f2c2-e12b-4cbd-b074-f40e9fb5c98b" />
<img width="1903" height="1012" alt="Ekran görüntüsü 2026-10-27 172956" src="https://github.com/user-attachments/assets/569c0f53-a16c-4781-b9b8-f7fb9c919555" />
<img width="1903" height="1012" alt="Ekran görüntüsü 2026-10-27 172956" src="https://github.com/user-attachments/assets/569c0f53-a16c-4781-b9b8-f7fb9c919555" />
<img width="1897" height="1000" alt="Ekran görüntüsü 2026-10-27 173009" src="https://github.com/user-attachments/assets/8b843ad1-9158-40de-bb47-1aaa3fd980be" />
<img width="1896" height="1003" alt="Ekran görüntüsü 2026-10-27 173023" src="https://github.com/user-attachments/assets/be6e9397-a588-47ff-93d0-04f87d4fa699" />



#writer paneli

#dashboard
<img width="1907" height="1002" alt="Ekran görüntüsü 2026-10-27 173053" src="https://github.com/user-attachments/assets/0cab60f2-64f6-40bc-b0b1-a91f8b0d5a49" />
<img width="1913" height="1013" alt="Ekran görüntüsü 2026-10-27 173100" src="https://github.com/user-attachments/assets/bd257e1e-f3f2-4dad-bc80-9dab746b740d" />

#bloglarım

<img width="1911" height="1000" alt="Ekran görüntüsü 2026-10-27 173110" src="https://github.com/user-attachments/assets/7d2294c6-066f-4c64-b348-58a8bba73d9a" />
<img width="1911" height="1005" alt="Ekran görüntüsü 2026-10-27 173116" src="https://github.com/user-attachments/assets/46b10344-7e36-4463-9c3c-b346cde700d0" />

#bloglarıana gelen yotumlar
<img width="1907" height="1007" alt="Ekran görüntüsü 2026-10-27 173125" src="https://github.com/user-attachments/assets/19b92dba-3d5c-421b-94d3-5ad832f357b1" />
#kedni yorumlaerı
<img width="1907" height="760" alt="Ekran görüntüsü 2026-10-27 173227" src="https://github.com/user-attachments/assets/8dd77940-8194-4755-80b3-11d0f09132b3" />

#profil hesap kapatma 
































































---

## ⚙️ Kurulum

<details>
<summary><b>Gereksinimleri ve yerel kurulum adımlarını görmek için tıklayın</b></summary>

### Gereksinimler

- .NET 9 SDK ve EF Core araçları (`dotnet-ef`)
- SQL Server
- Node.js ve Angular CLI 20 (frontend için)
- Docker Desktop (isteğe bağlı: Elasticsearch ve Kibana için)
- İlgili özellikler için Azure Storage hesabı, Gmail uygulama şifresi, Google OAuth istemci kimliği ve Cloudflare Turnstile anahtarı

### 1. Yapılandırma (User Secrets)

Gizli bilgiler `ZenBlog.API` projesinde **User Secrets** ile tutulur. Gerçek anahtarları ve şifreleri depoya eklemeyin.

| Ayar anahtarı | Kullanım |
| --- | --- |
| `ConnectionStrings:MSSQL_Connection` | SQL Server bağlantı bilgisi |
| `EmailSettings:Host` · `Port` | SMTP sunucusu (ör. `smtp.gmail.com`, `587`) |
| `EmailSettings:SenderEmail` · `SenderName` | Gönderici adresi ve görünen ad |
| `EmailSettings:Password` | SMTP uygulama şifresi |
| `AzureBlobStorage:ConnectionString` | Azure Storage bağlantı bilgisi |
| `AzureBlobStorage:ContainerName` | Dosyaların saklanacağı kapsayıcı |
| `JwtTokenOptions:Issuer` · `Audience` | Token'ı yayınlayan ve hedef kitle |
| `JwtTokenOptions:Key` | İmza anahtarı (uzun ve rastgele olmalı) |
| `JwtTokenOptions:ExpireInMinutes` | Token süresi (dakika) |
| `GoogleAuth:ClientId` | Google ile giriş istemci kimliği |
| `Turnstile:SecretKey` | Cloudflare Turnstile gizli anahtarı |
| `Elasticsearch:Url` | Log sunucusu (ör. `http://localhost:9200`); boşsa loglar sadece konsola yazılır |

Örnek:

```bash
cd Presentation/ZenBlog.API
dotnet user-secrets set "ConnectionStrings:MSSQL_Connection" "Server=.;Database=ZenBlogDb;Trusted_Connection=True;TrustServerCertificate=True"
dotnet user-secrets set "JwtTokenOptions:Key" "EN_AZ_32_KARAKTERLIK_RASTGELE_BIR_ANAHTAR"
```

`Serilog` bölümü (log seviyeleri) gizli bilgi değildir; `appsettings.json` içinde tutulabilir.

### 2. Veritabanı

```bash
dotnet ef database update --project Infrastructure/ZenBlog.Persistence --startup-project Presentation/ZenBlog.API
```

### 3. Loglama altyapısı (isteğe bağlı)

```bash
docker compose up -d
```

- Elasticsearch: `http://localhost:9200`
- Kibana: `http://localhost:5601` (data view: `logs-zenblog-*`)

Bu yapılandırma **yalnızca geliştirme içindir**: Elasticsearch güvenliği (`xpack.security`) kapalıdır.

### 4. Çalıştırma

```bash
# Backend
dotnet run --project Presentation/ZenBlog.API      # https://localhost:7200 · Scalar: /scalar

# Frontend (ayrı depo)
npm install
ng serve                                            # http://localhost:4200
```

</details>

---

## 🧭 Geliştirme Notları

- Turnstile, geliştirme ortamında Cloudflare'in test anahtarlarıyla çalışacak şekilde yapılandırılabilir; canlı ortam için gerçek anahtarlar gerekir.
- Elasticsearch ve Kibana, Docker Compose ile yalnızca yerel geliştirme için kurgulanmıştır.
- API ve frontend adresleri geliştirme ortamına göre (`localhost`) yapılandırılmıştır.

---

<div align="center">

## 💬 Proje ve İletişim

Projenin geliştirme süreci ve teknik detayları hakkında iletişime geçebilirsiniz.

<a href="https://www.linkedin.com/in/burakhanulusoy/">
  <img src="https://img.shields.io/badge/LinkedIn-Burakhan%20Ulusoy-0A66C2?style=for-the-badge&logo=linkedin&logoColor=white" alt="Burakhan Ulusoy LinkedIn" />
</a>

<br>
<br>

**🌿 ZenBlog · .NET 9 Minimal API + Angular 20 ile çok rollü blog platformu**

</div>


