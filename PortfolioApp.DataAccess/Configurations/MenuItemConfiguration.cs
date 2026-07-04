using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioApp.Entity.Concrete;

namespace PortfolioApp.DataAccess.Configurations;

public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.HasOne(e => e.ParentMenuItem)
            .WithMany(m => m.Children)
            .HasForeignKey(e => e.ParentMenuItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.UserId);
        builder.Property(e => e.Title).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Url).HasMaxLength(500);
    }
}
