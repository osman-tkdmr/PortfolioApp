using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioApp.Entity.Concrete;

namespace PortfolioApp.DataAccess.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasOne(e => e.ParentComment)
            .WithMany(c => c.Replies)
            .HasForeignKey(e => e.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.BlogPostId);
        builder.HasIndex(e => e.UserId);
        builder.Property(e => e.Content).HasMaxLength(2000).IsRequired();
        builder.Property(e => e.AuthorName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.AuthorEmail).HasMaxLength(200).IsRequired();
    }
}
