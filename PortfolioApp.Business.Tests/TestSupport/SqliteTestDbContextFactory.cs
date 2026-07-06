using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PortfolioApp.DataAccess.Context;

namespace PortfolioApp.Business.Tests.TestSupport;

public sealed class SqliteTestDbContextFactory : IDisposable
{
    private readonly SqliteConnection _connection;

    public SqliteTestDbContextFactory()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        using var context = CreateContext();
        context.Database.EnsureCreated();
    }

    public PortfolioDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PortfolioDbContext>()
            .UseSqlite(_connection)
            .Options;
        return new TestPortfolioDbContext(options);
    }

    public void Dispose() => _connection.Dispose();
}
