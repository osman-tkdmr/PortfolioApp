using PortfolioApp.DataAccess.Context;
using PortfolioApp.Entity.Concrete;

namespace PortfolioApp.Business.Tests.TestSupport;

public static class ApplicationUserSeeder
{
    public static async Task<ApplicationUser> SeedAsync(PortfolioDbContext context, string userId, string handle)
    {
        var user = new ApplicationUser { Id = userId, UserName = $"{handle}@test.local", Handle = handle };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }
}
