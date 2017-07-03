using CaptchaGen;
using PlainElastic.Net;
using PlainElastic.Net.Queries;
using PlainElastic.Net.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZSZ.Common;
using ZSZ.CommonMVC;
using ZSZ.DTO;
using ZSZ.FrontWeb.Models;
using ZSZ.IService;

namespace ZSZ.FrontWeb.Controllers
{
    public class MainController : Controller
    {
        public ICityService cityService { get; set; }
        public ISettingService settingService { get; set; }
        public IUserService userService { get; set; }

        public ActionResult Index()
        {
            //一定能拿到一个CityId
            long cityId = FrontUtils.GetCityId(HttpContext);
            string cityName = cityService.GetById(cityId).Name;
            ViewBag.cityName = cityName;
            //不用ViewBag，后面需要改成ViewModel
            var cities = cityService.GetAll();
            return View(cities);
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        public ActionResult CreateVerifyCode()
        {
            string verifyCode = CommonHelper.CreateVerifyCode(4);
            //验证码放入TempData中最安全
            TempData["verifyCode"] = verifyCode;
            MemoryStream ms = ImageFactory.GenerateImage(verifyCode, 60, 100, 20, 6);
            return File(ms, "image/jpeg");
        }

        public ActionResult SendSmsVerifyCode(string phoneNum, string verifyCode)
        {
            //取出服务器中保存的验证码
            string serverfyCode = (string)TempData["verifyCode"];
            //比较验证码
            if (serverfyCode != verifyCode)
            {
                return Json(new AjaxResult { Status = "error", ErrorMsg = "图形验证码错误" });
            }
            //读数据库中的配置信息
            string userName = settingService.GetValue("短信平台Name");
            string appKey = settingService.GetValue("短信平台AppKey");
            string tempId = settingService.GetValue("注册短信模板Id");
            //短信验证码一般都是数字
            string smsCode = new Random().Next(1000, 9999).ToString();
            TempData["smsCode"] = smsCode;
            //放入TempData中便于[HttpPost] public ActionResult Register()验证
            ZSZSMSSender smsSende = new ZSZSMSSender();
            smsSende.AppKey = appKey;
            smsSende.UserName = userName;

            var sendResult = smsSende.SendSMS(tempId, smsCode, phoneNum);

            //检测有没有发送成功
            if (sendResult.code == 0)
            {
                //把发送验证码的手机号放到TempData中，在注册的时候再次检查一下注册的是不是这个手机号
                //防止网站漏洞
                TempData["RegPhoneNum"] = phoneNum;
                return Json(new AjaxResult { Status = "ok" });
            }
            else
            {
                return Json(new AjaxResult { Status = "error", ErrorMsg = sendResult.msg });
            }
        }

        [HttpPost]
        public ActionResult Register(UserRegModel model)
        {
            //表单合法性验证
            if (ModelState.IsValid == false)
            {
                return Json(new AjaxResult
                {
                    Status = "error",
                    ErrorMsg = MVCHelper.GetValidMsg(ModelState)
                });
            }
            //检查注册的是不刚刚发送验证码的手机号
            string serverPhoneNum = (string)TempData["RegPhoneNum"];
            if (serverPhoneNum != model.PhoneNum)
            {
                return Json(new AjaxResult
                {
                    Status = "error",
                    ErrorMsg = "注册的手机号和刚才发送短信的手机号不一致"
                });
            }
            //把服务器中smsCode取出来和输入的短信验证码比较是否一致
            string serversmsCode = (string)TempData["smsCode"];
            if (model.SmsCode != serversmsCode)
            {
                return Json(new AjaxResult { Status = "error", ErrorMsg = "短信验证码错误" });
            }
            //检查手机号是否被占用
            if (userService.GetByPhoneNum(model.PhoneNum) != null)
            {
                return Json(new AjaxResult { Status = "error", ErrorMsg = "此手机号已被注册" });
            }
            userService.AddNew(model.PhoneNum, model.Password);
            return Json(new AjaxResult { Status = "ok" });
        }


        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserLoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new AjaxResult { Status = "error", ErrorMsg = MVCHelper.GetValidMsg(ModelState) });
            }
            var user = userService.GetByPhoneNum(model.PhoneNum);

            //手机号存在再判断是否已经被锁定
            if (user != null)
            {
                if (userService.IsLocked(user.Id))//被锁定
                {
                    TimeSpan? leftTimeSpan = TimeSpan.FromMinutes(30) - (DateTime.Now - user.LastLoginErrorDateTime);
                    return Json(new AjaxResult
                    {
                        Status = "error",
                        ErrorMsg = "账号已被锁定，请"
                        + (int)leftTimeSpan.Value.TotalMinutes + "分钟后再试"
                    });
                }
            }

            //检查用户名密码对不对
            bool isOK = userService.CheckLogin(model.PhoneNum, model.Password);

            if (isOK)
            {
                //一旦登录成功，就重置所有登录错误信息，避免影响下一次登录
                userService.ResetLoginError(user.Id);

                //2017-06-26
                //登录成功后把当前登录用户信息存入Session
                Session["UserId"] = user.Id;
                Session["CityId"] = user.CityId;
                //cityId可能为null

                return Json(new AjaxResult { Status = "ok" });
            }
            else
            {
                //只有手机号正确的时候才记录错误次数
                if (user != null)
                {
                    userService.IncrLoginError(user.Id);
                }

                return Json(new AjaxResult { Status = "error", ErrorMsg = "用户名或密码错误" });
            }
        }

        //切换当前用户所在城市ID
        public ActionResult SwitchCityId(long cityId)
        {
            //先判断有没有用户登录
            long? userId = FrontUtils.GetUserId(HttpContext);
            if (userId == null)//无人登录
            {
                Session["CityId"] = cityId;
            }
            else
            {
                userService.SetUserCityId((long)userId, cityId);
            }
            return Json(new AjaxResult { Status = "ok" });
        }


        //ES搜索
        public ActionResult EsSearch(string Msg)
        {
            //从Es中查数据
            //模糊查询
            ElasticConnection client = new ElasticConnection("localhost", 9200);
            SearchCommand cmd = new SearchCommand("ZSZHouse", "House");//要查询的数据库，表名字
            var query = new QueryBuilder<HouseAddnewDTO>()
            .Query(b =>
            b.Bool(m =>
            //并且关系
            m.Must(t =>
            //分词的最小单位或关系查询
            t.QueryString(t1 => t1.DefaultField("Address").Query(Msg))//title字段必须含有“美女”
            )
            )
            )
            .Build();
            var result = client.Post(cmd, query);
            var serializer = new JsonNetSerializer();
            var searchResult = serializer.ToSearchResult<HouseAddnewDTO>(result);
            //searchResult.hits.total;//一共有多少匹配结果
            //searchResult.Documents;//当前页的查询结果

            IndexHouseSearchViewModel model = new IndexHouseSearchViewModel();
            foreach (var doc in searchResult.Documents)
            {
                var r = doc.Address;
                model.Address = r;
                if (r.Contains(Msg))
                {
                    return Json(new AjaxResult { Status = "ok" });
                }
            }
            return Json(new AjaxResult { Status = "error" });

        }
    }
}