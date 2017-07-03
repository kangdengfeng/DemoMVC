using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSZ.Service.Entities;

namespace ZSZ.Service.Config
{
    class CommunitieConfig : EntityTypeConfiguration<CommunitieEntity>
    {
        public CommunitieConfig()
        {
            this.ToTable("T_Communities");
            //Community必须有Region，一个Region有很多的Community，他们使用RegionId做外键
            this.HasRequired(e => e.Region).WithMany().HasForeignKey(e => e.RegionId).WillCascadeOnDelete(false);
            this.Property(e => e.Location).HasMaxLength(1024).IsRequired();
            Property(e => e.Name).HasMaxLength(20).IsRequired();
        }
    }
}
