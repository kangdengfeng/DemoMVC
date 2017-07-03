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
    public class RoleServiceUnitTest
    {
        private RoleService roleService = new RoleService();
        [TestMethod]
        public void TestRole()
        {
           Assert.IsNotNull(roleService.GetByName("最高管理员"));
        }
    }
}
