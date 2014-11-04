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
using MongoDB.Driver.Builders;

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

        public ManipAnalysisFunctions(ManipAnalysisGui myManipAnalysisGui, MatlabWrapper myMatlabWrapper, MongoDbWrapper myDatabaseWrapper)
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
            bool serverAvailable = false;

            using (var tcp = new TcpClient())
            {
                //IAsyncResult ar = tcp.BeginConnect(server, 1433, null, null); // For MS-SQL
                IAsyncResult ar = tcp.BeginConnect(server, 27017, null, null);
                // For MongoDB
                WaitHandle wh = ar.AsyncWaitHandle;
                try
                {
                    if (!ar.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(timeOutMilliSec), false))
                    {
                        throw new Exception();
                    }

                    tcp.EndConnect(ar);
                    serverAvailable = true;
                }
                catch
                {
                    
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
            bool retVal = false;
            int timeOut = 5000;
            if (CheckDatabaseServerAvailability(server, timeOut))
            {
                _myManipAnalysisGui.WriteToLogBox("Connected to Database-Server at \"" + server + "\"");
                _myDatabaseWrapper.SetDatabaseServer(server);
                _myManipAnalysisGui.SetSqlDatabases(_myDatabaseWrapper.GetDatabases());
                retVal = true;
            }
            else
            {
                _myManipAnalysisGui.WriteToLogBox("Database-Server at \"" + server + "\" not reachable! (Timeout: " + timeOut + "ms)");
            }

            return retVal;
        }

        /// <summary>
        ///     Sets a Database in the SQL-Wrapper
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
        /// <param name="szenario"></param>
        /// <returns></returns>
        public IEnumerable<SubjectContainer> GetSubjects(string study, string group)
        {
            return _myDatabaseWrapper.GetSubjects(study, group);
        }

        /// <summary>
        ///     Gets all turns from database of a given study, szenario and subject
        /// </summary>
        /// <param name="study"></param>
        /// <param name="szenario"></param>
        /// <param name="subject"></param>
        /// <returns></returns>
        public IEnumerable<string> GetTurns(string study, string szenario, SubjectContainer subject)
        {
            var turnList = new List<string>();
            int turns = _myDatabaseWrapper.GetTurns(study, szenario, subject).Count();
            for (int turn = 1; turn <= turns; turn++)
            {
                turnList.Add("Turn " + turn);
            }
            return turnList;
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
            int turns = _myDatabaseWrapper.GetTurns(study, group, szenario, subject).Count();
            for (int turn = 1; turn <= turns; turn++)
            {
                turnList.Add("Turn " + turn);
            }
            return turnList;
        }

        /// <summary>
        ///     Gets all targets from database of a given study and szenario
        /// </summary>
        /// <param name="study"></param>
        /// <param name="szenario"></param>
        /// <returns></returns>
        public IEnumerable<string> GetTargets(string study, string szenario)
        {
            return _myDatabaseWrapper.GetTargets(study, szenario).Select(t => "Target " + t.ToString("00"));
        }

        /// <summary>
        ///     Gets all trials from database of a given study and szenario
        /// </summary>
        /// <param name="study"></param>
        /// <param name="szenario"></param>
        /// <returns></returns>
        public IEnumerable<string> GetTrials(string study, string szenario)
        {
            return _myDatabaseWrapper.GetTargetTrials(study, szenario).Select(t => "Trial " + t.ToString("000"));
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

        public void PlotSzenarioMeanTimes(string study, string group, string szenario, SubjectContainer subject, int turn)
        {
            TaskManager.PushBack(Task.Factory.StartNew(() =>
            {
                DateTime turnDateTime = GetTurnDateTime(study, group, szenario, subject, turn);
                SzenarioMeanTime[] szenarioMeanTimes = _myDatabaseWrapper.GetSzenarioMeanTime(study, group, szenario, subject, turnDateTime);

                _myMatlabWrapper.CreateMeanTimeFigure();

                for (int szenarioMeanTimeCounter = 0; szenarioMeanTimeCounter < szenarioMeanTimes.Length & !TaskManager.Cancel; szenarioMeanTimeCounter++)
                {
                    _myMatlabWrapper.SetWorkspaceData("target", szenarioMeanTimes[szenarioMeanTimeCounter].Target.Number);
                    _myMatlabWrapper.SetWorkspaceData("meanTime", szenarioMeanTimes[szenarioMeanTimeCounter].MeanTime.TotalSeconds);
                    _myMatlabWrapper.SetWorkspaceData("meanTimeStd", szenarioMeanTimes[szenarioMeanTimeCounter].MeanTimeStd.TotalSeconds);
                    _myMatlabWrapper.PlotMeanTimeErrorBar("target", "meanTime", "meanTimeStd");
                }

                // Add one more Target (mean) for overall mean value
                _myMatlabWrapper.SetWorkspaceData("target", 17);
                //szenarioMeanTimes.Select(t => t.Target.Number).Max() + 1);
                _myMatlabWrapper.SetWorkspaceData("meanTime", szenarioMeanTimes.Select(t => t.MeanTime.TotalSeconds).Average());
                _myMatlabWrapper.SetWorkspaceData("meanTimeStd", szenarioMeanTimes.Select(t => t.MeanTimeStd.TotalSeconds).Average());
                _myMatlabWrapper.PlotMeanTimeErrorBar("target", "meanTime", "meanTimeStd");

                _myMatlabWrapper.ClearWorkspace();
                TaskManager.Remove(Task.CurrentId);
            }));
        }

        public void ExportSzenarioMeanTimes(string study, string group, string szenario, SubjectContainer subject, int turn, string fileName)
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

        public void PlotTrajectoryBaseline(string study, string group, SubjectContainer subject, int[] targets, IEnumerable<Trial.TrialTypeEnum> trialTypes, IEnumerable<Trial.ForceFieldTypeEnum> forceFields, IEnumerable<Trial.HandednessEnum> handedness)
        {
            TaskManager.PushBack(Task.Factory.StartNew(() =>
            {
                _myMatlabWrapper.CreateTrajectoryFigure("Trajectory baseline plot");
                _myMatlabWrapper.DrawTargets(0.003, 0.1, 0, 0);

                var baselineFields = new FieldsBuilder<Baseline>();
                baselineFields.Include(t => t.ZippedPosition);
                Baseline[] baselines = _myDatabaseWrapper.GetBaseline(study, group, subject, targets, trialTypes, forceFields, handedness, baselineFields);

                for (int baselineCounter = 0; baselineCounter < baselines.Length & !TaskManager.Cancel; baselineCounter++)
                {
                    baselines[baselineCounter].Position = Gzip<List<PositionContainer>>.DeCompress(baselines[baselineCounter].ZippedPosition).OrderBy(t => t.TimeStamp).ToList();
                    _myMatlabWrapper.SetWorkspaceData("X", baselines[baselineCounter].Position.Select(u => u.X).ToArray());
                    _myMatlabWrapper.SetWorkspaceData("Y", baselines[baselineCounter].Position.Select(u => u.Y).ToArray());
                    _myMatlabWrapper.Plot("X", "Y", "black", 2);
                }

                _myMatlabWrapper.ClearWorkspace();
                TaskManager.Remove(Task.CurrentId);
            }));
        }

        public IEnumerable<string> GetTrialsOfSzenario(string study, string szenario, IEnumerable<Trial.TrialTypeEnum> trialTypes, IEnumerable<Trial.ForceFieldTypeEnum> forceFields, IEnumerable<Trial.HandednessEnum> handedness)
        {
            return _myDatabaseWrapper.GetSzenarioTrials(study, szenario, trialTypes, forceFields, handedness).Select(t => "Trial " + t.ToString("000"));
        }

        public void PlotExportDescriptiveStatistic1(IEnumerable<StatisticPlotContainer> selectedTrials, string statisticType, string fitEquation, int pdTime, bool plotFit, bool plotErrorbars, string fileName)
        {
            TaskManager.PushBack(Task.Factory.StartNew(() =>
            {
                try
                {
                    _myManipAnalysisGui.WriteProgressInfo("Getting data...");
                    List<StatisticPlotContainer> selectedTrialsList = selectedTrials.ToList();
                    double sumOfAllTrials = selectedTrialsList.Sum(t => t.Trials.Count);
                    double processedTrialsCount = 0;

                    var fields = new FieldsBuilder<Trial>();
                    fields.Include(t => t.ZippedStatistics);
                    fields.Include(t => t.TrialNumberInSzenario);
                    // Neccessary for sorting!

                    if (selectedTrialsList.Any())
                    {
                        List<int> trialList = selectedTrialsList.ElementAt(0).Trials;

                        if (selectedTrialsList.Any(temp => !trialList.SequenceEqual(temp.Trials)))
                        {
                            _myManipAnalysisGui.WriteToLogBox("Trial selections are not equal!");
                        }
                        else
                        {
                            int meanCount = 0;
                            var statisticData = new double[trialList.Count, selectedTrialsList.Count()];
                            for (meanCount = 0; meanCount < selectedTrialsList.Count() & !TaskManager.Cancel; meanCount++)
                            {
                                StatisticPlotContainer tempStatisticPlotContainer = selectedTrialsList.ElementAt(meanCount);

                                DateTime turnDateTime = GetTurnDateTime(tempStatisticPlotContainer.Study, tempStatisticPlotContainer.Group, tempStatisticPlotContainer.Szenario, tempStatisticPlotContainer.Subject, Convert.ToInt32(tempStatisticPlotContainer.Turn.Substring("Turn".Length)));

                                Trial[] trialsArray = _myDatabaseWrapper.GetTrials(tempStatisticPlotContainer.Study, tempStatisticPlotContainer.Group, tempStatisticPlotContainer.Szenario, tempStatisticPlotContainer.Subject, turnDateTime, trialList, fields).ToArray();

                                for (int trialsArrayCounter = 0; trialsArrayCounter < trialList.Count & !TaskManager.Cancel; trialsArrayCounter++)
                                {
                                    _myManipAnalysisGui.SetProgressBarValue((100.0/sumOfAllTrials)*processedTrialsCount++);

                                    trialsArray[trialsArrayCounter].Statistics = Gzip<StatisticContainer>.DeCompress(trialsArray[trialsArrayCounter].ZippedStatistics);


                                    long pdTimeTick = trialsArray[trialsArrayCounter].Statistics.SignedPerpendicularDisplacement.Min(t => t.TimeStamp).Ticks + TimeSpan.FromMilliseconds(pdTime).Ticks;

                                    DateTime msIndex = trialsArray[trialsArrayCounter].Statistics.SignedPerpendicularDisplacement.Select(t => t.TimeStamp).OrderBy(t => Math.Abs(t.Ticks - pdTimeTick)).ElementAt(0);

                                    if (pdTimeTick > trialsArray[trialsArrayCounter].Statistics.SignedPerpendicularDisplacement.Max(t => t.TimeStamp).Ticks)
                                    {
                                        _myManipAnalysisGui.WriteToLogBox("Warning! Selected PD-Time is larger then movement time! [" + tempStatisticPlotContainer.Study + " - " + tempStatisticPlotContainer.Group + " - " + tempStatisticPlotContainer.Szenario + " - " + tempStatisticPlotContainer.Subject + " - " + tempStatisticPlotContainer.Turn + " - Trial " + trialsArray[trialsArrayCounter].TrialNumberInSzenario + "]");
                                    }

                                    switch (statisticType)
                                    {
                                        case "Vector correlation":
                                            statisticData[trialsArrayCounter, meanCount] = trialsArray[trialsArrayCounter].Statistics.VelocityVectorCorrelation;
                                            break;

                                        case "Vector correlation fisher-z":
                                            _myMatlabWrapper.SetWorkspaceData("vcorr", trialsArray[trialsArrayCounter].Statistics.VelocityVectorCorrelation);
                                            _myMatlabWrapper.Execute("fisherZ = vectorCorrelationFisherZTransform(vcorr);");
                                            statisticData[trialsArrayCounter, meanCount] = _myMatlabWrapper.GetWorkspaceData("fisherZ");
                                            _myMatlabWrapper.ClearWorkspace();
                                            break;

                                        case "Vector correlation fisher-z to r-values":
                                            _myMatlabWrapper.SetWorkspaceData("vcorr", trialsArray[trialsArrayCounter].Statistics.VelocityVectorCorrelation);
                                            _myMatlabWrapper.Execute("fisherZ = vectorCorrelationFisherZTransform(vcorr);");
                                            statisticData[trialsArrayCounter, meanCount] = _myMatlabWrapper.GetWorkspaceData("fisherZ");
                                            _myMatlabWrapper.ClearWorkspace();
                                            break;

                                        case "MidMovementForce - PD":
                                            statisticData[trialsArrayCounter, meanCount] = trialsArray[trialsArrayCounter].Statistics.PerpendicularMidMovementForce;
                                            break;

                                        case "MidMovementForce - PD Raw":
                                            statisticData[trialsArrayCounter, meanCount] = trialsArray[trialsArrayCounter].Statistics.PerpendicularMidMovementForceRaw;
                                            break;

                                        case "MidMovementForce - Para":
                                            statisticData[trialsArrayCounter, meanCount] = trialsArray[trialsArrayCounter].Statistics.ParallelMidMovementForce;
                                            break;

                                        case "MidMovementForce - Abs":
                                            statisticData[trialsArrayCounter, meanCount] = trialsArray[trialsArrayCounter].Statistics.AbsoluteMidMovementForce;
                                            break;

                                        case "PD - Abs":
                                            statisticData[trialsArrayCounter, meanCount] = trialsArray[trialsArrayCounter].Statistics.AbsolutePerpendicularDisplacement.Single(t => t.TimeStamp == msIndex).PerpendicularDisplacement;
                                            break;

                                        case "PDmean - Abs":
                                            statisticData[trialsArrayCounter, meanCount] = trialsArray[trialsArrayCounter].Statistics.AbsoluteMeanPerpendicularDisplacement;
                                            break;

                                        case "PDmax - Abs":
                                            statisticData[trialsArrayCounter, meanCount] = trialsArray[trialsArrayCounter].Statistics.AbsoluteMaximalPerpendicularDisplacement;
                                            break;

                                        case "PDVmax - Abs":
                                            statisticData[trialsArrayCounter, meanCount] = trialsArray[trialsArrayCounter].Statistics.AbsoluteMaximalPerpendicularDisplacementVmax;
                                            break;

                                        case "PD - Sign":
                                            statisticData[trialsArrayCounter, meanCount] = trialsArray[trialsArrayCounter].Statistics.SignedPerpendicularDisplacement.Single(t => t.TimeStamp == msIndex).PerpendicularDisplacement;
                                            break;

                                        case "PDmax - Sign":
                                            statisticData[trialsArrayCounter, meanCount] = trialsArray[trialsArrayCounter].Statistics.SignedMaximalPerpendicularDisplacement;
                                            break;

                                        case "PDVmax - Sign":
                                            statisticData[trialsArrayCounter, meanCount] = trialsArray[trialsArrayCounter].Statistics.SignedMaximalPerpendicularDisplacementVmax;
                                            break;

                                        case "Trajectory length abs":
                                            statisticData[trialsArrayCounter, meanCount] = trialsArray[trialsArrayCounter].Statistics.AbsoluteTrajectoryLength;
                                            break;

                                        case "Trajectory length ratio":
                                            statisticData[trialsArrayCounter, meanCount] = trialsArray[trialsArrayCounter].Statistics.AbsoluteBaselineTrajectoryLengthRatio;
                                            break;

                                        case "Enclosed area":
                                            statisticData[trialsArrayCounter, meanCount] = trialsArray[trialsArrayCounter].Statistics.EnclosedArea;
                                            break;

                                        case "ForcefieldCompenstionFactor":
                                            statisticData[trialsArrayCounter, meanCount] = trialsArray[trialsArrayCounter].Statistics.ForcefieldCompenstionFactor;
                                            break;

                                        case "ForcefieldCompenstionFactor Raw":
                                            statisticData[trialsArrayCounter, meanCount] = trialsArray[trialsArrayCounter].Statistics.ForcefieldCompenstionFactorRaw;
                                            break;

                                        case "RMSE":
                                            statisticData[trialsArrayCounter, meanCount] = trialsArray[trialsArrayCounter].Statistics.RMSE;
                                            break;
                                    }
                                }
                            }

                            _myMatlabWrapper.SetWorkspaceData("statisticData", statisticData);
                            if (meanCount > 1)
                            {
                                if (statisticType == "Vector correlation fisher-z to r-values")
                                {
                                    _myMatlabWrapper.Execute("statisticDataPlot = fisherZVectorCorrelationTransform(mean(transpose(statisticData)));");
                                    _myMatlabWrapper.Execute("statisticDataStd = fisherZVectorCorrelationTransform(std(transpose(statisticData)));");
                                }
                                else
                                {
                                    _myMatlabWrapper.Execute("statisticDataPlot = mean(transpose(statisticData));");
                                    _myMatlabWrapper.Execute("statisticDataStd = std(transpose(statisticData));");
                                }
                            }
                            else
                            {
                                if (statisticType == "Vector correlation fisher-z to r-values")
                                {
                                    _myMatlabWrapper.Execute("statisticDataPlot = fisherZVectorCorrelationTransform(statisticData);");
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
                                    case "Vector correlation":
                                        _myMatlabWrapper.CreateStatisticFigure("Velocity Vector Correlation plot", "statisticDataPlot", "fit(transpose([1:1:length(statisticDataPlot)]),transpose(statisticDataPlot),'" + fitEquation + "')", "statisticDataStd", "[Trial]", "Velocity Vector Correlation", 1, (statisticData.Length/meanCount), 0.5, 1, plotFit, plotErrorbars);
                                        break;

                                    case "Vector correlation fisher-z":
                                        _myMatlabWrapper.CreateStatisticFigure("Velocity Vector Correlation Fisher Z plot", "statisticDataPlot", "fit(transpose([1:1:length(statisticDataPlot)]),transpose(statisticDataPlot),'" + fitEquation + "')", "statisticDataStd", "[Trial]", "Velocity Vector Correlation Fisher Z", 1, (statisticData.Length/meanCount), 0.0, 2.0, plotFit, plotErrorbars);
                                        break;

                                    case "Vector correlation fisher-z to r-values":
                                        _myMatlabWrapper.CreateStatisticFigure("Velocity Vector Correlation Fisher Z to r-Values  plot", "statisticDataPlot", "fit(transpose([1:1:length(statisticDataPlot)]),transpose(statisticDataPlot),'" + fitEquation + "')", "statisticDataStd", "[Trial]", "Velocity Vector Correlation Fisher Z", 1, (statisticData.Length/meanCount), 0.5, 1, plotFit, plotErrorbars);
                                        break;

                                    case "MidMovementForce - PD":
                                        _myMatlabWrapper.CreateStatisticFigure("MidMovementForce PD plot", "statisticDataPlot", "fit(transpose([1:1:length(statisticDataPlot)]),transpose(statisticDataPlot),'" + fitEquation + "')", "statisticDataStd", "[Trial]", "Newton [N]", 1, (statisticData.Length/meanCount), -3.0, 3.0, plotFit, plotErrorbars);
                                        break;

                                    case "MidMovementForce - PD Raw":
                                        _myMatlabWrapper.CreateStatisticFigure("MidMovementForce PD Raw plot", "statisticDataPlot", "fit(transpose([1:1:length(statisticDataPlot)]),transpose(statisticDataPlot),'" + fitEquation + "')", "statisticDataStd", "[Trial]", "Newton [N]", 1, (statisticData.Length/meanCount), -3.0, 3.0, plotFit, plotErrorbars);
                                        break;

                                    case "MidMovementForce - Para":
                                        _myMatlabWrapper.CreateStatisticFigure("MidMovementForce Para plot", "statisticDataPlot", "fit(transpose([1:1:length(statisticDataPlot)]),transpose(statisticDataPlot),'" + fitEquation + "')", "statisticDataStd", "[Trial]", "Newton [N]", 1, (statisticData.Length/meanCount), -3.0, 3.0, plotFit, plotErrorbars);
                                        break;

                                    case "MidMovementForce - Abs":
                                        _myMatlabWrapper.CreateStatisticFigure("MidMovementForce Abs plot", "statisticDataPlot", "fit(transpose([1:1:length(statisticDataPlot)]),transpose(statisticDataPlot),'" + fitEquation + "')", "statisticDataStd", "[Trial]", "Newton [N]", 1, (statisticData.Length/meanCount), -3.0, 3.0, plotFit, plotErrorbars);
                                        break;

                                    case "PD - Abs":
                                        _myMatlabWrapper.CreateStatisticFigure("PD" + pdTime + " abs plot", "statisticDataPlot", "fit(transpose([1:1:length(statisticDataPlot)]),transpose(statisticDataPlot),'" + fitEquation + "')", "statisticDataStd", "[Trial]", "PD" + pdTime + " [m]", 1, (statisticData.Length/meanCount), 0, 0.05, plotFit, plotErrorbars);
                                        break;

                                    case "PDmean - Abs":
                                        _myMatlabWrapper.CreateStatisticFigure("MeanPD abs plot", "statisticDataPlot", "fit(transpose([1:1:length(statisticDataPlot)]),transpose(statisticDataPlot),'" + fitEquation + "')", "statisticDataStd", "[Trial]", "MeanPD [m]", 1, (statisticData.Length/meanCount), 0, 0.05, plotFit, plotErrorbars);
                                        break;

                                    case "PDmax - Abs":
                                        _myMatlabWrapper.CreateStatisticFigure("MaxPD abs plot", "statisticDataPlot", "fit(transpose([1:1:length(statisticDataPlot)]),transpose(statisticDataPlot),'" + fitEquation + "')", "statisticDataStd", "[Trial]", "MaxPD [m]", 1, (statisticData.Length/meanCount), 0, 0.05, plotFit, plotErrorbars);
                                        break;

                                    case "PDVmax - Abs":
                                        _myMatlabWrapper.CreateStatisticFigure("VmaxPD abs plot", "statisticDataPlot", "fit(transpose([1:1:length(statisticDataPlot)]),transpose(statisticDataPlot),'" + fitEquation + "')", "statisticDataStd", "[Trial]", "MaxPD [m]", 1, (statisticData.Length/meanCount), 0, 0.05, plotFit, plotErrorbars);
                                        break;

                                    case "PD - Sign":
                                        _myMatlabWrapper.CreateStatisticFigure("PD" + pdTime + " sign plot", "statisticDataPlot", "fit(transpose([1:1:length(statisticDataPlot)]),transpose(statisticDataPlot),'" + fitEquation + "')", "statisticDataStd", "[Trial]", "PD" + pdTime + " [m]", 1, (statisticData.Length/meanCount), -0.05, 0.05, plotFit, plotErrorbars);
                                        break;

                                    case "PDmax - Sign":
                                        _myMatlabWrapper.CreateStatisticFigure("MaxPD sign plot", "statisticDataPlot", "fit(transpose([1:1:length(statisticDataPlot)]),transpose(statisticDataPlot),'" + fitEquation + "')", "statisticDataStd", "[Trial]", "MaxPD [m]", 1, (statisticData.Length/meanCount), -0.05, 0.05, plotFit, plotErrorbars);
                                        break;

                                    case "PDVmax - Sign":
                                        _myMatlabWrapper.CreateStatisticFigure("VmaxPD sign plot", "statisticDataPlot", "fit(transpose([1:1:length(statisticDataPlot)]),transpose(statisticDataPlot),'" + fitEquation + "')", "statisticDataStd", "[Trial]", "MaxPD [m]", 1, (statisticData.Length/meanCount), -0.05, 0.05, plotFit, plotErrorbars);
                                        break;

                                    case "Trajectory length abs":
                                        _myMatlabWrapper.CreateStatisticFigure("Trajectory Length plot", "statisticDataPlot", "fit(transpose([1:1:length(statisticDataPlot)]),transpose(statisticDataPlot),'" + fitEquation + "')", "statisticDataStd", "[Trial]", "Trajectory Length [m]", 1, (statisticData.Length/meanCount), 0.07, 0.2, plotFit, plotErrorbars);
                                        break;

                                    case "Trajectory length ratio":
                                        _myMatlabWrapper.CreateStatisticFigure("Trajectory Length Ratio plot", "statisticDataPlot", "fit(transpose([1:1:length(statisticDataPlot)]),transpose(statisticDataPlot),'" + fitEquation + "')", "statisticDataStd", "[Trial]", "Trajectory Length Ratio", 1, (statisticData.Length/meanCount), 0.2, 1.8, plotFit, plotErrorbars);
                                        break;

                                    case "Enclosed area":
                                        _myMatlabWrapper.CreateStatisticFigure("Enclosed area plot", "statisticDataPlot", "fit(transpose([1:1:length(statisticDataPlot)]),transpose(statisticDataPlot),'" + fitEquation + "')", "statisticDataStd", "[Trial]", "Enclosed Area [m²]", 1, (statisticData.Length/meanCount), 0, 0.002, plotFit, plotErrorbars);
                                        break;

                                    case "ForcefieldCompenstionFactor":
                                        _myMatlabWrapper.CreateStatisticFigure("Forcefield Compenstion Factor plot", "statisticDataPlot", "fit(transpose([1:1:length(statisticDataPlot)]),transpose(statisticDataPlot),'" + fitEquation + "')", "statisticDataStd", "[Trial]", "Forcefield Compenstion Factor", 1, (statisticData.Length / meanCount), -1.0, 1.0, plotFit, plotErrorbars);
                                        break;

                                    case "ForcefieldCompenstionFactor Raw":
                                        _myMatlabWrapper.CreateStatisticFigure("Forcefield Compenstion Factor Raw plot", "statisticDataPlot", "fit(transpose([1:1:length(statisticDataPlot)]),transpose(statisticDataPlot),'" + fitEquation + "')", "statisticDataStd", "[Trial]", "Forcefield Compenstion Factor", 1, (statisticData.Length / meanCount), -1.0, 1.0, plotFit, plotErrorbars);
                                        break;

                                    case "RMSE":
                                        _myMatlabWrapper.CreateStatisticFigure("Root Mean Square Error plot", "statisticDataPlot", "fit(transpose([1:1:length(statisticDataPlot)]),transpose(statisticDataPlot),'" + fitEquation + "')", "statisticDataStd", "[Trial]", "Root Mean Square Error", 1, (statisticData.Length/meanCount), 0, 0.1, plotFit, plotErrorbars);

                                        break;
                                }
                            }
                            else
                            {
                                var cache = new List<string>();
                                var meanDataFileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                                var meanDataFileWriter = new StreamWriter(meanDataFileStream);
                                string personNames = "";

                                double[,] dataMean = null;
                                double[,] dataStd = null;
                                if (meanCount > 1)
                                {
                                    dataMean = _myMatlabWrapper.GetWorkspaceData("statisticDataPlot");
                                    dataStd = _myMatlabWrapper.GetWorkspaceData("statisticDataStd");
                                }


                                for (int i = 0; i < selectedTrialsList.Count() & !TaskManager.Cancel; i++)
                                {
                                    StatisticPlotContainer tempStatisticPlotContainer = selectedTrialsList.ElementAt(i);

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

                                for (int trialListCounter = 0; trialListCounter < trialList.Count & !TaskManager.Cancel; trialListCounter++)
                                {
                                    string tempLine = trialList.ElementAt(trialListCounter) + ";";

                                    for (int meanCounter = 0; meanCounter < meanCount; meanCounter++)
                                    {
                                        tempLine += DoubleConverter.ToExactString(statisticData[trialListCounter, meanCounter]) + ";";
                                    }

                                    if (meanCount > 1)
                                    {
                                        tempLine += DoubleConverter.ToExactString(dataMean[0, trialListCounter]) + ";" + DoubleConverter.ToExactString(dataStd[0, trialListCounter]);
                                    }
                                    else
                                    {
                                        tempLine += "0.0;0.0";
                                    }
                                    cache.Add(tempLine);
                                }

                                for (int i = 0; i < cache.Count() & !TaskManager.Cancel; i++)
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

        public void ExportDescriptiveStatistic2Data(IEnumerable<StatisticPlotContainer> selectedTrials, string statisticType, string fileName, int pdTime)
        {
            TaskManager.PushBack(Task.Factory.StartNew(() =>
            {
                _myManipAnalysisGui.WriteProgressInfo("Getting data...");
                List<StatisticPlotContainer> selectedTrialsList = selectedTrials.ToList();

                var fields = new FieldsBuilder<Trial>();
                fields.Include(t => t.ZippedStatistics);
                fields.Include(t => t.TrialNumberInSzenario);
                // Neccessary for sorting!

                if (selectedTrialsList.Any())
                {
                    List<int> trialList = selectedTrialsList.ElementAt(0).Trials;

                    if (selectedTrialsList.Any(temp => !trialList.SequenceEqual(temp.Trials)))
                    {
                        _myManipAnalysisGui.WriteToLogBox("Trial selections are not equal!");
                    }
                    else
                    {
                        var statisticData = new double[selectedTrialsList.Count(), trialList.Count];

                        int subjectCounter = 0;
                        for (subjectCounter = 0; subjectCounter < selectedTrialsList.Count & !TaskManager.Cancel; subjectCounter++)
                        {
                            StatisticPlotContainer tempStatisticPlotContainer = selectedTrialsList.ElementAt(subjectCounter);

                            DateTime turnDateTime = GetTurnDateTime(tempStatisticPlotContainer.Study, tempStatisticPlotContainer.Group, tempStatisticPlotContainer.Szenario, tempStatisticPlotContainer.Subject, Convert.ToInt32(tempStatisticPlotContainer.Turn.Substring("Turn".Length)));

                            Trial[] trialsArray = _myDatabaseWrapper.GetTrials(tempStatisticPlotContainer.Study, tempStatisticPlotContainer.Group, tempStatisticPlotContainer.Szenario, tempStatisticPlotContainer.Subject, turnDateTime, trialList, fields).ToArray();

                            for (int trialsArrayCounter = 0; trialsArrayCounter < trialList.Count & !TaskManager.Cancel; trialsArrayCounter++)
                            {
                                _myManipAnalysisGui.SetProgressBarValue((100.0/selectedTrialsList.Count)*subjectCounter);

                                trialsArray[trialsArrayCounter].Statistics = Gzip<StatisticContainer>.DeCompress(trialsArray[trialsArrayCounter].ZippedStatistics);


                                long pdTimeTick = trialsArray[trialsArrayCounter].Statistics.SignedPerpendicularDisplacement.Min(t => t.TimeStamp).Ticks + TimeSpan.FromMilliseconds(pdTime).Ticks;

                                DateTime msIndex = trialsArray[trialsArrayCounter].Statistics.SignedPerpendicularDisplacement.Select(t => t.TimeStamp).OrderBy(t => Math.Abs(t.Ticks - pdTimeTick)).ElementAt(0);

                                if (pdTimeTick > trialsArray[trialsArrayCounter].Statistics.SignedPerpendicularDisplacement.Max(t => t.TimeStamp).Ticks)
                                {
                                    _myManipAnalysisGui.WriteToLogBox("Warning! Selected PD-Time is larger then movement time! [" + tempStatisticPlotContainer.Study + " - " + tempStatisticPlotContainer.Group + " - " + tempStatisticPlotContainer.Szenario + " - " + tempStatisticPlotContainer.Subject + " - " + tempStatisticPlotContainer.Turn + " - Trial " + trialsArray[trialsArrayCounter].TrialNumberInSzenario + "]");
                                }

                                switch (statisticType)
                                {
                                    case "Vector correlation":
                                        statisticData[subjectCounter, trialsArrayCounter] = trialsArray[trialsArrayCounter].Statistics.VelocityVectorCorrelation;
                                        break;

                                    case "Vector correlation fisher-z":
                                        _myMatlabWrapper.SetWorkspaceData("vcorr", trialsArray[trialsArrayCounter].Statistics.VelocityVectorCorrelation);
                                        _myMatlabWrapper.Execute("fisherZ = vectorCorrelationFisherZTransform(vcorr);");
                                        statisticData[subjectCounter, trialsArrayCounter] = _myMatlabWrapper.GetWorkspaceData("fisherZ");
                                        _myMatlabWrapper.ClearWorkspace();
                                        break;

                                    case "Vector correlation fisher-z to r-values":
                                        _myMatlabWrapper.SetWorkspaceData("vcorr", trialsArray[trialsArrayCounter].Statistics.VelocityVectorCorrelation);
                                        _myMatlabWrapper.Execute("fisherZ = vectorCorrelationFisherZTransform(vcorr);");
                                        statisticData[subjectCounter, trialsArrayCounter] = _myMatlabWrapper.GetWorkspaceData("fisherZ");
                                        _myMatlabWrapper.ClearWorkspace();
                                        break;

                                    case "MidMovementForce - PD":
                                        statisticData[subjectCounter, trialsArrayCounter] = trialsArray[trialsArrayCounter].Statistics.PerpendicularMidMovementForce;
                                        break;

                                    case "MidMovementForce - PD Raw":
                                        statisticData[subjectCounter, trialsArrayCounter] = trialsArray[trialsArrayCounter].Statistics.PerpendicularMidMovementForceRaw;
                                        break;

                                    case "MidMovementForce - Para":
                                        statisticData[subjectCounter, trialsArrayCounter] = trialsArray[trialsArrayCounter].Statistics.ParallelMidMovementForce;
                                        break;

                                    case "MidMovementForce - Abs":
                                        statisticData[subjectCounter, trialsArrayCounter] = trialsArray[trialsArrayCounter].Statistics.AbsoluteMidMovementForce;
                                        break;

                                    case "PD - Abs":
                                        statisticData[subjectCounter, trialsArrayCounter] = trialsArray[trialsArrayCounter].Statistics.AbsolutePerpendicularDisplacement.Single(t => t.TimeStamp == msIndex).PerpendicularDisplacement;
                                        break;

                                    case "PDmean - Abs":
                                        statisticData[subjectCounter, trialsArrayCounter] = trialsArray[trialsArrayCounter].Statistics.AbsoluteMeanPerpendicularDisplacement;
                                        break;

                                    case "PDmax - Abs":
                                        statisticData[subjectCounter, trialsArrayCounter] = trialsArray[trialsArrayCounter].Statistics.AbsoluteMaximalPerpendicularDisplacement;
                                        break;

                                    case "PDVmax - Abs":
                                        statisticData[subjectCounter, trialsArrayCounter] = trialsArray[trialsArrayCounter].Statistics.AbsoluteMaximalPerpendicularDisplacementVmax;
                                        break;

                                    case "PD - Sign":
                                        statisticData[subjectCounter, trialsArrayCounter] = trialsArray[trialsArrayCounter].Statistics.SignedPerpendicularDisplacement.Single(t => t.TimeStamp == msIndex).PerpendicularDisplacement;
                                        break;

                                    case "PDmax - Sign":
                                        statisticData[subjectCounter, trialsArrayCounter] = trialsArray[trialsArrayCounter].Statistics.SignedMaximalPerpendicularDisplacement;
                                        break;

                                    case "PDVmax - Sign":
                                        statisticData[subjectCounter, trialsArrayCounter] = trialsArray[trialsArrayCounter].Statistics.SignedMaximalPerpendicularDisplacementVmax;
                                        break;

                                    case "Trajectory length abs":
                                        statisticData[subjectCounter, trialsArrayCounter] = trialsArray[trialsArrayCounter].Statistics.AbsoluteTrajectoryLength;
                                        break;

                                    case "Trajectory length ratio":
                                        statisticData[subjectCounter, trialsArrayCounter] = trialsArray[trialsArrayCounter].Statistics.AbsoluteBaselineTrajectoryLengthRatio;
                                        break;

                                    case "Enclosed area":
                                        statisticData[subjectCounter, trialsArrayCounter] = trialsArray[trialsArrayCounter].Statistics.EnclosedArea;
                                        break;

                                    case "ForcefieldCompenstionFactor":
                                        statisticData[subjectCounter, trialsArrayCounter] = trialsArray[trialsArrayCounter].Statistics.ForcefieldCompenstionFactor;
                                        break;

                                    case "ForcefieldCompenstionFactor Raw":
                                        statisticData[subjectCounter, trialsArrayCounter] = trialsArray[trialsArrayCounter].Statistics.ForcefieldCompenstionFactorRaw;
                                        break;

                                    case "RMSE":
                                        statisticData[subjectCounter, trialsArrayCounter] = trialsArray[trialsArrayCounter].Statistics.RMSE;
                                        break;
                                }
                            }
                        }

                        _myMatlabWrapper.SetWorkspaceData("statisticData", statisticData);
                        if (trialList.Count > 1)
                        {
                            if (statisticType == "Vector correlation fisher-z to r-values")
                            {
                                _myMatlabWrapper.Execute("statisticDataMean = transpose(fisherZVectorCorrelationTransform(mean(transpose(statisticData))));");
                                _myMatlabWrapper.Execute("statisticDataStd = transpose(fisherZVectorCorrelationTransform(std(transpose(statisticData))));");
                            }
                            else
                            {
                                _myMatlabWrapper.Execute("statisticDataMean = transpose(mean(transpose(statisticData)));");
                                _myMatlabWrapper.Execute("statisticDataStd = transpose(std(transpose(statisticData)));");
                            }
                        }
                        else
                        {
                            if (statisticType == "Vector correlation fisher-z to r-values")
                            {
                                _myMatlabWrapper.Execute("statisticDataMean = fisherZVectorCorrelationTransform(statisticData);");
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


                        for (int i = 0; i < selectedTrialsList.Count & !TaskManager.Cancel; i++)
                        {
                            StatisticPlotContainer tempStatisticPlotContainer = selectedTrialsList.ElementAt(i);
                            string meanValue = DoubleConverter.ToExactString(dataMean[i, 0]);
                            string stdValue = dataStd == null ? "" : DoubleConverter.ToExactString(dataStd[i, 0]);

                            cache.Add(tempStatisticPlotContainer.Study + ";" + tempStatisticPlotContainer.Group + ";" + tempStatisticPlotContainer.Szenario + ";" + tempStatisticPlotContainer.Subject + ";" + tempStatisticPlotContainer.Turn + ";" + tempStatisticPlotContainer.GetTrialsString() + ";" + meanValue + ";" + stdValue);
                        }

                        for (int i = 0; i < cache.Count() & !TaskManager.Cancel; i++)
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

        public void ImportMeasureFiles(List<string> measureFilesList, int samplesPerSecond, int butterFilterOrder, int butterFilterCutOffPosition, int butterFilterCutOffForce, int percentPeakVelocity, int timeNormalizationSamples)
        {
            TaskManager.PushBack(Task.Factory.StartNew(delegate
            {
                while (TaskManager.GetIndex(Task.CurrentId) != 0 & !TaskManager.Cancel)
                {
                    Thread.Sleep(100);
                }

                _myManipAnalysisGui.EnableTabPages(false);
                _myManipAnalysisGui.SetProgressBarValue(0);

                int cpuCount = Environment.ProcessorCount;
                var taskMatlabWrappers = new List<MatlabWrapper>();
                for (int i = 0; i < cpuCount; i++)
                {
                    taskMatlabWrappers.Add(new MatlabWrapper(_myManipAnalysisGui, MatlabWrapper.MatlabInstanceType.Single));
                }

                for (int files = 0; files < measureFilesList.Count & !TaskManager.Cancel; files++)
                {
                    try
                    {
                        while (TaskManager.Pause & !TaskManager.Cancel)
                        {
                            Thread.Sleep(100);
                        }

                        _myManipAnalysisGui.SetProgressBarValue((100.0/measureFilesList.Count)*files);

                        string filename = measureFilesList.ElementAt(files);

                        string tempFileHash = Md5.ComputeHash(filename);

                        if (!_myDatabaseWrapper.CheckIfMeasureFileHashExists(tempFileHash))
                        {
                            //var myParser = new BioMotionBotMeasureFileParser(trialsContainer, _myManipAnalysisGui);
                            var myParser = new KinarmMeasureFileParser(_myManipAnalysisGui);

                            _myManipAnalysisGui.WriteProgressInfo("Parsing file...");
                            if (myParser.ParseFile(filename) && myParser.TrialsContainer.Count > 0)
                            {
                                List<Trial> trialsContainer = myParser.TrialsContainer;
                                List<SzenarioMeanTime> szenarioMeanTimesContainer;

                                var taskTrialListParts = new List<List<Trial>>();
                                int threadCount = 0;

                                if (trialsContainer.Count > cpuCount)
                                {
                                    for (int cpuCounter = 0; cpuCounter < cpuCount; cpuCounter++)
                                    {
                                        taskTrialListParts.Add(new List<Trial>());
                                    }

                                    int trialCounter = 0;
                                    int listCounter = 0;
                                    while (trialCounter < trialsContainer.Count)
                                    {
                                        taskTrialListParts[listCounter].Add(trialsContainer[trialCounter]);
                                        trialCounter
                                            ++;
                                        listCounter
                                            ++;
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

                                for (int i = 0; i < threadCount; i++)
                                {
                                    List<Trial> tempTaskTrialList = taskTrialListParts.ElementAt(i).ToList();
                                    MatlabWrapper tempMatlabWrapper = taskMatlabWrappers.ElementAt(i);

                                    _myManipAnalysisGui.WriteProgressInfo("Processing data...");
                                    calculatingTasks.Add(Task.Factory.StartNew(delegate
                                    {
                                        List<Trial> taskTrialList = tempTaskTrialList;
                                        MatlabWrapper taskMatlabWrapper = tempMatlabWrapper;

                                        ButterWorthFilter(taskMatlabWrapper, taskTrialList, butterFilterOrder, butterFilterCutOffPosition, butterFilterCutOffForce, samplesPerSecond);

                                        VelocityCalculation(taskMatlabWrapper, taskTrialList, samplesPerSecond);

                                        TimeNormalization(taskMatlabWrapper, taskTrialList, timeNormalizationSamples, percentPeakVelocity);

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

                                _myManipAnalysisGui.WriteProgressInfo("Calculating szenario mean times...");
                                szenarioMeanTimesContainer = CalculateSzenarioMeanTimes(trialsContainer);

                                _myManipAnalysisGui.WriteProgressInfo("Compressing data...");
                                CompressTrialData(trialsContainer);

                                _myManipAnalysisGui.WriteProgressInfo("Uploading into database...");
                                try
                                {
                                    _myDatabaseWrapper.Insert(trialsContainer);
                                    _myDatabaseWrapper.Insert(szenarioMeanTimesContainer);
                                }
                                catch (Exception ex)
                                {
                                    _myDatabaseWrapper.RemoveMeasureFile(trialsContainer[0].MeasureFile);
                                    throw ex;
                                }
                            }
                            else
                            {
                                _myManipAnalysisGui.WriteToLogBox("Skipping \"" + filename + "\"");
                            }
                        }
                        else
                        {
                            _myManipAnalysisGui.WriteToLogBox("File already imported: " + measureFilesList.ElementAt(files));
                        }
                    }
                    catch (Exception ex)
                    {
                        _myManipAnalysisGui.WriteToLogBox("Error in \"" + measureFilesList.ElementAt(files) + "\":\n" + ex + "\nSkipped file.");
                    }
                    finally
                    {
                        taskMatlabWrappers.ForEach(t => t.Dispose());
                    }
                }
                _myManipAnalysisGui.SetProgressBarValue(0);
                _myManipAnalysisGui.WriteProgressInfo("Ready");
                _myManipAnalysisGui.EnableTabPages(true);

                TaskManager.Remove(Task.CurrentId);
            }));
        }

        private void ButterWorthFilter(MatlabWrapper myMatlabWrapper, List<Trial> trialsContainer, int butterFilterOrder, int butterFilterCutOffPosition, int butterFilterCutOffForce, int samplesPerSecond)
        {
            myMatlabWrapper.SetWorkspaceData("filterOrder", Convert.ToDouble(butterFilterOrder));
            myMatlabWrapper.SetWorkspaceData("cutoffFreqPosition", Convert.ToDouble(butterFilterCutOffPosition));
            myMatlabWrapper.SetWorkspaceData("cutoffFreqForce", Convert.ToDouble(butterFilterCutOffForce));
            myMatlabWrapper.SetWorkspaceData("samplesPerSecond", Convert.ToDouble(samplesPerSecond));
            myMatlabWrapper.Execute("[bPosition,aPosition] = butter(filterOrder,(cutoffFreqPosition/(samplesPerSecond/2)));");
            myMatlabWrapper.Execute("[bForce,aForce] = butter(filterOrder,(cutoffFreqForce/(samplesPerSecond/2)));");

            for (int trialCounter = 0; trialCounter < trialsContainer.Count; trialCounter++)
            {
                myMatlabWrapper.SetWorkspaceData("force_actual_x", trialsContainer[trialCounter].MeasuredForcesRaw.Select(t => t.X).ToArray());
                myMatlabWrapper.SetWorkspaceData("force_actual_y", trialsContainer[trialCounter].MeasuredForcesRaw.Select(t => t.Y).ToArray());
                myMatlabWrapper.SetWorkspaceData("force_actual_z", trialsContainer[trialCounter].MeasuredForcesRaw.Select(t => t.Z).ToArray());

                if (trialsContainer[trialCounter].NominalForcesRaw != null)
                {
                    myMatlabWrapper.SetWorkspaceData("force_nominal_x", trialsContainer[trialCounter].NominalForcesRaw.Select(t => t.X).ToArray());
                    myMatlabWrapper.SetWorkspaceData("force_nominal_y", trialsContainer[trialCounter].NominalForcesRaw.Select(t => t.Y).ToArray());
                    myMatlabWrapper.SetWorkspaceData("force_nominal_z", trialsContainer[trialCounter].NominalForcesRaw.Select(t => t.Z).ToArray());
                }

                myMatlabWrapper.SetWorkspaceData("force_moment_x", trialsContainer[trialCounter].MomentForcesRaw.Select(t => t.X).ToArray());
                myMatlabWrapper.SetWorkspaceData("force_moment_y", trialsContainer[trialCounter].MomentForcesRaw.Select(t => t.Y).ToArray());
                myMatlabWrapper.SetWorkspaceData("force_moment_z", trialsContainer[trialCounter].MomentForcesRaw.Select(t => t.Z).ToArray());

                myMatlabWrapper.SetWorkspaceData("position_cartesian_x", trialsContainer[trialCounter].PositionRaw.Select(t => t.X).ToArray());
                myMatlabWrapper.SetWorkspaceData("position_cartesian_y", trialsContainer[trialCounter].PositionRaw.Select(t => t.Y).ToArray());
                myMatlabWrapper.SetWorkspaceData("position_cartesian_z", trialsContainer[trialCounter].PositionRaw.Select(t => t.Z).ToArray());

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

                for (int frameCount = 0; frameCount < trialsContainer[trialCounter].PositionRaw.Count; frameCount++)
                {
                    var measuredForcesFiltered = new ForceContainer();
                    ForceContainer nominalForcesFiltered = null;
                    if (trialsContainer[trialCounter].NominalForcesRaw != null)
                    {
                        nominalForcesFiltered = new ForceContainer();
                    }
                    var momentForcesFiltered = new ForceContainer();
                    var positionFiltered = new PositionContainer();

                    measuredForcesFiltered.PositionStatus = trialsContainer[trialCounter].MeasuredForcesRaw[frameCount].PositionStatus;
                    measuredForcesFiltered.TimeStamp = trialsContainer[trialCounter].MeasuredForcesRaw[frameCount].TimeStamp;
                    measuredForcesFiltered.X = forceActualX[0, frameCount];
                    measuredForcesFiltered.Y = forceActualY[0, frameCount];
                    measuredForcesFiltered.Z = forceActualZ[0, frameCount];

                    if (trialsContainer[trialCounter].NominalForcesRaw != null)
                    {
                        nominalForcesFiltered.PositionStatus = trialsContainer[trialCounter].NominalForcesRaw[frameCount].PositionStatus;
                        nominalForcesFiltered.TimeStamp = trialsContainer[trialCounter].NominalForcesRaw[frameCount].TimeStamp;
                        nominalForcesFiltered.X = forceNominalX[0, frameCount];
                        nominalForcesFiltered.Y = forceNominalY[0, frameCount];
                        nominalForcesFiltered.Z = forceNominalZ[0, frameCount];
                    }

                    momentForcesFiltered.PositionStatus = trialsContainer[trialCounter].MomentForcesRaw[frameCount].PositionStatus;
                    momentForcesFiltered.TimeStamp = trialsContainer[trialCounter].MomentForcesRaw[frameCount].TimeStamp;
                    momentForcesFiltered.X = forceMomentX[0, frameCount];
                    momentForcesFiltered.Y = forceMomentY[0, frameCount];
                    momentForcesFiltered.Z = forceMomentZ[0, frameCount];

                    positionFiltered.PositionStatus = trialsContainer[trialCounter].PositionRaw[frameCount].PositionStatus;
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

                    trialsContainer[trialCounter].FilteredDataSampleRate = trialsContainer[trialCounter].RawDataSampleRate;
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

            myMatlabWrapper.ClearWorkspace();
        }


        private void VelocityCalculation(MatlabWrapper myMatlabWrapper, List<Trial> trialsContainer, double samplesPerSecond)
        {
            for (int trialCounter = 0; trialCounter < trialsContainer.Count; trialCounter++)
            {
                myMatlabWrapper.SetWorkspaceData("position_cartesian_x", trialsContainer[trialCounter].PositionFiltered.Select(t => t.X).ToArray());
                myMatlabWrapper.SetWorkspaceData("position_cartesian_y", trialsContainer[trialCounter].PositionFiltered.Select(t => t.Y).ToArray());
                myMatlabWrapper.SetWorkspaceData("position_cartesian_z", trialsContainer[trialCounter].PositionFiltered.Select(t => t.Z).ToArray());

                myMatlabWrapper.SetWorkspaceData("sampleRate", samplesPerSecond);

                myMatlabWrapper.Execute("velocity_x = numDiff(position_cartesian_x, sampleRate);");
                myMatlabWrapper.Execute("velocity_y = numDiff(position_cartesian_y, sampleRate);");
                myMatlabWrapper.Execute("velocity_z = numDiff(position_cartesian_z, sampleRate);");

                double[,] velocityX = myMatlabWrapper.GetWorkspaceData("velocity_x");
                double[,] velocityY = myMatlabWrapper.GetWorkspaceData("velocity_y");
                double[,] velocityZ = myMatlabWrapper.GetWorkspaceData("velocity_z");

                trialsContainer[trialCounter].VelocityFiltered = new List<VelocityContainer>();

                for (int frameCount = 0; frameCount < trialsContainer[trialCounter].PositionFiltered.Count; frameCount++)
                {
                    var velocityFiltered = new VelocityContainer();
                    velocityFiltered.PositionStatus = trialsContainer[trialCounter].PositionFiltered[frameCount].PositionStatus;
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

        private void TimeNormalization(MatlabWrapper myMatlabWrapper, List<Trial> trialsContainer, int timeNormalizationSamples, double percentPeakVelocity)
        {
            for (int trialCounter = 0; trialCounter < trialsContainer.Count; trialCounter++)
            {
                myMatlabWrapper.SetWorkspaceData("newSampleRate", Convert.ToDouble(timeNormalizationSamples));

                trialsContainer[trialCounter].NormalizedDataSampleRate = timeNormalizationSamples;
                trialsContainer[trialCounter].VelocityTrimThresholdPercent = percentPeakVelocity;
                trialsContainer[trialCounter].VelocityTrimThresholdForTrial = (trialsContainer[trialCounter].VelocityFiltered.Where(t => t.PositionStatus == 1).Max(t => Math.Sqrt(Math.Pow(t.X, 2) + Math.Pow(t.Y, 2)))/100.0)*percentPeakVelocity;

                DateTime startTime;
                DateTime stopTime;
                try
                {
                    // First element with PositionStatus == 0 and a velocity higher than the threshold
                    startTime = trialsContainer[trialCounter].VelocityFiltered.Where(t => t.PositionStatus == 0).OrderBy(t => t.TimeStamp).First(t => Math.Sqrt(Math.Pow(t.X, 2) + Math.Pow(t.Y, 2)) >= trialsContainer[trialCounter].VelocityTrimThresholdForTrial).TimeStamp;
                }
                catch
                {
                    // First element with PositionStatus == 1
                    startTime = trialsContainer[trialCounter].VelocityFiltered.OrderBy(t => t.TimeStamp).First(t => t.PositionStatus == 1).TimeStamp;
                }

                try
                {
                    // First element with PositionStatus == 2 and a velocity lower than the threshold
                    stopTime = trialsContainer[trialCounter].VelocityFiltered.Where(t => t.PositionStatus == 2).OrderBy(t => t.TimeStamp).First(t => Math.Sqrt(Math.Pow(t.X, 2) + Math.Pow(t.Y, 2)) <= trialsContainer[trialCounter].VelocityTrimThresholdForTrial).TimeStamp;
                }
                catch
                {
                    // Last element with PositionStatus == 2
                    stopTime = trialsContainer[trialCounter].VelocityFiltered.OrderBy(t => t.TimeStamp).Last().TimeStamp;
                }

                IEnumerable<ForceContainer> measuredForcesFilteredCut = trialsContainer[trialCounter].MeasuredForcesFiltered.Where(t => t.TimeStamp >= startTime && t.TimeStamp <= stopTime).OrderBy(t => t.TimeStamp);

                IEnumerable<ForceContainer> momentForcesFilteredCut = trialsContainer[trialCounter].MomentForcesFiltered.Where(t => t.TimeStamp >= startTime && t.TimeStamp <= stopTime).OrderBy(t => t.TimeStamp);

                IEnumerable<ForceContainer> nominalForcesFilteredCut = null;
                if (trialsContainer[trialCounter].NominalForcesFiltered != null)
                {
                    nominalForcesFilteredCut = trialsContainer[trialCounter].NominalForcesFiltered.Where(t => t.TimeStamp >= startTime && t.TimeStamp <= stopTime).OrderBy(t => t.TimeStamp);
                }

                IEnumerable<PositionContainer> positionFilteredCut = trialsContainer[trialCounter].PositionFiltered.Where(t => t.TimeStamp >= startTime && t.TimeStamp <= stopTime).OrderBy(t => t.TimeStamp);

                IEnumerable<VelocityContainer> velocityFilteredCut = trialsContainer[trialCounter].VelocityFiltered.Where(t => t.TimeStamp >= startTime && t.TimeStamp <= stopTime).OrderBy(t => t.TimeStamp);


                myMatlabWrapper.SetWorkspaceData("measure_data_time", positionFilteredCut.Select(t => Convert.ToDouble(t.TimeStamp.Ticks)).ToArray());

                myMatlabWrapper.SetWorkspaceData("forceActualX", measuredForcesFilteredCut.Select(t => t.X).ToArray());
                myMatlabWrapper.SetWorkspaceData("forceActualY", measuredForcesFilteredCut.Select(t => t.Y).ToArray());
                myMatlabWrapper.SetWorkspaceData("forceActualZ", measuredForcesFilteredCut.Select(t => t.Z).ToArray());

                if (nominalForcesFilteredCut != null)
                {
                    myMatlabWrapper.SetWorkspaceData("forceNominalX", nominalForcesFilteredCut.Select(t => t.X).ToArray());
                    myMatlabWrapper.SetWorkspaceData("forceNominalY", nominalForcesFilteredCut.Select(t => t.Y).ToArray());
                    myMatlabWrapper.SetWorkspaceData("forceNominalY", nominalForcesFilteredCut.Select(t => t.Z).ToArray());
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

                myMatlabWrapper.SetWorkspaceData("positionStatus", velocityFilteredCut.Select(t => Convert.ToDouble(t.PositionStatus)).ToArray());

                var errorList = new List<string>();

                myMatlabWrapper.Execute("[errorvar1, forceActualX, newMeasureTime] = timeNorm(forceActualX, measure_data_time, newSampleRate);");
                myMatlabWrapper.Execute("[errorvar2, forceActualY, newMeasureTime] = timeNorm(forceActualY, measure_data_time, newSampleRate);");
                myMatlabWrapper.Execute("[errorvar3, forceActualZ, newMeasureTime] = timeNorm(forceActualZ, measure_data_time, newSampleRate);");

                if (nominalForcesFilteredCut != null)
                {
                    myMatlabWrapper.Execute("[errorvar4, forceNominalX, newMeasureTime] = timeNorm(forceNominalX, measure_data_time, newSampleRate);");
                    myMatlabWrapper.Execute("[errorvar5, forceNominalY, newMeasureTime] = timeNorm(forceNominalY, measure_data_time, newSampleRate);");
                    myMatlabWrapper.Execute("[errorvar6, forceNominalZ, newMeasureTime] = timeNorm(forceNominalZ, measure_data_time, newSampleRate);");
                }

                myMatlabWrapper.Execute("[errorvar7, forceMomentX, newMeasureTime] = timeNorm(forceMomentX, measure_data_time, newSampleRate);");
                myMatlabWrapper.Execute("[errorvar8, forceMomentY, newMeasureTime] = timeNorm(forceMomentY, measure_data_time, newSampleRate);");
                myMatlabWrapper.Execute("[errorvar9, forceMomentZ, newMeasureTime] = timeNorm(forceMomentZ, measure_data_time, newSampleRate);");

                myMatlabWrapper.Execute("[errorvar10, positionCartesianX, newMeasureTime] = timeNorm(positionCartesianX, measure_data_time, newSampleRate);");
                myMatlabWrapper.Execute("[errorvar11, positionCartesianY, newMeasureTime] = timeNorm(positionCartesianY, measure_data_time, newSampleRate);");
                myMatlabWrapper.Execute("[errorvar12, positionCartesianZ, newMeasureTime] = timeNorm(positionCartesianZ, measure_data_time, newSampleRate);");

                myMatlabWrapper.Execute("[errorvar13, velocityX, newMeasureTime] = timeNorm(velocityX, measure_data_time, newSampleRate);");
                myMatlabWrapper.Execute("[errorvar14, velocityY, newMeasureTime] = timeNorm(velocityY, measure_data_time, newSampleRate);");
                myMatlabWrapper.Execute("[errorvar15, velocityZ, newMeasureTime] = timeNorm(velocityZ, measure_data_time, newSampleRate);");

                myMatlabWrapper.Execute("[errorvar16, positionStatus, newMeasureTime] = timeNorm(positionStatus, measure_data_time, newSampleRate);");


                for (int errorVarCounterCounter = 1; errorVarCounterCounter <= 16; errorVarCounterCounter++)
                {
                    if (nominalForcesFilteredCut == null && errorVarCounterCounter >= 4 && errorVarCounterCounter <= 6)
                    {
                    }
                    else
                    {
                        errorList.Add(Convert.ToString(myMatlabWrapper.GetWorkspaceData("errorvar" + errorVarCounterCounter)));
                    }
                }

                if (errorList.Any(t => !string.IsNullOrEmpty(t)))
                {
                    string output = errorList.Where(t => !string.IsNullOrEmpty(t)).Select(t => t + " in " + trialsContainer[trialCounter].MeasureFile.FileName + " at szenario-trial-number " + trialsContainer[trialCounter].TrialNumberInSzenario).Aggregate("", (current, line) => current + line);
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

                for (int frameCount = 0; frameCount < measureDataTime.Length; frameCount++)
                {
                    int newPositionStatus = Convert.ToInt32(positionStatus[frameCount, 0]);
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

            foreach (string szenario in trialsContainer.Select(t => t.Szenario).Distinct())
            {
                IEnumerable<Trial> szenarioTrialsContainer = trialsContainer.Where(t => t.Szenario == szenario);
                foreach (int targetCounter in szenarioTrialsContainer.Select(t => t.Target.Number).Distinct())
                {
                    var tempSzenarioMeanTime = new SzenarioMeanTime();
                    var targetContainer = new TargetContainer {Number = targetCounter};

                    tempSzenarioMeanTime.Group = szenarioTrialsContainer.ElementAt(0).Group;
                    tempSzenarioMeanTime.MeasureFile = szenarioTrialsContainer.ElementAt(0).MeasureFile;
                    tempSzenarioMeanTime.Study = szenarioTrialsContainer.ElementAt(0).Study;
                    tempSzenarioMeanTime.Subject = szenarioTrialsContainer.ElementAt(0).Subject;
                    tempSzenarioMeanTime.Szenario = szenarioTrialsContainer.ElementAt(0).Szenario;
                    tempSzenarioMeanTime.Target = targetContainer;

                    long[] targetDurationTimes = null;
                    lock (trialsContainer)
                    {
                        targetDurationTimes = trialsContainer.Where(t => t.Target.Number == targetCounter).Select(t => t.PositionNormalized.Max(u => u.TimeStamp.Ticks) - t.PositionNormalized.Min(u => u.TimeStamp.Ticks)).ToArray();
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
                    byte[] data = Gzip<List<ForceContainer>>.Compress(trialsContainer[trialCounter].MeasuredForcesFiltered);
                    lock (trialsContainer)
                    {
                        trialsContainer[trialCounter].ZippedMeasuredForcesFiltered = data;
                        trialsContainer[trialCounter].MeasuredForcesFiltered = null;
                    }
                }
                if (trialsContainer[trialCounter].MeasuredForcesNormalized != null)
                {
                    byte[] data = Gzip<List<ForceContainer>>.Compress(trialsContainer[trialCounter].MeasuredForcesNormalized);
                    lock (trialsContainer)
                    {
                        trialsContainer[trialCounter].ZippedMeasuredForcesNormalized = data;
                        trialsContainer[trialCounter].MeasuredForcesNormalized = null;
                    }
                }
                if (trialsContainer[trialCounter].MeasuredForcesRaw != null)
                {
                    byte[] data = Gzip<List<ForceContainer>>.Compress(trialsContainer[trialCounter].MeasuredForcesRaw);
                    lock (trialsContainer)
                    {
                        trialsContainer[trialCounter].ZippedMeasuredForcesRaw = data;
                        trialsContainer[trialCounter].MeasuredForcesRaw = null;
                    }
                }
                if (trialsContainer[trialCounter].NominalForcesFiltered != null)
                {
                    byte[] data = Gzip<List<ForceContainer>>.Compress(trialsContainer[trialCounter].NominalForcesFiltered);
                    lock (trialsContainer)
                    {
                        trialsContainer[trialCounter].ZippedNominalForcesFiltered = data;
                        trialsContainer[trialCounter].NominalForcesFiltered = null;
                    }
                }
                if (trialsContainer[trialCounter].NominalForcesNormalized != null)
                {
                    byte[] data = Gzip<List<ForceContainer>>.Compress(trialsContainer[trialCounter].NominalForcesNormalized);
                    lock (trialsContainer)
                    {
                        trialsContainer[trialCounter].ZippedNominalForcesNormalized = data;
                        trialsContainer[trialCounter].NominalForcesNormalized = null;
                    }
                }
                if (trialsContainer[trialCounter].NominalForcesRaw != null)
                {
                    byte[] data = Gzip<List<ForceContainer>>.Compress(trialsContainer[trialCounter].NominalForcesRaw);
                    lock (trialsContainer)
                    {
                        trialsContainer[trialCounter].ZippedNominalForcesRaw = data;
                        trialsContainer[trialCounter].NominalForcesRaw = null;
                    }
                }
                if (trialsContainer[trialCounter].MomentForcesFiltered != null)
                {
                    byte[] data = Gzip<List<ForceContainer>>.Compress(trialsContainer[trialCounter].MomentForcesFiltered);
                    lock (trialsContainer)
                    {
                        trialsContainer[trialCounter].ZippedMomentForcesFiltered = data;
                        trialsContainer[trialCounter].MomentForcesFiltered = null;
                    }
                }
                if (trialsContainer[trialCounter].MomentForcesNormalized != null)
                {
                    byte[] data = Gzip<List<ForceContainer>>.Compress(trialsContainer[trialCounter].MomentForcesNormalized);
                    lock (trialsContainer)
                    {
                        trialsContainer[trialCounter].ZippedMomentForcesNormalized = data;
                        trialsContainer[trialCounter].MomentForcesNormalized = null;
                    }
                }
                if (trialsContainer[trialCounter].MomentForcesRaw != null)
                {
                    byte[] data = Gzip<List<ForceContainer>>.Compress(trialsContainer[trialCounter].MomentForcesRaw);
                    lock (trialsContainer)
                    {
                        trialsContainer[trialCounter].ZippedMomentForcesRaw = data;
                        trialsContainer[trialCounter].MomentForcesRaw = null;
                    }
                }
                if (trialsContainer[trialCounter].PositionFiltered != null)
                {
                    byte[] data = Gzip<List<PositionContainer>>.Compress(trialsContainer[trialCounter].PositionFiltered);
                    lock (trialsContainer)
                    {
                        trialsContainer[trialCounter].ZippedPositionFiltered = data;
                        trialsContainer[trialCounter].PositionFiltered = null;
                    }
                }
                if (trialsContainer[trialCounter].PositionNormalized != null)
                {
                    byte[] data = Gzip<List<PositionContainer>>.Compress(trialsContainer[trialCounter].PositionNormalized);
                    lock (trialsContainer)
                    {
                        trialsContainer[trialCounter].ZippedPositionNormalized = data;
                        trialsContainer[trialCounter].PositionNormalized = null;
                    }
                }
                if (trialsContainer[trialCounter].PositionRaw != null)
                {
                    byte[] data = Gzip<List<PositionContainer>>.Compress(trialsContainer[trialCounter].PositionRaw);
                    lock (trialsContainer)
                    {
                        trialsContainer[trialCounter].ZippedPositionRaw = data;
                        trialsContainer[trialCounter].PositionRaw = null;
                    }
                }
                if (trialsContainer[trialCounter].VelocityFiltered != null)
                {
                    byte[] data = Gzip<List<VelocityContainer>>.Compress(trialsContainer[trialCounter].VelocityFiltered);
                    lock (trialsContainer)
                    {
                        trialsContainer[trialCounter].ZippedVelocityFiltered = data;
                        trialsContainer[trialCounter].VelocityFiltered = null;
                    }
                }
                if (trialsContainer[trialCounter].VelocityNormalized != null)
                {
                    byte[] data = Gzip<List<VelocityContainer>>.Compress(trialsContainer[trialCounter].VelocityNormalized);
                    lock (trialsContainer)
                    {
                        trialsContainer[trialCounter].ZippedVelocityNormalized = data;
                        trialsContainer[trialCounter].VelocityNormalized = null;
                    }
                }
                if (trialsContainer[trialCounter].Statistics != null)
                {
                    byte[] data = Gzip<StatisticContainer>.Compress(trialsContainer[trialCounter].Statistics);
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
                    byte[] data = Gzip<List<ForceContainer>>.Compress(baselinesContainer[baselineCounter].MeasuredForces);
                    lock (baselinesContainer)
                    {
                        baselinesContainer[baselineCounter].ZippedMeasuredForces = data;
                        baselinesContainer[baselineCounter].MeasuredForces = null;
                    }
                }
                if (baselinesContainer[baselineCounter].MomentForces != null)
                {
                    byte[] data = Gzip<List<ForceContainer>>.Compress(baselinesContainer[baselineCounter].MomentForces);
                    lock (baselinesContainer)
                    {
                        baselinesContainer[baselineCounter].ZippedMomentForces = data;
                        baselinesContainer[baselineCounter].MomentForces = null;
                    }
                }
                if (baselinesContainer[baselineCounter].NominalForces != null)
                {
                    byte[] data = Gzip<List<ForceContainer>>.Compress(baselinesContainer[baselineCounter].NominalForces);
                    lock (baselinesContainer)
                    {
                        baselinesContainer[baselineCounter].ZippedNominalForces = data;
                        baselinesContainer[baselineCounter].NominalForces = null;
                    }
                }
                if (baselinesContainer[baselineCounter].Position != null)
                {
                    byte[] data = Gzip<List<PositionContainer>>.Compress(baselinesContainer[baselineCounter].Position);
                    lock (baselinesContainer)
                    {
                        baselinesContainer[baselineCounter].ZippedPosition = data;
                        baselinesContainer[baselineCounter].Position = null;
                    }
                }
                if (baselinesContainer[baselineCounter].Velocity != null)
                {
                    byte[] data = Gzip<List<VelocityContainer>>.Compress(baselinesContainer[baselineCounter].Velocity);
                    lock (baselinesContainer)
                    {
                        baselinesContainer[baselineCounter].ZippedVelocity = data;
                        baselinesContainer[baselineCounter].Velocity = null;
                    }
                }
            });
        }

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
                    var baselinesContainer = new List<Baseline>();
                    IEnumerable<string> studys = _myDatabaseWrapper.GetStudys();

                    foreach (string
                        study
                        in
                        studys)
                    {
                        if (study == "Study 7")
                        {
                            IEnumerable<string> groups = _myDatabaseWrapper.GetGroups(study);

                            foreach (string
                                group
                                in
                                groups)
                            {
                                var baselineFields = new FieldsBuilder<Trial>();
                                baselineFields.Include(t1 => t1.ZippedPositionNormalized, t2 => t2.ZippedVelocityNormalized, t3 => t3.ZippedMeasuredForcesNormalized, t4 => t4.ZippedMomentForcesNormalized, t5 => t5.TrialType, t6 => t6.ForceFieldType, t7 => t7.Handedness, t8 => t8.Study, t9 => t9.Group, t10 => t10.Subject, t11 => t11.TrialNumberInSzenario, t12 => t12.Target, t13 => t13.NormalizedDataSampleRate, t14 => t14.Id);

                                IEnumerable<SubjectContainer> subjectsLR = _myDatabaseWrapper.GetSubjects(study, group, "LR_Base1");
                                IEnumerable<SubjectContainer> subjectsRL = _myDatabaseWrapper.GetSubjects(study, group, "RL_Base1");

                                foreach (SubjectContainer
                                    subject
                                    in
                                    subjectsLR)
                                {
                                    DateTime turnBase1 = _myDatabaseWrapper.GetTurns(study, group, "LR_Base1", subject).ElementAt(0);
                                    DateTime turnBase2a = _myDatabaseWrapper.GetTurns(study, group, "LR_Base2a", subject).ElementAt(0);
                                    DateTime turnBase2b = _myDatabaseWrapper.GetTurns(study, group, "LR_Base2b", subject).ElementAt(0);

                                    List<Trial> base1 = _myDatabaseWrapper.GetTrials(study, group, "LR_Base1", subject, turnBase1, Enumerable.Range(1, 216), baselineFields).ToList();
                                    List<Trial> base2a = _myDatabaseWrapper.GetTrials(study, group, "LR_Base2a", subject, turnBase2a, Enumerable.Range(1, 12), baselineFields).ToList();
                                    List<Trial> base2b = _myDatabaseWrapper.GetTrials(study, group, "LR_Base2b", subject, turnBase2b, Enumerable.Range(1, 12), baselineFields).ToList();

                                    base1.ForEach(t => t.PositionNormalized = Gzip<List<PositionContainer>>.DeCompress(t.ZippedPositionNormalized).OrderBy(u => u.TimeStamp).ToList());
                                    base1.ForEach(t => t.VelocityNormalized = Gzip<List<VelocityContainer>>.DeCompress(t.ZippedVelocityNormalized).OrderBy(u => u.TimeStamp).ToList());
                                    base1.ForEach(t => t.MeasuredForcesNormalized = Gzip<List<ForceContainer>>.DeCompress(t.ZippedMeasuredForcesNormalized).OrderBy(u => u.TimeStamp).ToList());
                                    base1.ForEach(t => t.MomentForcesNormalized = Gzip<List<ForceContainer>>.DeCompress(t.ZippedMomentForcesNormalized).OrderBy(u => u.TimeStamp).ToList());

                                    base2a.ForEach(t => t.PositionNormalized = Gzip<List<PositionContainer>>.DeCompress(t.ZippedPositionNormalized).OrderBy(u => u.TimeStamp).ToList());
                                    base2a.ForEach(t => t.VelocityNormalized = Gzip<List<VelocityContainer>>.DeCompress(t.ZippedVelocityNormalized).OrderBy(u => u.TimeStamp).ToList());
                                    base2a.ForEach(t => t.MeasuredForcesNormalized = Gzip<List<ForceContainer>>.DeCompress(t.ZippedMeasuredForcesNormalized).OrderBy(u => u.TimeStamp).ToList());
                                    base2a.ForEach(t => t.MomentForcesNormalized = Gzip<List<ForceContainer>>.DeCompress(t.ZippedMomentForcesNormalized).OrderBy(u => u.TimeStamp).ToList());

                                    base2b.ForEach(t => t.PositionNormalized = Gzip<List<PositionContainer>>.DeCompress(t.ZippedPositionNormalized).OrderBy(u => u.TimeStamp).ToList());
                                    base2b.ForEach(t => t.VelocityNormalized = Gzip<List<VelocityContainer>>.DeCompress(t.ZippedVelocityNormalized).OrderBy(u => u.TimeStamp).ToList());
                                    base2b.ForEach(t => t.MeasuredForcesNormalized = Gzip<List<ForceContainer>>.DeCompress(t.ZippedMeasuredForcesNormalized).OrderBy(u => u.TimeStamp).ToList());
                                    base2b.ForEach(t => t.MomentForcesNormalized = Gzip<List<ForceContainer>>.DeCompress(t.ZippedMomentForcesNormalized).OrderBy(u => u.TimeStamp).ToList());

                                    List<Trial> forceFieldCatchTrialBaselineLeftHand = base1.Where(t => t.TrialNumberInSzenario == 18 || t.TrialNumberInSzenario == 31 || t.TrialNumberInSzenario == 44 || t.TrialNumberInSzenario == 117 || t.TrialNumberInSzenario == 135 || t.TrialNumberInSzenario == 150).ToList();

                                    List<Trial> forceFieldCatchTrialBaselineRightHand = base1.Where(t => t.TrialNumberInSzenario == 72 || t.TrialNumberInSzenario == 85 || t.TrialNumberInSzenario == 98 || t.TrialNumberInSzenario == 171 || t.TrialNumberInSzenario == 189 || t.TrialNumberInSzenario == 204).ToList();

                                    List<Trial> errorClampBaselineLeftHand = base1.Where(t => t.TrialNumberInSzenario == 15 || t.TrialNumberInSzenario == 28 || t.TrialNumberInSzenario == 52 || t.TrialNumberInSzenario == 115 || t.TrialNumberInSzenario == 130 || t.TrialNumberInSzenario == 145).ToList();
                                    errorClampBaselineLeftHand.AddRange(base2b.Where(t => t.TrialNumberInSzenario >= 7 && t.TrialNumberInSzenario <= 12));

                                    List<Trial> errorClampBaselineRightHand = base1.Where(t => t.TrialNumberInSzenario == 69 || t.TrialNumberInSzenario == 82 || t.TrialNumberInSzenario == 106 || t.TrialNumberInSzenario == 169 || t.TrialNumberInSzenario == 184 || t.TrialNumberInSzenario == 199).ToList();
                                    errorClampBaselineRightHand.AddRange(base2a.Where(t => t.TrialNumberInSzenario >= 7 && t.TrialNumberInSzenario <= 12));

                                    List<Trial> nullFieldBaselineLeftHand = base1.Where(t => t.TrialNumberInSzenario >= 157 && t.TrialNumberInSzenario <= 162).ToList();
                                    nullFieldBaselineLeftHand.AddRange(base2b.Where(t => t.TrialNumberInSzenario >= 1 && t.TrialNumberInSzenario <= 6));

                                    List<Trial> nullFieldBaselineRightHand = base1.Where(t => t.TrialNumberInSzenario >= 211 && t.TrialNumberInSzenario <= 216).ToList();
                                    nullFieldBaselineRightHand.AddRange(base2a.Where(t => t.TrialNumberInSzenario >= 1 && t.TrialNumberInSzenario <= 6));

                                    if (forceFieldCatchTrialBaselineLeftHand.All(t => t.TrialType == Trial.TrialTypeEnum.StandardTrial && t.ForceFieldType == Trial.ForceFieldTypeEnum.ForceFieldCW && t.Handedness == Trial.HandednessEnum.LeftHand) && forceFieldCatchTrialBaselineRightHand.All(t => t.TrialType == Trial.TrialTypeEnum.StandardTrial && t.ForceFieldType == Trial.ForceFieldTypeEnum.ForceFieldCW && t.Handedness == Trial.HandednessEnum.RightHand) && errorClampBaselineLeftHand.All(t => t.TrialType == Trial.TrialTypeEnum.ErrorClampTrial && t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField && t.Handedness == Trial.HandednessEnum.LeftHand) && errorClampBaselineRightHand.All(t => t.TrialType == Trial.TrialTypeEnum.ErrorClampTrial && t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField && t.Handedness == Trial.HandednessEnum.RightHand) &&
                                        nullFieldBaselineLeftHand.All(t => t.TrialType == Trial.TrialTypeEnum.StandardTrial && t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField && t.Handedness == Trial.HandednessEnum.LeftHand) && nullFieldBaselineRightHand.All(t => t.TrialType == Trial.TrialTypeEnum.StandardTrial && t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField && t.Handedness == Trial.HandednessEnum.RightHand))
                                    {
                                        baselinesContainer.AddRange(doBaselineCalculation(forceFieldCatchTrialBaselineLeftHand));
                                        baselinesContainer.AddRange(doBaselineCalculation(forceFieldCatchTrialBaselineRightHand));
                                        baselinesContainer.AddRange(doBaselineCalculation(errorClampBaselineLeftHand));
                                        baselinesContainer.AddRange(doBaselineCalculation(errorClampBaselineRightHand));
                                        baselinesContainer.AddRange(doBaselineCalculation(nullFieldBaselineLeftHand));
                                        baselinesContainer.AddRange(doBaselineCalculation(nullFieldBaselineRightHand));
                                    }
                                    else
                                    {
                                        _myManipAnalysisGui.WriteToLogBox("Error calculating Baseline. Incorrect TrialTypes. " + study + " / " + group + " / " + subject);
                                    }
                                }

                                foreach (SubjectContainer
                                    subject
                                    in
                                    subjectsRL)
                                {
                                    DateTime turnBase1 = _myDatabaseWrapper.GetTurns(study, group, "RL_Base1", subject).ElementAt(0);
                                    DateTime turnBase2a = _myDatabaseWrapper.GetTurns(study, group, "RL_Base2a", subject).ElementAt(0);
                                    DateTime turnBase2b = _myDatabaseWrapper.GetTurns(study, group, "RL_Base2b", subject).ElementAt(0);

                                    List<Trial> base1 = _myDatabaseWrapper.GetTrials(study, group, "RL_Base1", subject, turnBase1, Enumerable.Range(1, 216), baselineFields).ToList();
                                    List<Trial> base2a = _myDatabaseWrapper.GetTrials(study, group, "RL_Base2a", subject, turnBase2a, Enumerable.Range(1, 12), baselineFields).ToList();
                                    List<Trial> base2b = _myDatabaseWrapper.GetTrials(study, group, "RL_Base2b", subject, turnBase2b, Enumerable.Range(1, 12), baselineFields).ToList();

                                    base1.ForEach(t => t.PositionNormalized = Gzip<List<PositionContainer>>.DeCompress(t.ZippedPositionNormalized).OrderBy(u => u.TimeStamp).ToList());
                                    base1.ForEach(t => t.VelocityNormalized = Gzip<List<VelocityContainer>>.DeCompress(t.ZippedVelocityNormalized).OrderBy(u => u.TimeStamp).ToList());
                                    base1.ForEach(t => t.MeasuredForcesNormalized = Gzip<List<ForceContainer>>.DeCompress(t.ZippedMeasuredForcesNormalized).OrderBy(u => u.TimeStamp).ToList());
                                    base1.ForEach(t => t.MomentForcesNormalized = Gzip<List<ForceContainer>>.DeCompress(t.ZippedMomentForcesNormalized).OrderBy(u => u.TimeStamp).ToList());

                                    base2a.ForEach(t => t.PositionNormalized = Gzip<List<PositionContainer>>.DeCompress(t.ZippedPositionNormalized).OrderBy(u => u.TimeStamp).ToList());
                                    base2a.ForEach(t => t.VelocityNormalized = Gzip<List<VelocityContainer>>.DeCompress(t.ZippedVelocityNormalized).OrderBy(u => u.TimeStamp).ToList());
                                    base2a.ForEach(t => t.MeasuredForcesNormalized = Gzip<List<ForceContainer>>.DeCompress(t.ZippedMeasuredForcesNormalized).OrderBy(u => u.TimeStamp).ToList());
                                    base2a.ForEach(t => t.MomentForcesNormalized = Gzip<List<ForceContainer>>.DeCompress(t.ZippedMomentForcesNormalized).OrderBy(u => u.TimeStamp).ToList());

                                    base2b.ForEach(t => t.PositionNormalized = Gzip<List<PositionContainer>>.DeCompress(t.ZippedPositionNormalized).OrderBy(u => u.TimeStamp).ToList());
                                    base2b.ForEach(t => t.VelocityNormalized = Gzip<List<VelocityContainer>>.DeCompress(t.ZippedVelocityNormalized).OrderBy(u => u.TimeStamp).ToList());
                                    base2b.ForEach(t => t.MeasuredForcesNormalized = Gzip<List<ForceContainer>>.DeCompress(t.ZippedMeasuredForcesNormalized).OrderBy(u => u.TimeStamp).ToList());
                                    base2b.ForEach(t => t.MomentForcesNormalized = Gzip<List<ForceContainer>>.DeCompress(t.ZippedMomentForcesNormalized).OrderBy(u => u.TimeStamp).ToList());

                                    List<Trial> forceFieldCatchTrialBaselineLeftHand = base1.Where(t => t.TrialNumberInSzenario == 72 || t.TrialNumberInSzenario == 85 || t.TrialNumberInSzenario == 98 || t.TrialNumberInSzenario == 171 || t.TrialNumberInSzenario == 189 || t.TrialNumberInSzenario == 204).ToList();

                                    List<Trial> forceFieldCatchTrialBaselineRightHand = base1.Where(t => t.TrialNumberInSzenario == 18 || t.TrialNumberInSzenario == 31 || t.TrialNumberInSzenario == 44 || t.TrialNumberInSzenario == 117 || t.TrialNumberInSzenario == 135 || t.TrialNumberInSzenario == 150).ToList();

                                    List<Trial> errorClampBaselineLeftHand = base1.Where(t => t.TrialNumberInSzenario == 69 || t.TrialNumberInSzenario == 82 || t.TrialNumberInSzenario == 106 || t.TrialNumberInSzenario == 169 || t.TrialNumberInSzenario == 184 || t.TrialNumberInSzenario == 199).ToList();
                                    errorClampBaselineLeftHand.AddRange(base2a.Where(t => t.TrialNumberInSzenario >= 7 && t.TrialNumberInSzenario <= 12));

                                    List<Trial> errorClampBaselineRightHand = base1.Where(t => t.TrialNumberInSzenario == 15 || t.TrialNumberInSzenario == 28 || t.TrialNumberInSzenario == 52 || t.TrialNumberInSzenario == 115 || t.TrialNumberInSzenario == 130 || t.TrialNumberInSzenario == 145).ToList();
                                    errorClampBaselineRightHand.AddRange(base2b.Where(t => t.TrialNumberInSzenario >= 7 && t.TrialNumberInSzenario <= 12));

                                    List<Trial> nullFieldBaselineLeftHand = base1.Where(t => t.TrialNumberInSzenario >= 211 && t.TrialNumberInSzenario <= 216).ToList();
                                    nullFieldBaselineLeftHand.AddRange(base2a.Where(t => t.TrialNumberInSzenario >= 1 && t.TrialNumberInSzenario <= 6));

                                    List<Trial> nullFieldBaselineRightHand = base1.Where(t => t.TrialNumberInSzenario >= 157 && t.TrialNumberInSzenario <= 162).ToList();
                                    nullFieldBaselineRightHand.AddRange(base2b.Where(t => t.TrialNumberInSzenario >= 1 && t.TrialNumberInSzenario <= 6));

                                    if (forceFieldCatchTrialBaselineLeftHand.All(t => t.TrialType == Trial.TrialTypeEnum.StandardTrial && t.ForceFieldType == Trial.ForceFieldTypeEnum.ForceFieldCW && t.Handedness == Trial.HandednessEnum.LeftHand) && forceFieldCatchTrialBaselineRightHand.All(t => t.TrialType == Trial.TrialTypeEnum.StandardTrial && t.ForceFieldType == Trial.ForceFieldTypeEnum.ForceFieldCW && t.Handedness == Trial.HandednessEnum.RightHand) && errorClampBaselineLeftHand.All(t => t.TrialType == Trial.TrialTypeEnum.ErrorClampTrial && t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField && t.Handedness == Trial.HandednessEnum.LeftHand) && errorClampBaselineRightHand.All(t => t.TrialType == Trial.TrialTypeEnum.ErrorClampTrial && t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField && t.Handedness == Trial.HandednessEnum.RightHand) &&
                                        nullFieldBaselineLeftHand.All(t => t.TrialType == Trial.TrialTypeEnum.StandardTrial && t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField && t.Handedness == Trial.HandednessEnum.LeftHand) && nullFieldBaselineRightHand.All(t => t.TrialType == Trial.TrialTypeEnum.StandardTrial && t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField && t.Handedness == Trial.HandednessEnum.RightHand))
                                    {
                                        baselinesContainer.AddRange(doBaselineCalculation(forceFieldCatchTrialBaselineLeftHand));
                                        baselinesContainer.AddRange(doBaselineCalculation(forceFieldCatchTrialBaselineRightHand));
                                        baselinesContainer.AddRange(doBaselineCalculation(errorClampBaselineLeftHand));
                                        baselinesContainer.AddRange(doBaselineCalculation(errorClampBaselineRightHand));
                                        baselinesContainer.AddRange(doBaselineCalculation(nullFieldBaselineLeftHand));
                                        baselinesContainer.AddRange(doBaselineCalculation(nullFieldBaselineRightHand));
                                    }
                                    else
                                    {
                                        _myManipAnalysisGui.WriteToLogBox("Error calculating Baseline. Incorrect TrialTypes. " + study + " / " + group + " / " + subject);
                                    }
                                }
                            }
                        }
                    }

                    if (baselinesContainer.Any())
                    {
                        CompressBaselineData(baselinesContainer);
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

        private List<Baseline> doBaselineCalculation(List<Trial> inputTrials)
        {
            var baselines = new List<Baseline>();

            foreach (int
                targetCounter
                in
                inputTrials.Select(t => t.Target.Number).Distinct())
            {
                List<Trial> baselineTrials = inputTrials.Where(t => t.Target.Number == targetCounter).ToList();

                List<double[]> measuredForcesX = baselineTrials.Select(t => t.MeasuredForcesNormalized.Select(u => u.X).ToArray()).ToList();
                List<double[]> measuredForcesY = baselineTrials.Select(t => t.MeasuredForcesNormalized.Select(u => u.Y).ToArray()).ToList();
                List<double[]> measuredForcesZ = baselineTrials.Select(t => t.MeasuredForcesNormalized.Select(u => u.Z).ToArray()).ToList();

                List<double[]> nominalForcesX = null;
                List<double[]> nominalForcesY = null;
                List<double[]> nominalForcesZ = null;
                if (baselineTrials[0].NominalForcesNormalized != null)
                {
                    nominalForcesX = baselineTrials.Select(t => t.NominalForcesNormalized.Select(u => u.X).ToArray()).ToList();
                    nominalForcesY = baselineTrials.Select(t => t.NominalForcesNormalized.Select(u => u.Y).ToArray()).ToList();
                    nominalForcesZ = baselineTrials.Select(t => t.NominalForcesNormalized.Select(u => u.Z).ToArray()).ToList();
                }

                List<double[]> momentForcesX = baselineTrials.Select(t => t.MomentForcesNormalized.Select(u => u.X).ToArray()).ToList();
                List<double[]> momentForcesY = baselineTrials.Select(t => t.MomentForcesNormalized.Select(u => u.Y).ToArray()).ToList();
                List<double[]> momentForcesZ = baselineTrials.Select(t => t.MomentForcesNormalized.Select(u => u.Z).ToArray()).ToList();

                List<double[]> positionX = baselineTrials.Select(t => t.PositionNormalized.Select(u => u.X).ToArray()).ToList();
                List<double[]> positionY = baselineTrials.Select(t => t.PositionNormalized.Select(u => u.Y).ToArray()).ToList();
                List<double[]> positionZ = baselineTrials.Select(t => t.PositionNormalized.Select(u => u.Z).ToArray()).ToList();

                List<double[]> velocityX = baselineTrials.Select(t => t.VelocityNormalized.Select(u => u.X).ToArray()).ToList();
                List<double[]> velocityY = baselineTrials.Select(t => t.VelocityNormalized.Select(u => u.Y).ToArray()).ToList();
                List<double[]> velocityZ = baselineTrials.Select(t => t.VelocityNormalized.Select(u => u.Z).ToArray()).ToList();


                var tempBaseline = new Baseline();
                tempBaseline.Group = baselineTrials[0].Group;
                tempBaseline.Study = baselineTrials[0].Study;
                tempBaseline.MeasureFile = baselineTrials[0].MeasureFile;
                tempBaseline.Subject = baselineTrials[0].Subject;
                tempBaseline.Szenario = baselineTrials[0].Szenario;
                tempBaseline.Target = baselineTrials[0].Target;
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

                int frameCount = measuredForcesX[0].Length;
                int baselineTrialCount = baselineTrials.Count;

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
                for (int timeSample = 0; timeSample < frameCount; timeSample++)
                {
                    baselineTimeStamps[timeSample] = DateTime.Now;
                    //baselineTrials[0].MeasureFile.CreationTime;
                    baselineTimeStamps[timeSample] = baselineTimeStamps[timeSample].AddSeconds((1.0/Convert.ToDouble(baselineTrials[0].NormalizedDataSampleRate))*Convert.ToDouble(timeSample));
                }

                for (int trialCounter = 0; trialCounter < baselineTrialCount; trialCounter++)
                {
                    for (int frameCounter = 0; frameCounter < frameCount; frameCounter++)
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

                for (int frameCounter = 0; frameCounter < frameCount; frameCounter++)
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
                int counter = 0;
                try
                {
                    var statisticFields = new FieldsBuilder<Trial>();
                    statisticFields.Include(t1 => t1.ZippedVelocityNormalized, t2 => t2.ZippedPositionNormalized, t3 => t3.ZippedMeasuredForcesNormalized, t4 => t4.Study, t5 => t5.Group, t6 => t6.Szenario, t7 => t7.Subject, t8 => t8.Target, t9 => t9.TrialNumberInSzenario, t10 => t10.TrialType, t11 => t11.ForceFieldType, t12 => t12.Handedness);
                    List<Trial> trialList = _myDatabaseWrapper.GetTrialsWithoutStatistics(statisticFields).ToList();

                    var baselineBuffer = new List<Baseline>();
                    int cpuCount = Environment.ProcessorCount;

                    if (trialList.Count > 0)
                    {
                        var taskTrialListParts = new List<List<Trial>>();
                        int threadCount = 0;

                        if (trialList.Count > cpuCount)
                        {
                            for (int cpuCounter = 0; cpuCounter < cpuCount; cpuCounter++)
                            {
                                taskTrialListParts.Add(new List<Trial>());
                            }

                            int trialCounter = 0;
                            int listCounter = 0;
                            while (trialCounter < trialList.Count)
                            {
                                taskTrialListParts[listCounter].Add(trialList[trialCounter]);
                                trialCounter
                                    ++;
                                listCounter
                                    ++;
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

                        for (int i = 0; i < threadCount; i++)
                        {
                            List<Trial> tempTaskTrialList = taskTrialListParts.ElementAt(i).ToList();

                            calculatingTasks.Add(Task.Factory.StartNew(delegate
                            {
                                List<Trial> taskTrialList = tempTaskTrialList;
                                var taskMatlabWrapper = new MatlabWrapper(_myManipAnalysisGui, MatlabWrapper.MatlabInstanceType.Single);

                                try
                                {
                                    foreach (Trial
                                        trial
                                        in
                                        taskTrialList)
                                    {
                                        if (TaskManager.Cancel)
                                        {
                                            break;
                                        }
                                        while (TaskManager.Pause & !TaskManager.Cancel)
                                        {
                                            Thread.Sleep(100);
                                        }

                                        var baselineFields = new FieldsBuilder<Baseline>();
                                        baselineFields.Include(t1 => t1.Study, t2 => t2.Group, t3 => t3.Subject, t4 => t4.Target, t5 => t5.TrialType, t6 => t6.ForceFieldType, t7 => t7.Handedness, t8 => t8.ZippedVelocity, t9 => t9.ZippedPosition, t10 => t10.ZippedMeasuredForces);
                                        Baseline baseline = null;

                                        if (trial.Study == "Study 7")
                                        {
                                            if (trial.TrialType == Trial.TrialTypeEnum.ErrorClampTrial)
                                            {
                                                baseline = baselineBuffer.Find(t => t.Study == trial.Study && t.Group == trial.Group && t.Subject == trial.Subject && t.Target.Number == trial.Target.Number && t.TrialType == trial.TrialType && t.ForceFieldType == Trial.ForceFieldTypeEnum.NullField && t.Handedness == trial.Handedness);
                                                if (baseline == null)
                                                {
                                                    baseline = _myDatabaseWrapper.GetBaseline(trial.Study, trial.Group, trial.Subject, trial.Target.Number, trial.TrialType, Trial.ForceFieldTypeEnum.NullField, trial.Handedness, baselineFields);
                                                    lock (baselineBuffer)
                                                    {
                                                        baselineBuffer.Add(baseline);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                baseline = baselineBuffer.Find(t => t.Study == trial.Study && t.Group == trial.Group && t.Subject == trial.Subject && t.Target.Number == trial.Target.Number && t.TrialType == trial.TrialType && t.ForceFieldType == trial.ForceFieldType && t.Handedness == trial.Handedness);
                                                if (baseline == null)
                                                {
                                                    baseline = _myDatabaseWrapper.GetBaseline(trial.Study, trial.Group, trial.Subject, trial.Target.Number, trial.TrialType, trial.ForceFieldType, trial.Handedness, baselineFields);
                                                    lock (baselineBuffer)
                                                    {
                                                        baselineBuffer.Add(baseline);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            baseline = baselineBuffer.Find(t => t.Study == trial.Study && t.Group == trial.Group && t.Subject == trial.Subject && t.Target.Number == trial.Target.Number && t.TrialType == trial.TrialType && t.ForceFieldType == trial.ForceFieldType && t.Handedness == trial.Handedness);
                                            if (baseline == null)
                                            {
                                                baseline = _myDatabaseWrapper.GetBaseline(trial.Study, trial.Group, trial.Subject, trial.Target.Number, trial.TrialType, trial.ForceFieldType, trial.Handedness, baselineFields);
                                                lock (baselineBuffer)
                                                {
                                                    baselineBuffer.Add(baseline);
                                                }
                                            }
                                        }

                                        if (baseline != null)
                                        {
                                            baseline.Position = Gzip<List<PositionContainer>>.DeCompress(baseline.ZippedPosition).OrderBy(t => t.TimeStamp).ToList();
                                            baseline.Velocity = Gzip<List<VelocityContainer>>.DeCompress(baseline.ZippedVelocity).OrderBy(t => t.TimeStamp).ToList();
                                            baseline.MeasuredForces = Gzip<List<ForceContainer>>.DeCompress(baseline.ZippedMeasuredForces).OrderBy(t => t.TimeStamp).ToList();
                                            trial.PositionNormalized = Gzip<List<PositionContainer>>.DeCompress(trial.ZippedPositionNormalized).OrderBy(t => t.TimeStamp).ToList();
                                            trial.VelocityNormalized = Gzip<List<VelocityContainer>>.DeCompress(trial.ZippedVelocityNormalized).OrderBy(t => t.TimeStamp).ToList();
                                            trial.MeasuredForcesNormalized = Gzip<List<ForceContainer>>.DeCompress(trial.ZippedMeasuredForcesNormalized).OrderBy(t => t.TimeStamp).ToList();

                                            taskMatlabWrapper.ClearWorkspace();

                                            taskMatlabWrapper.SetWorkspaceData("targetNumber", trial.Target.Number);
                                            taskMatlabWrapper.SetWorkspaceData("positionX", trial.PositionNormalized.Select(t => t.X).ToArray());
                                            taskMatlabWrapper.SetWorkspaceData("positionY", trial.PositionNormalized.Select(t => t.Y).ToArray());
                                            taskMatlabWrapper.SetWorkspaceData("velocityX", trial.VelocityNormalized.Select(t => t.X).ToArray());
                                            taskMatlabWrapper.SetWorkspaceData("velocityY", trial.VelocityNormalized.Select(t => t.Y).ToArray());
                                            taskMatlabWrapper.SetWorkspaceData("forceX", trial.MeasuredForcesNormalized.Select(t => t.X).ToArray());
                                            taskMatlabWrapper.SetWorkspaceData("forceY", trial.MeasuredForcesNormalized.Select(t => t.Y).ToArray());

                                            taskMatlabWrapper.SetWorkspaceData("baselinePositionX", baseline.Position.Select(t => t.X).ToArray());
                                            taskMatlabWrapper.SetWorkspaceData("baselinePositionY", baseline.Position.Select(t => t.Y).ToArray());
                                            taskMatlabWrapper.SetWorkspaceData("baselineVelocityX", baseline.Velocity.Select(t => t.X).ToArray());
                                            taskMatlabWrapper.SetWorkspaceData("baselineVelocityY", baseline.Velocity.Select(t => t.Y).ToArray());
                                            taskMatlabWrapper.SetWorkspaceData("baselineForceX", baseline.MeasuredForces.Select(t => t.X).ToArray());
                                            taskMatlabWrapper.SetWorkspaceData("baselineForceY", baseline.MeasuredForces.Select(t => t.Y).ToArray());

                                            // Matlab statistic calculations
                                            taskMatlabWrapper.Execute("vector_correlation = vectorCorrelation([velocityX velocityY], [baselineVelocityX baselineVelocityY]);");
                                            taskMatlabWrapper.Execute("enclosed_area = enclosedArea(positionX, positionY);");
                                            taskMatlabWrapper.Execute("length_abs = trajectLength(positionX', positionY');");
                                            taskMatlabWrapper.Execute("length_ratio = trajectLength(positionX', positionY') / trajectLength(baselinePositionX', baselinePositionY');");
                                            taskMatlabWrapper.Execute("distanceAbs = distance2curveAbs([positionX' positionY'], targetNumber);");
                                            taskMatlabWrapper.Execute("distanceSign = distance2curveSign([positionX' positionY'], targetNumber);");
                                            taskMatlabWrapper.Execute("meanDistanceAbs = mean(distanceAbs);");
                                            taskMatlabWrapper.Execute("maxDistanceAbs = max(distanceAbs);");
                                            taskMatlabWrapper.Execute("[~, posDistanceSign] = max(abs(distanceSign));");
                                            taskMatlabWrapper.Execute("maxDistanceSign = distanceSign(posDistanceSign);");
                                            taskMatlabWrapper.Execute("rmse = rootMeanSquareError([positionX positionY], [baselinePositionX baselinePositionY]);");

                                            // Create StatisticContainer and fill it with calculated Matlab statistics
                                            var statisticContainer = new StatisticContainer();
                                            statisticContainer.VelocityVectorCorrelation = taskMatlabWrapper.GetWorkspaceData("vector_correlation");
                                            statisticContainer.EnclosedArea = taskMatlabWrapper.GetWorkspaceData("enclosed_area");
                                            statisticContainer.AbsoluteTrajectoryLength = taskMatlabWrapper.GetWorkspaceData("length_abs");
                                            statisticContainer.AbsoluteBaselineTrajectoryLengthRatio = taskMatlabWrapper.GetWorkspaceData("length_ratio");
                                            statisticContainer.AbsoluteMeanPerpendicularDisplacement = taskMatlabWrapper.GetWorkspaceData("meanDistanceAbs");
                                            statisticContainer.AbsoluteMaximalPerpendicularDisplacement = taskMatlabWrapper.GetWorkspaceData("maxDistanceAbs");
                                            statisticContainer.SignedMaximalPerpendicularDisplacement = taskMatlabWrapper.GetWorkspaceData("maxDistanceSign");
                                            statisticContainer.RMSE = taskMatlabWrapper.GetWorkspaceData("rmse");

                                            // Fill StatisticContainer with Abs and Sign PerpendicularDisplacement array
                                            double[,] absolutePerpendicularDisplacement = taskMatlabWrapper.GetWorkspaceData("distanceAbs");
                                            double[,] signedPerpendicularDisplacement = taskMatlabWrapper.GetWorkspaceData("distanceSign");

                                            for (int perpendicularDisplacementCounter = 0; perpendicularDisplacementCounter < trial.PositionNormalized.Select(t => t.TimeStamp).Count(); perpendicularDisplacementCounter++)
                                            {
                                                var absolute = new PerpendicularDisplacementContainer();
                                                var signed = new PerpendicularDisplacementContainer();

                                                absolute.PerpendicularDisplacement = absolutePerpendicularDisplacement[perpendicularDisplacementCounter, 0];
                                                absolute.TimeStamp = trial.PositionNormalized[perpendicularDisplacementCounter].TimeStamp;

                                                signed.PerpendicularDisplacement = signedPerpendicularDisplacement[perpendicularDisplacementCounter, 0];
                                                signed.TimeStamp = trial.PositionNormalized[perpendicularDisplacementCounter].TimeStamp;

                                                statisticContainer.AbsolutePerpendicularDisplacement.Add(absolute);
                                                statisticContainer.SignedPerpendicularDisplacement.Add(signed);
                                            }

                                            // Calculate and fill Absolute/Signed MaximalPerpendicularDisplacementVmax
                                            DateTime maxVtime = trial.VelocityNormalized.First(t => Math.Sqrt(Math.Pow(t.X, 2) + Math.Pow(t.Y, 2)) == trial.VelocityNormalized.Max(u => Math.Sqrt(Math.Pow(u.X, 2) + Math.Pow(u.Y, 2)))).TimeStamp;
                                            statisticContainer.AbsoluteMaximalPerpendicularDisplacementVmax = statisticContainer.AbsolutePerpendicularDisplacement.First(t => t.TimeStamp == maxVtime).PerpendicularDisplacement;
                                            statisticContainer.SignedMaximalPerpendicularDisplacementVmax = statisticContainer.SignedPerpendicularDisplacement.First(t => t.TimeStamp == maxVtime).PerpendicularDisplacement;

                                            // Calculate MidMovementForce
                                            List<DateTime> vMaxCorridor = trial.VelocityNormalized.Where(t => (t.TimeStamp - maxVtime).TotalMilliseconds < 70).Select(t => t.TimeStamp).ToList();
                                            var perpendicularForcesMidMovementForce = new List<double>();
                                            var perpendicularForcesRawMidMovementForce = new List<double>();
                                            var parallelForcesMidMovementForce = new List<double>();
                                            var absoluteForcesMidMovementForce = new List<double>();
                                            var perpendicularForcesForcefieldCompenstionFactor = new List<double>();
                                            var perpendicularForcesRawForcefieldCompenstionFactor = new List<double>();

                                            for (int dataPoint = 2; dataPoint <= trial.PositionNormalized.Count; dataPoint++)
                                            {
                                                taskMatlabWrapper.Execute("[forcePD, forcePDsign] = pdForceLineSegment([forceX(" + (dataPoint - 1) + ") forceY(" + (dataPoint - 1) + ")], [positionX(" + (dataPoint - 1) + ") positionY(" + (dataPoint - 1) + ")], [positionX(" + dataPoint + ") positionY(" + dataPoint + ")]);");
                                                taskMatlabWrapper.Execute("[baselineForcePD, baselineForcePDsign] = pdForceLineSegment([baselineForceX(" + (dataPoint - 1) + ") baselineForceY(" + (dataPoint - 1) + ")], [baselinePositionX(" + (dataPoint - 1) + ") baselinePositionY(" + (dataPoint - 1) + ")], [baselinePositionX(" + dataPoint + ") baselinePositionY(" + dataPoint + ")]);");

                                                taskMatlabWrapper.Execute("forcePara = paraForceLineSegment([forceX(" + (dataPoint - 1) + ") forceY(" + (dataPoint - 1) + ")], [positionX(" + (dataPoint - 1) + ") positionY(" + (dataPoint - 1) + ")], [positionX(" + dataPoint + ") positionY(" + dataPoint + ")]);");
                                                taskMatlabWrapper.Execute("baselineForcePara = paraForceLineSegment([baselineForceX(" + (dataPoint - 1) + ") baselineForceY(" + (dataPoint - 1) + ")], [baselinePositionX(" + (dataPoint - 1) + ") baselinePositionY(" + (dataPoint - 1) + ")], [baselinePositionX(" + dataPoint + ") baselinePositionY(" + dataPoint + ")]);");

                                                taskMatlabWrapper.Execute("forcePD = forcePDsign * sqrt(forcePD(1)^2 + forcePD(2)^2);");
                                                taskMatlabWrapper.Execute("baselineForcePD = baselineForcePDsign * sqrt(baselineForcePD(1)^2 + baselineForcePD(2)^2);");

                                                taskMatlabWrapper.Execute("forcePara = sqrt(forcePara(1)^2 + forcePara(2)^2);");
                                                taskMatlabWrapper.Execute("baselineForcePara = sqrt(baselineForcePara(1)^2 + baselineForcePara(2)^2);");

                                                taskMatlabWrapper.Execute("absoluteForce = sqrt(forceX(" + (dataPoint - 1) + ")^2 + forceY(" + (dataPoint - 1) + ")^2);");
                                                taskMatlabWrapper.Execute("baselineAbsoluteForce = sqrt(baselineForceX(" + (dataPoint - 1) + ")^2 + baselineForceY(" + (dataPoint - 1) + ")^2);");

                                                perpendicularForcesForcefieldCompenstionFactor.Add(taskMatlabWrapper.GetWorkspaceData("forcePD") - taskMatlabWrapper.GetWorkspaceData("baselineForcePD"));
                                                perpendicularForcesRawForcefieldCompenstionFactor.Add(taskMatlabWrapper.GetWorkspaceData("forcePD"));

                                                if (vMaxCorridor.Contains(trial.PositionNormalized[dataPoint - 2].TimeStamp))
                                                {
                                                    perpendicularForcesMidMovementForce.Add(taskMatlabWrapper.GetWorkspaceData("forcePD") - taskMatlabWrapper.GetWorkspaceData("baselineForcePD"));
                                                    perpendicularForcesRawMidMovementForce.Add(taskMatlabWrapper.GetWorkspaceData("forcePD"));
                                                    parallelForcesMidMovementForce.Add(taskMatlabWrapper.GetWorkspaceData("forcePara") - taskMatlabWrapper.GetWorkspaceData("baselineForcePara"));
                                                    absoluteForcesMidMovementForce.Add(taskMatlabWrapper.GetWorkspaceData("absoluteForce") - taskMatlabWrapper.GetWorkspaceData("baselineAbsoluteForce"));
                                                }
                                            }

                                            statisticContainer.PerpendicularMidMovementForce = perpendicularForcesMidMovementForce.Average();
                                            statisticContainer.PerpendicularMidMovementForceRaw = perpendicularForcesRawMidMovementForce.Average();
                                            statisticContainer.ParallelMidMovementForce = parallelForcesMidMovementForce.Average();
                                            statisticContainer.AbsoluteMidMovementForce = absoluteForcesMidMovementForce.Average();

                                            // Calculate ForcefieldCompenstionFactor
                                            taskMatlabWrapper.SetWorkspaceData("forcePDArray", perpendicularForcesForcefieldCompenstionFactor.ToArray());
                                            taskMatlabWrapper.SetWorkspaceData("forcePDRawArray", perpendicularForcesRawForcefieldCompenstionFactor.ToArray());
                                            taskMatlabWrapper.Execute("forceCompFactor = forceCompensationFactor(forcePDArray, velocityX, velocityY);");
                                            taskMatlabWrapper.Execute("forceCompFactorRaw = forceCompensationFactor(forcePDRawArray, velocityX, velocityY);");

                                            statisticContainer.ForcefieldCompenstionFactor = taskMatlabWrapper.GetWorkspaceData("forceCompFactor");
                                            statisticContainer.ForcefieldCompenstionFactorRaw = taskMatlabWrapper.GetWorkspaceData("forceCompFactorRaw");

                                            // Set Metadata and upload to Database
                                            trial.Statistics = statisticContainer;
                                            trial.BaselineObjectId = baseline.Id;

                                            CompressTrialData(new List<Trial> {trial});
                                            _myDatabaseWrapper.UpdateTrialStatisticsAndBaselineId(trial);

                                            _myManipAnalysisGui.SetProgressBarValue((100.0/trialList.Count)*++counter);
                                        }
                                        else
                                        {
                                            _myManipAnalysisGui.WriteToLogBox("No matching Baseline for Trial: " + trial.Study + " / " + trial.Group + " / " + trial.Subject.PId + " / " + trial.Szenario + " / Trial " + trial.TrialNumberInSzenario + " / " + Enum.GetName(typeof (Trial.TrialTypeEnum), trial.TrialType) + " / " + Enum.GetName(typeof (Trial.ForceFieldTypeEnum), trial.ForceFieldType) + " / " + Enum.GetName(typeof (Trial.HandednessEnum), trial.Handedness));
                                        }
                                    }
                                }
                                catch (Exception
                                    ex)
                                {
                                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                                    taskMatlabWrapper.Dispose();
                                }

                                taskMatlabWrapper.Dispose();

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

        public void PlotTrajectoryVelocityForce(IEnumerable<TrajectoryVelocityPlotContainer> selectedTrials, string meanIndividual, string trajectoryVelocityForce, IEnumerable<Trial.TrialTypeEnum> trialTypes, IEnumerable<Trial.ForceFieldTypeEnum> forceFields, IEnumerable<Trial.HandednessEnum> handedness, bool showForceVectors, bool showPdForceVectors)
        {
            TaskManager.PushBack(Task.Factory.StartNew(() =>
            {
                try
                {
                    _myManipAnalysisGui.WriteProgressInfo("Getting data...");
                    List<TrajectoryVelocityPlotContainer> selectedTrialsList = selectedTrials.ToList();
                    double sumOfAllTrials = selectedTrialsList.Sum(t => t.Trials.Count);
                    double processedTrialsCount = 0;

                    var fields = new FieldsBuilder<Trial>();
                    // Neccessary for sorting!
                    fields.Include(t => t.TargetTrialNumberInSzenario);

                    if (trajectoryVelocityForce == "Velocity - Normalized")
                    {
                        fields.Include(t1 => t1.ZippedVelocityNormalized);
                        _myMatlabWrapper.CreateVelocityFigure("Velocity plot normalized", 101);
                    }
                    else if (trajectoryVelocityForce == "Velocity - Filtered")
                    {
                        fields.Include(t1 => t1.ZippedVelocityFiltered);
                        _myMatlabWrapper.CreateFigure("Velocity plot filtered", "[Samples]", "Velocity [m/s]");
                    }
                    else if (trajectoryVelocityForce == "Trajectory - Normalized")
                    {
                        if (showForceVectors || showPdForceVectors)
                        {
                            fields.Include(t1 => t1.ZippedPositionNormalized, t2 => t2.ZippedMeasuredForcesNormalized);
                            _myMatlabWrapper.CreateTrajectoryForceFigure("Trajectory plot normalized");
                        }
                        else
                        {
                            fields.Include(t1 => t1.ZippedPositionNormalized);
                            _myMatlabWrapper.CreateTrajectoryFigure("Trajectory plot normalized");
                        }
                        _myMatlabWrapper.DrawTargets(0.003, 0.1, 0, 0);
                    }
                    else if (trajectoryVelocityForce == "Trajectory - Filtered")
                    {
                        if (showForceVectors || showPdForceVectors)
                        {
                            fields.Include(t1 => t1.ZippedPositionFiltered, t2 => t2.ZippedMeasuredForcesFiltered);
                            _myMatlabWrapper.CreateTrajectoryForceFigure("Trajectory plot filtered");
                        }
                        else
                        {
                            fields.Include(t1 => t1.ZippedPositionFiltered);
                            _myMatlabWrapper.CreateTrajectoryFigure("Trajectory plot filtered");
                        }
                        _myMatlabWrapper.DrawTargets(0.003, 0.1, 0, 0);
                    }
                    else if (trajectoryVelocityForce == "Trajectory - Raw")
                    {
                        if (showForceVectors || showPdForceVectors)
                        {
                            fields.Include(t1 => t1.ZippedPositionRaw, t2 => t2.ZippedMeasuredForcesRaw);
                            _myMatlabWrapper.CreateTrajectoryForceFigure("Trajectory plot raw");
                        }
                        else
                        {
                            fields.Include(t1 => t1.ZippedPositionRaw);
                            _myMatlabWrapper.CreateTrajectoryFigure("Trajectory plot raw");
                        }
                        _myMatlabWrapper.DrawTargets(0.003, 0.1, 0, 0);
                    }
                    else if (trajectoryVelocityForce == "Force - Normalized")
                    {
                        fields.Include(t1 => t1.ZippedPositionNormalized, t2 => t2.ZippedMeasuredForcesNormalized);
                        _myMatlabWrapper.CreateForceFigure("Force plot normalized", "[Samples]", "Force [N]");
                    }
                    else if (trajectoryVelocityForce == "Force - Filtered")
                    {
                        fields.Include(t1 => t1.ZippedPositionFiltered, t2 => t2.ZippedMeasuredForcesFiltered);
                        _myMatlabWrapper.CreateForceFigure("Force plot filtered", "[Samples]", "Force [N]");
                    }
                    else if (trajectoryVelocityForce == "Force - Raw")
                    {
                        fields.Include(t1 => t1.ZippedPositionRaw, t2 => t2.ZippedMeasuredForcesRaw);
                        _myMatlabWrapper.CreateForceFigure("Force plot raw", "[Samples]", "Force [N]");
                    }

                    if (meanIndividual == "Individual")
                    {
                        foreach (TrajectoryVelocityPlotContainer
                            tempContainer
                            in
                            selectedTrialsList)
                        {
                            if (TaskManager.Cancel)
                            {
                                break;
                            }

                            DateTime turnDateTime = _myDatabaseWrapper.GetTurns(tempContainer.Study, tempContainer.Group, tempContainer.Szenario, tempContainer.Subject).OrderBy(t => t).ElementAt(tempContainer.Turn - 1);

                            Trial[] trialsArray = _myDatabaseWrapper.GetTrial(tempContainer.Study, tempContainer.Group, tempContainer.Szenario, tempContainer.Subject, turnDateTime, tempContainer.Target, tempContainer.Trials, trialTypes, forceFields, handedness, fields).ToArray();

                            for (int trialsArrayCounter = 0; trialsArrayCounter < trialsArray.Length; trialsArrayCounter++)
                            {
                                if (TaskManager.Cancel)
                                {
                                    break;
                                }

                                _myManipAnalysisGui.SetProgressBarValue((100.0/sumOfAllTrials)*processedTrialsCount++);

                                if (trialsArray != null)
                                {
                                    if (trajectoryVelocityForce == "Velocity - Normalized")
                                    {
                                        trialsArray[trialsArrayCounter].VelocityNormalized = Gzip<List<VelocityContainer>>.DeCompress(trialsArray[trialsArrayCounter].ZippedVelocityNormalized).OrderBy(t => t.TimeStamp).ToList();
                                        _myMatlabWrapper.SetWorkspaceData("velocity", trialsArray[trialsArrayCounter].VelocityNormalized.Select(t => Math.Sqrt(Math.Pow(t.X, 2) + Math.Pow(t.Y, 2))).ToArray());
                                        _myMatlabWrapper.Plot("velocity", "black", 2);
                                    }
                                    else if (trajectoryVelocityForce == "Velocity - Filtered")
                                    {
                                        trialsArray[trialsArrayCounter].VelocityFiltered = Gzip<List<VelocityContainer>>.DeCompress(trialsArray[trialsArrayCounter].ZippedVelocityFiltered).OrderBy(t => t.TimeStamp).ToList();
                                        _myMatlabWrapper.SetWorkspaceData("velocity", trialsArray[trialsArrayCounter].VelocityFiltered.Select(t => Math.Sqrt(Math.Pow(t.X, 2) + Math.Pow(t.Y, 2))).ToArray());
                                        _myMatlabWrapper.Plot("velocity", "black", 2);
                                    }
                                    else if (trajectoryVelocityForce == "Trajectory - Normalized")
                                    {
                                        trialsArray[trialsArrayCounter].PositionNormalized = Gzip<List<PositionContainer>>.DeCompress(trialsArray[trialsArrayCounter].ZippedPositionNormalized).OrderBy(t => t.TimeStamp).ToList();

                                        _myMatlabWrapper.SetWorkspaceData("positionDataX", trialsArray[trialsArrayCounter].PositionNormalized.Select(t => t.X).ToArray());
                                        _myMatlabWrapper.SetWorkspaceData("positionDataY", trialsArray[trialsArrayCounter].PositionNormalized.Select(t => t.Y).ToArray());

                                        _myMatlabWrapper.Plot("positionDataX", "positionDataY", "black", 2);

                                        if (showForceVectors || showPdForceVectors)
                                        {
                                            trialsArray[trialsArrayCounter].MeasuredForcesNormalized = Gzip<List<ForceContainer>>.DeCompress(trialsArray[trialsArrayCounter].ZippedMeasuredForcesNormalized).OrderBy(t => t.TimeStamp).ToList();
                                            for (int i = 2; i <= trialsArray[trialsArrayCounter].PositionNormalized.Count & !TaskManager.Pause; i++)
                                            {
                                                _myMatlabWrapper.SetWorkspaceData("vpos1", new[] {trialsArray[trialsArrayCounter].PositionNormalized.Select(t => t.X).ElementAt(i - 2), trialsArray[trialsArrayCounter].PositionNormalized.Select(t => t.Y).ElementAt(i - 2)});

                                                _myMatlabWrapper.SetWorkspaceData("vpos2", new[] {trialsArray[trialsArrayCounter].PositionNormalized.Select(t => t.X).ElementAt(i - 1), trialsArray[trialsArrayCounter].PositionNormalized.Select(t => t.Y).ElementAt(i - 1)});

                                                _myMatlabWrapper.SetWorkspaceData("vforce", new[] {trialsArray[trialsArrayCounter].MeasuredForcesNormalized.Select(t => t.X).ElementAt(i - 2)/100.0, trialsArray[trialsArrayCounter].MeasuredForcesNormalized.Select(t => t.Y).ElementAt(i - 2)/100.0});

                                                if (showForceVectors)
                                                {
                                                    _myMatlabWrapper.Execute("quiver(vpos2(1),vpos2(2),vforce(1),vforce(2),'Color','red');");
                                                }
                                                if (showPdForceVectors)
                                                {
                                                    _myMatlabWrapper.Execute("[fPD, fPDsign] = pdForceLineSegment([vforce(1,1) vforce(1,2)], [vpos1(1,1) vpos1(1,2)], [vpos2(1,1) vpos2(1,2)]);");
                                                    _myMatlabWrapper.Execute("quiver(vpos2(1),vpos2(2),fPD(1),fPD(2),'Color','blue');");
                                                }
                                            }
                                        }
                                    }
                                    else if (trajectoryVelocityForce == "Trajectory - Filtered")
                                    {
                                        trialsArray[trialsArrayCounter].PositionFiltered = Gzip<List<PositionContainer>>.DeCompress(trialsArray[trialsArrayCounter].ZippedPositionFiltered).OrderBy(t => t.TimeStamp).ToList();

                                        _myMatlabWrapper.SetWorkspaceData("positionDataX", trialsArray[trialsArrayCounter].PositionFiltered.Select(t => t.X).ToArray());
                                        _myMatlabWrapper.SetWorkspaceData("positionDataY", trialsArray[trialsArrayCounter].PositionFiltered.Select(t => t.Y).ToArray());

                                        _myMatlabWrapper.Plot("positionDataX", "positionDataY", "black", 2);

                                        if (showForceVectors || showPdForceVectors)
                                        {
                                            trialsArray[trialsArrayCounter].MeasuredForcesFiltered = Gzip<List<ForceContainer>>.DeCompress(trialsArray[trialsArrayCounter].ZippedMeasuredForcesFiltered).OrderBy(t => t.TimeStamp).ToList();
                                            for (int i = 2; i <= trialsArray[trialsArrayCounter].PositionFiltered.Count & !TaskManager.Pause; i++)
                                            {
                                                _myMatlabWrapper.SetWorkspaceData("vpos1", new[] {trialsArray[trialsArrayCounter].PositionFiltered.Select(t => t.X).ElementAt(i - 2), trialsArray[trialsArrayCounter].PositionFiltered.Select(t => t.Y).ElementAt(i - 2)});

                                                _myMatlabWrapper.SetWorkspaceData("vpos2", new[] {trialsArray[trialsArrayCounter].PositionFiltered.Select(t => t.X).ElementAt(i - 1), trialsArray[trialsArrayCounter].PositionFiltered.Select(t => t.Y).ElementAt(i - 1)});

                                                _myMatlabWrapper.SetWorkspaceData("vforce", new[] {trialsArray[trialsArrayCounter].MeasuredForcesFiltered.Select(t => t.X).ElementAt(i - 2)/100.0, trialsArray[trialsArrayCounter].MeasuredForcesFiltered.Select(t => t.Y).ElementAt(i - 2)/100.0});

                                                if (showForceVectors)
                                                {
                                                    _myMatlabWrapper.Execute("quiver3(vpos2(1),vpos2(2),vforce(1),vforce(2),'Color','red');");
                                                }
                                                if (showPdForceVectors)
                                                {
                                                    _myMatlabWrapper.Execute("[fPD, fPDsign] = pdForceLineSegment([vforce(1,1) vforce(1,2)], [vpos1(1,1) vpos1(1,2)], [vpos2(1,1) vpos2(1,2)]);");
                                                    _myMatlabWrapper.Execute("quiver(vpos2(1),vpos2(2),fPD(1),fPD(2),'Color','blue');");
                                                }
                                            }
                                        }
                                    }
                                    else if (trajectoryVelocityForce == "Trajectory - Raw")
                                    {
                                        trialsArray[trialsArrayCounter].PositionRaw = Gzip<List<PositionContainer>>.DeCompress(trialsArray[trialsArrayCounter].ZippedPositionRaw).OrderBy(t => t.TimeStamp).ToList();

                                        _myMatlabWrapper.SetWorkspaceData("positionDataX", trialsArray[trialsArrayCounter].PositionRaw.Select(t => t.X).ToArray());
                                        _myMatlabWrapper.SetWorkspaceData("positionDataY", trialsArray[trialsArrayCounter].PositionRaw.Select(t => t.Y).ToArray());

                                        _myMatlabWrapper.Plot("positionDataX", "positionDataY", "black", 2);

                                        if (showForceVectors || showPdForceVectors)
                                        {
                                            trialsArray[trialsArrayCounter].MeasuredForcesRaw = Gzip<List<ForceContainer>>.DeCompress(trialsArray[trialsArrayCounter].ZippedMeasuredForcesRaw).OrderBy(t => t.TimeStamp).ToList();
                                            for (int i = 2; i <= trialsArray[trialsArrayCounter].PositionRaw.Count & !TaskManager.Pause; i++)
                                            {
                                                _myMatlabWrapper.SetWorkspaceData("vpos1", new[] {trialsArray[trialsArrayCounter].PositionRaw.Select(t => t.X).ElementAt(i - 2), trialsArray[trialsArrayCounter].PositionRaw.Select(t => t.Y).ElementAt(i - 2)});

                                                _myMatlabWrapper.SetWorkspaceData("vpos2", new[] {trialsArray[trialsArrayCounter].PositionRaw.Select(t => t.X).ElementAt(i - 1), trialsArray[trialsArrayCounter].PositionRaw.Select(t => t.Y).ElementAt(i - 1)});

                                                _myMatlabWrapper.SetWorkspaceData("vforce", new[] {trialsArray[trialsArrayCounter].MeasuredForcesRaw.Select(t => t.X).ElementAt(i - 2)/100.0, trialsArray[trialsArrayCounter].MeasuredForcesRaw.Select(t => t.Y).ElementAt(i - 2)/100.0});

                                                if (showForceVectors)
                                                {
                                                    _myMatlabWrapper.Execute("quiver3(vpos2(1),vpos2(2),vforce(1),vforce(2),'Color','red');");
                                                }
                                                if (showPdForceVectors)
                                                {
                                                    _myMatlabWrapper.Execute("[fPD, fPDsign] = pdForceLineSegment([vforce(1,1) vforce(1,2)], [vpos1(1,1) vpos1(1,2)], [vpos2(1,1) vpos2(1,2)]);");
                                                    _myMatlabWrapper.Execute("quiver(vpos2(1),vpos2(2),fPD(1),fPD(2),'Color','blue');");
                                                }
                                            }
                                        }
                                    }
                                    else if (trajectoryVelocityForce == "Force - Normalized")
                                    {
                                        trialsArray[trialsArrayCounter].PositionNormalized = Gzip<List<PositionContainer>>.DeCompress(trialsArray[trialsArrayCounter].ZippedPositionNormalized).OrderBy(t => t.TimeStamp).ToList();
                                        trialsArray[trialsArrayCounter].MeasuredForcesNormalized = Gzip<List<ForceContainer>>.DeCompress(trialsArray[trialsArrayCounter].ZippedMeasuredForcesNormalized).OrderBy(t => t.TimeStamp).ToList();

                                        _myMatlabWrapper.Execute("forcePDVector = zeros(1, " + (trialsArray[trialsArrayCounter].PositionNormalized.Count - 1) + ");");
                                        _myMatlabWrapper.Execute("forceParaVector = zeros(1, " + (trialsArray[trialsArrayCounter].PositionNormalized.Count - 1) + ");");
                                        _myMatlabWrapper.Execute("forceAbsVector = zeros(1, " + (trialsArray[trialsArrayCounter].PositionNormalized.Count - 1) + ");");

                                        for (int i = 2; i <= trialsArray[trialsArrayCounter].PositionNormalized.Count & !TaskManager.Pause; i++)
                                        {
                                            _myMatlabWrapper.SetWorkspaceData("vpos1", new[] { trialsArray[trialsArrayCounter].PositionNormalized.Select(t => t.X).ElementAt(i - 2), trialsArray[trialsArrayCounter].PositionNormalized.Select(t => t.Y).ElementAt(i - 2) });
                                            _myMatlabWrapper.SetWorkspaceData("vpos2", new[] { trialsArray[trialsArrayCounter].PositionNormalized.Select(t => t.X).ElementAt(i - 1), trialsArray[trialsArrayCounter].PositionNormalized.Select(t => t.Y).ElementAt(i - 1) });
                                            _myMatlabWrapper.SetWorkspaceData("vforce", new[] { trialsArray[trialsArrayCounter].MeasuredForcesNormalized.Select(t => t.X).ElementAt(i - 2), trialsArray[trialsArrayCounter].MeasuredForcesNormalized.Select(t => t.Y).ElementAt(i - 2) });
                                            
                                            _myMatlabWrapper.Execute("[fPD, fPDsign] = pdForceLineSegment([vforce(1,1) vforce(1,2)], [vpos1(1,1) vpos1(1,2)], [vpos2(1,1) vpos2(1,2)]);");
                                            _myMatlabWrapper.Execute("[fPara, fParasign] = paraForceLineSegment([vforce(1,1) vforce(1,2)], [vpos1(1,1) vpos1(1,2)], [vpos2(1,1) vpos2(1,2)]);");

                                            _myMatlabWrapper.Execute("forcePDVector(" + (i - 1) + ") = sqrt(fPD(1)^2 + fPD(2)^2) * fPDsign;");
                                            _myMatlabWrapper.Execute("forceParaVector(" + (i - 1) + ") = sqrt(fPara(1)^2 + fPara(2)^2) * fParasign;");
                                            _myMatlabWrapper.Execute("forceAbsVector(" + (i - 1) + ") = sqrt(vforce(1,1)^2 + vforce(1,2)^2);");
                                        }

                                        _myMatlabWrapper.Plot("forcePDVector", "blue", 2);
                                        _myMatlabWrapper.Plot("forceParaVector", "red", 2);
                                        _myMatlabWrapper.Plot("forceAbsVector", "black", 2);
                                        _myMatlabWrapper.AddLegend("Force PD", "Force Para", "Force Abs");
                                    }
                                    else if (trajectoryVelocityForce == "Force - Filtered")
                                    {
                                        trialsArray[trialsArrayCounter].PositionFiltered = Gzip<List<PositionContainer>>.DeCompress(trialsArray[trialsArrayCounter].ZippedPositionFiltered).OrderBy(t => t.TimeStamp).ToList();
                                        trialsArray[trialsArrayCounter].MeasuredForcesFiltered = Gzip<List<ForceContainer>>.DeCompress(trialsArray[trialsArrayCounter].ZippedMeasuredForcesFiltered).OrderBy(t => t.TimeStamp).ToList();

                                        _myMatlabWrapper.Execute("forcePDVector = zeros(1, " + (trialsArray[trialsArrayCounter].PositionFiltered.Count - 1) + ");");
                                        _myMatlabWrapper.Execute("forceParaVector = zeros(1, " + (trialsArray[trialsArrayCounter].PositionFiltered.Count - 1) + ");");
                                        _myMatlabWrapper.Execute("forceAbsVector = zeros(1, " + (trialsArray[trialsArrayCounter].PositionFiltered.Count - 1) + ");");

                                        for (int i = 2; i <= trialsArray[trialsArrayCounter].PositionFiltered.Count & !TaskManager.Pause; i++)
                                        {
                                            _myMatlabWrapper.SetWorkspaceData("vpos1", new[] { trialsArray[trialsArrayCounter].PositionFiltered.Select(t => t.X).ElementAt(i - 2), trialsArray[trialsArrayCounter].PositionFiltered.Select(t => t.Y).ElementAt(i - 2) });
                                            _myMatlabWrapper.SetWorkspaceData("vpos2", new[] { trialsArray[trialsArrayCounter].PositionFiltered.Select(t => t.X).ElementAt(i - 1), trialsArray[trialsArrayCounter].PositionFiltered.Select(t => t.Y).ElementAt(i - 1) });
                                            _myMatlabWrapper.SetWorkspaceData("vforce", new[] { trialsArray[trialsArrayCounter].MeasuredForcesFiltered.Select(t => t.X).ElementAt(i - 2), trialsArray[trialsArrayCounter].MeasuredForcesFiltered.Select(t => t.Y).ElementAt(i - 2) });

                                            _myMatlabWrapper.Execute("[fPD, fPDsign] = pdForceLineSegment([vforce(1,1) vforce(1,2)], [vpos1(1,1) vpos1(1,2)], [vpos2(1,1) vpos2(1,2)]);");
                                            _myMatlabWrapper.Execute("[fPara, fParasign] = paraForceLineSegment([vforce(1,1) vforce(1,2)], [vpos1(1,1) vpos1(1,2)], [vpos2(1,1) vpos2(1,2)]);");

                                            _myMatlabWrapper.Execute("forcePDVector(" + (i - 1) + ") = sqrt(fPD(1)^2 + fPD(2)^2) * fPDsign;");
                                            _myMatlabWrapper.Execute("forceParaVector(" + (i - 1) + ") = sqrt(fPara(1)^2 + fPara(2)^2) * fParasign;");
                                            _myMatlabWrapper.Execute("forceAbsVector(" + (i - 1) + ") = sqrt(vforce(1,1)^2 + vforce(1,2)^2);");
                                        }

                                        _myMatlabWrapper.Plot("forcePDVector", "blue", 2);
                                        _myMatlabWrapper.Plot("forceParaVector", "red", 2);
                                        _myMatlabWrapper.Plot("forceAbsVector", "black", 2);
                                        _myMatlabWrapper.AddLegend("Force PD", "Force Para", "Force Abs");
                                    }
                                    else if (trajectoryVelocityForce == "Force - Raw")
                                    {
                                        trialsArray[trialsArrayCounter].PositionRaw = Gzip<List<PositionContainer>>.DeCompress(trialsArray[trialsArrayCounter].ZippedPositionRaw).OrderBy(t => t.TimeStamp).ToList();
                                        trialsArray[trialsArrayCounter].MeasuredForcesRaw = Gzip<List<ForceContainer>>.DeCompress(trialsArray[trialsArrayCounter].ZippedMeasuredForcesRaw).OrderBy(t => t.TimeStamp).ToList();

                                        _myMatlabWrapper.Execute("forcePDVector = zeros(1, " + (trialsArray[trialsArrayCounter].PositionRaw.Count - 1) + ");");
                                        _myMatlabWrapper.Execute("forceParaVector = zeros(1, " + (trialsArray[trialsArrayCounter].PositionRaw.Count - 1) + ");");
                                        _myMatlabWrapper.Execute("forceAbsVector = zeros(1, " + (trialsArray[trialsArrayCounter].PositionRaw.Count - 1) + ");");

                                        for (int i = 2; i <= trialsArray[trialsArrayCounter].PositionRaw.Count & !TaskManager.Pause; i++)
                                        {
                                            _myMatlabWrapper.SetWorkspaceData("vpos1", new[] { trialsArray[trialsArrayCounter].PositionRaw.Select(t => t.X).ElementAt(i - 2), trialsArray[trialsArrayCounter].PositionRaw.Select(t => t.Y).ElementAt(i - 2) });
                                            _myMatlabWrapper.SetWorkspaceData("vpos2", new[] { trialsArray[trialsArrayCounter].PositionRaw.Select(t => t.X).ElementAt(i - 1), trialsArray[trialsArrayCounter].PositionRaw.Select(t => t.Y).ElementAt(i - 1) });
                                            _myMatlabWrapper.SetWorkspaceData("vforce", new[] { trialsArray[trialsArrayCounter].MeasuredForcesRaw.Select(t => t.X).ElementAt(i - 2), trialsArray[trialsArrayCounter].MeasuredForcesRaw.Select(t => t.Y).ElementAt(i - 2) });

                                            _myMatlabWrapper.Execute("[fPD, fPDsign] = pdForceLineSegment([vforce(1,1) vforce(1,2)], [vpos1(1,1) vpos1(1,2)], [vpos2(1,1) vpos2(1,2)]);");
                                            _myMatlabWrapper.Execute("[fPara, fParasign] = paraForceLineSegment([vforce(1,1) vforce(1,2)], [vpos1(1,1) vpos1(1,2)], [vpos2(1,1) vpos2(1,2)]);");

                                            _myMatlabWrapper.Execute("forcePDVector(" + (i - 1) + ") = sqrt(fPD(1)^2 + fPD(2)^2) * fPDsign;");
                                            _myMatlabWrapper.Execute("forceParaVector(" + (i - 1) + ") = sqrt(fPara(1)^2 + fPara(2)^2) * fParasign;");
                                            _myMatlabWrapper.Execute("forceAbsVector(" + (i - 1) + ") = sqrt(vforce(1,1)^2 + vforce(1,2)^2);");
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
                            int[] targetArray = selectedTrialsList.Select(t => t.Target).Distinct().ToArray();

                            for (int targetCounter = 0; targetCounter < targetArray.Length & !TaskManager.Cancel; targetCounter++)
                            {
                                var positionData = new List<List<PositionContainer>>();
                                var velocityData = new List<List<VelocityContainer>>();
                                var forceData = new List<List<ForceContainer>>();

                                foreach (TrajectoryVelocityPlotContainer tempContainer in selectedTrialsList.Where(t => t.Target == targetArray[targetCounter]))
                                {
                                    if (TaskManager.Cancel)
                                    {
                                        break;
                                    }

                                    DateTime turnDateTime = _myDatabaseWrapper.GetTurns(tempContainer.Study, tempContainer.Group, tempContainer.Szenario, tempContainer.Subject).OrderBy(t => t).ElementAt(tempContainer.Turn - 1);

                                    Trial[] trialsArray = _myDatabaseWrapper.GetTrial(tempContainer.Study, tempContainer.Group, tempContainer.Szenario, tempContainer.Subject, turnDateTime, tempContainer.Target, tempContainer.Trials, trialTypes, forceFields, handedness, fields).ToArray();

                                    for (int trialsArrayCounter = 0; trialsArrayCounter < trialsArray.Length; trialsArrayCounter++)
                                    {
                                        if (TaskManager.Cancel)
                                        {
                                            break;
                                        }

                                        _myManipAnalysisGui.SetProgressBarValue((100.0/sumOfAllTrials)*processedTrialsCount++);

                                        if (trialsArray != null)
                                        {
                                            if (trajectoryVelocityForce == "Trajectory - Normalized")
                                            {
                                                trialsArray[trialsArrayCounter].PositionNormalized = Gzip<List<PositionContainer>>.DeCompress(trialsArray[trialsArrayCounter].ZippedPositionNormalized).OrderBy(t => t.TimeStamp).ToList();
                                                positionData.Add(trialsArray[trialsArrayCounter].PositionNormalized);

                                                if (showForceVectors || showPdForceVectors)
                                                {
                                                    trialsArray[trialsArrayCounter].MeasuredForcesNormalized = Gzip<List<ForceContainer>>.DeCompress(trialsArray[trialsArrayCounter].ZippedMeasuredForcesNormalized).OrderBy(t => t.TimeStamp).ToList();
                                                    forceData.Add(trialsArray[trialsArrayCounter].MeasuredForcesNormalized);
                                                }
                                            }
                                            else if (trajectoryVelocityForce == "Velocity - Normalized")
                                            {
                                                trialsArray[trialsArrayCounter].VelocityNormalized = Gzip<List<VelocityContainer>>.DeCompress(trialsArray[trialsArrayCounter].ZippedVelocityNormalized).OrderBy(t => t.TimeStamp).ToList();
                                                velocityData.Add(trialsArray[trialsArrayCounter].VelocityNormalized);
                                            }
                                            else if (trajectoryVelocityForce == "Force - Normalized")
                                            {
                                                trialsArray[trialsArrayCounter].PositionNormalized = Gzip<List<PositionContainer>>.DeCompress(trialsArray[trialsArrayCounter].ZippedPositionNormalized).OrderBy(t => t.TimeStamp).ToList();
                                                trialsArray[trialsArrayCounter].MeasuredForcesNormalized = Gzip<List<ForceContainer>>.DeCompress(trialsArray[trialsArrayCounter].ZippedMeasuredForcesNormalized).OrderBy(t => t.TimeStamp).ToList();
                                                positionData.Add(trialsArray[trialsArrayCounter].PositionNormalized);
                                                forceData.Add(trialsArray[trialsArrayCounter].MeasuredForcesNormalized);
                                            }
                                            else
                                            {
                                                _myManipAnalysisGui.WriteToLogBox("Mean can only be calculated for normalized values.");
                                            }
                                        }
                                    }
                                }

                                int frameCount = 0;
                                int meanCount = 0;
                                
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

                                    for (int meanCounter = 0; meanCounter < meanCount; meanCounter++)
                                    {
                                        for (int frameCounter = 0; frameCounter < frameCount; frameCounter++)
                                        {
                                            if (trajectoryVelocityForce == "Trajectory - Normalized")
                                            {
                                                xData[frameCounter] += positionData[meanCounter][frameCounter].X;
                                                yData[frameCounter] += positionData[meanCounter][frameCounter].Y;

                                                if (showForceVectors || showPdForceVectors)
                                                {
                                                    forceVectorDataX[frameCounter] += forceData[meanCounter][frameCounter].X;
                                                    forceVectorDataY[frameCounter] += forceData[meanCounter][frameCounter].Y;
                                                }
                                            }
                                            else if (trajectoryVelocityForce == "Velocity - Normalized")
                                            {
                                                xData[frameCounter] += Math.Sqrt(Math.Pow(velocityData[meanCounter][frameCounter].X, 2) + Math.Pow(velocityData[meanCounter][frameCounter].Y, 2));
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

                                    for (int frameCounter = 0; frameCounter < frameCount; frameCounter++)
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
                                            for (int i = 2; i <= xData.Length & !TaskManager.Pause; i++)
                                            {
                                                _myMatlabWrapper.SetWorkspaceData("vpos1", new[] { xData[i - 2], yData[i - 2] });

                                                _myMatlabWrapper.SetWorkspaceData("vpos2", new[] { xData[i - 1], yData[i - 1] });

                                                _myMatlabWrapper.SetWorkspaceData("vforce", new[] { forceVectorDataX[i - 2] / 100.0, forceVectorDataY[i - 2] / 100.0 });

                                                if (showForceVectors)
                                                {
                                                    _myMatlabWrapper.Execute("quiver(vpos2(1),vpos2(2),vforce(1),vforce(2),'Color','red');");
                                                }
                                                if (showPdForceVectors)
                                                {
                                                    _myMatlabWrapper.Execute("[fPD, fPDsign] = pdForceLineSegment([vforce(1,1) vforce(1,2)], [vpos1(1,1) vpos1(1,2)], [vpos2(1,1) vpos2(1,2)]);");
                                                    _myMatlabWrapper.Execute("quiver(vpos2(1),vpos2(2),fPD(1),fPD(2),'Color','blue');");
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
                                        _myMatlabWrapper.Execute("forceParaVector = zeros(1, " + (xData.Length - 1) + ");");
                                        _myMatlabWrapper.Execute("forceAbsVector = zeros(1, " + (xData.Length - 1) + ");");

                                        for (int i = 2; i <= xData.Length & !TaskManager.Pause; i++)
                                        {
                                            _myMatlabWrapper.SetWorkspaceData("vpos1", new[] { xData[i - 2], yData[i - 2] });
                                            _myMatlabWrapper.SetWorkspaceData("vpos2", new[] { xData[i - 1], yData[i - 1] });
                                            _myMatlabWrapper.SetWorkspaceData("vforce", new[] { forceVectorDataX[i - 2], forceVectorDataY[i - 2] });

                                            _myMatlabWrapper.Execute("[fPD, fPDsign] = pdForceLineSegment([vforce(1,1) vforce(1,2)], [vpos1(1,1) vpos1(1,2)], [vpos2(1,1) vpos2(1,2)]);");
                                            _myMatlabWrapper.Execute("[fPara, fParasign] = paraForceLineSegment([vforce(1,1) vforce(1,2)], [vpos1(1,1) vpos1(1,2)], [vpos2(1,1) vpos2(1,2)]);");

                                            _myMatlabWrapper.Execute("forcePDVector(" + (i - 1) + ") = sqrt(fPD(1)^2 + fPD(2)^2) * fPDsign;");
                                            _myMatlabWrapper.Execute("forceParaVector(" + (i - 1) + ") = sqrt(fPara(1)^2 + fPara(2)^2) * fParasign;");
                                            _myMatlabWrapper.Execute("forceAbsVector(" + (i - 1) + ") = sqrt(vforce(1,1)^2 + vforce(1,2)^2);");
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

        public void ExportTrajectoryVelocityForce(IEnumerable<TrajectoryVelocityPlotContainer> selectedTrials, string meanIndividual, string trajectoryVelocityForce, IEnumerable<Trial.TrialTypeEnum> trialTypes, IEnumerable<Trial.ForceFieldTypeEnum> forceFields, IEnumerable<Trial.HandednessEnum> handedness, bool showForceVectors, bool showPdForceVectors, string fileName)
        {
            TaskManager.PushBack(Task.Factory.StartNew(() =>
            {
                _myManipAnalysisGui.WriteProgressInfo("Getting data...");
                List<TrajectoryVelocityPlotContainer> selectedTrialsList = selectedTrials.ToList();
                double sumOfAllTrials = selectedTrialsList.Sum(t => t.Trials.Count);
                double processedTrialsCount = 0;

                var fields = new FieldsBuilder<Trial>();
                var dataFileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                var dataFileWriter = new StreamWriter(dataFileStream);

                if (trajectoryVelocityForce == "Velocity - Normalized")
                {
                    fields.Include(t1 => t1.ZippedVelocityNormalized);
                    if (meanIndividual == "Individual")
                    {
                        dataFileWriter.WriteLine("Study;Group;Szenario;Subject;Turn;Target;Trial;TimeStamp;VelocityX;VelocityY");
                    }
                    else if (meanIndividual == "Mean")
                    {
                        dataFileWriter.WriteLine("Study;Group;Szenario;Subject;Turn;Target;Trial;DataPoint;VelocityX;VelocityY");
                    }
                }
                else if (trajectoryVelocityForce == "Velocity - Filtered")
                {
                    fields.Include(t1 => t1.ZippedVelocityFiltered);
                    if (meanIndividual == "Individual")
                    {
                        dataFileWriter.WriteLine("Study;Group;Szenario;Subject;Turn;Target;Trial;TimeStamp;VelocityX;VelocityY");
                    }
                    else if (meanIndividual == "Mean")
                    {
                        dataFileWriter.WriteLine("Study;Group;Szenario;Subject;Turn;Target;Trial;DataPoint;VelocityX;VelocityY");
                    }
                }
                else if (trajectoryVelocityForce == "Trajectory - Normalized")
                {
                    fields.Include(t1 => t1.ZippedPositionNormalized);
                    if (meanIndividual == "Individual")
                    {
                        dataFileWriter.WriteLine("Study;Group;Szenario;Subject;Turn;Target;Trial;TimeStamp;PositionCartesianX;PositionCartesianY");
                    }
                    else if (meanIndividual == "Mean")
                    {
                        dataFileWriter.WriteLine("Study;Group;Szenario;Subject;Turn;Target;Trial;DataPoint;PositionCartesianX;PositionCartesianY");
                    }
                }
                else if (trajectoryVelocityForce == "Trajectory - Filtered")
                {
                    fields.Include(t1 => t1.ZippedPositionFiltered);
                    if (meanIndividual == "Individual")
                    {
                        dataFileWriter.WriteLine("Study;Group;Szenario;Subject;Turn;Target;Trial;TimeStamp;PositionCartesianX;PositionCartesianY");
                    }
                    else if (meanIndividual == "Mean")
                    {
                        dataFileWriter.WriteLine("Study;Group;Szenario;Subject;Turn;Target;Trial;DataPoint;PositionCartesianX;PositionCartesianY");
                    }
                }
                else if (trajectoryVelocityForce == "Trajectory - Raw")
                {
                    fields.Include(t1 => t1.ZippedPositionRaw);
                    if (meanIndividual == "Individual")
                    {
                        dataFileWriter.WriteLine("Study;Group;Szenario;Subject;Turn;Target;Trial;TimeStamp;PositionCartesianX;PositionCartesianY");
                    }
                    else if (meanIndividual == "Mean")
                    {
                        dataFileWriter.WriteLine("Study;Group;Szenario;Subject;Turn;Target;Trial;DataPoint;PositionCartesianX;PositionCartesianY");
                    }
                }
                else if (trajectoryVelocityForce == "Force - Normalized")
                {
                    fields.Include(t1 => t1.ZippedMeasuredForcesNormalized);
                    if (meanIndividual == "Individual")
                    {
                        dataFileWriter.WriteLine("Study;Group;Szenario;Subject;Turn;Target;Trial;TimeStamp;MeasuredForcesX;MeasuredForcesY");
                    }
                    else if (meanIndividual == "Mean")
                    {
                        dataFileWriter.WriteLine("Study;Group;Szenario;Subject;Turn;Target;Trial;DataPoint;MeasuredForcesX;MeasuredForcesY");
                    }
                }
                else if (trajectoryVelocityForce == "Force - Filtered")
                {
                    fields.Include(t1 => t1.ZippedMeasuredForcesFiltered);
                    if (meanIndividual == "Individual")
                    {
                        dataFileWriter.WriteLine("Study;Group;Szenario;Subject;Turn;Target;Trial;TimeStamp;MeasuredForcesX;MeasuredForcesY");
                    }
                    else if (meanIndividual == "Mean")
                    {
                        dataFileWriter.WriteLine("Study;Group;Szenario;Subject;Turn;Target;Trial;DataPoint;MeasuredForcesX;MeasuredForcesY");
                    }
                }
                else if (trajectoryVelocityForce == "Force - Raw")
                {
                    fields.Include(t1 => t1.ZippedMeasuredForcesRaw);
                    if (meanIndividual == "Individual")
                    {
                        dataFileWriter.WriteLine("Study;Group;Szenario;Subject;Turn;Target;Trial;TimeStamp;MeasuredForcesX;MeasuredForcesY");
                    }
                    else if (meanIndividual == "Mean")
                    {
                        dataFileWriter.WriteLine("Study;Group;Szenario;Subject;Turn;Target;Trial;DataPoint;MeasuredForcesX;MeasuredForcesY");
                    }
                }

                if (meanIndividual == "Individual")
                {
                    foreach (TrajectoryVelocityPlotContainer
                        tempContainer
                        in
                        selectedTrialsList)
                    {
                        if (TaskManager.Cancel)
                        {
                            break;
                        }

                        DateTime turnDateTime = _myDatabaseWrapper.GetTurns(tempContainer.Study, tempContainer.Group, tempContainer.Szenario, tempContainer.Subject).OrderBy(t => t).ElementAt(tempContainer.Turn - 1);

                        Trial[] trialsArray = _myDatabaseWrapper.GetTrials(tempContainer.Study, tempContainer.Group, tempContainer.Szenario, tempContainer.Subject, turnDateTime, tempContainer.Trials, trialTypes, forceFields, handedness, fields).ToArray();

                        for (int trialsArrayCounter = 0; trialsArrayCounter < tempContainer.Trials.Count & !TaskManager.Cancel; trialsArrayCounter++)
                        {
                            if (TaskManager.Cancel)
                            {
                                break;
                            }

                            _myManipAnalysisGui.SetProgressBarValue((100.0/sumOfAllTrials)*processedTrialsCount++);

                            if (trialsArray != null)
                            {
                                if (trajectoryVelocityForce == "Velocity - Normalized")
                                {
                                    trialsArray[trialsArrayCounter].VelocityNormalized = Gzip<List<VelocityContainer>>.DeCompress(trialsArray[trialsArrayCounter].ZippedVelocityNormalized).OrderBy(t => t.TimeStamp).ToList();
                                    for (int i = 0; i < trialsArray[trialsArrayCounter].VelocityNormalized.Count; i++)
                                    {
                                        dataFileWriter.WriteLine(tempContainer.Study + ";" + tempContainer.Group + ";" + tempContainer.Szenario + ";" + tempContainer.Subject.PId + ";" + tempContainer.Turn + ";" + tempContainer.Target + ";" + trialsArray[trialsArrayCounter].TargetTrialNumberInSzenario + ";" + trialsArray[trialsArrayCounter].VelocityNormalized[i].TimeStamp.ToString("dd.MM.yyyy HH:mm:ss.fffffff") + ";" + DoubleConverter.ToExactString(Convert.ToDouble(trialsArray[trialsArrayCounter].VelocityNormalized[i].X)) + ";" + DoubleConverter.ToExactString(Convert.ToDouble(trialsArray[trialsArrayCounter].VelocityNormalized[i].Y)));
                                    }
                                }
                                else if (trajectoryVelocityForce == "Velocity - Filtered")
                                {
                                    trialsArray[trialsArrayCounter].VelocityFiltered = Gzip<List<VelocityContainer>>.DeCompress(trialsArray[trialsArrayCounter].ZippedVelocityFiltered).OrderBy(t => t.TimeStamp).ToList();
                                    for (int i = 0; i < trialsArray[trialsArrayCounter].VelocityFiltered.Count; i++)
                                    {
                                        dataFileWriter.WriteLine(tempContainer.Study + ";" + tempContainer.Group + ";" + tempContainer.Szenario + ";" + tempContainer.Subject.PId + ";" + tempContainer.Turn + ";" + tempContainer.Target + ";" + trialsArray[trialsArrayCounter].TargetTrialNumberInSzenario + ";" + trialsArray[trialsArrayCounter].VelocityFiltered[i].TimeStamp.ToString("dd.MM.yyyy HH:mm:ss.fffffff") + ";" + DoubleConverter.ToExactString(Convert.ToDouble(trialsArray[trialsArrayCounter].VelocityFiltered[i].X)) + ";" + DoubleConverter.ToExactString(Convert.ToDouble(trialsArray[trialsArrayCounter].VelocityFiltered[i].Y)));
                                    }
                                }
                                else if (trajectoryVelocityForce == "Trajectory - Normalized")
                                {
                                    trialsArray[trialsArrayCounter].PositionNormalized = Gzip<List<PositionContainer>>.DeCompress(trialsArray[trialsArrayCounter].ZippedPositionNormalized).OrderBy(t => t.TimeStamp).ToList();
                                    for (int i = 0; i < trialsArray[trialsArrayCounter].PositionNormalized.Count; i++)
                                    {
                                        dataFileWriter.WriteLine(tempContainer.Study + ";" + tempContainer.Group + ";" + tempContainer.Szenario + ";" + tempContainer.Subject.PId + ";" + tempContainer.Turn + ";" + tempContainer.Target + ";" + trialsArray[trialsArrayCounter].TargetTrialNumberInSzenario + ";" + trialsArray[trialsArrayCounter].PositionNormalized[i].TimeStamp.ToString("dd.MM.yyyy HH:mm:ss.fffffff") + ";" + DoubleConverter.ToExactString(Convert.ToDouble(trialsArray[trialsArrayCounter].PositionNormalized[i].X)) + ";" + DoubleConverter.ToExactString(Convert.ToDouble(trialsArray[trialsArrayCounter].PositionNormalized[i].Y)));
                                    }
                                }
                                else if (trajectoryVelocityForce == "Trajectory - Filtered")
                                {
                                    trialsArray[trialsArrayCounter].PositionFiltered = Gzip<List<PositionContainer>>.DeCompress(trialsArray[trialsArrayCounter].ZippedPositionFiltered).OrderBy(t => t.TimeStamp).ToList();
                                    for (int i = 0; i < trialsArray[trialsArrayCounter].PositionFiltered.Count; i++)
                                    {
                                        dataFileWriter.WriteLine(tempContainer.Study + ";" + tempContainer.Group + ";" + tempContainer.Szenario + ";" + tempContainer.Subject.PId + ";" + tempContainer.Turn + ";" + tempContainer.Target + ";" + trialsArray[trialsArrayCounter].TargetTrialNumberInSzenario + ";" + trialsArray[trialsArrayCounter].PositionFiltered[i].TimeStamp.ToString("dd.MM.yyyy HH:mm:ss.fffffff") + ";" + DoubleConverter.ToExactString(Convert.ToDouble(trialsArray[trialsArrayCounter].PositionFiltered[i].X)) + ";" + DoubleConverter.ToExactString(Convert.ToDouble(trialsArray[trialsArrayCounter].PositionFiltered[i].Y)));
                                    }
                                }
                                else if (trajectoryVelocityForce == "Trajectory - Raw")
                                {
                                    trialsArray[trialsArrayCounter].PositionRaw = Gzip<List<PositionContainer>>.DeCompress(trialsArray[trialsArrayCounter].ZippedPositionRaw).OrderBy(t => t.TimeStamp).ToList();
                                    for (int i = 0; i < trialsArray[trialsArrayCounter].PositionRaw.Count; i++)
                                    {
                                        dataFileWriter.WriteLine(tempContainer.Study + ";" + tempContainer.Group + ";" + tempContainer.Szenario + ";" + tempContainer.Subject.PId + ";" + tempContainer.Turn + ";" + tempContainer.Target + ";" + trialsArray[trialsArrayCounter].TargetTrialNumberInSzenario + ";" + trialsArray[trialsArrayCounter].PositionRaw[i].TimeStamp.ToString("dd.MM.yyyy HH:mm:ss.fffffff") + ";" + DoubleConverter.ToExactString(Convert.ToDouble(trialsArray[trialsArrayCounter].PositionRaw[i].X)) + ";" + DoubleConverter.ToExactString(Convert.ToDouble(trialsArray[trialsArrayCounter].PositionRaw[i].Y)));
                                    }
                                }
                                else if (trajectoryVelocityForce == "Force - Normalized")
                                {
                                    trialsArray[trialsArrayCounter].MeasuredForcesNormalized = Gzip<List<ForceContainer>>.DeCompress(trialsArray[trialsArrayCounter].ZippedMeasuredForcesNormalized).OrderBy(t => t.TimeStamp).ToList();
                                    for (int i = 0; i < trialsArray[trialsArrayCounter].MeasuredForcesNormalized.Count; i++)
                                    {
                                        dataFileWriter.WriteLine(tempContainer.Study + ";" + tempContainer.Group + ";" + tempContainer.Szenario + ";" + tempContainer.Subject.PId + ";" + tempContainer.Turn + ";" + tempContainer.Target + ";" + trialsArray[trialsArrayCounter].TargetTrialNumberInSzenario + ";" + trialsArray[trialsArrayCounter].MeasuredForcesNormalized[i].TimeStamp.ToString("dd.MM.yyyy HH:mm:ss.fffffff") + ";" + DoubleConverter.ToExactString(Convert.ToDouble(trialsArray[trialsArrayCounter].MeasuredForcesNormalized[i].X)) + ";" + DoubleConverter.ToExactString(Convert.ToDouble(trialsArray[trialsArrayCounter].MeasuredForcesNormalized[i].Y)));
                                    }
                                }
                                else if (trajectoryVelocityForce == "Force - Filtered")
                                {
                                    trialsArray[trialsArrayCounter].MeasuredForcesFiltered = Gzip<List<ForceContainer>>.DeCompress(trialsArray[trialsArrayCounter].ZippedMeasuredForcesFiltered).OrderBy(t => t.TimeStamp).ToList();
                                    for (int i = 0; i < trialsArray[trialsArrayCounter].MeasuredForcesFiltered.Count; i++)
                                    {
                                        dataFileWriter.WriteLine(tempContainer.Study + ";" + tempContainer.Group + ";" + tempContainer.Szenario + ";" + tempContainer.Subject.PId + ";" + tempContainer.Turn + ";" + tempContainer.Target + ";" + trialsArray[trialsArrayCounter].TargetTrialNumberInSzenario + ";" + trialsArray[trialsArrayCounter].MeasuredForcesFiltered[i].TimeStamp.ToString("dd.MM.yyyy HH:mm:ss.fffffff") + ";" + DoubleConverter.ToExactString(Convert.ToDouble(trialsArray[trialsArrayCounter].MeasuredForcesFiltered[i].X)) + ";" + DoubleConverter.ToExactString(Convert.ToDouble(trialsArray[trialsArrayCounter].MeasuredForcesFiltered[i].Y)));
                                    }
                                }
                                else if (trajectoryVelocityForce == "Force - Raw")
                                {
                                    trialsArray[trialsArrayCounter].MeasuredForcesRaw = Gzip<List<ForceContainer>>.DeCompress(trialsArray[trialsArrayCounter].ZippedMeasuredForcesRaw).OrderBy(t => t.TimeStamp).ToList();
                                    for (int i = 0; i < trialsArray[trialsArrayCounter].MeasuredForcesRaw.Count; i++)
                                    {
                                        dataFileWriter.WriteLine(tempContainer.Study + ";" + tempContainer.Group + ";" + tempContainer.Szenario + ";" + tempContainer.Subject.PId + ";" + tempContainer.Turn + ";" + tempContainer.Target + ";" + trialsArray[trialsArrayCounter].TargetTrialNumberInSzenario + ";" + trialsArray[trialsArrayCounter].MeasuredForcesRaw[i].TimeStamp.ToString("dd.MM.yyyy HH:mm:ss.fffffff") + ";" + DoubleConverter.ToExactString(Convert.ToDouble(trialsArray[trialsArrayCounter].MeasuredForcesRaw[i].X)) + ";" + DoubleConverter.ToExactString(Convert.ToDouble(trialsArray[trialsArrayCounter].MeasuredForcesRaw[i].Y)));
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
                        int[] targetArray = selectedTrialsList.Select(t => t.Target).Distinct().ToArray();

                        for (int targetCounter = 0; targetCounter < targetArray.Length & !TaskManager.Cancel; targetCounter++)
                        {
                            var positionData = new List<List<PositionContainer>>();
                            var velocityData = new List<List<VelocityContainer>>();
                            var forceData = new List<List<ForceContainer>>();

                            foreach (TrajectoryVelocityPlotContainer
                                tempContainer
                                in
                                selectedTrialsList.Where(t => t.Target == targetArray[targetCounter]))
                            {
                                if (TaskManager.Cancel)
                                {
                                    break;
                                }

                                DateTime turnDateTime = _myDatabaseWrapper.GetTurns(tempContainer.Study, tempContainer.Group, tempContainer.Szenario, tempContainer.Subject).OrderBy(t => t).ElementAt(tempContainer.Turn - 1);

                                Trial[] trialsArray = _myDatabaseWrapper.GetTrials(tempContainer.Study, tempContainer.Group, tempContainer.Szenario, tempContainer.Subject, turnDateTime, tempContainer.Trials, fields).ToArray();

                                for (int trialsArrayCounter = 0; trialsArrayCounter < tempContainer.Trials.Count & !TaskManager.Cancel; trialsArrayCounter++)
                                {
                                    if (TaskManager.Cancel)
                                    {
                                        break;
                                    }

                                    _myManipAnalysisGui.SetProgressBarValue((100.0/sumOfAllTrials)*processedTrialsCount++);

                                    if (trialsArray != null)
                                    {
                                        if (trajectoryVelocityForce == "Trajectory - Normalized")
                                        {
                                            trialsArray[trialsArrayCounter].PositionNormalized = Gzip<List<PositionContainer>>.DeCompress(trialsArray[trialsArrayCounter].ZippedPositionNormalized).OrderBy(t => t.TimeStamp).ToList();
                                            positionData.Add(trialsArray[trialsArrayCounter].PositionNormalized);
                                        }
                                        else if (trajectoryVelocityForce == "Velocity - Normalized")
                                        {
                                            trialsArray[trialsArrayCounter].VelocityNormalized = Gzip<List<VelocityContainer>>.DeCompress(trialsArray[trialsArrayCounter].ZippedVelocityNormalized).OrderBy(t => t.TimeStamp).ToList();
                                            velocityData.Add(trialsArray[trialsArrayCounter].VelocityNormalized);
                                        }
                                        else if (trajectoryVelocityForce == "Force - Normalized")
                                        {
                                            trialsArray[trialsArrayCounter].MeasuredForcesNormalized = Gzip<List<ForceContainer>>.DeCompress(trialsArray[trialsArrayCounter].ZippedMeasuredForcesNormalized).OrderBy(t => t.TimeStamp).ToList();
                                            forceData.Add(trialsArray[trialsArrayCounter].MeasuredForcesNormalized);
                                        }
                                        else
                                        {
                                            _myManipAnalysisGui.WriteToLogBox("Mean can only be calculated for normalized values.");
                                        }
                                    }
                                }

                                int frameCount = 0;
                                int meanCount = 0;
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

                                    for (int meanCounter = 0; meanCounter < meanCount; meanCounter++)
                                    {
                                        for (int frameCounter = 0; frameCounter < frameCount; frameCounter++)
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
                                                xData[frameCounter] += forceData[meanCounter][frameCounter].X;
                                                yData[frameCounter] += forceData[meanCounter][frameCounter].Y;
                                            }
                                        }
                                    }

                                    for (int frameCounter = 0; frameCounter < frameCount; frameCounter++)
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
                                        }
                                    }

                                    if (trajectoryVelocityForce == "Trajectory - Normalized")
                                    {
                                        for (int i = 0; i < xData.Length; i++)
                                        {
                                            dataFileWriter.WriteLine(tempContainer.Study + ";" + tempContainer.Group + ";" + tempContainer.Szenario + ";" + tempContainer.Subject.PId + ";" + tempContainer.Turn + ";" + tempContainer.Target + ";" + tempContainer.GetTrialsString() + ";" + i + ";" + DoubleConverter.ToExactString(xData[i]) + ";" + DoubleConverter.ToExactString(yData[i]));
                                        }
                                    }
                                    else if (trajectoryVelocityForce == "Velocity - Normalized")
                                    {
                                        for (int i = 0; i < xData.Length; i++)
                                        {
                                            dataFileWriter.WriteLine(tempContainer.Study + ";" + tempContainer.Group + ";" + tempContainer.Szenario + ";" + tempContainer.Subject.PId + ";" + tempContainer.Turn + ";" + tempContainer.Target + ";" + tempContainer.GetTrialsString() + ";" + i + ";" + DoubleConverter.ToExactString(xData[i]) + ";" + DoubleConverter.ToExactString(yData[i]));
                                        }
                                    }
                                    else if (trajectoryVelocityForce == "Force - Normalized")
                                    {
                                        for (int i = 0; i < xData.Length; i++)
                                        {
                                            dataFileWriter.WriteLine(tempContainer.Study + ";" + tempContainer.Group + ";" + tempContainer.Szenario + ";" + tempContainer.Subject.PId + ";" + tempContainer.Turn + ";" + tempContainer.Target + ";" + tempContainer.GetTrialsString() + ";" + i + ";" + DoubleConverter.ToExactString(xData[i]) + ";" + DoubleConverter.ToExactString(yData[i]));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                dataFileWriter.Close();
                dataFileStream.Close();
                _myManipAnalysisGui.SetProgressBarValue(0);
                _myManipAnalysisGui.WriteProgressInfo("Ready");
                TaskManager.Remove(Task.CurrentId);
            }));
        }

        public void ExportTrajectoryData(IEnumerable<TrajectoryVelocityPlotContainer> selectedTrials, string meanIndividual, string fileName)
        {
            /*
            TaskManager.PushBack(Task.Factory.StartNew(() =>
            {
                List<TrajectoryVelocityPlotContainer> selectedTrailsList = selectedTrials.ToList();
                if (meanIndividual == "Individual")
                {
                    int counter = 0;
                    foreach (TrajectoryVelocityPlotContainer tempContainer in selectedTrailsList)
                    {
                        if (TaskManager.Cancel)
                        {
                            break;
                        }
                        _myManipAnalysisGui.SetProgressBarValue((100.0/selectedTrailsList.Count)*counter);
                        counter++;
                        DateTime turnDateTime = _myDatabaseWrapper.GetTurnDateTime(tempContainer.Study,
                            tempContainer.Group,
                            tempContainer.Szenario,
                            tempContainer.Subject,
                            tempContainer.Turn);
                        foreach (int trial in tempContainer.Trials)
                        {
                            if (TaskManager.Cancel)
                            {
                                break;
                            }
                            int trialID = _myDatabaseWrapper.GetTrailID(tempContainer.Study, tempContainer.Group,
                                tempContainer.Szenario,
                                tempContainer.Subject, turnDateTime,
                                tempContainer.Target, trial);
                            DataSet measureDataSet = _myDatabaseWrapper.GetMeasureDataNormalizedDataSet(trialID);

                            var cache = new List<string>
                            {
                                "Study;Group;Szenario;Subject;Turn;Target;Trial;TimeStamp;PositionCartesianX;PositionCartesianZ"
                            };

                            TrajectoryVelocityPlotContainer container = tempContainer;
                            int trialVar = trial;
                            cache.AddRange(from DataRow row in measureDataSet.Tables[0].Rows
                                select
                                    container.Study + ";" + container.Group + ";" +
                                    container.Szenario + ";" + container.Subject + ";" +
                                    container.Turn + ";" + container.Target + ";" + trialVar + ";" +
                                    Convert.ToDateTime(row["time_stamp"])
                                        .ToString("dd.MM.yyyy HH:mm:ss.fffffff") + ";" +
                                    DoubleConverter.ToExactString(
                                        Convert.ToDouble(row["position_cartesian_x"])) + ";" +
                                    DoubleConverter.ToExactString(
                                        Convert.ToDouble(row["position_cartesian_z"])));

                            var dataFileStream = new FileStream(fileName, FileMode.Create,
                                FileAccess.Write);
                            var dataFileWriter = new StreamWriter(dataFileStream);

                            for (int i = 0; i < cache.Count(); i++)
                            {
                                dataFileWriter.WriteLine(cache[i]);
                            }

                            dataFileWriter.Close();
                        }
                    }
                }

                else if (meanIndividual == "Mean")
                {
                    if (
                        selectedTrailsList.Select(t => t.Trials.ToArray())
                            .Distinct(new ArrayComparer())
                            .Count() > 1)
                    {
                        _myManipAnalysisGui.WriteToLogBox("Trial selections are not equal!");
                    }
                    else
                    {
                        string[] studyArray =
                            selectedTrailsList.Select(t => t.Study).Distinct().ToArray();
                        string[] groupArray =
                            selectedTrailsList.Select(t => t.Group).Distinct().ToArray();
                        string[] szenarioArray =
                            selectedTrailsList.Select(t => t.Szenario).Distinct().ToArray();
                        SubjectContainer[] subjectArray =
                            selectedTrailsList.Select(t => t.Subject).Distinct().ToArray();
                        int[] turnArray =
                            selectedTrailsList.Select(t => t.Turn)
                                .Distinct()
                                .ToArray();
                        int[] targetArray =
                            selectedTrailsList.Select(t => t.Target).Distinct().ToArray();
                        string trials = selectedTrailsList.ElementAt(0).GetTrialsString();

                        var cache = new List<string>
                        {
                            "Study;Group;Szenario;Subject;Turn;Target;Trial;DataPoint;PositionCartesianX;PositionCartesianZ"
                        };


                        int counter = 0;
                        for (int targetCounter = 0;
                            targetCounter < targetArray.Length & !TaskManager.Cancel;
                            targetCounter++)
                        {
                            int meanCounter = 0;
                            var dataX = new List<double>();
                            var dataZ = new List<double>();

                            int counterVar = targetCounter;
                            foreach (TrajectoryVelocityPlotContainer tempContainer in selectedTrailsList)
                            {
                                if (TaskManager.Cancel)
                                {
                                    break;
                                }
                                if (tempContainer.Target == targetArray[counterVar])
                                {
                                    _myManipAnalysisGui.SetProgressBarValue((100.0/selectedTrailsList.Count)*counter);
                                    counter++;
                                    DateTime turnDateTime = _myDatabaseWrapper.GetTurnDateTime(tempContainer.Study,
                                        tempContainer.Group,
                                        tempContainer.Szenario,
                                        tempContainer.Subject,
                                        tempContainer.Turn);


                                    foreach (int trial in tempContainer.Trials)
                                    {
                                        if (TaskManager.Cancel)
                                        {
                                            break;
                                        }
                                        int trialID = _myDatabaseWrapper.GetTrailID(tempContainer.Study,
                                            tempContainer.Group,
                                            tempContainer.Szenario,
                                            tempContainer.Subject,
                                            turnDateTime,
                                            tempContainer.Target, trial);
                                        DataSet dataSet = _myDatabaseWrapper.GetMeasureDataNormalizedDataSet(trialID);
                                        for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                                        {
                                            DataRow row = dataSet.Tables[0].Rows[i];

                                            if (dataX.Count <= i)
                                            {
                                                dataX.Add(Convert.ToDouble(row["position_cartesian_x"]));
                                            }
                                            else
                                            {
                                                dataX[i] += Convert.ToDouble(row["position_cartesian_x"]);
                                            }

                                            if (dataZ.Count <= i)
                                            {
                                                dataZ.Add(Convert.ToDouble(row["position_cartesian_z"]));
                                            }
                                            else
                                            {
                                                dataZ[i] += Convert.ToDouble(row["position_cartesian_z"]);
                                            }
                                        }
                                        meanCounter++;
                                    }
                                }
                            }


                            for (int i = 0; i < dataX.Count; i++)
                            {
                                dataX[i] /= meanCounter;
                                dataZ[i] /= meanCounter;
                            }

                            counterVar = targetCounter;
                            cache.AddRange(
                                dataX.Select(
                                    (t, i) =>
                                        String.Join(",", studyArray) + ";" + String.Join(",", groupArray) + ";" +
                                        String.Join(",", szenarioArray) + ";" +
                                        String.Join<SubjectContainer>(",", subjectArray) + ";" +
                                        String.Join(",", turnArray) + ";" + targetArray[counterVar] + ";" + trials +
                                        ";" + i + ";" + DoubleConverter.ToExactString(t) + ";" +
                                        DoubleConverter.ToExactString(dataZ[i])));

                            var dataFileStream = new FileStream(fileName, FileMode.Create,
                                FileAccess.Write);
                            var dataFileWriter = new StreamWriter(dataFileStream);

                            for (int i = 0; i < cache.Count(); i++)
                            {
                                dataFileWriter.WriteLine(cache[i]);
                            }

                            dataFileWriter.Close();
                        }
                    }
                }

                _myMatlabWrapper.ClearWorkspace();
                TaskManager.Remove(Task.CurrentId);
            }));
            */
        }

        public void ExportVelocityData(IEnumerable<TrajectoryVelocityPlotContainer> selectedTrials, string meanIndividual, string fileName)
        {
            /*
            TaskManager.PushBack(Task.Factory.StartNew(() =>
            {
                List<TrajectoryVelocityPlotContainer> selectedTrailsList = selectedTrials.ToList();
                if (meanIndividual == "Individual")
                {
                    int counter = 0;
                    foreach (TrajectoryVelocityPlotContainer tempContainer in selectedTrailsList)
                    {
                        if (TaskManager.Cancel)
                        {
                            break;
                        }
                        _myManipAnalysisGui.SetProgressBarValue((100.0/selectedTrailsList.Count)*counter);
                        counter++;
                        DateTime turnDateTime = _myDatabaseWrapper.GetTurnDateTime(tempContainer.Study,
                            tempContainer.Group,
                            tempContainer.Szenario,
                            tempContainer.Subject,
                            tempContainer.Turn);
                        foreach (int trial in tempContainer.Trials)
                        {
                            if (TaskManager.Cancel)
                            {
                                break;
                            }
                            int trialID = _myDatabaseWrapper.GetTrailID(tempContainer.Study, tempContainer.Group,
                                tempContainer.Szenario,
                                tempContainer.Subject, turnDateTime,
                                tempContainer.Target, trial);
                            DataSet velocityDataSet = _myDatabaseWrapper.GetVelocityDataNormalizedDataSet(trialID);

                            var cache = new List<string>
                            {
                                "Study;Group;Szenario;Subject;Turn;Target;Trial;TimeStamp;VelocityXZ"
                            };

                            TrajectoryVelocityPlotContainer container = tempContainer;
                            int trialVar = trial;
                            cache.AddRange(from DataRow row in velocityDataSet.Tables[0].Rows
                                select
                                    container.Study + ";" + container.Group + ";" +
                                    container.Szenario + ";" + container.Subject + ";" +
                                    container.Turn + ";" + container.Target + ";" + trialVar + ";" +
                                    Convert.ToDateTime(row["time_stamp"])
                                        .ToString("dd.MM.yyyy HH:mm:ss.fffffff") + ";" +
                                    DoubleConverter.ToExactString(
                                        Convert.ToDouble(
                                            Math.Sqrt(
                                                Math.Pow(Convert.ToDouble(row["velocity_x"]), 2) +
                                                Math.Pow(Convert.ToDouble(row["velocity_z"]), 2)))));

                            var dataFileStream = new FileStream(fileName, FileMode.Create,
                                FileAccess.Write);
                            var dataFileWriter = new StreamWriter(dataFileStream);

                            for (int i = 0; i < cache.Count(); i++)
                            {
                                dataFileWriter.WriteLine(cache[i]);
                            }

                            dataFileWriter.Close();
                        }
                    }
                }

                else if (meanIndividual == "Mean")
                {
                    if (
                        selectedTrailsList.Select(t => t.Trials.ToArray())
                            .Distinct(new ArrayComparer())
                            .Count() > 1)
                    {
                        _myManipAnalysisGui.WriteToLogBox("Trial selections are not equal!");
                    }
                    else
                    {
                        string[] studyArray =
                            selectedTrailsList.Select(t => t.Study).Distinct().ToArray();
                        string[] groupArray =
                            selectedTrailsList.Select(t => t.Group).Distinct().ToArray();
                        string[] szenarioArray =
                            selectedTrailsList.Select(t => t.Szenario).Distinct().ToArray();
                        SubjectContainer[] subjectArray =
                            selectedTrailsList.Select(t => t.Subject).Distinct().ToArray();
                        int[] turnArray =
                            selectedTrailsList.Select(t => t.Turn)
                                .Distinct()
                                .ToArray();
                        int[] targetArray =
                            selectedTrailsList.Select(t => t.Target).Distinct().ToArray();
                        string trials = selectedTrailsList.ElementAt(0).GetTrialsString();

                        var cache = new List<string>
                        {
                            "Study;Group;Szenario;Subject;Turn;Target;Trial;DataPoint;VelocityXZ"
                        };


                        int counter = 0;
                        for (int targetCounter = 0;
                            targetCounter < targetArray.Length & !TaskManager.Cancel;
                            targetCounter++)
                        {
                            int meanCounter = 0;
                            var dataXZ = new List<double>();

                            int counterVar = targetCounter;
                            foreach (TrajectoryVelocityPlotContainer tempContainer in selectedTrailsList)
                            {
                                if (TaskManager.Cancel)
                                {
                                    break;
                                }
                                if (tempContainer.Target == targetArray[counterVar])
                                {
                                    _myManipAnalysisGui.SetProgressBarValue((100.0/selectedTrailsList.Count)*counter);
                                    counter++;
                                    DateTime turnDateTime = _myDatabaseWrapper.GetTurnDateTime(tempContainer.Study,
                                        tempContainer.Group,
                                        tempContainer.Szenario,
                                        tempContainer.Subject,
                                        tempContainer.Turn);


                                    foreach (int trial in tempContainer.Trials)
                                    {
                                        if (TaskManager.Cancel)
                                        {
                                            break;
                                        }
                                        int trialID = _myDatabaseWrapper.GetTrailID(tempContainer.Study,
                                            tempContainer.Group,
                                            tempContainer.Szenario,
                                            tempContainer.Subject,
                                            turnDateTime,
                                            tempContainer.Target, trial);

                                        DataSet dataSet =
                                            _myDatabaseWrapper.GetVelocityDataNormalizedDataSet(trialID);
                                        for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                                        {
                                            DataRow row = dataSet.Tables[0].Rows[i];

                                            if (dataXZ.Count <= i)
                                            {
                                                dataXZ.Add(
                                                    Math.Sqrt(
                                                        Math.Pow(Convert.ToDouble(row["velocity_x"]), 2) +
                                                        Math.Pow(Convert.ToDouble(row["velocity_z"]), 2)));
                                            }
                                            else
                                            {
                                                dataXZ[i] +=
                                                    Math.Sqrt(
                                                        Math.Pow(Convert.ToDouble(row["velocity_x"]), 2) +
                                                        Math.Pow(Convert.ToDouble(row["velocity_z"]), 2));
                                            }
                                        }
                                        meanCounter++;
                                    }
                                }
                            }

                            for (int i = 0; i < dataXZ.Count; i++)
                            {
                                dataXZ[i] /= meanCounter;
                            }

                            counterVar = targetCounter;
                            cache.AddRange(
                                dataXZ.Select(
                                    (t, i) => String.Join(",", studyArray) + ";" +
                                              String.Join(",", groupArray) + ";" +
                                              String.Join(",", szenarioArray) + ";" +
                                              String.Join<SubjectContainer>(",", subjectArray) +
                                              ";" +
                                              String.Join(",", turnArray) + ";" + targetArray[counterVar] +
                                              ";" + trials +
                                              ";" + i + ";" + DoubleConverter.ToExactString(t)));

                            var dataFileStream = new FileStream(fileName, FileMode.Create,
                                FileAccess.Write);
                            var dataFileWriter = new StreamWriter(dataFileStream);

                            for (int i = 0; i < cache.Count(); i++)
                            {
                                dataFileWriter.WriteLine(cache[i]);
                            }

                            dataFileWriter.Close();
                        }
                    }
                }

                _myMatlabWrapper.ClearWorkspace();
                TaskManager.Remove(Task.CurrentId);
            }));
            */
        }

        public void ExportTrajectoryBaseline(string study, string group, string szenario, SubjectContainer subject, string fileName)
        {
            /*
            TaskManager.PushBack(Task.Factory.StartNew(() =>
            {
                DataSet baseline = _myDatabaseWrapper.GetBaselineDataSet(study, group, szenario, subject);

                List<object[]> baselineData = (from DataRow row in baseline.Tables[0].Rows
                    select new object[]
                    {
                        Convert.ToDouble(row["baseline_position_cartesian_x"]),
                        Convert.ToDouble(row["baseline_position_cartesian_z"]),
                        Convert.ToInt32(row["target_number"])
                    }).ToList();

                int[] targetNumberArray = baselineData.Select(t => Convert.ToInt32(t[2])).Distinct().ToArray();

                var cache = new List<string>
                {
                    "Study;Group;Szenario;Subject;Target;DataPoint;PositionCartesianX;PositionCartesianZ"
                };

                for (int i = 0; i < targetNumberArray.Length & !TaskManager.Cancel; i++)
                {
                    double[] tempX =
                        baselineData.Where(t => Convert.ToInt32(t[2]) == targetNumberArray[i])
                            .Select(t => Convert.ToDouble(t[0]))
                            .ToArray();
                    double[] tempZ =
                        baselineData.Where(t => Convert.ToInt32(t[2]) == targetNumberArray[i])
                            .Select(t => Convert.ToDouble(t[1]))
                            .ToArray();

                    int iVar = i;
                    cache.AddRange(
                        tempX.Select(
                            (t, j) =>
                                study + ";" + @group + ";" + szenario + ";" + subject + ";" + targetNumberArray[iVar] +
                                ";" +
                                j + ";" + DoubleConverter.ToExactString(t) + ";" +
                                DoubleConverter.ToExactString(tempZ[j])));
                }

                var dataFileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                var dataFileWriter = new StreamWriter(dataFileStream);

                for (int i = 0; i < cache.Count(); i++)
                {
                    dataFileWriter.WriteLine(cache[i]);
                }

                dataFileWriter.Close();
                TaskManager.Remove(Task.CurrentId);
            }));
            */
        }

        public void ExportVelocityBaseline(string study, string group, string szenario, SubjectContainer subject, string fileName)
        {
            /*
            TaskManager.PushBack(Task.Factory.StartNew(() =>
            {
                DataSet baseline = _myDatabaseWrapper.GetBaselineDataSet(study, group, szenario, subject);

                List<object[]> baselineData = (from DataRow row in baseline.Tables[0].Rows
                    select new object[]
                    {
                        Convert.ToDouble(row["baseline_velocity_x"]),
                        Convert.ToDouble(row["baseline_velocity_z"]),
                        Convert.ToInt32(row["target_number"])
                    }).ToList();

                int[] targetNumberArray = baselineData.Select(t => Convert.ToInt32(t[2])).Distinct().ToArray();

                var cache = new List<string>
                {
                    "Study;Group;Szenario;Subject;Target;DataPoint;VelocityX;VelocityZ;VelocityXZ"
                };

                for (int i = 0; i < targetNumberArray.Length & !TaskManager.Cancel; i++)
                {
                    double[] tempX =
                        baselineData.Where(t => Convert.ToInt32(t[2]) == targetNumberArray[i])
                            .Select(t => Convert.ToDouble(t[0]))
                            .ToArray();
                    double[] tempZ =
                        baselineData.Where(t => Convert.ToInt32(t[2]) == targetNumberArray[i])
                            .Select(t => Convert.ToDouble(t[1]))
                            .ToArray();

                    int iVar = i;
                    cache.AddRange(
                        tempX.Select(
                            (t, j) =>
                                study + ";" + @group + ";" + szenario + ";" + subject + ";" + targetNumberArray[iVar] +
                                ";" +
                                j + ";" + DoubleConverter.ToExactString(t) + ";" +
                                DoubleConverter.ToExactString(tempZ[j])));
                }

                var dataFileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                var dataFileWriter = new StreamWriter(dataFileStream);

                for (int i = 0; i < cache.Count(); i++)
                {
                    dataFileWriter.WriteLine(cache[i]);
                }

                dataFileWriter.Close();
                TaskManager.Remove(Task.CurrentId);
            }));
            */
        }

        public void PlotVelocityBaselines(string study, string group, SubjectContainer subject, int[] targets, IEnumerable<Trial.TrialTypeEnum> trialTypes, IEnumerable<Trial.ForceFieldTypeEnum> forceFields, IEnumerable<Trial.HandednessEnum> handedness)
        {
            TaskManager.PushBack(Task.Factory.StartNew(() =>
            {
                _myMatlabWrapper.CreateVelocityFigure("Velocity baseline plot", 101);

                var baselineFields = new FieldsBuilder<Baseline>();
                baselineFields.Include(t => t.ZippedVelocity);
                Baseline[] baselines = _myDatabaseWrapper.GetBaseline(study, group, subject, targets, trialTypes, forceFields, handedness, baselineFields);

                for (int baselineCounter = 0; baselineCounter < baselines.Length & !TaskManager.Cancel; baselineCounter++)
                {
                    baselines[baselineCounter].Velocity = Gzip<List<VelocityContainer>>.DeCompress(baselines[baselineCounter].ZippedVelocity).OrderBy(t => t.TimeStamp).ToList();
                    _myMatlabWrapper.SetWorkspaceData("XY", baselines[baselineCounter].Velocity.Select(t => Math.Sqrt(Math.Pow(t.X, 2) + Math.Pow(t.Y, 2))).ToArray());
                    _myMatlabWrapper.Plot("XY", "black", 2);
                }

                _myMatlabWrapper.ClearWorkspace();
                TaskManager.Remove(Task.CurrentId);
            }));
        }

        public void PlotForceBaselines(string study, string group, SubjectContainer subject, int[] targets, IEnumerable<Trial.TrialTypeEnum> trialTypes, IEnumerable<Trial.ForceFieldTypeEnum> forceFields, IEnumerable<Trial.HandednessEnum> handedness)
        {
            TaskManager.PushBack(Task.Factory.StartNew(() =>
            {
                _myMatlabWrapper.CreateForceFigure("Force baseline plot", "[Samples]", "Force [N]");

                var baselineFields = new FieldsBuilder<Baseline>();
                baselineFields.Include(t => t.ZippedMeasuredForces, t2 => t2.ZippedPosition);
                Baseline[] baselines = _myDatabaseWrapper.GetBaseline(study, group, subject, targets, trialTypes, forceFields, handedness, baselineFields);

                for (int baselineCounter = 0; baselineCounter < baselines.Length & !TaskManager.Cancel; baselineCounter++)
                {
                    baselines[baselineCounter].Position = Gzip<List<PositionContainer>>.DeCompress(baselines[baselineCounter].ZippedPosition).OrderBy(t => t.TimeStamp).ToList();
                    baselines[baselineCounter].MeasuredForces = Gzip<List<ForceContainer>>.DeCompress(baselines[baselineCounter].ZippedMeasuredForces).OrderBy(t => t.TimeStamp).ToList();

                    _myMatlabWrapper.Execute("forcePDVector = zeros(1, " + (baselines[baselineCounter].Position.Count - 1) + ");");
                    _myMatlabWrapper.Execute("forceParaVector = zeros(1, " + (baselines[baselineCounter].Position.Count - 1) + ");");
                    _myMatlabWrapper.Execute("forceAbsVector = zeros(1, " + (baselines[baselineCounter].Position.Count - 1) + ");");

                    for (int i = 2; i <= baselines[baselineCounter].Position.Count & !TaskManager.Pause; i++)
                    {
                        _myMatlabWrapper.SetWorkspaceData("vpos1", new[] { baselines[baselineCounter].Position.Select(t => t.X).ElementAt(i - 2), baselines[baselineCounter].Position.Select(t => t.Y).ElementAt(i - 2) });
                        _myMatlabWrapper.SetWorkspaceData("vpos2", new[] { baselines[baselineCounter].Position.Select(t => t.X).ElementAt(i - 1), baselines[baselineCounter].Position.Select(t => t.Y).ElementAt(i - 1) });
                        _myMatlabWrapper.SetWorkspaceData("vforce", new[] { baselines[baselineCounter].MeasuredForces.Select(t => t.X).ElementAt(i - 2), baselines[baselineCounter].MeasuredForces.Select(t => t.Y).ElementAt(i - 2) });

                        _myMatlabWrapper.Execute("[fPD, fPDsign] = pdForceLineSegment([vforce(1,1) vforce(1,2)], [vpos1(1,1) vpos1(1,2)], [vpos2(1,1) vpos2(1,2)]);");
                        _myMatlabWrapper.Execute("[fPara, fParasign] = paraForceLineSegment([vforce(1,1) vforce(1,2)], [vpos1(1,1) vpos1(1,2)], [vpos2(1,1) vpos2(1,2)]);");

                        _myMatlabWrapper.Execute("forcePDVector(" + (i - 1) + ") = sqrt(fPD(1)^2 + fPD(2)^2) * fPDsign;");
                        _myMatlabWrapper.Execute("forceParaVector(" + (i - 1) + ") = sqrt(fPara(1)^2 + fPara(2)^2) * fParasign;");
                        _myMatlabWrapper.Execute("forceAbsVector(" + (i - 1) + ") = sqrt(vforce(1,1)^2 + vforce(1,2)^2);");
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

        public void RecalculateBaselines(IEnumerable<TrajectoryVelocityPlotContainer> selectedTrials, Trial.TrialTypeEnum trialType, Trial.ForceFieldTypeEnum forceFieldType, Trial.HandednessEnum handedness)
        {
            foreach (TrajectoryVelocityPlotContainer targetData in selectedTrials)
            {
                var baselineFields = new FieldsBuilder<Baseline>();
                baselineFields.Include(t => t.Id);
                Baseline baseline = _myDatabaseWrapper.GetBaseline(targetData.Study, targetData.Group, targetData.Subject, targetData.Target, trialType, forceFieldType, handedness, baselineFields);


            }
            
            /*
            List<TrajectoryVelocityPlotContainer> selectedTrialsList = selectedTrials.ToList();
            int[] targetArray = selectedTrialsList.Select(t => t.Target).Distinct().ToArray();
            SubjectContainer subject = selectedTrialsList.Select(t => t.Subject).ElementAt(0);

            int counter = 0;

            _myManipAnalysisGui.WriteProgressInfo("Recalculating baselines...");

            for (int targetCounter = 0; targetCounter < targetArray.Length; targetCounter++)
            {
                int targetCounterVar = targetCounter;
                int meanCounter = 0;
                var measureDataNormalizedDataX = new List<double>();
                var measureDataNormalizedDataY = new List<double>();
                var measureDataNormalizedDataZ = new List<double>();
                var velocityDataNormalizedDataX = new List<double>();
                var velocityDataNormalizedDataY = new List<double>();
                var velocityDataNormalizedDataZ = new List<double>();

                TrajectoryVelocityPlotContainer tempContainer =
                    selectedTrialsList.Where(t => t.Target == targetArray[targetCounterVar]).ElementAt(0);

                int baselineID = _myDatabaseWrapper.GetBaselineID(tempContainer.Study,
                    tempContainer.Group,
                    tempContainer.Szenario,
                    tempContainer.Subject,
                    tempContainer.Target);


                _myManipAnalysisGui.SetProgressBarValue((100.0/selectedTrialsList.Count())*
                                                        counter);
                counter++;
                DateTime turnDateTime = _myDatabaseWrapper.GetTurnDateTime(tempContainer.Study,
                    tempContainer.Group,
                    tempContainer.Szenario,
                    tempContainer.Subject,
                    tempContainer.Turn);


                foreach (int trial in tempContainer.Trials)
                {
                    int trialID = _myDatabaseWrapper.GetTrailID(tempContainer.Study,
                        tempContainer.Group,
                        tempContainer.Szenario,
                        tempContainer.Subject,
                        turnDateTime,
                        tempContainer.Target,
                        trial);

                    DataSet measureDataNormalizedDataSet = _myDatabaseWrapper.GetMeasureDataNormalizedDataSet(trialID);
                    DataSet velocityDataNormalizedDataSet = _myDatabaseWrapper.GetVelocityDataNormalizedDataSet(trialID);

                    for (int i = 0; i < measureDataNormalizedDataSet.Tables[0].Rows.Count; i++)
                    {
                        DataRow row = measureDataNormalizedDataSet.Tables[0].Rows[i];

                        if (measureDataNormalizedDataX.Count <= i)
                        {
                            measureDataNormalizedDataX.Add(Convert.ToDouble(row["position_cartesian_x"]));
                        }
                        else
                        {
                            measureDataNormalizedDataX[i] += Convert.ToDouble(row["position_cartesian_x"]);
                        }

                        if (measureDataNormalizedDataY.Count <= i)
                        {
                            measureDataNormalizedDataY.Add(Convert.ToDouble(row["position_cartesian_y"]));
                        }
                        else
                        {
                            measureDataNormalizedDataY[i] += Convert.ToDouble(row["position_cartesian_y"]);
                        }

                        if (measureDataNormalizedDataZ.Count <= i)
                        {
                            measureDataNormalizedDataZ.Add(Convert.ToDouble(row["position_cartesian_z"]));
                        }
                        else
                        {
                            measureDataNormalizedDataZ[i] += Convert.ToDouble(row["position_cartesian_z"]);
                        }
                    }

                    for (int i = 0; i < velocityDataNormalizedDataSet.Tables[0].Rows.Count; i++)
                    {
                        DataRow row = velocityDataNormalizedDataSet.Tables[0].Rows[i];

                        if (velocityDataNormalizedDataX.Count <= i)
                        {
                            velocityDataNormalizedDataX.Add(Convert.ToDouble(row["velocity_x"]));
                        }
                        else
                        {
                            velocityDataNormalizedDataX[i] += Convert.ToDouble(row["velocity_x"]);
                        }

                        if (velocityDataNormalizedDataY.Count <= i)
                        {
                            velocityDataNormalizedDataY.Add(Convert.ToDouble(row["velocity_y"]));
                        }
                        else
                        {
                            velocityDataNormalizedDataY[i] += Convert.ToDouble(row["velocity_y"]);
                        }

                        if (velocityDataNormalizedDataZ.Count <= i)
                        {
                            velocityDataNormalizedDataZ.Add(Convert.ToDouble(row["velocity_z"]));
                        }
                        else
                        {
                            velocityDataNormalizedDataZ[i] += Convert.ToDouble(row["velocity_z"]);
                        }
                    }

                    meanCounter++;
                }

                for (int i = 0; i < measureDataNormalizedDataX.Count; i++)
                {
                    measureDataNormalizedDataX[i] /= meanCounter;
                    measureDataNormalizedDataY[i] /= meanCounter;
                    measureDataNormalizedDataZ[i] /= meanCounter;
                }

                for (int i = 0; i < velocityDataNormalizedDataX.Count; i++)
                {
                    velocityDataNormalizedDataX[i] /= meanCounter;
                    velocityDataNormalizedDataY[i] /= meanCounter;
                    velocityDataNormalizedDataZ[i] /= meanCounter;
                }

                if (measureDataNormalizedDataX.Count() == velocityDataNormalizedDataX.Count())
                {
                    _myDatabaseWrapper.DeleteBaselineData(baselineID);
                    _myManipAnalysisGui.WriteToLogBox("Deleted old baseline of Target " + tempContainer.Target +
                                                      ", uploading new one...");

                    for (int i = 0; i < measureDataNormalizedDataX.Count(); i++)
                    {
                        _myDatabaseWrapper.InsertBaselineData(baselineID,
                            turnDateTime.AddMilliseconds(i*5),
                            measureDataNormalizedDataX[i],
                            measureDataNormalizedDataY[i],
                            measureDataNormalizedDataZ[i],
                            velocityDataNormalizedDataX[i],
                            velocityDataNormalizedDataY[i],
                            velocityDataNormalizedDataZ[i]);
                    }
                }
                else
                {
                    _myManipAnalysisGui.WriteToLogBox("Trajectory- and Velocity-Baseline are of unequal length!");
                }
            }
            _myDatabaseWrapper.DeleteSubjectStatistics(subject);
            _myManipAnalysisGui.WriteToLogBox("Deleted subjects (" + subject + ") statistics...");


            _myManipAnalysisGui.WriteProgressInfo("Ready.");
            _myManipAnalysisGui.SetProgressBarValue(0);
            */
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