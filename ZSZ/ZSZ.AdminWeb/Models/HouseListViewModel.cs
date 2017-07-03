using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZSZ.DTO;

namespace ZSZ.AdminWeb.Models
{
    public class HouseListViewModel
    {
        public HouseDTO House { get; set; }
        public HousePicDTO[] Pics { get; set; }
        public AttachementDTO[] Attachments { get; set; }
    }
}