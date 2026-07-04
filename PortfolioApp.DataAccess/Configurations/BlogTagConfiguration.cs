using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioApp.Entity.Concrete;

namespace PortfolioApp.DataAccess.Configurations;

public class BlogTagConfiguration : IEntityTypeConfiguration<BlogTag>
{
    public void Configure(EntityTypeBuilder<BlogTag> builder)
    {
        builder.HasIndex(e => e.UserId);
    }
}
