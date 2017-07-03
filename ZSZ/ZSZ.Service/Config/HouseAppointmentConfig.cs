using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSZ.Service.Entities;

namespace ZSZ.Service.Config
{
    class HouseAppointmentConfig : EntityTypeConfiguration<HouseAppointmentEntity>
    {
        public HouseAppointmentConfig()
        {
            this.ToTable("T_HouseAppointments");
            HasOptional(e => e.User).WithMany().HasForeignKey(e => e.UserId).WillCascadeOnDelete(false);
            HasRequired(e => e.House).WithMany().HasForeignKey(h => h.HouseId).WillCascadeOnDelete(false);
            HasOptional(h => h.FollowAdminUser).WithMany().HasForeignKey(h => h.FollowAdminUserId).WillCascadeOnDelete(false);
            Property(h => h.Name).IsRequired().HasMaxLength(20);
            Property(h => h.PhoneNum).IsRequired().HasMaxLength(20).IsUnicode(false);
            Property(h => h.Status).IsRequired().HasMaxLength(20);
            Property(h => h.RowVersion).IsRequired().IsRowVersion();
        }
    }
}
