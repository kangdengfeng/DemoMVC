using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSZ.Test
{
    class TestJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            /*
            try
            {
                Console.WriteLine("任务执行啦！" + DateTime.Now);
                //SqlConnection conn = new SqlConnection();
                //conn.Open();
                Console.WriteLine("完成了");
            }
            catch (Exception ex)
            {
                ILog log = LogManager.GetLogger(typeof(Program));
                log.Error("定时任务异常",ex);
                Console.WriteLine("定时任务异常！");
            }
            */
            
        }
    }
}
