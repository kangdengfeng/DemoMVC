using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZSZ.AdminWeb.App_Start;
using ZSZ.AdminWeb.Models;
using ZSZ.CommonMVC;
using ZSZ.IService;
using ZSZ.Service;

namespace ZSZ.AdminWeb.Controllers
{
    public class AdminUserController : Controller
    {
        public IAdminUserService admUserService { get; set; }
        //IAdminUserService admUserService = new AdminUserService();
        public IRoleService roleService { get; set; }
        public ICityService cityService { get; set; }



        // GET: AdminUser
        [CheckHasPermission("AdminUser.List")]
        //访问list必须具有“Admin.List”权限
        //在ZSZAuthorizeFilter中编写识别标签的代码
        public ActionResult List()
        {
            //每个用户都要判断是否具有权限
            //AOP:AuthorizeFilter/ActionFilter/ResultFilter/ExceptionFilter

            var roles = admUserService.GetAll();
            return View(roles);
        }


        [CheckHasPermission("AdminUser.Add")]
        [HttpGet]
        public ActionResult Add()
        {
            var cities = cityService.GetAll().ToList();
            //在城市的最前面加一个总部，转为list数组之后才可以添加
            //insert只有list才支持，getall返回的是数组
            cities.Insert(0, new DTO.CityDTO { Id = 0, Name = "总部" });
            var roles = roleService.GetAll();
            AdminUserAddViewModel model = new AdminUserAddViewModel();
            model.Cities = cities.ToArray();
            model.Roles = roles;
            return View(model);
        }


        [CheckHasPermission("AdminUser.Add")]
        [HttpPost]
        public ActionResult Add(AdminUserAddModel model)
        {
            if (!ModelState.IsValid)
            {
                string msg = MVCHelper.GetValidMsg(ModelState);
                return Json(new AjaxResult { Status = "erroe", ErrorMsg = msg });
            }
            //服务器端的校验必不可少
            bool exists = admUserService.GetByPhoneNum(model.PhoneNum) != null;
            if (exists)
            {
                return Json(new AjaxResult { Status = "error", ErrorMsg = "手机号已经存在" });
            }
            //新增的时候给城市前面加了一个总部为0的cityId
            long? cityId = null;
            if (model.CityId != 0)//cityId=0
            {
                cityId = model.CityId;
            }
            //新增用户
            long userId = admUserService.AddAdminUser(model.Name, model.PhoneNum, model.Password, model.Email, cityId);
            //新增用户获得的角色
            roleService.AddRoleIds(userId, model.RoleIds);
            return Json(new AjaxResult { Status = "ok" });
        }



        [CheckHasPermission(" AdminUser.SearchPhoneNum")]
        public ActionResult CheckPhoneNum(string phone, long? userId)
        {
            //如果没有给userId,则说明是“插入”,只要检查是不是存在这个手机号
            var user = admUserService.GetByPhoneNum(phone);
            bool isOk = false;
            if (user == null)
            {
                isOk = (user == null);
            }
            else//如果有userId，则说明是修改，则要把自己排出在外
            {
                isOk = (user.Id == userId || user == null);
            }
            return Json(new AjaxResult { Status = isOk ? "OK" : "exists" });
        }



        [CheckHasPermission("AdminUser.Edit")]
        [HttpGet]
        public ActionResult Edit(long id)
        {
            //先查询有没有这个用户
            var adminuser = admUserService.GetById(id);
            if (adminuser == null)
            {
                //第二个参数如果不强制转为object,就会调用另一个重载的方法
                return View("Error", (object)"id指定的操作员不存在");
            }
            var cities = cityService.GetAll().ToList();
            //在城市的最前面加一个总部，转为list数组之后才可以添加
            //insert只有list才支持，getall返回的是数组
            cities.Insert(0, new DTO.CityDTO { Id = 0, Name = "总部" });

            var roles = roleService.GetAll();
            var userRoles = roleService.GetByAdminUserId(id);

            AdminUserEditViewModel model = new AdminUserEditViewModel();
            model.UserRoleIds = userRoles.Select(r => r.Id).ToArray();
            model.AdminUser = adminuser;
            model.Cities = cities.ToArray();
            model.Roles = roles;

            return View(model);
        }



        [CheckHasPermission("AdminUser.Edit")]
        [HttpPost]
        public ActionResult Edit(AdminUserEditModel model)
        {
            long? cityId = null;
            if (model.CityId > 0)
            {
                cityId = model.CityId;
            }
            admUserService.UpdateAdminUser(model.Id, model.Name, model.PhoneNum, model.Password, model.Email, cityId);
            roleService.UpdateRoleIds(model.Id, model.RoleIds);
            return Json(new AjaxResult { Status = "ok" });
        }



        [CheckHasPermission("AdminUser.Delete")]
        public ActionResult Delete(long id)
        {
            admUserService.MarkDeleted(id);
            return Json(new AjaxResult { Status = "ok" });
        }



        [CheckHasPermission("AdminUser.Delete")]
        public ActionResult BatchDelete(long[] selectedIds)
        {
            foreach (long id in selectedIds)
            {
                admUserService.MarkDeleted(id);
            }
            return Json(new AjaxResult { Status = "ok" });
        }
    }
}