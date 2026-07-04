using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioApp.Entity.Concrete;

namespace PortfolioApp.DataAccess.Configurations;

public class ProjectCategoryConfiguration : IEntityTypeConfiguration<ProjectCategory>
{
    public void Configure(EntityTypeBuilder<ProjectCategory> builder)
    {
        builder.HasIndex(e => e.UserId);
    }
}
