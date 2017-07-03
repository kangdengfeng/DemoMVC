using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZSZ.DTO;

namespace ZSZ.AdminWeb.Models
{
    public class AdminUserEditViewModel
    {
        public AdminUserDTO AdminUser { get; set; }
        public CityDTO[] Cities { get; set; }
        public RoleDTO[] Roles { get; set; }
        //Role在页面中既要用到Id，还要显示Name
        /// <summary>
        /// 当前用户拥有的角色id
        /// </summary>
        public long[] UserRoleIds { get; set; }
        //只是在下拉列表中使用UserRoleId，如果用DTO还要select().Contains(r=>r.Id)

    }
}