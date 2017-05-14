using ManipAnalysis_v2.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ManipAnalysis_v2
{
    public class XMLParser
    {
        XmlDocument document;
        Trial trial = new Trial();

        public XMLParser (string path, int trialNumber)
        {
            //Assertion: The Trial_Num in the c3d file can be identified and it actually represents the TrialNumberInSzenario
            document = new XmlDocument();
            document.Load(path);
            trial.TrialNumberInSzenario = trialNumber;
        }
        //Mit der trialNumber kann dann der tpTable durchsucht werden (Trialnumber = Index im TpTable)

        public string getTpTableEntry()
        {
            XmlNode node = document.SelectSingleNode("/task_protocol/tptable/text()");
            if (node != null)
            {
                return node.Value;
            }
            else
            {
                return "Error reading the node";
            }
        }
        public int getTrialTargetNumber()
        {

            return 0;
        }
    }
}
