using EnterpriseWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enterprise_Web.Configuration
{
  public class CategoryConfiguration : IEntityTypeConfiguration<Category>
  {
    public void Configure(EntityTypeBuilder<Category> builder)
    {
      builder.ToTable("Category");
      builder.HasKey(x => x.Id);
      builder.Property(x => x.Id).UseIdentityColumn();
      builder.Property(x => x.Name).IsRequired(true).HasMaxLength(100);
      builder.Property(x => x.CreatedBy).IsRequired(false).HasMaxLength(50);
      builder.Property(x => x.CreatedAt).IsRequired(true);
    }
  }
}
