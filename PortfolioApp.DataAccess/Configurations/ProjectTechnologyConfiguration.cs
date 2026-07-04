using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioApp.Entity.Concrete;

namespace PortfolioApp.DataAccess.Configurations;

public class ProjectTechnologyConfiguration : IEntityTypeConfiguration<ProjectTechnology>
{
    public void Configure(EntityTypeBuilder<ProjectTechnology> builder)
    {
        builder.HasKey(e => new { e.ProjectId, e.TechnologyId });

        builder.HasOne(e => e.Project)
            .WithMany(p => p.ProjectTechnologies)
            .HasForeignKey(e => e.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Technology)
            .WithMany(t => t.ProjectTechnologies)
            .HasForeignKey(e => e.TechnologyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(e => !e.Project.IsDeleted);

        builder.ToTable("ProjectTechnologies");
    }
}
