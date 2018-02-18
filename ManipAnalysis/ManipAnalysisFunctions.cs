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


                                        case "ForcefieldCompenstionFactor Raw":
                                            statisticData[trialsArrayCounter, meanCount] =
                                                trialsArray[trialsArrayCounter].Statistics
                                                    .ForcefieldCompenstionFactorRaw;
                                            break;

                                            
                                        case "ForcefieldCompenstionFactor Raw fisher-z":
                                            _myMatlabWrapper.SetWorkspaceData("ffcfraw",
                                                trialsArray[trialsArrayCounter].Statistics
                                                    .ForcefieldCompenstionFactorRaw);
                                            _myMatlabWrapper.Execute("fisherZ = fisherZTransform(ffcfraw);");
                                            statisticData[trialsArrayCounter, meanCount] =
                                                _myMatlabWrapper.GetWorkspaceData("fisherZ");
                                            _myMatlabWrapper.ClearWorkspace();
                                            break;

                                            
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
                                
                                if (statisticType == "ForcefieldCompenstionFactor Raw fisher-z to r-values")
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
                                if (statisticType == "ForcefieldCompenstionFactor Raw fisher-z to r-values")
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
                                    
                                    case "MidMovementForce - PD Raw":
                                        statisticData[subjectCounter, trialsArrayCounter] =
                                            trialsArray[trialsArrayCounter].Statistics.PerpendicularMidMovementForceRaw;
                                        break;

                                    case "PD - Abs":
                                        statisticData[subjectCounter, trialsArrayCounter] =
                                            trialsArray[trialsArrayCounter].Statistics.AbsolutePerpendicularDisplacement
                                                .Single(t => t.TimeStamp == msIndex).PerpendicularDisplacement;
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
                                        
                                    case "Enclosed area":
                                        statisticData[subjectCounter, trialsArrayCounter] =
                                            trialsArray[trialsArrayCounter].Statistics.EnclosedArea;
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
                                }
                            }
                        }

                        _myMatlabWrapper.SetWorkspaceData("statisticData", statisticData);
                        if (trialList.Count > 1)
                        {
                            if (statisticType == "ForcefieldCompenstionFactor Raw fisher-z to r-values")
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
                            if (statisticType == "ForcefieldCompenstionFactor Raw fisher-z to r-values")
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

       
        public void ImportMeasureFiles(List<string> measureFilesList, List<string> dtpFilesList, int samplesPerSecond, int butterFilterOrder,
            int butterFilterCutOffPosition, int butterFilterCutOffForce, int percentPeakVelocity,
            int timeNormalizationSamples)
        {
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
                            var myParser = new KinarmMeasureFileParser(_myManipAnalysisGui);

                            _myManipAnalysisGui.WriteProgressInfo("Parsing file...");
                            //In this if-condition the c3d data is parsed from the file filename.
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

                                            
                                            // Matlab statistic calculations
                                            taskMatlabWrapper.Execute(
                                                "enclosed_area = enclosedArea(positionX, positionY);");
                                            taskMatlabWrapper.Execute(
                                                "length_abs = trajectLength(positionX', positionY');");
                                            taskMatlabWrapper.Execute(
                                                "[distanceAbs, distance_sign_pd, distance_sign_ff] = distanceToCurve([positionX' positionY'], startPoint, endPoint, forceFieldMatrix);");
                                            taskMatlabWrapper.Execute("distanceSign = distanceAbs .* distance_sign_ff;");
                                            taskMatlabWrapper.Execute("maxDistanceAbs = max(distanceAbs);");
                                            taskMatlabWrapper.Execute("[~, posDistanceSign] = max(abs(distanceSign));");
                                            taskMatlabWrapper.Execute("maxDistanceSign = distanceSign(posDistanceSign);");
                                            // Create StatisticContainer and fill it with calculated Matlab statistics
                                            var statisticContainer = new StatisticContainer();

                                            
                                            statisticContainer.EnclosedArea =
                                                taskMatlabWrapper.GetWorkspaceData("enclosed_area");
                                            statisticContainer.AbsoluteTrajectoryLength =
                                                taskMatlabWrapper.GetWorkspaceData("length_abs");
                                            statisticContainer.AbsoluteMaximalPerpendicularDisplacement =
                                                taskMatlabWrapper.GetWorkspaceData("maxDistanceAbs");
                                            statisticContainer.SignedMaximalPerpendicularDisplacement =
                                                taskMatlabWrapper.GetWorkspaceData("maxDistanceSign");
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
                                            
                                            var vMaxCorridor =
                                                trial.VelocityNormalized.Where(
                                                    t => (t.TimeStamp - maxVtime).TotalMilliseconds < 70)
                                                    .Select(t => t.TimeStamp)
                                                    .ToList();
                                            
                                            var perpendicularForcesRawMidMovementForce = new List<double>();
                                            
                                            

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
                                                
                                                taskMatlabWrapper.Execute(
                                                    "forcePDRaw = ffSignRaw * sqrt(forcePDRaw(1)^2 + forcePDRaw(2)^2);");
                                                //Keep it!
                                                perpendicularForcesRawForcefieldCompenstionFactor.Add(
                                                    taskMatlabWrapper.GetWorkspaceData("forcePDRaw"));

                                                if (
                                                    vMaxCorridor.Contains(
                                                        trial.PositionNormalized[dataPoint - 2].TimeStamp))
                                                {
                                                    //ForcePDRaw behalten!
                                                    perpendicularForcesRawMidMovementForce.Add(
                                                        taskMatlabWrapper.GetWorkspaceData("forcePDRaw"));
                                                }
                                            }
                                            statisticContainer.PerpendicularMidMovementForceRaw =
                                                perpendicularForcesRawMidMovementForce.Average();
                                            //Keep
                                            taskMatlabWrapper.SetWorkspaceData("forcePDRawArray",
                                                perpendicularForcesRawForcefieldCompenstionFactor.ToArray());
                                            //Keep
                                            taskMatlabWrapper.Execute(
                                                "forceCompFactorRaw = forceCompensationFactor(forcePDRawArray, velocityX, velocityY, forceFieldMatrix);");
                                            statisticContainer.ForcefieldCompenstionFactorRaw =
                                                taskMatlabWrapper.GetWorkspaceData("forceCompFactorRaw");

                                            // Set Metadata and upload to Database
                                            trial.Statistics = statisticContainer;
                                            

                                            CompressTrialData(new List<Trial> { trial });
                                            _myDatabaseWrapper.UpdateTrialStatistics(trial);

                                            _myManipAnalysisGui.SetProgressBarValue(100.0 / trialList.Count * ++counter);
                                        

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
                        //TODO: Improve before using this function!
                        //_myMatlabWrapper.DrawTargets(_myDatabaseWrapper.getTargetContainers(study));

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
                        //TODO: Improve before using this function!
                        //_myMatlabWrapper.DrawTargets(_myDatabaseWrapper.getTargetContainers(study));
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
                        //TODO: Improve before using this function!
                        //_myMatlabWrapper.DrawTargets(_myDatabaseWrapper.getTargetContainers(study));
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

        public void DropStatistics()
        {
            _myDatabaseWrapper.DropStatistics();
        }
    }
}