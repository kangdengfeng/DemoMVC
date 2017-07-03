using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace WebApplication1
{
    public class TestJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                //用HostingEnvironment.MapPath()替代HttpContext
                //Job的execute是在单独的线程中进行，和web请求没有关系
                string path = HostingEnvironment.MapPath("~/Web.config");
                //string path = HttpContext.Current.Server.MapPath("~/Web.config");
                File.AppendAllText(@"d:/log.txt", "执行了" + DateTime.Now);
            }
            catch (Exception ex)
            {

                File.AppendAllText(@"d:/log.txt", "出错啦" + ex);
            }
          
        }
    }
}