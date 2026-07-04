using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioApp.Entity.Concrete;

namespace PortfolioApp.DataAccess.Configurations;

public class BlogPostTagConfiguration : IEntityTypeConfiguration<BlogPostTag>
{
    public void Configure(EntityTypeBuilder<BlogPostTag> builder)
    {
        builder.HasKey(e => new { e.BlogPostId, e.BlogTagId });

        builder.HasOne(e => e.BlogPost)
            .WithMany(p => p.BlogPostTags)
            .HasForeignKey(e => e.BlogPostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.BlogTag)
            .WithMany(t => t.BlogPostTags)
            .HasForeignKey(e => e.BlogTagId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(e => !e.BlogPost.IsDeleted);

        builder.ToTable("BlogPostTags");
    }
}
