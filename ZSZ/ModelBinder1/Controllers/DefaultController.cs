using ModelBinder1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ModelBinder1.Controllers
{
    public class DefaultController : Controller
    {
        [HttpGet]
        // GET: Default
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(User model)
        {
            //model.UserName=ToDBC(model.UserName.Trim());
            if (model.UserName=="admin"&&model.Password=="123")
            {
                return Content("成功");
            }
            else
            {
                return Content("错误");
            }
        }
        public ActionResult Pager1()
        {
            return View();
        }
    }
}