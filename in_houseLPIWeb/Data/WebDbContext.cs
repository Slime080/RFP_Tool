using in_houseLPIWeb.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace in_houseLPIWeb.Data
{
    public class WebDbContext : DbContext
    {
        internal readonly object TypesOfCharges;

        public WebDbContext(DbContextOptions<WebDbContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        //public DbSet<Login> Logins { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<rfpForm> rfpForms { get; set; }
        public DbSet<PoPList> PoP { get; set; }
        public DbSet<Payee> Payees { get; set; }
        public DbSet<TOC> TOCs { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Entity> Entities { get; set; }
        public DbSet<StoreType> StoreTypes { get; set; }
        //public DbSet<ifcView> ifcViews { get; set; }

        public DbSet<Permission> Permissions { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<CDJ_Extraction> CDJ_Extraction { get; set; }
        public DbSet<RfpCombinedData> RfpCombinedData { get; set; } // Add this line


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RfpCombinedData>()
                .HasNoKey(); // This specifies that RfpCombinedData is a keyless entity
        }
    }
}
