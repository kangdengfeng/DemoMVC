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
    public class IdNameServiceUnit
    {
        private IdNameService inSer = new IdNameService();
        [TestMethod]
        public void Test1()
        {
            string typeName = Guid.NewGuid().ToString();
            string name1 = Guid.NewGuid().ToString();
            string name2 = Guid.NewGuid().ToString();
            long id1 = inSer.AddNew(typeName, name1);
            long ud2 = inSer.AddNew(typeName, name2);

            Assert.AreEqual(inSer.GetById(id1).Name, name1);
            Assert.AreEqual(inSer.GetAll(typeName).Length, 2);
            Assert.AreEqual(inSer.GetAll(typeName)[1].Name, name2);
        }
    }
}
