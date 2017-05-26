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
    public class XMLParserTests
    {
        XMLParser parser;
        [TestMethod()]
        public void XMLParserTest()
        {
            parser = new XMLParser(@"C:\RL_DF.dtp", 10);
        }

        [TestMethod()]
        public void getTpTableEntryTest()
        {
            parser = new XMLParser(@"C:\RL_DF.xml", 10);
            Console.WriteLine(parser.getTpTableEntry());
        }

        [TestMethod()]
        public void getTrialStartTargetTest()
        {
            parser = new XMLParser(@"C:\RL_DF.dtp", 10);
            Console.WriteLine(parser.getTrialStartTarget());
        }
    }
}