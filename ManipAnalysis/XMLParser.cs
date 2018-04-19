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
        int tpNumber;
        public string[] tpTable;
        public string[] targetTable;
        public string[] loadTable;
        const string TPTABLEPATH = "/task_protocol/tptable/text()";
        const string TARGETTABLEPATH = "/task_protocol/targettable/text()";
        const string LOADTABLEPATH = "/task_protocol/loadtable/text()";
        const string SZENARIONAMEPATH = "/task_protocol/name/text()";
        const string TASKLEVELPARAMSPATH = "/task_protocol/tasklevelparams/text()";

        /// <summary>
        /// Gets a path to the dtp/xml file of an szenario and the trialNumberInSzenario to find the right trial in the dtp file
        /// 
        /// In the future this could also get improved by parsing the protocol.dtp that one can find in each folder that belongs to a specific subjects szenario.
        /// Then you must be able to get the names of each field from the protocol.dtp as you can not hardcode the indices anymore afaik.
        /// Would be much more elegant though!
        /// </summary>
        /// <param name="path">path to the dtp file</param>
        /// <param name="trialNumber">trialNumberInSzenario of the trial</param>
        /// <param name="trial">the trial in which to write</param>
        public XMLParser (string path, int tpNumber, Trial trial)
        {

            //We don't use the trialNumberInSzenario, we use the tpNumber!
            
            document = new XmlDocument();
            if (isValidDocument(path))
            {
                document.Load(path);
                this.tpNumber = tpNumber;
                tpTable = getTable(TPTABLEPATH);
                targetTable = getTable(TARGETTABLEPATH);
                loadTable = getTable(LOADTABLEPATH);
                this.trial = trial;
            } else
            {
                throw new Exception("Path leads to invalid document");
            }
            
        }
        public Trial parseTrial()
        {
            
            
            if (isValidTrial())
            {
                //trial.Target.Number gibt an, welches Target angesteuert wird:
                trial.Target.Number = getTrialEndTargetNumber();
                
                //SzenarioName eintragen
                //Not needed at the moment, because we can also get the szenarioName from the c3d reader
                //trial.Szenario = getSzenarioName();

                //ForceFieldType eintragen
                setForceFieldType();

                //TrialType eintragen
                setTrialType();

                //TrialHandedness eintragen
                setTrialHandedness();


                //Koordinaten des StartTargets eintragen
                trial.Origin.XPos = getTrialStartTargetPosition()[0] / 100.0f;
                trial.Origin.YPos = getTrialStartTargetPosition()[1] / 100.0f;
                trial.Origin.ZPos = getTrialStartTargetPosition()[2] / 100.0f;

                //Radius des StartTargets eintragen
                //trial.Origin.Radius = getTrialStartTargetRadius() / 2.0f / 100.0f; in some studies the ManipRadius was halfed, but it should not be so I think...
                trial.Origin.Radius = getTrialStartTargetRadius() / 100.0f;

                //Koordinaten des EndTargets eintragen
                trial.Target.XPos = getTrialEndTargetPosition()[0] / 100.0f;
                trial.Target.YPos = getTrialEndTargetPosition()[1] / 100.0f;
                trial.Target.ZPos = getTrialEndTargetPosition()[2] / 100.0f;

                //Radius des EndTargets eintragen
                //trial.Target.Radius = getTrialEndTargetRadius() / 2.0f / 100.0f; in some studies the ManipRadius was halfed, but it should not be so I think...
                trial.Target.Radius = getTrialEndTargetRadius() / 100.0f;


                //Default Values for the reference matrix to calculate certain statistics.
                //Might need to be dynamic later maybe(Handled by user input)?
                trial.ForceFieldMatrix[0, 0] = 0;
                trial.ForceFieldMatrix[0, 1] = 15;
                trial.ForceFieldMatrix[1, 0] = -15;
                trial.ForceFieldMatrix[1, 1] = 0;
            } else
            {
                trial = null;
            }
            

            return trial;
        }




        /// <summary>
        /// Checks if the trial has to be parsed or not
        /// Trials that should not be parsed because they don't contain any relevant data are:
        /// PositionControlTrials
        /// StartTrials
        /// </summary>
        /// <returns>true/false</returns>
        public bool isValidTrial()
        {
            //StartTrials in Simulink/BKIN: 
            //Start und EndTarget ist identisch


            if (getPositionControlEnabled() == 1) //PositionControlTrial you shall not parse
            {
                return false;
            //Wenn StartTarget und EndTarget identisch sind, werden keine relevanten Daten erzeugt.
            //StartTrials nutzen dies aus und können dadurch gefiltert werden.
            } else if(getTrialStartTargetNumber() == getTrialEndTargetNumber())
            {
                return false;
            }

            //TODO: Remove after it is not needed anymore!
            //This was used, because for some weird reason, the familiarization scenario had a different format in its .dtp file than any other scenario...
            //Also we do not need the familiarization in ManipAnalysis anyways so it does not matter.
            //#####################
            if (getSzenarioName().Contains("familiarization"))
            {
                return false;
            }
            //#####################
            return true;
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
                Console.WriteLine(path + e.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Given an xml path this method returns the content of this xml node, split by "]," and filtered
        /// </summary>
        /// <param name="path">xml path to the node to extract</param>
        /// <returns>string array of each entry in this node</returns>
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
                    //Filtering everything from the Strings except numbers, letters, points, minus, underscores and commas
                    parsedContent[i] = Regex.Replace(parsedContent[i], @"[^0-9A-Za-zäöüßÄÖÜ\.\-_,]", "");
                }
                return parsedContent;
            }
            else
            {
                throw new Exception ("Error parsing the xml file");
            }
        }


/*
------------------------------------------------------------------------------------------
The following methods provide easy access to the required metadata stored in the dtp file.
------------------------------------------------------------------------------------------
*/



        /// <summary>
        /// Returns an array with the X, Y and Z position of the StartTarget, stored in the 1st, 2nd and 3rd slot of the array
        /// </summary>
        /// <returns>array[X, Y, Z]</returns>
        public double[] getTrialStartTargetPosition()
        {
            /*

            WHAT TO DO WHEN THE START/END TARGETNUMBER IS ZERO?

            */
            double[] position = { 0, 0, 0 };
            int startTargetNumber = getTrialStartTargetNumber();
            //X-Position is always the first entry in the array split at the comma
            //To make parsing doubles possible, we need CultureInfo, for recognizing doubles with points instead of commas
            if (startTargetNumber > 0)
            {
                position[0] = double.Parse(targetTable[startTargetNumber - 1].Split(',')[0], CultureInfo.InvariantCulture);
                //Y-Position is always the second entry in the array split at the comma
                position[1] = double.Parse(targetTable[startTargetNumber - 1].Split(',')[1], CultureInfo.InvariantCulture);
                //Z-Value is always 0...
            }
            else
            {
                //Well what do we do if we have a Target 0 as StartTarget(means no starttarget was set in the protocol)
            }

            return position;
        }


        /// <summary>
        /// Returns the radius of the startTarget as double
        /// </summary>
        /// <returns>Radius</returns>
        public double getTrialStartTargetRadius()
        {
            double radius = 0;
            int startTargetNumber = getTrialStartTargetNumber();
            if (startTargetNumber > 0)
            {
                //Radius of the target is always stored at the 2nd index of the targetTable
                radius = double.Parse(targetTable[startTargetNumber - 1].Split(',')[2], CultureInfo.InvariantCulture);
            }
            else
            {
                //Do something when StartTargetNumber is 0
            }
            return radius;
        }


        /// <summary>
        /// Returns an array with the X, Y and Z position of the EndTarget, stored in the 1st, 2nd and 3rd slot of the array
        /// </summary>
        /// <returns>array[X, Y, Z]</returns>
        public double[] getTrialEndTargetPosition()
        {
            double[] position = { 0, 0, 0 };
            int endTargetNumber = getTrialEndTargetNumber();
            //X-Position is always the first entry in the array split at the comma
            //To make parsing doubles possible, we need CultureInfo, for recognizing doubles with points instead of commas
            if (endTargetNumber > 0)
            {
                position[0] = double.Parse(targetTable[endTargetNumber - 1].Split(',')[0], CultureInfo.InvariantCulture);
                //Y-Position is always the second entry in the array split at the comma
                position[1] = double.Parse(targetTable[endTargetNumber - 1].Split(',')[1], CultureInfo.InvariantCulture);
                //Z-Value is always 0...
            }
            else
            {
                //Do something when EndTargetNumber is 0 (thats the case, when the endtarget was not set in the protocol; should never happen actually
            }

            return position;
        }


        /// <summary>
        /// Returns the radius of the endTarget as double
        /// </summary>
        /// <returns>radius</returns>
        public double getTrialEndTargetRadius()
        {
            double radius = 0;
            int endTargetNumber = getTrialEndTargetNumber();
            if (endTargetNumber > 0)
            {
                //Radius of the target is always stored at the 2nd index of the targetTable
                radius = double.Parse(targetTable[endTargetNumber - 1].Split(',')[2], CultureInfo.InvariantCulture);
            } else
            {
                //Do something when EndTargetNumber is 0; should never happen actually
            }
            return radius;
        }


        /// <summary>
        /// Returns 1, if the trial has an active force channel, 0 otherwise
        /// </summary>
        /// <returns>1/0</returns>
        public int getForceChannelEnabled()
        {
            string ForceChannelEnabled = getTpTableEntry().Split(',')[5];
            return int.Parse(ForceChannelEnabled);
        }


        /// <summary>
        /// Returns 1, if the trial has active position control, 0 otherwise
        /// </summary>
        /// <returns>1/0</returns>
        public int getPositionControlEnabled()
        {
            string PositionControlEnabled = getTpTableEntry().Split(',')[6];
            return int.Parse(PositionControlEnabled);
        }


        /// <summary>
        /// Returns 1, if the trial has active DFForceField, 0 otherwise
        /// </summary>
        /// <returns>1/0</returns>
        public int getDFForceFieldEnabled()
        {
            string DFForceFieldEnabled = getTpTableEntry().Split(',')[7];
            return int.Parse(DFForceFieldEnabled);
        }


        /// <summary>
        /// Returns the SzenarioName as string
        /// </summary>
        /// <returns>SzenarioName as String</returns>
        public string getSzenarioName()
        {
            string output;
            //Because there is always just one szenarioname for each dtp file,
            //we can access it safe, by looking at the first entry of the array
            output = getTable(SZENARIONAMEPATH)[0];
            return output;
        }


        /// <summary>
        /// This method gives a string representing the ForceFieldType which can be:
        /// ForceFieldDF, ForceFieldCW, ForceFieldCCW or Nullfield.
        /// This String must then be translated into a ForceFieldTypeEnum and set in the trial object.
        /// This can not be done directly, because of internal classes and bad OO desig...
        /// </summary>
        /// <returns>string representing the ForceFieldTypeEnum</returns>
        private string getForceFieldType()
        {
            string ForceFieldType = "";
            float[] ForceFieldMatrix = getForceFieldMatrix();
            if (getDFForceFieldEnabled() == 1)
            {
                ForceFieldType = "ForceFieldDF";
            } else if (ForceFieldMatrix[1] < 0 && ForceFieldMatrix[2] > 0)
            {
                //When B < 0 && C > 0
                ForceFieldType = "ForceFieldCCW";
            } else if (ForceFieldMatrix[1] > 0 && ForceFieldMatrix[2] < 0)
            {
                //When B > 0 && C < 0!
                ForceFieldType = "ForceFieldCW";
            } else if (true)
            {
                //When A, B, C and D == 0
                //Should always be the case if the above conditions fail...
                ForceFieldType = "NullField";
            }
            return ForceFieldType;
        }


        /// <summary>
        /// Sets the proper ForceFieldType in the trial, because we can not create ForceFieldTypeEnums due to bad access rights
        /// </summary>
        public void setForceFieldType()
        {
            string ForceField = getForceFieldType();
            switch (ForceField)
            {
                case ("ForceFieldDF"):
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldDF;
                    break;
                case ("NullField"):
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.NullField;
                    break;
                case ("ForceFieldCW"):
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCW;
                    break;
                case ("ForceFieldCCW"):
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCCW;
                    break;        
            }
        }


        /// <summary>
        /// This method gives a string representing the TrialType which can be:
        /// ErrorClampTrial, PositionControlTrial, CatchTrial or StandardTrial.
        /// This String must then be translated into a TrialTypeEnum and set in the trial object.
        /// This can not be done directly, because of internal classes and bad OO desig...
        /// </summary>
        /// <returns>string representing the TrialTypeEnum</returns>
        private string getTrialType()
        {
            string TrialType = "";
            if (getForceChannelEnabled() == 1)
            {
                TrialType = "ErrorClampTrial";
            } else if(getPositionControlEnabled() == 1)
            {
                TrialType = "PositionControlTrial";
            //} else if (false)
            //{
            //    // TODO: Currently there should not be CatchTrials as an independent trialType. This should be removed in the future.
            //    TrialType = "CatchTrial";
            } else
            {
                TrialType = "StandardTrial";
            }
            return TrialType;
        }


        /// <summary>
        /// Sets the proper TrialType in the trial, because we can not create TrialTypeEnums due to bad access rights
        /// </summary>
        public void setTrialType()
        {
            string TrialType = getTrialType();
            switch (TrialType)
            {
                case ("ErrorClampTrial"):
                    trial.TrialType = Trial.TrialTypeEnum.ErrorClampTrial;
                    break;
                case ("PositionControlTrial"):
                    trial.TrialType = Trial.TrialTypeEnum.PositionControlTrial;
                    break;
                case ("Standardtrial"):
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                    break;
            }
        }

        /// <summary>
        /// This method gives a string representing the Handedness which can be 0 or 1, with 0 being RightHand and 1 being LeftHand
        /// Additionally it can be implemented to translate numbers 2 and 3 to RigthHandVicon or LeftHandVicon if needed in the future.
        /// This string then must be translated into a HandednessEnum with the setHandednessFunction
        /// </summary>
        /// <returns>0 or 1 for RightHand or LeftHand</returns>
        private string getHandedness()
        {
            //Handedness can be found in 1st Entry of the TaskLevelParams as it must be set as global parameter in BKIN now
            string handednessEntry;
            string handedness = "0";//Handedness is set to be RightHand by default unless stated otherwise
            try {
                handednessEntry = getTable(TASKLEVELPARAMSPATH)[0];
                handedness = handednessEntry.Split(',')[1];
            } catch (IndexOutOfRangeException e)
            {
                Console.WriteLine(e);
                //Handedness was not set so it will stay default on 0...
            }
            return handedness;
        }

        /// <summary>
        /// Sets the proper Handedness in the trial, because we can not create HandednessEnums due to bad access rights
        /// </summary>
        public void setTrialHandedness()
        {
            string handedness = getHandedness();
            switch (handedness)
            {
                case ("0"):
                    trial.Handedness = Trial.HandednessEnum.RightHand;
                    break;
                case ("1"):
                    trial.Handedness = Trial.HandednessEnum.LeftHand;
                    break;
                case ("2"):
                    //Is not explained in the TaskLevelTextbox in Dexterit 3.6 so far, because it is not used so far
                    trial.Handedness = Trial.HandednessEnum.RightHandVicon;
                    break;
                case ("3"):
                    //Is not explained in the TaskLevelTextbox in Dexterit 3.6 so far, because it is not used so far
                    trial.Handedness = Trial.HandednessEnum.LeftHandVicon;
                    break;
            }
        }





        /*
        ----------------------------------
        HelperFunctions for better clarity
        ----------------------------------
        */



        /// <summary>
        /// Gives the TpTableEntry of a given Trial
        /// </summary>
        /// <returns></returns>
        private string getTpTableEntry()
        {
            //The tpTable can be accessed with the tpNumber from the *.c3d TP_NUM field
            if (tpNumber >= 40 && tpNumber <= 50)
            {

            }
            return tpTable[tpNumber - 1];
        }


        /// <summary>
        /// Delivers the index of the targetTable for the StartTarget of the given trial
        /// </summary>
        /// <returns>index into targetTable as int</returns>
        private int getTrialStartTargetNumber()
        {
            string startTarget = getTpTableEntry().Split(',')[0];
            return int.Parse(startTarget);
        }


        /// <summary>
        /// Delivers the index of the targetTable for the EndTarget of the given trial
        /// </summary>
        /// <returns>index into targetTable as int</returns>
        private int getTrialEndTargetNumber()
        {
            string endTarget = getTpTableEntry().Split(',')[1];
            return int.Parse(endTarget);
        }

        
        /// <summary>
        /// Delivers the index in the loadTable for the given trial
        /// </summary>
        /// <returns></returns>
        private int getForceFieldColumn()
        {
            string ForceFieldColumn = getTpTableEntry().Split(',')[4];
            //TODO: Remove
            if (ForceFieldColumn == "4")
            {

            }
            return int.Parse(ForceFieldColumn);
        }


        /// <summary>
        /// Delivers the A, B, C and D value of the rotational forcefield matrix in this order
        /// </summary>
        /// <returns>{A, B, C, D}</returns>
        public float[] getForceFieldMatrix()
        {
            float[] matrix = { 0, 0, 0, 0 };
            string[] forceFieldEntries = loadTable[getForceFieldColumn() - 1].Split(',');
            for (int i = 0; i < 4; i++)
            {
                //The first 4 entries of the loadTable represent the entries A, B, C and D for the matrix
                //string[] forceFieldEntries = loadTable[trial.TrialNumberInSzenario - 1].Split(',');
                
                matrix[i] = float.Parse(forceFieldEntries[i], CultureInfo.InvariantCulture);
            }
            //TODO: Remove
            if (matrix[1] <= 0 || matrix[2] >= 0)
            {

            }
            return matrix;
        }
    }
}