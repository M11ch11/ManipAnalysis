using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManipAnalysis_v2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ManipAnalysis_v2.MongoDb;

namespace ManipAnalysis_v2.Tests
{
    [TestClass()]
    public class XMLParserTests
    {
        XMLParser parser;
        //string path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), @"TestFiles\01_TestForAllTrials.dtp");
        string path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), @"TestFiles\03_MICIE_blocked_V1.dtp");
        Trial t1 = new Trial();
        int tpNumber = 41;
        [TestMethod()]
        public void XMLParserTest()
        {
            parser = new XMLParser(path, tpNumber, t1);

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

        //Former public methods, for testing only, now private
        //[TestMethod()]
        //public void getTpTableEntryTest()
        //{
        //    parser = new XMLParser(path, 10);
        //    Console.WriteLine(parser.getTpTableEntry());
        //}

        //Former public methods, for testing only, now private
        //[TestMethod()]
        //public void getTrialStartTargetTest()
        //{
        //    parser = new XMLParser(path, 10);
        //    Console.WriteLine(parser.getTrialStartTarget());
        //}

        [TestMethod()]
        public void getTrialEndTargetPositionTest()
        {
            parser = new XMLParser(path, 3, t1);
            for (int i = 0; i < parser.getTrialEndTargetPosition().Count(); i++)
            {
                Console.WriteLine(parser.getTrialEndTargetPosition()[i]);
                switch (i)
                {
                    case 0:
                        Assert.AreEqual(parser.getTrialEndTargetPosition()[i], 8.66);
                        break;
                    case 1:
                        Assert.AreEqual(parser.getTrialEndTargetPosition()[i], -5);
                        break;
                    case 2:
                        Assert.AreEqual(parser.getTrialEndTargetPosition()[i], 0);
                        break;
                }
            }
        }

        [TestMethod()]
        public void getTrialStartTargetPositionTest()
        {
            parser = new XMLParser(path, 3, t1);
            for (int i = 0; i < parser.getTrialStartTargetPosition().Count(); i++)
            {
                Console.WriteLine(parser.getTrialStartTargetPosition()[i]);
                Assert.AreEqual(parser.getTrialStartTargetPosition()[i], 0);
            }
        }

        [TestMethod()]
        public void getSzenarioNameTest()
        {
            parser = new XMLParser(path, tpNumber, t1);
            Console.WriteLine(parser.getSzenarioName());
            Assert.AreEqual(parser.getSzenarioName(), "03_MICIE_blocked_V1");
        }

        [TestMethod()]
        public void isValidTrialTest()
        {
            parser = new XMLParser(path, tpNumber, t1);
            Console.WriteLine(parser.isValidTrial());
        }

        [TestMethod()]
        public void getForceFieldMatrixTest()
        {
            parser = new XMLParser(path, tpNumber, t1);
            float[] matrix = parser.getForceFieldMatrix();
            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine(matrix[i]);
                switch (i)
                {
                    case 0:
                        Assert.AreEqual(0, matrix[i]);
                        break;
                    case 1:
                        Assert.AreEqual(22, matrix[i]);
                        break;
                    case 2:
                        Assert.AreEqual(-22, matrix[i]);
                        break;
                    case 3:
                        Assert.AreEqual(0, matrix[i]);
                        break;
                }
            }
        }

        //Former public methods, for testing only, now private


        //[TestMethod()]
        //public void getForceFieldMatrixTest()
        //{
        //    parser = new XMLParser(path, 3);
        //    float[] matrix = parser.getForceFieldMatrix();
        //    for (int i = 0; i < matrix.Count(); i++)
        //    {
        //        Console.WriteLine(matrix[i]);
        //    }
        //}


        /*
        [TestMethod()]
        public void getForceFieldColumnTest()
        {
            parser = new XMLParser(path, tpNumber, t1);
            int loadColumn = parser.getForceFieldColumn();
            Console.WriteLine(loadColumn);
            Assert.AreEqual(3, loadColumn);
        }
        */
    }
}