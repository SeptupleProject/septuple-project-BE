using Enterprise_Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enterprise_Web.Configuration
{

    public class ReactionConfiguration : IEntityTypeConfiguration<Reaction>
    {
        public void Configure(EntityTypeBuilder<Reaction> builder)
        {
            builder.ToTable("Reactions");
            builder.HasKey(x => new { x.IdeaId, x.UserId });
            builder.Property(x => x.Like).IsRequired(false);
            builder.Property(x => x.Dislike).IsRequired(false);
            builder.HasOne(u => u.User).WithMany(r => r.Reactions)
                .HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(i => i.Idea).WithMany(r => r.Reactions)
                .HasForeignKey(r => r.IdeaId).OnDelete(DeleteBehavior.NoAction);
        }
    }    
}
