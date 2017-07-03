using CaptchaGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZSZ.AdminWeb.Models;
using ZSZ.Common;
using ZSZ.CommonMVC;
using ZSZ.IService;

namespace ZSZ.AdminWeb.Controllers
{
    public class MainController : Controller
    {

        public IAdminUserService adminUserService { get; set; }
        public IRoleService roleService { get; set; }
        // GET: Main
        public ActionResult Index()
        {
            long? userId = AdminHelper.GetUserId(HttpContext);
            if (userId == null)
            {
                return Redirect("~/Main/Login");
            }
            var user = adminUserService.GetById((long)userId);
            IndexViewModel model = new IndexViewModel();
            model.AdminUsers = user;
            return View(model);
        }


        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new AjaxResult { Status = "" });
            }
            //验证码漏洞
            //if (model.VerifyCode!=Session["verifyCode"])
            //if (model.VerifyCode != (string)Session["verifyCode"])
            if (model.VerifyCode != (string)TempData["verifyCode"])
            {
                return Json(new AjaxResult { Status = "error", ErrorMsg = "验证码错误" });
            }
            bool result = adminUserService.CheckLogin(model.PhoneNum, model.Password);
            if (result)
            {
                //Session中保存当前登录用户Id
                Session["LoginUserId"] = adminUserService.GetByPhoneNum(model.PhoneNum).Id;
                //后面检查当前Session登录的这个用户有没有操作权限
                return Json(new AjaxResult { Status = "ok" });
            }
            else
            {
                return Json(new AjaxResult { Status = "error", ErrorMsg = "用户名或密码错误" });
            }
        }



        public ActionResult Logout()
        {
            Session.Abandon();//销毁Session
            return Redirect("/Main/Login");
        }

        public ActionResult CreateVerifyCode()
        {
            //产生4位验证码
            string verifyCode = CommonHelper.CreateVerifyCode(4);
            //验证码放入Session
            //Session["verifyCode"] = verifyCode;
            //验证码应该放入TempData
            TempData["verifyCode"] = verifyCode;
            #region
            /*
            using (MemoryStream ms = ImageFactory.GenerateImage(verifyCode, 60, 100, 20, 6))
            //用组件生成验证码图片
            //using (FileStream fs = File.OpenWrite(@"c:\temp\1.jpg"))
            {   //不保存到文件里了
                //FileStreamResult
                return File(ms, "image/jpeg");
                //返回服务器一个图片流

            }
            */
            #endregion
            //EF会自己dispose
            MemoryStream ms = ImageFactory.GenerateImage(verifyCode, 60, 100, 20, 6);
            return File(ms, "image/jpeg");
        }


    }
}