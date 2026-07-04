using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioApp.Entity.Concrete;

namespace PortfolioApp.DataAccess.Configurations;

public class VisitorLogConfiguration : IEntityTypeConfiguration<VisitorLog>
{
    public void Configure(EntityTypeBuilder<VisitorLog> builder)
    {
        builder.HasIndex(e => e.VisitedAt);
        builder.HasIndex(e => new { e.VisitedAt, e.IsBot });
        builder.HasIndex(e => new { e.UserId, e.VisitedAt });

        builder.Property(e => e.IpAddress).HasMaxLength(45);
        builder.Property(e => e.PageUrl).HasMaxLength(500);
        builder.Property(e => e.Referrer).HasMaxLength(500);
        builder.Property(e => e.UserAgent).HasMaxLength(500);
        builder.Property(e => e.SessionId).HasMaxLength(100);
    }
}
