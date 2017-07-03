using Autofac;
using Autofac.Integration.Mvc;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ZSZ.AdminWeb.App_Start;
using ZSZ.AdminWeb.Jobs;
using ZSZ.CommonMVC;
using ZSZ.IService;

namespace ZSZ.AdminWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //log4net
            log4net.Config.XmlConfigurator.Configure();

            /*
            //通过FilterConfig一起注册，这样可以在Global中少些代码
            GlobalFilters.Filters.Add(new JsonNetActionFilter());
            GlobalFilters.Filters.Add(new ZSZExceptionFilter());
            GlobalFilters.Filters.Add(new ZSZAuthorizeFilter());
            */
            FilterConfig.RegisterFilters(GlobalFilters.Filters);


            //去空格、全角转半角
            ModelBinders.Binders.Add(typeof(string),new TrimToDBCModelBinder());
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
                            &&typeof(IServiceSupport).IsAssignableFrom(type))
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

            startQuartz();


        }
        private void startQuartz()
        {
            IScheduler sched = new StdSchedulerFactory().GetScheduler();


            //每个Job对应中间这一块

            //发送报表Job开始
            JobDetailImpl jdBossReport = new JobDetailImpl("jbBossReport", typeof(BossReportJob));
            //jdTest给任务起个名字    //第二个参数是执行任务的类的类名，该类必须实现IJob接口
            //创建一个Trigger
            IMutableTrigger triggerBossReport = CronScheduleBuilder.DailyAtHourAndMinute(10, 55).Build();
            //每天 23:45 执行一次
            triggerBossReport.Key = new TriggerKey("triggerTest");
            sched.ScheduleJob(jdBossReport, triggerBossReport);
            //计划者安排在triggerBossReport条件下执行jdBossReport
            //而jdBossReport是执行类TestJob中的execute方法
            //发送报表Job结束


            sched.Start();
            //最后开始执行

        }
    }
}
