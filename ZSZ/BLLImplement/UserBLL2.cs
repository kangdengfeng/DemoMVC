using IBLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLLImplement
{
    public class UserBLL2:IUserBll
    {
        public void AddNew(string name, int pwd)
        {
            Console.WriteLine("222" + name);
        }

        public bool Check(string name, int pwd)
        {
            Console.WriteLine("222" + name);
            return true;
        }
    }
}
