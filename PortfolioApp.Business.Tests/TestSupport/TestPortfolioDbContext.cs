using Microsoft.EntityFrameworkCore;
using PortfolioApp.DataAccess.Context;

namespace PortfolioApp.Business.Tests.TestSupport;

/// <summary>
/// SQLite can't parse SQL-Server-only column type strings (e.g. "nvarchar(max)", configured in
/// ProjectConfiguration/BlogPostConfiguration/EducationConfiguration for the real SQL Server provider) —
/// strip them after the real model is built so the same entity/relationship/index model runs against SQLite in tests.
/// </summary>
public sealed class TestPortfolioDbContext : PortfolioDbContext
{
    public TestPortfolioDbContext(DbContextOptions<PortfolioDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            foreach (var property in entityType.GetProperties())
                if (property.GetColumnType() is not null)
                    property.SetColumnType(null);
    }
}
