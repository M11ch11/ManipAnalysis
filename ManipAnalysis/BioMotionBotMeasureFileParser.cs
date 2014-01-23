using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ManipAnalysis_v2.Container;

namespace ManipAnalysis_v2
{
    internal class BioMotionBotMeasureFileParser
    {
        private readonly DataContainer _dataContainer;
        private readonly ManipAnalysisGui _myManipAnalysisGui;
        private string _measureFilePath;

        public BioMotionBotMeasureFileParser(DataContainer container, ManipAnalysisGui myManipAnalysisGui)
        {
            _myManipAnalysisGui = myManipAnalysisGui;
            _dataContainer = container;
        }

        public bool ParseFile(string path)
        {
            _measureFilePath = path;

            bool retVal = ParseFileInfo();

            if (retVal)
            {
                retVal = ParseMeasureData();
            }

            return retVal;
        }

        private bool ParseFileInfo()
        {
            bool retVal = false;

            string filenameInfoString =
                _measureFilePath.Substring(_measureFilePath.LastIndexOf("\\Szenario", StringComparison.Ordinal) + 1);
            filenameInfoString = filenameInfoString.Remove(filenameInfoString.IndexOf(".csv", StringComparison.Ordinal));
            string[] filenameInfoStringArray = filenameInfoString.Split('-');

            /*
            if (filenameInfoStringArray.Count() == 6)   // Study 1
            {
                dataContainer.measureFileHash = MD5.computeHash(measureFilePath);
                dataContainer.studyName = "Study 1";
                dataContainer.szenarioName = filenameInfoStringArray[0].Trim();
                dataContainer.groupName = filenameInfoStringArray[5].Trim();
                dataContainer.subjectName = filenameInfoStringArray[4].Trim();
                dataContainer.subjectID = filenameInfoStringArray[3].Trim();
                dataContainer.measureFileCreationDate = filenameInfoStringArray[1];
                dataContainer.measureFileCreationTime = filenameInfoStringArray[2].Replace('.', ':');

                retVal = true;
            }
            */
            if (filenameInfoStringArray.Count() == 7) // Study 2 and above
            {
                _dataContainer.MeasureFileHash = Md5.ComputeHash(_measureFilePath);
                _dataContainer.StudyName = filenameInfoStringArray[3].Trim();
                _dataContainer.SzenarioName = filenameInfoStringArray[0].Trim();
                _dataContainer.GroupName = filenameInfoStringArray[4].Trim();
                _dataContainer.SubjectName = filenameInfoStringArray[5].Trim();
                _dataContainer.SubjectID = filenameInfoStringArray[6].Trim();
                _dataContainer.MeasureFileCreationDate = filenameInfoStringArray[1];
                _dataContainer.MeasureFileCreationTime = filenameInfoStringArray[2].Replace('.', ':');

                retVal = true;
            }

            return retVal;
        }

        private bool ParseMeasureData()
        {
            bool retVal = true;

            if (_dataContainer.MeasureFileHash != null)
            {
                var measureFileStream = new FileStream(_measureFilePath, FileMode.Open, FileAccess.Read);
                var measureFileReader = new StreamReader(measureFileStream);
                _dataContainer.MeasureDataRaw.Clear();

                int expectedSzenarioTrialCount = 0;
                int expectedTargetTrialCount = 0;

                if (_dataContainer.SzenarioName == "Szenario02" ||
                    _dataContainer.SzenarioName == "Szenario30" ||
                    _dataContainer.SzenarioName == "Szenario42_N" ||
                    _dataContainer.SzenarioName == "Szenario42_R"
                    )
                {
                    expectedSzenarioTrialCount = 96;
                    expectedTargetTrialCount = 6;
                }
                else if (_dataContainer.SzenarioName == "Szenario03" ||
                         _dataContainer.SzenarioName == "Szenario04" ||
                         _dataContainer.SzenarioName == "Szenario05" ||
                         _dataContainer.SzenarioName == "Szenario06"
                    )
                {
                    expectedSzenarioTrialCount = 640;
                    expectedTargetTrialCount = 40;
                }
                else if (_dataContainer.SzenarioName == "Szenario07" ||
                         _dataContainer.SzenarioName == "Szenario08" ||
                         _dataContainer.SzenarioName == "Szenario09" ||
                         _dataContainer.SzenarioName == "Szenario10" ||
                         _dataContainer.SzenarioName == "Szenario31" ||
                         _dataContainer.SzenarioName == "Szenario32"
                    )
                {
                    expectedSzenarioTrialCount = 256;
                    expectedTargetTrialCount = 16;
                }
                else if (_dataContainer.SzenarioName == "Szenario11" ||
                         _dataContainer.SzenarioName == "Szenario12" ||
                         _dataContainer.SzenarioName == "Szenario13" ||
                         _dataContainer.SzenarioName == "Szenario14" ||
                         _dataContainer.SzenarioName == "Szenario15" ||
                         _dataContainer.SzenarioName == "Szenario16" ||
                         _dataContainer.SzenarioName == "Szenario17" ||
                         _dataContainer.SzenarioName == "Szenario18" ||
                         _dataContainer.SzenarioName == "Szenario19" ||
                         _dataContainer.SzenarioName == "Szenario20" ||
                         _dataContainer.SzenarioName == "Szenario50" ||
                         _dataContainer.SzenarioName == "Szenario51"
                    )
                {
                    expectedSzenarioTrialCount = 400;
                    expectedTargetTrialCount = 25;
                }
                else if (_dataContainer.SzenarioName == "Szenario43_N" ||
                         _dataContainer.SzenarioName == "Szenario43_R" ||
                         _dataContainer.SzenarioName == "Szenario44_N" ||
                         _dataContainer.SzenarioName == "Szenario44_R"
                    )
                {
                    expectedSzenarioTrialCount = 480;
                    expectedTargetTrialCount = 30;
                }
                else if (_dataContainer.SzenarioName == "Szenario45_N" ||
                         _dataContainer.SzenarioName == "Szenario45_R"
                    )
                {
                    expectedSzenarioTrialCount = 32;
                    expectedTargetTrialCount = 2;
                }

                //const string checkHeader = "Time, ForceActualX, ForceActualY, ForceActualZ, ForceNominalX, ForceNominalY, ForceNominalZ, ForceMomentX, ForceMomentY, ForceMomentZ, PositionCartesianX, PositionCartesianY, PositionCartesianZ, OldTarget, ActiveTarget, TargetNumber, TrialNumber, isCatchTrial, hasLeftTarget";  // Study 1
                //const string checkHeader = "Time, ForceActualX, ForceActualY, ForceActualZ, ForceNominalX, ForceNominalY, ForceNominalZ, ForceMomentX, ForceMomentY, ForceMomentZ, PositionCartesianX, PositionCartesianY, PositionCartesianZ, OldTarget, ActiveTarget, TargetNumber, TrialNumber, IsCatchTrial, PositionStatus"; // Study 2 & 3
                const string checkHeader =
                    "Time, ForceActualX, ForceActualY, ForceActualZ, ForceNominalX, ForceNominalY, ForceNominalZ, ForceMomentX, ForceMomentY, ForceMomentZ, PositionCartesianX, PositionCartesianY, PositionCartesianZ, OldTarget, ActiveTarget, TargetNumber, TrialNumber, IsCatchTrial, IsErrorClampTrial, PositionStatus";
                // Study 4


                if (checkHeader == measureFileReader.ReadLine())
                {
                    while (!measureFileReader.EndOfStream)
                    {
                        string readLine = measureFileReader.ReadLine();
                        if (readLine != null)
                        {
                            string[] measureFileLine = readLine.Split(new[] {", "}, StringSplitOptions.None);

                            if (measureFileLine.Count() == 20) // Study 4
                            {
                                if ((_dataContainer.MeasureDataRaw.Count > 0) &&
                                    (DateTime.Parse(_dataContainer.MeasureFileCreationDate + " " + measureFileLine[0])
                                        .Subtract(_dataContainer.MeasureDataRaw.Last().TimeStamp)
                                        .TotalMilliseconds > 500)
                                    )
                                {
                                    while ((Convert.ToInt32(measureFileLine[16]) ==
                                            _dataContainer.MeasureDataRaw.Last().SzenarioTrialNumber) &&
                                           (!measureFileReader.EndOfStream)
                                        )
                                    {
                                        readLine = measureFileReader.ReadLine();
                                        if (readLine != null)
                                        {
                                            measureFileLine = readLine
                                                .Split(new[] {", "},
                                                    StringSplitOptions.None);
                                        }
                                    }
                                }
                                else if ((_dataContainer.MeasureDataRaw.Count > 0) &&
                                         (Convert.ToInt32(measureFileLine[15]) !=
                                          _dataContainer.MeasureDataRaw.Last().TargetNumber) &&
                                         (Convert.ToInt32(measureFileLine[16]) ==
                                          _dataContainer.MeasureDataRaw.Last().SzenarioTrialNumber)
                                    )
                                {
                                    while (
                                        (Convert.ToInt32(measureFileLine[15]) !=
                                         _dataContainer.MeasureDataRaw.Last().TargetNumber) &&
                                        (Convert.ToInt32(measureFileLine[16]) ==
                                         _dataContainer.MeasureDataRaw.Last().SzenarioTrialNumber) &&
                                        (!measureFileReader.EndOfStream)
                                        )
                                    {
                                        readLine = measureFileReader.ReadLine();
                                        if (readLine != null)
                                        {
                                            measureFileLine = readLine
                                                .Split(new[] {", "},
                                                    StringSplitOptions.None);
                                        }
                                    }
                                }

                                if (Convert.ToInt32(measureFileLine[16]) <= expectedSzenarioTrialCount)
                                {
                                    _dataContainer.MeasureDataRaw.Add(new MeasureDataContainer(
                                        DateTime.Parse(
                                            _dataContainer.MeasureFileCreationDate +
                                            " " +
                                            measureFileLine[0]),
                                        //Forces in Newton
                                        Convert.ToDouble(measureFileLine[1])/1E3,
                                        Convert.ToDouble(measureFileLine[2])/1E3,
                                        Convert.ToDouble(measureFileLine[3])/1E3,
                                        Convert.ToDouble(measureFileLine[4])/1E3,
                                        Convert.ToDouble(measureFileLine[5])/1E3,
                                        Convert.ToDouble(measureFileLine[6])/1E3,
                                        Convert.ToDouble(measureFileLine[7])/1E3,
                                        Convert.ToDouble(measureFileLine[8])/1E3,
                                        Convert.ToDouble(measureFileLine[9])/1E3,
                                        //Positions in Meter
                                        Convert.ToDouble(measureFileLine[10])/1E6,
                                        Convert.ToDouble(measureFileLine[11])/1E6,
                                        Convert.ToDouble(measureFileLine[12])/1E6,
                                        Convert.ToInt32(measureFileLine[15]),
                                        -1,
                                        Convert.ToInt32(measureFileLine[16]),
                                        Convert.ToBoolean(measureFileLine[17]),
                                        Convert.ToBoolean(measureFileLine[18]),
                                        Convert.ToInt32(measureFileLine[19])
                                        ));
                                }
                            }
                            else
                            {
                                _myManipAnalysisGui.WriteToLogBox("Measure file header error: invalid column count");
                                retVal = false;
                            }
                        }
                    }

                    /*
                    //-------------------- 30.07.2013
                    if (_dataContainer.MeasureFileCreationDate == "24.07.2013" ||
                        _dataContainer.MeasureFileCreationDate == "25.07.2013" ||
                        _dataContainer.MeasureFileCreationDate == "26.07.2013" ||
                        _dataContainer.MeasureFileCreationDate == "27.07.2013" ||
                        _dataContainer.MeasureFileCreationDate == "28.07.2013" ||
                        _dataContainer.MeasureFileCreationDate == "29.07.2013" ||
                        _dataContainer.MeasureFileCreationDate == "30.07.2013"
                       )
                    {
                        int sztrnmbr = -1;
                        int addsztrnmbr = -1;

                        if (_dataContainer.SzenarioName.Contains("Szenario42"))
                        {
                            sztrnmbr = 67;
                            addsztrnmbr = 97;
                        }
                        else if (_dataContainer.SzenarioName.Contains("Szenario43") ||
                                    _dataContainer.SzenarioName.Contains("Szenario44")
                                )
                        {
                            sztrnmbr = 455;
                            addsztrnmbr = 481;
                        }
                        else if (_dataContainer.SzenarioName.Contains("Szenario45"))
                        {
                            sztrnmbr = 3;
                            addsztrnmbr = 33;
                        }

                        if (sztrnmbr != -1)
                        {
                            DateTime startTime = _dataContainer.MeasureDataRaw.Last().TimeStamp;
                            startTime.AddSeconds(1.0);

                            List<MeasureDataContainer> bufferList = _dataContainer.MeasureDataRaw.Where(t => t.SzenarioTrialNumber == sztrnmbr).ToList();

                            for (int blc = 0; blc < bufferList.Count; blc++)
                            {
                                MeasureDataContainer tmdc = new MeasureDataContainer(
                                    startTime.AddMilliseconds(blc * 5),
                                    bufferList[blc].ForceActualX,
                                    bufferList[blc].ForceActualY,
                                    bufferList[blc].ForceActualZ,
                                    bufferList[blc].ForceNominalX,
                                    bufferList[blc].ForceNominalY,
                                    bufferList[blc].ForceNominalZ,
                                    bufferList[blc].ForceMomentX,
                                    bufferList[blc].ForceMomentY,
                                    bufferList[blc].ForceMomentZ,
                                    bufferList[blc].PositionCartesianX,
                                    bufferList[blc].PositionCartesianY,
                                    bufferList[blc].PositionCartesianZ,
                                    bufferList[blc].TargetNumber,
                                    bufferList[blc].TargetTrialNumber,
                                    addsztrnmbr,
                                    bufferList[blc].IsCatchTrial,
                                    bufferList[blc].IsErrorclampTrial,
                                    bufferList[blc].PositionStatus);

                                tmdc.ContainsDuplicates = true;

                                _dataContainer.MeasureDataRaw.Add(tmdc);
                            }
                        }

                        for (int stn = 0; stn < _dataContainer.MeasureDataRaw.Count; stn++)
                        {
                            _dataContainer.MeasureDataRaw.ElementAt(stn).SzenarioTrialNumber--;
                        }
                    }
                    //--------------------
                    */
                    int maxTrialCount = _dataContainer.MeasureDataRaw.Max(t => t.SzenarioTrialNumber);
                    int realTrialCount =
                        _dataContainer.MeasureDataRaw.Select(t => t.SzenarioTrialNumber).Distinct().Count();


                    if ((maxTrialCount != expectedSzenarioTrialCount) || (realTrialCount != expectedSzenarioTrialCount))
                    {
                        _myManipAnalysisGui.WriteToLogBox("Trial count error: expected " + expectedSzenarioTrialCount +
                                                          " trials but " + realTrialCount + " were found.");
                        retVal = false;
                    }
                }
                else
                {
                    _myManipAnalysisGui.WriteToLogBox("Error in measure file header.");
                    retVal = false;
                }
                measureFileReader.Close();

                int[] targetArray = _dataContainer.MeasureDataRaw.Select(t => t.TargetNumber).Distinct().ToArray();

                for (int i = 0; i < targetArray.Length; i++)
                {
                    int[] szenarioTrialNumberArray =
                        _dataContainer.MeasureDataRaw.Where(t => t.TargetNumber == targetArray[i])
                            .Select(t => t.SzenarioTrialNumber)
                            .Distinct()
                            .OrderBy(t => t)
                            .ToArray();

                    if (expectedTargetTrialCount == szenarioTrialNumberArray.Length)
                    {
                        for (int j = 0; j < szenarioTrialNumberArray.Length; j++)
                        {
                            List<MeasureDataContainer> tempList =
                                _dataContainer.MeasureDataRaw.Where(
                                    t => t.SzenarioTrialNumber == szenarioTrialNumberArray[j])
                                    .OrderBy(t => t.TimeStamp)
                                    .ToList();

                            for (int k = 0; k < tempList.Count; k++)
                            {
                                tempList.ElementAt(k).TargetTrialNumber = j + 1;
                            }
                        }
                    }
                    else
                    {
                        _myManipAnalysisGui.WriteToLogBox("Target Trial number incorrect.");
                        retVal = false;
                    }
                }
            }
            else
            {
                retVal = false;
            }

            if (!retVal)
            {
                _dataContainer.MeasureDataRaw.Clear();
            }

            return retVal;
        }
    }
}