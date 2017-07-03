using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace 锁
{
    class MyDbContext : DbContext
    {
        public MyDbContext() : base("name=connstr")
        {

        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            Assembly asm = Assembly.GetExecutingAssembly();
            modelBuilder.Configurations.AddFromAssembly(asm);
            /*
            modelBuilder.Entity<Girl>().ToTable("T_Girls");
            modelBuilder.Entity<Girl>().Property(g => g.rowver).IsRowVersion();
            */

        }
        public DbSet<Girl> Girls { get; set; }
    }
}
