using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ManipAnalysis_v2.Container;
using ManipAnalysis_v2.MeasureFileParser;
using ManipAnalysis_v2.MongoDb;
using MongoDB.Driver;
using ManipAnalysis_v2.SzenarioParseDefinitions;

namespace ManipAnalysis_v2
{
    /// <summary>
    ///     This class provides all interactive functionality between the Gui and the programm.
    /// </summary>
    internal class ManipAnalysisFunctions

    {
        private readonly MongoDbWrapper _myDatabaseWrapper;

        private readonly ManipAnalysisGui _myManipAnalysisGui;

        private readonly MatlabWrapper _myMatlabWrapper;

        public ManipAnalysisFunctions(ManipAnalysisGui myManipAnalysisGui, MatlabWrapper myMatlabWrapper,
            MongoDbWrapper myDatabaseWrapper)
        {
            _myMatlabWrapper = myMatlabWrapper;
            _myDatabaseWrapper = myDatabaseWrapper;
            _myManipAnalysisGui = myManipAnalysisGui;
        }

        /// <summary>
        ///     Does a network reachability check if the given server is available over the network.
        /// </summary>
        /// <param name="server">The server that has to be checked</param>
        /// <param name="timeOutMilliSec">Timeout in milliseconds for the check</param>
        /// <returns></returns>
        private bool CheckDatabaseServerAvailability(string server, int timeOutMilliSec)
        {
            var serverAvailable = false;

            using (var tcp = new TcpClient())
            {
                var ar = tcp.BeginConnect(server, 27017, null, null);
                // For MongoDB
                var wh = ar.AsyncWaitHandle;
                try
                {
                    if (!ar.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(timeOutMilliSec), false))
                    {
                        throw new Exception();
                    }

                    tcp.EndConnect(ar);
                    serverAvailable = true;
                }
                finally
                {
                    wh.Close();
                }
            }

            return serverAvailable;
        }

        /// <summary>
        ///     Changes the Database-Server-Value in the Database-Wrapper.
        ///     Checks wether the server is available first.
        /// </summary>
        /// <param name="server">The new Database-Server</param>
        /// <returns></returns>
        public bool ConnectToDatabaseServer(string server)
        {
            var retVal = false;
            var timeOut = 5000;
            if (CheckDatabaseServerAvailability(server, timeOut))
            {
                _myManipAnalysisGui.WriteToLogBox("Connected to Database-Server at \"" + server + "\"");
                _myDatabaseWrapper.SetDatabaseServer(server);
                _myManipAnalysisGui.SetDatabaseNames(_myDatabaseWrapper.GetDatabases());
                retVal = true;
            }
            else
            {
                _myManipAnalysisGui.WriteToLogBox("Database-Server at \"" + server + "\" not reachable! (Timeout: " +
                                                  timeOut + "ms)");
            }

            return retVal;
        }

        /// <summary>
        ///     Sets a Database
        /// </summary>
        /// <param name="database">The new Database</param>
        public void SetDatabase(string database)
        {
            _myDatabaseWrapper.SetDatabase(database);
        }

        /// <summary>
        ///     Gets all studys from database
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetStudys()
        {
            return _myDatabaseWrapper.GetStudys();
        }

        /// <summary>
        ///     Gets all groups from database of a given study
        /// </summary>
        /// <param name="study"></param>
        /// <returns></returns>
        public IEnumerable<string> GetGroups(string study)
        {
            return _myDatabaseWrapper.GetGroups(study);
        }

        /// <summary>
        ///     Gets all szenarios from database of a given study and group
        /// </summary>
        /// <param name="study"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public IEnumerable<string> GetSzenarios(string study, string group)
        {
            return _myDatabaseWrapper.GetSzenarios(study, group);
        }

        /// <summary>
        ///     Gets all szenarios from database of a given study, group and subject
        /// </summary>
        /// <param name="study"></param>
        /// <param name="group"></param>
        /// <param name="subject"></param>
        /// <returns></returns>
        public IEnumerable<string> GetSzenarios(string study, string group, SubjectContainer subject)
        {
            return _myDatabaseWrapper.GetSzenarios(study, group, subject);
        }

        /// <summary>
        ///     Gets all subjects from database of a given study, group and szenario
        /// </summary>
        /// <param name="study"></param>
        /// <param name="group"></param>
        /// <param name="szenario"></param>
        /// <returns></returns>
        public IEnumerable<SubjectContainer> GetSubjects(string study, string group, string szenario)
        {
            return _myDatabaseWrapper.GetSubjects(study, group, szenario);
        }

        /// <summary>
        ///     Gets all subjects from database of a given study and group
        /// </summary>
        /// <param name="study"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public IEnumerable<SubjectContainer> GetSubjects(string study, string group)
        {
            return _myDatabaseWrapper.GetSubjects(study, group);
        }

        /// <summary>
        ///     Gets all turns from database of a given study, group, szenario and subject
        /// </summary>
        /// <param name="study"></param>
        /// <param name="group"></param>
        /// <param name="szenario"></param>
        /// <param name="subject"></param>
        /// <returns></returns>
        public IEnumerable<string> GetTurns(string study, string group, string szenario, SubjectContainer subject)
        {
            var turnList = new List<string>();
            var turns = _myDatabaseWrapper.GetTurns(study, group, szenario, subject).Count();
            for (var turn = 1; turn <= turns; turn++)
            {
                turnList.Add("Turn " + turn);
            }
            return turnList;
        }

        /// <summary>
        ///     Gets all turns from database of a given study, szenario and subject and group-array
        /// </summary>
        /// <param name="study"></param>
        /// <param name="group"></param>
        /// <param name="szenario"></param>
        /// <param name="subject"></param>
        /// <returns></returns>
        public IEnumerable<string> GetTurns(string study, string[] group, string szenario, SubjectContainer subject)
        {
            var turnList = new List<string>();
            var turns = _myDatabaseWrapper.GetTurns(study, group, szenario, subject).Count();
            for (var turn = 1; turn <= turns; turn++)
            {
                turnList.Add("Turn " + turn);
            }
            return turnList;
        }

        /// <summary>
        ///     Gets all targets from database of a given study and szenario
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetTargets(string studyName, string groupName, string szenarioName,
            SubjectContainer subject)
        {
            return
                _myDatabaseWrapper.GetTargets(studyName, groupName, szenarioName, subject)
                    .Select(t => "Target " + t.ToString("00"));
        }

        /// <summary>
        ///     Gets all trials from database of a given study and szenario
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetTrials(string studyName, string groupName, string szenarioName,
            SubjectContainer subject)
        {
            return
                _myDatabaseWrapper.GetTargetTrials(studyName, groupName, szenarioName, subject)
                    .Select(t => "Trial " + t.ToString("000"));
        }

        /// <summary>
        ///     Checks wether a raw-measure-file-hash already exists in database
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public bool CheckIfMeasureFileHashAlreadyExists(string hash)
        {
            return _myDatabaseWrapper.CheckIfMeasureFileHashExists(hash);
        }

        /// <summary>
        ///     Checks wether the given file is a valid measure-data-file
        /// </summary>
        /// <param name="filePath">The FilePath to check</param>
        /// <returns>[True] when file is valid, [False] if not.</returns>
        public bool IsValidMeasureDataFile(string filePath)
        {
            if (KinarmMeasureFileParser.IsValidFile(_myManipAnalysisGui, this, filePath))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Gets the DateTime of a turn from database of a given study, group, szenario, subject and turn
        /// </summary>
        /// <param name="study"></param>
        /// <param name="group"></param>
        /// <param name="szenario"></param>
        /// <param name="subject"></param>
        /// <param name="turn"></param>
        /// <returns></returns>
        private DateTime GetTurnDateTime(string study, string group, string szenario, SubjectContainer subject, int turn)
        {
            return _myDatabaseWrapper.GetTurns(study, group, szenario, subject).ElementAt(turn - 1);
        }

        /// <summary>
        ///     Deletes all data from a given measure-file-id from the database
        /// </summary>
        /// <param name="measureFileId"></param>
        public void DeleteMeasureFile(int measureFileId)
        {
        }

        public void PlotSzenarioMeanTimes(string study, string group, string szenario, SubjectContainer subject,
            int turn)
        {
            TaskManager.PushBack(Task.Factory.StartNew(() =>
            {
                var turnDateTime = GetTurnDateTime(study, group, szenario, subject, turn);
                var szenarioMeanTimes =
                    _myDatabaseWrapper.GetSzenarioMeanTime(study, group, szenario, subject, turnDateTime).ToArray();

                _myMatlabWrapper.CreateMeanTimeFigure();

                for (var szenarioMeanTimeCounter = 0;
                    szenarioMeanTimeCounter < szenarioMeanTimes.Length & !TaskManager.Cancel;
                    szenarioMeanTimeCounter++)
                {
                    _myMatlabWrapper.SetWorkspaceData("target", szenarioMeanTimes[szenarioMeanTimeCounter].Target.Number);
                    _myMatlabWrapper.SetWorkspaceData("meanTime",
                        szenarioMeanTimes[szenarioMeanTimeCounter].MeanTime.TotalSeconds);
                    _myMatlabWrapper.SetWorkspaceData("meanTimeStd",
                        szenarioMeanTimes[szenarioMeanTimeCounter].MeanTimeStd.TotalSeconds);
                    _myMatlabWrapper.PlotMeanTimeErrorBar("target", "meanTime", "meanTimeStd");
                }

                // Add one more Target (mean) for overall mean value
                _myMatlabWrapper.SetWorkspaceData("target", 17);
                //szenarioMeanTimes.Select(t => t.Target.Number).Max() + 1);
                _myMatlabWrapper.SetWorkspaceData("meanTime",
                    szenarioMeanTimes.Select(t => t.MeanTime.TotalSeconds).Average());
                _myMatlabWrapper.SetWorkspaceData("meanTimeStd",
                    szenarioMeanTimes.Select(t => t.MeanTimeStd.TotalSeconds).Average());
                _myMatlabWrapper.PlotMeanTimeErrorBar("target", "meanTime", "meanTimeStd");

                _myMatlabWrapper.ClearWorkspace();
                TaskManager.Remove(Task.CurrentId);
            }));
        }

        public void ExportSzenarioMeanTimes(string study, string group, string szenario, SubjectContainer subject,
            int turn, string fileName)
        {
            /*
            TaskManager.PushBack(Task.Factory.StartNew(() =>
            {
                DateTime turnDateTime = _myDatabaseWrapper.GetTurnDateTime(study, group, szenario, subject, turn);

                DataSet meanTimeDataSet = _myDatabaseWrapper.GetMeanTimeDataSet(study, group, szenario, subject, turnDateTime);

                var cache = new List<string>
                {
                    "Study;Group;Szenario;Subject;Turn;Target;SzenarioMeanTime;SzenarioMeanTimeStd"
                };

                cache.AddRange(from DataRow row in meanTimeDataSet.Tables[0].Rows
                    select
                        study + ";" + @group + ";" + szenario + ";" + subject + ";" + turn + ";" +
                        Convert.ToInt32(row["target_number"]) + ";" +
                        DoubleConverter.ToExactString(
                            TimeSpan.Parse(Convert.ToString(row["szenario_mean_time"])).TotalMilliseconds) +
                        ";" +
                        DoubleConverter.ToExactString(
                            TimeSpan.Parse(Convert.ToString(row["szenario_mean_time_std"])).TotalMilliseconds));

                var dataFileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                var dataFileWriter = new StreamWriter(dataFileStream);

                for (int i = 0; i < cache.Count(); i++)
                {
                    dataFileWriter.WriteLine(cache[i]);
                }

                dataFileWriter.Close();
                TaskManager.Remove(Task.CurrentId);
            }));
             * */
        }

        public void ToggleMatlabCommandWindow()
        {
            _myMatlabWrapper.ToggleCommandWindow();
        }

        public void ShowMatlabWorkspace()
        {
            _myMatlabWrapper.ShowWorkspaceWindow();
        }

        public void CompactDatabase()
        {
            TaskManager.PushBack(Task.Factory.StartNew(delegate
            {
                while (TaskManager.GetIndex(Task.CurrentId) != 0 & !TaskManager.Cancel)
                {
                    Thread.Sleep(100);
                }

                _myManipAnalysisGui.WriteToLogBox("Compacting database...");

                try
                {
                    _myDatabaseWrapper.CompactDatabase();
                }
                catch (Exception
                    ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                }

                _myManipAnalysisGui.WriteToLogBox("Ready.");

                TaskManager.Remove(Task.CurrentId);
            }));
        }

        public void DropDatabase()
        {
            TaskManager.PushBack(Task.Factory.StartNew(delegate
            {
                while (TaskManager.GetIndex(Task.CurrentId) != 0 & !TaskManager.Cancel)
                {
                    Thread.Sleep(100);
                }

                _myManipAnalysisGui.WriteToLogBox("Dropping database...");

                try
                {
                    _myDatabaseWrapper.DropDatabase();
                }
                catch (Exception
                    ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                }

                _myManipAnalysisGui.WriteToLogBox("Ready.");

                TaskManager.Remove(Task.CurrentId);
            }));
        }

        public void EnsureIndexes()
        {
            TaskManager.PushBack(Task.Factory.StartNew(delegate
            {
                while (TaskManager.GetIndex(Task.CurrentId) != 0 & !TaskManager.Cancel)
                {
                    Thread.Sleep(100);
                }

                _myManipAnalysisGui.WriteToLogBox("Ensuring indexes...");

                try
                {
                    _myDatabaseWrapper.EnsureIndexes();
                }
                catch (Exception
                    ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                }

                _myManipAnalysisGui.WriteToLogBox("Ready.");

                TaskManager.Remove(Task.CurrentId);
            }));
        }

        public void RebuildIndexes()
        {
            TaskManager.PushBack(Task.Factory.StartNew(delegate
            {
                while (TaskManager.GetIndex(Task.CurrentId) != 0 & !TaskManager.Cancel)
                {
                    Thread.Sleep(100);
                }

                _myManipAnalysisGui.WriteToLogBox("Rebuilding indexes...");

                try
                {
                    _myDatabaseWrapper.RebuildIndexes();
                }
                catch (Exception
                    ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                }

                _myManipAnalysisGui.WriteToLogBox("Ready.");

                TaskManager.Remove(Task.CurrentId);
            }));
        }

        public void DropIndexes()
        {
            TaskManager.PushBack(Task.Factory.StartNew(delegate
            {
                while (TaskManager.GetIndex(Task.CurrentId) != 0 & !TaskManager.Cancel)
                {
                    Thread.Sleep(100);
                }

                _myManipAnalysisGui.WriteToLogBox("Dropping indexes...");

                try
                {
                    _myDatabaseWrapper.DropAllIndexes();
                }
                catch (Exception
                    ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                }

                _myManipAnalysisGui.WriteToLogBox("Ready.");

                TaskManager.Remove(Task.CurrentId);
            }));
        }

        public void PlotTrajectoryBaseline(string study, string group, SubjectContainer subject, int[] targets,
            IEnumerable<Trial.TrialTypeEnum> trialTypes, IEnumerable<Trial.ForceFieldTypeEnum> forceFields,
            IEnumerable<Trial.HandednessEnum> handedness)
        {
            TaskManager.PushBack(Task.Factory.StartNew(() =>
            {
                _myMatlabWrapper.CreateTrajectoryFigure("Trajectory baseline plot");

                if (study == "Study 7")
                {
                    _myMatlabWrapper.DrawTargetsCenterOut3(0.003, 0.1, 0, 0);
                }
                else if (study == "Study 08")
                {
                    _myMatlabWrapper.DrawTargetsCenterOut3(0.003, 0.1, 0, 0);
                }
                else if (study == "Study 09")
                {
                    _myMatlabWrapper.DrawTargetsCenterOut8(0.003, 0.1, 0, 0);
                }
                else if (study == "Study 10")
                {
                    _myMatlabWrapper.DrawTargetsCenterOut8(0.003, 0.1, 0, 0);
                }

                var baselineFields = Builders<Baseline>.Projection.Include(t => t.ZippedPosition);
                var baselines =
                    _myDatabaseWrapper.GetBaseline(study, group, subject, targets, trialTypes, forceFields, handedness,
                        baselineFields).ToArray();

                for (var baselineCounter = 0;
                    baselineCounter < baselines.Length & !TaskManager.Cancel;
                    baselineCounter++)
                {
                    baselines[baselineCounter].Position =
                        Gzip<List<PositionContainer>>.DeCompress(baselines[baselineCounter].ZippedPosition)
                            .OrderBy(t => t.TimeStamp)
                            .ToList();
                    _myMatlabWrapper.SetWorkspaceData("X",
                        baselines[baselineCounter].Position.Select(u => u.X).ToArray());
                    _myMatlabWrapper.SetWorkspaceData("Y",
                        baselines[baselineCounter].Position.Select(u => u.Y).ToArray());
                    _myMatlabWrapper.Plot("X", "Y", "black", 2);
                }

                _myMatlabWrapper.ClearWorkspace();
                TaskManager.Remove(Task.CurrentId);
            }));
        }

        public IEnumerable<string> GetTrialsOfSzenario(string studyName, string groupName, string szenarioName,
            SubjectContainer subject, IEnumerable<Trial.TrialTypeEnum> trialTypes,
            IEnumerable<Trial.ForceFieldTypeEnum> forceFields, IEnumerable<Trial.HandednessEnum> handedness)
        {
            return
                _myDatabaseWrapper.GetSzenarioTrials(studyName, groupName, szenarioName, subject, trialTypes,
                    forceFields, handedness).Select(t => "Trial " + t.ToString("000"));
        }

        public void PlotExportDescriptiveStatistic1(IEnumerable<StatisticPlotContainer> selectedTrials,
            string statisticType, string fitEquation, int pdTime, bool plotFit, bool plotErrorbars, string fileName)
        {
            TaskManager.PushBack(Task.Factory.StartNew(() =>
            {
                try
                {
                    _myManipAnalysisGui.WriteProgressInfo("Getting data...");
                    var selectedTrialsList = selectedTrials.ToList();
                    double sumOfAllTrials = selectedTrialsList.Sum(t => t.Trials.Count);
                    double processedTrialsCount = 0;

                    var fields = Builders<Trial>.Projection.Include(t => t.ZippedStatistics);
                    fields = fields.Include(t => t.TrialNumberInSzenario);
                    // Neccessary for sorting!

                    if (selectedTrialsList.Any())
                    {
                        var trialList = selectedTrialsList.ElementAt(0).Trials;

                        if (selectedTrialsList.Any(temp => !trialList.SequenceEqual(temp.Trials)))
                        {
                            _myManipAnalysisGui.WriteToLogBox("Trial selections are not equal!");
                        }
                        else
                        {
                            var statisticData = new double[trialList.Count, selectedTrialsList.Count];
                            var meanCount = 0;
                            for (; meanCount < selectedTrialsList.Count & !TaskManager.Cancel; meanCount++)
                            {
                                var tempStatisticPlotContainer = selectedTrialsList.ElementAt(meanCount);

                                var turnDateTime = GetTurnDateTime(tempStatisticPlotContainer.Study,
                                    tempStatisticPlotContainer.Group, tempStatisticPlotContainer.Szenario,
                                    tempStatisticPlotContainer.Subject,
                                    Convert.ToInt32(tempStatisticPlotContainer.Turn.Substring("Turn".Length)));

                                var trialsArray =
                                    _myDatabaseWrapper.GetTrials(tempStatisticPlotContainer.Study,
                                        tempStatisticPlotContainer.Group, tempStatisticPlotContainer.Szenario,
                                        tempStatisticPlotContainer.Subject, turnDateTime, trialList, fields).ToArray();

                                for (var trialsArrayCounter = 0;
                                    trialsArrayCounter < trialList.Count & !TaskManager.Cancel;
                                    trialsArrayCounter++)
                                {
                                    _myManipAnalysisGui.SetProgressBarValue(100.0 / sumOfAllTrials * processedTrialsCount++);

                                    trialsArray[trialsArrayCounter].Statistics =
                                        Gzip<StatisticContainer>.DeCompress(
                                            trialsArray[trialsArrayCounter].ZippedStatistics);


                                    var pdTimeTick =
                                        trialsArray[trialsArrayCounter].Statistics.SignedPerpendicularDisplacement.Min(
                                            t => t.TimeStamp).Ticks + TimeSpan.FromMilliseconds(pdTime).Ticks;

                                    var msIndex =
                                        trialsArray[trialsArrayCounter].Statistics.SignedPerpendicularDisplacement
                                            .Select(t => t.TimeStamp)
                                            .OrderBy(t => Math.Abs(t.Ticks - pdTimeTick))
                                            .ElementAt(0);

                                    if (pdTimeTick >
                                        trialsArray[trialsArrayCounter].Statistics.SignedPerpendicularDisplacement.Max(
                                            t => t.TimeStamp).Ticks)
                                    {
                                        _myManipAnalysisGui.WriteToLogBox(
                                            "Warning! Selected PD-Time is larger then movement time! [" +
                                            tempStatisticPlotContainer.Study + " - " + tempStatisticPlotContainer.Group +
                                            " - " + tempStatisticPlotContainer.Szenario + " - " +
                                            tempStatisticPlotContainer.Subject + " - " + tempStatisticPlotContainer.Turn +
                                            " - Trial " + trialsArray[trialsArrayCounter].TrialNumberInSzenario + "]");
                                    }

                                    switch (statisticType)
                                    {
                                        case "MidMovementForce - PD Raw":
                                            statisticData[trialsArrayCounter, meanCount] =
                                                trialsArray[trialsArrayCounter].Statistics
                                                    .PerpendicularMidMovementForceRaw;
                                            break;

                                        case "PD - Abs":
                                            statisticData[trialsArrayCounter, meanCount] =
                                                trialsArray[trialsArrayCounter].Statistics
                                                    .AbsolutePerpendicularDisplacement.Single(
                                                        t => t.TimeStamp == msIndex).PerpendicularDisplacement;
                                            break;

                                        case "PDmax - Abs":
                                            statisticData[trialsArrayCounter, meanCount] =
                                                trialsArray[trialsArrayCounter].Statistics
                                                    .AbsoluteMaximalPerpendicularDisplacement;
                                            break;

                                        case "PDVmax - Abs":
                                            statisticData[trialsArrayCounter, meanCount] =
                                                trialsArray[trialsArrayCounter].Statistics
                                                    .AbsoluteMaximalPerpendicularDisplacementVmax;
                                            break;

                                        case "PD - Sign":
                                            statisticData[trialsArrayCounter, meanCount] =
                                                trialsArray[trialsArrayCounter].Statistics
                                                    .SignedPerpendicularDisplacement.Single(t => t.TimeStamp == msIndex)
                                                    .PerpendicularDisplacement;
                                            break;

                                        case "PDmax - Sign":
                                            statisticData[trialsArrayCounter, meanCount] =
                                                trialsArray[trialsArrayCounter].Statistics
                                                    .SignedMaximalPerpendicularDisplacement;
                                            break;

                                        case "PDVmax - Sign":
                                            statisticData[trialsArrayCounter, meanCount] =
                                                trialsArray[trialsArrayCounter].Statistics
                                                    .SignedMaximalPerpendicularDisplacementVmax;
                                            break;

                                        case "Trajectory length abs":
                                            statisticData[trialsArrayCounter, meanCount] =
                                                trialsArray[trialsArrayCounter].Statistics.AbsoluteTrajectoryLength;
                                            break;
                                            
                                        case "Enclosed area":
                                            statisticData[trialsArrayCounter, meanCount] =
                                                trialsArray[trialsArrayCounter].Statistics.EnclosedArea;
                                            break;

                                        case "ForcefieldCompenstionFactor":
                                            statisticData[trialsArrayCounter, meanCount] =
                                                trialsArray[trialsArrayCounter].Statistics.ForcefieldCompenstionFactor;
                                            break;

                                            //TODO: Raus?
                                        case "ForcefieldCompenstionFactor fisher-z":
                                            _myMatlabWrapper.SetWorkspaceData("ffcf",
                                                trialsArray[trialsArrayCounter].Statistics.ForcefieldCompenstionFactor);
                                            _myMatlabWrapper.Execute("fisherZ = fisherZTransform(ffcf);");
                                            statisticData[trialsArrayCounter, meanCount] =
                                                _myMatlabWrapper.GetWorkspaceData("fisherZ");
                                            _myMatlabWrapper.ClearWorkspace();
                                            break;

                                            //TODO: Raus?
                                        case "ForcefieldCompenstionFactor fisher-z to r-values":
                                            _myMatlabWrapper.SetWorkspaceData("ffcf",
                                                trialsArray[trialsArrayCounter].Statistics.ForcefieldCompenstionFactor);
                                            _myMatlabWrapper.Execute("fisherZ = fisherZTransform(ffcf);");
                                            statisticData[trialsArrayCounter, meanCount] =
                                                _myMatlabWrapper.GetWorkspaceData("fisherZ");
                                            _myMatlabWrapper.ClearWorkspace();
                                            break;

                                        case "ForcefieldCompenstionFactor Raw":
                                            statisticData[trialsArrayCounter, meanCount] =
                                                trialsArray[trialsArrayCounter].Statistics
                                                    .ForcefieldCompenstionFactorRaw;
                                            break;

                                            //TODO: Raus?
                                        case "ForcefieldCompenstionFactor Raw fisher-z":
                                            _myMatlabWrapper.SetWorkspaceData("ffcfraw",
                                                trialsArray[trialsArrayCounter].Statistics
                                                    .ForcefieldCompenstionFactorRaw);
                                            _myMatlabWrapper.Execute("fisherZ = fisherZTransform(ffcfraw);");
                                            statisticData[trialsArrayCounter, meanCount] =
                                                _myMatlabWrapper.GetWorkspaceData("fisherZ");
                                            _myMatlabWrapper.ClearWorkspace();
                                            break;

                                            //TODO: Raus?
                                        case "ForcefieldCompenstionFactor Raw fisher-z to r-values":
                                            _myMatlabWrapper.SetWorkspaceData("ffcfraw",
                                                trialsArray[trialsArrayCounter].Statistics
                                                    .ForcefieldCompenstionFactorRaw);
                                            _myMatlabWrapper.Execute("fisherZ = fisherZTransform(ffcfraw);");
                                            statisticData[trialsArrayCounter, meanCount] =
                                                _myMatlabWrapper.GetWorkspaceData("fisherZ");
                                            _myMatlabWrapper.ClearWorkspace();
                                            break;
                                    }
                                }
                            }

                            _myMatlabWrapper.SetWorkspaceData("statisticData", statisticData);
                            if (meanCount > 1)
                            {
                                //TODO: Cleanup the statisticTypes that were removed!
                                
                                if (statisticType == "ForcefieldCompenstionFactor fisher-z to r-values")
                                {
                                    _myMatlabWrapper.Execute(
                                        "statisticDataPlot = fisherZtoRTransform(mean(transpose(statisticData)));");
                                    _myMatlabWrapper.Execute(
                                        "statisticDataStd = fisherZtoRTransform(std(transpose(statisticData)));");
                                }
                                else if (statisticType == "ForcefieldCompenstionFactor Raw fisher-z to r-values")
                                {
                                    _myMatlabWrapper.Execute(
                                        "statisticDataPlot = fisherZtoRTransform(mean(transpose(statisticData)));");
                                    _myMatlabWrapper.Execute(
                                        "statisticDataStd = fisherZtoRTransform(std(transpose(statisticData)));");
                                }
                                else
                                {
                                    _myMatlabWrapper.Execute("statisticDataPlot = mean(transpose(statisticData));");
                                    _myMatlabWrapper.Execute("statisticDataStd = std(transpose(statisticData));");
                                }
                            }
                            else
                            {
                                if (statisticType == "ForcefieldCompenstionFactor fisher-z to r-values")
                                {
                                    _myMatlabWrapper.Execute("statisticDataPlot = fisherZtoRTransform(statisticData);");
                                }
                                else if (statisticType == "ForcefieldCompenstionFactor Raw fisher-z to r-values")
                                {
                                    _myMatlabWrapper.Execute("statisticDataPlot = fisherZtoRTransform(statisticData);");
                                }
                                else
                                {
                                    _myMatlabWrapper.Execute("statisticDataPlot = statisticData;");
                                }
                            }

                            if (fileName == null) // Plot or export?
                            {
                                switch (statisticType)
                                {
                                    case "MidMovementForce - PD Raw":
                                        _myMatlabWrapper.CreateStatisticFigure("MidMovementForce PD Raw plot",
                                            "statisticDataPlot",
                                            "fit(transpose([1:1:length(statisticDataPlot)]),transpose(statisticDataPlot),'" +
                                            fitEquation + "')", "statisticDataStd", "[Trial]", "Newton [N]", 1,
                                            statisticData.Length / meanCount, -3.0, 3.0, plotFit, plotErrorbars);
                                        break;

                                    case "PD - Abs":
                                        _myMatlabWrapper.CreateStatisticFigure("PD" + pdTime + " abs plot",
                                            "statisticDataPlot",
                                            "fit(transpose([1:1:length(statisticDataPlot)]),transpose(statisticDataPlot),'" +
                                            fitEquation + "')", "statisticDataStd", "[Trial]", "PD" + pdTime + " [m]", 1,
                                            statisticData.Length / meanCount, 0, 0.05, plotFit, plotErrorbars);
                                        break;

                                    case "PDmax - Abs":
                                        _myMatlabWrapper.CreateStatisticFigure("MaxPD abs plot", "statisticDataPlot",
                                            "fit(transpose([1:1:length(statisticDataPlot)]),transpose(statisticDataPlot),'" +
                                            fitEquation + "')", "statisticDataStd", "[Trial]", "MaxPD [m]", 1,
                                            statisticData.Length / meanCount, 0, 0.05, plotFit, plotErrorbars);
                                        break;

                                    case "PDVmax - Abs":
                                        _myMatlabWrapper.CreateStatisticFigure("VmaxPD abs plot", "statisticDataPlot",
                                            "fit(transpose([1:1:length(statisticDataPlot)]),transpose(statisticDataPlot),'" +
                                            fitEquation + "')", "statisticDataStd", "[Trial]", "MaxPD [m]", 1,
                                            statisticData.Length / meanCount, 0, 0.05, plotFit, plotErrorbars);
                                        break;

                                    case "PD - Sign":
                                        _myMatlabWrapper.CreateStatisticFigure("PD" + pdTime + " sign plot",
                                            "statisticDataPlot",
                                            "fit(transpose([1:1:length(statisticDataPlot)]),transpose(statisticDataPlot),'" +
                                            fitEquation + "')", "statisticDataStd", "[Trial]", "PD" + pdTime + " [m]", 1,
                                            statisticData.Length / meanCount, -0.05, 0.05, plotFit, plotErrorbars);
                                        break;

                                    case "PDmax - Sign":
                                        _myMatlabWrapper.CreateStatisticFigure("MaxPD sign plot", "statisticDataPlot",
                                            "fit(transpose([1:1:length(statisticDataPlot)]),transpose(statisticDataPlot),'" +
                                            fitEquation + "')", "statisticDataStd", "[Trial]", "MaxPD [m]", 1,
                                            statisticData.Length / meanCount, -0.05, 0.05, plotFit, plotErrorbars);
                                        break;

                                    case "PDVmax - Sign":
                                        _myMatlabWrapper.CreateStatisticFigure("VmaxPD sign plot", "statisticDataPlot",
                                            "fit(transpose([1:1:length(statisticDataPlot)]),transpose(statisticDataPlot),'" +
                                            fitEquation + "')", "statisticDataStd", "[Trial]", "MaxPD [m]", 1,
                                            statisticData.Length / meanCount, -0.05, 0.05, plotFit, plotErrorbars);
                                        break;

                                    case "Trajectory length abs":
                                        _myMatlabWrapper.CreateStatisticFigure("Trajectory Length plot",
                                            "statisticDataPlot",
                                            "fit(transpose([1:1:length(statisticDataPlot)]),transpose(statisticDataPlot),'" +
                                            fitEquation + "')", "statisticDataStd", "[Trial]", "Trajectory Length [m]",
                                            1, statisticData.Length / meanCount, 0.07, 0.2, plotFit, plotErrorbars);
                                        break;

                                    case "Enclosed area":
                                        _myMatlabWrapper.CreateStatisticFigure("Enclosed area plot", "statisticDataPlot",
                                            "fit(transpose([1:1:length(statisticDataPlot)]),transpose(statisticDataPlot),'" +
                                            fitEquation + "')", "statisticDataStd", "[Trial]", "Enclosed Area [m²]", 1,
                                            statisticData.Length / meanCount, 0, 0.002, plotFit, plotErrorbars);
                                        break;

                                    case "ForcefieldCompenstionFactor":
                                        _myMatlabWrapper.CreateStatisticFigure("Forcefield Compenstion Factor plot",
                                            "statisticDataPlot",
                                            "fit(transpose([1:1:length(statisticDataPlot)]),transpose(statisticDataPlot),'" +
                                            fitEquation + "')", "statisticDataStd", "[Trial]",
                                            "Forcefield Compenstion Factor", 1, statisticData.Length / meanCount, -1.0,
                                            1.0, plotFit, plotErrorbars);
                                        break;

                                    //TODO: Raus?
                                    case "ForcefieldCompenstionFactor fisher-z":
                                        _myMatlabWrapper.CreateStatisticFigure(
                                            "Forcefield Compenstion Factor fisher-z plot", "statisticDataPlot",
                                            "fit(transpose([1:1:length(statisticDataPlot)]),transpose(statisticDataPlot),'" +
                                            fitEquation + "')", "statisticDataStd", "[Trial]",
                                            "Forcefield Compenstion Factor", 1, statisticData.Length / meanCount, -1.0,
                                            1.0, plotFit, plotErrorbars);
                                        break;

                                    case "ForcefieldCompenstionFactor fisher-z to r-values":
                                        _myMatlabWrapper.CreateStatisticFigure(
                                            "Forcefield Compenstion Factor fisher-z to r-values plot",
                                            "statisticDataPlot",
                                            "fit(transpose([1:1:length(statisticDataPlot)]),transpose(statisticDataPlot),'" +
                                            fitEquation + "')", "statisticDataStd", "[Trial]",
                                            "Forcefield Compenstion Factor", 1, statisticData.Length / meanCount, -1.0,
                                            1.0, plotFit, plotErrorbars);
                                        break;

                                    case "ForcefieldCompenstionFactor Raw":
                                        _myMatlabWrapper.CreateStatisticFigure(
                                            "Forcefield Compenstion Factor Raw plot", "statisticDataPlot",
                                            "fit(transpose([1:1:length(statisticDataPlot)]),transpose(statisticDataPlot),'" +
                                            fitEquation + "')", "statisticDataStd", "[Trial]",
                                            "Forcefield Compenstion Factor", 1, statisticData.Length / meanCount, -1.0,
                                            1.0, plotFit, plotErrorbars);
                                        break;

                                    case "ForcefieldCompenstionFactor Raw fisher-z":
                                        _myMatlabWrapper.CreateStatisticFigure(
                                            "Forcefield Compenstion Factor Raw fisher-z plot", "statisticDataPlot",
                                            "fit(transpose([1:1:length(statisticDataPlot)]),transpose(statisticDataPlot),'" +
                                            fitEquation + "')", "statisticDataStd", "[Trial]",
                                            "Forcefield Compenstion Factor", 1, statisticData.Length / meanCount, -1.0,
                                            1.0, plotFit, plotErrorbars);
                                        break;

                                    case "ForcefieldCompenstionFactor Raw fisher-z to r-values":
                                        _myMatlabWrapper.CreateStatisticFigure(
                                            "Forcefield Compenstion Factor Raw fisher-z to r-values plot",
                                            "statisticDataPlot",
                                            "fit(transpose([1:1:length(statisticDataPlot)]),transpose(statisticDataPlot),'" +
                                            fitEquation + "')", "statisticDataStd", "[Trial]",
                                            "Forcefield Compenstion Factor", 1, statisticData.Length / meanCount, -1.0,
                                            1.0, plotFit, plotErrorbars);
                                        break;
                                }
                            }
                            else
                            {
                                var cache = new List<string>();
                                var meanDataFileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                                var meanDataFileWriter = new StreamWriter(meanDataFileStream);
                                var personNames = "";

                                double[,] dataMean = null;
                                double[,] dataStd = null;
                                if (meanCount > 1)
                                {
                                    dataMean = _myMatlabWrapper.GetWorkspaceData("statisticDataPlot");
                                    dataStd = _myMatlabWrapper.GetWorkspaceData("statisticDataStd");
                                }


                                for (var i = 0; i < selectedTrialsList.Count & !TaskManager.Cancel; i++)
                                {
                                    var tempStatisticPlotContainer = selectedTrialsList.ElementAt(i);

                                    if (i == 0)
                                    {
                                        personNames += tempStatisticPlotContainer.Subject;
                                    }
                                    else
                                    {
                                        personNames += ";" + tempStatisticPlotContainer.Subject;
                                    }
                                }

                                cache.Add("Trial;" + personNames + ";Mean;Std");

                                for (var trialListCounter = 0;
                                    trialListCounter < trialList.Count & !TaskManager.Cancel;
                                    trialListCounter++)
                                {
                                    var tempLine = trialList.ElementAt(trialListCounter) + ";";

                                    for (var meanCounter = 0; meanCounter < meanCount; meanCounter++)
                                    {
                                        tempLine +=
                                            DoubleConverter.ToExactString(statisticData[trialListCounter, meanCounter]) +
                                            ";";
                                    }

                                    if (meanCount > 1 && dataMean != null && dataStd != null)
                                    {
                                        tempLine += DoubleConverter.ToExactString(dataMean[0, trialListCounter]) + ";" +
                                                    DoubleConverter.ToExactString(dataStd[0, trialListCounter]);
                                    }
                                    else
                                    {
                                        tempLine += "0.0;0.0";
                                    }
                                    cache.Add(tempLine);
                                }

                                for (var i = 0; i < cache.Count & !TaskManager.Cancel; i++)
                                {
                                    meanDataFileWriter.WriteLine(cache[i]);
                                }

                                meanDataFileWriter.Close();
                            }
                            _myMatlabWrapper.ClearWorkspace();
                        }
                    }

                    _myManipAnalysisGui.WriteProgressInfo("Ready");
                    _myManipAnalysisGui.SetProgressBarValue(0);
                    TaskManager.Remove(Task.CurrentId);
                }
                catch (Exception
                    ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                }
            }));
        }

        public void ExportDescriptiveStatistic2Data(IEnumerable<StatisticPlotContainer> selectedTrials,
            string statisticType, string fileName, int pdTime)
        {
            TaskManager.PushBack(Task.Factory.StartNew(() =>
            {
                _myManipAnalysisGui.WriteProgressInfo("Getting data...");
                var selectedTrialsList = selectedTrials.ToList();

                var fields = Builders<Trial>.Projection.Include(t => t.ZippedStatistics);
                fields = fields.Include(t => t.TrialNumberInSzenario);
                // Neccessary for sorting!

                if (selectedTrialsList.Any())
                {
                    var trialList = selectedTrialsList.ElementAt(0).Trials;

                    if (selectedTrialsList.Any(temp => !trialList.SequenceEqual(temp.Trials)))
                    {
                        _myManipAnalysisGui.WriteToLogBox("Trial selections are not equal!");
                    }
                    else
                    {
                        var statisticData = new double[selectedTrialsList.Count, trialList.Count];

                        var subjectCounter = 0;
                        for (; subjectCounter < selectedTrialsList.Count & !TaskManager.Cancel; subjectCounter++)
                        {
                            var tempStatisticPlotContainer = selectedTrialsList.ElementAt(subjectCounter);

                            var turnDateTime = GetTurnDateTime(tempStatisticPlotContainer.Study,
                                tempStatisticPlotContainer.Group, tempStatisticPlotContainer.Szenario,
                                tempStatisticPlotContainer.Subject,
                                Convert.ToInt32(tempStatisticPlotContainer.Turn.Substring("Turn".Length)));

                            var trialsArray =
                                _myDatabaseWrapper.GetTrials(tempStatisticPlotContainer.Study,
                                    tempStatisticPlotContainer.Group, tempStatisticPlotContainer.Szenario,
                                    tempStatisticPlotContainer.Subject, turnDateTime, trialList, fields).ToArray();

                            for (var trialsArrayCounter = 0;
                                trialsArrayCounter < trialList.Count & !TaskManager.Cancel;
                                trialsArrayCounter++)
                            {
                                _myManipAnalysisGui.SetProgressBarValue(100.0 / selectedTrialsList.Count * subjectCounter);

                                trialsArray[trialsArrayCounter].Statistics =
                                    Gzip<StatisticContainer>.DeCompress(trialsArray[trialsArrayCounter].ZippedStatistics);


                                var pdTimeTick =
                                    trialsArray[trialsArrayCounter].Statistics.SignedPerpendicularDisplacement.Min(
                                        t => t.TimeStamp).Ticks + TimeSpan.FromMilliseconds(pdTime).Ticks;

                                var msIndex =
                                    trialsArray[trialsArrayCounter].Statistics.SignedPerpendicularDisplacement.Select(
                                        t => t.TimeStamp).OrderBy(t => Math.Abs(t.Ticks - pdTimeTick)).ElementAt(0);

                                if (pdTimeTick >
                                    trialsArray[trialsArrayCounter].Statistics.SignedPerpendicularDisplacement.Max(
                                        t => t.TimeStamp).Ticks)
                                {
                                    _myManipAnalysisGui.WriteToLogBox(
                                        "Warning! Selected PD-Time is larger then movement time! [" +
                                        tempStatisticPlotContainer.Study + " - " + tempStatisticPlotContainer.Group +
                                        " - " + tempStatisticPlotContainer.Szenario + " - " +
                                        tempStatisticPlotContainer.Subject + " - " + tempStatisticPlotContainer.Turn +
                                        " - Trial " + trialsArray[trialsArrayCounter].TrialNumberInSzenario + "]");
                                }

                                switch (statisticType)
                                {
                                    //TODO: Raus!
                                    case "Vector correlation fisher-z":
                                        _myMatlabWrapper.SetWorkspaceData("vcorr",
                                            trialsArray[trialsArrayCounter].Statistics.VelocityVectorCorrelation);
                                        _myMatlabWrapper.Execute("fisherZ = fisherZTransform(vcorr);");
                                        statisticData[subjectCounter, trialsArrayCounter] =
                                            _myMatlabWrapper.GetWorkspaceData("fisherZ");
                                        _myMatlabWrapper.ClearWorkspace();
                                        break;
                                    //TODO: Raus!
                                    case "Vector correlation fisher-z to r-values":
                                        _myMatlabWrapper.SetWorkspaceData("vcorr",
                                            trialsArray[trialsArrayCounter].Statistics.VelocityVectorCorrelation);
                                        _myMatlabWrapper.Execute("fisherZ = fisherZTransform(vcorr);");
                                        statisticData[subjectCounter, trialsArrayCounter] =
                                            _myMatlabWrapper.GetWorkspaceData("fisherZ");
                                        _myMatlabWrapper.ClearWorkspace();
                                        break;

                                    //TODO: Raus!
                                    case "MidMovementForce - PD":
                                        statisticData[subjectCounter, trialsArrayCounter] =
                                            trialsArray[trialsArrayCounter].Statistics.PerpendicularMidMovementForce;
                                        break;

                                    case "MidMovementForce - PD Raw":
                                        statisticData[subjectCounter, trialsArrayCounter] =
                                            trialsArray[trialsArrayCounter].Statistics.PerpendicularMidMovementForceRaw;
                                        break;

                                    case "PD - Abs":
                                        statisticData[subjectCounter, trialsArrayCounter] =
                                            trialsArray[trialsArrayCounter].Statistics.AbsolutePerpendicularDisplacement
                                                .Single(t => t.TimeStamp == msIndex).PerpendicularDisplacement;
                                        break;

                                    //TODO: Raus!
                                    case "PDmean - Abs":
                                        statisticData[subjectCounter, trialsArrayCounter] =
                                            trialsArray[trialsArrayCounter].Statistics
                                                .AbsoluteMeanPerpendicularDisplacement;
                                        break;

                                    case "PDmax - Abs":
                                        statisticData[subjectCounter, trialsArrayCounter] =
                                            trialsArray[trialsArrayCounter].Statistics
                                                .AbsoluteMaximalPerpendicularDisplacement;
                                        break;

                                    case "PDVmax - Abs":
                                        statisticData[subjectCounter, trialsArrayCounter] =
                                            trialsArray[trialsArrayCounter].Statistics
                                                .AbsoluteMaximalPerpendicularDisplacementVmax;
                                        break;

                                    case "PD - Sign":
                                        statisticData[subjectCounter, trialsArrayCounter] =
                                            trialsArray[trialsArrayCounter].Statistics.SignedPerpendicularDisplacement
                                                .Single(t => t.TimeStamp == msIndex).PerpendicularDisplacement;
                                        break;

                                    case "PDmax - Sign":
                                        statisticData[subjectCounter, trialsArrayCounter] =
                                            trialsArray[trialsArrayCounter].Statistics
                                                .SignedMaximalPerpendicularDisplacement;
                                        break;

                                    case "PDVmax - Sign":
                                        statisticData[subjectCounter, trialsArrayCounter] =
                                            trialsArray[trialsArrayCounter].Statistics
                                                .SignedMaximalPerpendicularDisplacementVmax;
                                        break;

                                    case "Trajectory length abs":
                                        statisticData[subjectCounter, trialsArrayCounter] =
                                            trialsArray[trialsArrayCounter].Statistics.AbsoluteTrajectoryLength;
                                        break;

                                    //TODO: Raus!
                                    case "Trajectory length ratio":
                                        statisticData[subjectCounter, trialsArrayCounter] =
                                            trialsArray[trialsArrayCounter].Statistics
                                                .AbsoluteBaselineTrajectoryLengthRatio;
                                        break;

                                    case "Enclosed area":
                                        statisticData[subjectCounter, trialsArrayCounter] =
                                            trialsArray[trialsArrayCounter].Statistics.EnclosedArea;
                                        break;

                                    case "ForcefieldCompenstionFactor":
                                        statisticData[subjectCounter, trialsArrayCounter] =
                                            trialsArray[trialsArrayCounter].Statistics.ForcefieldCompenstionFactor;
                                        break;

                                    case "ForcefieldCompenstionFactor fisher-z":
                                        _myMatlabWrapper.SetWorkspaceData("ffcf",
                                            trialsArray[trialsArrayCounter].Statistics.ForcefieldCompenstionFactor);
                                        _myMatlabWrapper.Execute("fisherZ = fisherZTransform(ffcf);");
                                        statisticData[subjectCounter, trialsArrayCounter] =
                                            _myMatlabWrapper.GetWorkspaceData("fisherZ");
                                        _myMatlabWrapper.ClearWorkspace();
                                        break;

                                    case "ForcefieldCompenstionFactor fisher-z to r-values":
                                        _myMatlabWrapper.SetWorkspaceData("ffcf",
                                            trialsArray[trialsArrayCounter].Statistics.ForcefieldCompenstionFactor);
                                        _myMatlabWrapper.Execute("fisherZ = fisherZTransform(ffcf);");
                                        statisticData[subjectCounter, trialsArrayCounter] =
                                            _myMatlabWrapper.GetWorkspaceData("fisherZ");
                                        _myMatlabWrapper.ClearWorkspace();
                                        break;

                                    case "ForcefieldCompenstionFactor Raw":
                                        statisticData[subjectCounter, trialsArrayCounter] =
                                            trialsArray[trialsArrayCounter].Statistics.ForcefieldCompenstionFactorRaw;
                                        break;

                                    case "ForcefieldCompenstionFactor Raw fisher-z":
                                        _myMatlabWrapper.SetWorkspaceData("ffcfraw",
                                            trialsArray[trialsArrayCounter].Statistics.ForcefieldCompenstionFactorRaw);
                                        _myMatlabWrapper.Execute("fisherZ = fisherZTransform(ffcfraw);");
                                        statisticData[subjectCounter, trialsArrayCounter] =
                                            _myMatlabWrapper.GetWorkspaceData("fisherZ");
                                        _myMatlabWrapper.ClearWorkspace();
                                        break;

                                    case "ForcefieldCompenstionFactor Raw fisher-z to r-values":
                                        _myMatlabWrapper.SetWorkspaceData("ffcfraw",
                                            trialsArray[trialsArrayCounter].Statistics.ForcefieldCompenstionFactorRaw);
                                        _myMatlabWrapper.Execute("fisherZ = fisherZTransform(ffcfraw);");
                                        statisticData[subjectCounter, trialsArrayCounter] =
                                            _myMatlabWrapper.GetWorkspaceData("fisherZ");
                                        _myMatlabWrapper.ClearWorkspace();
                                        break;

                                    //TODO: Raus!
                                    case "RMSE":
                                        statisticData[subjectCounter, trialsArrayCounter] =
                                            trialsArray[trialsArrayCounter].Statistics.RMSE;
                                        break;

                                    //TODO: Raus!
                                    case "PredictionAngle":
                                        statisticData[subjectCounter, trialsArrayCounter] =
                                            trialsArray[trialsArrayCounter].Statistics.PredictionAngle;
                                        break;

                                    //TODO: Raus!
                                    case "FeedbackAngle":
                                        statisticData[subjectCounter, trialsArrayCounter] =
                                            trialsArray[trialsArrayCounter].Statistics.FeedbackAngle;
                                        break;
                                }
                            }
                        }

                        _myMatlabWrapper.SetWorkspaceData("statisticData", statisticData);
                        if (trialList.Count > 1)
                        {
                            if (statisticType == "Vector correlation fisher-z to r-values")
                            {
                                _myMatlabWrapper.Execute(
                                    "statisticDataMean = transpose(fisherZtoRTransform(mean(transpose(statisticData))));");
                                _myMatlabWrapper.Execute(
                                    "statisticDataStd = transpose(fisherZtoRTransform(std(transpose(statisticData))));");
                            }
                            else if (statisticType == "ForcefieldCompenstionFactor fisher-z to r-values")
                            {
                                _myMatlabWrapper.Execute(
                                    "statisticDataMean = transpose(fisherZtoRTransform(mean(transpose(statisticData))));");
                                _myMatlabWrapper.Execute(
                                    "statisticDataStd = transpose(fisherZtoRTransform(std(transpose(statisticData))));");
                            }
                            else if (statisticType == "ForcefieldCompenstionFactor Raw fisher-z to r-values")
                            {
                                _myMatlabWrapper.Execute(
                                    "statisticDataMean = transpose(fisherZtoRTransform(mean(transpose(statisticData))));");
                                _myMatlabWrapper.Execute(
                                    "statisticDataStd = transpose(fisherZtoRTransform(std(transpose(statisticData))));");
                            }
                            else
                            {
                                _myMatlabWrapper.Execute(
                                    "statisticDataMean = transpose(mean(transpose(statisticData)));");
                                _myMatlabWrapper.Execute("statisticDataStd = transpose(std(transpose(statisticData)));");
                            }
                        }
                        else
                        {
                            if (statisticType == "Vector correlation fisher-z to r-values")
                            {
                                _myMatlabWrapper.Execute("statisticDataMean = fisherZtoRTransform(statisticData);");
                            }
                            else if (statisticType == "ForcefieldCompenstionFactor fisher-z to r-values")
                            {
                                _myMatlabWrapper.Execute("statisticDataMean = fisherZtoRTransform(statisticData);");
                            }
                            else if (statisticType == "ForcefieldCompenstionFactor Raw fisher-z to r-values")
                            {
                                _myMatlabWrapper.Execute("statisticDataMean = fisherZtoRTransform(statisticData);");
                            }
                            else
                            {
                                _myMatlabWrapper.Execute("statisticDataMean = statisticData;");
                            }
                        }

                        double[,] dataMean = null;
                        double[,] dataStd = null;
                        if (subjectCounter > 1)
                        {
                            dataMean = _myMatlabWrapper.GetWorkspaceData("statisticDataMean");
                            dataStd = _myMatlabWrapper.GetWorkspaceData("statisticDataStd");
                        }
                        else
                        {
                            dataMean = new double[1, 1];
                            dataMean[0, 0] = _myMatlabWrapper.GetWorkspaceData("statisticDataMean");
                        }

                        var cache = new List<string>();
                        var meanDataFileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                        var meanDataFileWriter = new StreamWriter(meanDataFileStream);
                        cache.Add("Study;Group;Szenario;Subject;Turn;Trials;Mean;Std");


                        for (var i = 0; i < selectedTrialsList.Count & !TaskManager.Cancel; i++)
                        {
                            var tempStatisticPlotContainer = selectedTrialsList.ElementAt(i);
                            var meanValue = DoubleConverter.ToExactString(dataMean[i, 0]);
                            var stdValue = dataStd == null ? "" : DoubleConverter.ToExactString(dataStd[i, 0]);

                            cache.Add(tempStatisticPlotContainer.Study + ";" + tempStatisticPlotContainer.Group + ";" +
                                      tempStatisticPlotContainer.Szenario + ";" + tempStatisticPlotContainer.Subject +
                                      ";" + tempStatisticPlotContainer.Turn + ";" +
                                      tempStatisticPlotContainer.GetTrialsString() + ";" + meanValue + ";" + stdValue);
                        }

                        for (var i = 0; i < cache.Count & !TaskManager.Cancel; i++)
                        {
                            meanDataFileWriter.WriteLine(cache[i]);
                        }

                        meanDataFileWriter.Close();
                    }
                }

                _myMatlabWrapper.ClearWorkspace();
                _myManipAnalysisGui.WriteProgressInfo("Ready");
                _myManipAnalysisGui.SetProgressBarValue(0);
                TaskManager.Remove(Task.CurrentId);
            }));
        }

        public void ChangeGroupId(int groupId, int newGroupId)
        {
            //_myDatabaseWrapper.ChangeGroupID(groupId, newGroupId);
        }

        public void ChangeGroupGroupName(int groupId, string newGroupName)
        {
            //_myDatabaseWrapper.ChangeGroupName(groupId, newGroupName);
        }

        public void ChangeSubjectId(int subjectId, int newSubjectId)
        {
            //_myDatabaseWrapper.ChangeSubjectID(subjectId, newSubjectId);
        }

        public void ChangeSubjectSubjectName(int subjectId, string newSubjectName)
        {
            //_myDatabaseWrapper.ChangeSubjectName(subjectId, newSubjectName);
        }

        public void ChangeSubjectSubjectId(int subjectId, string newSubjectSubjectId)
        {
            //_myDatabaseWrapper.ChangeSubjectSubjectID(subjectId, newSubjectSubjectId);
        }

        public void ImportMeasureFiles(List<string> measureFilesList, List<string> dtpFilesList, int samplesPerSecond, int butterFilterOrder,
            int butterFilterCutOffPosition, int butterFilterCutOffForce, int percentPeakVelocity,
            int timeNormalizationSamples)
        {
            //TODO: WHAT/WHERE TO DO WITH THE dtpFilesList NOW?...
            //Should be somewhere, where the szenarioParseDefinitions are used, as the dtp files are supposed to replace those...
            //Add new Task to the Taskmanager which handles the execution of the tasks
            TaskManager.PushBack(Task.Factory.StartNew(delegate
            {
                //Sleep until Taskmanager gets cancelled(?) or current task is the first task in the Taskmanager(?)... Something seems wrong here, but it's from Matthias
                //and it works I guess, so don't touch it?
                while (TaskManager.GetIndex(Task.CurrentId) != 0 & !TaskManager.Cancel)
                {
                    Thread.Sleep(100);
                }

                _myManipAnalysisGui.EnableTabPages(false);
                _myManipAnalysisGui.SetProgressBarValue(0);

                //Creating new Matlabinstances for each processor to parallelize
                var cpuCount = Environment.ProcessorCount;
                var taskMatlabWrappers = new List<MatlabWrapper>();
                for (var i = 0; i < cpuCount; i++)
                {
                    taskMatlabWrappers.Add(new MatlabWrapper(_myManipAnalysisGui,
                        MatlabWrapper.MatlabInstanceType.Single));
                }

                for (var files = 0; files < measureFilesList.Count & !TaskManager.Cancel; files++)
                {
                    try
                    {
                        while (TaskManager.Pause & !TaskManager.Cancel)
                        {
                            Thread.Sleep(100);
                        }

                        _myManipAnalysisGui.SetProgressBarValue(100.0 / measureFilesList.Count * files);

                        var filename = measureFilesList.ElementAt(files);
                        
                        

                        var tempFileHash = Md5.ComputeHash(filename);

                        if (!_myDatabaseWrapper.CheckIfMeasureFileHashExists(tempFileHash))
                        {
                            //var myParser = new BioMotionBotMeasureFileParser(trialsContainer, _myManipAnalysisGui);
                            var myParser = new KinarmMeasureFileParser(_myManipAnalysisGui);

                            _myManipAnalysisGui.WriteProgressInfo("Parsing file...");
                            //In this if-condition the c3d data is parsed from the file filename.
                            //Here we now need to also include the proper dtp file.
                            if (myParser.ParseFile(filename, dtpFilesList) && myParser.TrialsContainer.Count > 0)
                            {
                                var trialsContainer = myParser.TrialsContainer;
                                var taskTrialListParts = new List<List<Trial>>();
                                var threadCount = 0;

                                if (trialsContainer.Count > cpuCount)
                                {
                                    for (var cpuCounter = 0; cpuCounter < cpuCount; cpuCounter++)
                                    {
                                        taskTrialListParts.Add(new List<Trial>());
                                    }

                                    var trialCounter = 0;
                                    var listCounter = 0;
                                    while (trialCounter < trialsContainer.Count)
                                    {
                                        taskTrialListParts[listCounter].Add(trialsContainer[trialCounter]);
                                        trialCounter++;
                                        listCounter++;
                                        if (listCounter >= cpuCount)
                                        {
                                            listCounter = 0;
                                        }
                                    }

                                    threadCount = cpuCount;
                                }
                                else
                                {
                                    taskTrialListParts.Add(trialsContainer);
                                    threadCount = 1;
                                }

                                var calculatingTasks = new List<Task>();

                                for (var i = 0; i < threadCount; i++)
                                {
                                    var tempTaskTrialList = taskTrialListParts.ElementAt(i).ToList();
                                    var tempMatlabWrapper = taskMatlabWrappers.ElementAt(i);

                                    _myManipAnalysisGui.WriteProgressInfo("Processing data...");
                                    calculatingTasks.Add(Task.Factory.StartNew(delegate
                                    {
                                        try {
                                            var taskTrialList = tempTaskTrialList;
                                            var taskMatlabWrapper = tempMatlabWrapper;

                                            ButterWorthFilter(taskMatlabWrapper, taskTrialList, butterFilterOrder,
                                                butterFilterCutOffPosition, butterFilterCutOffForce, samplesPerSecond);

                                            VelocityCalculation(taskMatlabWrapper, taskTrialList, samplesPerSecond);

                                            TimeNormalization(taskMatlabWrapper, taskTrialList, timeNormalizationSamples,
                                                percentPeakVelocity);

                                            lock (calculatingTasks)
                                            {
                                                calculatingTasks.Remove(calculatingTasks.First(t => t.Id == Task.CurrentId));
                                            }
                                        }
                                        catch (Exception ex) {
                                            _myManipAnalysisGui.WriteToLogBox("Exception: " + ex.ToString());
                                        }
                                    }));
                                }

                                while (calculatingTasks.Any())
                                {
                                    Thread.Sleep(500);
                                }
                                
                                _myManipAnalysisGui.WriteProgressInfo("Calculating szenario mean times...");
                                var szenarioMeanTimesContainer = CalculateSzenarioMeanTimes(trialsContainer);
                                
                                _myManipAnalysisGui.WriteProgressInfo("Compressing data...");
                                CompressTrialData(trialsContainer);

                                _myManipAnalysisGui.WriteProgressInfo("Uploading into database...");
                                try
                                {
                                    _myDatabaseWrapper.Insert(trialsContainer);
                                    _myDatabaseWrapper.Insert(szenarioMeanTimesContainer);
                                }
                                catch
                                {
                                    _myDatabaseWrapper.RemoveMeasureFile(trialsContainer[0].MeasureFile);
                                    throw;
                                }
                            }
                            else
                            {
                                _myManipAnalysisGui.WriteToLogBox("Skipping \"" + filename + "\"");
                            }
                        }
                        else
                        {
                            _myManipAnalysisGui.WriteToLogBox("File already imported: " +
                                                              measureFilesList.ElementAt(files));
                        }
                    }
                    catch (Exception ex)
                    {
                        _myManipAnalysisGui.WriteToLogBox("Error in \"" + measureFilesList.ElementAt(files) + "\":\n" +
                                                          ex + "\nSkipped file.");
                        taskMatlabWrappers.ForEach(t => t.Dispose());
                    }
                    finally
                    {
                        taskMatlabWrappers.ForEach(t => t.Dispose());
                    }
                }
                taskMatlabWrappers.ForEach(t => t.Dispose());
                _myManipAnalysisGui.SetProgressBarValue(0);
                _myManipAnalysisGui.WriteProgressInfo("Ready");
                _myManipAnalysisGui.EnableTabPages(true);

                TaskManager.Remove(Task.CurrentId);
            }));
        }

        private void ButterWorthFilter(MatlabWrapper myMatlabWrapper, List<Trial> trialsContainer, int butterFilterOrder,
            int butterFilterCutOffPosition, int butterFilterCutOffForce, int samplesPerSecond)
        {
            myMatlabWrapper.SetWorkspaceData("filterOrder", Convert.ToDouble(butterFilterOrder));
            myMatlabWrapper.SetWorkspaceData("cutoffFreqPosition", Convert.ToDouble(butterFilterCutOffPosition));
            myMatlabWrapper.SetWorkspaceData("cutoffFreqForce", Convert.ToDouble(butterFilterCutOffForce));
            myMatlabWrapper.SetWorkspaceData("samplesPerSecond", Convert.ToDouble(samplesPerSecond));
            myMatlabWrapper.Execute(
                "[bPosition,aPosition] = butter(filterOrder,(cutoffFreqPosition/(samplesPerSecond/2)));");
            myMatlabWrapper.Execute("[bForce,aForce] = butter(filterOrder,(cutoffFreqForce/(samplesPerSecond/2)));");

            for (var trialCounter = 0; trialCounter < trialsContainer.Count; trialCounter++)
            {
                try {
                    myMatlabWrapper.SetWorkspaceData("force_actual_x",
                        trialsContainer[trialCounter].MeasuredForcesRaw.Select(t => t.X).ToArray());
                    myMatlabWrapper.SetWorkspaceData("force_actual_y",
                        trialsContainer[trialCounter].MeasuredForcesRaw.Select(t => t.Y).ToArray());
                    myMatlabWrapper.SetWorkspaceData("force_actual_z",
                        trialsContainer[trialCounter].MeasuredForcesRaw.Select(t => t.Z).ToArray());

                    if (trialsContainer[trialCounter].NominalForcesRaw != null)
                    {
                        myMatlabWrapper.SetWorkspaceData("force_nominal_x",
                            trialsContainer[trialCounter].NominalForcesRaw.Select(t => t.X).ToArray());
                        myMatlabWrapper.SetWorkspaceData("force_nominal_y",
                            trialsContainer[trialCounter].NominalForcesRaw.Select(t => t.Y).ToArray());
                        myMatlabWrapper.SetWorkspaceData("force_nominal_z",
                            trialsContainer[trialCounter].NominalForcesRaw.Select(t => t.Z).ToArray());
                    }

                    myMatlabWrapper.SetWorkspaceData("force_moment_x",
                        trialsContainer[trialCounter].MomentForcesRaw.Select(t => t.X).ToArray());
                    myMatlabWrapper.SetWorkspaceData("force_moment_y",
                        trialsContainer[trialCounter].MomentForcesRaw.Select(t => t.Y).ToArray());
                    myMatlabWrapper.SetWorkspaceData("force_moment_z",
                        trialsContainer[trialCounter].MomentForcesRaw.Select(t => t.Z).ToArray());

                    myMatlabWrapper.SetWorkspaceData("position_cartesian_x",
                        trialsContainer[trialCounter].PositionRaw.Select(t => t.X).ToArray());
                    myMatlabWrapper.SetWorkspaceData("position_cartesian_y",
                        trialsContainer[trialCounter].PositionRaw.Select(t => t.Y).ToArray());
                    myMatlabWrapper.SetWorkspaceData("position_cartesian_z",
                        trialsContainer[trialCounter].PositionRaw.Select(t => t.Z).ToArray());

                    myMatlabWrapper.Execute("force_actual_x = filtfilt(bForce, aForce, force_actual_x);");
                    myMatlabWrapper.Execute("force_actual_y = filtfilt(bForce, aForce, force_actual_y);");
                    myMatlabWrapper.Execute("force_actual_z = filtfilt(bForce, aForce, force_actual_z);");

                    if (trialsContainer[trialCounter].NominalForcesRaw != null)
                    {
                        myMatlabWrapper.Execute("force_nominal_x = filtfilt(bForce, aForce,force_nominal_x);");
                        myMatlabWrapper.Execute("force_nominal_y = filtfilt(bForce, aForce,force_nominal_y);");
                        myMatlabWrapper.Execute("force_nominal_z = filtfilt(bForce, aForce,force_nominal_z);");
                    }
                    myMatlabWrapper.Execute("force_moment_x = filtfilt(bForce, aForce, force_moment_x);");
                    myMatlabWrapper.Execute("force_moment_y = filtfilt(bForce, aForce, force_moment_y);");
                    myMatlabWrapper.Execute("force_moment_z = filtfilt(bForce, aForce, force_moment_z);");

                    myMatlabWrapper.Execute("position_cartesian_x = filtfilt(bPosition, aPosition, position_cartesian_x);");
                    myMatlabWrapper.Execute("position_cartesian_y = filtfilt(bPosition, aPosition, position_cartesian_y);");
                    myMatlabWrapper.Execute("position_cartesian_z = filtfilt(bPosition, aPosition, position_cartesian_z);");


                    double[,] forceActualX = myMatlabWrapper.GetWorkspaceData("force_actual_x");
                    double[,] forceActualY = myMatlabWrapper.GetWorkspaceData("force_actual_y");
                    double[,] forceActualZ = myMatlabWrapper.GetWorkspaceData("force_actual_z");

                    double[,] forceNominalX = null;
                    double[,] forceNominalY = null;
                    double[,] forceNominalZ = null;
                    if (trialsContainer[trialCounter].NominalForcesRaw != null)
                    {
                        forceNominalX = myMatlabWrapper.GetWorkspaceData("force_nominal_x");
                        forceNominalY = myMatlabWrapper.GetWorkspaceData("force_nominal_y");
                        forceNominalZ = myMatlabWrapper.GetWorkspaceData("force_nominal_z");
                    }
                    double[,] forceMomentX = myMatlabWrapper.GetWorkspaceData("force_moment_x");
                    double[,] forceMomentY = myMatlabWrapper.GetWorkspaceData("force_moment_y");
                    double[,] forceMomentZ = myMatlabWrapper.GetWorkspaceData("force_moment_z");

                    double[,] positionCartesianX = myMatlabWrapper.GetWorkspaceData("position_cartesian_x");
                    double[,] positionCartesianY = myMatlabWrapper.GetWorkspaceData("position_cartesian_y");
                    double[,] positionCartesianZ = myMatlabWrapper.GetWorkspaceData("position_cartesian_z");


                    trialsContainer[trialCounter].MeasuredForcesFiltered = new List<ForceContainer>();
                    if (trialsContainer[trialCounter].NominalForcesRaw != null)
                    {
                        trialsContainer[trialCounter].NominalForcesFiltered = new List<ForceContainer>();
                    }
                    trialsContainer[trialCounter].MomentForcesFiltered = new List<ForceContainer>();
                    trialsContainer[trialCounter].PositionFiltered = new List<PositionContainer>();

                    for (var frameCount = 0; frameCount < trialsContainer[trialCounter].PositionRaw.Count; frameCount++)
                    {
                        var measuredForcesFiltered = new ForceContainer();
                        ForceContainer nominalForcesFiltered = null;
                        if (trialsContainer[trialCounter].NominalForcesRaw != null)
                        {
                            nominalForcesFiltered = new ForceContainer();
                        }
                        var momentForcesFiltered = new ForceContainer();
                        var positionFiltered = new PositionContainer();

                        measuredForcesFiltered.PositionStatus =
                            trialsContainer[trialCounter].MeasuredForcesRaw[frameCount].PositionStatus;
                        measuredForcesFiltered.TimeStamp =
                            trialsContainer[trialCounter].MeasuredForcesRaw[frameCount].TimeStamp;
                        measuredForcesFiltered.X = forceActualX[0, frameCount];
                        measuredForcesFiltered.Y = forceActualY[0, frameCount];
                        measuredForcesFiltered.Z = forceActualZ[0, frameCount];

                        if (trialsContainer[trialCounter].NominalForcesRaw != null && nominalForcesFiltered != null && forceNominalX != null && forceNominalY != null && forceNominalZ != null)
                        {
                            nominalForcesFiltered.PositionStatus =
                                trialsContainer[trialCounter].NominalForcesRaw[frameCount].PositionStatus;
                            nominalForcesFiltered.TimeStamp =
                                trialsContainer[trialCounter].NominalForcesRaw[frameCount].TimeStamp;
                            nominalForcesFiltered.X = forceNominalX[0, frameCount];
                            nominalForcesFiltered.Y = forceNominalY[0, frameCount];
                            nominalForcesFiltered.Z = forceNominalZ[0, frameCount];
                        }

                        momentForcesFiltered.PositionStatus =
                            trialsContainer[trialCounter].MomentForcesRaw[frameCount].PositionStatus;
                        momentForcesFiltered.TimeStamp = trialsContainer[trialCounter].MomentForcesRaw[frameCount].TimeStamp;
                        momentForcesFiltered.X = forceMomentX[0, frameCount];
                        momentForcesFiltered.Y = forceMomentY[0, frameCount];
                        momentForcesFiltered.Z = forceMomentZ[0, frameCount];

                        positionFiltered.PositionStatus =
                            trialsContainer[trialCounter].PositionRaw[frameCount].PositionStatus;
                        positionFiltered.TimeStamp = trialsContainer[trialCounter].PositionRaw[frameCount].TimeStamp;
                        positionFiltered.X = positionCartesianX[0, frameCount];
                        positionFiltered.Y = positionCartesianY[0, frameCount];
                        positionFiltered.Z = positionCartesianZ[0, frameCount];

                        trialsContainer[trialCounter].MeasuredForcesFiltered.Add(measuredForcesFiltered);
                        if (trialsContainer[trialCounter].NominalForcesRaw != null)
                        {
                            trialsContainer[trialCounter].NominalForcesFiltered.Add(nominalForcesFiltered);
                        }
                        trialsContainer[trialCounter].MomentForcesFiltered.Add(momentForcesFiltered);
                        trialsContainer[trialCounter].PositionFiltered.Add(positionFiltered);

                        trialsContainer[trialCounter].FilteredDataSampleRate =
                            trialsContainer[trialCounter].RawDataSampleRate;
                    }

                    myMatlabWrapper.ClearWorkspaceData("force_actual_x");
                    myMatlabWrapper.ClearWorkspaceData("force_actual_y");
                    myMatlabWrapper.ClearWorkspaceData("force_actual_z");

                    myMatlabWrapper.ClearWorkspaceData("force_nominal_x");
                    myMatlabWrapper.ClearWorkspaceData("force_nominal_y");
                    myMatlabWrapper.ClearWorkspaceData("force_nominal_z");

                    myMatlabWrapper.ClearWorkspaceData("force_moment_x");
                    myMatlabWrapper.ClearWorkspaceData("force_moment_y");
                    myMatlabWrapper.ClearWorkspaceData("force_moment_z");

                    myMatlabWrapper.ClearWorkspaceData("position_cartesian_x");
                    myMatlabWrapper.ClearWorkspaceData("position_cartesian_y");
                    myMatlabWrapper.ClearWorkspaceData("position_cartesian_z");
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox("Could not filter Trial " + trialsContainer[trialCounter].TrialNumberInSzenario);
                }
            }

            myMatlabWrapper.ClearWorkspace();
        }


        private void VelocityCalculation(MatlabWrapper myMatlabWrapper, List<Trial> trialsContainer,
            double samplesPerSecond)
        {
            for (var trialCounter = 0; trialCounter < trialsContainer.Count; trialCounter++)
            {
                myMatlabWrapper.SetWorkspaceData("position_cartesian_x",
                    trialsContainer[trialCounter].PositionFiltered.Select(t => t.X).ToArray());
                myMatlabWrapper.SetWorkspaceData("position_cartesian_y",
                    trialsContainer[trialCounter].PositionFiltered.Select(t => t.Y).ToArray());
                myMatlabWrapper.SetWorkspaceData("position_cartesian_z",
                    trialsContainer[trialCounter].PositionFiltered.Select(t => t.Z).ToArray());

                myMatlabWrapper.SetWorkspaceData("sampleRate", samplesPerSecond);

                myMatlabWrapper.Execute("velocity_x = numDiff(position_cartesian_x, sampleRate);");
                myMatlabWrapper.Execute("velocity_y = numDiff(position_cartesian_y, sampleRate);");
                myMatlabWrapper.Execute("velocity_z = numDiff(position_cartesian_z, sampleRate);");

                double[,] velocityX = myMatlabWrapper.GetWorkspaceData("velocity_x");
                double[,] velocityY = myMatlabWrapper.GetWorkspaceData("velocity_y");
                double[,] velocityZ = myMatlabWrapper.GetWorkspaceData("velocity_z");

                trialsContainer[trialCounter].VelocityFiltered = new List<VelocityContainer>();

                for (var frameCount = 0;
                    frameCount < trialsContainer[trialCounter].PositionFiltered.Count;
                    frameCount++)
                {
                    var velocityFiltered = new VelocityContainer();
                    velocityFiltered.PositionStatus =
                        trialsContainer[trialCounter].PositionFiltered[frameCount].PositionStatus;
                    velocityFiltered.TimeStamp = trialsContainer[trialCounter].PositionFiltered[frameCount].TimeStamp;
                    velocityFiltered.X = velocityX[0, frameCount];
                    velocityFiltered.Y = velocityY[0, frameCount];
                    velocityFiltered.Z = velocityZ[0, frameCount];

                    trialsContainer[trialCounter].VelocityFiltered.Add(velocityFiltered);
                }

                myMatlabWrapper.ClearWorkspace();
            }
            myMatlabWrapper.ClearWorkspace();
        }

        private void TimeNormalization(MatlabWrapper myMatlabWrapper, List<Trial> trialsContainer,
            int timeNormalizationSamples, double percentPeakVelocity)
        {
            for (var trialCounter = 0; trialCounter < trialsContainer.Count; trialCounter++)
            {
                myMatlabWrapper.SetWorkspaceData("newSampleRate", Convert.ToDouble(timeNormalizationSamples));

                trialsContainer[trialCounter].NormalizedDataSampleRate = timeNormalizationSamples;
                trialsContainer[trialCounter].VelocityTrimThresholdPercent = percentPeakVelocity;
                //###
                /* That was used for debuggin only
                var test = trialsContainer[trialCounter].VelocityFiltered.Where(t => t.PositionStatus == 1);
                if (test.Count() == 0)
                {
                    _myManipAnalysisGui.WriteToLogBox("No matching position Status == 1 for trial: " + trialsContainer[trialCounter].TrialNumberInSzenario);
                }
                */
                //###
                trialsContainer[trialCounter].VelocityTrimThresholdForTrial =
                    trialsContainer[trialCounter].VelocityFiltered.Where(t => t.PositionStatus == 1)
                        .Max(t => Math.Sqrt(Math.Pow(t.X, 2) + Math.Pow(t.Y, 2))) / 100.0 * percentPeakVelocity;

                DateTime startTime;
                DateTime stopTime;
                try
                {
                    // First element with PositionStatus == 0 and a velocity higher than the threshold
                    startTime =
                        trialsContainer[trialCounter].VelocityFiltered.Where(t => t.PositionStatus == 0)
                            .OrderBy(t => t.TimeStamp)
                            .First(
                                t =>
                                    Math.Sqrt(Math.Pow(t.X, 2) + Math.Pow(t.Y, 2)) >=
                                    trialsContainer[trialCounter].VelocityTrimThresholdForTrial)
                            .TimeStamp;
                }
                catch
                {
                    // First element with PositionStatus == 1
                    startTime =
                        trialsContainer[trialCounter].VelocityFiltered.OrderBy(t => t.TimeStamp)
                            .First(t => t.PositionStatus == 1)
                            .TimeStamp;
                }

                try
                {
                    // First element with PositionStatus == 2 and a velocity lower than the threshold
                    stopTime =
                        trialsContainer[trialCounter].VelocityFiltered.Where(t => t.PositionStatus == 2)
                            .OrderBy(t => t.TimeStamp)
                            .First(
                                t =>
                                    Math.Sqrt(Math.Pow(t.X, 2) + Math.Pow(t.Y, 2)) <=
                                    trialsContainer[trialCounter].VelocityTrimThresholdForTrial)
                            .TimeStamp;
                }
                catch
                {
                    // Last element with PositionStatus == 2
                    stopTime = trialsContainer[trialCounter].VelocityFiltered.OrderBy(t => t.TimeStamp).Last().TimeStamp;
                }

                IEnumerable<ForceContainer> measuredForcesFilteredCut =
                    trialsContainer[trialCounter].MeasuredForcesFiltered.Where(
                        t => t.TimeStamp >= startTime && t.TimeStamp <= stopTime).OrderBy(t => t.TimeStamp);

                IEnumerable<ForceContainer> momentForcesFilteredCut =
                    trialsContainer[trialCounter].MomentForcesFiltered.Where(
                        t => t.TimeStamp >= startTime && t.TimeStamp <= stopTime).OrderBy(t => t.TimeStamp);

                IEnumerable<ForceContainer> nominalForcesFilteredCut = null;
                if (trialsContainer[trialCounter].NominalForcesFiltered != null)
                {
                    nominalForcesFilteredCut =
                        trialsContainer[trialCounter].NominalForcesFiltered.Where(
                            t => t.TimeStamp >= startTime && t.TimeStamp <= stopTime).OrderBy(t => t.TimeStamp);
                }

                IEnumerable<PositionContainer> positionFilteredCut =
                    trialsContainer[trialCounter].PositionFiltered.Where(
                        t => t.TimeStamp >= startTime && t.TimeStamp <= stopTime).OrderBy(t => t.TimeStamp);

                IEnumerable<VelocityContainer> velocityFilteredCut =
                    trialsContainer[trialCounter].VelocityFiltered.Where(
                        t => t.TimeStamp >= startTime && t.TimeStamp <= stopTime).OrderBy(t => t.TimeStamp);


                myMatlabWrapper.SetWorkspaceData("measure_data_time",
                    positionFilteredCut.Select(t => Convert.ToDouble(t.TimeStamp.Ticks)).ToArray());

                myMatlabWrapper.SetWorkspaceData("forceActualX", measuredForcesFilteredCut.Select(t => t.X).ToArray());
                myMatlabWrapper.SetWorkspaceData("forceActualY", measuredForcesFilteredCut.Select(t => t.Y).ToArray());
                myMatlabWrapper.SetWorkspaceData("forceActualZ", measuredForcesFilteredCut.Select(t => t.Z).ToArray());

                if (nominalForcesFilteredCut != null)
                {
                    myMatlabWrapper.SetWorkspaceData("forceNominalX",
                        nominalForcesFilteredCut.Select(t => t.X).ToArray());
                    myMatlabWrapper.SetWorkspaceData("forceNominalY",
                        nominalForcesFilteredCut.Select(t => t.Y).ToArray());
                    myMatlabWrapper.SetWorkspaceData("forceNominalY",
                        nominalForcesFilteredCut.Select(t => t.Z).ToArray());
                }

                myMatlabWrapper.SetWorkspaceData("forceMomentX", momentForcesFilteredCut.Select(t => t.X).ToArray());
                myMatlabWrapper.SetWorkspaceData("forceMomentY", momentForcesFilteredCut.Select(t => t.Y).ToArray());
                myMatlabWrapper.SetWorkspaceData("forceMomentZ", momentForcesFilteredCut.Select(t => t.Z).ToArray());

                myMatlabWrapper.SetWorkspaceData("positionCartesianX", positionFilteredCut.Select(t => t.X).ToArray());
                myMatlabWrapper.SetWorkspaceData("positionCartesianY", positionFilteredCut.Select(t => t.Y).ToArray());
                myMatlabWrapper.SetWorkspaceData("positionCartesianZ", positionFilteredCut.Select(t => t.Z).ToArray());

                myMatlabWrapper.SetWorkspaceData("velocityX", velocityFilteredCut.Select(t => t.X).ToArray());
                myMatlabWrapper.SetWorkspaceData("velocityY", velocityFilteredCut.Select(t => t.Y).ToArray());
                myMatlabWrapper.SetWorkspaceData("velocityZ", velocityFilteredCut.Select(t => t.Z).ToArray());

                myMatlabWrapper.SetWorkspaceData("positionStatus",
                    velocityFilteredCut.Select(t => Convert.ToDouble(t.PositionStatus)).ToArray());

                var errorList = new List<string>();

                myMatlabWrapper.Execute(
                    "[errorvar1, forceActualX, newMeasureTime] = timeNorm(forceActualX, measure_data_time, newSampleRate);");
                myMatlabWrapper.Execute(
                    "[errorvar2, forceActualY, newMeasureTime] = timeNorm(forceActualY, measure_data_time, newSampleRate);");
                myMatlabWrapper.Execute(
                    "[errorvar3, forceActualZ, newMeasureTime] = timeNorm(forceActualZ, measure_data_time, newSampleRate);");

                if (nominalForcesFilteredCut != null)
                {
                    myMatlabWrapper.Execute(
                        "[errorvar4, forceNominalX, newMeasureTime] = timeNorm(forceNominalX, measure_data_time, newSampleRate);");
                    myMatlabWrapper.Execute(
                        "[errorvar5, forceNominalY, newMeasureTime] = timeNorm(forceNominalY, measure_data_time, newSampleRate);");
                    myMatlabWrapper.Execute(
                        "[errorvar6, forceNominalZ, newMeasureTime] = timeNorm(forceNominalZ, measure_data_time, newSampleRate);");
                }

                myMatlabWrapper.Execute(
                    "[errorvar7, forceMomentX, newMeasureTime] = timeNorm(forceMomentX, measure_data_time, newSampleRate);");
                myMatlabWrapper.Execute(
                    "[errorvar8, forceMomentY, newMeasureTime] = timeNorm(forceMomentY, measure_data_time, newSampleRate);");
                myMatlabWrapper.Execute(
                    "[errorvar9, forceMomentZ, newMeasureTime] = timeNorm(forceMomentZ, measure_data_time, newSampleRate);");

                myMatlabWrapper.Execute(
                    "[errorvar10, positionCartesianX, newMeasureTime] = timeNorm(positionCartesianX, measure_data_time, newSampleRate);");
                myMatlabWrapper.Execute(
                    "[errorvar11, positionCartesianY, newMeasureTime] = timeNorm(positionCartesianY, measure_data_time, newSampleRate);");
                myMatlabWrapper.Execute(
                    "[errorvar12, positionCartesianZ, newMeasureTime] = timeNorm(positionCartesianZ, measure_data_time, newSampleRate);");

                myMatlabWrapper.Execute(
                    "[errorvar13, velocityX, newMeasureTime] = timeNorm(velocityX, measure_data_time, newSampleRate);");
                myMatlabWrapper.Execute(
                    "[errorvar14, velocityY, newMeasureTime] = timeNorm(velocityY, measure_data_time, newSampleRate);");
                myMatlabWrapper.Execute(
                    "[errorvar15, velocityZ, newMeasureTime] = timeNorm(velocityZ, measure_data_time, newSampleRate);");

                myMatlabWrapper.Execute(
                    "[errorvar16, positionStatus, newMeasureTime] = timeNorm(positionStatus, measure_data_time, newSampleRate);");


                for (var errorVarCounterCounter = 1; errorVarCounterCounter <= 16; errorVarCounterCounter++)
                {
                    if (nominalForcesFilteredCut == null && errorVarCounterCounter >= 4 && errorVarCounterCounter <= 6)
                    {
                    }
                    else
                    {
                        errorList.Add(
                            Convert.ToString(myMatlabWrapper.GetWorkspaceData("errorvar" + errorVarCounterCounter)));
                    }
                }

                if (errorList.Any(t => !string.IsNullOrEmpty(t)))
                {
                    var output =
                        errorList.Where(t => !string.IsNullOrEmpty(t))
                            .Select(
                                t =>
                                    t + " in " + trialsContainer[trialCounter].MeasureFile.FileName +
                                    " at szenario-trial-number " + trialsContainer[trialCounter].TrialNumberInSzenario)
                            .Aggregate("", (current, line) => current + line);
                    _myManipAnalysisGui.WriteToLogBox(output);
                }

                double[,] measureDataTime = myMatlabWrapper.GetWorkspaceData("newMeasureTime");

                double[,] forceActualX = myMatlabWrapper.GetWorkspaceData("forceActualX");
                double[,] forceActualY = myMatlabWrapper.GetWorkspaceData("forceActualY");
                double[,] forceActualZ = myMatlabWrapper.GetWorkspaceData("forceActualZ");

                double[,] forceNominalX = null;
                double[,] forceNominalY = null;
                double[,] forceNominalZ = null;
                if (nominalForcesFilteredCut != null)
                {
                    forceNominalX = myMatlabWrapper.GetWorkspaceData("forceNominalX");
                    forceNominalY = myMatlabWrapper.GetWorkspaceData("forceNominalY");
                    forceNominalZ = myMatlabWrapper.GetWorkspaceData("forceNominalZ");
                }
                double[,] forceMomentX = myMatlabWrapper.GetWorkspaceData("forceMomentX");
                double[,] forceMomentY = myMatlabWrapper.GetWorkspaceData("forceMomentY");
                double[,] forceMomentZ = myMatlabWrapper.GetWorkspaceData("forceMomentZ");

                double[,] positionCartesianX = myMatlabWrapper.GetWorkspaceData("positionCartesianX");
                double[,] positionCartesianY = myMatlabWrapper.GetWorkspaceData("positionCartesianY");
                double[,] positionCartesianZ = myMatlabWrapper.GetWorkspaceData("positionCartesianZ");

                double[,] positionStatus = myMatlabWrapper.GetWorkspaceData("positionStatus");

                double[,] velocityX = myMatlabWrapper.GetWorkspaceData("velocityX");
                double[,] velocityY = myMatlabWrapper.GetWorkspaceData("velocityY");
                double[,] velocityZ = myMatlabWrapper.GetWorkspaceData("velocityZ");

                //-----

                trialsContainer[trialCounter].MeasuredForcesNormalized = new List<ForceContainer>();
                if (nominalForcesFilteredCut != null)
                {
                    trialsContainer[trialCounter].NominalForcesNormalized = new List<ForceContainer>();
                }
                trialsContainer[trialCounter].MomentForcesNormalized = new List<ForceContainer>();
                trialsContainer[trialCounter].PositionNormalized = new List<PositionContainer>();
                trialsContainer[trialCounter].VelocityNormalized = new List<VelocityContainer>();

                for (var frameCount = 0; frameCount < measureDataTime.Length; frameCount++)
                {
                    var newPositionStatus = Convert.ToInt32(positionStatus[frameCount, 0]);
                    var newTimeStamp = new DateTime(Convert.ToInt64(measureDataTime[frameCount, 0]));

                    var measuredForcesNormalized = new ForceContainer();
                    ForceContainer nominalForcesNormalized = null;
                    if (nominalForcesFilteredCut != null)
                    {
                        nominalForcesNormalized = new ForceContainer();
                    }
                    var momentForcesNormalized = new ForceContainer();
                    var positionNormalized = new PositionContainer();
                    var velocityNormalized = new VelocityContainer();

                    measuredForcesNormalized.PositionStatus = newPositionStatus;
                    measuredForcesNormalized.TimeStamp = newTimeStamp;
                    measuredForcesNormalized.X = forceActualX[frameCount, 0];
                    measuredForcesNormalized.Y = forceActualY[frameCount, 0];
                    measuredForcesNormalized.Z = forceActualZ[frameCount, 0];

                    if (nominalForcesNormalized != null)
                    {
                        nominalForcesNormalized.PositionStatus = newPositionStatus;
                        nominalForcesNormalized.TimeStamp = newTimeStamp;
                        nominalForcesNormalized.X = forceNominalX[frameCount, 0];
                        nominalForcesNormalized.Y = forceNominalY[frameCount, 0];
                        nominalForcesNormalized.Z = forceNominalZ[frameCount, 0];
                    }

                    momentForcesNormalized.PositionStatus = newPositionStatus;
                    momentForcesNormalized.TimeStamp = newTimeStamp;
                    momentForcesNormalized.X = forceMomentX[frameCount, 0];
                    momentForcesNormalized.Y = forceMomentY[frameCount, 0];
                    momentForcesNormalized.Z = forceMomentZ[frameCount, 0];

                    positionNormalized.PositionStatus = newPositionStatus;
                    positionNormalized.TimeStamp = newTimeStamp;
                    positionNormalized.X = positionCartesianX[frameCount, 0];
                    positionNormalized.Y = positionCartesianY[frameCount, 0];
                    positionNormalized.Z = positionCartesianZ[frameCount, 0];

                    velocityNormalized.PositionStatus = newPositionStatus;
                    velocityNormalized.TimeStamp = newTimeStamp;
                    velocityNormalized.X = velocityX[frameCount, 0];
                    velocityNormalized.Y = velocityY[frameCount, 0];
                    velocityNormalized.Z = velocityZ[frameCount, 0];

                    trialsContainer[trialCounter].MeasuredForcesNormalized.Add(measuredForcesNormalized);
                    if (nominalForcesNormalized != null)
                    {
                        trialsContainer[trialCounter].NominalForcesNormalized.Add(nominalForcesNormalized);
                    }
                    trialsContainer[trialCounter].MomentForcesNormalized.Add(momentForcesNormalized);
                    trialsContainer[trialCounter].PositionNormalized.Add(positionNormalized);
                    trialsContainer[trialCounter].VelocityNormalized.Add(velocityNormalized);
                }

                myMatlabWrapper.ClearWorkspace();
            }

            myMatlabWrapper.ClearWorkspace();
        }

        private List<SzenarioMeanTime> CalculateSzenarioMeanTimes(List<Trial> trialsContainer)
        {
            var szenarioMeanTimes = new List<SzenarioMeanTime>();

            foreach (var szenario in trialsContainer.Select(t => t.Szenario).Distinct())
            {
                var szenarioTrialsContainer = trialsContainer.Where(t => t.Szenario == szenario);
                foreach (var targetCounter in szenarioTrialsContainer.Select(t => t.Target.Number).Distinct())
                {
                    var tempSzenarioMeanTime = new SzenarioMeanTime();

                    tempSzenarioMeanTime.Group = szenarioTrialsContainer.ElementAt(0).Group;
                    tempSzenarioMeanTime.MeasureFile = szenarioTrialsContainer.ElementAt(0).MeasureFile;
                    tempSzenarioMeanTime.Study = szenarioTrialsContainer.ElementAt(0).Study;
                    tempSzenarioMeanTime.Subject = szenarioTrialsContainer.ElementAt(0).Subject;
                    tempSzenarioMeanTime.Szenario = szenarioTrialsContainer.ElementAt(0).Szenario;
                    tempSzenarioMeanTime.Target = szenarioTrialsContainer.ElementAt(0).Target;
                    tempSzenarioMeanTime.Origin = szenarioTrialsContainer.ElementAt(0).Origin;

                    long[] targetDurationTimes = null;
                    lock (trialsContainer)
                    {
                        targetDurationTimes =
                            trialsContainer.Where(t => t.Target.Number == targetCounter)
                                .Select(
                                    t =>
                                        t.PositionNormalized.Max(u => u.TimeStamp.Ticks) -
                                        t.PositionNormalized.Min(u => u.TimeStamp.Ticks))
                                .ToArray();
                    }
                    tempSzenarioMeanTime.MeanTime = new TimeSpan(Convert.ToInt64(targetDurationTimes.Average()));
                    tempSzenarioMeanTime.MeanTimeStd = new TimeSpan(Convert.ToInt64(targetDurationTimes.StdDev()));

                    lock (szenarioMeanTimes)
                    {
                        szenarioMeanTimes.Add(tempSzenarioMeanTime);
                    }
                }
            }

            return szenarioMeanTimes;
        }

        public void CompressTrialData(List<Trial> trialsContainer)
        {
            Parallel.For(0, trialsContainer.Count, trialCounter =>
            {
                if (trialsContainer[trialCounter].MeasuredForcesFiltered != null)
                {
                    var data = Gzip<List<ForceContainer>>.Compress(trialsContainer[trialCounter].MeasuredForcesFiltered);
                    lock (trialsContainer)
                    {
                        trialsContainer[trialCounter].ZippedMeasuredForcesFiltered = data;
                        trialsContainer[trialCounter].MeasuredForcesFiltered = null;
                    }
                }
                if (trialsContainer[trialCounter].MeasuredForcesNormalized != null)
                {
                    var data =
                        Gzip<List<ForceContainer>>.Compress(trialsContainer[trialCounter].MeasuredForcesNormalized);
                    lock (trialsContainer)
                    {
                        trialsContainer[trialCounter].ZippedMeasuredForcesNormalized = data;
                        trialsContainer[trialCounter].MeasuredForcesNormalized = null;
                    }
                }
                if (trialsContainer[trialCounter].MeasuredForcesRaw != null)
                {
                    var data = Gzip<List<ForceContainer>>.Compress(trialsContainer[trialCounter].MeasuredForcesRaw);
                    lock (trialsContainer)
                    {
                        trialsContainer[trialCounter].ZippedMeasuredForcesRaw = data;
                        trialsContainer[trialCounter].MeasuredForcesRaw = null;
                    }
                }
                if (trialsContainer[trialCounter].NominalForcesFiltered != null)
                {
                    var data = Gzip<List<ForceContainer>>.Compress(trialsContainer[trialCounter].NominalForcesFiltered);
                    lock (trialsContainer)
                    {
                        trialsContainer[trialCounter].ZippedNominalForcesFiltered = data;
                        trialsContainer[trialCounter].NominalForcesFiltered = null;
                    }
                }
                if (trialsContainer[trialCounter].NominalForcesNormalized != null)
                {
                    var data = Gzip<List<ForceContainer>>.Compress(trialsContainer[trialCounter].NominalForcesNormalized);
                    lock (trialsContainer)
                    {
                        trialsContainer[trialCounter].ZippedNominalForcesNormalized = data;
                        trialsContainer[trialCounter].NominalForcesNormalized = null;
                    }
                }
                if (trialsContainer[trialCounter].NominalForcesRaw != null)
                {
                    var data = Gzip<List<ForceContainer>>.Compress(trialsContainer[trialCounter].NominalForcesRaw);
                    lock (trialsContainer)
                    {
                        trialsContainer[trialCounter].ZippedNominalForcesRaw = data;
                        trialsContainer[trialCounter].NominalForcesRaw = null;
                    }
                }
                if (trialsContainer[trialCounter].MomentForcesFiltered != null)
                {
                    var data = Gzip<List<ForceContainer>>.Compress(trialsContainer[trialCounter].MomentForcesFiltered);
                    lock (trialsContainer)
                    {
                        trialsContainer[trialCounter].ZippedMomentForcesFiltered = data;
                        trialsContainer[trialCounter].MomentForcesFiltered = null;
                    }
                }
                if (trialsContainer[trialCounter].MomentForcesNormalized != null)
                {
                    var data = Gzip<List<ForceContainer>>.Compress(trialsContainer[trialCounter].MomentForcesNormalized);
                    lock (trialsContainer)
                    {
                        trialsContainer[trialCounter].ZippedMomentForcesNormalized = data;
                        trialsContainer[trialCounter].MomentForcesNormalized = null;
                    }
                }
                if (trialsContainer[trialCounter].MomentForcesRaw != null)
                {
                    var data = Gzip<List<ForceContainer>>.Compress(trialsContainer[trialCounter].MomentForcesRaw);
                    lock (trialsContainer)
                    {
                        trialsContainer[trialCounter].ZippedMomentForcesRaw = data;
                        trialsContainer[trialCounter].MomentForcesRaw = null;
                    }
                }
                if (trialsContainer[trialCounter].PositionFiltered != null)
                {
                    var data = Gzip<List<PositionContainer>>.Compress(trialsContainer[trialCounter].PositionFiltered);
                    lock (trialsContainer)
                    {
                        trialsContainer[trialCounter].ZippedPositionFiltered = data;
                        trialsContainer[trialCounter].PositionFiltered = null;
                    }
                }
                if (trialsContainer[trialCounter].PositionNormalized != null)
                {
                    var data = Gzip<List<PositionContainer>>.Compress(trialsContainer[trialCounter].PositionNormalized);
                    lock (trialsContainer)
                    {
                        trialsContainer[trialCounter].ZippedPositionNormalized = data;
                        trialsContainer[trialCounter].PositionNormalized = null;
                    }
                }
                if (trialsContainer[trialCounter].PositionRaw != null)
                {
                    var data = Gzip<List<PositionContainer>>.Compress(trialsContainer[trialCounter].PositionRaw);
                    lock (trialsContainer)
                    {
                        trialsContainer[trialCounter].ZippedPositionRaw = data;
                        trialsContainer[trialCounter].PositionRaw = null;
                    }
                }
                if (trialsContainer[trialCounter].VelocityFiltered != null)
                {
                    var data = Gzip<List<VelocityContainer>>.Compress(trialsContainer[trialCounter].VelocityFiltered);
                    lock (trialsContainer)
                    {
                        trialsContainer[trialCounter].ZippedVelocityFiltered = data;
                        trialsContainer[trialCounter].VelocityFiltered = null;
                    }
                }
                if (trialsContainer[trialCounter].VelocityNormalized != null)
                {
                    var data = Gzip<List<VelocityContainer>>.Compress(trialsContainer[trialCounter].VelocityNormalized);
                    lock (trialsContainer)
                    {
                        trialsContainer[trialCounter].ZippedVelocityNormalized = data;
                        trialsContainer[trialCounter].VelocityNormalized = null;
                    }
                }
                if (trialsContainer[trialCounter].Statistics != null)
                {
                    var data = Gzip<StatisticContainer>.Compress(trialsContainer[trialCounter].Statistics);
                    lock (trialsContainer)
                    {
                        trialsContainer[trialCounter].ZippedStatistics = data;
                        trialsContainer[trialCounter].Statistics = null;
                    }
                }
            });
        }

        public void CompressBaselineData(List<Baseline> baselinesContainer)
        {
            Parallel.For(0, baselinesContainer.Count, baselineCounter =>
            {
                if (baselinesContainer[baselineCounter].MeasuredForces != null)
                {
                    var data = Gzip<List<ForceContainer>>.Compress(baselinesContainer[baselineCounter].MeasuredForces);
                    lock (baselinesContainer)
                    {
                        baselinesContainer[baselineCounter].ZippedMeasuredForces = data;
                        baselinesContainer[baselineCounter].MeasuredForces = null;
                    }
                }
                if (baselinesContainer[baselineCounter].MomentForces != null)
                {
                    var data = Gzip<List<ForceContainer>>.Compress(baselinesContainer[baselineCounter].MomentForces);
                    lock (baselinesContainer)
                    {
                        baselinesContainer[baselineCounter].ZippedMomentForces = data;
                        baselinesContainer[baselineCounter].MomentForces = null;
                    }
                }
                if (baselinesContainer[baselineCounter].NominalForces != null)
                {
                    var data = Gzip<List<ForceContainer>>.Compress(baselinesContainer[baselineCounter].NominalForces);
                    lock (baselinesContainer)
                    {
                        baselinesContainer[baselineCounter].ZippedNominalForces = data;
                        baselinesContainer[baselineCounter].NominalForces = null;
                    }
                }
                if (baselinesContainer[baselineCounter].Position != null)
                {
                    var data = Gzip<List<PositionContainer>>.Compress(baselinesContainer[baselineCounter].Position);
                    lock (baselinesContainer)
                    {
                        baselinesContainer[baselineCounter].ZippedPosition = data;
                        baselinesContainer[baselineCounter].Position = null;
                    }
                }
                if (baselinesContainer[baselineCounter].Velocity != null)
                {
                    var data = Gzip<List<VelocityContainer>>.Compress(baselinesContainer[baselineCounter].Velocity);
                    lock (baselinesContainer)
                    {
                        baselinesContainer[baselineCounter].ZippedVelocity = data;
                        baselinesContainer[baselineCounter].Velocity = null;
                    }
                }
            });
        }

        /// <summary>
        /// This is a horrible method! It loops over the studies that are in one dropDownEntry of ManipAnalysis in the database and depending on the if clause that matches
        /// a studies name  (which is a terrible solution as well!) it groups the trials of that study that belong to the baseline
        /// by selecting them manually by their trialNumberInSzenario. To group the trials, they must be grouped in such a way that 
        /// all trials in one group share the same metadata, because the helperfunction "doBaselineCalculation" is also very bad
        /// and can not handle groups that do not share the same metadata...
        /// I might want to improve this greatly in the future, as with automated parsing, there should be no more need for all
        /// this hassle...
        /// </summary>
        public void CalculateBaselines()
        {
            TaskManager.PushBack(Task.Factory.StartNew(delegate
            {
                while (TaskManager.GetIndex(Task.CurrentId) != 0 & !TaskManager.Cancel)
                {
                    Thread.Sleep(100);
                }

                _myManipAnalysisGui.EnableTabPages(false);
                _myManipAnalysisGui.WriteProgressInfo("Calculating baselines...");

                try
                {
                    var baselineFields = Builders<Trial>.Projection.Include(t1 => t1.ZippedPositionNormalized);
                    baselineFields = baselineFields.Include(t2 => t2.ZippedVelocityNormalized);
                    baselineFields = baselineFields.Include(t3 => t3.ZippedMeasuredForcesNormalized);
                    baselineFields = baselineFields.Include(t4 => t4.ZippedMomentForcesNormalized);
                    baselineFields = baselineFields.Include(t5 => t5.TrialType);
                    baselineFields = baselineFields.Include(t6 => t6.ForceFieldType);
                    baselineFields = baselineFields.Include(t7 => t7.Handedness);
                    baselineFields = baselineFields.Include(t8 => t8.Study);
                    baselineFields = baselineFields.Include(t9 => t9.Group);
                    baselineFields = baselineFields.Include(t10 => t10.Subject);
                    baselineFields = baselineFields.Include(t11 => t11.TrialNumberInSzenario);
                    baselineFields = baselineFields.Include(t12 => t12.Target);
                    baselineFields = baselineFields.Include(t13 => t13.Origin);
                    baselineFields = baselineFields.Include(t14 => t14.NormalizedDataSampleRate);
                    baselineFields = baselineFields.Include(t15 => t15.Id);
                    var baselinesContainer = new List<Baseline>();
                    //GetStudys returns the name of the studies that is found in the c3d description of the trials of the study that is currently selected in the GUI
                    //For older studies, GetStudies returns the studyName that was set in the szenarioParseDefintionFile.
                    var studys = _myDatabaseWrapper.GetStudys();

                    //To use doBaselineCalculation, we must split the trials in such a way, that each group of trials that we give to 
                    //the doBaselineCalculation() must have the same meta data, as this method can not handle groups of trials with different metadata.
                    //Therefore we must split into the same:
                    /*
                    - trialtype
                    - ForcefieldType
                    - Handedness
                    - Study
                    - Group
                    - Subject
                    - Szenario
                    - Target
                    - Origin
                    In practice it should be enough to split by trialtype, forcefieldtype and handedness,
                    as we have to loop over study, group and subject anyway(?) and the szenario is fix (*Baseline*)
                    */

                    foreach (var study in studys)
                    {
                        var groups = _myDatabaseWrapper.GetGroups(study);
                        foreach (var group in groups)
                        {
                            var baselineSzenarios = _myDatabaseWrapper.GetBaselineSzenarios(study, group);
                            foreach (var baselineSzenario in baselineSzenarios)
                            {
                                var subjects = _myDatabaseWrapper.GetSubjects(study, group, baselineSzenario);
                                foreach (var subject in subjects)
                                {
                                    
                                    var turnDateTime = _myDatabaseWrapper.GetTurns(study, group, baselineSzenario, subject).ElementAt(0);
                                    //baselineTrials gets filled with all trials that belong to that szenario
                                    var baselineTrials =
                                        _myDatabaseWrapper.GetTrials(study, group, baselineSzenario,
                                        subject, turnDateTime, baselineFields).ToList();
                                    //Unzipping the trialdata and writing it in the trial
                                    baselineTrials.ForEach(
                                       t =>
                                           t.PositionNormalized =
                                               Gzip<List<PositionContainer>>.DeCompress(t.ZippedPositionNormalized)
                                                   .OrderBy(u => u.TimeStamp)
                                                   .ToList());
                                    baselineTrials.ForEach(
                                        t =>
                                            t.VelocityNormalized =
                                                Gzip<List<VelocityContainer>>.DeCompress(t.ZippedVelocityNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    baselineTrials.ForEach(
                                        t =>
                                            t.MeasuredForcesNormalized =
                                                Gzip<List<ForceContainer>>.DeCompress(t.ZippedMeasuredForcesNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    baselineTrials.ForEach(
                                        t =>
                                            t.MomentForcesNormalized =
                                                Gzip<List<ForceContainer>>.DeCompress(t.ZippedMomentForcesNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    //group the baselineTrials into metadatagroups:
                                    //FFCW_RH, FFCW_LH, FFCCW_RH. FFCCW_LH, FC_RH, FC_LH, NF_RH, NF_LH
                                    //We don't have the logic for vicon trials yet, but vicon trials will probably also never be used anymore?
                                    //If you have vicontrials somewhere, you will probably also have to add groups where you check the handedness for 
                                    //RightHandVicon and LeftHandVicon instead of RightHand and LeftHand...

                                    /*
                                    Right now we take all trials from the baseLineSzenario.
                                    For the future maybe we don't want all trials of the baseline that fulfill these criteria in a group
                                    but for example only the first 6, then we just change the .Where function so that it only takes the first 6 occurences.
                                    */
                                    var forceFieldCWRightHand = baselineTrials.Where(t => t.ForceFieldType == Trial.ForceFieldTypeEnum.ForceFieldCW
                                    && t.Handedness == Trial.HandednessEnum.RightHand
                                    && t.TrialType != Trial.TrialTypeEnum.ErrorClampTrial).ToList();

                                    var forceFieldCWLeftHand = baselineTrials.Where(t => t.ForceFieldType == Trial.ForceFieldTypeEnum.ForceFieldCW
                                    && t.Handedness == Trial.HandednessEnum.LeftHand
                                    && t.TrialType != Trial.TrialTypeEnum.ErrorClampTrial).ToList();

                                    var forceFieldCCWRightHand = baselineTrials.Where(t => t.ForceFieldType == Trial.ForceFieldTypeEnum.ForceFieldCCW
                                    && t.Handedness == Trial.HandednessEnum.RightHand
                                    && t.TrialType != Trial.TrialTypeEnum.ErrorClampTrial).ToList();

                                    var forceFieldCCWLeftHand = baselineTrials.Where(t => t.ForceFieldType == Trial.ForceFieldTypeEnum.ForceFieldCCW
                                    && t.Handedness == Trial.HandednessEnum.LeftHand
                                    && t.TrialType != Trial.TrialTypeEnum.ErrorClampTrial).ToList();

                                    var forceFieldDFRightHand = baselineTrials.Where(t => t.ForceFieldType == Trial.ForceFieldTypeEnum.ForceFieldDF
                                    && t.Handedness == Trial.HandednessEnum.RightHand
                                    && t.TrialType != Trial.TrialTypeEnum.ErrorClampTrial).ToList();

                                    var forceFieldDFLeftHand = baselineTrials.Where(t => t.ForceFieldType == Trial.ForceFieldTypeEnum.ForceFieldDF
                                    && t.Handedness == Trial.HandednessEnum.LeftHand
                                    && t.TrialType != Trial.TrialTypeEnum.ErrorClampTrial).ToList();

                                    var forceChannelRightHand = baselineTrials.Where(t => t.TrialType == Trial.TrialTypeEnum.ErrorClampTrial
                                    && t.Handedness == Trial.HandednessEnum.RightHand).ToList();

                                    var forceChannelLeftHand = baselineTrials.Where(t => t.TrialType == Trial.TrialTypeEnum.ErrorClampTrial
                                    && t.Handedness == Trial.HandednessEnum.LeftHand).ToList();

                                    var nullFieldRightHand = baselineTrials.Where(t => t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField
                                    && t.Handedness == Trial.HandednessEnum.RightHand
                                    && t.TrialType != Trial.TrialTypeEnum.ErrorClampTrial).ToList();

                                    var nullFieldLeftHand = baselineTrials.Where(t => t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField
                                    && t.Handedness == Trial.HandednessEnum.LeftHand
                                    && t.TrialType != Trial.TrialTypeEnum.ErrorClampTrial).ToList();

                                    //Add the groups to the baseLineContainer if they contain data.
                                    if (forceFieldCWRightHand.Any())
                                    {
                                        baselinesContainer.AddRange(doBaselineCalculation(forceFieldCWRightHand));
                                    }
                                    if (forceFieldCWLeftHand.Any())
                                    {
                                        baselinesContainer.AddRange(doBaselineCalculation(forceFieldCWLeftHand));
                                    }
                                    if (forceFieldCCWRightHand.Any())
                                    {
                                        baselinesContainer.AddRange(doBaselineCalculation(forceFieldCCWRightHand));
                                    }
                                    if (forceFieldCCWLeftHand.Any())
                                    {
                                        baselinesContainer.AddRange(doBaselineCalculation(forceFieldCCWLeftHand));
                                    }
                                    if (forceFieldDFRightHand.Any())
                                    {
                                        baselinesContainer.AddRange(doBaselineCalculation(forceFieldDFRightHand));
                                    }
                                    if (forceFieldDFLeftHand.Any())
                                    {
                                        baselinesContainer.AddRange(doBaselineCalculation(forceFieldDFLeftHand));
                                    }
                                    if (forceChannelRightHand.Any())
                                    {
                                        baselinesContainer.AddRange(doBaselineCalculation(forceChannelRightHand));
                                    }
                                    if (forceChannelLeftHand.Any())
                                    {
                                        baselinesContainer.AddRange(doBaselineCalculation(forceChannelLeftHand));
                                    }
                                    if (nullFieldRightHand.Any())
                                    {
                                        baselinesContainer.AddRange(doBaselineCalculation(nullFieldRightHand));
                                    }
                                    if (nullFieldLeftHand.Any())
                                    {
                                        baselinesContainer.AddRange(doBaselineCalculation(nullFieldLeftHand));
                                    }
                                }
                            }
                        }
                    }



                    //DEPRECATED & UNSAFE! ##############################################
                    foreach (var study in studys)
                    {
                        if (study == "Study_12_HEiKA")
                        {
                            var groups = _myDatabaseWrapper.GetGroups(study);
                            foreach (var group in groups)
                            {
                                var subjects = _myDatabaseWrapper.GetSubjects(study, group, "Baseline");
                                foreach (var subject in subjects)
                                {
                                    var turnDateTime = _myDatabaseWrapper.GetTurns(study, group, "Baseline", subject).ElementAt(0);
                                    var baselineTrials =
                                        _myDatabaseWrapper.GetTrials(study, group, "Baseline", subject, turnDateTime,
                                            Enumerable.Range(1, 216), baselineFields).ToList();
                                    //baselineTrials enthält laut Debugger nur Einträge mit Szenario = "null"...
                                    
                                    baselineTrials.ForEach(
                                       t =>
                                           t.PositionNormalized =
                                               Gzip<List<PositionContainer>>.DeCompress(t.ZippedPositionNormalized)
                                                   .OrderBy(u => u.TimeStamp)
                                                   .ToList());
                                    baselineTrials.ForEach(
                                        t =>
                                            t.VelocityNormalized =
                                                Gzip<List<VelocityContainer>>.DeCompress(t.ZippedVelocityNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    baselineTrials.ForEach(
                                        t =>
                                            t.MeasuredForcesNormalized =
                                                Gzip<List<ForceContainer>>.DeCompress(t.ZippedMeasuredForcesNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    baselineTrials.ForEach(
                                        t =>
                                            t.MomentForcesNormalized =
                                                Gzip<List<ForceContainer>>.DeCompress(t.ZippedMomentForcesNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());

                                    var forceFieldRightHandBaselineTrials = baselineTrials.Where(t => t.TrialNumberInSzenario == 8 ||
                                    t.TrialNumberInSzenario == 20 ||
                                    t.TrialNumberInSzenario == 32 ||
                                    t.TrialNumberInSzenario == 43 ||
                                    t.TrialNumberInSzenario == 105 ||
                                    t.TrialNumberInSzenario == 119).ToList();

                                    var forceFieldLeftHandBaselineTrials = baselineTrials.Where(t => t.TrialNumberInSzenario == 56 ||
                                    t.TrialNumberInSzenario == 68 ||
                                    t.TrialNumberInSzenario == 80 ||
                                    t.TrialNumberInSzenario == 91 ||
                                    t.TrialNumberInSzenario == 153 ||
                                    t.TrialNumberInSzenario == 167).ToList();

                                    var forceChannelRightHandBaselineTrials = baselineTrials.Where(t => (t.TrialNumberInSzenario >= 139 &&
                                    t.TrialNumberInSzenario <= 144)
                                    || (t.TrialNumberInSzenario >= 211 && t.TrialNumberInSzenario <= 216)).ToList();

                                    var forceChannelLeftHandBaselineTrials = baselineTrials.Where(t => (t.TrialNumberInSzenario >= 187 &&
                                    t.TrialNumberInSzenario <= 192)
                                    || (t.TrialNumberInSzenario >= 199 && t.TrialNumberInSzenario <= 204)).ToList();

                                    var nullFieldRightHandBaselineTrials = baselineTrials.Where(t => (t.TrialNumberInSzenario >= 133 &&
                                    t.TrialNumberInSzenario <= 138)
                                    || (t.TrialNumberInSzenario >= 205 && t.TrialNumberInSzenario <= 210)).ToList();

                                    var nullFieldLeftHandBaselineTrials = baselineTrials.Where(t => (t.TrialNumberInSzenario >= 181 &&
                                    t.TrialNumberInSzenario <= 186)
                                    || (t.TrialNumberInSzenario >= 193 && t.TrialNumberInSzenario <= 198)).ToList();


                                    if (
                                        forceFieldRightHandBaselineTrials.All(
                                            t =>
                                                t.TrialType == Trial.TrialTypeEnum.StandardTrial &&
                                                (t.ForceFieldType == Trial.ForceFieldTypeEnum.ForceFieldCW
                                                || t.ForceFieldType == Trial.ForceFieldTypeEnum.ForceFieldDF) &&
                                                t.Handedness == Trial.HandednessEnum.RightHandVicon)
                                        &&
                                        forceFieldLeftHandBaselineTrials.All(
                                            t =>
                                                t.TrialType == Trial.TrialTypeEnum.StandardTrial &&
                                                (t.ForceFieldType == Trial.ForceFieldTypeEnum.ForceFieldCW
                                                || t.ForceFieldType == Trial.ForceFieldTypeEnum.ForceFieldDF) &&
                                                t.Handedness == Trial.HandednessEnum.LeftHandVicon)
                                        &&
                                        forceChannelRightHandBaselineTrials.All(
                                            t =>
                                                t.TrialType == Trial.TrialTypeEnum.ErrorClampTrial &&
                                                t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField &&
                                                t.Handedness == Trial.HandednessEnum.RightHandVicon)
                                        &&
                                        forceChannelLeftHandBaselineTrials.All(
                                            t =>
                                                t.TrialType == Trial.TrialTypeEnum.ErrorClampTrial &&
                                                t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField &&
                                                t.Handedness == Trial.HandednessEnum.LeftHandVicon)
                                        &&
                                        nullFieldRightHandBaselineTrials.All(
                                            t =>
                                                t.TrialType == Trial.TrialTypeEnum.StandardTrial &&
                                                t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField &&
                                                t.Handedness == Trial.HandednessEnum.RightHandVicon)
                                        &&
                                        nullFieldLeftHandBaselineTrials.All(
                                            t =>
                                                t.TrialType == Trial.TrialTypeEnum.StandardTrial &&
                                                t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField &&
                                                t.Handedness == Trial.HandednessEnum.LeftHandVicon))
                                    {
                                        baselinesContainer.AddRange(doBaselineCalculation(forceFieldRightHandBaselineTrials));
                                        baselinesContainer.AddRange(doBaselineCalculation(forceFieldLeftHandBaselineTrials));
                                        baselinesContainer.AddRange(doBaselineCalculation(forceChannelRightHandBaselineTrials));
                                        baselinesContainer.AddRange(doBaselineCalculation(forceChannelLeftHandBaselineTrials));
                                        baselinesContainer.AddRange(doBaselineCalculation(nullFieldRightHandBaselineTrials));
                                        baselinesContainer.AddRange(doBaselineCalculation(nullFieldLeftHandBaselineTrials));
                                    }
                                    else
                                    {
                                        _myManipAnalysisGui.WriteToLogBox(
                                            "Error calculating Baseline. Incorrect TrialTypes. " + study + " / " + group +
                                            " / " + subject);
                                    }
                                }
                            }
                        }

                        //else if study == "Study 10_DAVOS"...
                        else if (study == "Study 10_DAVOS")
                        {

                            var groups = _myDatabaseWrapper.GetGroups(study);

                            foreach (var group in groups)
                            {
                                var subjects = _myDatabaseWrapper.GetSubjects(study, group, "02_baseline");

                                foreach (var subject in subjects)
                                {
                                    var turnDateTime =
                                        _myDatabaseWrapper.GetTurns(study, group, "02_baseline", subject).ElementAt(0);

                                    //var baselineTrialsNull = _myDatabaseWrapper.GetTrials(study, group, "02_baseline", subject, turnDateTime, Enumerable.Range(19, 12),
                                    //    baselineFields).ToList();
                                    //var baselineTrialsEC = _myDatabaseWrapper.GetTrials(study, group, "02_baseline", subject, turnDateTime, Enumerable.Range(31, 37),
                                    //    baselineFields).ToList();

                                    //Hier testen, ob baselineTrialsNull jemals 12 Trials enthält?
                                    var baselineTrialsNull =
                                        _myDatabaseWrapper.GetTrials(study, group, "02_baseline", subject, turnDateTime,
                                            Extensions.AlternateRange(37, 24), baselineFields).ToList();

                                    var baselineTrialsEC =
                                        _myDatabaseWrapper.GetTrials(study, group, "02_baseline", subject, turnDateTime,
                                            Extensions.AlternateRange(61, 12), baselineFields).ToList();

                                    baselineTrialsNull.ForEach(
                                        t =>
                                            t.PositionNormalized =
                                                Gzip<List<PositionContainer>>.DeCompress(t.ZippedPositionNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    baselineTrialsNull.ForEach(
                                        t =>
                                            t.VelocityNormalized =
                                                Gzip<List<VelocityContainer>>.DeCompress(t.ZippedVelocityNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    baselineTrialsNull.ForEach(
                                        t =>
                                            t.MeasuredForcesNormalized =
                                                Gzip<List<ForceContainer>>.DeCompress(t.ZippedMeasuredForcesNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    baselineTrialsNull.ForEach(
                                        t =>
                                            t.MomentForcesNormalized =
                                                Gzip<List<ForceContainer>>.DeCompress(t.ZippedMomentForcesNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());

                                    baselineTrialsEC.ForEach(
                                       t =>
                                           t.PositionNormalized =
                                               Gzip<List<PositionContainer>>.DeCompress(t.ZippedPositionNormalized)
                                                   .OrderBy(u => u.TimeStamp)
                                                   .ToList());
                                    baselineTrialsEC.ForEach(
                                        t =>
                                            t.VelocityNormalized =
                                                Gzip<List<VelocityContainer>>.DeCompress(t.ZippedVelocityNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    baselineTrialsEC.ForEach(
                                        t =>
                                            t.MeasuredForcesNormalized =
                                                Gzip<List<ForceContainer>>.DeCompress(t.ZippedMeasuredForcesNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    baselineTrialsEC.ForEach(
                                        t =>
                                            t.MomentForcesNormalized =
                                                Gzip<List<ForceContainer>>.DeCompress(t.ZippedMomentForcesNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());

                                    if (
                                        baselineTrialsNull.All(
                                            t =>
                                                t.TrialType == Trial.TrialTypeEnum.StandardTrial &&
                                                t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField &&
                                                t.Handedness == Trial.HandednessEnum.RightHand)
                                        &&
                                        baselineTrialsEC.All(
                                            t =>
                                                t.TrialType == Trial.TrialTypeEnum.ErrorClampTrial &&
                                                t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField &&
                                                t.Handedness == Trial.HandednessEnum.RightHand))
                                    {
                                        baselinesContainer.AddRange(doBaselineCalculation(baselineTrialsNull));
                                        baselinesContainer.AddRange(doBaselineCalculation(baselineTrialsEC));
                                    }
                                    else
                                    {
                                        _myManipAnalysisGui.WriteToLogBox(
                                            "Error calculating Baseline. Incorrect TrialTypes. " + study + " / " + group +
                                            " / " + subject);
                                    }
                                }
                            }

                        }
                        else if (study == "Study 10")
                        {
                            var groups = _myDatabaseWrapper.GetGroups(study);

                            foreach (var group in groups)
                            {
                                var subjects = _myDatabaseWrapper.GetSubjects(study, group, "02_baseline");

                                foreach (var subject in subjects)
                                {
                                    var turnDateTime =
                                        _myDatabaseWrapper.GetTurns(study, group, "02_baseline", subject).ElementAt(0);

                                    var baselineTrialsNull =
                                        _myDatabaseWrapper.GetTrials(study, group, "02_baseline", subject, turnDateTime,
                                            Extensions.AlternateRange(1, 64), baselineFields).ToList();

                                    var baselineTrialsEC =
                                        _myDatabaseWrapper.GetTrials(study, group, "02_baseline", subject, turnDateTime,
                                            Extensions.AlternateRange(65, 80), baselineFields).ToList();

                                    baselineTrialsNull.ForEach(
                                        t =>
                                            t.PositionNormalized =
                                                Gzip<List<PositionContainer>>.DeCompress(t.ZippedPositionNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    baselineTrialsNull.ForEach(
                                        t =>
                                            t.VelocityNormalized =
                                                Gzip<List<VelocityContainer>>.DeCompress(t.ZippedVelocityNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    baselineTrialsNull.ForEach(
                                        t =>
                                            t.MeasuredForcesNormalized =
                                                Gzip<List<ForceContainer>>.DeCompress(t.ZippedMeasuredForcesNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    baselineTrialsNull.ForEach(
                                        t =>
                                            t.MomentForcesNormalized =
                                                Gzip<List<ForceContainer>>.DeCompress(t.ZippedMomentForcesNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());

                                    baselineTrialsEC.ForEach(
                                       t =>
                                           t.PositionNormalized =
                                               Gzip<List<PositionContainer>>.DeCompress(t.ZippedPositionNormalized)
                                                   .OrderBy(u => u.TimeStamp)
                                                   .ToList());
                                    baselineTrialsEC.ForEach(
                                        t =>
                                            t.VelocityNormalized =
                                                Gzip<List<VelocityContainer>>.DeCompress(t.ZippedVelocityNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    baselineTrialsEC.ForEach(
                                        t =>
                                            t.MeasuredForcesNormalized =
                                                Gzip<List<ForceContainer>>.DeCompress(t.ZippedMeasuredForcesNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    baselineTrialsEC.ForEach(
                                        t =>
                                            t.MomentForcesNormalized =
                                                Gzip<List<ForceContainer>>.DeCompress(t.ZippedMomentForcesNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());

                                    if (
                                        baselineTrialsNull.All(
                                            t =>
                                                t.TrialType == Trial.TrialTypeEnum.StandardTrial &&
                                                t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField &&
                                                t.Handedness == Trial.HandednessEnum.RightHand)
                                        &&
                                        baselineTrialsEC.All(
                                            t =>
                                                t.TrialType == Trial.TrialTypeEnum.ErrorClampTrial &&
                                                t.ForceFieldType == Trial.ForceFieldTypeEnum.ForceFieldCW &&
                                                t.Handedness == Trial.HandednessEnum.RightHand))
                                    {
                                        baselinesContainer.AddRange(doBaselineCalculation(baselineTrialsNull));
                                        baselinesContainer.AddRange(doBaselineCalculation(baselineTrialsEC));
                                    }
                                    else
                                    {
                                        _myManipAnalysisGui.WriteToLogBox(
                                            "Error calculating Baseline. Incorrect TrialTypes. " + study + " / " + group +
                                            " / " + subject);
                                    }
                                }
                            }
                        }
                        else if (study == "Study 09")
                        {
                            var baseLineName = "02_CI_baseline";
                            var groups = _myDatabaseWrapper.GetGroups(study);

                            foreach (var group in groups)
                            {
                                var subjects = _myDatabaseWrapper.GetSubjects(study, group, baseLineName);

                                foreach (var subject in subjects)
                                {
                                    var turnDateTime =
                                        _myDatabaseWrapper.GetTurns(study, group, baseLineName, subject).ElementAt(0);

                                    var baselineTrialsNull =
                                        _myDatabaseWrapper.GetTrials(study, group, baseLineName, subject, turnDateTime,
                                            Extensions.AlternateRange(33, 32), baselineFields).ToList();
                                    var baselineTrialsFF =
                                        _myDatabaseWrapper.GetTrials(study, group, baseLineName, subject, turnDateTime,
                                            Extensions.AlternateRange(65, 16), baselineFields).ToList();

                                    baselineTrialsNull.ForEach(
                                        t =>
                                            t.PositionNormalized =
                                                Gzip<List<PositionContainer>>.DeCompress(t.ZippedPositionNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    baselineTrialsNull.ForEach(
                                        t =>
                                            t.VelocityNormalized =
                                                Gzip<List<VelocityContainer>>.DeCompress(t.ZippedVelocityNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    baselineTrialsNull.ForEach(
                                        t =>
                                            t.MeasuredForcesNormalized =
                                                Gzip<List<ForceContainer>>.DeCompress(t.ZippedMeasuredForcesNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    baselineTrialsNull.ForEach(
                                        t =>
                                            t.MomentForcesNormalized =
                                                Gzip<List<ForceContainer>>.DeCompress(t.ZippedMomentForcesNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());

                                    baselineTrialsFF.ForEach(
                                        t =>
                                            t.PositionNormalized =
                                                Gzip<List<PositionContainer>>.DeCompress(t.ZippedPositionNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    baselineTrialsFF.ForEach(
                                        t =>
                                            t.VelocityNormalized =
                                                Gzip<List<VelocityContainer>>.DeCompress(t.ZippedVelocityNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    baselineTrialsFF.ForEach(
                                        t =>
                                            t.MeasuredForcesNormalized =
                                                Gzip<List<ForceContainer>>.DeCompress(t.ZippedMeasuredForcesNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    baselineTrialsFF.ForEach(
                                        t =>
                                            t.MomentForcesNormalized =
                                                Gzip<List<ForceContainer>>.DeCompress(t.ZippedMomentForcesNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());

                                    if (
                                        baselineTrialsNull.All(
                                            t =>
                                                t.TrialType == Trial.TrialTypeEnum.StandardTrial &&
                                                t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField &&
                                                t.Handedness == Trial.HandednessEnum.RightHand) &&
                                        baselineTrialsFF.All(
                                            t =>
                                                t.TrialType == Trial.TrialTypeEnum.ErrorClampTrial &&
                                                t.ForceFieldType == Trial.ForceFieldTypeEnum.ForceFieldCW &&
                                                t.Handedness == Trial.HandednessEnum.RightHand))
                                    {
                                        baselinesContainer.AddRange(doBaselineCalculation(baselineTrialsNull));
                                        baselinesContainer.AddRange(doBaselineCalculation(baselineTrialsFF));
                                    }
                                    else
                                    {
                                        _myManipAnalysisGui.WriteToLogBox(
                                            "Error calculating Baseline. Incorrect TrialTypes. " + study + " / " + group +
                                            " / " + subject);
                                    }
                                }
                            }
                        }
                        else if (study == "Study 08")
                        {
                            var groups = _myDatabaseWrapper.GetGroups(study);

                            foreach (var group in groups)
                            {
                                var subjects = _myDatabaseWrapper.GetSubjects(study, group, "Base1");

                                foreach (var subject in subjects)
                                {
                                    var turnDateTimeBase1 =
                                        _myDatabaseWrapper.GetTurns(study, group, "Base1", subject).ElementAt(0);
                                    var turnDateTimeBase2 =
                                        _myDatabaseWrapper.GetTurns(study, group, "Base2", subject).ElementAt(0);

                                    var baselineTrialsBase1 =
                                        _myDatabaseWrapper.GetTrials(study, group, "Base1", subject, turnDateTimeBase1,
                                            Enumerable.Range(1, 192), baselineFields).ToList();
                                    var baselineTrialsBase2 =
                                        _myDatabaseWrapper.GetTrials(study, group, "Base2", subject, turnDateTimeBase2,
                                            Enumerable.Range(1, 24), baselineFields).ToList();

                                    baselineTrialsBase1.ForEach(
                                        t =>
                                            t.PositionNormalized =
                                                Gzip<List<PositionContainer>>.DeCompress(t.ZippedPositionNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    baselineTrialsBase1.ForEach(
                                        t =>
                                            t.VelocityNormalized =
                                                Gzip<List<VelocityContainer>>.DeCompress(t.ZippedVelocityNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    baselineTrialsBase1.ForEach(
                                        t =>
                                            t.MeasuredForcesNormalized =
                                                Gzip<List<ForceContainer>>.DeCompress(t.ZippedMeasuredForcesNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    baselineTrialsBase1.ForEach(
                                        t =>
                                            t.MomentForcesNormalized =
                                                Gzip<List<ForceContainer>>.DeCompress(t.ZippedMomentForcesNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());

                                    baselineTrialsBase2.ForEach(
                                        t =>
                                            t.PositionNormalized =
                                                Gzip<List<PositionContainer>>.DeCompress(t.ZippedPositionNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    baselineTrialsBase2.ForEach(
                                        t =>
                                            t.VelocityNormalized =
                                                Gzip<List<VelocityContainer>>.DeCompress(t.ZippedVelocityNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    baselineTrialsBase2.ForEach(
                                        t =>
                                            t.MeasuredForcesNormalized =
                                                Gzip<List<ForceContainer>>.DeCompress(t.ZippedMeasuredForcesNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    baselineTrialsBase2.ForEach(
                                        t =>
                                            t.MomentForcesNormalized =
                                                Gzip<List<ForceContainer>>.DeCompress(t.ZippedMomentForcesNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());

                                    var forceFieldCatchTrialBaselineRightHand =
                                        baselineTrialsBase1.Where(
                                            t =>
                                                t.TrialNumberInSzenario == 18 || t.TrialNumberInSzenario == 31 ||
                                                t.TrialNumberInSzenario == 44 || t.TrialNumberInSzenario == 97 ||
                                                t.TrialNumberInSzenario == 110 || t.TrialNumberInSzenario == 123)
                                            .ToList();

                                    var forceFieldCatchTrialBaselineLeftHand =
                                        baselineTrialsBase1.Where(
                                            t =>
                                                t.TrialNumberInSzenario == 62 || t.TrialNumberInSzenario == 75 ||
                                                t.TrialNumberInSzenario == 88 || t.TrialNumberInSzenario == 145 ||
                                                t.TrialNumberInSzenario == 158 || t.TrialNumberInSzenario == 171)
                                            .ToList();

                                    var errorClampBaselineRightHand =
                                        baselineTrialsBase1.Where(
                                            t =>
                                                t.TrialNumberInSzenario == 15 || t.TrialNumberInSzenario == 28 ||
                                                t.TrialNumberInSzenario == 38 || t.TrialNumberInSzenario == 95 ||
                                                t.TrialNumberInSzenario == 104 || t.TrialNumberInSzenario == 117)
                                            .ToList();
                                    errorClampBaselineRightHand.AddRange(
                                        baselineTrialsBase2.Where(
                                            t => t.TrialNumberInSzenario >= 19 && t.TrialNumberInSzenario <= 24));

                                    var errorClampBaselineLeftHand =
                                        baselineTrialsBase1.Where(
                                            t =>
                                                t.TrialNumberInSzenario == 59 || t.TrialNumberInSzenario == 72 ||
                                                t.TrialNumberInSzenario == 82 || t.TrialNumberInSzenario == 143 ||
                                                t.TrialNumberInSzenario == 152 || t.TrialNumberInSzenario == 165)
                                            .ToList();
                                    errorClampBaselineLeftHand.AddRange(
                                        baselineTrialsBase2.Where(
                                            t => t.TrialNumberInSzenario >= 7 && t.TrialNumberInSzenario <= 12));

                                    var nullFieldBaselineRightHand =
                                        baselineTrialsBase1.Where(
                                            t => t.TrialNumberInSzenario >= 131 && t.TrialNumberInSzenario <= 136)
                                            .ToList();
                                    nullFieldBaselineRightHand.AddRange(
                                        baselineTrialsBase2.Where(
                                            t => t.TrialNumberInSzenario >= 13 && t.TrialNumberInSzenario <= 18));

                                    var nullFieldBaselineLeftHand =
                                        baselineTrialsBase1.Where(
                                            t => t.TrialNumberInSzenario >= 179 && t.TrialNumberInSzenario <= 184)
                                            .ToList();
                                    nullFieldBaselineLeftHand.AddRange(
                                        baselineTrialsBase2.Where(
                                            t => t.TrialNumberInSzenario >= 1 && t.TrialNumberInSzenario <= 6));

                                    if (
                                        (forceFieldCatchTrialBaselineLeftHand.All(
                                            t =>
                                                t.TrialType == Trial.TrialTypeEnum.StandardTrial &&
                                                t.ForceFieldType == Trial.ForceFieldTypeEnum.ForceFieldCW &&
                                                t.Handedness == Trial.HandednessEnum.LeftHand) ||
                                         forceFieldCatchTrialBaselineLeftHand.All(
                                             t =>
                                                 t.TrialType == Trial.TrialTypeEnum.StandardTrial &&
                                                 t.ForceFieldType == Trial.ForceFieldTypeEnum.ForceFieldCCW &&
                                                 t.Handedness == Trial.HandednessEnum.LeftHand)) &&
                                        (forceFieldCatchTrialBaselineRightHand.All(
                                            t =>
                                                t.TrialType == Trial.TrialTypeEnum.StandardTrial &&
                                                t.ForceFieldType == Trial.ForceFieldTypeEnum.ForceFieldCW &&
                                                t.Handedness == Trial.HandednessEnum.RightHand) ||
                                         forceFieldCatchTrialBaselineRightHand.All(
                                             t =>
                                                 t.TrialType == Trial.TrialTypeEnum.StandardTrial &&
                                                 t.ForceFieldType == Trial.ForceFieldTypeEnum.ForceFieldCCW &&
                                                 t.Handedness == Trial.HandednessEnum.RightHand)) &&
                                        errorClampBaselineLeftHand.All(
                                            t =>
                                                t.TrialType == Trial.TrialTypeEnum.ErrorClampTrial &&
                                                t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField &&
                                                t.Handedness == Trial.HandednessEnum.LeftHand) &&
                                        errorClampBaselineRightHand.All(
                                            t =>
                                                t.TrialType == Trial.TrialTypeEnum.ErrorClampTrial &&
                                                t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField &&
                                                t.Handedness == Trial.HandednessEnum.RightHand) &&
                                        nullFieldBaselineLeftHand.All(
                                            t =>
                                                t.TrialType == Trial.TrialTypeEnum.StandardTrial &&
                                                t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField &&
                                                t.Handedness == Trial.HandednessEnum.LeftHand) &&
                                        nullFieldBaselineRightHand.All(
                                            t =>
                                                t.TrialType == Trial.TrialTypeEnum.StandardTrial &&
                                                t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField &&
                                                t.Handedness == Trial.HandednessEnum.RightHand))
                                    {
                                        baselinesContainer.AddRange(
                                            doBaselineCalculation(forceFieldCatchTrialBaselineLeftHand));
                                        baselinesContainer.AddRange(
                                            doBaselineCalculation(forceFieldCatchTrialBaselineRightHand));
                                        baselinesContainer.AddRange(doBaselineCalculation(errorClampBaselineLeftHand));
                                        baselinesContainer.AddRange(doBaselineCalculation(errorClampBaselineRightHand));
                                        baselinesContainer.AddRange(doBaselineCalculation(nullFieldBaselineLeftHand));
                                        baselinesContainer.AddRange(doBaselineCalculation(nullFieldBaselineRightHand));
                                    }
                                    else
                                    {
                                        _myManipAnalysisGui.WriteToLogBox(
                                            "Error calculating Baseline. Incorrect TrialTypes. " + study + " / " + group +
                                            " / " + subject);
                                    }
                                }
                            }
                        }
                        else if (study == "Study 7")
                        {
                            var groups = _myDatabaseWrapper.GetGroups(study);

                            foreach (var group in groups)
                            {
                                var subjectsLR = _myDatabaseWrapper.GetSubjects(study, group, "LR_Base1");
                                var subjectsRL = _myDatabaseWrapper.GetSubjects(study, group, "RL_Base1");

                                foreach (var subject in subjectsLR)
                                {
                                    var turnBase1 =
                                        _myDatabaseWrapper.GetTurns(study, group, "LR_Base1", subject).ElementAt(0);
                                    var turnBase2a =
                                        _myDatabaseWrapper.GetTurns(study, group, "LR_Base2a", subject).ElementAt(0);
                                    var turnBase2b =
                                        _myDatabaseWrapper.GetTurns(study, group, "LR_Base2b", subject).ElementAt(0);

                                    var base1 =
                                        _myDatabaseWrapper.GetTrials(study, group, "LR_Base1", subject, turnBase1,
                                            Enumerable.Range(1, 216), baselineFields).ToList();
                                    var base2a =
                                        _myDatabaseWrapper.GetTrials(study, group, "LR_Base2a", subject, turnBase2a,
                                            Enumerable.Range(1, 12), baselineFields).ToList();
                                    var base2b =
                                        _myDatabaseWrapper.GetTrials(study, group, "LR_Base2b", subject, turnBase2b,
                                            Enumerable.Range(1, 12), baselineFields).ToList();

                                    base1.ForEach(
                                        t =>
                                            t.PositionNormalized =
                                                Gzip<List<PositionContainer>>.DeCompress(t.ZippedPositionNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    base1.ForEach(
                                        t =>
                                            t.VelocityNormalized =
                                                Gzip<List<VelocityContainer>>.DeCompress(t.ZippedVelocityNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    base1.ForEach(
                                        t =>
                                            t.MeasuredForcesNormalized =
                                                Gzip<List<ForceContainer>>.DeCompress(t.ZippedMeasuredForcesNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    base1.ForEach(
                                        t =>
                                            t.MomentForcesNormalized =
                                                Gzip<List<ForceContainer>>.DeCompress(t.ZippedMomentForcesNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());

                                    base2a.ForEach(
                                        t =>
                                            t.PositionNormalized =
                                                Gzip<List<PositionContainer>>.DeCompress(t.ZippedPositionNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    base2a.ForEach(
                                        t =>
                                            t.VelocityNormalized =
                                                Gzip<List<VelocityContainer>>.DeCompress(t.ZippedVelocityNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    base2a.ForEach(
                                        t =>
                                            t.MeasuredForcesNormalized =
                                                Gzip<List<ForceContainer>>.DeCompress(t.ZippedMeasuredForcesNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    base2a.ForEach(
                                        t =>
                                            t.MomentForcesNormalized =
                                                Gzip<List<ForceContainer>>.DeCompress(t.ZippedMomentForcesNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());

                                    base2b.ForEach(
                                        t =>
                                            t.PositionNormalized =
                                                Gzip<List<PositionContainer>>.DeCompress(t.ZippedPositionNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    base2b.ForEach(
                                        t =>
                                            t.VelocityNormalized =
                                                Gzip<List<VelocityContainer>>.DeCompress(t.ZippedVelocityNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    base2b.ForEach(
                                        t =>
                                            t.MeasuredForcesNormalized =
                                                Gzip<List<ForceContainer>>.DeCompress(t.ZippedMeasuredForcesNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    base2b.ForEach(
                                        t =>
                                            t.MomentForcesNormalized =
                                                Gzip<List<ForceContainer>>.DeCompress(t.ZippedMomentForcesNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());

                                    var forceFieldCatchTrialBaselineLeftHand =
                                        base1.Where(
                                            t =>
                                                t.TrialNumberInSzenario == 18 || t.TrialNumberInSzenario == 31 ||
                                                t.TrialNumberInSzenario == 44 || t.TrialNumberInSzenario == 117 ||
                                                t.TrialNumberInSzenario == 135 || t.TrialNumberInSzenario == 150)
                                            .ToList();

                                    var forceFieldCatchTrialBaselineRightHand =
                                        base1.Where(
                                            t =>
                                                t.TrialNumberInSzenario == 72 || t.TrialNumberInSzenario == 85 ||
                                                t.TrialNumberInSzenario == 98 || t.TrialNumberInSzenario == 171 ||
                                                t.TrialNumberInSzenario == 189 || t.TrialNumberInSzenario == 204)
                                            .ToList();

                                    var errorClampBaselineLeftHand =
                                        base1.Where(
                                            t =>
                                                t.TrialNumberInSzenario == 15 || t.TrialNumberInSzenario == 28 ||
                                                t.TrialNumberInSzenario == 52 || t.TrialNumberInSzenario == 115 ||
                                                t.TrialNumberInSzenario == 130 || t.TrialNumberInSzenario == 145)
                                            .ToList();
                                    errorClampBaselineLeftHand.AddRange(
                                        base2b.Where(t => t.TrialNumberInSzenario >= 7 && t.TrialNumberInSzenario <= 12));

                                    var errorClampBaselineRightHand =
                                        base1.Where(
                                            t =>
                                                t.TrialNumberInSzenario == 69 || t.TrialNumberInSzenario == 82 ||
                                                t.TrialNumberInSzenario == 106 || t.TrialNumberInSzenario == 169 ||
                                                t.TrialNumberInSzenario == 184 || t.TrialNumberInSzenario == 199)
                                            .ToList();
                                    errorClampBaselineRightHand.AddRange(
                                        base2a.Where(t => t.TrialNumberInSzenario >= 7 && t.TrialNumberInSzenario <= 12));

                                    var nullFieldBaselineLeftHand =
                                        base1.Where(
                                            t => t.TrialNumberInSzenario >= 157 && t.TrialNumberInSzenario <= 162)
                                            .ToList();
                                    nullFieldBaselineLeftHand.AddRange(
                                        base2b.Where(t => t.TrialNumberInSzenario >= 1 && t.TrialNumberInSzenario <= 6));

                                    var nullFieldBaselineRightHand =
                                        base1.Where(
                                            t => t.TrialNumberInSzenario >= 211 && t.TrialNumberInSzenario <= 216)
                                            .ToList();
                                    nullFieldBaselineRightHand.AddRange(
                                        base2a.Where(t => t.TrialNumberInSzenario >= 1 && t.TrialNumberInSzenario <= 6));

                                    if (
                                        forceFieldCatchTrialBaselineLeftHand.All(
                                            t =>
                                                t.TrialType == Trial.TrialTypeEnum.StandardTrial &&
                                                t.ForceFieldType == Trial.ForceFieldTypeEnum.ForceFieldCW &&
                                                t.Handedness == Trial.HandednessEnum.LeftHand) &&
                                        forceFieldCatchTrialBaselineRightHand.All(
                                            t =>
                                                t.TrialType == Trial.TrialTypeEnum.StandardTrial &&
                                                t.ForceFieldType == Trial.ForceFieldTypeEnum.ForceFieldCW &&
                                                t.Handedness == Trial.HandednessEnum.RightHand) &&
                                        errorClampBaselineLeftHand.All(
                                            t =>
                                                t.TrialType == Trial.TrialTypeEnum.ErrorClampTrial &&
                                                t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField &&
                                                t.Handedness == Trial.HandednessEnum.LeftHand) &&
                                        errorClampBaselineRightHand.All(
                                            t =>
                                                t.TrialType == Trial.TrialTypeEnum.ErrorClampTrial &&
                                                t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField &&
                                                t.Handedness == Trial.HandednessEnum.RightHand) &&
                                        nullFieldBaselineLeftHand.All(
                                            t =>
                                                t.TrialType == Trial.TrialTypeEnum.StandardTrial &&
                                                t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField &&
                                                t.Handedness == Trial.HandednessEnum.LeftHand) &&
                                        nullFieldBaselineRightHand.All(
                                            t =>
                                                t.TrialType == Trial.TrialTypeEnum.StandardTrial &&
                                                t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField &&
                                                t.Handedness == Trial.HandednessEnum.RightHand))
                                    {
                                        baselinesContainer.AddRange(
                                            doBaselineCalculation(forceFieldCatchTrialBaselineLeftHand));
                                        baselinesContainer.AddRange(
                                            doBaselineCalculation(forceFieldCatchTrialBaselineRightHand));
                                        baselinesContainer.AddRange(doBaselineCalculation(errorClampBaselineLeftHand));
                                        baselinesContainer.AddRange(doBaselineCalculation(errorClampBaselineRightHand));
                                        baselinesContainer.AddRange(doBaselineCalculation(nullFieldBaselineLeftHand));
                                        baselinesContainer.AddRange(doBaselineCalculation(nullFieldBaselineRightHand));
                                    }
                                    else
                                    {
                                        _myManipAnalysisGui.WriteToLogBox(
                                            "Error calculating Baseline. Incorrect TrialTypes. " + study + " / " + group +
                                            " / " + subject);
                                    }
                                }

                                foreach (var subject in subjectsRL)
                                {
                                    var turnBase1 =
                                        _myDatabaseWrapper.GetTurns(study, group, "RL_Base1", subject).ElementAt(0);
                                    var turnBase2a =
                                        _myDatabaseWrapper.GetTurns(study, group, "RL_Base2a", subject).ElementAt(0);
                                    var turnBase2b =
                                        _myDatabaseWrapper.GetTurns(study, group, "RL_Base2b", subject).ElementAt(0);

                                    var base1 =
                                        _myDatabaseWrapper.GetTrials(study, group, "RL_Base1", subject, turnBase1,
                                            Enumerable.Range(1, 216), baselineFields).ToList();
                                    var base2a =
                                        _myDatabaseWrapper.GetTrials(study, group, "RL_Base2a", subject, turnBase2a,
                                            Enumerable.Range(1, 12), baselineFields).ToList();
                                    var base2b =
                                        _myDatabaseWrapper.GetTrials(study, group, "RL_Base2b", subject, turnBase2b,
                                            Enumerable.Range(1, 12), baselineFields).ToList();

                                    base1.ForEach(
                                        t =>
                                            t.PositionNormalized =
                                                Gzip<List<PositionContainer>>.DeCompress(t.ZippedPositionNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    base1.ForEach(
                                        t =>
                                            t.VelocityNormalized =
                                                Gzip<List<VelocityContainer>>.DeCompress(t.ZippedVelocityNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    base1.ForEach(
                                        t =>
                                            t.MeasuredForcesNormalized =
                                                Gzip<List<ForceContainer>>.DeCompress(t.ZippedMeasuredForcesNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    base1.ForEach(
                                        t =>
                                            t.MomentForcesNormalized =
                                                Gzip<List<ForceContainer>>.DeCompress(t.ZippedMomentForcesNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());

                                    base2a.ForEach(
                                        t =>
                                            t.PositionNormalized =
                                                Gzip<List<PositionContainer>>.DeCompress(t.ZippedPositionNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    base2a.ForEach(
                                        t =>
                                            t.VelocityNormalized =
                                                Gzip<List<VelocityContainer>>.DeCompress(t.ZippedVelocityNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    base2a.ForEach(
                                        t =>
                                            t.MeasuredForcesNormalized =
                                                Gzip<List<ForceContainer>>.DeCompress(t.ZippedMeasuredForcesNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    base2a.ForEach(
                                        t =>
                                            t.MomentForcesNormalized =
                                                Gzip<List<ForceContainer>>.DeCompress(t.ZippedMomentForcesNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());

                                    base2b.ForEach(
                                        t =>
                                            t.PositionNormalized =
                                                Gzip<List<PositionContainer>>.DeCompress(t.ZippedPositionNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    base2b.ForEach(
                                        t =>
                                            t.VelocityNormalized =
                                                Gzip<List<VelocityContainer>>.DeCompress(t.ZippedVelocityNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    base2b.ForEach(
                                        t =>
                                            t.MeasuredForcesNormalized =
                                                Gzip<List<ForceContainer>>.DeCompress(t.ZippedMeasuredForcesNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());
                                    base2b.ForEach(
                                        t =>
                                            t.MomentForcesNormalized =
                                                Gzip<List<ForceContainer>>.DeCompress(t.ZippedMomentForcesNormalized)
                                                    .OrderBy(u => u.TimeStamp)
                                                    .ToList());

                                    var forceFieldCatchTrialBaselineLeftHand =
                                        base1.Where(
                                            t =>
                                                t.TrialNumberInSzenario == 72 || t.TrialNumberInSzenario == 85 ||
                                                t.TrialNumberInSzenario == 98 || t.TrialNumberInSzenario == 171 ||
                                                t.TrialNumberInSzenario == 189 || t.TrialNumberInSzenario == 204)
                                            .ToList();

                                    var forceFieldCatchTrialBaselineRightHand =
                                        base1.Where(
                                            t =>
                                                t.TrialNumberInSzenario == 18 || t.TrialNumberInSzenario == 31 ||
                                                t.TrialNumberInSzenario == 44 || t.TrialNumberInSzenario == 117 ||
                                                t.TrialNumberInSzenario == 135 || t.TrialNumberInSzenario == 150)
                                            .ToList();

                                    var errorClampBaselineLeftHand =
                                        base1.Where(
                                            t =>
                                                t.TrialNumberInSzenario == 69 || t.TrialNumberInSzenario == 82 ||
                                                t.TrialNumberInSzenario == 106 || t.TrialNumberInSzenario == 169 ||
                                                t.TrialNumberInSzenario == 184 || t.TrialNumberInSzenario == 199)
                                            .ToList();
                                    errorClampBaselineLeftHand.AddRange(
                                        base2a.Where(t => t.TrialNumberInSzenario >= 7 && t.TrialNumberInSzenario <= 12));

                                    var errorClampBaselineRightHand =
                                        base1.Where(
                                            t =>
                                                t.TrialNumberInSzenario == 15 || t.TrialNumberInSzenario == 28 ||
                                                t.TrialNumberInSzenario == 52 || t.TrialNumberInSzenario == 115 ||
                                                t.TrialNumberInSzenario == 130 || t.TrialNumberInSzenario == 145)
                                            .ToList();
                                    errorClampBaselineRightHand.AddRange(
                                        base2b.Where(t => t.TrialNumberInSzenario >= 7 && t.TrialNumberInSzenario <= 12));

                                    var nullFieldBaselineLeftHand =
                                        base1.Where(
                                            t => t.TrialNumberInSzenario >= 211 && t.TrialNumberInSzenario <= 216)
                                            .ToList();
                                    nullFieldBaselineLeftHand.AddRange(
                                        base2a.Where(t => t.TrialNumberInSzenario >= 1 && t.TrialNumberInSzenario <= 6));

                                    var nullFieldBaselineRightHand =
                                        base1.Where(
                                            t => t.TrialNumberInSzenario >= 157 && t.TrialNumberInSzenario <= 162)
                                            .ToList();
                                    nullFieldBaselineRightHand.AddRange(
                                        base2b.Where(t => t.TrialNumberInSzenario >= 1 && t.TrialNumberInSzenario <= 6));

                                    if (
                                        forceFieldCatchTrialBaselineLeftHand.All(
                                            t =>
                                                t.TrialType == Trial.TrialTypeEnum.StandardTrial &&
                                                t.ForceFieldType == Trial.ForceFieldTypeEnum.ForceFieldCW &&
                                                t.Handedness == Trial.HandednessEnum.LeftHand) &&
                                        forceFieldCatchTrialBaselineRightHand.All(
                                            t =>
                                                t.TrialType == Trial.TrialTypeEnum.StandardTrial &&
                                                t.ForceFieldType == Trial.ForceFieldTypeEnum.ForceFieldCW &&
                                                t.Handedness == Trial.HandednessEnum.RightHand) &&
                                        errorClampBaselineLeftHand.All(
                                            t =>
                                                t.TrialType == Trial.TrialTypeEnum.ErrorClampTrial &&
                                                t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField &&
                                                t.Handedness == Trial.HandednessEnum.LeftHand) &&
                                        errorClampBaselineRightHand.All(
                                            t =>
                                                t.TrialType == Trial.TrialTypeEnum.ErrorClampTrial &&
                                                t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField &&
                                                t.Handedness == Trial.HandednessEnum.RightHand) &&
                                        nullFieldBaselineLeftHand.All(
                                            t =>
                                                t.TrialType == Trial.TrialTypeEnum.StandardTrial &&
                                                t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField &&
                                                t.Handedness == Trial.HandednessEnum.LeftHand) &&
                                        nullFieldBaselineRightHand.All(
                                            t =>
                                                t.TrialType == Trial.TrialTypeEnum.StandardTrial &&
                                                t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField &&
                                                t.Handedness == Trial.HandednessEnum.RightHand))
                                    {
                                        baselinesContainer.AddRange(
                                            doBaselineCalculation(forceFieldCatchTrialBaselineLeftHand));
                                        baselinesContainer.AddRange(
                                            doBaselineCalculation(forceFieldCatchTrialBaselineRightHand));
                                        baselinesContainer.AddRange(doBaselineCalculation(errorClampBaselineLeftHand));
                                        baselinesContainer.AddRange(doBaselineCalculation(errorClampBaselineRightHand));
                                        baselinesContainer.AddRange(doBaselineCalculation(nullFieldBaselineLeftHand));
                                        baselinesContainer.AddRange(doBaselineCalculation(nullFieldBaselineRightHand));
                                    }
                                    else
                                    {
                                        _myManipAnalysisGui.WriteToLogBox(
                                            "Error calculating Baseline. Incorrect TrialTypes. " + study + " / " + group +
                                            " / " + subject);
                                    }
                                }
                            }
                        }
                    }

                    if (baselinesContainer.Any())
                    {
                        CompressBaselineData(baselinesContainer);
                        _myDatabaseWrapper.DropBaselines();
                        _myDatabaseWrapper.Insert(baselinesContainer);
                    }
                }
                catch (Exception
                    ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                }
                _myManipAnalysisGui.SetProgressBarValue(0);
                _myManipAnalysisGui.WriteProgressInfo("Ready");
                _myManipAnalysisGui.EnableTabPages(true);

                TaskManager.Remove(Task.CurrentId);
            }));
        }

        /// <summary>
        /// given a list of trials, this separates the trials into distinct targetNumbers and adds a new baseline for each targetNumber
        /// (by unzipping the trials, and doing some weird calculations and stuff?.
        /// Then returns the list of baselines
        /// </summary>
        /// <param name="inputTrials"></param>
        /// <returns></returns>
        private List<Baseline> doBaselineCalculation(List<Trial> inputTrials)
        {
            var baselines = new List<Baseline>();

            foreach (var targetCounter in inputTrials.Select(t => t.Target.Number).Distinct())
            {
                var baselineTrials = inputTrials.Where(t => t.Target.Number == targetCounter).ToList();

                var measuredForcesX =
                    baselineTrials.Select(t => t.MeasuredForcesNormalized.Select(u => u.X).ToArray()).ToList();
                var measuredForcesY =
                    baselineTrials.Select(t => t.MeasuredForcesNormalized.Select(u => u.Y).ToArray()).ToList();
                var measuredForcesZ =
                    baselineTrials.Select(t => t.MeasuredForcesNormalized.Select(u => u.Z).ToArray()).ToList();

                List<double[]> nominalForcesX = null;
                List<double[]> nominalForcesY = null;
                List<double[]> nominalForcesZ = null;
                if (baselineTrials[0].NominalForcesNormalized != null)
                {
                    nominalForcesX =
                        baselineTrials.Select(t => t.NominalForcesNormalized.Select(u => u.X).ToArray()).ToList();
                    nominalForcesY =
                        baselineTrials.Select(t => t.NominalForcesNormalized.Select(u => u.Y).ToArray()).ToList();
                    nominalForcesZ =
                        baselineTrials.Select(t => t.NominalForcesNormalized.Select(u => u.Z).ToArray()).ToList();
                }

                var momentForcesX =
                    baselineTrials.Select(t => t.MomentForcesNormalized.Select(u => u.X).ToArray()).ToList();
                var momentForcesY =
                    baselineTrials.Select(t => t.MomentForcesNormalized.Select(u => u.Y).ToArray()).ToList();
                var momentForcesZ =
                    baselineTrials.Select(t => t.MomentForcesNormalized.Select(u => u.Z).ToArray()).ToList();

                var positionX = baselineTrials.Select(t => t.PositionNormalized.Select(u => u.X).ToArray()).ToList();
                var positionY = baselineTrials.Select(t => t.PositionNormalized.Select(u => u.Y).ToArray()).ToList();
                var positionZ = baselineTrials.Select(t => t.PositionNormalized.Select(u => u.Z).ToArray()).ToList();

                var velocityX = baselineTrials.Select(t => t.VelocityNormalized.Select(u => u.X).ToArray()).ToList();
                var velocityY = baselineTrials.Select(t => t.VelocityNormalized.Select(u => u.Y).ToArray()).ToList();
                var velocityZ = baselineTrials.Select(t => t.VelocityNormalized.Select(u => u.Z).ToArray()).ToList();


                var tempBaseline = new Baseline();
                tempBaseline.Group = baselineTrials[0].Group;
                tempBaseline.Study = baselineTrials[0].Study;
                tempBaseline.MeasureFile = baselineTrials[0].MeasureFile;
                tempBaseline.Subject = baselineTrials[0].Subject;
                tempBaseline.Szenario = baselineTrials[0].Szenario;
                tempBaseline.Target = baselineTrials[0].Target;
                tempBaseline.Origin = baselineTrials[0].Origin;
                tempBaseline.TrialType = baselineTrials[0].TrialType;
                tempBaseline.ForceFieldType = baselineTrials[0].ForceFieldType;
                tempBaseline.Handedness = baselineTrials[0].Handedness;

                tempBaseline.MeasuredForces = new List<ForceContainer>();
                if (baselineTrials[0].NominalForcesNormalized != null)
                {
                    tempBaseline.NominalForces = new List<ForceContainer>();
                }
                tempBaseline.MomentForces = new List<ForceContainer>();
                tempBaseline.Position = new List<PositionContainer>();
                tempBaseline.Velocity = new List<VelocityContainer>();

                var frameCount = measuredForcesX[0].Length;
                var baselineTrialCount = baselineTrials.Count;

                var tempMeasuredForcesX = new double[frameCount];
                var tempMeasuredForcesY = new double[frameCount];
                var tempMeasuredForcesZ = new double[frameCount];

                double[] tempNominalForcesX = null;
                double[] tempNominalForcesY = null;
                double[] tempNominalForcesZ = null;
                if (nominalForcesX != null)
                {
                    tempNominalForcesX = new double[frameCount];
                    tempNominalForcesY = new double[frameCount];
                    tempNominalForcesZ = new double[frameCount];
                }
                var tempMomentForcesX = new double[frameCount];
                var tempMomentForcesY = new double[frameCount];
                var tempMomentForcesZ = new double[frameCount];

                var tempPositionX = new double[frameCount];
                var tempPositionY = new double[frameCount];
                var tempPositionZ = new double[frameCount];

                var tempVelocityX = new double[frameCount];
                var tempVelocityY = new double[frameCount];
                var tempVelocityZ = new double[frameCount];

                var baselineTimeStamps = new DateTime[frameCount];
                for (var timeSample = 0; timeSample < frameCount; timeSample++)
                {
                    baselineTimeStamps[timeSample] = DateTime.Now;
                    //baselineTrials[0].MeasureFile.CreationTime;
                    baselineTimeStamps[timeSample] = baselineTimeStamps[timeSample].AddSeconds(1.0 / Convert.ToDouble(baselineTrials[0].NormalizedDataSampleRate) * Convert.ToDouble(timeSample));
                }

                for (var trialCounter = 0; trialCounter < baselineTrialCount; trialCounter++)
                {
                    for (var frameCounter = 0; frameCounter < frameCount; frameCounter++)
                    {
                        tempMeasuredForcesX[frameCounter] += measuredForcesX[trialCounter][frameCounter];
                        tempMeasuredForcesY[frameCounter] += measuredForcesY[trialCounter][frameCounter];
                        tempMeasuredForcesZ[frameCounter] += measuredForcesZ[trialCounter][frameCounter];

                        if (nominalForcesX != null)
                        {
                            tempNominalForcesX[frameCounter] += nominalForcesX[trialCounter][frameCounter];
                            tempNominalForcesY[frameCounter] += nominalForcesY[trialCounter][frameCounter];
                            tempNominalForcesZ[frameCounter] += nominalForcesZ[trialCounter][frameCounter];
                        }

                        tempMomentForcesX[frameCounter] += momentForcesX[trialCounter][frameCounter];
                        tempMomentForcesY[frameCounter] += momentForcesY[trialCounter][frameCounter];
                        tempMomentForcesZ[frameCounter] += momentForcesZ[trialCounter][frameCounter];

                        tempPositionX[frameCounter] += positionX[trialCounter][frameCounter];
                        tempPositionY[frameCounter] += positionY[trialCounter][frameCounter];
                        tempPositionZ[frameCounter] += positionZ[trialCounter][frameCounter];

                        tempVelocityX[frameCounter] += velocityX[trialCounter][frameCounter];
                        tempVelocityY[frameCounter] += velocityY[trialCounter][frameCounter];
                        tempVelocityZ[frameCounter] += velocityZ[trialCounter][frameCounter];
                    }
                }

                for (var frameCounter = 0; frameCounter < frameCount; frameCounter++)
                {
                    tempMeasuredForcesX[frameCounter] /= baselineTrialCount;
                    tempMeasuredForcesY[frameCounter] /= baselineTrialCount;
                    tempMeasuredForcesZ[frameCounter] /= baselineTrialCount;

                    if (nominalForcesX != null)
                    {
                        tempNominalForcesX[frameCounter] /= baselineTrialCount;
                        tempNominalForcesY[frameCounter] /= baselineTrialCount;
                        tempNominalForcesZ[frameCounter] /= baselineTrialCount;
                    }

                    tempMomentForcesX[frameCounter] /= baselineTrialCount;
                    tempMomentForcesY[frameCounter] /= baselineTrialCount;
                    tempMomentForcesZ[frameCounter] /= baselineTrialCount;

                    tempPositionX[frameCounter] /= baselineTrialCount;
                    tempPositionY[frameCounter] /= baselineTrialCount;
                    tempPositionZ[frameCounter] /= baselineTrialCount;

                    tempVelocityX[frameCounter] /= baselineTrialCount;
                    tempVelocityY[frameCounter] /= baselineTrialCount;
                    tempVelocityZ[frameCounter] /= baselineTrialCount;

                    var measuredForceContainer = new ForceContainer();
                    ForceContainer nominalForceContainer = null;
                    if (nominalForcesX != null)
                    {
                        nominalForceContainer = new ForceContainer();
                    }
                    var momentForceContainer = new ForceContainer();
                    var positionContainer = new PositionContainer();
                    var velocityContainer = new VelocityContainer();

                    measuredForceContainer.TimeStamp = baselineTimeStamps[frameCounter];
                    measuredForceContainer.PositionStatus = -1;
                    measuredForceContainer.X = tempMeasuredForcesX[frameCounter];
                    measuredForceContainer.Y = tempMeasuredForcesY[frameCounter];
                    measuredForceContainer.Z = tempMeasuredForcesZ[frameCounter];

                    if (nominalForceContainer != null)
                    {
                        nominalForceContainer.TimeStamp = baselineTimeStamps[frameCounter];
                        nominalForceContainer.PositionStatus = -1;
                        nominalForceContainer.X = tempNominalForcesX[frameCounter];
                        nominalForceContainer.Y = tempNominalForcesY[frameCounter];
                        nominalForceContainer.Z = tempNominalForcesZ[frameCounter];
                    }

                    momentForceContainer.TimeStamp = baselineTimeStamps[frameCounter];
                    momentForceContainer.PositionStatus = -1;
                    momentForceContainer.X = tempMomentForcesX[frameCounter];
                    momentForceContainer.Y = tempMomentForcesY[frameCounter];
                    momentForceContainer.Z = tempMomentForcesZ[frameCounter];

                    positionContainer.TimeStamp = baselineTimeStamps[frameCounter];
                    positionContainer.PositionStatus = -1;
                    positionContainer.X = tempPositionX[frameCounter];
                    positionContainer.Y = tempPositionY[frameCounter];
                    positionContainer.Z = tempPositionZ[frameCounter];

                    velocityContainer.TimeStamp = baselineTimeStamps[frameCounter];
                    velocityContainer.PositionStatus = -1;
                    velocityContainer.X = tempVelocityX[frameCounter];
                    velocityContainer.Y = tempVelocityY[frameCounter];
                    velocityContainer.Z = tempVelocityZ[frameCounter];

                    tempBaseline.MeasuredForces.Add(measuredForceContainer);
                    if (nominalForceContainer != null)
                    {
                        tempBaseline.NominalForces.Add(nominalForceContainer);
                    }
                    tempBaseline.MomentForces.Add(momentForceContainer);
                    tempBaseline.Position.Add(positionContainer);
                    tempBaseline.Velocity.Add(velocityContainer);
                }

                baselines.Add(tempBaseline);
            }
            return baselines;
        }
        /// <summary>
        /// Helper function that converts the Trial.HandednessEnum from Left/RightHand to Left/RightHandVicon
        /// </summary>
        /// <param name="handedness">the original Handedness</param>
        /// <returns>the new handedness with vicon</returns>
        private Trial.HandednessEnum convertViconHandedness (Trial.HandednessEnum handedness)
        {
            switch (handedness)
            {
                case (Trial.HandednessEnum.LeftHand): return Trial.HandednessEnum.LeftHandVicon;
                case (Trial.HandednessEnum.RightHand): return Trial.HandednessEnum.RightHandVicon;
                case (Trial.HandednessEnum.RightHandVicon): return Trial.HandednessEnum.RightHandVicon;
                case (Trial.HandednessEnum.LeftHandVicon): return Trial.HandednessEnum.LeftHandVicon;
                default: return Trial.HandednessEnum.RightHandVicon;//Should never happen, and too lazy to let the enum implement nullable.
            }
        }

        public void CalculateStatistics()
        {
            TaskManager.PushBack(Task.Factory.StartNew(delegate
            {
                while (TaskManager.GetIndex(Task.CurrentId) != 0 & !TaskManager.Cancel)
                {
                    Thread.Sleep(100);
                }

                _myManipAnalysisGui.EnableTabPages(false);
                _myManipAnalysisGui.WriteProgressInfo("Calculating statistics...");
                var counter = 0;
                try
                {
                    var statisticFields = Builders<Trial>.Projection.Include(t1 => t1.ZippedVelocityNormalized);
                    statisticFields = statisticFields.Include(t2 => t2.ZippedPositionNormalized);
                    statisticFields = statisticFields.Include(t3 => t3.ZippedMeasuredForcesNormalized);
                    statisticFields = statisticFields.Include(t4 => t4.Study);
                    statisticFields = statisticFields.Include(t5 => t5.Group);
                    statisticFields = statisticFields.Include(t6 => t6.Szenario);
                    statisticFields = statisticFields.Include(t7 => t7.Subject);
                    statisticFields = statisticFields.Include(t8 => t8.Origin);
                    statisticFields = statisticFields.Include(t9 => t9.Target);
                    statisticFields = statisticFields.Include(t10 => t10.TrialNumberInSzenario);
                    statisticFields = statisticFields.Include(t11 => t11.TrialType);
                    statisticFields = statisticFields.Include(t12 => t12.ForceFieldType);
                    statisticFields = statisticFields.Include(t13 => t13.Handedness);
                    statisticFields = statisticFields.Include(t14 => t14.ForceFieldMatrix);

                    var trialList = _myDatabaseWrapper.GetTrialsWithoutStatistics(statisticFields).ToList();
                    var baselineBuffer = new List<Baseline>();
                    var cpuCount = Environment.ProcessorCount;

                    if (trialList.Count > 0)
                    {
                        var taskTrialListParts = new List<List<Trial>>();
                        var threadCount = 0;

                        if (trialList.Count > cpuCount)
                        {
                            for (var cpuCounter = 0; cpuCounter < cpuCount; cpuCounter++)
                            {
                                taskTrialListParts.Add(new List<Trial>());
                            }

                            var trialCounter = 0;
                            var listCounter = 0;
                            while (trialCounter < trialList.Count)
                            {
                                taskTrialListParts[listCounter].Add(trialList[trialCounter]);
                                trialCounter++;
                                listCounter++;
                                if (listCounter >= cpuCount)
                                {
                                    listCounter = 0;
                                }
                            }

                            threadCount = cpuCount;
                        }
                        else
                        {
                            taskTrialListParts.Add(trialList);
                            threadCount = 1;
                        }

                        var calculatingTasks = new List<Task>();

                        for (var i = 0; i < threadCount; i++)
                        {
                            var tempTaskTrialList = taskTrialListParts.ElementAt(i).ToList();

                            calculatingTasks.Add(Task.Factory.StartNew(delegate
                            {
                                var taskTrialList = tempTaskTrialList;
                                var taskMatlabWrapper = new MatlabWrapper(_myManipAnalysisGui,
                                    MatlabWrapper.MatlabInstanceType.Single);

                                try
                                {
                                    foreach (var trial in taskTrialList)
                                    {
                                        if (TaskManager.Cancel)
                                        {
                                            break;
                                        }
                                        while (TaskManager.Pause & !TaskManager.Cancel)
                                        {
                                            Thread.Sleep(100);
                                        }

                                        Baseline baseline = null;

                                        if (trial.Study == "Study 7")
                                        {
                                            if (trial.TrialType == Trial.TrialTypeEnum.ErrorClampTrial)
                                            {
                                                baseline =
                                                    baselineBuffer.Find(
                                                        t =>
                                                            t.Study == trial.Study && t.Group == trial.Group &&
                                                            t.Subject == trial.Subject &&
                                                            t.Target.Number == trial.Target.Number &&
                                                            t.TrialType == trial.TrialType &&
                                                            t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField &&
                                                            t.Handedness == trial.Handedness);
                                                if (baseline == null)
                                                {
                                                    baseline = _myDatabaseWrapper.GetBaseline(trial.Study, trial.Group,
                                                        trial.Subject, trial.Target.Number, trial.TrialType,
                                                        Trial.ForceFieldTypeEnum.NullField, trial.Handedness);
                                                    if (baseline != null)
                                                    {
                                                        lock (baselineBuffer)
                                                        {
                                                            baselineBuffer.Add(baseline);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                baseline =
                                                    baselineBuffer.Find(
                                                        t =>
                                                            t.Study == trial.Study && t.Group == trial.Group &&
                                                            t.Subject == trial.Subject &&
                                                            t.Target.Number == trial.Target.Number &&
                                                            t.TrialType == trial.TrialType &&
                                                            t.ForceFieldType == trial.ForceFieldType &&
                                                            t.Handedness == trial.Handedness);
                                                if (baseline == null)
                                                {
                                                    baseline = _myDatabaseWrapper.GetBaseline(trial.Study, trial.Group,
                                                        trial.Subject, trial.Target.Number, trial.TrialType,
                                                        trial.ForceFieldType, trial.Handedness);
                                                    if (baseline != null)
                                                    {
                                                        lock (baselineBuffer)
                                                        {
                                                            baselineBuffer.Add(baseline);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else if (trial.Study == "Study 08")
                                        {
                                            if (trial.TrialType == Trial.TrialTypeEnum.ErrorClampTrial)
                                            {
                                                baseline =
                                                    baselineBuffer.Find(
                                                        t =>
                                                            t.Study == trial.Study && t.Group == trial.Group &&
                                                            t.Subject == trial.Subject &&
                                                            t.Target.Number == trial.Target.Number &&
                                                            t.TrialType == trial.TrialType &&
                                                            t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField &&
                                                            t.Handedness == trial.Handedness);
                                                if (baseline == null)
                                                {
                                                    baseline = _myDatabaseWrapper.GetBaseline(trial.Study, trial.Group,
                                                        trial.Subject, trial.Target.Number, trial.TrialType,
                                                        Trial.ForceFieldTypeEnum.NullField, trial.Handedness);
                                                    if (baseline != null)
                                                    {
                                                        lock (baselineBuffer)
                                                        {
                                                            baselineBuffer.Add(baseline);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                baseline =
                                                    baselineBuffer.Find(
                                                        t =>
                                                            t.Study == trial.Study && t.Group == trial.Group &&
                                                            t.Subject == trial.Subject &&
                                                            t.Target.Number == trial.Target.Number &&
                                                            t.TrialType == trial.TrialType &&
                                                            t.ForceFieldType == trial.ForceFieldType &&
                                                            t.Handedness == trial.Handedness);
                                                if (baseline == null)
                                                {
                                                    baseline = _myDatabaseWrapper.GetBaseline(trial.Study, trial.Group,
                                                        trial.Subject, trial.Target.Number, trial.TrialType,
                                                        trial.ForceFieldType, trial.Handedness);
                                                    if (baseline != null)
                                                    {
                                                        lock (baselineBuffer)
                                                        {
                                                            baselineBuffer.Add(baseline);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else if (trial.Study == "Study 09")
                                        {
                                            if (trial.ForceFieldType == Trial.ForceFieldTypeEnum.ForceFieldCW)
                                            {
                                                int targetNumber = trial.Target.Number;

                                                /**
                                                In Szenario _11_CI_transfer_IW, the subjects perform inward movements instead of outwards.
                                                The Baseline to be used is from the opposite Target outward movement (18 => 4, 11 => 5, ...)
                                                RMSE Statistics will be way off because of this, all other parameters should be fine.
                                                **/
                                                if (trial.Szenario == _11_CI_transfer_IW.SzenarioName)
                                                {
                                                    targetNumber = ((trial.Target.Number >= 11) && (trial.Target.Number <= 18)) ? targetNumber - 10 : targetNumber;
                                                    targetNumber = (targetNumber + 4) % 8;
                                                    targetNumber = (targetNumber == 0) ? 8 : targetNumber;
                                                }

                                                baseline =
                                                    baselineBuffer.Find(
                                                        t =>
                                                            t.Study == trial.Study && t.Group == trial.Group &&
                                                            t.Subject == trial.Subject &&
                                                            t.Target.Number == targetNumber &&
                                                            t.TrialType == Trial.TrialTypeEnum.StandardTrial &&
                                                            t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField &&
                                                            t.Handedness == trial.Handedness);
                                                if (baseline == null)
                                                {
                                                    baseline = _myDatabaseWrapper.GetBaseline(trial.Study, trial.Group,
                                                        trial.Subject, targetNumber,
                                                        Trial.TrialTypeEnum.StandardTrial,
                                                        Trial.ForceFieldTypeEnum.NullField,
                                                        trial.Handedness);
                                                    if (baseline != null)
                                                    {
                                                        lock (baselineBuffer)
                                                        {
                                                            baselineBuffer.Add(baseline);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                baseline =
                                                    baselineBuffer.Find(
                                                        t =>
                                                            t.Study == trial.Study && t.Group == trial.Group &&
                                                            t.Subject == trial.Subject &&
                                                            t.Target.Number == trial.Target.Number &&
                                                            t.TrialType == trial.TrialType &&
                                                            t.ForceFieldType == trial.ForceFieldType &&
                                                            t.Handedness == trial.Handedness);
                                                if (baseline == null)
                                                {
                                                    baseline = _myDatabaseWrapper.GetBaseline(trial.Study, trial.Group,
                                                        trial.Subject, trial.Target.Number, trial.TrialType,
                                                        trial.ForceFieldType, trial.Handedness);
                                                    if (baseline != null)
                                                    {
                                                        lock (baselineBuffer)
                                                        {
                                                            baselineBuffer.Add(baseline);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else if (trial.Study == "Study 10_DAVOS")
                                        {
                                            //Insert Code!
                                            //Handedness is always set to the RightHand even for left hand trials, because
                                            //We dont have a good baseline for the left hand and the Statistics that require a baseline
                                            //are not used anyways according to Benjamin.
                                            if (trial.TrialType == Trial.TrialTypeEnum.StandardTrial && trial.ForceFieldType == Trial.ForceFieldTypeEnum.ForceFieldCW)
                                            {
                                                baseline =
                                                baselineBuffer.Find(
                                                    t =>
                                                        t.Study == trial.Study && t.Group == trial.Group &&
                                                        t.Subject == trial.Subject &&
                                                        t.Target.Number == trial.Target.Number &&
                                                        t.TrialType == Trial.TrialTypeEnum.StandardTrial &&
                                                        t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField &&
                                                        t.Handedness == Trial.HandednessEnum.RightHand);
                                                if (baseline == null)
                                                {
                                                    baseline = _myDatabaseWrapper.GetBaseline(trial.Study, trial.Group,
                                                        trial.Subject, trial.Target.Number,
                                                        Trial.TrialTypeEnum.StandardTrial,
                                                        Trial.ForceFieldTypeEnum.NullField, Trial.HandednessEnum.RightHand);
                                                    if (baseline != null)
                                                    {
                                                        lock (baselineBuffer)
                                                        {
                                                            baselineBuffer.Add(baseline);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        _myManipAnalysisGui.WriteToLogBox("Es konnte kein Trial gefunden werden für entsprechende Standard-Trials mit Forcefield CW.");
                                                    }
                                                }
                                            }
                                            else if (trial.TrialType == Trial.TrialTypeEnum.ErrorClampTrial)
                                            {
                                                baseline =
                                                baselineBuffer.Find(
                                                    t =>
                                                        t.Study == trial.Study && t.Group == trial.Group &&
                                                        t.Subject == trial.Subject &&
                                                        t.Target.Number == trial.Target.Number &&
                                                        t.TrialType == Trial.TrialTypeEnum.ErrorClampTrial &&
                                                        t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField &&
                                                        t.Handedness == Trial.HandednessEnum.RightHand);
                                                if (baseline == null)
                                                {
                                                    baseline = _myDatabaseWrapper.GetBaseline(trial.Study, trial.Group,
                                                        trial.Subject, trial.Target.Number,
                                                        Trial.TrialTypeEnum.ErrorClampTrial,
                                                        Trial.ForceFieldTypeEnum.NullField, Trial.HandednessEnum.RightHand);
                                                    if (baseline != null)
                                                    {
                                                        lock (baselineBuffer)
                                                        {
                                                            baselineBuffer.Add(baseline);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        _myManipAnalysisGui.WriteToLogBox("Es konnte kein Trial gefunden werden für entsprechende Errorclamptrials mit bel. Forcefield.");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                baseline =
                                                   baselineBuffer.Find(
                                                       t =>
                                                           t.Study == trial.Study && t.Group == trial.Group &&
                                                           t.Subject == trial.Subject &&
                                                           t.Target.Number == trial.Target.Number &&
                                                           t.TrialType == trial.TrialType &&
                                                           t.ForceFieldType == trial.ForceFieldType &&
                                                           t.Handedness == trial.Handedness);
                                                if (baseline == null)
                                                {
                                                    baseline = _myDatabaseWrapper.GetBaseline(trial.Study, trial.Group,
                                                        trial.Subject, trial.Target.Number, trial.TrialType,
                                                        trial.ForceFieldType, trial.Handedness);
                                                    if (baseline != null)
                                                    {
                                                        lock (baselineBuffer)
                                                        {
                                                            baselineBuffer.Add(baseline);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else if (trial.Study == "Study 10")
                                        {
                                            
                                            if (trial.TrialType == Trial.TrialTypeEnum.StandardTrial &&
                                                (trial.ForceFieldType == Trial.ForceFieldTypeEnum.ForceFieldCCW ||
                                                trial.ForceFieldType == Trial.ForceFieldTypeEnum.ForceFieldCW))
                                            {
                                                baseline =
                                                    baselineBuffer.Find(
                                                        t =>
                                                            t.Study == trial.Study && t.Group == trial.Group &&
                                                            t.Subject == trial.Subject &&
                                                            t.Target.Number == trial.Target.Number &&
                                                            t.TrialType == Trial.TrialTypeEnum.StandardTrial &&
                                                            t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField &&
                                                            t.Handedness == trial.Handedness);
                                                if (baseline == null)
                                                {
                                                    baseline = _myDatabaseWrapper.GetBaseline(trial.Study, trial.Group,
                                                        trial.Subject, trial.Target.Number,
                                                        Trial.TrialTypeEnum.StandardTrial,
                                                        Trial.ForceFieldTypeEnum.NullField, trial.Handedness);
                                                    if (baseline != null)
                                                    {
                                                        lock (baselineBuffer)
                                                        {
                                                            baselineBuffer.Add(baseline);
                                                        }
                                                    }
                                                }
                                            }
                                            else if (trial.TrialType == Trial.TrialTypeEnum.ErrorClampTrial)
                                            {
                                                baseline =
                                                    baselineBuffer.Find(
                                                        t =>
                                                            t.Study == trial.Study && t.Group == trial.Group &&
                                                            t.Subject == trial.Subject &&
                                                            t.Target.Number == trial.Target.Number &&
                                                            t.TrialType == Trial.TrialTypeEnum.ErrorClampTrial &&
                                                            t.ForceFieldType == Trial.ForceFieldTypeEnum.ForceFieldCW && // Für Import von 06_transfer_IW_B wird hier keine passende Baseline gefunden...
                                                            t.Handedness == trial.Handedness);
                                                if (baseline == null)
                                                {
                                                    baseline = _myDatabaseWrapper.GetBaseline(trial.Study, trial.Group,
                                                        trial.Subject, trial.Target.Number,
                                                        Trial.TrialTypeEnum.ErrorClampTrial,
                                                        Trial.ForceFieldTypeEnum.ForceFieldCW, trial.Handedness);
                                                    if (baseline != null)
                                                    {
                                                        lock (baselineBuffer)
                                                        {
                                                            baselineBuffer.Add(baseline);
                                                        }
                                                    }
                                                }
                                            }
                                        } else if (trial.Study == "Study 11")
                                        {
                                            if (trial.TrialType == Trial.TrialTypeEnum.StandardTrial
                                            && trial.ForceFieldType == Trial.ForceFieldTypeEnum.ForceFieldCCW)
                                            {
                                                baseline =
                                                    baselineBuffer.Find(
                                                        t =>
                                                            t.Study == trial.Study && t.Group == trial.Group &&
                                                            t.Subject == trial.Subject &&
                                                            t.Target.Number == trial.Target.Number &&
                                                            t.TrialType == Trial.TrialTypeEnum.StandardTrial &&
                                                            t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField &&
                                                            t.Handedness == trial.Handedness);
                                                if (baseline == null)
                                                {
                                                    baseline = _myDatabaseWrapper.GetBaseline(trial.Study, trial.Group,
                                                        trial.Subject, trial.Target.Number,
                                                        Trial.TrialTypeEnum.StandardTrial,
                                                        Trial.ForceFieldTypeEnum.NullField, trial.Handedness);
                                                    if (baseline != null)
                                                    {
                                                        lock (baselineBuffer)
                                                        {
                                                            baselineBuffer.Add(baseline);
                                                        }
                                                    }
                                                }
                                            }
                                            else if (trial.TrialType == Trial.TrialTypeEnum.ErrorClampTrial)
                                            {
                                                baseline =
                                                    baselineBuffer.Find(
                                                        t =>
                                                            t.Study == trial.Study && t.Group == trial.Group &&
                                                            t.Subject == trial.Subject &&
                                                            t.Target.Number == trial.Target.Number &&
                                                            t.TrialType == Trial.TrialTypeEnum.ErrorClampTrial &&
                                                            t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField &&
                                                            t.Handedness == trial.Handedness);
                                                if (baseline == null)
                                                {
                                                    baseline = _myDatabaseWrapper.GetBaseline(trial.Study, trial.Group,
                                                        trial.Subject, trial.Target.Number,
                                                        Trial.TrialTypeEnum.ErrorClampTrial,
                                                        Trial.ForceFieldTypeEnum.NullField, trial.Handedness);
                                                    if (baseline != null)
                                                    {
                                                        lock (baselineBuffer)
                                                        {
                                                            baselineBuffer.Add(baseline);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                baseline =
                                                    baselineBuffer.Find(
                                                        t =>
                                                            t.Study == trial.Study && t.Group == trial.Group &&
                                                            t.Subject == trial.Subject &&
                                                            t.Target.Number == trial.Target.Number &&
                                                            t.TrialType == trial.TrialType &&
                                                            t.ForceFieldType == trial.ForceFieldType &&
                                                            t.Handedness == trial.Handedness);
                                                if (baseline == null)
                                                {
                                                    baseline = _myDatabaseWrapper.GetBaseline(trial.Study, trial.Group,
                                                        trial.Subject, trial.Target.Number, trial.TrialType,
                                                        trial.ForceFieldType, trial.Handedness);
                                                    if (baseline != null)
                                                    {
                                                        lock (baselineBuffer)
                                                        {
                                                            baselineBuffer.Add(baseline);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else if(trial.Study == "Study_12_HEiKA")
                                        {
                                            if (trial.TrialType == Trial.TrialTypeEnum.ErrorClampTrial)
                                            {
                                                baseline =
                                                    baselineBuffer.Find(
                                                        t =>
                                                            t.Study == trial.Study && t.Group == trial.Group &&
                                                            t.Subject == trial.Subject &&
                                                            t.Target.Number == trial.Target.Number &&
                                                            t.TrialType == Trial.TrialTypeEnum.ErrorClampTrial && //Can't you also just write t.TrialType == trial.TrialType?
                                                            t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField &&
                                                            t.Handedness == convertViconHandedness(trial.Handedness)); //statt LeftHand oder RightHand muss LeftHandVicon bzw. RightHandVicon gewählt werden
                                                if (baseline == null)
                                                {
                                                    baseline = _myDatabaseWrapper.GetBaseline(trial.Study, trial.Group,
                                                        trial.Subject, trial.Target.Number,
                                                        Trial.TrialTypeEnum.ErrorClampTrial,
                                                        Trial.ForceFieldTypeEnum.NullField, convertViconHandedness(trial.Handedness));
                                                    if (baseline != null)
                                                    {
                                                        lock (baselineBuffer)
                                                        {
                                                            baselineBuffer.Add(baseline);
                                                        }
                                                    }
                                                }
                                            } else if ((trial.TrialType != Trial.TrialTypeEnum.ErrorClampTrial) &&
                                                (trial.Handedness == Trial.HandednessEnum.LeftHand ||
                                                trial.Handedness == Trial.HandednessEnum.RightHand))
                                            {
                                                baseline =
                                                    baselineBuffer.Find(
                                                        t =>
                                                            t.Study == trial.Study && t.Group == trial.Group &&
                                                            t.Subject == trial.Subject &&
                                                            t.Target.Number == trial.Target.Number &&
                                                            t.TrialType == trial.TrialType &&
                                                            t.ForceFieldType == trial.ForceFieldType &&
                                                            t.Handedness == convertViconHandedness(trial.Handedness));
                                                if (baseline == null)
                                                {
                                                    baseline = _myDatabaseWrapper.GetBaseline(trial.Study, trial.Group,
                                                        trial.Subject, trial.Target.Number,
                                                        trial.TrialType,
                                                        trial.ForceFieldType, convertViconHandedness(trial.Handedness));
                                                    if (baseline != null)
                                                    {
                                                        lock (baselineBuffer)
                                                        {
                                                            baselineBuffer.Add(baseline);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                baseline =
                                                    baselineBuffer.Find(
                                                        t =>
                                                            t.Study == trial.Study && t.Group == trial.Group &&
                                                            t.Subject == trial.Subject &&
                                                            t.Target.Number == trial.Target.Number &&
                                                            t.TrialType == trial.TrialType &&
                                                            t.ForceFieldType == trial.ForceFieldType &&
                                                            t.Handedness == trial.Handedness);
                                                if (baseline == null)
                                                {
                                                    baseline = _myDatabaseWrapper.GetBaseline(trial.Study, trial.Group,
                                                        trial.Subject, trial.Target.Number, trial.TrialType,
                                                        trial.ForceFieldType, trial.Handedness);
                                                    if (baseline != null)
                                                    {
                                                        lock (baselineBuffer)
                                                        {
                                                            baselineBuffer.Add(baseline);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            baseline =
                                                baselineBuffer.Find(
                                                    t =>
                                                        t.Study == trial.Study && t.Group == trial.Group &&
                                                        t.Subject == trial.Subject &&
                                                        t.Target.Number == trial.Target.Number &&
                                                        t.TrialType == trial.TrialType &&
                                                        t.ForceFieldType == trial.ForceFieldType &&
                                                        t.Handedness == trial.Handedness);
                                            if (baseline == null)
                                            {
                                                baseline = _myDatabaseWrapper.GetBaseline(trial.Study, trial.Group,
                                                    trial.Subject, trial.Target.Number, trial.TrialType,
                                                    trial.ForceFieldType, trial.Handedness);
                                                if (baseline != null)
                                                {
                                                    lock (baselineBuffer)
                                                    {
                                                        baselineBuffer.Add(baseline);
                                                    }
                                                }
                                            }
                                        }
                                        if (baseline != null)
                                        {
                                            baseline.Position =
                                                Gzip<List<PositionContainer>>.DeCompress(baseline.ZippedPosition)
                                                    .OrderBy(t => t.TimeStamp)
                                                    .ToList();
                                            baseline.Velocity =
                                                Gzip<List<VelocityContainer>>.DeCompress(baseline.ZippedVelocity)
                                                    .OrderBy(t => t.TimeStamp)
                                                    .ToList();
                                            baseline.MeasuredForces =
                                                Gzip<List<ForceContainer>>.DeCompress(baseline.ZippedMeasuredForces)
                                                    .OrderBy(t => t.TimeStamp)
                                                    .ToList();
                                            trial.PositionNormalized =
                                                Gzip<List<PositionContainer>>.DeCompress(trial.ZippedPositionNormalized)
                                                    .OrderBy(t => t.TimeStamp)
                                                    .ToList();
                                            trial.VelocityNormalized =
                                                Gzip<List<VelocityContainer>>.DeCompress(trial.ZippedVelocityNormalized)
                                                    .OrderBy(t => t.TimeStamp)
                                                    .ToList();
                                            trial.MeasuredForcesNormalized =
                                                Gzip<List<ForceContainer>>.DeCompress(
                                                    trial.ZippedMeasuredForcesNormalized)
                                                    .OrderBy(t => t.TimeStamp)
                                                    .ToList();

                                            taskMatlabWrapper.ClearWorkspace();

                                            DateTime startTimeStamp = trial.PositionNormalized.Select(u => u.TimeStamp).Min();
                                            try
                                            {
                                                var positionDataPoint180ms =
                                                    trial.PositionNormalized.Where(t => (t.TimeStamp - startTimeStamp).TotalMilliseconds >= 180).OrderBy(t => t.TimeStamp).First();
                                                taskMatlabWrapper.SetWorkspaceData("dataPoint180ms",
                                                   new[,] { { positionDataPoint180ms.X, positionDataPoint180ms.Y } });
                                            }

                                            //TODO: Raus, sollte nicht vorkommen, da diese Werte nicht mehr berechnet werden?
                                            catch (Exception ex)
                                            {
                                                _myManipAnalysisGui.WriteToLogBox("Could not determine positionDataPoint180ms for predicitionAndFeedbackAngle\n" + ex.ToString());
                                            }
                                            try
                                            {
                                                var positionDataPoint350ms =
                                                   trial.PositionNormalized.Where(t => (t.TimeStamp - startTimeStamp).TotalMilliseconds >= 350).OrderBy(t => t.TimeStamp).First();
                                                taskMatlabWrapper.SetWorkspaceData("dataPoint350ms",
                                                    new[,] { { positionDataPoint350ms.X, positionDataPoint350ms.Y } });
                                            }
                                            //TODO: Same here, should not happen, because we dont calculate these values anymore...
                                            catch (Exception ex)
                                            {
                                                _myManipAnalysisGui.WriteToLogBox("Could not determine positionDataPoint350ms for predicitionAndFeedbackAngle in Trial " + trial.TrialNumberInSzenario + " in Szenario " + trial.Szenario + "\n" + ex.ToString());
                                            }

                                            taskMatlabWrapper.SetWorkspaceData("startPoint",
                                                new[,] { { trial.Origin.XPos, trial.Origin.YPos } });
                                            taskMatlabWrapper.SetWorkspaceData("endPoint",
                                                new[,] { { trial.Target.XPos, trial.Target.YPos } });
                                            taskMatlabWrapper.SetWorkspaceData("forceFieldMatrix",
                                                trial.ForceFieldMatrix);
                                            taskMatlabWrapper.SetWorkspaceData("positionX",
                                                trial.PositionNormalized.Select(t => t.X).ToArray());
                                            taskMatlabWrapper.SetWorkspaceData("positionY",
                                                trial.PositionNormalized.Select(t => t.Y).ToArray());
                                            taskMatlabWrapper.SetWorkspaceData("velocityX",
                                                trial.VelocityNormalized.Select(t => t.X).ToArray());
                                            taskMatlabWrapper.SetWorkspaceData("velocityY",
                                                trial.VelocityNormalized.Select(t => t.Y).ToArray());
                                            taskMatlabWrapper.SetWorkspaceData("forceX",
                                                trial.MeasuredForcesNormalized.Select(t => t.X).ToArray());
                                            taskMatlabWrapper.SetWorkspaceData("forceY",
                                                trial.MeasuredForcesNormalized.Select(t => t.Y).ToArray());

                                            taskMatlabWrapper.SetWorkspaceData("baselinePositionX",
                                                baseline.Position.Select(t => t.X).ToArray());
                                            taskMatlabWrapper.SetWorkspaceData("baselinePositionY",
                                                baseline.Position.Select(t => t.Y).ToArray());
                                            taskMatlabWrapper.SetWorkspaceData("baselineVelocityX",
                                                baseline.Velocity.Select(t => t.X).ToArray());
                                            taskMatlabWrapper.SetWorkspaceData("baselineVelocityY",
                                                baseline.Velocity.Select(t => t.Y).ToArray());
                                            taskMatlabWrapper.SetWorkspaceData("baselineForceX",
                                                baseline.MeasuredForces.Select(t => t.X).ToArray());
                                            taskMatlabWrapper.SetWorkspaceData("baselineForceY",
                                                baseline.MeasuredForces.Select(t => t.Y).ToArray());

                                            // Matlab statistic calculations
                                            //VectorCorrelation raus
                                            taskMatlabWrapper.Execute(
                                                "vector_correlation = vectorCorrelation([velocityX velocityY], [baselineVelocityX baselineVelocityY]);");
                                            taskMatlabWrapper.Execute(
                                                "enclosed_area = enclosedArea(positionX, positionY);");
                                            taskMatlabWrapper.Execute(
                                                "length_abs = trajectLength(positionX', positionY');");
                                            //lengthRatio raus
                                            taskMatlabWrapper.Execute(
                                                "length_ratio = trajectLength(positionX', positionY') / trajectLength(baselinePositionX', baselinePositionY');");

                                            taskMatlabWrapper.Execute(
                                                "[distanceAbs, distance_sign_pd, distance_sign_ff] = distanceToCurve([positionX' positionY'], startPoint, endPoint, forceFieldMatrix);");
                                            taskMatlabWrapper.Execute("distanceSign = distanceAbs .* distance_sign_ff;");

                                            //MeanDistanceAbs raus
                                            taskMatlabWrapper.Execute("meanDistanceAbs = mean(distanceAbs);");


                                            taskMatlabWrapper.Execute("maxDistanceAbs = max(distanceAbs);");
                                            taskMatlabWrapper.Execute("[~, posDistanceSign] = max(abs(distanceSign));");
                                            taskMatlabWrapper.Execute("maxDistanceSign = distanceSign(posDistanceSign);");
                                            //RMSE raus
                                            taskMatlabWrapper.Execute("rmse = rootMeanSquareError([positionX positionY], [baselinePositionX baselinePositionY]);");
                                            //PredictionAngle and FeedbackAngle raus
                                            taskMatlabWrapper.Execute("[predictionAngle, feedbackAngle] = predicitionAndFeedbackAngle(startPoint, endPoint, dataPoint180ms, dataPoint350ms);");


                                            // Create StatisticContainer and fill it with calculated Matlab statistics
                                            var statisticContainer = new StatisticContainer();

                                            //TODO: Raus!
                                            statisticContainer.VelocityVectorCorrelation =
                                                taskMatlabWrapper.GetWorkspaceData("vector_correlation");

                                            statisticContainer.EnclosedArea =
                                                taskMatlabWrapper.GetWorkspaceData("enclosed_area");
                                            statisticContainer.AbsoluteTrajectoryLength =
                                                taskMatlabWrapper.GetWorkspaceData("length_abs");

                                            //TODO: Raus!
                                            statisticContainer.AbsoluteBaselineTrajectoryLengthRatio =
                                                taskMatlabWrapper.GetWorkspaceData("length_ratio");

                                            //TODO: Raus!
                                            statisticContainer.AbsoluteMeanPerpendicularDisplacement =
                                                taskMatlabWrapper.GetWorkspaceData("meanDistanceAbs");

                                            statisticContainer.AbsoluteMaximalPerpendicularDisplacement =
                                                taskMatlabWrapper.GetWorkspaceData("maxDistanceAbs");
                                            statisticContainer.SignedMaximalPerpendicularDisplacement =
                                                taskMatlabWrapper.GetWorkspaceData("maxDistanceSign");

                                            //TODO: Raus!
                                            statisticContainer.RMSE = taskMatlabWrapper.GetWorkspaceData("rmse");
                                            //TODO: Gesamter Block mit Pred und Feedb Angle raus!
                                            try
                                            {
                                                statisticContainer.PredictionAngle = taskMatlabWrapper.GetWorkspaceData("predictionAngle");
                                                statisticContainer.FeedbackAngle = taskMatlabWrapper.GetWorkspaceData("feedbackAngle");
                                            }
                                            catch
                                            {
                                                statisticContainer.PredictionAngle = 0;
                                                statisticContainer.FeedbackAngle = 0;
                                            }

                                            // Fill StatisticContainer with Abs and Sign PerpendicularDisplacement array
                                            double[,] absolutePerpendicularDisplacement =
                                                taskMatlabWrapper.GetWorkspaceData("distanceAbs");
                                            double[,] signedPerpendicularDisplacement =
                                                taskMatlabWrapper.GetWorkspaceData("distanceSign");

                                            for (var perpendicularDisplacementCounter = 0;
                                                perpendicularDisplacementCounter <
                                                trial.PositionNormalized.Select(t => t.TimeStamp).Count();
                                                perpendicularDisplacementCounter++)
                                            {
                                                var absolute = new PerpendicularDisplacementContainer();
                                                var signed = new PerpendicularDisplacementContainer();

                                                absolute.PerpendicularDisplacement =
                                                    absolutePerpendicularDisplacement[
                                                        perpendicularDisplacementCounter, 0];
                                                absolute.TimeStamp =
                                                    trial.PositionNormalized[perpendicularDisplacementCounter].TimeStamp;

                                                signed.PerpendicularDisplacement =
                                                    signedPerpendicularDisplacement[perpendicularDisplacementCounter, 0];
                                                signed.TimeStamp =
                                                    trial.PositionNormalized[perpendicularDisplacementCounter].TimeStamp;

                                                statisticContainer.AbsolutePerpendicularDisplacement.Add(absolute);
                                                statisticContainer.SignedPerpendicularDisplacement.Add(signed);
                                            }

                                            // Calculate and fill Absolute/Signed MaximalPerpendicularDisplacementVmax
                                            var maxVtime =
                                                trial.VelocityNormalized.First(
                                                    t =>
                                                        Math.Sqrt(Math.Pow(t.X, 2) + Math.Pow(t.Y, 2)) ==
                                                        trial.VelocityNormalized.Max(
                                                            u => Math.Sqrt(Math.Pow(u.X, 2) + Math.Pow(u.Y, 2))))
                                                    .TimeStamp;
                                            statisticContainer.AbsoluteMaximalPerpendicularDisplacementVmax =
                                                statisticContainer.AbsolutePerpendicularDisplacement.First(
                                                    t => t.TimeStamp == maxVtime).PerpendicularDisplacement;
                                            statisticContainer.SignedMaximalPerpendicularDisplacementVmax =
                                                statisticContainer.SignedPerpendicularDisplacement.First(
                                                    t => t.TimeStamp == maxVtime).PerpendicularDisplacement;

                                            // Calculate MidMovementForce
                                            //TODO: MidMovementForce raus!
                                            var vMaxCorridor =
                                                trial.VelocityNormalized.Where(
                                                    t => (t.TimeStamp - maxVtime).TotalMilliseconds < 70)
                                                    .Select(t => t.TimeStamp)
                                                    .ToList();
                                            //TODO: Raus!
                                            var perpendicularForcesMidMovementForce = new List<double>();
                                            var perpendicularForcesRawMidMovementForce = new List<double>();
                                            //TODO: Raus!
                                            var parallelForcesMidMovementForce = new List<double>();
                                            //TODO: Raus!
                                            var absoluteForcesMidMovementForce = new List<double>();

                                            //TODO: Raus!
                                            var perpendicularForcesForcefieldCompenstionFactor = new List<double>();

                                            var perpendicularForcesRawForcefieldCompenstionFactor = new List<double>();

                                            for (var dataPoint = 2;
                                                dataPoint <= trial.PositionNormalized.Count;
                                                dataPoint++)
                                            {
                                                taskMatlabWrapper.Execute(
                                                    "[forcePDRaw, forcePDsignRaw, ffSignRaw] = pdForceDirectionLineSegment([forceX(" +
                                                    (dataPoint - 1) + ") forceY(" + (dataPoint - 1) + ")], [positionX(" +
                                                    (dataPoint - 1) + ") positionY(" + (dataPoint - 1) +
                                                    ")], [positionX(" + dataPoint + ") positionY(" + dataPoint +
                                                    ")], forceFieldMatrix);");
                                                //TODO: forcePD, forcePDSign, ffsign Gegen Baseline --> raus!
                                                taskMatlabWrapper.Execute(
                                                    "[forcePD, forcePDsign, ffSign] = pdForceDirectionLineSegment([(forceX(" +
                                                    (dataPoint - 1) + ")-baselineForceX(" + (dataPoint - 1) +
                                                    ")) (forceY(" + (dataPoint - 1) + ")-baselineForceY(" +
                                                    (dataPoint - 1) + "))], [positionX(" + (dataPoint - 1) +
                                                    ") positionY(" + (dataPoint - 1) + ")], [positionX(" + dataPoint +
                                                    ") positionY(" + dataPoint + ")], forceFieldMatrix);");
                                                //TODO: ForceParaRaw raus? Wird eigentlich nie verwertet, nur berechnet...
                                                taskMatlabWrapper.Execute(
                                                    "forceParaRaw = paraForceLineSegment([forceX(" + (dataPoint - 1) +
                                                    ") forceY(" + (dataPoint - 1) + ")], [positionX(" + (dataPoint - 1) +
                                                    ") positionY(" + (dataPoint - 1) + ")], [positionX(" + dataPoint +
                                                    ") positionY(" + dataPoint + ")]);");
                                                //TODO: Gegen Baseline -_> raus
                                                taskMatlabWrapper.Execute("forcePara = paraForceLineSegment([(forceX(" +
                                                                          (dataPoint - 1) + ")-baselineForceX(" +
                                                                          (dataPoint - 1) + ")) (forceY(" +
                                                                          (dataPoint - 1) + ")-baselineForceY(" +
                                                                          (dataPoint - 1) + "))], [positionX(" +
                                                                          (dataPoint - 1) + ") positionY(" +
                                                                          (dataPoint - 1) + ")], [positionX(" +
                                                                          dataPoint + ") positionY(" + dataPoint +
                                                                          ")]);");

                                                taskMatlabWrapper.Execute(
                                                    "forcePDRaw = ffSignRaw * sqrt(forcePDRaw(1)^2 + forcePDRaw(2)^2);");
                                                //TODO: Raus!
                                                taskMatlabWrapper.Execute(
                                                    "forcePD = ffSign * sqrt(forcePD(1)^2 + forcePD(2)^2);");
                                                //TODO: Raus!
                                                taskMatlabWrapper.Execute(
                                                    "forceParaRaw = sqrt(forceParaRaw(1)^2 + forceParaRaw(2)^2);");
                                                //TODO: Raus!
                                                taskMatlabWrapper.Execute(
                                                    "forcePara = sqrt(forcePara(1)^2 + forcePara(2)^2);");

                                                //TODO: Raus!
                                                taskMatlabWrapper.Execute("absoluteForceRaw = sqrt(forceX(" +
                                                                          (dataPoint - 1) + ")^2 + forceY(" +
                                                                          (dataPoint - 1) + ")^2);");
                                                //TODO: Raus!
                                                taskMatlabWrapper.Execute("absoluteForce = sqrt((forceX(" +
                                                                          (dataPoint - 1) + ")-baselineForceX(" +
                                                                          (dataPoint - 1) + "))^2 + (forceY(" +
                                                                          (dataPoint - 1) + ")-baselineForceY(" +
                                                                          (dataPoint - 1) + "))^2);");

                                                //TODO: Raus!
                                                perpendicularForcesForcefieldCompenstionFactor.Add(
                                                    taskMatlabWrapper.GetWorkspaceData("forcePD"));
                                                //Keep it!
                                                perpendicularForcesRawForcefieldCompenstionFactor.Add(
                                                    taskMatlabWrapper.GetWorkspaceData("forcePDRaw"));

                                                if (
                                                    vMaxCorridor.Contains(
                                                        trial.PositionNormalized[dataPoint - 2].TimeStamp))
                                                {
                                                    //TODO: ForcePD nicht mehr benötigt! raus!
                                                    perpendicularForcesMidMovementForce.Add(
                                                        taskMatlabWrapper.GetWorkspaceData("forcePD"));
                                                    //ForcePDRaw behalten!
                                                    perpendicularForcesRawMidMovementForce.Add(
                                                        taskMatlabWrapper.GetWorkspaceData("forcePDRaw"));
                                                    //TODO: Raus!
                                                    parallelForcesMidMovementForce.Add(
                                                        taskMatlabWrapper.GetWorkspaceData("forcePara"));

                                                    //TODO: Raus!
                                                    absoluteForcesMidMovementForce.Add(
                                                        taskMatlabWrapper.GetWorkspaceData("absoluteForce"));
                                                }
                                            }

                                            //TODO: Raus!
                                            statisticContainer.PerpendicularMidMovementForce =
                                                perpendicularForcesMidMovementForce.Average();

                                            statisticContainer.PerpendicularMidMovementForceRaw =
                                                perpendicularForcesRawMidMovementForce.Average();
                                            //TODO: Raus!
                                            statisticContainer.ParallelMidMovementForce =
                                                parallelForcesMidMovementForce.Average();
                                            //TODO: Raus!
                                            statisticContainer.AbsoluteMidMovementForce =
                                                absoluteForcesMidMovementForce.Average();

                                            // Calculate ForcefieldCompenstionFactor
                                            //TODO: Raus!
                                            taskMatlabWrapper.SetWorkspaceData("forcePDArray",
                                                perpendicularForcesForcefieldCompenstionFactor.ToArray());
                                            //Keep
                                            taskMatlabWrapper.SetWorkspaceData("forcePDRawArray",
                                                perpendicularForcesRawForcefieldCompenstionFactor.ToArray());
                                            //TODO: Raus!
                                            taskMatlabWrapper.Execute(
                                                "forceCompFactor = forceCompensationFactor(forcePDArray, velocityX, velocityY, forceFieldMatrix);");
                                            //Keep
                                            taskMatlabWrapper.Execute(
                                                "forceCompFactorRaw = forceCompensationFactor(forcePDRawArray, velocityX, velocityY, forceFieldMatrix);");

                                            statisticContainer.ForcefieldCompenstionFactor =
                                                taskMatlabWrapper.GetWorkspaceData("forceCompFactor");
                                            statisticContainer.ForcefieldCompenstionFactorRaw =
                                                taskMatlabWrapper.GetWorkspaceData("forceCompFactorRaw");

                                            //Noch mal drüber schauen, wie man am besten die BaselineID entfernt, wenn sie überhaupt noch nötig ist.
                                            // Set Metadata and upload to Database
                                            trial.Statistics = statisticContainer;
                                            trial.BaselineObjectId = baseline.Id;

                                            CompressTrialData(new List<Trial> { trial });
                                            _myDatabaseWrapper.UpdateTrialStatisticsAndBaselineId(trial);

                                            _myManipAnalysisGui.SetProgressBarValue(100.0 / trialList.Count * ++counter);
                                        }
                                        else
                                        {
                                            if (trial.Szenario != "06_transfer_IW")
                                            {
                                                _myManipAnalysisGui.WriteToLogBox("No matching Baseline for Trial: " +
                                                                              trial.Study + " / " + trial.Group + " / " +
                                                                              trial.Subject.PId + " / " + trial.Szenario +
                                                                              " / Trial " + trial.TrialNumberInSzenario +
                                                                              " / " +
                                                                              Enum.GetName(
                                                                                  typeof(Trial.TrialTypeEnum),
                                                                                  trial.TrialType) + " / " +
                                                                              Enum.GetName(
                                                                                  typeof(Trial.ForceFieldTypeEnum),
                                                                                  trial.ForceFieldType) + " / " +
                                                                              Enum.GetName(
                                                                                  typeof(Trial.HandednessEnum),
                                                                                  trial.Handedness));
                                            }
                                            
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                                }
                                finally
                                {
                                    taskMatlabWrapper.Dispose();
                                }

                                lock (calculatingTasks)
                                {
                                    calculatingTasks.Remove(calculatingTasks.First(t => t.Id == Task.CurrentId));
                                }
                            }));
                        }

                        while (calculatingTasks.Any())
                        {
                            Thread.Sleep(500);
                        }
                    }
                    else
                    {
                        _myManipAnalysisGui.WriteToLogBox("Statistics already calculated!");
                    }
                }
                catch (Exception
                    ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                }

                _myManipAnalysisGui.SetProgressBarValue(0);
                _myManipAnalysisGui.WriteProgressInfo("Ready");
                _myManipAnalysisGui.EnableTabPages(true);

                TaskManager.Remove(Task.CurrentId);
            }));
        }

        public void PlotTrajectoryVelocityForce(IEnumerable<TrajectoryVelocityPlotContainer> selectedTrials,
            string meanIndividual, string trajectoryVelocityForce, IEnumerable<Trial.TrialTypeEnum> trialTypes,
            IEnumerable<Trial.ForceFieldTypeEnum> forceFields, IEnumerable<Trial.HandednessEnum> handedness,
            bool showForceVectors, bool showPdForceVectors)
        {
            TaskManager.PushBack(Task.Factory.StartNew(() =>
            {
                try
                {
                    _myManipAnalysisGui.WriteProgressInfo("Getting data...");
                    var selectedTrialsList = selectedTrials.ToList();
                    double sumOfAllTrials = selectedTrialsList.Sum(t => t.Trials.Count);
                    double processedTrialsCount = 0;
                    var study = selectedTrialsList.Select(t => t.Study).Distinct().First();

                    var fields = Builders<Trial>.Projection.Include(t => t.ForceFieldMatrix);
                    if (trajectoryVelocityForce == "Velocity - Normalized")
                    {
                        fields = fields.Include(t1 => t1.ZippedVelocityNormalized);
                        _myMatlabWrapper.CreateVelocityFigure("Velocity plot normalized", 101);
                    }
                    else if (trajectoryVelocityForce == "Velocity - Filtered")
                    {
                        fields = fields.Include(t1 => t1.ZippedVelocityFiltered);
                        _myMatlabWrapper.CreateFigure("Velocity plot filtered", "[Samples]", "Velocity [m/s]");
                    }
                    else if (trajectoryVelocityForce == "Trajectory - Normalized")
                    {
                        if (showForceVectors || showPdForceVectors)
                        {
                            fields = fields.Include(t1 => t1.ZippedPositionNormalized);
                            fields = fields.Include(t2 => t2.ZippedMeasuredForcesNormalized);
                            _myMatlabWrapper.CreateTrajectoryForceFigure("Trajectory plot normalized");
                        }
                        else
                        {
                            fields = fields.Include(t1 => t1.ZippedPositionNormalized);
                            _myMatlabWrapper.CreateTrajectoryFigure("Trajectory plot normalized");
                        }
                        if (study == "Study_12_HEiKA")
                        {
                            _myMatlabWrapper.DrawTargetsCenterOut3(0.003, 0.1, 0, 0);
                        }
                        else if (study == "Study 7")
                        {
                            _myMatlabWrapper.DrawTargetsCenterOut3(0.003, 0.1, 0, 0);
                        }
                        else if (study == "Study 08")
                        {
                            _myMatlabWrapper.DrawTargetsCenterOut3(0.003, 0.1, 0, 0);
                        }
                        else if (study == "Study 09")
                        {
                            _myMatlabWrapper.DrawTargetsCenterOut8(0.003, 0.1, 0, 0);
                        }
                        else if (study == "Study 10")
                        {
                            _myMatlabWrapper.DrawTargetsCenterOut8(0.003, 0.1, 0, 0);
                        }
                    }
                    else if (trajectoryVelocityForce == "Trajectory - Filtered")
                    {
                        if (showForceVectors || showPdForceVectors)
                        {
                            fields = fields.Include(t1 => t1.ZippedPositionFiltered);
                            fields = fields.Include(t2 => t2.ZippedMeasuredForcesFiltered);
                            _myMatlabWrapper.CreateTrajectoryForceFigure("Trajectory plot filtered");
                        }
                        else
                        {
                            fields = fields.Include(t1 => t1.ZippedPositionFiltered);
                            _myMatlabWrapper.CreateTrajectoryFigure("Trajectory plot filtered");
                        }

                        if (study == "Study_12_HEiKA")
                        {
                            _myMatlabWrapper.DrawTargetsCenterOut3(0.003, 0.1, 0, 0);
                        }
                        else if (study == "Study 7")
                        {
                            _myMatlabWrapper.DrawTargetsCenterOut3(0.003, 0.1, 0, 0);
                        }
                        else if (study == "Study 08")
                        {
                            _myMatlabWrapper.DrawTargetsCenterOut3(0.003, 0.1, 0, 0);
                        }
                        else if (study == "Study 09")
                        {
                            _myMatlabWrapper.DrawTargetsCenterOut8(0.003, 0.1, 0, 0);
                        }
                        else if (study == "Study 10")
                        {
                            _myMatlabWrapper.DrawTargetsCenterOut8(0.003, 0.1, 0, 0);
                        }
                    }
                    else if (trajectoryVelocityForce == "Trajectory - Raw")
                    {
                        if (showForceVectors || showPdForceVectors)
                        {
                            fields = fields.Include(t1 => t1.ZippedPositionRaw);
                            fields = fields.Include(t2 => t2.ZippedMeasuredForcesRaw);
                            _myMatlabWrapper.CreateTrajectoryForceFigure("Trajectory plot raw");
                        }
                        else
                        {
                            fields = fields.Include(t1 => t1.ZippedPositionRaw);
                            _myMatlabWrapper.CreateTrajectoryFigure("Trajectory plot raw");
                        }

                        if (study == "Study_12_HEiKA")
                        {
                            _myMatlabWrapper.DrawTargetsCenterOut3(0.003, 0.1, 0, 0);
                        }
                        else if (study == "Study 7")
                        {
                            _myMatlabWrapper.DrawTargetsCenterOut3(0.003, 0.1, 0, 0);
                        }
                        else if (study == "Study 08")
                        {
                            _myMatlabWrapper.DrawTargetsCenterOut3(0.003, 0.1, 0, 0);
                        }
                        else if (study == "Study 09")
                        {
                            _myMatlabWrapper.DrawTargetsCenterOut8(0.003, 0.1, 0, 0);
                        }
                        else if (study == "Study 10")
                        {
                            _myMatlabWrapper.DrawTargetsCenterOut8(0.003, 0.1, 0, 0);
                        }
                    }
                    else if (trajectoryVelocityForce == "Force - Normalized")
                    {
                        fields = fields.Include(t1 => t1.ZippedPositionNormalized);
                        fields = fields.Include(t2 => t2.ZippedMeasuredForcesNormalized);
                        _myMatlabWrapper.CreateForceFigure("Force plot normalized", "[Samples]", "Force [N]");
                    }
                    else if (trajectoryVelocityForce == "Force - Filtered")
                    {
                        fields = fields.Include(t1 => t1.ZippedPositionFiltered);
                        fields = fields.Include(t2 => t2.ZippedMeasuredForcesFiltered);
                        _myMatlabWrapper.CreateForceFigure("Force plot filtered", "[Samples]", "Force [N]");
                    }
                    else if (trajectoryVelocityForce == "Force - Raw")
                    {
                        fields = fields.Include(t1 => t1.ZippedPositionRaw);
                        fields = fields.Include(t2 => t2.ZippedMeasuredForcesRaw);
                        _myMatlabWrapper.CreateForceFigure("Force plot raw", "[Samples]", "Force [N]");
                    }

                    if (meanIndividual == "Individual")
                    {
                        foreach (var tempContainer in selectedTrialsList)
                        {
                            if (TaskManager.Cancel)
                            {
                                break;
                            }

                            var turnDateTime =
                                _myDatabaseWrapper.GetTurns(tempContainer.Study, tempContainer.Group,
                                    tempContainer.Szenario, tempContainer.Subject)
                                    .OrderBy(t => t)
                                    .ElementAt(tempContainer.Turn - 1);

                            var trialsArray =
                                _myDatabaseWrapper.GetTrials(tempContainer.Study, tempContainer.Group,
                                    tempContainer.Szenario, tempContainer.Subject, turnDateTime, tempContainer.Target,
                                    tempContainer.Trials, trialTypes, forceFields, handedness, fields).ToArray();

                            for (var trialsArrayCounter = 0; trialsArrayCounter < trialsArray.Length; trialsArrayCounter++)
                            {
                                if (TaskManager.Cancel)
                                {
                                    break;
                                }

                                _myManipAnalysisGui.SetProgressBarValue(100.0 / sumOfAllTrials * processedTrialsCount++);

                                if (trialsArray != null)
                                {
                                    if (trajectoryVelocityForce == "Velocity - Normalized")
                                    {
                                        trialsArray[trialsArrayCounter].VelocityNormalized =
                                            Gzip<List<VelocityContainer>>.DeCompress(
                                                trialsArray[trialsArrayCounter].ZippedVelocityNormalized)
                                                .OrderBy(t => t.TimeStamp)
                                                .ToList();
                                        _myMatlabWrapper.SetWorkspaceData("velocity",
                                            trialsArray[trialsArrayCounter].VelocityNormalized.Select(
                                                t => Math.Sqrt(Math.Pow(t.X, 2) + Math.Pow(t.Y, 2))).ToArray());
                                        _myMatlabWrapper.Plot("velocity", "black", 2);
                                    }
                                    else if (trajectoryVelocityForce == "Velocity - Filtered")
                                    {
                                        trialsArray[trialsArrayCounter].VelocityFiltered =
                                            Gzip<List<VelocityContainer>>.DeCompress(
                                                trialsArray[trialsArrayCounter].ZippedVelocityFiltered)
                                                .OrderBy(t => t.TimeStamp)
                                                .ToList();
                                        _myMatlabWrapper.SetWorkspaceData("velocity",
                                            trialsArray[trialsArrayCounter].VelocityFiltered.Select(
                                                t => Math.Sqrt(Math.Pow(t.X, 2) + Math.Pow(t.Y, 2))).ToArray());
                                        _myMatlabWrapper.Plot("velocity", "black", 2);
                                    }
                                    else if (trajectoryVelocityForce == "Trajectory - Normalized")
                                    {
                                        trialsArray[trialsArrayCounter].PositionNormalized =
                                            Gzip<List<PositionContainer>>.DeCompress(
                                                trialsArray[trialsArrayCounter].ZippedPositionNormalized)
                                                .OrderBy(t => t.TimeStamp)
                                                .ToList();

                                        _myMatlabWrapper.SetWorkspaceData("positionDataX",
                                            trialsArray[trialsArrayCounter].PositionNormalized.Select(t => t.X)
                                                .ToArray());
                                        _myMatlabWrapper.SetWorkspaceData("positionDataY",
                                            trialsArray[trialsArrayCounter].PositionNormalized.Select(t => t.Y)
                                                .ToArray());

                                        _myMatlabWrapper.Plot("positionDataX", "positionDataY", "black", 2);

                                        if (showForceVectors || showPdForceVectors)
                                        {
                                            trialsArray[trialsArrayCounter].MeasuredForcesNormalized =
                                                Gzip<List<ForceContainer>>.DeCompress(
                                                    trialsArray[trialsArrayCounter].ZippedMeasuredForcesNormalized)
                                                    .OrderBy(t => t.TimeStamp)
                                                    .ToList();
                                            for (var i = 2;
                                                i <= trialsArray[trialsArrayCounter].PositionNormalized.Count &
                                                !TaskManager.Pause;
                                                i++)
                                            {
                                                _myMatlabWrapper.SetWorkspaceData("vpos1",
                                                    new[]
                                                    {
                                                        trialsArray[trialsArrayCounter].PositionNormalized.Select(
                                                            t => t.X).ElementAt(i - 2),
                                                        trialsArray[trialsArrayCounter].PositionNormalized.Select(
                                                            t => t.Y).ElementAt(i - 2)
                                                    });

                                                _myMatlabWrapper.SetWorkspaceData("vpos2",
                                                    new[]
                                                    {
                                                        trialsArray[trialsArrayCounter].PositionNormalized.Select(
                                                            t => t.X).ElementAt(i - 1),
                                                        trialsArray[trialsArrayCounter].PositionNormalized.Select(
                                                            t => t.Y).ElementAt(i - 1)
                                                    });

                                                _myMatlabWrapper.SetWorkspaceData("vforce",
                                                    new[]
                                                    {
                                                        trialsArray[trialsArrayCounter].MeasuredForcesNormalized.Select(
                                                            t => t.X).ElementAt(i - 2)/100.0,
                                                        trialsArray[trialsArrayCounter].MeasuredForcesNormalized.Select(
                                                            t => t.Y).ElementAt(i - 2)/100.0
                                                    });

                                                _myMatlabWrapper.SetWorkspaceData("forceFieldMatrix",
                                                    trialsArray[trialsArrayCounter].ForceFieldMatrix);

                                                if (showForceVectors)
                                                {
                                                    _myMatlabWrapper.Execute(
                                                        "quiver(vpos2(1),vpos2(2),vforce(1),vforce(2),'Color','red');");
                                                }
                                                if (showPdForceVectors)
                                                {
                                                    _myMatlabWrapper.Execute(
                                                        "[fPD, fPDsign, fFFsign] = pdForceDirectionLineSegment([vforce(1,1) vforce(1,2)], [vpos1(1,1) vpos1(1,2)], [vpos2(1,1) vpos2(1,2)], forceFieldMatrix);");
                                                    _myMatlabWrapper.Execute(
                                                        "quiver(vpos2(1),vpos2(2),fPD(1),fPD(2),'Color','blue');");
                                                }
                                            }
                                        }
                                    }
                                    else if (trajectoryVelocityForce == "Trajectory - Filtered")
                                    {
                                        trialsArray[trialsArrayCounter].PositionFiltered =
                                            Gzip<List<PositionContainer>>.DeCompress(
                                                trialsArray[trialsArrayCounter].ZippedPositionFiltered)
                                                .OrderBy(t => t.TimeStamp)
                                                .ToList();

                                        _myMatlabWrapper.SetWorkspaceData("positionDataX",
                                            trialsArray[trialsArrayCounter].PositionFiltered.Select(t => t.X).ToArray());
                                        _myMatlabWrapper.SetWorkspaceData("positionDataY",
                                            trialsArray[trialsArrayCounter].PositionFiltered.Select(t => t.Y).ToArray());

                                        _myMatlabWrapper.Plot("positionDataX", "positionDataY", "black", 2);

                                        if (showForceVectors || showPdForceVectors)
                                        {
                                            trialsArray[trialsArrayCounter].MeasuredForcesFiltered =
                                                Gzip<List<ForceContainer>>.DeCompress(
                                                    trialsArray[trialsArrayCounter].ZippedMeasuredForcesFiltered)
                                                    .OrderBy(t => t.TimeStamp)
                                                    .ToList();
                                            for (var i = 2;
                                                i <= trialsArray[trialsArrayCounter].PositionFiltered.Count &
                                                !TaskManager.Pause;
                                                i++)
                                            {
                                                _myMatlabWrapper.SetWorkspaceData("vpos1",
                                                    new[]
                                                    {
                                                        trialsArray[trialsArrayCounter].PositionFiltered.Select(t => t.X)
                                                            .ElementAt(i - 2),
                                                        trialsArray[trialsArrayCounter].PositionFiltered.Select(t => t.Y)
                                                            .ElementAt(i - 2)
                                                    });

                                                _myMatlabWrapper.SetWorkspaceData("vpos2",
                                                    new[]
                                                    {
                                                        trialsArray[trialsArrayCounter].PositionFiltered.Select(t => t.X)
                                                            .ElementAt(i - 1),
                                                        trialsArray[trialsArrayCounter].PositionFiltered.Select(t => t.Y)
                                                            .ElementAt(i - 1)
                                                    });

                                                _myMatlabWrapper.SetWorkspaceData("vforce",
                                                    new[]
                                                    {
                                                        trialsArray[trialsArrayCounter].MeasuredForcesFiltered.Select(
                                                            t => t.X).ElementAt(i - 2)/100.0,
                                                        trialsArray[trialsArrayCounter].MeasuredForcesFiltered.Select(
                                                            t => t.Y).ElementAt(i - 2)/100.0
                                                    });

                                                _myMatlabWrapper.SetWorkspaceData("forceFieldMatrix",
                                                    trialsArray[trialsArrayCounter].ForceFieldMatrix);

                                                if (showForceVectors)
                                                {
                                                    _myMatlabWrapper.Execute(
                                                        "quiver3(vpos2(1),vpos2(2),vforce(1),vforce(2),'Color','red');");
                                                }
                                                if (showPdForceVectors)
                                                {
                                                    _myMatlabWrapper.Execute(
                                                        "[fPD, fPDsign, fFFsign] = pdForceDirectionLineSegment([vforce(1,1) vforce(1,2)], [vpos1(1,1) vpos1(1,2)], [vpos2(1,1) vpos2(1,2)], forceFieldMatrix);");
                                                    _myMatlabWrapper.Execute(
                                                        "quiver(vpos2(1),vpos2(2),fPD(1),fPD(2),'Color','blue');");
                                                }
                                            }
                                        }
                                    }
                                    else if (trajectoryVelocityForce == "Trajectory - Raw")
                                    {
                                        trialsArray[trialsArrayCounter].PositionRaw =
                                            Gzip<List<PositionContainer>>.DeCompress(
                                                trialsArray[trialsArrayCounter].ZippedPositionRaw)
                                                .OrderBy(t => t.TimeStamp)
                                                .ToList();

                                        _myMatlabWrapper.SetWorkspaceData("positionDataX",
                                            trialsArray[trialsArrayCounter].PositionRaw.Select(t => t.X).ToArray());
                                        _myMatlabWrapper.SetWorkspaceData("positionDataY",
                                            trialsArray[trialsArrayCounter].PositionRaw.Select(t => t.Y).ToArray());

                                        _myMatlabWrapper.Plot("positionDataX", "positionDataY", "black", 2);

                                        if (showForceVectors || showPdForceVectors)
                                        {
                                            trialsArray[trialsArrayCounter].MeasuredForcesRaw =
                                                Gzip<List<ForceContainer>>.DeCompress(
                                                    trialsArray[trialsArrayCounter].ZippedMeasuredForcesRaw)
                                                    .OrderBy(t => t.TimeStamp)
                                                    .ToList();
                                            for (var i = 2;
                                                i <= trialsArray[trialsArrayCounter].PositionRaw.Count &
                                                !TaskManager.Pause;
                                                i++)
                                            {
                                                _myMatlabWrapper.SetWorkspaceData("vpos1",
                                                    new[]
                                                    {
                                                        trialsArray[trialsArrayCounter].PositionRaw.Select(t => t.X)
                                                            .ElementAt(i - 2),
                                                        trialsArray[trialsArrayCounter].PositionRaw.Select(t => t.Y)
                                                            .ElementAt(i - 2)
                                                    });

                                                _myMatlabWrapper.SetWorkspaceData("vpos2",
                                                    new[]
                                                    {
                                                        trialsArray[trialsArrayCounter].PositionRaw.Select(t => t.X)
                                                            .ElementAt(i - 1),
                                                        trialsArray[trialsArrayCounter].PositionRaw.Select(t => t.Y)
                                                            .ElementAt(i - 1)
                                                    });

                                                _myMatlabWrapper.SetWorkspaceData("vforce",
                                                    new[]
                                                    {
                                                        trialsArray[trialsArrayCounter].MeasuredForcesRaw.Select(
                                                            t => t.X).ElementAt(i - 2)/100.0,
                                                        trialsArray[trialsArrayCounter].MeasuredForcesRaw.Select(
                                                            t => t.Y).ElementAt(i - 2)/100.0
                                                    });

                                                _myMatlabWrapper.SetWorkspaceData("forceFieldMatrix",
                                                    trialsArray[trialsArrayCounter].ForceFieldMatrix);

                                                if (showForceVectors)
                                                {
                                                    _myMatlabWrapper.Execute(
                                                        "quiver3(vpos2(1),vpos2(2),vforce(1),vforce(2),'Color','red');");
                                                }
                                                if (showPdForceVectors)
                                                {
                                                    _myMatlabWrapper.Execute(
                                                        "[fPD, fPDsign, fFFsign] = pdForceDirectionLineSegment([vforce(1,1) vforce(1,2)], [vpos1(1,1) vpos1(1,2)], [vpos2(1,1) vpos2(1,2)], forceFieldMatrix);");
                                                    _myMatlabWrapper.Execute(
                                                        "quiver(vpos2(1),vpos2(2),fPD(1),fPD(2),'Color','blue');");
                                                }
                                            }
                                        }
                                    }
                                    else if (trajectoryVelocityForce == "Force - Normalized")
                                    {
                                        trialsArray[trialsArrayCounter].PositionNormalized =
                                            Gzip<List<PositionContainer>>.DeCompress(
                                                trialsArray[trialsArrayCounter].ZippedPositionNormalized)
                                                .OrderBy(t => t.TimeStamp)
                                                .ToList();
                                        trialsArray[trialsArrayCounter].MeasuredForcesNormalized =
                                            Gzip<List<ForceContainer>>.DeCompress(
                                                trialsArray[trialsArrayCounter].ZippedMeasuredForcesNormalized)
                                                .OrderBy(t => t.TimeStamp)
                                                .ToList();

                                        _myMatlabWrapper.Execute("forcePDVector = zeros(1, " +
                                                                 (trialsArray[trialsArrayCounter].PositionNormalized
                                                                     .Count - 1) + ");");
                                        _myMatlabWrapper.Execute("forceParaVector = zeros(1, " +
                                                                 (trialsArray[trialsArrayCounter].PositionNormalized
                                                                     .Count - 1) + ");");
                                        _myMatlabWrapper.Execute("forceAbsVector = zeros(1, " +
                                                                 (trialsArray[trialsArrayCounter].PositionNormalized
                                                                     .Count - 1) + ");");

                                        for (var i = 2;
                                            i <= trialsArray[trialsArrayCounter].PositionNormalized.Count &
                                            !TaskManager.Pause;
                                            i++)
                                        {
                                            _myMatlabWrapper.SetWorkspaceData("vpos1",
                                                new[]
                                                {
                                                    trialsArray[trialsArrayCounter].PositionNormalized.Select(t => t.X)
                                                        .ElementAt(i - 2),
                                                    trialsArray[trialsArrayCounter].PositionNormalized.Select(t => t.Y)
                                                        .ElementAt(i - 2)
                                                });
                                            _myMatlabWrapper.SetWorkspaceData("vpos2",
                                                new[]
                                                {
                                                    trialsArray[trialsArrayCounter].PositionNormalized.Select(t => t.X)
                                                        .ElementAt(i - 1),
                                                    trialsArray[trialsArrayCounter].PositionNormalized.Select(t => t.Y)
                                                        .ElementAt(i - 1)
                                                });
                                            _myMatlabWrapper.SetWorkspaceData("vforce",
                                                new[]
                                                {
                                                    trialsArray[trialsArrayCounter].MeasuredForcesNormalized.Select(
                                                        t => t.X).ElementAt(i - 2),
                                                    trialsArray[trialsArrayCounter].MeasuredForcesNormalized.Select(
                                                        t => t.Y).ElementAt(i - 2)
                                                });
                                            _myMatlabWrapper.SetWorkspaceData("forceFieldMatrix",
                                                trialsArray[trialsArrayCounter].ForceFieldMatrix);

                                            _myMatlabWrapper.Execute(
                                                "[fPD, fPDsign, fFFsign] = pdForceDirectionLineSegment([vforce(1,1) vforce(1,2)], [vpos1(1,1) vpos1(1,2)], [vpos2(1,1) vpos2(1,2)], forceFieldMatrix);");
                                            _myMatlabWrapper.Execute(
                                                "[fPara, fParasign] = paraForceLineSegment([vforce(1,1) vforce(1,2)], [vpos1(1,1) vpos1(1,2)], [vpos2(1,1) vpos2(1,2)]);");

                                            _myMatlabWrapper.Execute("forcePDVector(" + (i - 1) +
                                                                     ") = sqrt(fPD(1)^2 + fPD(2)^2) * fFFsign;");
                                            _myMatlabWrapper.Execute("forceParaVector(" + (i - 1) +
                                                                     ") = sqrt(fPara(1)^2 + fPara(2)^2) * fParasign;");
                                            _myMatlabWrapper.Execute("forceAbsVector(" + (i - 1) +
                                                                     ") = sqrt(vforce(1,1)^2 + vforce(1,2)^2);");
                                        }

                                        _myMatlabWrapper.Plot("forcePDVector", "blue", 2);
                                        _myMatlabWrapper.Plot("forceParaVector", "red", 2);
                                        _myMatlabWrapper.Plot("forceAbsVector", "black", 2);
                                        _myMatlabWrapper.AddLegend("Force PD", "Force Para", "Force Abs");
                                    }
                                    else if (trajectoryVelocityForce == "Force - Filtered")
                                    {
                                        trialsArray[trialsArrayCounter].PositionFiltered =
                                            Gzip<List<PositionContainer>>.DeCompress(
                                                trialsArray[trialsArrayCounter].ZippedPositionFiltered)
                                                .OrderBy(t => t.TimeStamp)
                                                .ToList();
                                        trialsArray[trialsArrayCounter].MeasuredForcesFiltered =
                                            Gzip<List<ForceContainer>>.DeCompress(
                                                trialsArray[trialsArrayCounter].ZippedMeasuredForcesFiltered)
                                                .OrderBy(t => t.TimeStamp)
                                                .ToList();

                                        _myMatlabWrapper.Execute("forcePDVector = zeros(1, " +
                                                                 (trialsArray[trialsArrayCounter].PositionFiltered.Count -
                                                                  1) + ");");
                                        _myMatlabWrapper.Execute("forceParaVector = zeros(1, " +
                                                                 (trialsArray[trialsArrayCounter].PositionFiltered.Count -
                                                                  1) + ");");
                                        _myMatlabWrapper.Execute("forceAbsVector = zeros(1, " +
                                                                 (trialsArray[trialsArrayCounter].PositionFiltered.Count -
                                                                  1) + ");");

                                        for (var i = 2;
                                            i <= trialsArray[trialsArrayCounter].PositionFiltered.Count &
                                            !TaskManager.Pause;
                                            i++)
                                        {
                                            _myMatlabWrapper.SetWorkspaceData("vpos1",
                                                new[]
                                                {
                                                    trialsArray[trialsArrayCounter].PositionFiltered.Select(t => t.X)
                                                        .ElementAt(i - 2),
                                                    trialsArray[trialsArrayCounter].PositionFiltered.Select(t => t.Y)
                                                        .ElementAt(i - 2)
                                                });
                                            _myMatlabWrapper.SetWorkspaceData("vpos2",
                                                new[]
                                                {
                                                    trialsArray[trialsArrayCounter].PositionFiltered.Select(t => t.X)
                                                        .ElementAt(i - 1),
                                                    trialsArray[trialsArrayCounter].PositionFiltered.Select(t => t.Y)
                                                        .ElementAt(i - 1)
                                                });
                                            _myMatlabWrapper.SetWorkspaceData("vforce",
                                                new[]
                                                {
                                                    trialsArray[trialsArrayCounter].MeasuredForcesFiltered.Select(
                                                        t => t.X).ElementAt(i - 2),
                                                    trialsArray[trialsArrayCounter].MeasuredForcesFiltered.Select(
                                                        t => t.Y).ElementAt(i - 2)
                                                });
                                            _myMatlabWrapper.SetWorkspaceData("forceFieldMatrix",
                                                trialsArray[trialsArrayCounter].ForceFieldMatrix);

                                            _myMatlabWrapper.Execute(
                                                "[fPD, fPDsign, fFFsign] = pdForceDirectionLineSegment([vforce(1,1) vforce(1,2)], [vpos1(1,1) vpos1(1,2)], [vpos2(1,1) vpos2(1,2)], forceFieldMatrix);");
                                            _myMatlabWrapper.Execute(
                                                "[fPara, fParasign] = paraForceLineSegment([vforce(1,1) vforce(1,2)], [vpos1(1,1) vpos1(1,2)], [vpos2(1,1) vpos2(1,2)]);");

                                            _myMatlabWrapper.Execute("forcePDVector(" + (i - 1) +
                                                                     ") = sqrt(fPD(1)^2 + fPD(2)^2) * fFFsign;");
                                            _myMatlabWrapper.Execute("forceParaVector(" + (i - 1) +
                                                                     ") = sqrt(fPara(1)^2 + fPara(2)^2) * fParasign;");
                                            _myMatlabWrapper.Execute("forceAbsVector(" + (i - 1) +
                                                                     ") = sqrt(vforce(1,1)^2 + vforce(1,2)^2);");
                                        }

                                        _myMatlabWrapper.Plot("forcePDVector", "blue", 2);
                                        _myMatlabWrapper.Plot("forceParaVector", "red", 2);
                                        _myMatlabWrapper.Plot("forceAbsVector", "black", 2);
                                        _myMatlabWrapper.AddLegend("Force PD", "Force Para", "Force Abs");
                                    }
                                    else if (trajectoryVelocityForce == "Force - Raw")
                                    {
                                        trialsArray[trialsArrayCounter].PositionRaw =
                                            Gzip<List<PositionContainer>>.DeCompress(
                                                trialsArray[trialsArrayCounter].ZippedPositionRaw)
                                                .OrderBy(t => t.TimeStamp)
                                                .ToList();
                                        trialsArray[trialsArrayCounter].MeasuredForcesRaw =
                                            Gzip<List<ForceContainer>>.DeCompress(
                                                trialsArray[trialsArrayCounter].ZippedMeasuredForcesRaw)
                                                .OrderBy(t => t.TimeStamp)
                                                .ToList();

                                        _myMatlabWrapper.Execute("forcePDVector = zeros(1, " +
                                                                 (trialsArray[trialsArrayCounter].PositionRaw.Count - 1) +
                                                                 ");");
                                        _myMatlabWrapper.Execute("forceParaVector = zeros(1, " +
                                                                 (trialsArray[trialsArrayCounter].PositionRaw.Count - 1) +
                                                                 ");");
                                        _myMatlabWrapper.Execute("forceAbsVector = zeros(1, " +
                                                                 (trialsArray[trialsArrayCounter].PositionRaw.Count - 1) +
                                                                 ");");

                                        for (var i = 2;
                                            i <= trialsArray[trialsArrayCounter].PositionRaw.Count & !TaskManager.Pause;
                                            i++)
                                        {
                                            _myMatlabWrapper.SetWorkspaceData("vpos1",
                                                new[]
                                                {
                                                    trialsArray[trialsArrayCounter].PositionRaw.Select(t => t.X)
                                                        .ElementAt(i - 2),
                                                    trialsArray[trialsArrayCounter].PositionRaw.Select(t => t.Y)
                                                        .ElementAt(i - 2)
                                                });
                                            _myMatlabWrapper.SetWorkspaceData("vpos2",
                                                new[]
                                                {
                                                    trialsArray[trialsArrayCounter].PositionRaw.Select(t => t.X)
                                                        .ElementAt(i - 1),
                                                    trialsArray[trialsArrayCounter].PositionRaw.Select(t => t.Y)
                                                        .ElementAt(i - 1)
                                                });
                                            _myMatlabWrapper.SetWorkspaceData("vforce",
                                                new[]
                                                {
                                                    trialsArray[trialsArrayCounter].MeasuredForcesRaw.Select(t => t.X)
                                                        .ElementAt(i - 2),
                                                    trialsArray[trialsArrayCounter].MeasuredForcesRaw.Select(t => t.Y)
                                                        .ElementAt(i - 2)
                                                });
                                            _myMatlabWrapper.SetWorkspaceData("forceFieldMatrix",
                                                trialsArray[trialsArrayCounter].ForceFieldMatrix);

                                            _myMatlabWrapper.Execute(
                                                "[fPD, fPDsign, fFFsign] = pdForceDirectionLineSegment([vforce(1,1) vforce(1,2)], [vpos1(1,1) vpos1(1,2)], [vpos2(1,1) vpos2(1,2)], forceFieldMatrix);");
                                            _myMatlabWrapper.Execute(
                                                "[fPara, fParasign] = paraForceLineSegment([vforce(1,1) vforce(1,2)], [vpos1(1,1) vpos1(1,2)], [vpos2(1,1) vpos2(1,2)]);");

                                            _myMatlabWrapper.Execute("forcePDVector(" + (i - 1) +
                                                                     ") = sqrt(fPD(1)^2 + fPD(2)^2) * fFFSign;");
                                            _myMatlabWrapper.Execute("forceParaVector(" + (i - 1) +
                                                                     ") = sqrt(fPara(1)^2 + fPara(2)^2) * fParasign;");
                                            _myMatlabWrapper.Execute("forceAbsVector(" + (i - 1) +
                                                                     ") = sqrt(vforce(1,1)^2 + vforce(1,2)^2);");
                                        }

                                        _myMatlabWrapper.Plot("forcePDVector", "blue", 2);
                                        _myMatlabWrapper.Plot("forceParaVector", "red", 2);
                                        _myMatlabWrapper.Plot("forceAbsVector", "black", 2);
                                        _myMatlabWrapper.AddLegend("Force PD", "Force Para", "Force Abs");
                                    }
                                }
                            }
                        }
                    }
                    else if (meanIndividual == "Mean")
                    {
                        if (selectedTrialsList.Select(t => t.Trials.ToArray()).Distinct(new ArrayComparer()).Count() > 1)
                        {
                            _myManipAnalysisGui.WriteToLogBox("Trial selections are not equal!");
                        }
                        else
                        {
                            var targetArray = selectedTrialsList.Select(t => t.Target).Distinct().ToArray();

                            for (var targetCounter = 0;
                                targetCounter < targetArray.Length & !TaskManager.Cancel;
                                targetCounter++)
                            {
                                var positionData = new List<List<PositionContainer>>();
                                var velocityData = new List<List<VelocityContainer>>();
                                var forceData = new List<List<ForceContainer>>();

                                foreach (var tempContainer in selectedTrialsList.Where(t => t.Target == targetArray[targetCounter]))
                                {
                                    if (TaskManager.Cancel)
                                    {
                                        break;
                                    }

                                    var turnDateTime =
                                        _myDatabaseWrapper.GetTurns(tempContainer.Study, tempContainer.Group,
                                            tempContainer.Szenario, tempContainer.Subject)
                                            .OrderBy(t => t)
                                            .ElementAt(tempContainer.Turn - 1);

                                    var trialsArray =
                                        _myDatabaseWrapper.GetTrials(tempContainer.Study, tempContainer.Group,
                                            tempContainer.Szenario, tempContainer.Subject, turnDateTime,
                                            tempContainer.Target, tempContainer.Trials, trialTypes, forceFields,
                                            handedness, fields).ToArray();

                                    for (var trialsArrayCounter = 0;
                                        trialsArrayCounter < trialsArray.Length;
                                        trialsArrayCounter++)
                                    {
                                        if (TaskManager.Cancel)
                                        {
                                            break;
                                        }

                                        _myManipAnalysisGui.SetProgressBarValue(100.0 / sumOfAllTrials *
                                                                                processedTrialsCount++);

                                        if (trialsArray != null)
                                        {
                                            if (trajectoryVelocityForce == "Trajectory - Normalized")
                                            {
                                                trialsArray[trialsArrayCounter].PositionNormalized =
                                                    Gzip<List<PositionContainer>>.DeCompress(
                                                        trialsArray[trialsArrayCounter].ZippedPositionNormalized)
                                                        .OrderBy(t => t.TimeStamp)
                                                        .ToList();
                                                positionData.Add(trialsArray[trialsArrayCounter].PositionNormalized);

                                                if (showForceVectors || showPdForceVectors)
                                                {
                                                    trialsArray[trialsArrayCounter].MeasuredForcesNormalized =
                                                        Gzip<List<ForceContainer>>.DeCompress(
                                                            trialsArray[trialsArrayCounter]
                                                                .ZippedMeasuredForcesNormalized)
                                                            .OrderBy(t => t.TimeStamp)
                                                            .ToList();
                                                    forceData.Add(
                                                        trialsArray[trialsArrayCounter].MeasuredForcesNormalized);
                                                }
                                            }
                                            else if (trajectoryVelocityForce == "Velocity - Normalized")
                                            {
                                                trialsArray[trialsArrayCounter].VelocityNormalized =
                                                    Gzip<List<VelocityContainer>>.DeCompress(
                                                        trialsArray[trialsArrayCounter].ZippedVelocityNormalized)
                                                        .OrderBy(t => t.TimeStamp)
                                                        .ToList();
                                                velocityData.Add(trialsArray[trialsArrayCounter].VelocityNormalized);
                                            }
                                            else if (trajectoryVelocityForce == "Force - Normalized")
                                            {
                                                trialsArray[trialsArrayCounter].PositionNormalized =
                                                    Gzip<List<PositionContainer>>.DeCompress(
                                                        trialsArray[trialsArrayCounter].ZippedPositionNormalized)
                                                        .OrderBy(t => t.TimeStamp)
                                                        .ToList();
                                                trialsArray[trialsArrayCounter].MeasuredForcesNormalized =
                                                    Gzip<List<ForceContainer>>.DeCompress(
                                                        trialsArray[trialsArrayCounter].ZippedMeasuredForcesNormalized)
                                                        .OrderBy(t => t.TimeStamp)
                                                        .ToList();
                                                positionData.Add(trialsArray[trialsArrayCounter].PositionNormalized);
                                                forceData.Add(trialsArray[trialsArrayCounter].MeasuredForcesNormalized);
                                            }
                                            else
                                            {
                                                _myManipAnalysisGui.WriteToLogBox(
                                                    "Mean can only be calculated for normalized values.");
                                            }
                                        }
                                    }
                                }

                                var frameCount = 0;
                                var meanCount = 0;

                                if (trajectoryVelocityForce == "Trajectory - Normalized")
                                {
                                    frameCount = positionData[0].Count;
                                    meanCount = positionData.Count;
                                }
                                else if (trajectoryVelocityForce == "Velocity - Normalized")
                                {
                                    frameCount = velocityData[0].Count;
                                    meanCount = velocityData.Count;
                                }
                                else if (trajectoryVelocityForce == "Force - Normalized")
                                {
                                    frameCount = forceData[0].Count;
                                    meanCount = forceData.Count;
                                }

                                if (frameCount > 0)
                                {
                                    var xData = new double[frameCount];
                                    var yData = new double[frameCount];
                                    var forceVectorDataX = new double[frameCount];
                                    var forceVectorDataY = new double[frameCount];

                                    for (var meanCounter = 0; meanCounter < meanCount; meanCounter++)
                                    {
                                        for (var frameCounter = 0; frameCounter < frameCount; frameCounter++)
                                        {
                                            if (trajectoryVelocityForce == "Trajectory - Normalized")
                                            {
                                                xData[frameCounter] += positionData[meanCounter][frameCounter].X;
                                                yData[frameCounter] += positionData[meanCounter][frameCounter].Y;

                                                if (showForceVectors || showPdForceVectors)
                                                {
                                                    forceVectorDataX[frameCounter] +=
                                                        forceData[meanCounter][frameCounter].X;
                                                    forceVectorDataY[frameCounter] +=
                                                        forceData[meanCounter][frameCounter].Y;
                                                }
                                            }
                                            else if (trajectoryVelocityForce == "Velocity - Normalized")
                                            {
                                                xData[frameCounter] +=
                                                    Math.Sqrt(Math.Pow(velocityData[meanCounter][frameCounter].X, 2) +
                                                              Math.Pow(velocityData[meanCounter][frameCounter].Y, 2));
                                            }
                                            else if (trajectoryVelocityForce == "Force - Normalized")
                                            {
                                                xData[frameCounter] += positionData[meanCounter][frameCounter].X;
                                                yData[frameCounter] += positionData[meanCounter][frameCounter].Y;
                                                forceVectorDataX[frameCounter] += forceData[meanCounter][frameCounter].X;
                                                forceVectorDataY[frameCounter] += forceData[meanCounter][frameCounter].Y;
                                            }
                                        }
                                    }

                                    for (var frameCounter = 0; frameCounter < frameCount; frameCounter++)
                                    {
                                        if (trajectoryVelocityForce == "Trajectory - Normalized")
                                        {
                                            xData[frameCounter] /= positionData.Count;
                                            yData[frameCounter] /= positionData.Count;

                                            if (showForceVectors || showPdForceVectors)
                                            {
                                                forceVectorDataX[frameCounter] /= positionData.Count;
                                                forceVectorDataY[frameCounter] /= positionData.Count;
                                            }
                                        }
                                        else if (trajectoryVelocityForce == "Velocity - Normalized")
                                        {
                                            xData[frameCounter] /= meanCount;
                                        }
                                        else if (trajectoryVelocityForce == "Force - Normalized")
                                        {
                                            xData[frameCounter] /= meanCount;
                                            yData[frameCounter] /= meanCount;
                                            forceVectorDataX[frameCounter] /= positionData.Count;
                                            forceVectorDataY[frameCounter] /= positionData.Count;
                                        }
                                    }

                                    if (trajectoryVelocityForce == "Trajectory - Normalized")
                                    {
                                        _myMatlabWrapper.SetWorkspaceData("positionDataX", xData);
                                        _myMatlabWrapper.SetWorkspaceData("positionDataY", yData);
                                        _myMatlabWrapper.Plot("positionDataX", "positionDataY", "black", 2);

                                        if (showForceVectors || showPdForceVectors)
                                        {
                                            for (var i = 2; i <= xData.Length & !TaskManager.Pause; i++)
                                            {
                                                _myMatlabWrapper.SetWorkspaceData("vpos1",
                                                    new[] { xData[i - 2], yData[i - 2] });

                                                _myMatlabWrapper.SetWorkspaceData("vpos2",
                                                    new[] { xData[i - 1], yData[i - 1] });

                                                _myMatlabWrapper.SetWorkspaceData("vforce",
                                                    new[] { forceVectorDataX[i - 2] / 100.0, forceVectorDataY[i - 2] / 100.0 });

                                                if (showForceVectors)
                                                {
                                                    _myMatlabWrapper.Execute(
                                                        "quiver(vpos2(1),vpos2(2),vforce(1),vforce(2),'Color','red');");
                                                }
                                                if (showPdForceVectors)
                                                {
                                                    _myMatlabWrapper.Execute(
                                                        "[fPD, fPDsign, fFFsign] = pdForceDirectionLineSegment([vforce(1,1) vforce(1,2)], [vpos1(1,1) vpos1(1,2)], [vpos2(1,1) vpos2(1,2)], [0 0; 0 0]);");
                                                    _myMatlabWrapper.Execute(
                                                        "quiver(vpos2(1),vpos2(2),fPD(1),fPD(2),'Color','blue');");
                                                }
                                            }
                                        }
                                    }
                                    else if (trajectoryVelocityForce == "Velocity - Normalized")
                                    {
                                        _myMatlabWrapper.SetWorkspaceData("velocityData", xData);
                                        _myMatlabWrapper.Plot("velocityData", "black", 2);
                                    }
                                    else if (trajectoryVelocityForce == "Force - Normalized")
                                    {
                                        _myMatlabWrapper.Execute("forcePDVector = zeros(1, " + (xData.Length - 1) + ");");
                                        _myMatlabWrapper.Execute("forceParaVector = zeros(1, " + (xData.Length - 1) +
                                                                 ");");
                                        _myMatlabWrapper.Execute("forceAbsVector = zeros(1, " + (xData.Length - 1) +
                                                                 ");");

                                        for (var i = 2; i <= xData.Length & !TaskManager.Pause; i++)
                                        {
                                            _myMatlabWrapper.SetWorkspaceData("vpos1",
                                                new[] { xData[i - 2], yData[i - 2] });
                                            _myMatlabWrapper.SetWorkspaceData("vpos2",
                                                new[] { xData[i - 1], yData[i - 1] });
                                            _myMatlabWrapper.SetWorkspaceData("vforce",
                                                new[] { forceVectorDataX[i - 2], forceVectorDataY[i - 2] });

                                            _myMatlabWrapper.Execute(
                                                "[fPD, fPDsign, fFFsign] = pdForceDirectionLineSegment([vforce(1,1) vforce(1,2)], [vpos1(1,1) vpos1(1,2)], [vpos2(1,1) vpos2(1,2)], [0 0; 0 0]);");
                                            _myMatlabWrapper.Execute(
                                                "[fPara, fParasign] = paraForceLineSegment([vforce(1,1) vforce(1,2)], [vpos1(1,1) vpos1(1,2)], [vpos2(1,1) vpos2(1,2)]);");

                                            _myMatlabWrapper.Execute("forcePDVector(" + (i - 1) +
                                                                     ") = sqrt(fPD(1)^2 + fPD(2)^2) * fFFsign;");
                                            _myMatlabWrapper.Execute("forceParaVector(" + (i - 1) +
                                                                     ") = sqrt(fPara(1)^2 + fPara(2)^2) * fParasign;");
                                            _myMatlabWrapper.Execute("forceAbsVector(" + (i - 1) +
                                                                     ") = sqrt(vforce(1,1)^2 + vforce(1,2)^2);");
                                        }

                                        _myMatlabWrapper.Plot("forcePDVector", "blue", 2);
                                        _myMatlabWrapper.Plot("forceParaVector", "red", 2);
                                        _myMatlabWrapper.Plot("forceAbsVector", "black", 2);
                                        _myMatlabWrapper.AddLegend("Force PD", "Force Para", "Force Abs");
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                }
                _myMatlabWrapper.ClearWorkspace();
                _myManipAnalysisGui.SetProgressBarValue(0);
                _myManipAnalysisGui.WriteProgressInfo("Ready");
                TaskManager.Remove(Task.CurrentId);
            }));
        }

        public void ExportTrajectoryVelocityForce(IEnumerable<TrajectoryVelocityPlotContainer> selectedTrials,
            string meanIndividual, string trajectoryVelocityForce, IEnumerable<Trial.TrialTypeEnum> trialTypes,
            IEnumerable<Trial.ForceFieldTypeEnum> forceFields, IEnumerable<Trial.HandednessEnum> handedness,
            bool showForceVectors, bool showPdForceVectors, string fileName)
        {
            TaskManager.PushBack(Task.Factory.StartNew(() =>
            {
                _myManipAnalysisGui.WriteProgressInfo("Getting data...");
                try
                {
                    var selectedTrialsList = selectedTrials.ToList();
                    double sumOfAllTrials = selectedTrialsList.Sum(t => t.Trials.Count);
                    double processedTrialsCount = 0;

                    var fields = Builders<Trial>.Projection.Include(t => t.ForceFieldMatrix);
                    var dataFileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                    var dataFileWriter = new StreamWriter(dataFileStream);

                    if (trajectoryVelocityForce == "Velocity - Normalized")
                    {
                        fields = fields.Include(t1 => t1.ZippedVelocityNormalized);
                        if (meanIndividual == "Individual")
                        {
                            dataFileWriter.WriteLine(
                                "Study;Group;Szenario;Subject;Turn;Target;Trial;TimeStamp;VelocityX;VelocityY");
                        }
                        else if (meanIndividual == "Mean")
                        {
                            dataFileWriter.WriteLine(
                                "Study;Group;Szenario;Subject;Turn;Target;Trial;DataPoint;VelocityX;VelocityY");
                        }
                    }
                    else if (trajectoryVelocityForce == "Velocity - Filtered")
                    {
                        fields = fields.Include(t1 => t1.ZippedVelocityFiltered);
                        if (meanIndividual == "Individual")
                        {
                            dataFileWriter.WriteLine(
                                "Study;Group;Szenario;Subject;Turn;Target;Trial;TimeStamp;VelocityX;VelocityY");
                        }
                        else if (meanIndividual == "Mean")
                        {
                            dataFileWriter.WriteLine(
                                "Study;Group;Szenario;Subject;Turn;Target;Trial;DataPoint;VelocityX;VelocityY");
                        }
                    }
                    else if (trajectoryVelocityForce == "Trajectory - Normalized")
                    {
                        fields = fields.Include(t1 => t1.ZippedPositionNormalized);
                        if (meanIndividual == "Individual")
                        {
                            dataFileWriter.WriteLine(
                                "Study;Group;Szenario;Subject;Turn;Target;Trial;TimeStamp;PositionCartesianX;PositionCartesianY");
                        }
                        else if (meanIndividual == "Mean")
                        {
                            dataFileWriter.WriteLine(
                                "Study;Group;Szenario;Subject;Turn;Target;Trial;DataPoint;PositionCartesianX;PositionCartesianY");
                        }
                    }
                    else if (trajectoryVelocityForce == "Trajectory - Filtered")
                    {
                        fields = fields.Include(t1 => t1.ZippedPositionFiltered);
                        if (meanIndividual == "Individual")
                        {
                            dataFileWriter.WriteLine(
                                "Study;Group;Szenario;Subject;Turn;Target;Trial;TimeStamp;PositionCartesianX;PositionCartesianY");
                        }
                        else if (meanIndividual == "Mean")
                        {
                            dataFileWriter.WriteLine(
                                "Study;Group;Szenario;Subject;Turn;Target;Trial;DataPoint;PositionCartesianX;PositionCartesianY");
                        }
                    }
                    else if (trajectoryVelocityForce == "Trajectory - Raw")
                    {
                        fields = fields.Include(t1 => t1.ZippedPositionRaw);
                        if (meanIndividual == "Individual")
                        {
                            dataFileWriter.WriteLine(
                                "Study;Group;Szenario;Subject;Turn;Target;Trial;TimeStamp;PositionCartesianX;PositionCartesianY");
                        }
                        else if (meanIndividual == "Mean")
                        {
                            dataFileWriter.WriteLine(
                                "Study;Group;Szenario;Subject;Turn;Target;Trial;DataPoint;PositionCartesianX;PositionCartesianY");
                        }
                    }
                    else if (trajectoryVelocityForce == "Force - Normalized")
                    {
                        fields = fields.Include(t1 => t1.ZippedMeasuredForcesNormalized);
                        fields = fields.Include(t2 => t2.ZippedPositionNormalized);
                        if (meanIndividual == "Individual")
                        {
                            dataFileWriter.WriteLine(
                                "Study;Group;Szenario;Subject;Turn;Target;Trial;TimeStamp;MeasuredForcesX;MeasuredForcesY;ForcePD;ForcePara;ForceAbs");
                        }
                        else if (meanIndividual == "Mean")
                        {
                            dataFileWriter.WriteLine(
                                "Study;Group;Szenario;Subject;Turn;Target;Trial;DataPoint;MeasuredForcesX;MeasuredForcesY;ForcePD;ForcePara;ForceAbs");
                        }
                    }
                    else if (trajectoryVelocityForce == "Force - Filtered")
                    {
                        fields = fields.Include(t1 => t1.ZippedMeasuredForcesFiltered);
                        fields = fields.Include(t2 => t2.ZippedPositionFiltered);
                        if (meanIndividual == "Individual")
                        {
                            dataFileWriter.WriteLine(
                                "Study;Group;Szenario;Subject;Turn;Target;Trial;TimeStamp;MeasuredForcesX;MeasuredForcesY;ForcePD;ForcePara;ForceAbs");
                        }
                        else if (meanIndividual == "Mean")
                        {
                            dataFileWriter.WriteLine(
                                "Study;Group;Szenario;Subject;Turn;Target;Trial;DataPoint;MeasuredForcesX;MeasuredForcesY;ForcePD;ForcePara;ForceAbs");
                        }
                    }
                    else if (trajectoryVelocityForce == "Force - Raw")
                    {
                        fields = fields.Include(t1 => t1.ZippedMeasuredForcesRaw);
                        fields = fields.Include(t2 => t2.ZippedPositionRaw);
                        if (meanIndividual == "Individual")
                        {
                            dataFileWriter.WriteLine(
                                "Study;Group;Szenario;Subject;Turn;Target;Trial;TimeStamp;MeasuredForcesX;MeasuredForcesY;ForcePD;ForcePara;ForceAbs");
                        }
                        else if (meanIndividual == "Mean")
                        {
                            dataFileWriter.WriteLine(
                                "Study;Group;Szenario;Subject;Turn;Target;Trial;DataPoint;MeasuredForcesX;MeasuredForcesY;ForcePD;ForcePara;ForceAbs");
                        }
                    }

                    if (meanIndividual == "Individual")
                    {
                        foreach (var tempContainer in selectedTrialsList)
                        {
                            if (TaskManager.Cancel)
                            {
                                break;
                            }

                            var turnDateTime =
                                _myDatabaseWrapper.GetTurns(tempContainer.Study, tempContainer.Group,
                                    tempContainer.Szenario, tempContainer.Subject)
                                    .OrderBy(t => t)
                                    .ElementAt(tempContainer.Turn - 1);

                            var trialsArray =
                                _myDatabaseWrapper.GetTrials(tempContainer.Study, tempContainer.Group,
                                    tempContainer.Szenario, tempContainer.Subject, turnDateTime, tempContainer.Target,
                                    tempContainer.Trials, trialTypes, forceFields, handedness, fields).ToArray();

                            for (var trialsArrayCounter = 0;
                                trialsArrayCounter < trialsArray.Length & !TaskManager.Cancel;
                                trialsArrayCounter++)
                            {
                                if (TaskManager.Cancel)
                                {
                                    break;
                                }

                                _myManipAnalysisGui.SetProgressBarValue(100.0 / sumOfAllTrials * processedTrialsCount++);


                                if (trajectoryVelocityForce == "Velocity - Normalized")
                                {
                                    trialsArray[trialsArrayCounter].VelocityNormalized =
                                        Gzip<List<VelocityContainer>>.DeCompress(
                                            trialsArray[trialsArrayCounter].ZippedVelocityNormalized)
                                            .OrderBy(t => t.TimeStamp)
                                            .ToList();
                                    for (var i = 0;
                                        i < trialsArray[trialsArrayCounter].VelocityNormalized.Count;
                                        i++)
                                    {
                                        dataFileWriter.WriteLine(tempContainer.Study + ";" + tempContainer.Group +
                                                                 ";" + tempContainer.Szenario + ";" +
                                                                 tempContainer.Subject.PId + ";" +
                                                                 tempContainer.Turn + ";" + tempContainer.Target +
                                                                 ";" +
                                                                 trialsArray[trialsArrayCounter]
                                                                     .TargetTrialNumberInSzenario + ";" +
                                                                 trialsArray[trialsArrayCounter].VelocityNormalized[
                                                                     i].TimeStamp.ToString(
                                                                         "dd.MM.yyyy HH:mm:ss.fffffff") + ";" +
                                                                 DoubleConverter.ToExactString(
                                                                     Convert.ToDouble(
                                                                         trialsArray[trialsArrayCounter]
                                                                             .VelocityNormalized[i].X)) + ";" +
                                                                 DoubleConverter.ToExactString(
                                                                     Convert.ToDouble(
                                                                         trialsArray[trialsArrayCounter]
                                                                             .VelocityNormalized[i].Y)));
                                    }
                                }
                                else if (trajectoryVelocityForce == "Velocity - Filtered")
                                {
                                    trialsArray[trialsArrayCounter].VelocityFiltered =
                                        Gzip<List<VelocityContainer>>.DeCompress(
                                            trialsArray[trialsArrayCounter].ZippedVelocityFiltered)
                                            .OrderBy(t => t.TimeStamp)
                                            .ToList();
                                    for (var i = 0; i < trialsArray[trialsArrayCounter].VelocityFiltered.Count; i++)
                                    {
                                        dataFileWriter.WriteLine(tempContainer.Study + ";" + tempContainer.Group +
                                                                 ";" + tempContainer.Szenario + ";" +
                                                                 tempContainer.Subject.PId + ";" +
                                                                 tempContainer.Turn + ";" + tempContainer.Target +
                                                                 ";" +
                                                                 trialsArray[trialsArrayCounter]
                                                                     .TargetTrialNumberInSzenario + ";" +
                                                                 trialsArray[trialsArrayCounter].VelocityFiltered[i]
                                                                     .TimeStamp.ToString(
                                                                         "dd.MM.yyyy HH:mm:ss.fffffff") + ";" +
                                                                 DoubleConverter.ToExactString(
                                                                     Convert.ToDouble(
                                                                         trialsArray[trialsArrayCounter]
                                                                             .VelocityFiltered[i].X)) + ";" +
                                                                 DoubleConverter.ToExactString(
                                                                     Convert.ToDouble(
                                                                         trialsArray[trialsArrayCounter]
                                                                             .VelocityFiltered[i].Y)));
                                    }
                                }
                                else if (trajectoryVelocityForce == "Trajectory - Normalized")
                                {
                                    trialsArray[trialsArrayCounter].PositionNormalized =
                                        Gzip<List<PositionContainer>>.DeCompress(
                                            trialsArray[trialsArrayCounter].ZippedPositionNormalized)
                                            .OrderBy(t => t.TimeStamp)
                                            .ToList();
                                    for (var i = 0;
                                        i < trialsArray[trialsArrayCounter].PositionNormalized.Count;
                                        i++)
                                    {
                                        dataFileWriter.WriteLine(tempContainer.Study + ";" + tempContainer.Group +
                                                                 ";" + tempContainer.Szenario + ";" +
                                                                 tempContainer.Subject.PId + ";" +
                                                                 tempContainer.Turn + ";" + tempContainer.Target +
                                                                 ";" +
                                                                 trialsArray[trialsArrayCounter]
                                                                     .TargetTrialNumberInSzenario + ";" +
                                                                 trialsArray[trialsArrayCounter].PositionNormalized[
                                                                     i].TimeStamp.ToString(
                                                                         "dd.MM.yyyy HH:mm:ss.fffffff") + ";" +
                                                                 DoubleConverter.ToExactString(
                                                                     Convert.ToDouble(
                                                                         trialsArray[trialsArrayCounter]
                                                                             .PositionNormalized[i].X)) + ";" +
                                                                 DoubleConverter.ToExactString(
                                                                     Convert.ToDouble(
                                                                         trialsArray[trialsArrayCounter]
                                                                             .PositionNormalized[i].Y)));
                                    }
                                }
                                else if (trajectoryVelocityForce == "Trajectory - Filtered")
                                {
                                    trialsArray[trialsArrayCounter].PositionFiltered =
                                        Gzip<List<PositionContainer>>.DeCompress(
                                            trialsArray[trialsArrayCounter].ZippedPositionFiltered)
                                            .OrderBy(t => t.TimeStamp)
                                            .ToList();
                                    for (var i = 0; i < trialsArray[trialsArrayCounter].PositionFiltered.Count; i++)
                                    {
                                        dataFileWriter.WriteLine(tempContainer.Study + ";" + tempContainer.Group +
                                                                 ";" + tempContainer.Szenario + ";" +
                                                                 tempContainer.Subject.PId + ";" +
                                                                 tempContainer.Turn + ";" + tempContainer.Target +
                                                                 ";" +
                                                                 trialsArray[trialsArrayCounter]
                                                                     .TargetTrialNumberInSzenario + ";" +
                                                                 trialsArray[trialsArrayCounter].PositionFiltered[i]
                                                                     .TimeStamp.ToString(
                                                                         "dd.MM.yyyy HH:mm:ss.fffffff") + ";" +
                                                                 DoubleConverter.ToExactString(
                                                                     Convert.ToDouble(
                                                                         trialsArray[trialsArrayCounter]
                                                                             .PositionFiltered[i].X)) + ";" +
                                                                 DoubleConverter.ToExactString(
                                                                     Convert.ToDouble(
                                                                         trialsArray[trialsArrayCounter]
                                                                             .PositionFiltered[i].Y)));
                                    }
                                }
                                else if (trajectoryVelocityForce == "Trajectory - Raw")
                                {
                                    trialsArray[trialsArrayCounter].PositionRaw =
                                        Gzip<List<PositionContainer>>.DeCompress(
                                            trialsArray[trialsArrayCounter].ZippedPositionRaw)
                                            .OrderBy(t => t.TimeStamp)
                                            .ToList();
                                    for (var i = 0; i < trialsArray[trialsArrayCounter].PositionRaw.Count; i++)
                                    {
                                        dataFileWriter.WriteLine(tempContainer.Study + ";" + tempContainer.Group +
                                                                 ";" + tempContainer.Szenario + ";" +
                                                                 tempContainer.Subject.PId + ";" +
                                                                 tempContainer.Turn + ";" + tempContainer.Target +
                                                                 ";" +
                                                                 trialsArray[trialsArrayCounter]
                                                                     .TargetTrialNumberInSzenario + ";" +
                                                                 trialsArray[trialsArrayCounter].PositionRaw[i]
                                                                     .TimeStamp.ToString(
                                                                         "dd.MM.yyyy HH:mm:ss.fffffff") + ";" +
                                                                 DoubleConverter.ToExactString(
                                                                     Convert.ToDouble(
                                                                         trialsArray[trialsArrayCounter].PositionRaw
                                                                             [i].X)) + ";" +
                                                                 DoubleConverter.ToExactString(
                                                                     Convert.ToDouble(
                                                                         trialsArray[trialsArrayCounter].PositionRaw
                                                                             [i].Y)));
                                    }
                                }
                                else if (trajectoryVelocityForce == "Force - Normalized")
                                {
                                    trialsArray[trialsArrayCounter].PositionNormalized =
                                        Gzip<List<PositionContainer>>.DeCompress(
                                            trialsArray[trialsArrayCounter].ZippedPositionNormalized)
                                            .OrderBy(t => t.TimeStamp)
                                            .ToList();
                                    trialsArray[trialsArrayCounter].MeasuredForcesNormalized =
                                        Gzip<List<ForceContainer>>.DeCompress(
                                            trialsArray[trialsArrayCounter].ZippedMeasuredForcesNormalized)
                                            .OrderBy(t => t.TimeStamp)
                                            .ToList();

                                    _myMatlabWrapper.Execute("forcePDVector = zeros(1, " +
                                                             (trialsArray[trialsArrayCounter].PositionNormalized
                                                                 .Count - 1) + ");");
                                    _myMatlabWrapper.Execute("forceParaVector = zeros(1, " +
                                                             (trialsArray[trialsArrayCounter].PositionNormalized
                                                                 .Count - 1) + ");");
                                    _myMatlabWrapper.Execute("forceAbsVector = zeros(1, " +
                                                             (trialsArray[trialsArrayCounter].PositionNormalized
                                                                 .Count - 1) + ");");

                                    for (var i = 2;
                                        i <= trialsArray[trialsArrayCounter].PositionNormalized.Count &
                                        !TaskManager.Pause;
                                        i++)
                                    {
                                        _myMatlabWrapper.SetWorkspaceData("vpos1",
                                            new[]
                                            {
                                                trialsArray[trialsArrayCounter].PositionNormalized.Select(t => t.X)
                                                    .ElementAt(i - 2),
                                                trialsArray[trialsArrayCounter].PositionNormalized.Select(t => t.Y)
                                                    .ElementAt(i - 2)
                                            });
                                        _myMatlabWrapper.SetWorkspaceData("vpos2",
                                            new[]
                                            {
                                                trialsArray[trialsArrayCounter].PositionNormalized.Select(t => t.X)
                                                    .ElementAt(i - 1),
                                                trialsArray[trialsArrayCounter].PositionNormalized.Select(t => t.Y)
                                                    .ElementAt(i - 1)
                                            });
                                        _myMatlabWrapper.SetWorkspaceData("vforce",
                                            new[]
                                            {
                                                trialsArray[trialsArrayCounter].MeasuredForcesNormalized.Select(
                                                    t => t.X).ElementAt(i - 2),
                                                trialsArray[trialsArrayCounter].MeasuredForcesNormalized.Select(
                                                    t => t.Y).ElementAt(i - 2)
                                            });
                                        _myMatlabWrapper.SetWorkspaceData("forceFieldMatrix",
                                            trialsArray[trialsArrayCounter].ForceFieldMatrix);

                                        _myMatlabWrapper.Execute(
                                            "[fPD, fPDsign, fFFsign] = pdForceDirectionLineSegment([vforce(1,1) vforce(1,2)], [vpos1(1,1) vpos1(1,2)], [vpos2(1,1) vpos2(1,2)], forceFieldMatrix);");
                                        _myMatlabWrapper.Execute(
                                            "[fPara, fParasign] = paraForceLineSegment([vforce(1,1) vforce(1,2)], [vpos1(1,1) vpos1(1,2)], [vpos2(1,1) vpos2(1,2)]);");

                                        _myMatlabWrapper.Execute("forcePDVector(" + (i - 1) +
                                                                 ") = sqrt(fPD(1)^2 + fPD(2)^2) * fFFsign;");
                                        _myMatlabWrapper.Execute("forceParaVector(" + (i - 1) +
                                                                 ") = sqrt(fPara(1)^2 + fPara(2)^2) * fParasign;");
                                        _myMatlabWrapper.Execute("forceAbsVector(" + (i - 1) +
                                                                 ") = sqrt(vforce(1,1)^2 + vforce(1,2)^2);");
                                    }

                                    double[,] forcePD = _myMatlabWrapper.GetWorkspaceData("forcePDVector");
                                    double[,] forcePara = _myMatlabWrapper.GetWorkspaceData("forceParaVector");
                                    double[,] forceAbs = _myMatlabWrapper.GetWorkspaceData("forceAbsVector");

                                    for (var i = 0;
                                        i < trialsArray[trialsArrayCounter].MeasuredForcesNormalized.Count;
                                        i++)
                                    {
                                        if (i < forcePD.Length)
                                        {
                                            dataFileWriter.WriteLine(tempContainer.Study + ";" + tempContainer.Group +
                                                                     ";" + tempContainer.Szenario + ";" +
                                                                     tempContainer.Subject.PId + ";" +
                                                                     tempContainer.Turn + ";" + tempContainer.Target +
                                                                     ";" +
                                                                     trialsArray[trialsArrayCounter]
                                                                         .TargetTrialNumberInSzenario + ";" +
                                                                     trialsArray[trialsArrayCounter]
                                                                         .MeasuredForcesNormalized[i].TimeStamp
                                                                         .ToString("dd.MM.yyyy HH:mm:ss.fffffff") +
                                                                     ";" +
                                                                     DoubleConverter.ToExactString(
                                                                         Convert.ToDouble(
                                                                             trialsArray[trialsArrayCounter]
                                                                                 .MeasuredForcesNormalized[i].X)) +
                                                                     ";" +
                                                                     DoubleConverter.ToExactString(
                                                                         Convert.ToDouble(
                                                                             trialsArray[trialsArrayCounter]
                                                                                 .MeasuredForcesNormalized[i].Y)) +
                                                                     ";" +
                                                                     DoubleConverter.ToExactString(forcePD[0, i]) +
                                                                     ";" +
                                                                     DoubleConverter.ToExactString(forcePara[0, i]) +
                                                                     ";" +
                                                                     DoubleConverter.ToExactString(forceAbs[0, i]));
                                        }
                                        else
                                        {
                                            dataFileWriter.WriteLine(tempContainer.Study + ";" + tempContainer.Group +
                                                                     ";" + tempContainer.Szenario + ";" +
                                                                     tempContainer.Subject.PId + ";" +
                                                                     tempContainer.Turn + ";" + tempContainer.Target +
                                                                     ";" +
                                                                     trialsArray[trialsArrayCounter]
                                                                         .TargetTrialNumberInSzenario + ";" +
                                                                     trialsArray[trialsArrayCounter]
                                                                         .MeasuredForcesNormalized[i].TimeStamp
                                                                         .ToString("dd.MM.yyyy HH:mm:ss.fffffff") +
                                                                     ";" +
                                                                     DoubleConverter.ToExactString(
                                                                         Convert.ToDouble(
                                                                             trialsArray[trialsArrayCounter]
                                                                                 .MeasuredForcesNormalized[i].X)) +
                                                                     ";" +
                                                                     DoubleConverter.ToExactString(
                                                                         Convert.ToDouble(
                                                                             trialsArray[trialsArrayCounter]
                                                                                 .MeasuredForcesNormalized[i].Y)) +
                                                                     ";;;");
                                        }
                                    }
                                }
                                else if (trajectoryVelocityForce == "Force - Filtered")
                                {
                                    trialsArray[trialsArrayCounter].PositionFiltered =
                                        Gzip<List<PositionContainer>>.DeCompress(
                                            trialsArray[trialsArrayCounter].ZippedPositionFiltered)
                                            .OrderBy(t => t.TimeStamp)
                                            .ToList();
                                    trialsArray[trialsArrayCounter].MeasuredForcesFiltered =
                                        Gzip<List<ForceContainer>>.DeCompress(
                                            trialsArray[trialsArrayCounter].ZippedMeasuredForcesFiltered)
                                            .OrderBy(t => t.TimeStamp)
                                            .ToList();

                                    _myMatlabWrapper.Execute("forcePDVector = zeros(1, " +
                                                             (trialsArray[trialsArrayCounter].PositionFiltered.Count -
                                                              1) + ");");
                                    _myMatlabWrapper.Execute("forceParaVector = zeros(1, " +
                                                             (trialsArray[trialsArrayCounter].PositionFiltered.Count -
                                                              1) + ");");
                                    _myMatlabWrapper.Execute("forceAbsVector = zeros(1, " +
                                                             (trialsArray[trialsArrayCounter].PositionFiltered.Count -
                                                              1) + ");");

                                    for (var i = 2;
                                        i <= trialsArray[trialsArrayCounter].PositionFiltered.Count &
                                        !TaskManager.Pause;
                                        i++)
                                    {
                                        _myMatlabWrapper.SetWorkspaceData("vpos1",
                                            new[]
                                            {
                                                trialsArray[trialsArrayCounter].PositionFiltered.Select(t => t.X)
                                                    .ElementAt(i - 2),
                                                trialsArray[trialsArrayCounter].PositionFiltered.Select(t => t.Y)
                                                    .ElementAt(i - 2)
                                            });
                                        _myMatlabWrapper.SetWorkspaceData("vpos2",
                                            new[]
                                            {
                                                trialsArray[trialsArrayCounter].PositionFiltered.Select(t => t.X)
                                                    .ElementAt(i - 1),
                                                trialsArray[trialsArrayCounter].PositionFiltered.Select(t => t.Y)
                                                    .ElementAt(i - 1)
                                            });
                                        _myMatlabWrapper.SetWorkspaceData("vforce",
                                            new[]
                                            {
                                                trialsArray[trialsArrayCounter].MeasuredForcesFiltered.Select(
                                                    t => t.X).ElementAt(i - 2),
                                                trialsArray[trialsArrayCounter].MeasuredForcesFiltered.Select(
                                                    t => t.Y).ElementAt(i - 2)
                                            });
                                        _myMatlabWrapper.SetWorkspaceData("forceFieldMatrix",
                                            trialsArray[trialsArrayCounter].ForceFieldMatrix);

                                        _myMatlabWrapper.Execute(
                                            "[fPD, fPDsign, fFFsign] = pdForceDirectionLineSegment([vforce(1,1) vforce(1,2)], [vpos1(1,1) vpos1(1,2)], [vpos2(1,1) vpos2(1,2)], forceFieldMatrix);");
                                        _myMatlabWrapper.Execute(
                                            "[fPara, fParasign] = paraForceLineSegment([vforce(1,1) vforce(1,2)], [vpos1(1,1) vpos1(1,2)], [vpos2(1,1) vpos2(1,2)]);");

                                        _myMatlabWrapper.Execute("forcePDVector(" + (i - 1) +
                                                                 ") = sqrt(fPD(1)^2 + fPD(2)^2) * fFFsign;");
                                        _myMatlabWrapper.Execute("forceParaVector(" + (i - 1) +
                                                                 ") = sqrt(fPara(1)^2 + fPara(2)^2) * fParasign;");
                                        _myMatlabWrapper.Execute("forceAbsVector(" + (i - 1) +
                                                                 ") = sqrt(vforce(1,1)^2 + vforce(1,2)^2);");
                                    }

                                    double[,] forcePD = _myMatlabWrapper.GetWorkspaceData("forcePDVector");
                                    double[,] forcePara = _myMatlabWrapper.GetWorkspaceData("forceParaVector");
                                    double[,] forceAbs = _myMatlabWrapper.GetWorkspaceData("forceAbsVector");

                                    for (var i = 0;
                                        i < trialsArray[trialsArrayCounter].MeasuredForcesFiltered.Count;
                                        i++)
                                    {
                                        if (i < forcePD.Length)
                                        {
                                            dataFileWriter.WriteLine(tempContainer.Study + ";" + tempContainer.Group +
                                                                     ";" + tempContainer.Szenario + ";" +
                                                                     tempContainer.Subject.PId + ";" +
                                                                     tempContainer.Turn + ";" + tempContainer.Target +
                                                                     ";" +
                                                                     trialsArray[trialsArrayCounter]
                                                                         .TargetTrialNumberInSzenario + ";" +
                                                                     trialsArray[trialsArrayCounter]
                                                                         .MeasuredForcesFiltered[i].TimeStamp
                                                                         .ToString("dd.MM.yyyy HH:mm:ss.fffffff") +
                                                                     ";" +
                                                                     DoubleConverter.ToExactString(
                                                                         Convert.ToDouble(
                                                                             trialsArray[trialsArrayCounter]
                                                                                 .MeasuredForcesFiltered[i].X)) +
                                                                     ";" +
                                                                     DoubleConverter.ToExactString(
                                                                         Convert.ToDouble(
                                                                             trialsArray[trialsArrayCounter]
                                                                                 .MeasuredForcesFiltered[i].Y)) +
                                                                     ";" +
                                                                     DoubleConverter.ToExactString(forcePD[0, i]) +
                                                                     ";" +
                                                                     DoubleConverter.ToExactString(forcePara[0, i]) +
                                                                     ";" +
                                                                     DoubleConverter.ToExactString(forceAbs[0, i]));
                                        }
                                        else
                                        {
                                            dataFileWriter.WriteLine(tempContainer.Study + ";" + tempContainer.Group +
                                                                     ";" + tempContainer.Szenario + ";" +
                                                                     tempContainer.Subject.PId + ";" +
                                                                     tempContainer.Turn + ";" + tempContainer.Target +
                                                                     ";" +
                                                                     trialsArray[trialsArrayCounter]
                                                                         .TargetTrialNumberInSzenario + ";" +
                                                                     trialsArray[trialsArrayCounter]
                                                                         .MeasuredForcesFiltered[i].TimeStamp
                                                                         .ToString("dd.MM.yyyy HH:mm:ss.fffffff") +
                                                                     ";" +
                                                                     DoubleConverter.ToExactString(
                                                                         Convert.ToDouble(
                                                                             trialsArray[trialsArrayCounter]
                                                                                 .MeasuredForcesFiltered[i].X)) +
                                                                     ";" +
                                                                     DoubleConverter.ToExactString(
                                                                         Convert.ToDouble(
                                                                             trialsArray[trialsArrayCounter]
                                                                                 .MeasuredForcesFiltered[i].Y)) +
                                                                     ";;;");
                                        }
                                    }
                                }
                                else if (trajectoryVelocityForce == "Force - Raw")
                                {
                                    trialsArray[trialsArrayCounter].PositionRaw =
                                        Gzip<List<PositionContainer>>.DeCompress(
                                            trialsArray[trialsArrayCounter].ZippedPositionRaw)
                                            .OrderBy(t => t.TimeStamp)
                                            .ToList();
                                    trialsArray[trialsArrayCounter].MeasuredForcesRaw =
                                        Gzip<List<ForceContainer>>.DeCompress(
                                            trialsArray[trialsArrayCounter].ZippedMeasuredForcesRaw)
                                            .OrderBy(t => t.TimeStamp)
                                            .ToList();

                                    _myMatlabWrapper.Execute("forcePDVector = zeros(1, " +
                                                             (trialsArray[trialsArrayCounter].PositionRaw.Count - 1) +
                                                             ");");
                                    _myMatlabWrapper.Execute("forceParaVector = zeros(1, " +
                                                             (trialsArray[trialsArrayCounter].PositionRaw.Count - 1) +
                                                             ");");
                                    _myMatlabWrapper.Execute("forceAbsVector = zeros(1, " +
                                                             (trialsArray[trialsArrayCounter].PositionRaw.Count - 1) +
                                                             ");");

                                    for (var i = 2;
                                        i <= trialsArray[trialsArrayCounter].PositionRaw.Count & !TaskManager.Pause;
                                        i++)
                                    {
                                        _myMatlabWrapper.SetWorkspaceData("vpos1",
                                            new[]
                                            {
                                                trialsArray[trialsArrayCounter].PositionRaw.Select(t => t.X)
                                                    .ElementAt(i - 2),
                                                trialsArray[trialsArrayCounter].PositionRaw.Select(t => t.Y)
                                                    .ElementAt(i - 2)
                                            });
                                        _myMatlabWrapper.SetWorkspaceData("vpos2",
                                            new[]
                                            {
                                                trialsArray[trialsArrayCounter].PositionRaw.Select(t => t.X)
                                                    .ElementAt(i - 1),
                                                trialsArray[trialsArrayCounter].PositionRaw.Select(t => t.Y)
                                                    .ElementAt(i - 1)
                                            });
                                        _myMatlabWrapper.SetWorkspaceData("vforce",
                                            new[]
                                            {
                                                trialsArray[trialsArrayCounter].MeasuredForcesRaw.Select(t => t.X)
                                                    .ElementAt(i - 2),
                                                trialsArray[trialsArrayCounter].MeasuredForcesRaw.Select(t => t.Y)
                                                    .ElementAt(i - 2)
                                            });
                                        _myMatlabWrapper.SetWorkspaceData("forceFieldMatrix",
                                            trialsArray[trialsArrayCounter].ForceFieldMatrix);

                                        _myMatlabWrapper.Execute(
                                            "[fPD, fPDsign, fFFsign] = pdForceDirectionLineSegment([vforce(1,1) vforce(1,2)], [vpos1(1,1) vpos1(1,2)], [vpos2(1,1) vpos2(1,2)], forceFieldMatrix);");
                                        _myMatlabWrapper.Execute(
                                            "[fPara, fParasign] = paraForceLineSegment([vforce(1,1) vforce(1,2)], [vpos1(1,1) vpos1(1,2)], [vpos2(1,1) vpos2(1,2)]);");

                                        _myMatlabWrapper.Execute("forcePDVector(" + (i - 1) +
                                                                 ") = sqrt(fPD(1)^2 + fPD(2)^2) * fFFsign;");
                                        _myMatlabWrapper.Execute("forceParaVector(" + (i - 1) +
                                                                 ") = sqrt(fPara(1)^2 + fPara(2)^2) * fParasign;");
                                        _myMatlabWrapper.Execute("forceAbsVector(" + (i - 1) +
                                                                 ") = sqrt(vforce(1,1)^2 + vforce(1,2)^2);");
                                    }

                                    double[,] forcePD = _myMatlabWrapper.GetWorkspaceData("forcePDVector");
                                    double[,] forcePara = _myMatlabWrapper.GetWorkspaceData("forceParaVector");
                                    double[,] forceAbs = _myMatlabWrapper.GetWorkspaceData("forceAbsVector");

                                    for (var i = 0;
                                        i < trialsArray[trialsArrayCounter].MeasuredForcesRaw.Count;
                                        i++)
                                    {
                                        if (i < forcePD.Length)
                                        {
                                            dataFileWriter.WriteLine(tempContainer.Study + ";" + tempContainer.Group +
                                                                     ";" + tempContainer.Szenario + ";" +
                                                                     tempContainer.Subject.PId + ";" +
                                                                     tempContainer.Turn + ";" + tempContainer.Target +
                                                                     ";" +
                                                                     trialsArray[trialsArrayCounter]
                                                                         .TargetTrialNumberInSzenario + ";" +
                                                                     trialsArray[trialsArrayCounter]
                                                                         .MeasuredForcesRaw[i].TimeStamp.ToString(
                                                                             "dd.MM.yyyy HH:mm:ss.fffffff") + ";" +
                                                                     DoubleConverter.ToExactString(
                                                                         Convert.ToDouble(
                                                                             trialsArray[trialsArrayCounter]
                                                                                 .MeasuredForcesRaw[i].X)) + ";" +
                                                                     DoubleConverter.ToExactString(
                                                                         Convert.ToDouble(
                                                                             trialsArray[trialsArrayCounter]
                                                                                 .MeasuredForcesRaw[i].Y)) + ";" +
                                                                     DoubleConverter.ToExactString(forcePD[0, i]) +
                                                                     ";" +
                                                                     DoubleConverter.ToExactString(forcePara[0, i]) +
                                                                     ";" +
                                                                     DoubleConverter.ToExactString(forceAbs[0, i]));
                                        }
                                        else
                                        {
                                            dataFileWriter.WriteLine(tempContainer.Study + ";" + tempContainer.Group +
                                                                     ";" + tempContainer.Szenario + ";" +
                                                                     tempContainer.Subject.PId + ";" +
                                                                     tempContainer.Turn + ";" + tempContainer.Target +
                                                                     ";" +
                                                                     trialsArray[trialsArrayCounter]
                                                                         .TargetTrialNumberInSzenario + ";" +
                                                                     trialsArray[trialsArrayCounter]
                                                                         .MeasuredForcesRaw[i].TimeStamp.ToString(
                                                                             "dd.MM.yyyy HH:mm:ss.fffffff") + ";" +
                                                                     DoubleConverter.ToExactString(
                                                                         Convert.ToDouble(
                                                                             trialsArray[trialsArrayCounter]
                                                                                 .MeasuredForcesRaw[i].X)) + ";" +
                                                                     DoubleConverter.ToExactString(
                                                                         Convert.ToDouble(
                                                                             trialsArray[trialsArrayCounter]
                                                                                 .MeasuredForcesRaw[i].Y)) + ";;;");
                                        }
                                    }
                                }
                            }
                        }
                    }

                    else if (meanIndividual == "Mean")
                    {
                        if (selectedTrialsList.Select(t => t.Trials.ToArray()).Distinct(new ArrayComparer()).Count() > 1)
                        {
                            _myManipAnalysisGui.WriteToLogBox("Trial selections are not equal!");
                        }
                        else
                        {
                            var targetArray = selectedTrialsList.Select(t => t.Target).Distinct().ToArray();

                            for (var targetCounter = 0;
                                targetCounter < targetArray.Length & !TaskManager.Cancel;
                                targetCounter++)
                            {
                                var positionData = new List<List<PositionContainer>>();
                                var velocityData = new List<List<VelocityContainer>>();
                                var forceData = new List<List<ForceContainer>>();

                                var studys = string.Join(",",
                                    selectedTrialsList.Select(t => t.Study).Distinct().ToArray());
                                var groups = string.Join(",",
                                    selectedTrialsList.Select(t => t.Group).Distinct().ToArray());
                                var szenarios = string.Join(",",
                                    selectedTrialsList.Select(t => t.Szenario).Distinct().ToArray());
                                var subjectPIds = string.Join(",",
                                    selectedTrialsList.Select(t => t.Subject.PId).Distinct().ToArray());
                                var turns = string.Join(",", selectedTrialsList.Select(t => t.Turn).Distinct().ToArray());
                                var trials = string.Join(",",
                                    selectedTrialsList.Select(t => t.GetTrialsString()).Distinct().ToArray());


                                foreach (
                                    var tempContainer in
                                        selectedTrialsList.Where(t => t.Target == targetArray[targetCounter]))
                                {
                                    if (TaskManager.Cancel)
                                    {
                                        break;
                                    }

                                    var turnDateTime =
                                        _myDatabaseWrapper.GetTurns(tempContainer.Study, tempContainer.Group,
                                            tempContainer.Szenario, tempContainer.Subject)
                                            .OrderBy(t => t)
                                            .ElementAt(tempContainer.Turn - 1);

                                    var trialsArray =
                                        _myDatabaseWrapper.GetTrials(tempContainer.Study, tempContainer.Group,
                                            tempContainer.Szenario, tempContainer.Subject, turnDateTime,
                                            tempContainer.Target, tempContainer.Trials, trialTypes, forceFields,
                                            handedness, fields).ToArray();

                                    for (var trialsArrayCounter = 0;
                                        trialsArrayCounter < trialsArray.Length & !TaskManager.Cancel;
                                        trialsArrayCounter++)
                                    {
                                        if (TaskManager.Cancel)
                                        {
                                            break;
                                        }

                                        _myManipAnalysisGui.SetProgressBarValue(100.0 / sumOfAllTrials *
                                                                                processedTrialsCount++);


                                        if (trajectoryVelocityForce == "Trajectory - Normalized")
                                        {
                                            trialsArray[trialsArrayCounter].PositionNormalized =
                                                Gzip<List<PositionContainer>>.DeCompress(
                                                    trialsArray[trialsArrayCounter].ZippedPositionNormalized)
                                                    .OrderBy(t => t.TimeStamp)
                                                    .ToList();
                                            positionData.Add(trialsArray[trialsArrayCounter].PositionNormalized);
                                        }
                                        else if (trajectoryVelocityForce == "Velocity - Normalized")
                                        {
                                            trialsArray[trialsArrayCounter].VelocityNormalized =
                                                Gzip<List<VelocityContainer>>.DeCompress(
                                                    trialsArray[trialsArrayCounter].ZippedVelocityNormalized)
                                                    .OrderBy(t => t.TimeStamp)
                                                    .ToList();
                                            velocityData.Add(trialsArray[trialsArrayCounter].VelocityNormalized);
                                        }
                                        else if (trajectoryVelocityForce == "Force - Normalized")
                                        {
                                            trialsArray[trialsArrayCounter].MeasuredForcesNormalized =
                                                Gzip<List<ForceContainer>>.DeCompress(
                                                    trialsArray[trialsArrayCounter].ZippedMeasuredForcesNormalized)
                                                    .OrderBy(t => t.TimeStamp)
                                                    .ToList();
                                            trialsArray[trialsArrayCounter].PositionNormalized =
                                                Gzip<List<PositionContainer>>.DeCompress(
                                                    trialsArray[trialsArrayCounter].ZippedPositionNormalized)
                                                    .OrderBy(t => t.TimeStamp)
                                                    .ToList();
                                            forceData.Add(trialsArray[trialsArrayCounter].MeasuredForcesNormalized);
                                            positionData.Add(trialsArray[trialsArrayCounter].PositionNormalized);
                                        }
                                        else
                                        {
                                            _myManipAnalysisGui.WriteToLogBox(
                                                "Mean can only be calculated for normalized values.");
                                        }
                                    }
                                }

                                var frameCount = 0;
                                var meanCount = 0;
                                if (trajectoryVelocityForce == "Trajectory - Normalized")
                                {
                                    frameCount = positionData[0].Count;
                                    meanCount = positionData.Count;
                                }
                                else if (trajectoryVelocityForce == "Velocity - Normalized")
                                {
                                    frameCount = velocityData[0].Count;
                                    meanCount = velocityData.Count;
                                }
                                else if (trajectoryVelocityForce == "Force - Normalized")
                                {
                                    frameCount = forceData[0].Count;
                                    meanCount = forceData.Count;
                                }

                                if (frameCount > 0)
                                {
                                    var xData = new double[frameCount];
                                    var yData = new double[frameCount];
                                    var forceVectorDataX = new double[frameCount];
                                    var forceVectorDataY = new double[frameCount];

                                    for (var meanCounter = 0; meanCounter < meanCount; meanCounter++)
                                    {
                                        for (var frameCounter = 0; frameCounter < frameCount; frameCounter++)
                                        {
                                            if (trajectoryVelocityForce == "Trajectory - Normalized")
                                            {
                                                xData[frameCounter] += positionData[meanCounter][frameCounter].X;
                                                yData[frameCounter] += positionData[meanCounter][frameCounter].Y;
                                            }
                                            else if (trajectoryVelocityForce == "Velocity - Normalized")
                                            {
                                                xData[frameCounter] += velocityData[meanCounter][frameCounter].X;
                                                yData[frameCounter] += velocityData[meanCounter][frameCounter].Y;
                                            }
                                            else if (trajectoryVelocityForce == "Force - Normalized")
                                            {
                                                xData[frameCounter] += positionData[meanCounter][frameCounter].X;
                                                yData[frameCounter] += positionData[meanCounter][frameCounter].Y;
                                                forceVectorDataX[frameCounter] += forceData[meanCounter][frameCounter].X;
                                                forceVectorDataY[frameCounter] += forceData[meanCounter][frameCounter].Y;
                                            }
                                        }
                                    }

                                    for (var frameCounter = 0; frameCounter < frameCount; frameCounter++)
                                    {
                                        if (trajectoryVelocityForce == "Trajectory - Normalized")
                                        {
                                            xData[frameCounter] /= meanCount;
                                            yData[frameCounter] /= meanCount;
                                        }
                                        else if (trajectoryVelocityForce == "Velocity - Normalized")
                                        {
                                            xData[frameCounter] /= meanCount;
                                            yData[frameCounter] /= meanCount;
                                        }
                                        else if (trajectoryVelocityForce == "Force - Normalized")
                                        {
                                            xData[frameCounter] /= meanCount;
                                            yData[frameCounter] /= meanCount;
                                            forceVectorDataX[frameCounter] /= meanCount;
                                            forceVectorDataY[frameCounter] /= meanCount;
                                        }
                                    }

                                    if (trajectoryVelocityForce == "Trajectory - Normalized")
                                    {
                                        for (var i = 0; i < xData.Length; i++)
                                        {
                                            dataFileWriter.WriteLine(studys + ";" + groups + ";" + szenarios + ";" +
                                                                     subjectPIds + ";" + turns + ";" +
                                                                     targetArray[targetCounter] + ";" + trials + ";" + i +
                                                                     ";" + DoubleConverter.ToExactString(xData[i]) + ";" +
                                                                     DoubleConverter.ToExactString(yData[i]));
                                        }
                                    }
                                    else if (trajectoryVelocityForce == "Velocity - Normalized")
                                    {
                                        for (var i = 0; i < xData.Length; i++)
                                        {
                                            dataFileWriter.WriteLine(studys + ";" + groups + ";" + szenarios + ";" +
                                                                     subjectPIds + ";" + turns + ";" +
                                                                     targetArray[targetCounter] + ";" + trials + ";" + i +
                                                                     ";" + DoubleConverter.ToExactString(xData[i]) + ";" +
                                                                     DoubleConverter.ToExactString(yData[i]));
                                        }
                                    }
                                    else if (trajectoryVelocityForce == "Force - Normalized")
                                    {
                                        _myMatlabWrapper.Execute("forcePDVector = zeros(1, " + (xData.Length - 1) + ");");
                                        _myMatlabWrapper.Execute("forceParaVector = zeros(1, " + (xData.Length - 1) +
                                                                 ");");
                                        _myMatlabWrapper.Execute("forceAbsVector = zeros(1, " + (xData.Length - 1) +
                                                                 ");");

                                        for (var i = 2; i <= xData.Length & !TaskManager.Pause; i++)
                                        {
                                            _myMatlabWrapper.SetWorkspaceData("vpos1",
                                                new[] { xData[i - 2], yData[i - 2] });
                                            _myMatlabWrapper.SetWorkspaceData("vpos2",
                                                new[] { xData[i - 1], yData[i - 1] });
                                            _myMatlabWrapper.SetWorkspaceData("vforce",
                                                new[] { forceVectorDataX[i - 2], forceVectorDataY[i - 2] });

                                            _myMatlabWrapper.Execute(
                                                "[fPD, fPDsign, fFFsign] = pdForceDirectionLineSegment([vforce(1,1) vforce(1,2)], [vpos1(1,1) vpos1(1,2)], [vpos2(1,1) vpos2(1,2)], [0 0; 0 0]);");
                                            _myMatlabWrapper.Execute(
                                                "[fPara, fParasign] = paraForceLineSegment([vforce(1,1) vforce(1,2)], [vpos1(1,1) vpos1(1,2)], [vpos2(1,1) vpos2(1,2)]);");

                                            _myMatlabWrapper.Execute("forcePDVector(" + (i - 1) +
                                                                     ") = sqrt(fPD(1)^2 + fPD(2)^2) * fFFsign;");
                                            _myMatlabWrapper.Execute("forceParaVector(" + (i - 1) +
                                                                     ") = sqrt(fPara(1)^2 + fPara(2)^2) * fParasign;");
                                            _myMatlabWrapper.Execute("forceAbsVector(" + (i - 1) +
                                                                     ") = sqrt(vforce(1,1)^2 + vforce(1,2)^2);");
                                        }

                                        double[,] forcePD = _myMatlabWrapper.GetWorkspaceData("forcePDVector");
                                        double[,] forcePara = _myMatlabWrapper.GetWorkspaceData("forceParaVector");
                                        double[,] forceAbs = _myMatlabWrapper.GetWorkspaceData("forceAbsVector");

                                        for (var i = 0; i < xData.Length; i++)
                                        {
                                            if (i < forcePD.Length)
                                            {
                                                dataFileWriter.WriteLine(studys + ";" + groups + ";" + szenarios + ";" +
                                                                         subjectPIds + ";" + turns + ";" +
                                                                         targetArray[targetCounter] + ";" + trials + ";" +
                                                                         i + ";" +
                                                                         DoubleConverter.ToExactString(
                                                                             forceVectorDataX[i]) + ";" +
                                                                         DoubleConverter.ToExactString(
                                                                             forceVectorDataY[i]) + ";" +
                                                                         DoubleConverter.ToExactString(forcePD[0, i]) +
                                                                         ";" +
                                                                         DoubleConverter.ToExactString(forcePara[0, i]) +
                                                                         ";" +
                                                                         DoubleConverter.ToExactString(forceAbs[0, i]));
                                            }
                                            else
                                            {
                                                dataFileWriter.WriteLine(studys + ";" + groups + ";" + szenarios + ";" +
                                                                         subjectPIds + ";" + turns + ";" +
                                                                         targetArray[targetCounter] + ";" + trials + ";" +
                                                                         i + ";" +
                                                                         DoubleConverter.ToExactString(
                                                                             forceVectorDataX[i]) + ";" +
                                                                         DoubleConverter.ToExactString(
                                                                             forceVectorDataY[i]) + ";;;");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    dataFileWriter.Close();
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                }
                _myManipAnalysisGui.SetProgressBarValue(0);
                _myManipAnalysisGui.WriteProgressInfo("Ready");
                TaskManager.Remove(Task.CurrentId);
            }));
        }

        public void PlotVelocityBaselines(string study, string group, SubjectContainer subject, int[] targets,
            IEnumerable<Trial.TrialTypeEnum> trialTypes, IEnumerable<Trial.ForceFieldTypeEnum> forceFields,
            IEnumerable<Trial.HandednessEnum> handedness)
        {
            TaskManager.PushBack(Task.Factory.StartNew(() =>
            {
                _myMatlabWrapper.CreateVelocityFigure("Velocity baseline plot", 101);

                var baselineFields = Builders<Baseline>.Projection.Include(t => t.ZippedVelocity);
                var baselines =
                    _myDatabaseWrapper.GetBaseline(study, group, subject, targets, trialTypes, forceFields, handedness,
                        baselineFields).ToArray();

                for (var baselineCounter = 0;
                    baselineCounter < baselines.Length & !TaskManager.Cancel;
                    baselineCounter++)
                {
                    baselines[baselineCounter].Velocity =
                        Gzip<List<VelocityContainer>>.DeCompress(baselines[baselineCounter].ZippedVelocity)
                            .OrderBy(t => t.TimeStamp)
                            .ToList();
                    _myMatlabWrapper.SetWorkspaceData("XY",
                        baselines[baselineCounter].Velocity.Select(t => Math.Sqrt(Math.Pow(t.X, 2) + Math.Pow(t.Y, 2)))
                            .ToArray());
                    _myMatlabWrapper.Plot("XY", "black", 2);
                }

                _myMatlabWrapper.ClearWorkspace();
                TaskManager.Remove(Task.CurrentId);
            }));
        }

        public void PlotForceBaselines(string study, string group, SubjectContainer subject, int[] targets,
            IEnumerable<Trial.TrialTypeEnum> trialTypes, IEnumerable<Trial.ForceFieldTypeEnum> forceFields,
            IEnumerable<Trial.HandednessEnum> handedness)
        {
            TaskManager.PushBack(Task.Factory.StartNew(() =>
            {
                _myMatlabWrapper.CreateForceFigure("Force baseline plot", "[Samples]", "Force [N]");

                var baselineFields = Builders<Baseline>.Projection.Include(t => t.ZippedMeasuredForces);
                baselineFields = baselineFields.Include(t2 => t2.ZippedPosition);
                var baselines =
                    _myDatabaseWrapper.GetBaseline(study, group, subject, targets, trialTypes, forceFields, handedness,
                        baselineFields).ToArray();

                for (var baselineCounter = 0;
                    baselineCounter < baselines.Length & !TaskManager.Cancel;
                    baselineCounter++)
                {
                    baselines[baselineCounter].Position =
                        Gzip<List<PositionContainer>>.DeCompress(baselines[baselineCounter].ZippedPosition)
                            .OrderBy(t => t.TimeStamp)
                            .ToList();
                    baselines[baselineCounter].MeasuredForces =
                        Gzip<List<ForceContainer>>.DeCompress(baselines[baselineCounter].ZippedMeasuredForces)
                            .OrderBy(t => t.TimeStamp)
                            .ToList();

                    _myMatlabWrapper.Execute("forcePDVector = zeros(1, " +
                                             (baselines[baselineCounter].Position.Count - 1) + ");");
                    _myMatlabWrapper.Execute("forceParaVector = zeros(1, " +
                                             (baselines[baselineCounter].Position.Count - 1) + ");");
                    _myMatlabWrapper.Execute("forceAbsVector = zeros(1, " +
                                             (baselines[baselineCounter].Position.Count - 1) + ");");

                    for (var i = 2; i <= baselines[baselineCounter].Position.Count & !TaskManager.Pause; i++)
                    {
                        _myMatlabWrapper.SetWorkspaceData("vpos1",
                            new[]
                            {
                                baselines[baselineCounter].Position.Select(t => t.X).ElementAt(i - 2),
                                baselines[baselineCounter].Position.Select(t => t.Y).ElementAt(i - 2)
                            });
                        _myMatlabWrapper.SetWorkspaceData("vpos2",
                            new[]
                            {
                                baselines[baselineCounter].Position.Select(t => t.X).ElementAt(i - 1),
                                baselines[baselineCounter].Position.Select(t => t.Y).ElementAt(i - 1)
                            });
                        _myMatlabWrapper.SetWorkspaceData("vforce",
                            new[]
                            {
                                baselines[baselineCounter].MeasuredForces.Select(t => t.X).ElementAt(i - 2),
                                baselines[baselineCounter].MeasuredForces.Select(t => t.Y).ElementAt(i - 2)
                            });

                        _myMatlabWrapper.Execute(
                            "[fPD, fPDsign, fFFsign] = pdForceDirectionLineSegment([vforce(1,1) vforce(1,2)], [vpos1(1,1) vpos1(1,2)], [vpos2(1,1) vpos2(1,2)], [0 0; 0 0]);");
                        _myMatlabWrapper.Execute(
                            "[fPara, fParasign] = paraForceLineSegment([vforce(1,1) vforce(1,2)], [vpos1(1,1) vpos1(1,2)], [vpos2(1,1) vpos2(1,2)]);");

                        _myMatlabWrapper.Execute("forcePDVector(" + (i - 1) + ") = sqrt(fPD(1)^2 + fPD(2)^2) * fFFsign;");
                        _myMatlabWrapper.Execute("forceParaVector(" + (i - 1) +
                                                 ") = sqrt(fPara(1)^2 + fPara(2)^2) * fParasign;");
                        _myMatlabWrapper.Execute("forceAbsVector(" + (i - 1) +
                                                 ") = sqrt(vforce(1,1)^2 + vforce(1,2)^2);");
                    }

                    _myMatlabWrapper.Plot("forcePDVector", "blue", 2);
                    _myMatlabWrapper.Plot("forceParaVector", "red", 2);
                    _myMatlabWrapper.Plot("forceAbsVector", "black", 2);
                    _myMatlabWrapper.AddLegend("Force PD", "Force Para", "Force Abs");
                }

                _myMatlabWrapper.ClearWorkspace();
                TaskManager.Remove(Task.CurrentId);
            }));
        }

        /// <summary>
        /// This method loops over the selectedTrials and creates a new BaselineObject for each selectedTrial.
        /// The selectedTrials objects can consist of more than 1 Trial but they all belong to the same target, study, group, etc...
        /// </summary>
        /// <param name="selectedTrials">all Trials in the Listbox from the GUI</param>
        /// <param name="trialType">the trialsType specified in the GUI</param>
        /// <param name="forceFieldType">the forceFieldType specified in the GUI</param>
        /// <param name="handedness">the handedness specified in the GUI</param>
        public void RecalculateBaselines(IEnumerable<TrajectoryVelocityPlotContainer> selectedTrials,
            Trial.TrialTypeEnum trialType, Trial.ForceFieldTypeEnum forceFieldType, Trial.HandednessEnum handedness)
        {
            _myManipAnalysisGui.WriteProgressInfo("Recalculating baselines...");

            foreach (var targetData in selectedTrials)
            {
                //All Baselinetrials that match with the selected Trials in the GUI and the selected metadata in the GUI (trialType, FF, handedness)
                var originalBaseline = _myDatabaseWrapper.GetBaseline(targetData.Study, targetData.Group,
                    targetData.Subject, targetData.Target, trialType, forceFieldType, handedness);
                //The TurnDateTime that matches with the selected Trials in the GUI
                var turnDateTime = GetTurnDateTime(targetData.Study, targetData.Group, targetData.Szenario,
                    targetData.Subject, targetData.Turn);

                var fields = Builders<Trial>.Projection.Include(t1 => t1.ZippedPositionNormalized);
                fields = fields.Include(t2 => t2.ZippedVelocityNormalized);
                fields = fields.Include(t3 => t3.ZippedMeasuredForcesNormalized);
                fields = fields.Include(t4 => t4.ZippedMomentForcesNormalized);
                fields = fields.Include(t5 => t5.TrialType);
                fields = fields.Include(t6 => t6.ForceFieldType);
                fields = fields.Include(t7 => t7.Handedness);
                fields = fields.Include(t8 => t8.Study);
                fields = fields.Include(t9 => t9.Group);
                fields = fields.Include(t10 => t10.Subject);
                fields = fields.Include(t11 => t11.TrialNumberInSzenario);
                fields = fields.Include(t12 => t12.Target);
                fields = fields.Include(t13 => t13.Origin);
                fields = fields.Include(t14 => t14.NormalizedDataSampleRate);
                fields = fields.Include(t15 => t15.Id);

                //The trials that were specified in the listbox:
                //As we loop over each listbox entry, a listboxentry can contain multiple trials, as long as they have the same target
                //and the same metadata (subject, group, szenario, study, turndatetime, etc.)
                //Example:
                /*
                We specify listboxentry 1 is 
                Study: MICIE
                Group: blocked_UNmatched
                Szenario: 02_MICIE_baseline_V1
                Subject: MICIE1
                Turn: 1
                Target: Target 01
                Trials: Trials 1 & 3
                --> trials will then contain the trials with TargetTrialNumberInSzenario 1 & 3 and the matching metadata.
                */
                var trials =
                    _myDatabaseWrapper.GetTrials(targetData.Study, targetData.Group, targetData.Szenario,
                        targetData.Subject, turnDateTime, targetData.Target, targetData.Trials, fields).ToList();

                trials.ForEach(
                    t =>
                        t.PositionNormalized =
                            Gzip<List<PositionContainer>>.DeCompress(t.ZippedPositionNormalized)
                                .OrderBy(u => u.TimeStamp)
                                .ToList());
                trials.ForEach(
                    t =>
                        t.VelocityNormalized =
                            Gzip<List<VelocityContainer>>.DeCompress(t.ZippedVelocityNormalized)
                                .OrderBy(u => u.TimeStamp)
                                .ToList());
                trials.ForEach(
                    t =>
                        t.MeasuredForcesNormalized =
                            Gzip<List<ForceContainer>>.DeCompress(t.ZippedMeasuredForcesNormalized)
                                .OrderBy(u => u.TimeStamp)
                                .ToList());
                trials.ForEach(
                    t =>
                        t.MomentForcesNormalized =
                            Gzip<List<ForceContainer>>.DeCompress(t.ZippedMomentForcesNormalized)
                                .OrderBy(u => u.TimeStamp)
                                .ToList());

                //Calculating a new baseline from the ListboxEntry
                var baselines = doBaselineCalculation(trials).ToList();

                if (baselines.Count == 1)
                {
                    //Setting the baselineObjects Metadata to the metadata we specified in the GUI
                    baselines[0].Id = originalBaseline.Id;
                    baselines[0].TrialType = trialType;
                    baselines[0].ForceFieldType = forceFieldType;
                    baselines[0].Handedness = handedness;

                    CompressBaselineData(baselines);

                    _myDatabaseWrapper.UpdateBaseline(baselines[0]);

                    _myDatabaseWrapper.DropStatistics(originalBaseline);
                }
                else
                {
                    _myManipAnalysisGui.WriteToLogBox("Error recalculating baselines.");
                    break;
                }
            }

            _myManipAnalysisGui.WriteProgressInfo("Ready.");
            _myManipAnalysisGui.SetProgressBarValue(0);
        }

        public void FixIndexes()
        {
            _myDatabaseWrapper.DropAllIndexes();
            _myDatabaseWrapper.EnsureIndexes();
            _myDatabaseWrapper.RebuildIndexes();
        }

        public void DropStatistics()
        {
            _myDatabaseWrapper.DropStatistics();
        }
    }
}