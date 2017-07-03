using CodeCarvings.Piczard;
using CodeCarvings.Piczard.Filters.Watermarks;
using PlainElastic.Net;
using PlainElastic.Net.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZSZ.AdminWeb.App_Start;
using ZSZ.AdminWeb.Models;
using ZSZ.Common;
using ZSZ.CommonMVC;
using ZSZ.DTO;
using ZSZ.IService;

namespace ZSZ.AdminWeb.Controllers
{
    public class HouseController : Controller
    {
        public IAdminUserService userSerivce { get; set; }
        public IHouseService houseService { get; set; }
        public ICityService cityService { get; set; }
        public IRegionService regionService { get; set; }
        public ICommunityService communityService { get; set; }
        public IIdNameService idNameService { get; set; }
        public IAttachmentService attService { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeId">房源类型：整租还是合租</param>
        /// /// <param name="pageIndex">看第几页</param>
        /// <returns></returns>
        // GET: House
        public ActionResult List(long typeId, int pageIndex = 1)//没有给pageIndex传值就默认值为1
        {
            //获取Session中当前用户的Id
            //因为AuthorizFilter做了是否登录的检查，因此这里不会取不到id
            long userId = (long)AdminHelper.GetUserId(HttpContext);
            long? cityId = userSerivce.GetById(userId).CityId;
            if (cityId == null)
            {
                return View("Error", (object)"总部不能进行房源管理");
            }
            //var houses = houseService.GetPagedData(cityId.Value, typeId, 10, 0);
            var houses = houseService.GetPagedData(cityId.Value, typeId, 10, (pageIndex - 1) * 10);
            //这个城市下一共有多少条房子记录
            long totalcount = houseService.GetTotalCount(cityId.Value, typeId);
            ViewBag.pageIndex = pageIndex;
            ViewBag.totalcount = totalcount;
            ViewBag.typeId = typeId;
            return View(houses);
        }


        public ActionResult Delete(long id)
        {
            houseService.MarkDeleted(id);
            return Json(new AjaxResult { Status = "ok" });
        }


        public ActionResult BatchDelete(long[] selectedIds)
        {
            foreach (long id in selectedIds)
            {
                houseService.MarkDeleted(id);
            }
            return Json(new AjaxResult { Status = "ok" });
        }


        [HttpGet]
        public ActionResult Add()
        {
            long? userId = AdminHelper.GetUserId(HttpContext);
            long? cityId = userSerivce.GetById(userId.Value).CityId;
            if (cityId == null)
            {
                return View("Error", (object)"总部不能进行房源管理");
            }
            var regions = regionService.GetAll(cityId.Value);
            var roomTypes = idNameService.GetAll("户型");
            var statuses = idNameService.GetAll("房屋状态");
            var decorateStatuses = idNameService.GetAll("装修状态");
            var attachments = attService.GetAll();
            var types = idNameService.GetAll("房屋类别");

            HouseAddViewModel model = new HouseAddViewModel();
            model.regions = regions;
            model.roomTypes = roomTypes;
            model.statuses = statuses;
            model.decorateStatuses = decorateStatuses;
            model.attachments = attachments;
            model.types = types;
            
            return View(model);
        }



        [ValidateInput(false)]
        //[CheckHasPermission("House.Add")]
        [HttpPost]
        public ActionResult Add(HouseAddModel model)
        {
            long? userId = AdminHelper.GetUserId(HttpContext);
            long? cityId = userSerivce.GetById(userId.Value).CityId;
            if (cityId == null)
            {
                return View("Error", (object)"总部不能进行房源管理");
            }
            HouseAddnewDTO dto = new HouseAddnewDTO();
            dto.Address = model.address;
            dto.Area = model.area;
            dto.AttachmentIds = model.attachmentIds;
            dto.CheckInDateTime = model.checkInDateTime;
            dto.CommunityId = model.CommunityId;
            dto.DecorateStatusId = model.DecorateStatusId;
            dto.Description = model.description;
            dto.Direction = model.direction;
            dto.FloorIndex = model.floorIndex;
            dto.LookableDateTime = model.lookableDateTime;
            dto.MonthRent = model.monthRent;
            dto.OwnerName = model.ownerName;
            dto.OwnerPhoneNum = model.ownerPhoneNum;
            dto.RoomTypeId = model.RoomTypeId;
            dto.StatusId = model.StatusId;
            dto.TotalFloorCount = model.totalFloor;
            dto.TypeId = model.TypeId;

            long houseId = houseService.AddNew(dto);

            //生成房源查看的html文件
            CreateStaticPage(houseId);



            //把房源信息写入ElasticSearch
            //创建与SQLLite的连接
            ElasticConnection client = new ElasticConnection("localhost", 9200);
            var serializer = new JsonNetSerializer();
            //写入数据
            //第一个参数相当于“数据库”，第二个参数相当于“表”，第三个参数相当于“主键”
            IndexCommand indexcmd = new IndexCommand("ZSZHouse", "House", houseId.ToString());
            //不用手动创建数据库，es会自动分配空间用zsz命名
            //Put()第二个参数是要插入的数据
            OperationResult result = client.Put(indexcmd, serializer.Serialize(dto));//把对象序列化成json放入Elastic中返回结果




            return Json(new AjaxResult { Status = "ok" });
        }
        private void CreateStaticPage(long houseId)
        {
            var house = houseService.GetById(houseId);

            //获得图片信息
            var pics = houseService.GetPics(houseId);
            List<AttachementDTO> atts = new List<AttachementDTO>();
            var attachments = attService.GetAttachments(houseId);
            HouseIndexViewModel model = new HouseIndexViewModel();
            model.House = house;
            model.Pics = pics;
            model.Attachments = attachments;
            string html = MVCHelper.RenderViewToString(this.ControllerContext, @"~/Views/House/StaticIndex.cshtml", model);
            //静态页面保存到网站的根目录下去,用房子Id命名
            System.IO.File.WriteAllText(@"E:\DemoMVC\ZSZ\ZSZ.FrontWeb\" + houseId + ".html", html);
        }
        //把房源信息写入ElasticSearch
        //private void AddElasticSearch(long houseId)
        //{
           
        //}

        public ActionResult LoadCommunities(long regionId)
        {
            var communities = communityService.GetByRegionId(regionId);
            return Json(new AjaxResult { Status = "ok", Data = communities });
        }

        [HttpGet]
        public ActionResult Edit(long id)
        {
            long? userId = AdminHelper.GetUserId(HttpContext);
            long? cityId = userSerivce.GetById(userId.Value).CityId;
            if (cityId == null)
            {
                return View("Error", (object)"总部不能进行房源管理");
            }
            var house = houseService.GetById(id);
            HouseEditViewModel model = new HouseEditViewModel();
            model.house = house;

            var regions = regionService.GetAll(cityId.Value);
            var roomTypes = idNameService.GetAll("户型");
            var statuses = idNameService.GetAll("房屋状态");
            var decorateStatuses = idNameService.GetAll("装修状态");
            var attachments = attService.GetAll();
            var types = idNameService.GetAll("房屋类别");

            model.regions = regions;
            model.roomTypes = roomTypes;
            model.statuses = statuses;
            model.decorateStatuses = decorateStatuses;
            model.attachments = attachments;
            model.types = types;
            return View(model);
        }


        [HttpPost]
        public ActionResult Edit(HouseEditModel model)
        {
            HouseDTO dto = new HouseDTO();
            dto.Address = model.address;
            dto.Area = model.area;
            dto.AttachmentIds = model.attachmentIds;
            dto.CheckInDateTime = model.checkInDateTime;
            //强硬用一些不适合的DTO，有一些没用的属性时候的迷茫？
            dto.CommunityId = model.CommunityId;
            dto.DecorateStatusId = model.DecorateStatusId;
            dto.Description = model.description;
            dto.Direction = model.direction;
            dto.FloorIndex = model.floorIndex;
            dto.Id = model.Id;
            dto.LookableDateTime = model.lookableDateTime;
            dto.MonthRent = model.monthRent;
            dto.OwnerName = model.ownerName;
            dto.OwnerPhoneNum = model.ownerPhoneNum;
            dto.RoomTypeId = model.RoomTypeId;
            dto.StatusId = model.StatusId;
            dto.TotalFloorCount = model.totalFloor;
            dto.TypeId = model.TypeId;

            houseService.Update(dto);

            CreateStaticPage(model.Id);//编辑房源的时候重新生成静态页面
            return Json(new AjaxResult { Status = "ok" });
        }

        //houseId : 图片上传到哪一个房子的
        public ActionResult PicUpload(long houseId)
        {
            return View(houseId);
        }

        //上传图片
        public ActionResult UploadPic(int houseId, HttpPostedFileBase file)
        //webuploader 文件是一个个上传的， 表单名(name)是 file
        {
            //if (houseId < 5)
            //{
            //    return Json(new AjaxResult { Status = "error", ErrorMsg = "id必须大于5" });
            //}
            //server配置上传地址
            //Path.GetExtension(file.FileName)获得上传文件的扩展名
            // file.SaveAs(HttpContext.Server.MapPath("~/" + houseId + Path.GetExtension(file.FileName)));
            //return Content("OK");
            string md5 = CommonHelper.CalcMD5(file.InputStream);
            string ext = Path.GetExtension(file.FileName);
            string path = "/upload" + DateTime.Now.ToString("yyyy/MM/dd") + "/" + md5 + ext;
            //缩略图的路径  "_thumb"
            string thumbpath = "/upload" + DateTime.Now.ToString("yyyy/MM/dd") + "/" + md5 + "_thumb" + ext;
            //转为物理路径
            string fullpath = HttpContext.Server.MapPath("~" + path);
            //缩略图全路径
            string thumbfullpath = HttpContext.Server.MapPath("~" + thumbpath);
            //文件夹可能不存在的
            //尝试创建可能不存在的文件夹,即使存在也不报错
            new FileInfo(fullpath).Directory.Create();

            file.InputStream.Position = 0;//指针复位

            //保存的原图file.SaveAs(fullpath);


            //给大图加水印，生成缩略图
            //保存缩略图
            ImageProcessingJob jobThumb = new ImageProcessingJob();
            jobThumb.Filters.Add(new FixedResizeConstraint(150, 150));//缩略图尺寸 200*200
            jobThumb.SaveProcessedImageToFileSystem(file.InputStream, thumbfullpath);


            file.InputStream.Position = 0;//指针复位
            //处理完缩略图指针已经到了流的末尾

            //大图加水印
            ImageWatermark imgWatermark =
               new ImageWatermark(HttpContext.Server.MapPath("~/images/watermark.png"));
            imgWatermark.ContentAlignment = System.Drawing.ContentAlignment.MiddleCenter;//水印位置
            imgWatermark.Alpha = 50;//透明度，需要水印图片是背景透明的png图片
            ImageProcessingJob jobNormal = new ImageProcessingJob();
            jobNormal.Filters.Add(imgWatermark);//添加水印
            //jobNormal.Filters.Add(new FixedResizeConstraint(600, 600));
            //保存有水印的图片
            jobNormal.SaveProcessedImageToFileSystem(file.InputStream, fullpath);



            //把路径写入数据库
            houseService.AddNewHousePic(new HousePicDTO { HouseId = houseId, Url = path, ThumbUrl = thumbpath });


            //上传图片或者删除图片房子信息改变，需要重新生成静态页面
            CreateStaticPage(houseId);


            return Json(new AjaxResult { Status = "ok" });

        }


        public ActionResult PicList(long id)
        {
            var pics = houseService.GetPics(id);
            return View(pics);
        }


        public ActionResult DeletePics(long[] selectedIds)
        {
            foreach (var picId in selectedIds)
            {
                houseService.DeleteHousePic(picId);
            }
            //删除图片之后也要重新生成静态页面
            //CreateStaticPage(houseId);
            return Json(new AjaxResult { Status = "ok" });
        }

        //重新生成所有房子的静态页面
        public ActionResult RebuildAllStaticPage()
        {
            var houses = houseService.GetAll();
            foreach (var house in houses)
            {
                CreateStaticPage(house.Id);
            }
            return Json(new AjaxResult { Status = "ok" });
        }
    }
}