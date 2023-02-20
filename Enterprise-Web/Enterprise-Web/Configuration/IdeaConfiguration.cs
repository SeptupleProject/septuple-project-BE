using Enterprise_Web.Models;
using EnterpriseWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enterprise_Web.Configuration
{
    public class IdeaConfiguration : IEntityTypeConfiguration<Idea>
    {
        public void Configure(EntityTypeBuilder<Idea> builder)
        {
            builder.ToTable("Ideas");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.Title).IsRequired(true).HasMaxLength(100);
            builder.Property(x => x.Content).IsRequired(true).HasMaxLength(1000);
            builder.Property(x => x.Views).IsRequired(true);
            builder.Property(x => x.Image).IsRequired(true).HasMaxLength(255);
            builder.Property(x => x.CreatedBy).IsRequired(true).HasMaxLength(50);
            builder.Property(x => x.IsAnonymos).IsRequired(true);
            builder.Property(x => x.AcademicYearId).IsRequired(true);
            builder.Property(x => x.CreatedAt).IsRequired(true);
            builder.Property(x => x.CategoryId).IsRequired(true);

            builder.HasOne(x => x.AcademicYear).WithMany(x => x.Ideas)
                .HasForeignKey(x => x.AcademicYearId).OnDelete(DeleteBehavior.Cascade); 
            builder.HasOne(x => x.Category).WithMany(x => x.Ideas)
                .HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Cascade);    

        }
    }
}
