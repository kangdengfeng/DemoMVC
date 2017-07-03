using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace WebApplication1
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // 在应用程序启动时运行的代码
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //{ 
            ////定时执行的代码
            //IScheduler sched = new StdSchedulerFactory().GetScheduler();
            //JobDetailImpl jdBossReport = new JobDetailImpl("jdTest", typeof(TestJob));
            ////jdTest给任务起个名字    //第二个参数是执行任务的类的类名，该类必须实现IJob接口
            ////创建一个Trigger
            //IMutableTrigger triggerBossReport = CronScheduleBuilder.DailyAtHourAndMinute(19, 55).Build();
            ////每天 18:32执行一次
            //triggerBossReport.Key = new TriggerKey("triggerTest");
            //sched.ScheduleJob(jdBossReport, triggerBossReport);
            ////计划者安排在triggerBossReport条件下执行jdBossReport
            ////而jdBossReport是执行类TestJob中的execute方法
            //sched.Start();
            //    //最后开始执行
            //}
        }
    }
}