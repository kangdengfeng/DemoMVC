using Autofac;
using Autofac.Integration.Mvc;
using log4net;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using ZSZ.IService;

namespace ZSZ.AdminWeb.Jobs
{
    public class BossReportJob : IJob
    {
        private static ILog log
            = LogManager.GetLogger(typeof(BossReportJob));
        public void Execute(IJobExecutionContext context)
        {
            log.Debug("准备收集今日房源数量");
            //Job是运行在单独的线程中，如果在这里面发生异常不会记录，调试也没用
            //需要单独try catch然后记录到日志中
            try
            {
                string bossEmails;//老板邮箱
                string smtpServer, smtpUserName, smtpPwd, smtpEmail;
                StringBuilder sbMsg = new StringBuilder();
                //要获得所有城市和房源信息
                //但是这是单独的线程，无法使用AutoFac获得对象
                var container = AutofacDependencyResolver.Current.ApplicationContainer;
                using (container.BeginLifetimeScope())
                {
                    var cityService = container.Resolve<ICityService>();
                    var houseService = container.Resolve<IHouseService>();


                    //获取Settiongs,用于发送邮件
                    var settingService = container.Resolve<ISettingService>();
                    bossEmails = settingService.GetValue("Boss邮箱");//读取配置中的多个邮箱
                    smtpServer = settingService.GetValue("SmtpServer");
                    smtpUserName = settingService.GetValue("SmtpUserName");
                    smtpPwd = settingService.GetValue("Smtp授权码");
                    smtpEmail = settingService.GetValue("SmtpAddress");

                    foreach (var city in cityService.GetAll())
                    {
                        long count = houseService.GetTodayNewHouseCount(city.Id);
                        sbMsg.Append(city.Name).Append("新增房源的数量是：").Append(count).AppendLine();
                    }
                }
                log.Debug("今日房源数量收集完毕"+sbMsg);



                //发送邮件 using System.Net.Mail;
                using (MailMessage mailMessage = new MailMessage())
                //using (SmtpClient smtpClient = new SmtpClient("smtp.163.com"))
                using (SmtpClient smtpClient = new SmtpClient(smtpServer))
                {
                    //数据库中存放了多个邮箱，foreach遍历
                    //数据库中使用的;分隔多个邮箱
                    foreach (var bossEmial in bossEmails.Split(';'))
                    {
                        mailMessage.To.Add(bossEmial);
                    }
                    mailMessage.Body = sbMsg.ToString();
                    /*mailMessage.From = new MailAddress("18021461160@163.com");*///从哪里发出
                    mailMessage.From = new MailAddress(smtpUserName);
                    mailMessage.Subject = "今天新增房源数量";
                    //smtpClient.Credentials = new System.Net.NetworkCredential("18021461160@163.com", "159357kdf");
                    //如果启用了“客户端授权码”，要用授权码代替密码
                    //如果邮箱需要启用SSL，则必须设置smptClient.EnableSsl = true;
                    smtpClient.Credentials = new System.Net.NetworkCredential(smtpEmail, smtpPwd);
                    smtpClient.Send(mailMessage);
                }
                log.Debug("发送报表完成");

            }
            catch (Exception ex)
            {

                log.Error("发送报表出错", ex);
            }
        }
    }
}