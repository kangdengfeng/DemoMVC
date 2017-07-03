﻿using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSZ.Service.Entities;

namespace ZSZ.Service.Config
{
    class HousepicConfig : EntityTypeConfiguration<HousepicEntity>
    {
        public HousepicConfig()
        {
            ToTable("T_HousePics");
            //HousePicConfig和AdminLogConfig不一样的地方：如果有反向导航属性，则要HousePicConfig的写法
            //否则就是AdminLogConfig的写法
            HasRequired(e => e.House).WithMany(e => e.HousePics).HasForeignKey(e => e.HouseId);
            Property(p => p.ThumbUrl).IsRequired().HasMaxLength(1024);
            Property(p => p.Url).IsRequired().HasMaxLength(1024);
        }
    }
}
