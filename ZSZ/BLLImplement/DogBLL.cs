using IBLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLLImplement
{
    public class DogBLL : IDogBll
    {
        public void Back(string name)
        {
            Console.WriteLine("汪汪汪"+name);
        }
    }
}
