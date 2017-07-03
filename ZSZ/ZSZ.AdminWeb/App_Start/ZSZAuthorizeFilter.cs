using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZSZ.AdminWeb.App_Start;
using ZSZ.CommonMVC;
using ZSZ.IService;

namespace ZSZ.AdminWeb.App_Start
{
    public class ZSZAuthorizeFilter : IAuthorizationFilter
    {
        //public IAdminUserService admUserService { get; set; }
        //admUserService没有注入，ZSZAuthorizeFilter对象不是AutoFac创建的，自己在FilterConfig中new的
        //需要手动获取Service对象
        public void OnAuthorization(AuthorizationContext filterContext)
        {

            //获得当前要执行的Action上标注的CheckPermissionAttribute实例对象
            //可能标注多个
            //[CheckHasPermissionAttribute("Admin.List")]
            //[CheckHasPermissionAttribute("Admin.Add")]
            CheckHasPermissionAttribute[] permAtts = (CheckHasPermissionAttribute[])filterContext.ActionDescriptor
                .GetCustomAttributes(typeof(CheckHasPermissionAttribute), false);

            if (permAtts.Length<=0)//没有标注任何的CheckHasPermissionAttribute，就不用检查是否登录
            {
                return;
            }
            //有标注，但是获得当前用户登录的Id发现没有登录，不能访问
            long? userId = (long?)filterContext.HttpContext.Session["LoginUserId"];
            if (userId==null)
            {
                //判断是普通请求还是Ajax请求
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    //如果是Ajax请求需要返回一个Json
                    AjaxResult ajaxResult = new AjaxResult();
                    ajaxResult.Data = "redirect";
                    ajaxResult.Status = "/Main/Login";
                    ajaxResult.ErrorMsg = "没有登录";
                    filterContext.Result = new JsonNetResult { Data = ajaxResult };
                }
                else
                {
                    //filterContext.HttpContext.Response.Write("没有登录");
                    //filterContext.Result = new ContentResult() { Content="没有登录！"};
                    filterContext.Result = new RedirectResult("~/Main/Login");
                }
                return;
            }

            //手动获取Service对象
            IAdminUserService admUserService = DependencyResolver.Current.GetService<IAdminUserService>();
            //开始检查是否有权限
            //到这里说明userId不为空
            foreach (var permAtt in permAtts)
            {
                //只要碰到任何一个没有的权限就不能访问
                //long? userId.value
                //if (admUserService.HasPermission((long)userId,permAtt.Permission))
                //在IAuthorizationFilter里面，只要修改filterContext.Result 
                //那么真正的Action方法就不会执行了
                if (!admUserService.HasPermission(userId.Value, permAtt.Permission))
                {
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        //如果是Ajax请求需要返回一个Json
                        AjaxResult ajaxResult = new AjaxResult();
                        ajaxResult.Status = "error";
                        ajaxResult.ErrorMsg = "没有权限"+permAtt.Permission;
                        filterContext.Result = new JsonNetResult { Data = ajaxResult };
                    }
                    else
                    {
                        filterContext.Result = new ContentResult { Content = "没有" + permAtt.Permission + "这个权限" };
                    }
                    return;
                }
            }
        }
    }
}