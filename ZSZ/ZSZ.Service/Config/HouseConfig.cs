using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSZ.Service.Entities;

namespace ZSZ.Service.Config
{
    class HouseConfig : EntityTypeConfiguration<HouseEntity>
    {
        public HouseConfig()
        {
            this.ToTable("T_Houses");
            HasRequired(e => e.Communitie).WithMany().HasForeignKey(e => e.CommunityId).WillCascadeOnDelete(false);
            HasRequired(e => e.RoomType).WithMany().HasForeignKey(e => e.RoomTypeId).WillCascadeOnDelete();
            HasRequired(e => e.Status).WithMany().HasForeignKey(e => e.StatusId).WillCascadeOnDelete(false);
            HasRequired(e => e.DecorateStatus).WithMany().HasForeignKey(e => e.DecorateStatusId).WillCascadeOnDelete(false);
            HasRequired(e => e.Type).WithMany().HasForeignKey(e => e.TypeId).WillCascadeOnDelete(false);
            Property(h => h.Address).IsRequired().HasMaxLength(128);
            Property(h => h.Description).IsOptional();
            Property(h => h.Direction).IsRequired().HasMaxLength(20);
            Property(h => h.OwnerName).IsRequired().HasMaxLength(20);
            Property(h => h.OwnerPhoneNum).IsRequired().HasMaxLength(20).IsUnicode(false);
        }
    }
}
