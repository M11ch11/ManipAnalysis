using ManipAnalysis_v2.MongoDb;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        public string[] tpTable;
        public string[] targetTable;
        public string[] loadTable;
        const string TPTABLEPATH = "/task_protocol/tptable/text()";
        const string TARGETTABLEPATH = "/task_protocol/targettable/text()";
        const string LOADTABLEPATH = "/task_protocol/loadtable/text()";

        /// <summary>
        /// Gets a path to an dtp/xml file and the trialNumberInSzenario to find the right trial in the dtp file
        /// </summary>
        /// <param name="path">path to the dtp file</param>
        /// <param name="trialNumber">trialNumberInSzenario of the trial</param>
        public XMLParser (string path, int trialNumber)
        {
            //Assertion: The Trial_Num in the c3d file can be identified and it actually represents the TrialNumberInSzenario
            document = new XmlDocument();
            document.Load(path);
            trial.TrialNumberInSzenario = trialNumber;
            tpTable = getTable(TPTABLEPATH);
            targetTable = getTable(TARGETTABLEPATH);
            loadTable = getTable(LOADTABLEPATH);
        }


        /// <summary>
        /// Checks if the file under the given path is a valid xml file
        /// </summary>
        /// <param name="path">full path to the file</param>
        /// <returns>true or false, depending on validity of the file</returns>
        public static bool isValidDocument (string path)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(path);
                return true;
            } catch (Exception e)
            {
                return false;
            }
        }


        

        private string[] getTable(string path)
        {
            XmlNode node = document.SelectSingleNode(path);
            string content;
            string[] parsedContent;
            if (node != null)
            {
                content = node.Value;
                //Separating the TableEntries via the closing brackets char
                parsedContent = Regex.Split(content, "],");
                for (int i = 0; i < parsedContent.Length; i++)
                {
                    //Filtering everything from the Strings except numbers, letters, points, minus and commas
                    parsedContent[i] = Regex.Replace(parsedContent[i], @"[^0-9A-Za-zäöüßÄÖÜ\.\-,]", "");
                }
                return parsedContent;
            }
            else
            {
                throw new Exception ("Error parsing the xml file");
            }
        }


        /// <summary>
        /// Gives the TpTableEntry of a given Trial
        /// </summary>
        /// <returns></returns>
        public string getTpTableEntry()
        {
            //Mit der trialNumber kann der tpTable durchsucht werden (Trialnumber = Index im TpTable)
            //Enumeration of the TrialNumberInSzenario starts at 1 so decrement it first
            /*

            WHAT TO DO WHEN TRIALNUMBERINSZENARIO IS ZERO? IS THIS POSSIBLE?

            */
            return tpTable[trial.TrialNumberInSzenario - 1];
        }
        public int getTrialStartTarget()
        {
            string startTarget = getTpTableEntry().Split(',')[0];
            return int.Parse(startTarget);
        }



        private int getTrialEndTarget()
        {
            string endTarget = getTpTableEntry().Split(',')[1];
            return int.Parse(endTarget);
        }

        /// <summary>
        /// Returns an array with the X, Y and Z position of the EndTarget, stored in the 1st, 2nd and 3rd slot of the array
        /// </summary>
        /// <returns>array[X, Y, Z]</returns>
        public double[] getTrialEndTargetPosition()
        {
            double[] position = { 0, 0, 0 };
            int endTargetNumber = getTrialEndTarget();
            //X-Position is always the first entry in the array split at the comma
            //To make parsing doubles possible, we need CultureInfo, for recognizing doubles with points instead of commas
            position[0] = double.Parse(targetTable[endTargetNumber - 1].Split(',')[0], CultureInfo.InvariantCulture);
            //Y-Position is always the second entry in the array split at the comma
            position[1] = double.Parse(targetTable[endTargetNumber - 1].Split(',')[1], CultureInfo.InvariantCulture);
            //Z-Value is always 0...
            return position;
        }

        /// <summary>
        /// Returns an array with the X, Y and Z position of the EndTarget, stored in the 1st, 2nd and 3rd slot of the array
        /// </summary>
        /// <returns>array[X, Y, Z]</returns>
        public double[] getTrialStartTargetPosition()
        {
            /*

            WHAT TO DO WHEN THE START/END TARGETNUMBER IS ZERO?

            */
            double[] position = { 0, 0, 0 };
            int startTargetNumber = getTrialStartTarget();
            //X-Position is always the first entry in the array split at the comma
            //To make parsing doubles possible, we need CultureInfo, for recognizing doubles with points instead of commas
            position[0] = double.Parse(targetTable[startTargetNumber - 1].Split(',')[0], CultureInfo.InvariantCulture);
            //Y-Position is always the second entry in the array split at the comma
            position[1] = double.Parse(targetTable[startTargetNumber - 1].Split(',')[1], CultureInfo.InvariantCulture);
            //Z-Value is always 0...
            return position;
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
