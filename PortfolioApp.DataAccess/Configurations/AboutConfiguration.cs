using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioApp.Entity.Concrete;

namespace PortfolioApp.DataAccess.Configurations;

public class AboutConfiguration : IEntityTypeConfiguration<About>
{
    public void Configure(EntityTypeBuilder<About> builder)
    {
        builder.HasIndex(e => e.UserId).IsUnique();
    }
}
