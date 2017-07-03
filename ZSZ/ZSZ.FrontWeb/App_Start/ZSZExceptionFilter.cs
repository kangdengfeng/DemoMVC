using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ZSZ.FrontWeb.App_Start
{
    //IExceptionFilter用来处理未处理异常
    public class ZSZExceptionFilter : IExceptionFilter
    {
        //定义一个变量
        private static ILog log = LogManager.GetLogger(typeof(ZSZExceptionFilter));
        public void OnException(ExceptionContext filterContext)
        {
            //当发生未处理异常是就记录下来
            log.Error("出现未处理异常",filterContext.Exception);
        }
    }
}