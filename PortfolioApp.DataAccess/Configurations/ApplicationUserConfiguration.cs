using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioApp.Entity.Concrete;

namespace PortfolioApp.DataAccess.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(e => e.Handle).HasMaxLength(50).IsRequired();
        builder.HasIndex(e => e.Handle).IsUnique();
    }
}
