# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

Portfolio/CMS web application built with ASP.NET Core MVC (**.NET 10**), EF Core + SQL Server, and ASP.NET Identity. It has a public-facing site and an `Admin` area for content management. User-facing messages (service results, validation) are written in **Turkish** — keep new messages consistent with that.

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

## Key conventions

**Result pattern** — services return `IResult` / `IDataResult<T>`, never throw for expected failures. Build them with `Result.Ok(msg)` / `Result.Fail(msg)` and `DataResult<T>.Ok(data)` / `DataResult<T>.Fail(msg)`. Controllers branch on `.Success`.

**Data access via UnitOfWork** — services take a `UnitOfWork` (note: the concrete class, registered scoped — not always `IUnitOfWork`). Use `_uow.GetRepository<T>()` for generic CRUD; specialized repos are exposed as typed properties (`_uow.BlogPosts`, `_uow.Projects`) for entity-specific queries. `_uow.Context` gives raw `DbContext` access (used for join entities like `BlogPostTag`). Always call `await _uow.SaveChangesAsync()` after mutations.

**Repositories** — `GenericRepository<T>` reads with `AsNoTracking()`; `Update` re-attaches and marks Modified. Add entity-specific repositories (e.g. `BlogPostRepository`) for custom queries and register them in `DataAccess`'s `ServiceCollectionExtensions` + expose as a `UnitOfWork` property.

**Soft delete** — never hard-delete `BaseEntity` rows. Use `SoftDeleteAsync(id)` (sets `IsDeleted`/`DeletedAt`); a global EF query filter in `OnModelCreating` excludes deleted rows automatically. `CreatedAt`/`UpdatedAt` are stamped centrally in the overridden `SaveChangesAsync` — do not set them manually.

**Mapping & validation** — map entity↔DTO with AutoMapper profiles (registered by assembly scan). Validate create/update DTOs with FluentValidation inside the service before mutating; return `Result.Fail` with joined error messages on failure.

**Admin controllers** — inherit `AdminBaseController` (`[Area("Admin")]` + `[Authorize(Roles = AppConstants.Roles.Admin)]`). Use its helpers: `Success`/`Error` (set `TempData`) and `JsonOk`/`JsonFail` for AJAX responses.

**Middleware order matters** (`Program.cs`): `MaintenanceMode` → `Theme` → `VisitorTracking`, all after auth/session. Custom routes exist for `blog/{slug}` and `projeler/{slug}` (Turkish "projects") slug detail pages.

## Configuration (`appsettings.json`)

`ConnectionStrings:DefaultConnection` (SQL Server), `AdminSettings` (seeded admin credentials), `EmailSettings` (MailKit SMTP), `FileUpload` (size/extension limits, `wwwroot/uploads` path), and `SiteSettings:MaintenanceMode`.
