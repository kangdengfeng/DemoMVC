using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSZ.Service;

namespace ServiceTests
{
    [TestClass]
    public class AdminUserServiceTest
    {
        private AdminUserService userService =
            new AdminUserService();
        [TestMethod]
        public void TestAddAdminUser()
        {
            //手机号不能重复
            //插入之后再删除
            long uid = userService.AddAdminUser("abc", "1233211", "123", "2432767038@qq.com", null);
            var user = userService.GetById(uid);
            Assert.AreEqual(user.Name, "abc");
            Assert.AreEqual(user.PhoneNum, "1233211");
            Assert.AreEqual(user.Email, "2432767038@qq.com");
            Assert.IsNull(user.CityId);
            Assert.IsTrue(userService.CheckLogin("1233211", "123"));
            Assert.IsFalse(userService.CheckLogin("1233211", "abc"));
            userService.GetAll();
            Assert.IsNotNull(userService.GetByPhoneNum("123321"));
            //为了保证测试案例可以重复执行，删除数据
            userService.MarkDeleted(uid);
        }
        [TestMethod]
        public void TestHasPerm()
        {
            try
            {
                //角色添加，权限添加，用户添加
                PermissionService perms = new PermissionService();
                //通过Guid保证产生的字符串不会重复
                //string pername1 = Guid.NewGuid().ToString();
                //long permId1 = perms.AddPermission(pername1, pername1);
                //string pername2 = Guid.NewGuid().ToString();
                //long permId2 = perms.AddPermission(pername2, pername2);

                RoleService roleService = new RoleService();
                //string roleName1 = Guid.NewGuid().ToString();
                //long roleId = roleService.AddNew(roleName1);

                //string userPhone = "1233211234567";
                //long userId = userService.AddAdminUser("aaa", userPhone, "123", "123@qq.com", null);

                //roleService.AddRoleIds(userId, new long[] { roleId });
                //perms.AddPermIds(roleId, new long[] { permId1 });
                Assert.IsTrue(userService.HasPermission(4, "AdminUser.List"));
                //Assert.IsFalse(userService.HasPermission(userId, pername2));
            }
            catch (DbEntityValidationException everr)
            {
                foreach (var item in everr.EntityValidationErrors.SelectMany(eer=>eer.ValidationErrors))
                {
                    Console.WriteLine(item.ErrorMessage);
                }
                throw;
            }
           
        }
    }
}
