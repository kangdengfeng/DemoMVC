using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZSZ.DTO;

namespace ZSZ.FrontWeb.Models
{
    [Serializable]
    //其他关联的类也要标记为可序列化
    //HouseDTO、HousePicDTO、AttachementDTO、BaseDTO都要标记为可序列化
    public class HouseIndexViewModel
    {
        public HouseDTO House { get; set; }
        public HousePicDTO[] Pics { get; set; }
        public AttachementDTO[] Attachments { get; set; }
    }
}