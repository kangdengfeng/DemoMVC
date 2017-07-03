using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestIService;

namespace WebApplication2
{
    public class Helper
    {
        public static void Test()
        {
            //快速注入,获得接口实现类
            //Global中已经给Service注册了
            IUserService svc = DependencyResolver.Current.GetService<IUserService>();
            bool b = svc.CheckLogin("","");
        }
    }
}