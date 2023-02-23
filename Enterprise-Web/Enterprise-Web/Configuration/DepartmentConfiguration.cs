using EnterpriseWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseWeb.Configuration
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("Departments");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.Name).IsRequired(true).HasMaxLength(100);
            builder.Property(x => x.CreatedBy).IsRequired(false).HasMaxLength(50);
            builder.Property(x => x.CreatedAt).IsRequired(true);
            builder.HasMany(x => x.Users).WithOne(x => x.Department).OnDelete(DeleteBehavior.SetNull);
        }
    }
}
