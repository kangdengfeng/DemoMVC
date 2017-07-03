using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestIService;
using TestService;

namespace WebApplication2.Controllers
{
    public class DefaultController : Controller
    {
        //Global中注入了TestService
        //.PropertiesAutowired();会自动给属性赋值
        //当前程序集中的Controller都注册
        public IUserService UserService { get; set; }
        ////获得Controller的属性
        // GET: Default
        public ActionResult Index()
        {
            /*
            bool b = UserService.CheckLogin("admin", "123");
            return Content(b.ToString());
            //没有使用AutoFac的方法
            //IUserService ius = new UserService();
            //bool b = ius.CheckLogin("admin", "123");
            //return Content(b.ToString());
            */

            /*
            bool b = UserService.CheckLogin("abc", "123");
            Helper.Test();
              //return Content(b.ToString());
            */


            //此例只是证明只有AutoFac创建的对象才能自动给属性赋值
            //PropertiesAutowired();
            //注册类之后对象不能直接new
            //Person p1 = new Person();
            Person p1 = DependencyResolver.Current.GetService<Person>();
            p1.Hello();
            return Content("OK");
        }
        [HttpGet]
        public ActionResult TestJson()
        {
            return View();
        }

        [HttpPost]
        public ActionResult TestJson(FormCollection fc)
        {
            Dog dog = new Dog() { BirthDate = DateTime.Now, Id = 5, Name = "小黑" };
            //return Json(dog);//反回的是：{"Id":5,"Name":"小黑","BirthDate":"\/Date(1498399205775)\/"}
            //return new JsonNetResult() { Data = dog };//1. JsonNetResul返回//{"id":5,"name":"小黑","birthDate":"2017-06-25 22:03:57"}
             //还用原来的return Json
            return Json(dog);//2.Filter

        }


    }
}