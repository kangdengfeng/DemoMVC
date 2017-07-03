using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZSZ.CommonMVC;
using ZSZ.IService;

namespace ZSZ.FrontWeb.Controllers
{
    public class UserController : Controller
    {
        public ISettingService settingService { get; set; }
        public IUserService userService { get; set; }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string phoneNum, string verifyCode)
        {
            //先检查验证码对不对
            //验证码是调用的Main里面的CreateVerifyCode
            //验证码保存在了TempData中
            string serververifyCode = (string)TempData["verifyCode"];
            if (serververifyCode != verifyCode)
            {
                return Json(new AjaxResult { Status = "error", ErrorMsg = "验证码错误" });
            }
            var user = userService.GetByPhoneNum(phoneNum);
            if (user == null)
            {
                return Json(new AjaxResult { Status = "error", ErrorMsg = "没有这个手机号!" });
            }

            //发短信,同前台注册发短信代码类似
            //读数据库中的配置信息
            string userName = settingService.GetValue("短信平台Name");
            string appKey = settingService.GetValue("短信平台AppKey");
            string tempId = settingService.GetValue("找回密码短信模板Id");
            //短信验证码一般都是数字
            string smsCode = new Random().Next(1000, 9999).ToString();
            
           
            ZSZSMSSender smsSende = new ZSZSMSSender();
            smsSende.AppKey = appKey;
            smsSende.UserName = userName;

            var sendResult = smsSende.SendSMS(tempId, smsCode, phoneNum);

            //检测有没有发送成功
            if (sendResult.code == 0)
            {
                //把发送验证码的手机号放到TempData中，在注册的时候再次检查一下注册的是不是这个手机号
                //防止网站漏洞
                //放入TempData只能读取一次，修改密码前还要判断手机号是不是之前输入的要找回密码的手机号
                //放入Session中,记录需要重置密码的手机号
                //TempData["ForgotPhoneNum"] = phoneNum;
                Session["ForgotPhoneNum"] = phoneNum;
                TempData["SmsCode"] = smsCode;
                // 短信中的验证码放入TempData中只需要用一次
                return Json(new AjaxResult { Status = "ok" });
            }
            else
            {
                return Json(new AjaxResult { Status = "error", ErrorMsg = sendResult.msg });
            }
           
        }

        [HttpGet]
        public ActionResult ForgotPassword2()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword2(string smsCode)
        {
            //比较短信验证码的内容
            string serverSmsCode = (string)TempData["SmsCode"];
            if (smsCode != serverSmsCode)
            {
                return Json(new AjaxResult { Status = "error", ErrorMsg = "短信验证码错误" });
            }
            else
            {
                //告诉第3步“短信验证码验证通过”，防止恶意用户跳过ForgotPassword2直接重置密码
                TempData["ForgotPassword2_OK"] = true;
                return Json(new AjaxResult
                {
                    Status = "ok"
                });
            }
        }

        [HttpGet]
        public ActionResult ForgotPassword3()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword3(string password)
        {
            bool? is2_OK = (bool?)TempData["ForgotPassword2_OK"];
            if (is2_OK!=true)
            {
                return Json(new AjaxResult{ Status="ok",ErrorMsg="请不要跳过短信验证！"});
            }
            //需要重置密码的手机号
            string phoneNum = (string)Session["ForgotPhoneNum"];
            //根据手机号把用户信息取出来
            var user = userService.GetByPhoneNum(phoneNum);
            userService.UpdatePwd(user.Id, password);
            return Json(new AjaxResult { Status="ok"});
        }

        [HttpGet]
        public ActionResult ForgotPassword4()
        {
            return View();
        }
    }
}