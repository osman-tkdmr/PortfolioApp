using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioApp.Entity.Concrete;

namespace PortfolioApp.DataAccess.Configurations;

public class SeoSettingsConfiguration : IEntityTypeConfiguration<SeoSettings>
{
    public void Configure(EntityTypeBuilder<SeoSettings> builder)
    {
        builder.HasIndex(e => new { e.UserId, e.PageSlug }).IsUnique();
        builder.Property(e => e.PageSlug).HasMaxLength(100).IsRequired();
        builder.Property(e => e.MetaTitle).HasMaxLength(200).IsRequired();
        builder.Property(e => e.MetaDescription).HasMaxLength(500);
        builder.Property(e => e.MetaKeywords).HasMaxLength(500);
    }
}
