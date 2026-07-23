# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

Multi-tenant portfolio/CMS web application built with ASP.NET Core MVC (**.NET 10**), EF Core + PostgreSQL, and ASP.NET Identity. Each registered user (a "tenant") gets their own portfolio site at `/u/{handle}` plus an `Admin` area scoped to their own content. User-facing messages (service results, validation) are written in **Turkish** — keep new messages consistent with that.

## Commands

```bash
# Build / run (run targets the Web startup project)
dotnet build
dotnet run --project PortfolioApp.Web

# EF Core migrations — DbContext lives in DataAccess, DI/connection string in Web,
# so both --project and --startup-project are required
dotnet ef migrations add <Name> --project PortfolioApp.DataAccess --startup-project PortfolioApp.Web
dotnet ef database update --project PortfolioApp.DataAccess --startup-project PortfolioApp.Web
```

There is no test project in the solution. The database is created/updated and seeded automatically at startup via `DataSeeder.SeedAsync` (called from `Program.cs`); the admin user is seeded from the `AdminSettings` config section.

## Architecture

N-tier, wired top-to-bottom in `PortfolioApp.Web/Program.cs`. Dependency direction: **Web → Business → DataAccess → Entity**, with `Core` (abstractions/utilities) and `DTO` shared across layers.

- **PortfolioApp.Core** — framework-agnostic contracts: `BaseEntity`, repository/service interfaces, the `Result`/`DataResult`/`PaginatedResult` types, enums, constants (`AppConstants`, incl. `Roles`), and utilities (`SlugHelper`, `PaginationHelper`, `FileHelper`).
- **PortfolioApp.Entity** — EF entity classes (`Concrete/`), including the Identity `ApplicationUser`.
- **PortfolioApp.DTO** — `DTOs/` (service boundary) and `ViewModels/` (`Public/` and `Admin/`). Keep these separate from entities.
- **PortfolioApp.DataAccess** — `PortfolioDbContext`, EF `Configurations/` (auto-applied via `ApplyConfigurationsFromAssembly`), `Migrations/`, repositories, and `UnitOfWork`.
- **PortfolioApp.Business** — `Services/` (Interfaces + Concrete), AutoMapper `Mappings/` profiles, FluentValidation `Validators/`.
- **PortfolioApp.Web** — MVC controllers, the `Admin` area, custom `Middleware/`, and `Infrastructure/` (`FileUploadService`, `EmailService`).

Each layer registers its own services via an `AddXxx()` extension (`AddDataAccess`, `AddBusiness`) called from `Program.cs`.

## Multi-tenancy

Almost every content entity (`Project`, `Hero`, `About`, `SiteSettings`, `SeoSettings`, etc. — 24 entities total) implements `IUserOwnedEntity` (`Core/Entities/IUserOwnedEntity.cs`) and carries a `UserId`. There is no separate "tenant" table — the tenant *is* the `ApplicationUser` row, identified publicly by its unique `Handle`. **Exception**: `BlogPost` predates the conversion and is owned via its existing `AuthorId` FK instead, not `IUserOwnedEntity`/`UserId` — `BlogCategory`/`BlogTag` still use `UserId` as usual.

- **`ICurrentUserService`** (`Core/Interfaces/ICurrentUserService.cs`, impl in `Business/Security/CurrentUserService.cs`) wraps the logged-in user's claims. Admin-side services call `_currentUser.RequireUserId()` when creating rows and filter reads/updates/deletes with `.Where(x => x.UserId == _currentUser.UserId)` — this per-row ownership check is a repeated service-layer idiom, not something a repository base class or global EF query filter enforces centrally, so it must be applied by hand in every new owned-entity service method (see `ProjectService`, `PortfolioServices.cs`). Public-facing "get all for this tenant" reads instead take an explicit `ownerId` parameter, since there's no `ICurrentUserService.UserId` on unauthenticated public requests.
- **Public routes are tenant-scoped by `{username}` in the URL**: `u/{username}`, `u/{username}/blog[/{slug}]`, `u/{username}/projeler[/{slug}]`, `u/{username}/iletisim` (see `Program.cs`). Public controllers (`HomeController`, `BlogController`, `ProjectController`) resolve the owning `ApplicationUser` via `_userManager.Users.FirstOrDefault(u => u.Handle == username && u.IsActive)` and scope all queries to that user's Id — there is no tenant-neutral landing page. A handful of legacy bare routes (`/`, `/blog/{slug}`, `/projeler[/{slug}]`) are kept only as permanent redirects to the earliest-created tenant, for old bookmarks/SEO.
- **`ThemeMiddleware`, `VisitorTrackingMiddleware`, `MaintenanceModeMiddleware`** all read the `username` route value (`context.GetRouteValue("username")`) and resolve it to an `ownerId` via `Handle` before touching tenant data (per-tenant theme cache key, per-tenant visitor logs, per-tenant maintenance flag). `MaintenanceModeMiddleware` additionally lets a tenant who is logged in as the owner of that specific site through even while its maintenance mode is on, so an owner is never locked out of their own portfolio.
- **`IUserProvisioningService`** (`ProvisionDefaultsAsync`, in `Business/Services/Concrete/UserProvisioningService.cs`) creates the default Hero/About/ContactInfo/SiteSettings/SeoSettings rows a new tenant needs on registration; each step is idempotent.
- **Roles**: `AppConstants.Roles.User` is the normal tenant/portfolio-owner role (policy `RequireTenantUser`, gates the whole `Admin` area via `AdminBaseController`). `AppConstants.Roles.SuperAdmin` (policy `RequireSuperAdmin`) gates cross-tenant/platform screens — currently just `Admin/Users` (list/activate/deactivate any user; deliberately not a full admin-of-admins panel). `AppConstants.Roles.Admin` still exists in the seed data but is no longer checked anywhere; don't rely on it for new authorization.

## Key conventions

**Result pattern** — services return `IResult` / `IDataResult<T>`, never throw for expected failures. Build them with `Result.Ok(msg)` / `Result.Fail(msg)` and `DataResult<T>.Ok(data)` / `DataResult<T>.Fail(msg)`. Controllers branch on `.Success`.

**Data access via UnitOfWork** — services take a `UnitOfWork` (note: the concrete class, registered scoped — not always `IUnitOfWork`). Use `_uow.GetRepository<T>()` for generic CRUD; specialized repos are exposed as typed properties (`_uow.BlogPosts`, `_uow.Projects`) for entity-specific queries. `_uow.Context` gives raw `DbContext` access (used for join entities like `BlogPostTag`). Always call `await _uow.SaveChangesAsync()` after mutations.

**Repositories** — `GenericRepository<T>` reads with `AsNoTracking()`; `Update` re-attaches and marks Modified. Add entity-specific repositories (e.g. `BlogPostRepository`) for custom queries and register them in `DataAccess`'s `ServiceCollectionExtensions` + expose as a `UnitOfWork` property.

**Soft delete** — never hard-delete `BaseEntity` rows. Use `SoftDeleteAsync(id)` (sets `IsDeleted`/`DeletedAt`); a global EF query filter in `OnModelCreating` excludes deleted rows automatically. `CreatedAt`/`UpdatedAt` are stamped centrally in the overridden `SaveChangesAsync` — do not set them manually.

**Mapping & validation** — map entity↔DTO with AutoMapper profiles (registered by assembly scan). Validate create/update DTOs with FluentValidation inside the service before mutating; return `Result.Fail` with joined error messages on failure.

**CancellationToken** — `IRepository<T>` and `IUnitOfWork` methods all accept a trailing `CancellationToken cancellationToken = default`; thread it through when adding new repository/service methods.

**Rich text** — any HTML coming from TinyMCE fields (blog body, about, etc.) must go through `RichTextSanitizer` (`Business/Security/`, backed by `HtmlSanitizer`) before being persisted or rendered.

**Admin controllers** — inherit `AdminBaseController` (`[Area("Admin")]` + `[Authorize(Policy = AuthorizationPolicies.RequireTenantUser)]` + `[AutoValidateAntiforgeryToken]`). Use its helpers: `CurrentUserId`, `Success`/`Error` (set `TempData`) and `JsonOk`/`JsonFail` for AJAX responses. Controllers that must be platform-wide instead of per-tenant (e.g. `UsersController`) add `[Authorize(Policy = AuthorizationPolicies.RequireSuperAdmin)]` on top.

**Middleware order matters** (`Program.cs`): `MaintenanceMode` → `Theme` → `VisitorTracking`, all after auth/session/routing so `Theme`/`VisitorTracking` can read the `username` route value. See [Multi-tenancy](#multi-tenancy) for the per-tenant public route patterns.

## Configuration (`appsettings.json`)

`ConnectionStrings:DefaultConnection` (PostgreSQL), `AdminSettings` (seeded admin credentials), `EmailSettings` (MailKit SMTP), `FileUpload` (size/extension limits, `wwwroot/uploads` path), `SiteSettings:MaintenanceMode`, and `TinyMCE:ApiKey` (rich-text editor in Admin).
