using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZSZ.Service;

namespace ServiceTests
{
    [TestClass]
    public class UnitTestAdminLog
    {
        [TestMethod]
        public void TestAddNew()
        {
            long id = new AdminLogService().AddNew(4, "测试消息");
            var log = new AdminLogService().GetById(id);
            Assert.AreEqual(log.Message, "测试消息");
        }
    }
}
