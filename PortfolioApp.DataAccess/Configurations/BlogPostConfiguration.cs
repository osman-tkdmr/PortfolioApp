using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioApp.Entity.Concrete;

namespace PortfolioApp.DataAccess.Configurations;

public class BlogPostConfiguration : IEntityTypeConfiguration<BlogPost>
{
    public void Configure(EntityTypeBuilder<BlogPost> builder)
    {
        builder.HasIndex(e => e.Slug).IsUnique();

        builder.Property(e => e.Content).HasColumnType("nvarchar(max)");
        builder.Property(e => e.Summary).HasMaxLength(500);
        builder.Property(e => e.Title).HasMaxLength(250).IsRequired();

        builder.HasOne(e => e.BlogCategory)
            .WithMany(c => c.BlogPosts)
            .HasForeignKey(e => e.BlogCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Author)
            .WithMany(u => u.BlogPosts)
            .HasForeignKey(e => e.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.Comments)
            .WithOne(c => c.BlogPost)
            .HasForeignKey(c => c.BlogPostId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
