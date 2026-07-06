# PortfolioApp

ASP.NET Core MVC (.NET 10) ile geliştirilmiş, çok kiracılı (multi-tenant) portföy/CMS web uygulaması. Kayıt olan her kullanıcı ("kiracı") kendi handle'ı altında `/u/{handle}` adresinde bir portföy sitesine ve kendi içeriğiyle sınırlı bir `Admin` alanına sahip olur.

## Özellikler

- **Genel site** (`/u/{username}/...`): Ana sayfa (hero, hakkımda, yetenekler, deneyim, eğitim, sertifikalar, başarılar), proje vitrini (`/u/{username}/projeler/{slug}`), blog (`/u/{username}/blog/{slug}`), referanslar, iletişim formu (`/u/{username}/iletisim`)
- **Admin paneli**: Blog, proje, yetenek, deneyim, eğitim, sertifika, başarı, referans, sosyal medya, menü, footer, SEO, site ayarları, tema ve dil yönetimi; ziyaretçi analitiği ve dashboard — tamamı giriş yapan kullanıcının kendi içeriğiyle sınırlı
- **Kimlik doğrulama**: ASP.NET Identity ile rol tabanlı yetkilendirme (`RequireTenantUser` politikası tüm `Admin` alanını korur; `RequireSuperAdmin` platform geneli ekranları — örn. `Admin/Users` — korur)
- **Bakım modu**, tema değiştirme ve ziyaretçi takibi için özel middleware'ler — hepsi `username` route değerinden kiracıyı çözümleyip veriyi ona göre süzer
- Yeni kayıt olan her kullanıcı için varsayılan Hero/Hakkımda/İletişim/Site Ayarları/SEO Ayarları kayıtlarının otomatik oluşturulması (`IUserProvisioningService`)
- Eski bare route'lar (`/`, `/blog/{slug}`, `/projeler[/{slug}]`) yalnızca en eski kiracıya kalıcı yönlendirme olarak korunur (eski yer imleri/SEO için)
- Otomatik veritabanı oluşturma/güncelleme ve seed verisi ile uygulama açılışı

## Mimari

Katmanlı (n-tier) mimari; bağımlılık yönü **Web → Business → DataAccess → Entity**, `Core` ve `DTO` katmanları tüm katmanlar arasında paylaşılır. Servisler, controller'lara asla exception fırlatmaz; `Result` / `DataResult<T>` desenini kullanır.

| Katman | Sorumluluk |
|---|---|
| `PortfolioApp.Core` | Framework-bağımsız sözleşmeler: `BaseEntity`, repository/servis arayüzleri, `Result`/`DataResult`/`PaginatedResult`, enum'lar, sabitler (`AppConstants`), yardımcılar (`SlugHelper`, `PaginationHelper`, `FileHelper`) |
| `PortfolioApp.Entity` | EF Core entity sınıfları (`Concrete/`), Identity `ApplicationUser` dahil |
| `PortfolioApp.DTO` | Servis sınırı için `DTOs/` ve MVC için `ViewModels/` (`Public/` ve `Admin/`) |
| `PortfolioApp.DataAccess` | `PortfolioDbContext`, EF `Configurations/`, `Migrations/`, repository'ler, `UnitOfWork` |
| `PortfolioApp.Business` | `Services/` (arayüz + implementasyon), AutoMapper `Mappings/`, FluentValidation `Validators/` |
| `PortfolioApp.Web` | MVC controller'lar, `Admin` alanı, özel `Middleware/`, `Infrastructure/` (dosya yükleme, e-posta servisi) |

Her katman, kendi servislerini `Program.cs`'den çağrılan bir `AddXxx()` extension metoduyla (`AddDataAccess`, `AddBusiness`) kaydeder.

## Çoklu Kiracılık (Multi-tenancy)

İçerik entity'lerinin neredeyse tamamı (`Project`, `Hero`, `About`, `SiteSettings`, `SeoSettings` vb. — toplam 24 entity) `IUserOwnedEntity` arayüzünü uygular ve bir `UserId` taşır. Ayrı bir "tenant" tablosu yoktur; kiracının kendisi, `Handle` alanıyla herkese açık şekilde tanımlanan `ApplicationUser` satırıdır (`BlogPost` bu dönüşümden önce geldiği için istisnadır: `UserId` yerine mevcut `AuthorId` FK'siyle sahiplenilir; `BlogCategory`/`BlogTag` yine `UserId` kullanır).

- `ICurrentUserService`, giriş yapmış kullanıcının claim'lerini sarmalar; Admin tarafındaki servisler kayıt oluştururken `RequireUserId()` çağırır, okuma/güncelleme/silme işlemlerinde `.Where(x => x.UserId == _currentUser.UserId)` ile satır bazlı sahiplik kontrolü yapar. Bu kontrol merkezi bir repository/global query filter ile değil, her yeni servis metodunda elle uygulanır.
- Genel (public) route'lar URL'deki `{username}` ile kiracıya özeldir: `u/{username}`, `u/{username}/blog[/{slug}]`, `u/{username}/projeler[/{slug}]`, `u/{username}/iletisim`. Public controller'lar (`HomeController`, `BlogController`, `ProjectController`) `Handle == username && IsActive` ile ilgili `ApplicationUser`'ı bulur ve tüm sorguları o kullanıcının Id'sine göre süzer.
- `ThemeMiddleware`, `VisitorTrackingMiddleware` ve `MaintenanceModeMiddleware`, `username` route değerini okuyup `Handle` üzerinden bir `ownerId`'ye çözümledikten sonra kiracıya özel veriye (tema önbelleği, ziyaretçi logları, bakım modu bayrağı) erişir. `MaintenanceModeMiddleware`, bir kiracı kendi sitesinin sahibi olarak giriş yapmışsa bakım modu açık olsa bile onu içeri alır.
- `IUserProvisioningService.ProvisionDefaultsAsync`, yeni bir kiracının kayıt olduğunda ihtiyaç duyduğu varsayılan Hero/Hakkımda/İletişim/Site Ayarları/SEO Ayarları kayıtlarını oluşturur; her adım idempotent'tir.
- **Roller**: `AppConstants.Roles.User` normal kiracı/portföy sahibi rolüdür (`RequireTenantUser` politikası, tüm `Admin` alanını korur). `AppConstants.Roles.SuperAdmin` (`RequireSuperAdmin` politikası) platformlar arası ekranları korur — şu an yalnızca `Admin/Users` (tüm kullanıcıları listeleme/aktifleştirme/pasifleştirme). `AppConstants.Roles.Admin` seed verisinde hâlâ var ama artık hiçbir yerde kontrol edilmiyor; yeni yetkilendirmeler için kullanılmamalı.

## Teknoloji Yığını

- ASP.NET Core MVC (.NET 10)
- Entity Framework Core + SQL Server
- ASP.NET Identity
- AutoMapper, FluentValidation
- MailKit (SMTP e-posta), SixLabors.ImageSharp (görsel işleme)
- TinyMCE (zengin metin editörü)

## Başlangıç

### Gereksinimler

- .NET 10 SDK
- SQL Server (yerel veya uzak)

### Yapılandırma

`PortfolioApp.Web/appsettings.json` (veya `dotnet user-secrets`) içinde aşağıdaki alanları doldurun:

- `ConnectionStrings:DefaultConnection` — SQL Server bağlantı dizesi
- `AdminSettings` — açılışta seed edilecek admin kullanıcısının e-posta/şifre bilgisi
- `EmailSettings` — SMTP (MailKit) ayarları
- `FileUpload` — yükleme boyutu/uzantı limitleri ve `wwwroot/uploads` yolu
- `SiteSettings:MaintenanceMode` — bakım modu anahtarı
- `TinyMCE:ApiKey` — TinyMCE editör API anahtarı

### Çalıştırma

```bash
dotnet build
dotnet run --project PortfolioApp.Web
```

Veritabanı ve seed verisi (`DataSeeder.SeedAsync`) uygulama açılışında otomatik olarak oluşturulur/güncellenir.

### Migration'lar

`DbContext` `DataAccess` katmanında, DI ve bağlantı dizesi `Web` katmanında olduğu için hem `--project` hem `--startup-project` gereklidir:

```bash
dotnet ef migrations add <MigrationName> --project PortfolioApp.DataAccess --startup-project PortfolioApp.Web
dotnet ef database update --project PortfolioApp.DataAccess --startup-project PortfolioApp.Web
```

## Notlar

- Solution'da test projesi bulunmamaktadır.
- Kullanıcıya gösterilen servis/validasyon mesajları Türkçe'dir; yeni mesajlar bu tutarlılığı korumalıdır.
