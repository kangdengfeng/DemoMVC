using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication2
{
    public class JsonNetActionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //把filterContext从JsonResult换成JsonNetResult
            //filterContext.Result指的就是Action执行返回的ActionResult对象
            if (filterContext.Result is JsonNetResult 
                && !(filterContext.Result is JsonNetResult))
            {
                //判断是JsonNetResult类型而不是JsonNetResult类型
                JsonResult jsonResult = (JsonResult)filterContext.Result;
                JsonNetResult jsonNetResult = new JsonNetResult();
                jsonNetResult.ContentEncoding = jsonResult.ContentEncoding;
                jsonNetResult.ContentType = jsonResult.ContentType;
                jsonNetResult.Data = jsonResult.Data;
                jsonNetResult.JsonRequestBehavior = jsonResult.JsonRequestBehavior;
                jsonNetResult.MaxJsonLength = jsonResult.MaxJsonLength;
                jsonNetResult.RecursionLimit = jsonResult.RecursionLimit; 

                filterContext.Result = jsonNetResult;
                //在Global中添加Filter
            }
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {

        }
    }
}