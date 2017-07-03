using IBLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLLImplement
{
    public class UserBLL : IUserBll
    {
        public void AddNew(string name, int pwd)
        {
            Console.WriteLine("AddNew"+name);
        }

        public bool Check(string name, int pwd)
        {
            Console.WriteLine("检查登录" + name);
            return true;
        }
    }
}
