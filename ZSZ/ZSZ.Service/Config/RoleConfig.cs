using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSZ.Service.Entities;

namespace ZSZ.Service.Config
{
    class RoleConfig : EntityTypeConfiguration<RoleEntity>
    {
        public RoleConfig()
        {
            ToTable("T_Roles");
            Property(p => p.Name).IsRequired().HasMaxLength(50);
        }
    }
}
