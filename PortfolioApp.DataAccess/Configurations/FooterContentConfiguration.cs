using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioApp.Entity.Concrete;

namespace PortfolioApp.DataAccess.Configurations;

public class FooterContentConfiguration : IEntityTypeConfiguration<FooterContent>
{
    public void Configure(EntityTypeBuilder<FooterContent> builder)
    {
        builder.HasIndex(e => e.UserId);
    }
}
