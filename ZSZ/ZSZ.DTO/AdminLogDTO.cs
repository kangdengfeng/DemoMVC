using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSZ.DTO
{
    public class AdminLogDTO : BaseDTO
    {
        public long AdminUserId { get; set; }
        public String Message { get; set; }
        //要获取管理员表中的管理员姓名和手机号，扁平化设计，不能搞关联属性
        public String AdminUserName { get; set; }
        public String AdminUserPhoneNum { get; set; }
    }
}
