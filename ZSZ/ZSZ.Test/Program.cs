using Autofac;
using BLLImplement;
using CaptchaGen;
using CodeCarvings.Piczard;
using CodeCarvings.Piczard.Filters.Watermarks;
using IBLL;
using log4net;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ZSZ.Common;
using ZSZ.CommonMVC;
using ZSZ.Service;

namespace ZSZ.Test
{
    class Program
    {
        public static int Add(int i, int j)
        {
            return i + j;
        }
        //Assert:断言
        public static void AsserrEqual(int value, int expectValue)
        {
            if (value != expectValue)
            {
                throw new Exception("两个值不一样!");
            }
        }
        static void Main(string[] args)
        {
            /*
            AsserrEqual(Add(1, 2), 3);//TestCase
            AsserrEqual(Add(0, 0), 0);
            AsserrEqual(Add(-1, 0), -1);
            */

            /*
            using (MyDbContext ctx = new MyDbContext())
            {
                ctx.Database.Delete();
                ctx.Database.Create();
            }
             Console.WriteLine("OK");
            Console.ReadKey();
            */

            //using System.Collections.Specialized;
            NameValueCollection nvc = new NameValueCollection();
            nvc["id"] = "5";
            nvc["age"] = "6";
            nvc["name"] = "kkk";
            Console.WriteLine(MVCHelper.ToQueryString(nvc));
            //id=5&age=6&name=kkk
            Console.WriteLine(MVCHelper.RemoveQueryString(nvc,"age"));
            Console.WriteLine(MVCHelper.RemoveQueryString(nvc, "aaa"));
            //id=5&age=6&name=kkk
            //id = 5 & name = kkk
            //id = 5 & age = 6 & name = kkk

            Console.WriteLine(MVCHelper.UpdateQueryString(nvc,"age","888"));
            Console.WriteLine(MVCHelper.UpdateQueryString(nvc, "vvv", "888"));
            //id = 5 & age = 888 & name = kkk
            //id = 5 & age = 6 & name = kkk & vvv = 888
            Console.ReadKey();
           
        }
        static void MainSecond(string[] args)
        {
            string userName = "康登凤";
            string appKey = "b7474dad139e63d4cfd1dd";
            string templateId = "360";
            string code = "0688";
            string phoneNum = "18021461160";
            /*
            //封装序列化
            WebClient wc = new WebClient();
            string url = "http://sms.rupeng.cn/SendSms.ashx?userName="+
                Uri.EscapeDataString(userName)+"&appKey="+Uri.EscapeDataString(appKey)+
                "&templateId="+templateId+"&code="+Uri.EscapeDataString(code)+
                "&phoneNum="+phoneNum;
            wc.Encoding = Encoding.UTF8;
            string resp = wc.DownloadString(url);
            //发出一个http请求（Get）返回值为响应报文体
            Console.WriteLine(resp);
            */
            ZSZSMSSender sender = new ZSZSMSSender();
            sender.AppKey = appKey;
            sender.UserName = userName;
            var result = sender.SendSMS(templateId, code, phoneNum);
            Console.WriteLine("返回码：" + result.code + "返回消息：" + result.msg);
            Console.WriteLine("OK");
            Console.ReadKey();
        }
        static void MainFirst(string[] args)
        {
            //专门用于测试的
            /*
            string s = CommonHelper.CreateVerifyCode(4);
            Console.WriteLine(s);*/
            //发送邮件
            /*
            using (MailMessage mailMessage = new MailMessage())
            using (SmtpClient smtpClient = new SmtpClient("smtp.163.com"))
            {
                mailMessage.To.Add("2432767038@qq.com");
                mailMessage.To.Add("3073611@qq.com");
                mailMessage.Body = "我是邮件的正文";
                mailMessage.From = new MailAddress("18021461160@163.com");
                mailMessage.Subject = "我是个邮件的标题";
                smtpClient.Credentials = new System.Net.NetworkCredential("18021461160@163.com", "159357kdf");
                //如果启用了“客户端授权码”，要用授权码代替密码
                //如果邮箱需要启用SSL，则必须设置smptClient.EnableSsl = true;
                smtpClient.Send(mailMessage);
            }
            */
            /*
            //缩略图
            ImageProcessingJob jobThumb = new ImageProcessingJob();
            jobThumb.Filters.Add(new FixedResizeConstraint(200, 200));
            jobThumb.SaveProcessedImageToFileSystem(@"E:\DemoMVC\img\1.jpg", @"E:\DemoMVC\img\2.jpg");
            
            */
            /*
            //水印
            ImageWatermark imgWatermark = new ImageWatermark(@"E:\DemoMVC\img\3.jpg");
            imgWatermark.ContentAlignment = System.Drawing.ContentAlignment.BottomRight;//水印位置
            imgWatermark.Alpha = 80;//透明度，需要水印图片是背景透明的 png 图片
            ImageProcessingJob jobNormal = new ImageProcessingJob();
            jobNormal.Filters.Add(imgWatermark);//添加水印
            //jobNormal.Filters.Add(new FixedResizeConstraint(600, 600));//限制图片的大小，避免生成 大图。如果想原图大小处理，就不用加这个 Filter
            jobNormal.SaveProcessedImageToFileSystem(@"E:\DemoMVC\img\1.jpg", @"E:\DemoMVC\img\4.jpg");
            */
            /*
            //验证码
            using (MemoryStream ms = ImageFactory.GenerateImage("AB12", 70, 120,30, 10))
                //把流保存到文件中
            using (FileStream fs = File.OpenWrite(@"E:\DemoMVC\img\yzm.jpg"))
            {
                ms.CopyTo(fs);
            }
            */
            /*
            //五、日志
            //首先从app.config中加载log4net的配置
            log4net.Config.XmlConfigurator.Configure();
            ILog logger = LogManager.GetLogger(typeof(Program));
            logger.Debug("这是个Debug");
            logger.Error("这是个error");
            logger.Warn("这是个warn"); 
            Console.WriteLine("OK");
            //记录异常对象
            try
            {
                SqlConnection conn = new SqlConnection();
                conn.Open();
            }
            catch (Exception ex)
            {

                logger.Error("连接数据库失败",ex);
            }
            */

            /*
            //常用定时任务框架
            IScheduler sched = new StdSchedulerFactory().GetScheduler();
            JobDetailImpl jdBossReport = new JobDetailImpl("jdTest", typeof(TestJob));
            //jdTest给任务起个名字    //第二个参数是执行任务的类的类名，该类必须实现IJob接口
            //创建一个Trigger
            IMutableTrigger triggerBossReport = CronScheduleBuilder.DailyAtHourAndMinute(18, 32).Build();
            //每天 23:45 执行一次
            triggerBossReport.Key = new TriggerKey("triggerTest");
            sched.ScheduleJob(jdBossReport, triggerBossReport);
            //计划者安排在triggerBossReport条件下执行jdBossReport
            //而jdBossReport是执行类TestJob中的execute方法
            sched.Start();
            //最后开始执行
            */
            /*
            {
                //定时模式
                IScheduler sched = new StdSchedulerFactory().GetScheduler();
                JobDetailImpl jdBossReport = new JobDetailImpl("jdTest", typeof(TestJob));
                CalendarIntervalScheduleBuilder builder = CalendarIntervalScheduleBuilder.Create();
                builder.WithInterval(3, IntervalUnit.Second);//每 3 秒钟执行一次
                IMutableTrigger triggerBossReport = builder.Build();
                triggerBossReport.Key = new TriggerKey("triggerTest");
                sched.ScheduleJob(jdBossReport, triggerBossReport);
                sched.Start();
            }

            //同时执行多个
            {
                IScheduler sched1 = new StdSchedulerFactory().GetScheduler();
                JobDetailImpl jdBossReport1 = new JobDetailImpl("jdTest1", typeof(IJobSecond));
                CalendarIntervalScheduleBuilder builder1 = CalendarIntervalScheduleBuilder.Create();
                builder1.WithInterval(3, IntervalUnit.Second);//每 3 秒钟执行一次
                IMutableTrigger triggerBossReport1 = builder1.Build();
                triggerBossReport1.Key = new TriggerKey("triggerTest1");
                sched1.ScheduleJob(jdBossReport1, triggerBossReport1);
                sched1.Start();
            }
            */

            /*
            //IOC
            //UserBLL bll = new UserBLL();//检查登录admin
            //IUserBll bll = new UserBLL();//检查登录admin
            ContainerBuilder builder = new ContainerBuilder();
            //把UserBLL注册为IUserBll的实现类
            //builder.RegisterType<UserBLL>().As<IUserBll>();
            //builder.RegisterType<DogBLL>().As<IDogBll>();
            builder.RegisterType<UserBLL>().AsImplementedInterfaces();
            builder.RegisterType<DogBLL>().AsImplementedInterfaces();
            IContainer containner = builder.Build();
            //创建IUserBll实现类的对象
            IUserBll bll = containner.Resolve<IUserBll>();
            bll.Check("admin", 123);
            //检查登录admin
            IDogBll dogbll = containner.Resolve<IDogBll>();
            dogbll.Back("旺财");
            //实现类所在的程序集名称
            */

            /*
            ContainerBuilder builder = new ContainerBuilder();
            //不用每个接口都要一个个注册
            Assembly asm = Assembly.Load("BLLImplement");//拿到实现类所在的程序集
            builder.RegisterAssemblyTypes(asm).AsImplementedInterfaces();

            IContainer containner = builder.Build();

            IUserBll userbll=containner.Resolve<IUserBll>();
            userbll.AddNew("admin",123);//addnewadmin

            IDogBll dogbll = containner.Resolve<IDogBll>();
            dogbll.Back("旺财");
            //不用管到底是哪个BLL了
            */

            /*
            //同时又两个实现类实现了同一个接口，UserBLL和UserBLL2
            //222admin
            //汪汪汪旺财
            //OK
            ContainerBuilder builder = new ContainerBuilder();
            Assembly asm = Assembly.Load("BLLImplement");//拿到实现类所在的程序集
            builder.RegisterAssemblyTypes(asm).AsImplementedInterfaces();
            IContainer containner = builder.Build();
            //拿到多个实现类
            IEnumerable<IUserBll> ubll = containner.Resolve<IEnumerable<IUserBll>>();
            foreach (IUserBll item in ubll)
            {
                Console.WriteLine(item.GetType());
                item.AddNew("admin",123);
                //两个实现类的AddNew都会被调用
            }
            */

            /*
            ContainerBuilder builder = new ContainerBuilder();
            Assembly asm = Assembly.Load("BLLImplement");//拿到实现类所在的程序集
            //builder.RegisterAssemblyTypes(asm).AsImplementedInterfaces();
            //2.PropertiesAutowired()自动注入实现类
            builder.RegisterAssemblyTypes(asm).AsImplementedInterfaces().PropertiesAutowired();
            IContainer containner = builder.Build();

            ISchool s = containner.Resolve<ISchool>();
            s.FangXue();
            //实现类中要实现别的接口（School中实现IDog接口）
            //1.声明一个接口类型的属性,在School实现类中
            //2.修改注册时候进行属性注入
            //3.在实现类中添加另一个接口的实现方法
            /*
                汪汪汪小狗
                放学啦！
                */

            /*
            //SingleInstance
            ContainerBuilder builder = new ContainerBuilder();
            Assembly asm = Assembly.Load("BLLImplement");//拿到实现类所在的程序集
            //builder.RegisterAssemblyTypes(asm).AsImplementedInterfaces();
            //2.PropertiesAutowired()自动注入实现类
            builder.RegisterAssemblyTypes(asm).AsImplementedInterfaces().PropertiesAutowired().SingleInstance();
            IContainer containner = builder.Build();

            ISchool s = containner.Resolve<ISchool>();//对象是AntuoFac创建出来的
            s.FangXue();
            ISchool s1 = containner.Resolve<ISchool>();
            s1.FangXue();
            Console.WriteLine(string.ReferenceEquals(s,s1));//true说明共享同一个对象
            */


            Console.WriteLine("OK");
            Console.ReadKey();
        }
    }
}
