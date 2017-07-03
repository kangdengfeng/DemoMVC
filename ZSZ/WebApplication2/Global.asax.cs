using Autofac;
using Autofac.Integration.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApplication2
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalFilters.Filters.Add(new JsonNetActionFilter());

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);



            var builder = new ContainerBuilder();

            //给Controller注入
            //using Autofac.Integration.Mvc;
            //因为RegisterControllers是扩展方法
            builder.RegisterControllers(typeof(MvcApplication).Assembly).PropertiesAutowired();
            //把当前程序集(MvcApplication)即WebApplication2所在的程序集中的 Controller 都注册到AutoFac中
            //PropertiesAutowired();在Controller中的属性就会被自动注入

            //给Service注册
            Assembly asmService = Assembly.Load("TestService");//获得程序集
            builder.RegisterAssemblyTypes(asmService).Where(t => !t.IsAbstract).AsImplementedInterfaces().PropertiesAutowired();
            //当前程序集中的Controller都注册


            //给当前程序集中所有类注册（不要随便给类注册到AutoFac），并把属性自动注入
            //builder.RegisterAssemblyTypes(typeof(MvcApplication).Assembly).PropertiesAutowired();

            var container = builder.Build();
            //注册系统级别的 DependencyResolver，这样当 MVC 框架创建 Controller 等对象的时候都是管 Autofac 要对象。
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));//!!!


        }
    }
}
