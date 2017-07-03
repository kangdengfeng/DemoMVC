using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSZ.Service;

namespace ServiceTests
{
    [TestClass]
    
    public class RBACUnitTest
    {
        private static RoleService roleService = new RoleService();
        private static PermissionService permService = new PermissionService();
        private static AdminUserService admService = new AdminUserService();
        private static long userId;
        [TestMethod]
        public void TestUserRole()
        {
            //角色、权限、用户添加
            string permName1 = Guid.NewGuid().ToString();
            long perm1Id = permService.AddPermission(permName1, permName1);
            string permName2 = Guid.NewGuid().ToString();
            long perm2Id = permService.AddPermission(permName2, permName2);

            string roleName1 = Guid.NewGuid().ToString();
            long role1Id =  roleService.AddNew(roleName1);
            string roleName2 = Guid.NewGuid().ToString();
            long role2Id = roleService.AddNew(roleName2);

            string userPhone = "178158";
            userId = admService.AddAdminUser("aaa", userPhone, "123", "123@qq.com", null);

            roleService.AddRoleIds(userId, new long[] { role1Id });
            permService.AddPermIds(role1Id, new long[] { perm1Id });
            Assert.IsTrue(admService.HasPermission(userId, permName1));

            roleService.UpdateRoleIds(userId, new long[] { role2Id });
            Assert.IsFalse(admService.HasPermission(userId, permName1));
            CollectionAssert.AreEqual(roleService.GetByAdminUserId(userId)
                .Select(r => r.Id).ToArray(), 
                new long[] { role2Id });
        }
        [ClassCleanup]
        public static void CleanUp()
        {
            if (userId!=0)
            {
                admService.MarkDeleted(userId);
            }
        }
    }
}
