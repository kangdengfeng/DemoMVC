using log4net;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ZSZ.Service.Entities;

namespace ZSZ.Service
{
    public class MyDbContext : DbContext
    {
        private static ILog log = LogManager.GetLogger(typeof(DbContext));
        public MyDbContext() : base("name=connstr1")
        {
            //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<MyDbContext>());
            //测试完毕开始禁用数据库自动生成
            Database.SetInitializer<MyDbContext>(null);
            //加上this表示Database的属性，否则代表的是一个类
            this.Database.Log = (sql) => {
                log.DebugFormat("EF执行SQL：{0}",sql);
                //如果配置中不输出这个级别的时候，就不会进行字符串拼接，提升性能。
            };  
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Configurations.AddFromAssembly(Assembly.GetExecutingAssembly());
        }
        public DbSet<AdminLogEntity> AdminLogs { get; set; }
        public DbSet<AdminUserEntity> AdminUsers { get; set; }
        public DbSet<AttachmentEntity> Attachments { get; set; }
        public DbSet<CitiesEntity> Cities { get; set; }
        public DbSet<CommunitieEntity> Communities { get; set; }
        public DbSet<HouseAppointmentEntity> HouseAppointments { get; set; }
        public DbSet<HouseEntity> Houses { get; set; }
        public DbSet<HousepicEntity> Housepics { get; set; }
        public DbSet<IdNameEntity> IdNames { get; set; }
        public DbSet<PermissionEntity> Permissions { get; set; }
        public DbSet<RegionEntity> Regions { get; set; }
        public DbSet<RoleEntity> Roles { get; set; }
        public DbSet<SettingEntity> Settings { get; set; }
        public DbSet<UserEntity> Users { get; set; }

    }
}
