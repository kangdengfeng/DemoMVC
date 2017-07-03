using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestIService;

namespace WebApplication2
{
    public class Person
    {
        //
        public IUserService UserService { get; set; }
        public void Hello()
        {
            UserService.CheckLogin("admin","123");
            //抛异常，UserService不是AutoFac创建出来的
        }
    }
}