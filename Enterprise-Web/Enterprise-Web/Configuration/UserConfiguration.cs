using EnterpriseWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseWeb.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.Email).IsRequired(true).HasMaxLength(50);
            builder.Property(x => x.Password).IsRequired(true).HasMaxLength(200);
            builder.Property(x => x.Role).IsRequired(true).HasMaxLength(100);

            builder.HasOne(x => x.Department).WithMany(x => x.Users)
                .HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
