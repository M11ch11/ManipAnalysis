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
        string path = @"C:\Users\Administrator\Desktop\BKINFiles\SampleDTPFiles\general_study\01_TestForAllTrials.dtp";
        [TestMethod()]
        public void XMLParserTest()
        {
            parser = new XMLParser(path, 10);

            Console.WriteLine("TpTable: ");
            for (int i = 0; i < parser.tpTable.Count(); i++)
            {
                Console.WriteLine(parser.tpTable[i]);
            }

            Console.WriteLine("TargetTable: ");
            for (int i = 0; i < parser.targetTable.Count(); i++)
            {
                Console.WriteLine(parser.targetTable[i]);
            }

            Console.WriteLine("LoadTable: ");
            for (int i = 0; i < parser.loadTable.Count(); i++)
            {
                Console.WriteLine(parser.loadTable[i]);
            }
        }

        [TestMethod()]
        public void getTpTableEntryTest()
        {
            parser = new XMLParser(path, 10);
            Console.WriteLine(parser.getTpTableEntry());
        }

        [TestMethod()]
        public void getTrialStartTargetTest()
        {
            parser = new XMLParser(path, 10);
            Console.WriteLine(parser.getTrialStartTarget());
        }

        [TestMethod()]
        public void getTrialEndTargetPositionTest()
        {
            parser = new XMLParser(path, 3);
            for (int i = 0; i < parser.getTrialEndTargetPosition().Count(); i++)
            {
                Console.WriteLine(parser.getTrialEndTargetPosition()[i]);
            }
        }

        [TestMethod()]
        public void getTrialStartTargetPositionTest()
        {
            parser = new XMLParser(path, 3);
            for (int i = 0; i < parser.getTrialStartTargetPosition().Count(); i++)
            {
                Console.WriteLine(parser.getTrialStartTargetPosition()[i]);
            }
        }
    }
}