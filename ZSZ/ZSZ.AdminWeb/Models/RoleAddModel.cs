using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ZSZ.AdminWeb.Models
{
    public class RoleAddModel
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        //一个用户角色有多个权限
        public long[] PermissionIds { get; set; }
    }
}