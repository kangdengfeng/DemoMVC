using Autofac;
using Autofac.Integration.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ZSZ.CommonMVC;
using ZSZ.FrontWeb.App_Start;
using ZSZ.IService;

namespace ZSZ.FrontWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure();
            GlobalFilters.Filters.Add(new ZSZExceptionFilter());

            //去空格、全角转半角
            //去空格、全角转半角
            ModelBinders.Binders.Add(typeof(string), new TrimToDBCModelBinder());
            ModelBinders.Binders.Add(typeof(int), new TrimToDBCModelBinder());
            ModelBinders.Binders.Add(typeof(double), new TrimToDBCModelBinder());
            ModelBinders.Binders.Add(typeof(long), new TrimToDBCModelBinder());

            //两行默认配置
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly).PropertiesAutowired();
            //把当前 程序集中的 Controller 都注册
            //不要忘了.PropertiesAutowired()
            // 获取所有相关类库的程序集
            /*
            Assembly[] assemblies = new Assembly[] { Assembly.Load("ZSZ.Service") };
            builder.RegisterAssemblyTypes(assemblies)
            .Where(type => !type.IsAbstract)
            .AsImplementedInterfaces().PropertiesAutowired();
            */
            //只加载Service中的非抽象类
            //Service中有些类不希望注册到AutoFac中，不需要通过AutoFac来获得对象
            //修改注册对象
            Assembly[] assemblies = new Assembly[] { Assembly.Load("ZSZ.Service") };
            builder.RegisterAssemblyTypes(assemblies)
            .Where(type => !type.IsAbstract
                            && typeof(IServiceSupport).IsAssignableFrom(type))
            .AsImplementedInterfaces().PropertiesAutowired();
            //type1.IsAssignableFrom(type2);
            //type1类型的变量是否可以指向type2类型的对象
            //type1类型的变量可以从type2类型赋值过来
            //接口：//表示type2是否实现了IServiceSupport接口
            //类： //type2是否继承自type1
            //typeof(IServiceSupport).IsAssignableFrom(type))只注册了IserviceSupport接口的实现类
            //避免其他无关的类注册到AutoFac中
            //AsImplementedInterfaces()所有接口都注册
            //.PropertiesAutowired()所有属性自动植入


            var container = builder.Build();
            //注册系统级别的 DependencyResolver，这样当 MVC 框架创建 Controller 等对象的时候都是管 Autofac 要对象。
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));//!!!

            GlobalFilters.Filters.Add(new JsonNetActionFilter());
        }
    }
}
