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
    public class SettingUnitTest
    {
        [TestMethod]
        public void Test1()
        {
            SettingService setSvc = new SettingService();
            string name = Guid.NewGuid().ToString();
            string value = Guid.NewGuid().ToString();
            Assert.IsNull(setSvc.GetValue(name));
            setSvc.SetValue(name, value);
            Assert.AreEqual(value, setSvc.GetValue(name));
            setSvc.SetIntValue(name, 3);
            Assert.AreEqual(setSvc.GetIntValue(name), 3);
        }
    }
}
