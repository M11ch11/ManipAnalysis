using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManipAnalysis_v2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ManipAnalysis_v2.Tests
{
    [TestClass()]
    public class C3DReaderTests
    {
        C3DReader c3dreader = new C3DReader();
        string path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), @"TestFiles\01_01_01.c3d");
        [TestMethod()]
        public void ReadFrameTest()
        {
            Assert.Fail();
        }
    }
}