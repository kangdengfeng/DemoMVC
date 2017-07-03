using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSZ.Service.Entities;

namespace ZSZ.Service.Config
{
    class PermissionConfig:EntityTypeConfiguration<PermissionEntity>
    {
        public PermissionConfig()
        {
            this.ToTable("T_Permissions");
            this.HasMany(e=>e.Roles).WithMany(e=>e.Perminssions)
                .Map(m => m.ToTable("T_RolePermissions")
                .MapLeftKey("PerminssionId").MapRightKey("RoleId"));
            Property(p => p.Description).IsOptional().HasMaxLength(1024);
            Property(p => p.Name).IsRequired().HasMaxLength(50);
        }
    }
}
