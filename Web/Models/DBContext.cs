using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class DBContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            //"Data Source=47.111.173.207,5566;Initial Catalog=KB;User Id =sa;Password=ghds12369..DB;MultipleActiveResultSets=true"
            //"Server=(localdb)\\MyLocaldb; Database=TMT;Integrated Security=true;MultipleActiveResultSets=true"
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Initial Catalog=TMT;Integrated Security=true;MultipleActiveResultSets=true");
        }
        protected override void OnModelCreating(ModelBuilder Builder)
        {
            Builder.Entity<TMT_Users>().HasKey(c => c.UserID);
            Builder.Entity<TMT_Users>().HasQueryFilter(c => !c.IsDelete);

            Builder.Entity<TMT_Roles>().HasKey(c => c.RoleID);
            Builder.Entity<TMT_Roles>().HasQueryFilter(c => !c.IsDelete);

            Builder.Entity<TMT_Projects>().HasKey(c => c.ProjectID);
            Builder.Entity<TMT_Projects>().HasQueryFilter(c => !c.IsDelete);

            Builder.Entity<TMT_Modules>().HasKey(c => c.ModuleID);
            Builder.Entity<TMT_Modules>().HasQueryFilter(c => !c.IsDelete);

            Builder.Entity<TMT_Documents>().HasKey(c => c.DocumentID);
            Builder.Entity<TMT_Documents>().HasQueryFilter(c => !c.IsDelete);

            Builder.Entity<TMT_Requirements>().HasKey(c => c.RequirementID);
            Builder.Entity<TMT_Requirements>().HasQueryFilter(c => !c.IsDelete);

            Builder.Entity<TMT_Requirements_Detaile>().HasKey(c => c.DetaileID);

            Builder.Entity<TMT_Logs>().HasKey(c => c.LogID);
            Builder.Entity<TMT_Logs>().HasQueryFilter(c => !c.IsDelete);
        }
        public DbSet<TMT_Users> TMT_Users { get; set; }
        public DbSet<TMT_Roles> TMT_Roles { get; set; }
        public DbSet<TMT_Projects> TMT_Projects { get; set; }
        public DbSet<TMT_Modules> TMT_Modules { get; set; }
        public DbSet<TMT_Documents> TMT_Documents { get; set; }

        public DbSet<TMT_Requirements> TMT_Requirements { get; set; }
        public DbSet<TMT_Requirements_Detaile> TMT_Requirements_Detaile { get; set; }
        public DbSet<TMT_Logs> TMT_Logs { get; set; }

    }
}