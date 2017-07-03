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
    public class RoleController : Controller
    {
        public IRoleService roleService { get; set; }
        //IRoleService roleService = new RoleService();
        public IPermissionService permService { get; set; }

        [CheckHasPermission("Role.List")]
        // GET: Role
        public ActionResult List()
        {
            var roles = roleService.GetAll();
            return View(roles);
        }

        [CheckHasPermission("Role.Delete")]
        public ActionResult Delete(long id)
        {
            roleService.MarkDeleted(id);
            return Json(new AjaxResult { Status = "ok" });
        }

        [CheckHasPermission("Role.Delete")]
        public ActionResult BatchDelete(long[] selectedIds)
        {
            //删除大量数据需要优化（删除的SQL原生语句效率高一点）
            //要考虑事务问题,如果出错不能一部分数据删除一部分没有被删除
            foreach (long id in selectedIds)
            {
                roleService.MarkDeleted(id);
            }
            return Json(new AjaxResult { Status = "ok" });
        }

        [CheckHasPermission("Role.Add")]
        [HttpGet]
        public ActionResult Add()
        {
            //获得所有可用的权限项
            var perms = permService.GetAll();
            return View(perms);
        }

        [CheckHasPermission("Role.Add")]
        [HttpPost]
        public ActionResult Add(RoleAddModel model)
        {
            //检查Model验证是否通过
            if (!ModelState.IsValid)
            {
                return Json(new AjaxResult { Status = "error", ErrorMsg = MVCHelper.GetValidMsg(ModelState) });
            }
            //事务问题！！！
            //假如role新增成功，permission新增失败，就会产生事务问题
            long roleId = roleService.AddNew(model.Name);
            permService.AddPermIds(roleId, model.PermissionIds);
            return Json(new AjaxResult { Status = "ok" });
        }

        [CheckHasPermission("Role.Edit")]
        [HttpGet]
        public ActionResult Edit(long id)
        {
            //根据Id取得角色信息和角色拥有的权限
            var role = roleService.GetById(id);
            //这个角色拥有的权限
            var rolePerms = permService.GetByRoleId(id);
            //编辑的时候要把所有的权限也要列出来
            var allPerm = permService.GetAll();
            //给视图传数据，尽量不要用viewbag，就用Model
            //弱类型,动态对象，效率较低
            /*
            ViewBag.role = role;
            ViewBag.rolePerms = rolePerms;
            */
            RoleEditGetModel model = new RoleEditGetModel();
            model.AllPerms = allPerm;
            model.Role = role;
            model.RolePerms = rolePerms;
            return View(model);
        }

        [CheckHasPermission("Role.Edit")]
        [HttpPost]
        public ActionResult Edit(RoleEditPostModel model)
        {
            //修改角色名称
            roleService.Update(model.Id, model.Name);
            //修改权限项
            permService.UpdatePermIds(model.Id, model.PermissionIds);
            return Json(new AjaxResult() { Status = "ok" });
        }
    }
}