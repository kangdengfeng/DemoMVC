using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSZ.Service.Entities;

namespace ZSZ.Service.Config
{
    class AttachmentConfig : EntityTypeConfiguration<AttachmentEntity>
    {
        public AttachmentConfig()
        {
            this.ToTable("T_Attchments");
            this.HasMany(e => e.Houses).WithMany(e => e.Attachments).Map(m => m.ToTable("T_HouseAttachments")
            .MapLeftKey("AttchmentsId").MapRightKey("HouseId"));

            this.Property(e=>e.IconName).IsRequired().HasMaxLength(50).IsUnicode(false);
            this.Property(e => e.Name).IsRequired().HasMaxLength(50);

        }
    }
}
