using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSZ.Service.Entities;

namespace ZSZ.Service.Config
{
    class CitiesConfig : EntityTypeConfiguration<CitiesEntity>
    {
        public CitiesConfig()
        {
            ToTable("T_Cities");
            Property(e => e.Name).IsRequired().HasMaxLength(20);
        }
    }
}
