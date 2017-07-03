using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebApplication1
{
    public class IJobSecond : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                File.AppendAllText(@"d:/log.txt", "22222222222222222222执行了" + DateTime.Now);
            }
            catch (Exception ex)
            {

                File.AppendAllText(@"d:/log.txt", "22222222222222222222出错啦" + ex);
            }

        }
    }
}