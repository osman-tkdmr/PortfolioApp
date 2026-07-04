using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioApp.Entity.Concrete;

namespace PortfolioApp.DataAccess.Configurations;

public class ProjectImageConfiguration : IEntityTypeConfiguration<ProjectImage>
{
    public void Configure(EntityTypeBuilder<ProjectImage> builder)
    {
        builder.HasIndex(e => e.UserId);
    }
}
