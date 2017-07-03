using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZSZ.AdminWeb.App_Start
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]//表示这个Attribute可以标注到方法上,而且可以添加多个 
    public class CheckHasPermissionAttribute:Attribute
    {
        
            public string Permission { get; set; }
            public CheckHasPermissionAttribute(string permission)
            {
                this.Permission = permission;
            }
    }
}