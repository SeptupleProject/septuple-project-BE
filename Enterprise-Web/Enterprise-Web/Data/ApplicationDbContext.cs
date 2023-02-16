using Enterprise_Web.Configuration;
using Enterprise_Web.Models;
using EnterpriseWeb.Configuration;
using EnterpriseWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseWeb.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new AcademicYearConfiguration());
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<AcademicYear> AcademicYears { get; set; }
    }
}
