using IBLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLLImplement
{
    public class School : ISchool
    {
        //1.调实现类,由AutoFac自动赋值合适的实现类
        //如果有多个实现了申明玮IEnumerable即可
        public IDogBll DogBll { get; set; }
        public void FangXue()
        {
            //3.
            DogBll.Back("小狗");
            //DogBll{BLLImplement.DogBLL}
            //属性的自动注入
            Console.WriteLine("放学啦！");
        }
    }
}
