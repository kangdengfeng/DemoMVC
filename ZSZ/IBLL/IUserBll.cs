using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBLL
{
    public interface IUserBll
    {
        bool Check(string name,int pwd);
        void AddNew(string name,int pwd);
    }
}
