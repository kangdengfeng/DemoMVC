using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSZ.DTO
{
    public class AdminUserDTO : BaseDTO
    {
        public String Name { get; set; }
        public String PhoneNum { get; set; }
        public String Email { get; set; }
        //cityid为空表示属于总部
        public long? CityId { get; set; }
        public String CityName { get; set; }
        public int LoginErrorTimes { get; set; }
        public DateTime? LastLoginErrorDateTime { get; set; }
        public long[] RoleId { get; set; }
        public String[] RoleName { get; set; }
    }
}
