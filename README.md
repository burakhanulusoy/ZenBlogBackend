<div align="center">

# 🌿 ZenBlog

### Çok Rollü, Güvenlik Odaklı Blog Platformu

![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Minimal API](https://img.shields.io/badge/ASP.NET_Core-Minimal_API-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Azure Blob](https://img.shields.io/badge/Microsoft_Azure-Blob_Storage-0078D4?style=for-the-badge&logo=microsoftazure&logoColor=white)
![Angular 20](https://img.shields.io/badge/Angular-20-DD0031?style=for-the-badge&logo=angular&logoColor=white)
![SQL Server](https://img.shields.io/badge/Database-SQL_Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)

![MediatR](https://img.shields.io/badge/Pattern-MediatR_%26_CQRS-7C3AED?style=for-the-badge)
![JWT](https://img.shields.io/badge/Auth-JWT_%2B_Identity-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white)
![PrimeNG](https://img.shields.io/badge/UI-PrimeNG_20-DD0031?style=for-the-badge)
![Cloudflare](https://img.shields.io/badge/Bot_Koruması-Turnstile-F38020?style=for-the-badge&logo=cloudflare&logoColor=white)

![Serilog](https://img.shields.io/badge/Logging-Serilog-1E293B?style=for-the-badge)
![Elasticsearch](https://img.shields.io/badge/Search-Elasticsearch_9-005571?style=for-the-badge&logo=elasticsearch&logoColor=white)
![Kibana](https://img.shields.io/badge/Kibana-9-E8478B?style=for-the-badge&logo=kibana&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?style=for-the-badge&logo=docker&logoColor=white)

<p align="center">
  <strong>Okurdan yazara, yazardan yöneticiye: rol bazlı paneller, güvenli hesap yaşam döngüsü ve gözlemlenebilir bir altyapı.</strong>
</p>
<p align="center">
  <i>.NET 9 Minimal API · Clean Architecture · CQRS · JWT + SecurityStamp · Rate Limiting · Cloudflare Turnstile<br>
  Azure Blob Storage · Serilog + Elasticsearch + Kibana · Angular 20 + PrimeNG ile üç panel ve ziyaretçi sitesi</i>
</p>

<p align="center">
  <a href="https://github.com/KULLANICI_ADIN/ZenBlogBackend"><b>⚙️ Backend (bu depo)</b></a>
  &nbsp;·&nbsp;
  <a href="https://github.com/KULLANICI_ADIN/ZenBlogFrontend"><b>🎨 Frontend deposu</b></a>
</p>

</div>

---

**ZenBlog**, okurların yazıları okuyup yorum yaptığı, yazarların içerik ürettiği ve yöneticilerin bütün sistemi denetlediği çok rollü bir blog platformudur. Backend **.NET 9 Minimal API** ile Clean Architecture prensiplerine göre; frontend **Angular 20 + PrimeNG** ile bir ziyaretçi sitesi ve üç ayrı panel olarak geliştirilmiştir. Görseller **Azure Blob Storage**'da saklanır, loglar **Elasticsearch**'e akar.

## ⚡ Bir Bakışta

- 🔒 **Rolü değişen ya da bloke edilen kullanıcının oturumu anında düşer:** JWT içindeki SecurityStamp her istekte kontrol edilir.
- ♻️ **30 günlük, geri alınabilir hesap kapatma:** içerikler gizlenir, süre dolmadan giriş yapan kullanıcının her şeyi geri gelir.
- 🚫 **Kanıtlı bloke sistemi:** gerekçe ve kanıt dosyası zorunlu, bütün geçmiş kayıt altında.
- ✍️ **Yazarlık başvurusu:** taahhütnameli form, yönetici değerlendirmesi, onayda otomatik rol.
- 🛡️ **Rate limiting, Cloudflare Turnstile ve hesap kilitleme** ile kötüye kullanıma karşı koruma.
- ☁️ **Azure Blob Storage** ile dosya yönetimi ve uzun süreli tarayıcı önbelleği.
- 📊 **Dönem karşılaştırmalı dashboard'lar** ve günlük tekil **okunma sayacı**.
- 🔭 **Serilog + Elasticsearch + Kibana** ve yönetim panelinde **Query DSL ile çalışan log ekranı**.

---

## 🌟 Temel Modüller

### 🔐 1. Kimlik Doğrulama ve JWT Güvenliği
- **ASP.NET Core Identity** (Guid anahtarlı) ve **JWT Bearer** (HS256; issuer, audience, imza ve süre doğrulaması, `ClockSkew = 0`).
- **SecurityStamp:** `OnTokenValidated` her istekte hesabın aktif olduğunu ve token'daki damganın güncel olduğunu kontrol eder. Rol, bloke ya da hesap durumu değişince eski token **bir sonraki istekte** geçersiz olur.
- **6 haneli kodla** e-posta doğrulama ve şifre sıfırlama (MailKit).
- **Google ile giriş:** ID token backend'de doğrulanır, hesap oluşturulur ya da mevcut hesapla ilişkilendirilir.
- Tek tip giriş hatası ("E-posta veya şifre hatalı") ve Türkçe Identity mesajları.

### 🛡️ 2. Kötüye Kullanıma Karşı Koruma
- **Güvenli varsayılan:** Bütün uçlar `/api` grubunda `RequireAuthorization()` ile korunur; açık uçlar tek tek `AllowAnonymous()` ile açılır.
- **Rate Limiting:** IP başına sabit pencere (giriş, doğrulama kodu ve iletişim formu için ayrı limitler); sınır aşılınca `429`.
- **Cloudflare Turnstile:** Kayıt, giriş ve iletişim formunda `IEndpointFilter` olarak; doğrulanmadan istek handler'a ulaşmaz.
- **Hesap kilitleme:** Art arda hatalı şifre denemesinde hesap geçici olarak kilitlenir.
- **Yorum sahipliği:** Yorumu sadece sahibi düzenler; sahibi, blogun yazarı ya da Admin silebilir.
- Beklenmeyen hatalarda kullanıcıya teknik ayrıntı değil, **hata kodu** döner.

### ♻️ 3. Hesap Yaşam Döngüsü
- **Hesabımı kapat:** Şifre onayı → 30 günlük süreç → içerikler gizlenir → 3 gün kala hatırlatma maili (`BackgroundService`).
- **Geri dönüş:** Süre dolmadan giriş yapan kullanıcının kapatma işlemi iptal edilir, içerikleri geri açılır.
- **Bloke:** Gerekçe (en az 20 karakter) + kanıt dosyası (jpg, png, webp, pdf) zorunlu. Kullanıcıya mail gider, içerikleri gizlenir, oturumu düşer. Bloke ve kaldırma geçmişi `UserBlock` tablosunda saklanır.
- **Kurallar:** Admin bloke edilemez, sistemde en az bir yönetici kalır.

### 👑 4. Rol Yönetimi ve Yazarlık Başvurusu
- **Rol atama:** `User` temel rolü kilitli; Admin kendi yönetici rolünü kaldıramaz; **Admin rolü verilirken şifre onayı** istenir.
- Her değişiklik `RoleChangeLog`'a yazılır, kullanıcıya mail gider.
- **Yazarlık başvurusu:** Konu seçimi, motivasyon, örnek bağlantılar ve 8 maddelik taahhütname. Onayda `Writer` rolü ve tebrik maili; redde gerekçe ve 30 gün bekleme.

### 📝 5. İçerik, Yorumlar ve Okunma Sayacı
- Bölümlü blog yapısı ve üç görsel. Dosyalar **Azure Blob Storage**'a yüklenir, `Cache-Control: immutable` ile tarayıcıda uzun süre önbelleğe alınır.
- Yorum ve tek seviyeli yanıtlar; "Yazar" rozeti ve "(düzenlendi)" işareti.
- **Günlük tekil okunma:** `(BlogId, Ziyaretçi, Gün)` benzersiz index'i; ziyaretçi kimliği **SHA-256 ile özetlenir**, IP saklanmaz; artış tek bir atomik SQL ile yapılır.
- Ziyaretçi sitesi: ana sayfa tek istekte (`/api/home`), ilgili yazılar ve veritabanında sayfalanan kategori sayfası.

### 📊 6. Paneller ve Dashboard'lar
- **Admin:** 7 gün / 30 gün / 12 ay; önceki döneme göre değişim; trafik grafiği; "Seni bekleyenler"; dağılımlar ve listeler.
- **Yazar:** Piksel (8-bit) tasarım, yazar seviye sistemi, okunma ve yorum grafiği.
- Site içeriği (Hakkımızda, Ekip, Sosyal medya, İletişim), gelen mesajlar ve yorum yönetimi sayfaları.

### 🔭 7. Loglama ve Gözlemlenebilirlik
- **Serilog** ile yapılandırılmış loglar; konsol ve **Elasticsearch** (ECS formatı, `logs-zenblog-*`).
- **`LoggingBehavior`** (MediatR): her komutun süresi, sonucu ve doğrulama hataları.
- **Önemli olaylar (`EventType`):** giriş, kilitlenme, bloke, rol, yazarlık, hesap kapatma, e-posta hataları.
- Hata kodu = `trace.id`: Kullanıcının söylediği kodla o isteğin bütün logları bulunur.
- Şifre, e-posta ve IP **loglanmaz**. Loglama hiçbir koşulda uygulamayı durdurmaz.
- **Kibana** dashboard'u ve admin panelinde **Elasticsearch Query DSL** ile çalışan "Log kayıtları" ekranı.

---

## 🏗️ Mimari ve Tasarım Yaklaşımı

Backend, **Clean Architecture** prensiplerine göre beş projeye ayrılmıştır: `Domain`, `Application`, `Persistence`, `Infrastructure` ve `API`. Bağımlılıklar içe doğrudur; Application katmanı veritabanını, Azure'u ya da Elasticsearch'ü değil, yalnızca arayüzleri bilir.

| Yapı / Konsept | Projedeki Kullanımı |
| :--- | :--- |
| **API** | .NET 9, ASP.NET Core **Minimal API**; uçlar `MapGroup` ile modül modül gruplanır |
| **Mimari** | Clean Architecture: Domain · Application · Persistence · Infrastructure · API |
| **Komut ve Sorgu Ayrımı** | **MediatR** ile CQRS: Commands, Queries, Handlers, Results |
| **Pipeline Behavior'lar** | `LoggingBehavior` (en dışta) → `ValidationBehavior` → Handler |
| **Doğrulama** | **FluentValidation**; hatalar alan bazlı `400` cevabına çevrilir |
| **Nesne Eşleme** | **AutoMapper** |
| **Veri Erişimi** | **EF Core** + **SQL Server**; Generic Repository ve Unit of Work |
| **Okuma Servisleri** | Yoğun ekranlar için **projeksiyonla** çalışan servisler (ana sayfa, kategori, yorumlar, dashboard); N+1 yok |
| **Kayıt Takibi ve Soft Delete** | `AuditDbContextInterceptor`; silme `IsDeleted`'a çevrilir; global sorgu filtresi |
| **Standart Cevap** | Bütün uçlar `BaseResult<T>` döndürür |
| **Kimlik ve Yetki** | ASP.NET Core Identity · JWT Bearer · Policy (`AdminOnly`, `WriterOrAdmin`) · SecurityStamp |
| **Dosya Depolama** | **Azure Blob Storage** (`IFileService`) |
| **E-posta** | **MailKit** ve ortak HTML mail şablonu |
| **Arka Plan İşleri** | `BackgroundService` (hesap kapatma hatırlatması) |
| **Koruma** | Rate Limiting · Cloudflare Turnstile (`IEndpointFilter`) |
| **Loglama** | Serilog · Elastic.Serilog.Sinks (ECS) · Elasticsearch 9 · Kibana 9 |
| **API Dokümantasyonu** | OpenAPI + **Scalar** |
| **Geliştirme Ortamı** | Docker Compose (Elasticsearch + Kibana) |

### ⚡ Minimal API ve Güvenli Varsayılan
Controller yerine **Minimal API** kullanılır. Her modülün uçları kendi dosyasında bir uzantı metodu olarak tanımlanır ve `/api` grubuna bağlanır. Grup seviyesinde `RequireAuthorization()` uygulandığı için herkese açık uçlar bilinçli olarak işaretlenir.

### 🧅 MediatR Pipeline
Her istek handler'a ulaşmadan önce iki katmandan geçer. **`LoggingBehavior`** süreyi ölçer ve sonucu loglar; **`ValidationBehavior`** FluentValidation kurallarını çalıştırır ve hata varsa isteği handler'a hiç göndermez.

### 🔎 Projeksiyon ile Okuma
Yoğun ekranlar ayrı okuma servisleriyle yazılmıştır. Ana sayfa verisi birkaç projeksiyon sorgusuyla gelir; kategori başına son 5 blog, EF Core'un `ROW_NUMBER() OVER (PARTITION BY ...)` çevirisiyle tek sorguda alınır.

---

## 👥 Kullanıcı Rolleri ve Yetkiler

| Rol | Yetki ve İşlevler |
| :--- | :--- |
| **Ziyaretçi** | Yazıları okuma, kategori, Hakkımızda ve İletişim sayfaları; iletişim formu; kayıt ve giriş. |
| **User (Okur)** | Yorum ve yanıt yazma, kendi yorumlarını düzenleme ve silme; profil yönetimi; hesabını kapatma; yazarlık başvurusu. |
| **Writer (Yazar)** | Okurun bütün yetkileri + blog yazma ve yönetme, bloglarına gelen yorumları yönetme, yazar dashboard'u. |
| **Admin (Yönetici)** | Bütün bloglar ve yorumlar, kategoriler, kullanıcılar (bloke, rol), yazarlık başvuruları, site içeriği, gelen mesajlar, admin dashboard'u ve log kayıtları. |

---

## 🔌 Uç Noktalara Genel Bakış

Bütün uçlar `/api` altındadır ve `BaseResult<T>` döndürür. Tam liste geliştirme ortamında Scalar'dan (`/scalar`) görülebilir.

| Modül | Örnek uçlar | Erişim |
| :--- | :--- | :--- |
| **Hesap** | `users/register` · `login` · `google-login` · `confirm-email` · `forgot-password` · `reset-password` | Açık (Turnstile + rate limit) |
| **Profil** | `users/me` · `users/me/close` · `users/me/writer-application` | Giriş yapmış |
| **Kullanıcı yönetimi** | `users` · `users/{id}/block` · `unblock` · `roles/assign` | Admin |
| **Yazarlık** | `writer-applications` · `{id}/approve` · `reject` | Admin |
| **Blog** | `blogs/{id}` · `{id}/related` · `{id}/view` · `blogs/my` | Okuma açık · yazma Writer/Admin |
| **Site** | `home` · `categories/{id}/blogs` · `abouts` · `teams` · `contacts` · `social-medias` | Açık |
| **Yorumlar** | `comments/byBlogId/{id}` · `mine` · `on-my-blogs` · `all` · `subcomments` | Rol ve sahipliğe göre |
| **Dashboard** | `dashboard/admin` · `dashboard/writer` `?period=7d\|30d\|12m` | Admin · Writer/Admin |
| **Loglar** | `logs` · `logs/summary` · `logs/detail` | Admin |

---

## 🔗 Entegrasyonlar

| Entegrasyon | Uygulamaya Katkısı |
| :--- | :--- |
| **Azure Blob Storage** | Blog, profil, ekip, Hakkımızda ve kanıt dosyalarının saklanması; uzun süreli tarayıcı önbelleği. |
| **MailKit (Gmail SMTP)** | Doğrulama ve şifre sıfırlama kodları; hesap kapatma, hatırlatma, bloke, rol ve yazarlık mailleri. |
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
│       ├── Constants/               # Roller, policy'ler, LogEvents, hesap kuralları
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

### 🏠 Ziyaretçi Sitesi

<details>
<summary><b>Görselleri incelemek için açın</b></summary>

#### Ana Sayfanın Tamamı
<img width="100%" alt="Ana sayfanın tamamı" src="https://github.com/user-attachments/assets/31e21495-c553-45e4-a320-33fbff077615" />

#### Ana Sayfa
> Slider, son yazılar, "Trending" ve kategori bölümleri.

<img width="100%" alt="Ana Sayfa — Görsel 1" src="https://github.com/user-attachments/assets/b6220c77-8653-4b7c-88be-bc23964db6a5" />
<img width="100%" alt="Ana Sayfa — Görsel 2" src="https://github.com/user-attachments/assets/e8a19328-3f05-44ab-9061-7a70aafe6565" />
<img width="100%" alt="Ana Sayfa — Görsel 3" src="https://github.com/user-attachments/assets/0031e3d8-df66-4472-84c5-84e8396a8ff5" />
<img width="100%" alt="Ana Sayfa — Görsel 4" src="https://github.com/user-attachments/assets/92f5e9c9-cb94-4658-9db7-e4bcfa2f08fb" />
<img width="100%" alt="Ana Sayfa — Görsel 5" src="https://github.com/user-attachments/assets/b56c83a2-3b8c-422c-83cc-cc85431e4b15" />
<img width="100%" alt="Ana Sayfa — Görsel 6" src="https://github.com/user-attachments/assets/7eebae2d-cca6-413d-99be-7f11b078e293" />

#### Hakkımızda ve Ekibimiz
<img width="100%" alt="Hakkımızda — Görsel 1" src="https://github.com/user-attachments/assets/623d87c5-c0f9-49f1-b192-7797b11661f6" />
<img width="100%" alt="Hakkımızda — Görsel 2" src="https://github.com/user-attachments/assets/b4b829cd-e96d-4264-b648-b86b1c664439" />
<img width="100%" alt="Hakkımızda — Görsel 3" src="https://github.com/user-attachments/assets/9f5adb41-f1f1-4449-a380-c76cec83dffc" />

#### Kategorilere Göre Bloglar
<img width="100%" alt="Kategori Sayfası — Görsel 1" src="https://github.com/user-attachments/assets/c26160ea-c307-486a-9be2-10eda0d0c96e" />
<img width="100%" alt="Kategori Sayfası — Görsel 2" src="https://github.com/user-attachments/assets/32fab2b0-2647-4130-a914-512596ae0aa8" />

#### İletişim
<img width="100%" alt="İletişim — Görsel 1" src="https://github.com/user-attachments/assets/bc0c6ab0-c41d-4220-9700-bbddeca40069" />
<img width="100%" alt="İletişim — Görsel 2" src="https://github.com/user-attachments/assets/c794b334-baa3-41a3-8ccd-1474800732d6" />

> **Rate limiting ve Turnstile:** İletişim formu bot doğrulaması olmadan gönderilemez; kısa sürede çok fazla mesaj gönderildiğinde istek `429` ile reddedilir.

<img width="100%" alt="İletişim Rate Limit — Görsel 1" src="https://github.com/user-attachments/assets/499a422d-5444-4f53-9c43-f6d8a786add1" />
<img width="100%" alt="İletişim Rate Limit — Görsel 2" src="https://github.com/user-attachments/assets/4ce9450f-dfee-4297-8a12-e801ab763ee3" />

</details>

---

### ✍️ Blog Detayı ve Yorumlar

<details>
<summary><b>Görselleri incelemek için açın</b></summary>

#### Blog Detayı
> Okuma süresi, okuma ilerleme çubuğu, yazar kutusu, paylaşım ve ilgili yazılar.

<img width="100%" alt="Blog Detayı — Görsel 1" src="https://github.com/user-attachments/assets/f1eb9fd5-9d0d-4136-8919-75a8c174e1d5" />
<img width="100%" alt="Blog Detayı — Görsel 2" src="https://github.com/user-attachments/assets/b06d06de-a742-4b34-a454-39ab64eb9adf" />
<img width="100%" alt="Blog Detayı — Görsel 3" src="https://github.com/user-attachments/assets/01599a1e-07f6-49c1-b268-9941f8fd68d7" />
<img width="100%" alt="Blog Detayı — Görsel 4" src="https://github.com/user-attachments/assets/1ded8277-4dd1-4c26-97ff-360c535c3091" />
<img width="100%" alt="Blog Detayı — Görsel 5" src="https://github.com/user-attachments/assets/7992d7d1-96d7-4d5b-9d31-ce1d428b6c20" />

#### Yorum İşlemleri
> Yorum yapmak için giriş zorunludur; giriş sonrası kullanıcı aynı yazıya geri döner. Yorum, yanıt, düzenleme ve silme.

<img width="100%" alt="Yorumlar — Görsel 1" src="https://github.com/user-attachments/assets/375aeb58-d2ca-4b0e-a024-51e5a23428e3" />
<img width="100%" alt="Yorumlar — Görsel 2" src="https://github.com/user-attachments/assets/650f0c09-7f0f-4bec-85e3-917ad0572448" />
<img width="100%" alt="Yorumlar — Görsel 3" src="https://github.com/user-attachments/assets/ddc77617-8833-4f3b-984f-cc43718576aa" />

</details>

---

### 🔐 Giriş, Kayıt ve Hesap Güvenliği

<details>
<summary><b>Görselleri incelemek için açın</b></summary>

> Cloudflare Turnstile doğrulaması yapılmadan giriş ve kayıt gönderilemez. Hatalı şifre denemeleri sınırlandırılır ve art arda hatalı denemede hesap geçici olarak kilitlenir. 6 haneli kodla e-posta doğrulama, şifre sıfırlama ve Google ile giriş.

<img width="100%" alt="Giriş ve Kayıt — Görsel 1" src="https://github.com/user-attachments/assets/8d187173-d8c1-48d1-b09f-703b9ab5f5ca" />
<img width="100%" alt="Giriş ve Kayıt — Görsel 2" src="https://github.com/user-attachments/assets/f9c498e1-03f0-4fc4-b80e-bd9467632c8d" />
<img width="100%" alt="Giriş ve Kayıt — Görsel 3" src="https://github.com/user-attachments/assets/4b24b0fe-4270-45ca-b22b-7e82464cd4f6" />
<img width="100%" alt="Giriş ve Kayıt — Görsel 4" src="https://github.com/user-attachments/assets/103ad1b9-4a62-4912-bc4b-a9a40ee88b5a" />
<img width="692" alt="Giriş ve Kayıt — Görsel 5" src="https://github.com/user-attachments/assets/0f51cec0-a3c9-448d-a76f-c25de0a79f67" />
<img width="100%" alt="Giriş ve Kayıt — Görsel 6" src="https://github.com/user-attachments/assets/618a4dbc-524b-4131-8cff-9b7a27af8407" />
<img width="100%" alt="Giriş ve Kayıt — Görsel 7" src="https://github.com/user-attachments/assets/5b4c124b-656c-4ee0-9aca-47acc8b17e98" />
<img width="100%" alt="Giriş ve Kayıt — Görsel 8" src="https://github.com/user-attachments/assets/94a34ab1-c458-42e3-8b15-818cb3dda919" />
<img width="100%" alt="Giriş ve Kayıt — Görsel 9" src="https://github.com/user-attachments/assets/46aa74b0-fb10-446a-8715-d8a0114fc747" />

#### 404 ve 403 Sayfaları
<img width="100%" alt="Hata Sayfaları — Görsel 1" src="https://github.com/user-attachments/assets/aab5bb04-96fa-4062-af82-e5dd08b7970d" />
<img width="100%" alt="Hata Sayfaları — Görsel 2" src="https://github.com/user-attachments/assets/36a2fec0-12c6-4919-a2c3-7509164afea4" />

</details>

---

### 🛠️ Admin Paneli

<details>
<summary><b>Görselleri incelemek için açın</b></summary>

#### Genel Bakış
> Dönem seçici, önceki döneme göre değişim, trafik grafiği, bekleyen işler, dağılımlar ve listeler.

<img width="100%" alt="Admin Genel Bakış — Görsel 1" src="https://github.com/user-attachments/assets/cc30b581-931c-43a7-a920-78e4285f4aa3" />
<img width="100%" alt="Admin Genel Bakış — Görsel 2" src="https://github.com/user-attachments/assets/d7eca0c3-cbf0-4a77-b5e9-a7d0564cbc52" />
<img width="100%" alt="Admin Genel Bakış — Görsel 3" src="https://github.com/user-attachments/assets/fb188536-1fa0-43d7-a968-4f3f019f3a62" />
<img width="100%" alt="Admin Genel Bakış — Görsel 4" src="https://github.com/user-attachments/assets/3615e6da-87b0-4615-91a0-6d3d4d2c1876" />
<img width="100%" alt="Admin Genel Bakış — Görsel 5" src="https://github.com/user-attachments/assets/f24c749a-6fe6-45ba-b362-5a7eda5fe70b" />

#### Bloglar
<img width="100%" alt="Bloglar — Görsel 1" src="https://github.com/user-attachments/assets/2b652546-2e9c-4586-a48e-938a2ef4e6c7" />
<img width="100%" alt="Bloglar — Görsel 2" src="https://github.com/user-attachments/assets/69cafe79-3d51-40d5-93c1-6b767a963fe6" />

#### Blog Detayı ve Yazar Bilgileri
<img width="100%" alt="Blog Detayı (Admin) — Görsel 1" src="https://github.com/user-attachments/assets/af44232d-0549-4dbf-896d-e7b9f49f1bed" />
<img width="100%" alt="Blog Detayı (Admin) — Görsel 2" src="https://github.com/user-attachments/assets/45d017e2-5230-4da1-a041-2c468a281a1a" />

#### Kategoriler
<img width="100%" alt="Kategoriler — Görsel 1" src="https://github.com/user-attachments/assets/eee03633-796c-4cbc-943e-73545cf4bce9" />

#### Tüm Kullanıcılar
<img width="100%" alt="Kullanıcılar — Görsel 1" src="https://github.com/user-attachments/assets/edce5e58-48c9-40af-a138-fbf117677dd1" />
<img width="100%" alt="Kullanıcılar — Görsel 2" src="https://github.com/user-attachments/assets/36d23fc4-8367-4016-8db3-086133923631" />

> Özel filtrelerle kullanıcıları görüntüleme:

<img width="100%" alt="Kullanıcı Filtreleri — Görsel 1" src="https://github.com/user-attachments/assets/bbf4df28-5287-44a4-b881-041e55463c5c" />

#### Kullanıcı Detayı, Bloke ve Rol İşlemleri
<img width="100%" alt="Kullanıcı Detayı — Görsel 1" src="https://github.com/user-attachments/assets/795d98a5-ff1b-425d-a271-5f2041f0fdca" />
<img width="100%" alt="Kullanıcı Detayı — Görsel 2" src="https://github.com/user-attachments/assets/c5888397-0e3f-42c3-ba57-a56840517b27" />
<img width="100%" alt="Kullanıcı Detayı — Görsel 3" src="https://github.com/user-attachments/assets/ae98ad57-bdf2-4e58-a082-c404bbad7e71" />
<img width="100%" alt="Kullanıcı Detayı — Görsel 4" src="https://github.com/user-attachments/assets/adabb869-9ab3-422f-971c-66bcfcecf1f2" />
<img width="100%" alt="Kullanıcı Detayı — Görsel 5" src="https://github.com/user-attachments/assets/e49a5c5d-624d-49cb-90e9-87c853b2b272" />

> **Bloke:** Gerekçe ve kanıt dosyası zorunludur; bloke eden yönetici ve bütün bilgiler sisteme kaydedilir.

<img width="100%" alt="Bloke — Görsel 1" src="https://github.com/user-attachments/assets/deefcd06-5c9b-4845-90bf-4409f6ae847a" />
<img width="100%" alt="Bloke — Görsel 2" src="https://github.com/user-attachments/assets/2781d058-3735-497e-9393-70f96533eedf" />
<img width="100%" alt="Bloke — Görsel 3" src="https://github.com/user-attachments/assets/e9976ce9-86bc-4387-821e-4037be39bb4e" />

> **Bloke kaldırma:**

<img width="100%" alt="Bloke Kaldırma — Görsel 1" src="https://github.com/user-attachments/assets/68e38e02-8107-4b54-8c12-de0235c0b042" />

> **Bloke işlemi mailleri:**

<img width="677" alt="Bloke Maili — Görsel 1" src="https://github.com/user-attachments/assets/cd1fb9c3-e77a-4eef-8558-dacd3eab5b5a" />
<img width="675" alt="Bloke Maili — Görsel 2" src="https://github.com/user-attachments/assets/c05d3a77-d539-41f1-a04f-c8dfe51947c1" />

#### Roller
<img width="100%" alt="Roller — Görsel 1" src="https://github.com/user-attachments/assets/bb5ec705-0603-4b98-ae66-ec0beee4fcaf" />

#### Yazarlık Başvuruları
<img width="100%" alt="Yazarlık Başvuruları — Görsel 1" src="https://github.com/user-attachments/assets/e5840add-57f4-4c13-861d-604c5c336eb8" />
<img width="100%" alt="Yazarlık Başvuruları — Görsel 2" src="https://github.com/user-attachments/assets/a817a55f-bb93-4f72-ae08-e763d59d8d11" />
<img width="100%" alt="Yazarlık Başvuruları — Görsel 3" src="https://github.com/user-attachments/assets/3702efcd-f08c-4fdd-8fa9-09c67db40a62" />
<img width="100%" alt="Yazarlık Başvuruları — Görsel 4" src="https://github.com/user-attachments/assets/8bf223f7-dd5c-4f7c-b629-a5076abd01f4" />
<img width="100%" alt="Yazarlık Başvuruları — Görsel 5" src="https://github.com/user-attachments/assets/8fde3ce4-b5f9-4ad8-85be-c79300f0b152" />
<img width="100%" alt="Yazarlık Başvuruları — Görsel 6" src="https://github.com/user-attachments/assets/a77244a5-ae33-4bef-a390-0fd4aee842d3" />
<img width="100%" alt="Yazarlık Başvuruları — Görsel 7" src="https://github.com/user-attachments/assets/d22e3a63-0c03-4b46-b090-2936527ca64c" />
<img width="100%" alt="Yazarlık Başvuruları — Görsel 8" src="https://github.com/user-attachments/assets/d1aca37d-0845-4928-a1c5-52a9b1974f54" />

#### Yorumlar
<img width="100%" alt="Yorum Yönetimi — Görsel 1" src="https://github.com/user-attachments/assets/067b5945-22ef-42d5-958a-9e36df1e9d48" />

#### Hesabını Kapatmak İsteyen Kullanıcı
<img width="100%" alt="Hesap Kapatma Süreci — Görsel 1" src="https://github.com/user-attachments/assets/7fe9c1a1-7642-4e09-a003-b41acc526e5c" />

#### İletişimden Gelen Mesajlar
<img width="100%" alt="Gelen Mesajlar — Görsel 1" src="https://github.com/user-attachments/assets/b89e7865-6176-47d2-9bba-2ac553184ec5" />
<img width="100%" alt="Gelen Mesajlar — Görsel 2" src="https://github.com/user-attachments/assets/9de2ca22-82ff-40b7-8285-4ae03d5fcd63" />

#### Site İçeriği Yönetimi
<img width="100%" alt="Site İçeriği — Görsel 1" src="https://github.com/user-attachments/assets/75688a35-0bdc-4806-ad18-9e458ea4bf5f" />
<img width="100%" alt="Site İçeriği — Görsel 2" src="https://github.com/user-attachments/assets/ae3faa83-91f3-462e-ace4-ee472de1816b" />
<img width="100%" alt="Site İçeriği — Görsel 3" src="https://github.com/user-attachments/assets/97a59e68-27ee-4b39-aa36-1f79ceba73ca" />
<img width="100%" alt="Site İçeriği — Görsel 4" src="https://github.com/user-attachments/assets/05430570-137e-4564-9399-738eff21dc2a" />
<img width="100%" alt="Site İçeriği — Görsel 5" src="https://github.com/user-attachments/assets/fceddd4c-387a-4788-856f-39acaf368a22" />

#### Yöneticiye Özel Alan
> Yöneticinin kendi blogları, yorumları ve profili.

<img width="100%" alt="Yöneticiye Özel — Görsel 1" src="https://github.com/user-attachments/assets/2ce607fe-8c21-46f6-ac1a-39710488f9cc" />
<img width="100%" alt="Yöneticiye Özel — Görsel 2" src="https://github.com/user-attachments/assets/a7d56405-448b-46b7-9add-f6c8c1d17224" />
<img width="100%" alt="Yöneticiye Özel — Görsel 3" src="https://github.com/user-attachments/assets/3459a908-2dab-4bd7-9123-507558eb5c72" />
<img width="100%" alt="Yöneticiye Özel — Görsel 4" src="https://github.com/user-attachments/assets/5ba9f2c2-e12b-4cbd-b074-f40e9fb5c98b" />
<img width="100%" alt="Yöneticiye Özel — Görsel 5" src="https://github.com/user-attachments/assets/569c0f53-a16c-4781-b9b8-f7fb9c919555" />
<img width="100%" alt="Yöneticiye Özel — Görsel 6" src="https://github.com/user-attachments/assets/8b843ad1-9158-40de-bb47-1aaa3fd980be" />
<img width="100%" alt="Yöneticiye Özel — Görsel 7" src="https://github.com/user-attachments/assets/be6e9397-a588-47ff-93d0-04f87d4fa699" />

</details>

---

### 🔭 Log Kayıtları

<details>
<summary><b>Görselleri incelemek için açın</b></summary>

> Elasticsearch'teki loglar admin panelinde: özet kartlar, zaman grafiği, önemli olaylar, en yavaş uçlar, filtreler ve liste.

<img width="100%" alt="Log Kayıtları — Görsel 1" src="https://github.com/user-attachments/assets/393564f6-9645-42f0-8704-1b8279f1ec4b" />
<img width="100%" alt="Log Kayıtları — Görsel 2" src="https://github.com/user-attachments/assets/4655483c-7221-417c-a223-812c7a781bed" />
<img width="100%" alt="Log Kayıtları — Görsel 3" src="https://github.com/user-attachments/assets/4dc591b2-5ca8-405f-9ce0-f4d02bc25c67" />
<img width="100%" alt="Log Kayıtları — Görsel 4" src="https://github.com/user-attachments/assets/a4504d9b-24af-4426-a217-3015e30efeaf" />

#### Log Detayı
> Logun bütün alanları, varsa hata ve yığın izi, aynı isteğe ait bütün loglar.

<img width="100%" alt="Log Detayı — Görsel 1" src="https://github.com/user-attachments/assets/e3f5d38a-93a3-42da-a7b4-21719d395406" />
<img width="100%" alt="Log Detayı — Görsel 2" src="https://github.com/user-attachments/assets/8ca5a6ba-06c0-456e-85ee-5a4ac30e4e47" />
<img width="100%" alt="Log Detayı — Görsel 3" src="https://github.com/user-attachments/assets/46e689ad-725c-498c-838d-fa4975234ef4" />
<img width="100%" alt="Log Detayı — Görsel 4" src="https://github.com/user-attachments/assets/bbfee1a3-9a06-4eb5-b8e4-c1cb1f5b101a" />
<img width="100%" alt="Log Detayı — Görsel 5" src="https://github.com/user-attachments/assets/f6d48250-bc10-4d23-8636-3c5d3133f199" />
<img width="100%" alt="Log Detayı — Görsel 6" src="https://github.com/user-attachments/assets/2e363cdc-5844-4e58-a72e-4e88a4da70c0" />

</details>

---

### 📝 Yazar Paneli

<details>
<summary><b>Görselleri incelemek için açın</b></summary>

#### Genel Bakış
> Piksel tasarım, yazar seviyesi, okunma ve yorum grafiği.

<img width="100%" alt="Yazar Genel Bakış — Görsel 1" src="https://github.com/user-attachments/assets/0cab60f2-64f6-40bc-b0b1-a91f8b0d5a49" />
<img width="100%" alt="Yazar Genel Bakış — Görsel 2" src="https://github.com/user-attachments/assets/bd257e1e-f3f2-4dad-bc80-9dab746b740d" />

#### Bloglarım
<img width="100%" alt="Bloglarım — Görsel 1" src="https://github.com/user-attachments/assets/7d2294c6-066f-4c64-b348-58a8bba73d9a" />
<img width="100%" alt="Bloglarım — Görsel 2" src="https://github.com/user-attachments/assets/46b10344-7e36-4463-9c3c-b346cde700d0" />

#### Bloglarıma Gelen Yorumlar
<img width="100%" alt="Gelen Yorumlar — Görsel 1" src="https://github.com/user-attachments/assets/19b92dba-3d5c-421b-94d3-5ad832f357b1" />

#### Yorumlarım
<img width="100%" alt="Yorumlarım — Görsel 1" src="https://github.com/user-attachments/assets/8dd77940-8194-4755-80b3-11d0f09132b3" />

#### Profil ve Hesap Kapatma
<img width="100%" alt="Profil — Görsel 1" src="https://github.com/user-attachments/assets/a4e50d35-6c20-4b70-86b2-bdd0b7bb89aa" />
<img width="100%" alt="Profil — Görsel 2" src="https://github.com/user-attachments/assets/fb94386c-b672-4c0b-a638-7b1a5290336d" />
<img width="100%" alt="Profil — Görsel 3" src="https://github.com/user-attachments/assets/fd15fbbf-1583-46db-8838-aa160009d707" />
<img width="100%" alt="Profil — Görsel 4" src="https://github.com/user-attachments/assets/ff3383dd-c7e5-456c-9af6-51b24d25f1c6" />
<img width="100%" alt="Profil — Görsel 5" src="https://github.com/user-attachments/assets/766859b1-82c7-431b-8fe5-96ad9a9aba46" />

</details>

---

### 👤 Okur Paneli

<details>
<summary><b>Görselleri incelemek için açın</b></summary>

<img width="100%" alt="Okur Paneli — Görsel 1" src="https://github.com/user-attachments/assets/3f2a5de2-ce42-4baa-9e70-3d32eb78f04b" />
<img width="100%" alt="Okur Paneli — Görsel 2" src="https://github.com/user-attachments/assets/bf92fde7-58c5-4d13-ae0e-3eb3ca43d22e" />

#### Yazar Olmak İstiyorum
> Başvuru formu, taahhütname, başvuru durumu ve sonuç mailleri.

<img width="100%" alt="Yazar Ol — Görsel 1" src="https://github.com/user-attachments/assets/53d22892-e703-46ad-8967-b39cda484ebd" />
<img width="100%" alt="Yazar Ol — Görsel 2" src="https://github.com/user-attachments/assets/fea39d3b-50ae-4fbc-96c5-c9134d644db6" />
<img width="100%" alt="Yazar Ol — Görsel 3" src="https://github.com/user-attachments/assets/76da973f-d84f-4ae1-b4b3-bba2e05a0109" />
<img width="100%" alt="Yazar Ol — Görsel 4" src="https://github.com/user-attachments/assets/20f5e0a6-520d-40ab-bf5b-691dc8c138a2" />
<img width="100%" alt="Yazar Ol — Görsel 5" src="https://github.com/user-attachments/assets/12f24012-397f-450d-932d-cb94cb076a0e" />
<img width="100%" alt="Yazar Ol — Görsel 6" src="https://github.com/user-attachments/assets/1188fe07-a9dc-4004-89d6-e324370a7390" />
<img width="100%" alt="Yazar Ol — Görsel 7" src="https://github.com/user-attachments/assets/97ba3013-5ddb-4478-a7c3-29bfe1a0479c" />
<img width="690" alt="Yazar Ol — Görsel 8" src="https://github.com/user-attachments/assets/d80f5cb5-5157-4bfd-9a02-3ffcaca9de25" />
<img width="682" alt="Yazar Ol — Görsel 9" src="https://github.com/user-attachments/assets/17d21c57-e694-4d40-83ce-c807837d93d2" />

</details>

---

### 🧰 Geliştirme Ortamı

<details>
<summary><b>Görselleri incelemek için açın</b></summary>

#### Scalar (API Dokümantasyonu)
<img width="100%" alt="Scalar — Görsel 1" src="https://github.com/user-attachments/assets/a9707971-10dd-498b-90e8-977044be6923" />
<img width="100%" alt="Scalar — Görsel 2" src="https://github.com/user-attachments/assets/7e75ffe4-1d5d-4a7d-ae28-9e7c9febbf84" />

#### SQL Server
<img width="100%" alt="SQL Server — Görsel 1" src="https://github.com/user-attachments/assets/b43629c7-72e2-4209-a9a6-479adbcd9e18" />

#### Visual Studio Code
<img width="100%" alt="Visual Studio Code — Görsel 1" src="https://github.com/user-attachments/assets/17f0289b-7726-4962-a019-ef802e887303" />

#### Docker (Elasticsearch + Kibana)
<img width="100%" alt="Docker — Görsel 1" src="https://github.com/user-attachments/assets/c2eedaef-1497-4852-84ba-670f22f71b1e" />

</details>

---

## ⚙️ Kurulum

<details>
<summary><b>Gereksinimleri ve yerel kurulum adımlarını görmek için tıklayın</b></summary>

### Gereksinimler

- .NET 9 SDK ve EF Core araçları (`dotnet-ef`)
- SQL Server
- Node.js ve Angular CLI 20 (frontend için)
- Docker Desktop (isteğe bağlı: Elasticsearch ve Kibana için)
- Azure Storage hesabı, Gmail uygulama şifresi, Google OAuth istemci kimliği ve Cloudflare Turnstile anahtarı

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
