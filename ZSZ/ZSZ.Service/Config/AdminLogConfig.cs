using System.Data.Entity.ModelConfiguration;
using ZSZ.Service.Entities;

namespace ZSZ.Service.Config
{
    class AdminLogConfig : EntityTypeConfiguration<AdminLogEntity>
    {
       public AdminLogConfig()
        {
            ToTable("T_AdminLogs");
            this.HasRequired(e => e.AdminUser).WithMany().HasForeignKey(e => e.AdminUserId)
                .WillCascadeOnDelete(false);
            this.Property(e => e.Msg).IsRequired();
        }
    }
}
