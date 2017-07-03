using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSZ.Service.Entities;

namespace ZSZ.Service.Config
{
    class RegionConfig : EntityTypeConfiguration<RegionEntity>
    {
        public RegionConfig()
        {
            ToTable("T_Regions");
            this.HasRequired(e => e.City).WithMany().HasForeignKey(e => e.CityId).WillCascadeOnDelete(false);
            Property(p => p.Name).IsRequired().HasMaxLength(50);
        }
    }
}
