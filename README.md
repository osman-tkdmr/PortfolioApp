# PortfolioApp

ASP.NET Core MVC (.NET 10) ile geliştirilmiş, kişisel portföy/CMS web uygulaması. Genel ziyaretçilere açık bir vitrin sitesi ile içerik yönetimi için bir `Admin` alanı içerir.

## Özellikler

- **Genel site**: Ana sayfa (hero, hakkımda, yetenekler, deneyim, eğitim, sertifikalar, başarılar), proje vitrini (`/projeler/{slug}`), blog (`/blog/{slug}`), referanslar, iletişim formu
- **Admin paneli**: Blog, proje, yetenek, deneyim, eğitim, sertifika, başarı, referans, sosyal medya, menü, footer, SEO, site ayarları, tema ve dil yönetimi; ziyaretçi analitiği ve dashboard
- **Kimlik doğrulama**: ASP.NET Identity ile rol tabanlı yetkilendirme (Admin alanı korumalı)
- **Bakım modu**, tema değiştirme ve ziyaretçi takibi için özel middleware'ler
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
