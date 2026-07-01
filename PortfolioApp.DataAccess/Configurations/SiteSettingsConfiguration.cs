using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioApp.Entity.Concrete;

namespace PortfolioApp.DataAccess.Configurations;

public class SiteSettingsConfiguration : IEntityTypeConfiguration<SiteSettings>
{
    public void Configure(EntityTypeBuilder<SiteSettings> builder)
    {
        builder.Property(e => e.SiteName).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Language).HasMaxLength(10).HasDefaultValue("tr");

        builder.HasOne(e => e.ActiveTheme)
            .WithMany()
            .HasForeignKey(e => e.ActiveThemeId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
