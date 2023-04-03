using EnterpriseWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseWeb.Configuration
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable("Comments");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.Content).IsRequired(true).HasMaxLength(1000);
            builder.Property(x => x.IsAnonymous).IsRequired(true);
            builder.Property(x => x.CreatedBy).IsRequired(false).HasMaxLength(50);
            builder.Property(x => x.CreatedAt).IsRequired(true);
            builder.HasOne(x => x.Idea).WithMany(x => x.Comments)
             .HasForeignKey(x => x.IdeaId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.User).WithMany(x => x.Comments)
              .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
