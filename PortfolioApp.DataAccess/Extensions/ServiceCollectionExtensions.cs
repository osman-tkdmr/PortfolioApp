using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PortfolioApp.Core.Entities;
using PortfolioApp.Core.Interfaces.Repositories;
using PortfolioApp.DataAccess.Context;
using PortfolioApp.DataAccess.Repositories.Concrete;
using PortfolioApp.DataAccess.UnitOfWork;

namespace PortfolioApp.DataAccess.Extensions;

public static class ServiceCollectionExtensions
{
    // Called from Web layer after DbContext is already registered
    public static IServiceCollection AddDataAccess(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
        services.AddScoped<BlogPostRepository>();
        services.AddScoped<ProjectRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

        return services;
    }

    // Convenience overload that also registers DbContext
    public static IServiceCollection AddDataAccess(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<PortfolioDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
                sql.EnableRetryOnFailure(3)));

        return services.AddDataAccess();
    }
}
