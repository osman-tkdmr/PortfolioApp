using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioApp.Entity.Concrete;

namespace PortfolioApp.DataAccess.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasIndex(e => e.Slug).IsUnique();
        builder.Property(e => e.Title).HasMaxLength(250).IsRequired();
        builder.Property(e => e.Description).HasColumnType("nvarchar(max)");
        builder.Property(e => e.ShortDescription).HasMaxLength(500);

        builder.HasOne(e => e.ProjectCategory)
            .WithMany(c => c.Projects)
            .HasForeignKey(e => e.ProjectCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.Images)
            .WithOne(i => i.Project)
            .HasForeignKey(i => i.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
