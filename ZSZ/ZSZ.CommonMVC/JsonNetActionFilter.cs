﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ZSZ.CommonMVC
{
    public class JsonNetActionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //把 filterContext.Result从JsonResult换成JsonNetResult
            //filterContext.Result指的就是Action执行返回的ActionResult对象
            //action执行返回的结果
            if (filterContext.Result is JsonResult
            && !(filterContext.Result is JsonNetResult))
            {
                //如果是JsonResult
                //而且不是JsonNetResult就替换为JsonNetResult
                JsonResult jsonResult = (JsonResult)filterContext.Result;
                JsonNetResult jsonNetResult = new JsonNetResult();
                jsonNetResult.ContentEncoding = jsonResult.ContentEncoding;
                jsonNetResult.ContentType = jsonResult.ContentType;
                jsonNetResult.Data = jsonResult.Data;
                jsonNetResult.JsonRequestBehavior = jsonResult.JsonRequestBehavior;
                jsonNetResult.MaxJsonLength = jsonResult.MaxJsonLength;
                jsonNetResult.RecursionLimit = jsonResult.RecursionLimit;
                filterContext.Result = jsonNetResult;
            }
        }
        public void OnActionExecuting(ActionExecutingContext filterContext)
        { }
    }
}