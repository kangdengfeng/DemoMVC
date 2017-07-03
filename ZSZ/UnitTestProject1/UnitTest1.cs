using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calc;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Assert.AreEqual(2, Class1.Add(1, 1));
            Assert.AreEqual(0, Class1.Add(-1, 1));
            Assert.AreEqual(1, Class1.Add(0, 1));
        }
    }
}
