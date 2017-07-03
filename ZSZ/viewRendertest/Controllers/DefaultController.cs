using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace viewRendertest.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default
        public ActionResult Index()
        {
            string html = RenderViewToString(this.ControllerContext, "~/Views/Default/Test.cshtml", "hello");
            System.IO.File.WriteAllText("E:/DemoMVC/1.txt", html);
            return Content("ok");
        }
        static string RenderViewToString(ControllerContext context, string viewPath, object model = null)
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