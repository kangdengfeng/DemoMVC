using Calc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject1
{
    [TestClass]
    public class TestFileUtils
    {
        [TestMethod]
        public void Test1()
        {
            Assert.IsFalse(FileUtils.IsFileExists("d:/1.txt"));
            FileUtils.WriteFile("d:/1.txt","abc123");
            string s = FileUtils.ReadFile("d:/1.txt");
            Assert.AreEqual(s,"abc123");
        }
        //测试类执行前执行
        [ClassInitialize]
        public static void InitClass(TestContext ctx)
        {
            File.Delete("d:/1.txt");
        }
        //可用来测试案例执行后恢复数据
        /*
        [ClassCleanup]
        public static  void CleanClass()
        {
            File.Delete("d:/1.txt");
        }
        */
    }
}
