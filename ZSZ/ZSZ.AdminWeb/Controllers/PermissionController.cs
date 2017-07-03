using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZSZ.AdminWeb.App_Start;
using ZSZ.AdminWeb.Models;
using ZSZ.CommonMVC;
using ZSZ.IService;

namespace ZSZ.AdminWeb.Controllers
{
    public class PermissionController : Controller
    {
        //权限列表
        // 属性注入
        public IPermissionService PermSvc { get; set; }
        //autofac会自动赋一个实现类


        [CheckHasPermission("Permission.List")]

        public ActionResult List()
        {
            //获得所有权限项内容
            var perms = PermSvc.GetAll();
            return View(perms);
        }


        [CheckHasPermission("Permission.Delete")]
        public ActionResult GetDelete(long id)
        {
            PermSvc.MarkDeleted(id);
            //return RedirectToAction("List");
            //删除之后刷新
            return RedirectToAction(nameof(List));
            //编译器会检查有没有List，最后还是编译成"List"
        }


        [CheckHasPermission("Permission.Delete")]
        //ajax删除
        public ActionResult Delete2(long id)
        {
            PermSvc.MarkDeleted(id);
            return Json(new AjaxResult { Status = "ok" });
        }


        [CheckHasPermission("Permission.Add")]
        //增加权限
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }


        [CheckHasPermission("Permission.Add")]
        [HttpPost]
        //public ActionResult Add(string name,string description)
        //使用Model类的方式传递参数,可以使用MVC的校验
        public ActionResult Add(PermissionAddNewModel model)
        {
            PermSvc.AddPermission(model.Name, model.Description);
            //return RedirectToAction(nameof(List));
            //todo:要检查权限项名字不能重复
            return Json(new AjaxResult { Status = "ok" });
        }


        [CheckHasPermission("Permission.Edit")]
        //修改权限
        [HttpGet]
        public ActionResult Edit(long id)
        {
            var perm = PermSvc.GetById(id);
            return View(perm);
        }


        [CheckHasPermission("Permission.Edit")]
        [HttpPost]
        public ActionResult Edit(PermissionEditModel model)
        {
            PermSvc.UpdatePermIds(model.Id, model.Name, model.Description);
            //todo:检查用户名不能重复
            return Json(new AjaxResult { Status = "ok" });
        }
    }
}