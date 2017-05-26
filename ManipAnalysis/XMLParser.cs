using ManipAnalysis_v2.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace ManipAnalysis_v2
{
    public class XMLParser
    {
        XmlDocument document;
        Trial trial = new Trial();
        string[] tpTable;

        public XMLParser (string path, int trialNumber)
        {
            //Assertion: The Trial_Num in the c3d file can be identified and it actually represents the TrialNumberInSzenario
            document = new XmlDocument();
            document.Load(path);
            trial.TrialNumberInSzenario = trialNumber;
            getTpTable();
        }
        //Mit der trialNumber kann dann der tpTable durchsucht werden (Trialnumber = Index im TpTable)

        private void getTpTable()
        {
            XmlNode node = document.SelectSingleNode("/task_protocol/tptable/text()");
            string content;
            string[] parsedContent;
            if (node != null)
            {
                content = node.Value;
                //Separating the TpTableEntries via the closing brackets char
                parsedContent = Regex.Split(content, "],");
                for (int i = 0; i < parsedContent.Length; i++)
                {
                    //Filtering everything from the Strings except numbers and commas
                    parsedContent[i] = Regex.Replace(parsedContent[i], @"[^0-9,]", "");
                }
                tpTable = parsedContent;
            }
            else
            {
                throw new Exception ("Error parsing the xml file");
            }
        }

        public string getTpTableEntry()
        {
            //Enumeration of the TrialNumberInSzenario starts at 1 so decrement it first
            return tpTable[trial.TrialNumberInSzenario - 1];
        }
        public int getTrialStartTarget()
        {
            string startTarget = getTpTableEntry().Split(',')[0];
            return int.Parse(startTarget);
        }

        public int getTrialEndTarget()
        {
            string endTarget = getTpTableEntry().Split(',')[1];
            return int.Parse(endTarget);
        }

        public int getForceFieldColumn()
        {
            string ForceFieldColumn = getTpTableEntry().Split(',')[4];
            return int.Parse(ForceFieldColumn);
        }

        public int getForceChannelEnabled()
        {
            string ForceChannelEnabled = getTpTableEntry().Split(',')[5];
            return int.Parse(ForceChannelEnabled);
        }

        public int getPositionControlEnabled()
        {
            string PositionControlEnabled = getTpTableEntry().Split(',')[6];
            return int.Parse(PositionControlEnabled);
        }

        public int getDFForceFieldEnabled()
        {
            string DFForceFieldEnabled = getTpTableEntry().Split(',')[7];
            return int.Parse(DFForceFieldEnabled);
        }
    }
}
