using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.IO;

namespace ZSZ.CommonMVC
{
    public class MVCHelper
    {

        public static string GetValidMsg(ModelStateDictionary modelState)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var key in modelState.Keys)
            {
                if (modelState[key].Errors.Count <= 0)
                {
                    continue;
                }
                sb.Append("属性【").Append(key).Append("】错误： ");
                foreach (var modelError in modelState[key].Errors)
                {
                    sb.AppendLine(modelError.ErrorMessage);
                }
            }
            return sb.ToString();
        }


        //把namevaluecollection键值对形式转换成字符串用于在地址栏
        public static string ToQueryString(NameValueCollection nvc)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var key in nvc.AllKeys)
            {
                string value = nvc[key];//根据键获取值
                sb.Append(key).Append("=").Append(Uri.EscapeDataString(value)).Append("&");
            }
            return sb.ToString().Trim('&');
        }
        

        /// 从 QueryString 中 移 除 name 的 值 public static string
        public static string RemoveQueryString(NameValueCollection queryString, string name)
        {
            NameValueCollection newNVC = new NameValueCollection(queryString);
            newNVC.Remove(name);
            return ToQueryString(newNVC);
        }
        


        /// 修改 QueryString 中 name 的值为 value，如果不存在，则添加
        // NameValueCollection 相当于 Dictionary，存放的是 QueryString 中的键值对
        public static string UpdateQueryString(NameValueCollection queryString, string name, object
        value)
        {
            //拷贝一份，不影响本来的 QueryString
            //     将项从指定的 System.Collections.Specialized.NameValueCollection 
            //复制到一个新的 System.Collections.Specialized.NameValueCollection，
            //这个新集合的初始容量与复制的项数相等，并使用与源集合相同的哈希代码提供程序和比较器。
            NameValueCollection newNVC = new NameValueCollection(queryString);
            if (newNVC.AllKeys.Contains(name))
            {
                newNVC[name] = Convert.ToString(value);
            }
            else
            {
                newNVC.Add(name, Convert.ToString(value));
            }
            //return newNVC.ToQueryString();
            return ToQueryString(newNVC);
        }


        //生成html
        public static string RenderViewToString(ControllerContext context, string viewPath, object model = null)
        {
            ViewEngineResult viewEngineResult =
            ViewEngines.Engines.FindView(context, viewPath, null);
            if (viewEngineResult == null)//找不到视图就报错
            {
                throw new FileNotFoundException("View" + viewPath + "cannot be found.");
            }
            var view = viewEngineResult.View;
            context.Controller.ViewData.Model = model;//找到了就给model赋值
            using (var sw = new StringWriter())
            {
                var ctx = new ViewContext(context, view,
                context.Controller.ViewData,
                context.Controller.TempData,
                sw);
                view.Render(ctx, sw);//生成字符串，生成到stringWrite中
                return sw.ToString();
            }
        }

    }
}
