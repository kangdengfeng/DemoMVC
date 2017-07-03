using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZSZ.DTO;

namespace ZSZ.FrontWeb.Models
{
    public class HouseSearchViewModel
    {
        public RegionDTO[] Regions { get; set; }

        //搜索结果，搜索出来的房子信息
        public HouseDTO[] houses { get; set; }
    }
}