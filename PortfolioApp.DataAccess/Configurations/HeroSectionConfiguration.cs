using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioApp.Entity.Concrete;

namespace PortfolioApp.DataAccess.Configurations;

public class HeroSectionConfiguration : IEntityTypeConfiguration<HeroSection>
{
    public void Configure(EntityTypeBuilder<HeroSection> builder)
    {
        builder.HasIndex(e => e.UserId).IsUnique();
    }
}
