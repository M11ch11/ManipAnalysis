using ManipAnalysis_v2.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ManipAnalysis_v2
{
    class XMLParser
    {
        XmlDocument document;
        Trial trial;

        public XMLParser (string path, int trialNumber)
        {
            //Assertion: The Trial_Num in the c3d file can be identified and it actually represents the TrialNumberInSzenario
            document = new XmlDocument();
            document.LoadXml(path);
            trial.TrialNumberInSzenario = trialNumber;
        }
        //Mit der trialNumber kann dann der tpTable durchsucht werden (Trialnumber = Index im TpTable)

        private string getTpTableEntry()
        {
            XmlNode node = document.SelectSingleNode("//task_protocol/tp_table/text()");

            return "null";
        }
        public int getTrialTargetNumber()
        {

            return 0;
        }
    }
}
