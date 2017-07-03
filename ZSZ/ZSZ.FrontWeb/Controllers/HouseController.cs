using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ZSZ.CommonMVC;
using ZSZ.DTO;
using ZSZ.FrontWeb.Models;
using ZSZ.IService;

namespace ZSZ.FrontWeb.Controllers
{
    public class HouseController : Controller
    {
        public IHouseService houseService { get; set; }
        public IAttachmentService attService { get; set; }
        public IRegionService regionService { get; set; }
        public IHouseAppointmentService appService { get; set; }

        #region 测试代码
        /*
        public ActionResult AA()
        {
            HouseSearchOptions opt = new HouseSearchOptions();
            opt.CityId = 1;
            opt.TypeId = 11;
            opt.StartMonthRent = 300;
            opt.OrderByType = HouseSearchOrderByType.AreaDesc;
            opt.Keywords = "楼";
            opt.PageSize = 10;
            opt.CurrentIndex = 1;
            var result = houseService.Search(opt);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("总结果条数：" + result.totalCount);
            foreach (var h in result.result)
            {
                sb.AppendLine(h.CommunityName + "," + h.Area + "," + h.MonthRent);
            }
            return Content(sb.ToString());
        }
        */
        #endregion


        public ActionResult Test()
        {
            return View();
        }

        

        /// <summary>
        /// 分析"200-300"、"300-*"这样的价格区间
        /// </summary>
        /// <param name="value">200-300</param>
        /// <param name="startMonthRent">解析出来的起始租金</param>
        /// <param name="endMonthRent">解析出来的结束租金</param>
        private void ParseMonthRent(string value,
            out int? startMonthRent, out int? endMonthRent)
        {
            //如果没有传递MonthRent参数，说明“不限制房租”
            if (string.IsNullOrEmpty(value))
            {
                startMonthRent = null;
                endMonthRent = null;
                return;
            }
            //*-100  200-300
            string[] values = value.Split('-');
            string strStart = values[0];
            string strEnd = values[1];
            if (strStart == "*")
            {
                startMonthRent = null;//不设限
            }
            else
            {
                startMonthRent = Convert.ToInt32(strStart);
            }
            if (strEnd == "*")
            {
                endMonthRent = null;//不设限
            }
            else
            {
                endMonthRent = Convert.ToInt32(strEnd);
            }
        }


        //Ajax加载
        public ActionResult Search2(long typeId, string keyWords, string monthRent,
            string orderByType, long? regionId)
        {
            long cityId = FrontUtils.GetCityId(HttpContext);
            var regions = regionService.GetAll(cityId);
            return View(regions);
        }


        //Ajax加载
        public ActionResult LoadMore(long typeId, string keyWords, string monthRent,
            string orderByType, long? regionId, int pageIndex)
        {
            long cityId = FrontUtils.GetCityId(HttpContext);
            HouseSearchOptions searchOpt = new HouseSearchOptions();
            searchOpt.CityId = cityId;
            searchOpt.CurrentIndex = pageIndex;

            //解析月租部分
            int? startMonthRent;
            int? endMonthRent;
            //ref/out
            ParseMonthRent(monthRent, out startMonthRent, out endMonthRent);
            searchOpt.EndMonthRent = endMonthRent;
            searchOpt.StartMonthRent = startMonthRent;

            searchOpt.Keywords = keyWords;
            switch (orderByType)
            {
                case "MonthRentAsc":
                    searchOpt.OrderByType = HouseSearchOrderByType.MonthRentAsc;
                    break;
                case "MonthRentDesc":
                    searchOpt.OrderByType = HouseSearchOrderByType.MonthRentDesc;
                    break;
                case "AreaAsc":
                    searchOpt.OrderByType = HouseSearchOrderByType.AreaAsc;
                    break;
                case "AreaDesc":
                    searchOpt.OrderByType = HouseSearchOrderByType.AreaDesc;
                    break;
            }
            searchOpt.PageSize = 10;
            searchOpt.RegionId = regionId;
            searchOpt.TypeId = typeId;

            //开始搜索
            var searchResult = houseService.Search(searchOpt);
            var houses = searchResult.result;
            return Json(new AjaxResult { Status = "ok", Data = houses });
        }



        //服务器端分页加载
        //long typeId,string keyWords, string monthRent, string orderByType, long? regionId
        //都是搜索条件,从QueryString中获取
        public ActionResult Search(long typeId, string keyWords, string monthRent, string orderByType, long? regionId)
        {

            //根据Session中保存的用户Id获得用户所在城市Id
            long cityId = FrontUtils.GetCityId(HttpContext);
            //取得城市下所有区域
            var regions = regionService.GetAll(cityId);
            //把用户所在城市区域传给界面
            HouseSearchViewModel model = new HouseSearchViewModel();
            model.Regions = regions;


            //动态构造HouseSearchOptions对象
            HouseSearchOptions searchOpt = new HouseSearchOptions();
            searchOpt.CityId = cityId;
            searchOpt.CurrentIndex = 1;


            //解析月租部分
            int? startMonthRent;
            int? endMonthRent;
            //ref/out
            ParseMonthRent(monthRent, out startMonthRent, out endMonthRent);
            searchOpt.EndMonthRent = endMonthRent;
            searchOpt.StartMonthRent = startMonthRent;

            //关键字
            searchOpt.Keywords = keyWords;

            //解析排序规则
            switch (orderByType)
            {
                case "MonthRentAsc":
                    searchOpt.OrderByType = HouseSearchOrderByType.MonthRentAsc;
                    break;
                case "MonthRentDesc":
                    searchOpt.OrderByType = HouseSearchOrderByType.MonthRentDesc;
                    break;
                case "AreaAsc":
                    searchOpt.OrderByType = HouseSearchOrderByType.AreaAsc;
                    break;
                case "AreaDesc":
                    searchOpt.OrderByType = HouseSearchOrderByType.AreaDesc;
                    break;
            }


            searchOpt.PageSize = 10;
            searchOpt.RegionId = regionId;
            searchOpt.TypeId = typeId;

            //开始搜索
            var searchResult = houseService.Search(searchOpt);
            //搜索结果
            model.houses = searchResult.result;

            return View(model);
        }
        public ActionResult Index(long id)
        {
            #region 没有使用缓存
            /*
            var house = houseService.GetById(id);
            if (house == null)
            {
                return View("Error", (object)"不存在的房源id");
            }
            //获得图片信息
            var pics = houseService.GetPics(id);
            List<AttachementDTO> atts = new List<AttachementDTO>();
            var attachments = attService.GetAttachments(id);

            HouseIndexViewModel model = new HouseIndexViewModel();
            model.House = house;
            model.Pics = pics;
            model.Attachments = attachments;
            */
            #endregion


            //内置缓存
            //先看缓存中有没有数据
            //缓存的key的名字一定不能重复
            string cachekey = "HouseIndex_" + id;
            HouseIndexViewModel model = (HouseIndexViewModel)HttpContext.Cache[cachekey];
            if (model == null)
            {
                //缓存中没有数据再去数据库中查
                var house = houseService.GetById(id);
                if (house == null)
                {
                    return View("Error", (object)"不存在的房源id");
                }
                //获得图片信息
                var pics = houseService.GetPics(id);
                List<AttachementDTO> atts = new List<AttachementDTO>();
                var attachments = attService.GetAttachments(id);
                model = new HouseIndexViewModel();
                model.House = house;
                model.Pics = pics;
                model.Attachments = attachments;
                //查到数据放入缓存
                //HttpContext.Cache.Add(cachekey,model,null,DateTime.Now.AddMinutes(1),TimeSpan.Zero, System.Web.Caching.CacheItemPriority.Default,null);
                HttpContext.Cache.Insert(cachekey, model, null, DateTime.Now.AddMinutes(1), TimeSpan.Zero);
            }
            

            //可以使用Memcached或者redis优化缓存
            #region Memcached缓存优化，类需要标记为Serializable且要开启memcached服务器,暂不使用
            /*
            //Memcached缓存优化
            string cachekey = "HouseIndex_" + id;
            HouseIndexViewModel model
                = (HouseIndexViewModel)MemcachedMg.Instance.GetValue(cachekey);

            //HouseIndexViewModel model
            //    = MemcachedMg.Instance.GetValue<HouseIndexViewModel>(cachekey);

            if (model == null)
            {
                //缓存中没有数据再去数据库中查
                var house = houseService.GetById(id);
                if (house == null)
                {
                    return View("Error", (object)"不存在的房源id");
                }
                //获得图片信息
                var pics = houseService.GetPics(id);
                List<AttachementDTO> atts = new List<AttachementDTO>();
                var attachments = attService.GetAttachments(id);
                model = new HouseIndexViewModel();
                model.House = house;
                model.Pics = pics;
                model.Attachments = attachments;
                //查到数据放入缓存
                //HttpContext.Cache.Add(cachekey,model,null,DateTime.Now.AddMinutes(1),TimeSpan.Zero, System.Web.Caching.CacheItemPriority.Default,null);
                //HttpContext.Cache.Insert(cachekey, model, null, DateTime.Now.AddMinutes(1), TimeSpan.Zero);
                MemcachedMg.Instance.SetValue(cachekey, model, 
                    TimeSpan.FromMinutes(1));
                //要求类要是可序列化的，把HouseIndexViewModel标记为Serializable

            }
            */
            #endregion
            

            return View(model);
        }




        //预约看房的实现
        //需要姓名、手机号、看房时间，用Model
        public ActionResult MakeAppointment(HouseMakeAppointmentModel model)
        {
            //验证Model合法性
            if (!ModelState.IsValid)
            {
                string msg = MVCHelper.GetValidMsg(ModelState);
                return Json(new AjaxResult { Status = "error", ErrorMsg = msg });
            }
            //没有登录也可以预约看房
            long? userId = FrontUtils.GetUserId(HttpContext);
            appService.AddNew(userId, model.Name,
                model.PhoneNum, model.HouseId, model.VisitDate);
            return Json(new AjaxResult { Status = "ok" });
        }
    }
}