using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using PortfolioApp.Business.Extensions;
using PortfolioApp.Core.Constants;
using PortfolioApp.DataAccess.Context;
using PortfolioApp.DataAccess.Extensions;
using PortfolioApp.DataAccess.SeedData;
using PortfolioApp.Entity.Concrete;
using PortfolioApp.Web;
using PortfolioApp.Web.Infrastructure;
using PortfolioApp.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ── Database ───────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<PortfolioDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Identity ───────────────────────────────────────────────────────────────────
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<PortfolioDbContext>()
    .AddDefaultTokenProviders();

// ── Authorization policies ─────────────────────────────────────────────────────
// Role checks alone were "any Admin can manage everything" — policies let us
// name the intent (coarse feature gate) separately from per-row ownership,
// which stays a service-layer concern (see ICurrentUserService usage).
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthorizationPolicies.RequireTenantUser, policy =>
        policy.RequireRole(AppConstants.Roles.User));

    options.AddPolicy(AuthorizationPolicies.RequireSuperAdmin, policy =>
        policy.RequireRole(AppConstants.Roles.SuperAdmin));
});

// Admin views send the antiforgery token via this header on fetch()-based AJAX calls
// (form-based posts still use the default __RequestVerificationToken form field).
builder.Services.AddAntiforgery(options => options.HeaderName = "RequestVerificationToken");

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// ── Application Services ───────────────────────────────────────────────────────
builder.Services.AddHttpContextAccessor();
builder.Services.AddDataAccess();
builder.Services.AddBusiness();

// ── Infrastructure ─────────────────────────────────────────────────────────────
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// ── Session ────────────────────────────────────────────────────────────────────
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ── Rate limiting ──────────────────────────────────────────────────────────────
// Partitioned per client IP so one abusive caller can't exhaust the limit for everyone else.
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy(RateLimitPolicies.Login, httpContext => RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        }));

    options.AddPolicy(RateLimitPolicies.Contact, httpContext => RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 3,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        }));

    options.AddPolicy(RateLimitPolicies.AccountAction, httpContext => RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        }));
});

// ── MVC ────────────────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization(options =>
        options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(SharedResource)));

// ── Localization ───────────────────────────────────────────────────────────────
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

var supportedCultures = new[] { "tr-TR", "en-US" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
localizationOptions.RequestCultureProviders = new List<IRequestCultureProvider>
{
    new CookieRequestCultureProvider(),
    new SiteSettingsRequestCultureProvider()
};

var app = builder.Build();

// ── Seed data ──────────────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    await DataSeeder.SeedAsync(scope.ServiceProvider);
}

// ── Error handling ─────────────────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseMiddleware<SecurityHeadersMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseRateLimiter();

app.UseRequestLocalization(localizationOptions);

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

// Custom middleware — order matters
app.UseMiddleware<MaintenanceModeMiddleware>();
app.UseMiddleware<ThemeMiddleware>();
app.UseMiddleware<VisitorTrackingMiddleware>();

// ── Routes ─────────────────────────────────────────────────────────────────────
app.MapAreaControllerRoute(
    name: "admin",
    areaName: "Admin",
    pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}");

// Per-tenant public portfolio routes — canonical for every user, including the
// legacy migrated tenant (see the redirect shims below for its old bare URLs).
app.MapControllerRoute(
    name: "publicBlogDetail",
    pattern: "u/{username}/blog/{slug}",
    defaults: new { controller = "Blog", action = "Detail" });

app.MapControllerRoute(
    name: "publicBlog",
    pattern: "u/{username}/blog",
    defaults: new { controller = "Blog", action = "Index" });

app.MapControllerRoute(
    name: "publicProjectDetail",
    pattern: "u/{username}/projeler/{slug}",
    defaults: new { controller = "Project", action = "Detail" });

app.MapControllerRoute(
    name: "publicProjects",
    pattern: "u/{username}/projeler",
    defaults: new { controller = "Project", action = "Index" });

app.MapControllerRoute(
    name: "publicContact",
    pattern: "u/{username}/iletisim",
    defaults: new { controller = "Home", action = "Contact" });

app.MapControllerRoute(
    name: "publicProfile",
    pattern: "u/{username}",
    defaults: new { controller = "Home", action = "Index" });

// Legacy bare routes (pre-multi-tenancy) — kept only as permanent redirects to the
// earliest-created (migrated) tenant's new /u/{handle}/... URLs, for SEO/bookmarks.
// New tenants never get bare routes; they only ever exist at /u/{username}/....
// The site root has no tenant-neutral landing page yet, so it redirects the same way.
app.MapGet("/", async (PortfolioDbContext db) =>
{
    var earliest = await db.Users.OrderBy(u => u.CreatedAt).FirstOrDefaultAsync();
    return earliest is null ? Results.NotFound() : Results.Redirect($"/u/{earliest.Handle}", permanent: true);
});

app.MapGet("/blog/{slug}", async (string slug, PortfolioDbContext db) =>
{
    var earliest = await db.Users.OrderBy(u => u.CreatedAt).FirstOrDefaultAsync();
    return earliest is null ? Results.NotFound() : Results.Redirect($"/u/{earliest.Handle}/blog/{slug}", permanent: true);
});

app.MapGet("/projeler/{slug}", async (string slug, PortfolioDbContext db) =>
{
    var earliest = await db.Users.OrderBy(u => u.CreatedAt).FirstOrDefaultAsync();
    return earliest is null ? Results.NotFound() : Results.Redirect($"/u/{earliest.Handle}/projeler/{slug}", permanent: true);
});

app.MapGet("/projeler", async (PortfolioDbContext db) =>
{
    var earliest = await db.Users.OrderBy(u => u.CreatedAt).FirstOrDefaultAsync();
    return earliest is null ? Results.NotFound() : Results.Redirect($"/u/{earliest.Handle}/projeler", permanent: true);
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
