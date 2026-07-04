using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioApp.Entity.Concrete;

namespace PortfolioApp.DataAccess.Configurations;

public class ContactInfoConfiguration : IEntityTypeConfiguration<ContactInfo>
{
    public void Configure(EntityTypeBuilder<ContactInfo> builder)
    {
        builder.HasIndex(e => e.UserId).IsUnique();
    }
}
