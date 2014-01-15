using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ManipAnalysis.Container;
using ManipAnalysis.MongoDb;


namespace ManipAnalysis
{
    /// <summary>
    ///     This class provides all interactive functionality between the Gui and the programm.
    /// </summary>
    public class ManipAnalysisFunctions
    {
        private readonly ManipAnalysisGui _myManipAnalysisGui;
        private readonly MatlabWrapper _myMatlabWrapper;
        private readonly SqlWrapper _myDatabaseWrapper;
        private readonly MongoDbWrapper _myMongoDbWrapperWrapper;

        public ManipAnalysisFunctions(ManipAnalysisGui myManipAnalysisGui, MatlabWrapper myMatlabWrapper,
            SqlWrapper mySqlWrapper)
        {
            _myMatlabWrapper = myMatlabWrapper;
            _myDatabaseWrapper = mySqlWrapper;
            _myManipAnalysisGui = myManipAnalysisGui;
            _myMongoDbWrapperWrapper = new MongoDbWrapper();
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
                IAsyncResult ar = tcp.BeginConnect(server, 27017, null, null); // For MongoDB
                WaitHandle wh = ar.AsyncWaitHandle;
                try
                {
                    if (!ar.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(timeOutMilliSec), false))
                    {
                        throw new Exception("Server didn't respond within 2 seconds.");
                    }

                    tcp.EndConnect(ar);
                    serverAvailable = true;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
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

            if (CheckDatabaseServerAvailability(server, 6000))
            {
                _myManipAnalysisGui.WriteToLogBox("Connected to Database-Server.");
                _myMongoDbWrapperWrapper.SetDatabaseServer(server);
                _myManipAnalysisGui.SetSqlDatabases(_myMongoDbWrapperWrapper.GetDatabases());
                retVal = true;
            }
            else
            {
                _myManipAnalysisGui.WriteToLogBox("Database-Server not reachable!");
            }

            return retVal;
        }

        /// <summary>
        ///     Sets a Database in the SQL-Wrapper
        /// </summary>
        /// <param name="database">The new Database</param>
        public void SetDatabase(string database)
        {
            _myMongoDbWrapperWrapper.SetDatabase(database);
        }

        /// <summary>
        ///     Gets all studys from database
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetStudys()
        {
            return _myMongoDbWrapperWrapper.GetStudyNames();
        }

        /// <summary>
        ///     Gets all groups from database of a given study
        /// </summary>
        /// <param name="study"></param>
        /// <returns></returns>
        public IEnumerable<string> GetGroups(string study)
        {
            return _myMongoDbWrapperWrapper.GetGroupNames(study);
        }

        /// <summary>
        ///     Gets all szenarios from database of a given study and group
        /// </summary>
        /// <param name="study"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public IEnumerable<string> GetSzenarios(string study, string group)
        {
            return _myDatabaseWrapper.GetSzenarioNames(study, group);
        }

        /// <summary>
        ///     Gets all subjects from database of a given study, group and szenario
        /// </summary>
        /// <param name="study"></param>
        /// <param name="group"></param>
        /// <param name="szenario"></param>
        /// <returns></returns>
        public IEnumerable<SubjectInformationContainer> GetSubjects(string study, string group, string szenario)
        {
            return _myDatabaseWrapper.GetSubjectInformations(study, group, szenario);
        }

        /// <summary>
        ///     Gets all turns from database of a given study, group, szenario and subject
        /// </summary>
        /// <param name="study"></param>
        /// <param name="group"></param>
        /// <param name="szenario"></param>
        /// <param name="subject"></param>
        /// <returns></returns>
        public IEnumerable<string> GetTurns(string study, string group, string szenario,
            SubjectInformationContainer subject)
        {
            return _myDatabaseWrapper.GetTurns(study, group, szenario, subject);
        }

        /// <summary>
        ///     Gets all targets from database of a given study and szenario
        /// </summary>
        /// <param name="study"></param>
        /// <param name="szenario"></param>
        /// <returns></returns>
        public IEnumerable<string> GetTargets(string study, string szenario)
        {
            return _myDatabaseWrapper.GetTargets(study, szenario);
        }

        /// <summary>
        ///     Gets all trials from database of a given study and szenario
        /// </summary>
        /// <param name="study"></param>
        /// <param name="szenario"></param>
        /// <returns></returns>
        public IEnumerable<string> GetTrials(string study, string szenario)
        {
            return _myDatabaseWrapper.GetTrials(study, szenario);
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
        ///     Gets the DateTime of a turn from database of a given study, group, szenario, subject and turn
        /// </summary>
        /// <param name="study"></param>
        /// <param name="group"></param>
        /// <param name="szenario"></param>
        /// <param name="subject"></param>
        /// <param name="turn"></param>
        /// <returns></returns>
        private DateTime GetTurnDateTime(string study, string group, string szenario,
            SubjectInformationContainer subject,
            int turn)
        {
            return _myDatabaseWrapper.GetTurnDateTime(study, group, szenario, subject, turn);
        }

        /// <summary>
        ///     Deletes all data from a given measure-file-id from the database
        /// </summary>
        /// <param name="measureFileId"></param>
        public void DeleteMeasureFile(int measureFileId)
        {
            _myDatabaseWrapper.DeleteMeasureFile(measureFileId);
        }

        public void PlotSzenarioMeanTimes(string study, string group, string szenario,
            SubjectInformationContainer subject, int turn)
        {
            TaskManager.PushBack(Task.Factory.StartNew(() =>
            {
                DateTime turnDateTime = GetTurnDateTime(study, group, szenario, subject, turn);
                DataSet meanTimeDataSet = _myDatabaseWrapper.GetMeanTimeDataSet(study, group, szenario, subject, turnDateTime);

                _myMatlabWrapper.CreateMeanTimeFigure();

                var meanTimeList = new List<double>();
                var meanTimeStdList = new List<double>();
                var targetList = new List<int>();

                foreach (DataRow row in meanTimeDataSet.Tables[0].Rows)
                {
                    if (TaskManager.Cancel)
                    {
                        break;
                    }
                    meanTimeList.Add(TimeSpan.Parse(Convert.ToString(row["szenario_mean_time"])).TotalSeconds);
                    meanTimeStdList.Add(TimeSpan.Parse(Convert.ToString(row["szenario_mean_time_std"])).TotalSeconds);
                    targetList.Add(Convert.ToInt32(row["target_number"]));
                }

                meanTimeList.Add(meanTimeList.Average());
                meanTimeStdList.Add(meanTimeStdList.Average());
                targetList.Add(17); // Add a 17th value for overall mean value

                _myMatlabWrapper.SetWorkspaceData("target", targetList.ToArray());
                _myMatlabWrapper.SetWorkspaceData("meanTime", meanTimeList.ToArray());
                _myMatlabWrapper.SetWorkspaceData("meanTimeStd", meanTimeStdList.ToArray());
                _myMatlabWrapper.PlotMeanTimeErrorBar("target", "meanTime", "meanTimeStd");

                _myMatlabWrapper.ClearWorkspace();
                TaskManager.Remove(Task.CurrentId);
            }));
        }

        public void ExportSzenarioMeanTimes(string study, string group, string szenario,
            SubjectInformationContainer subject, int turn, string fileName)
        {
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
        }

        public void ToggleMatlabCommandWindow()
        {
            _myMatlabWrapper.ToggleCommandWindow();
        }

        public void ShowMatlabWorkspace()
        {
            _myMatlabWrapper.ShowWorkspaceWindow();
        }

        public void InitializeSqlDatabase()
        {
            _myDatabaseWrapper.InitializeDatabase();
        }

        public void PlotBaseline(string study, string group, string szenario, SubjectInformationContainer subject)
        {
            TaskManager.PushBack(Task.Factory.StartNew(() =>
            {
                _myMatlabWrapper.CreateTrajectoryFigure("Trajectory baseline plot");
                _myMatlabWrapper.DrawTargets(0.005, 0.1, 0, 0);

                DataSet baseline = _myDatabaseWrapper.GetBaselineDataSet(study, group, szenario, subject);

                List<object[]> baselineData = (from DataRow row in baseline.Tables[0].Rows
                    select new object[]
                    {
                        Convert.ToDouble(row["baseline_position_cartesian_x"]),
                        Convert.ToDouble(row["baseline_position_cartesian_z"]),
                        Convert.ToInt32(row["target_number"])
                    }).ToList();

                int[] targetNumberArray = baselineData.Select(t => Convert.ToInt32(t[2])).Distinct().ToArray();

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

                    _myMatlabWrapper.SetWorkspaceData("X", tempX);
                    _myMatlabWrapper.SetWorkspaceData("Z", tempZ);
                    _myMatlabWrapper.Plot("X", "Z", "black", 2);
                }

                _myMatlabWrapper.ClearWorkspace();
                TaskManager.Remove(Task.CurrentId);
            }));
        }

        public IEnumerable<string> GetTrialsOfSzenario(string study, string szenario, bool showCatchTrials,
            bool showCatchTrialsExclusivly, bool showErrorclampTrials,
            bool showErrorclampTrialsExclusivly)
        {
            return _myDatabaseWrapper.GetSzenarioTrials(study, szenario, showCatchTrials, showCatchTrialsExclusivly,
                showErrorclampTrials, showErrorclampTrialsExclusivly);
        }

        public void PlotDescriptiveStatistic1(IEnumerable<StatisticPlotContainer> selectedTrials, string statisticType,
            string fitEquation, int pdTime, bool plotFit, bool plotErrorbars)
        {
            TaskManager.PushBack(Task.Factory.StartNew(() =>
            {
                _myManipAnalysisGui.WriteProgressInfo("Getting data...");
                List<StatisticPlotContainer> selectedTrialsList = selectedTrials.ToList();

                if (selectedTrialsList.Any())
                {
                    bool isValid = true;
                    List<int> trialList = selectedTrialsList.ElementAt(0).Trials;

                    if (selectedTrialsList.Any(temp => !trialList.SequenceEqual(temp.Trials)))
                    {
                        _myManipAnalysisGui.WriteToLogBox("Trial selections are not equal!");
                        isValid = false;
                    }

                    if (isValid)
                    {
                        int meanCounter;
                        var data = new double[trialList.Count, selectedTrialsList.Count()];

                        for (meanCounter = 0;
                            meanCounter < selectedTrialsList.Count() & !TaskManager.Cancel;
                            meanCounter++)
                        {
                            _myManipAnalysisGui.SetProgressBarValue((100.0/selectedTrialsList.Count())*meanCounter);
                            StatisticPlotContainer tempStatisticPlotContainer = selectedTrialsList.ElementAt(meanCounter);

                            DateTime turnDateTime = GetTurnDateTime(tempStatisticPlotContainer.Study,
                                tempStatisticPlotContainer.Group,
                                tempStatisticPlotContainer.Szenario,
                                tempStatisticPlotContainer.Subject,
                                Convert.ToInt32(
                                    tempStatisticPlotContainer.Turn.Substring(
                                        "Turn".Length)));

                            if (pdTime == -1)
                            {
                                DataSet statisticDataSet =
                                    _myDatabaseWrapper.GetStatisticDataSet(tempStatisticPlotContainer.Study,
                                        tempStatisticPlotContainer.Group,
                                        tempStatisticPlotContainer.Szenario,
                                        tempStatisticPlotContainer.Subject,
                                        turnDateTime);


                                int trialListCounter = 0;
                                foreach (DataRow row in statisticDataSet.Tables[0].Rows)
                                {
                                    if (TaskManager.Cancel)
                                    {
                                        break;
                                    }
                                    if (!row["szenario_trial_number"].GetType().IsInstanceOfType(DBNull.Value))
                                    {
                                        int szenarioTrialNumber = Convert.ToInt32(row["szenario_trial_number"]);
                                        if (trialList.Contains(szenarioTrialNumber))
                                        {
                                            switch (statisticType)
                                            {
                                                case "Vector correlation":
                                                    data[trialListCounter, meanCounter] =
                                                        Convert.ToDouble(row["velocity_vector_correlation"]);
                                                    break;

                                                case "Vector correlation fisher-z":
                                                    _myMatlabWrapper.SetWorkspaceData("vcorr",
                                                        Convert.ToDouble(row["velocity_vector_correlation"]));
                                                    _myMatlabWrapper.Execute(
                                                        "fisherZ = vectorCorrelationFisherZTransform(vcorr);");
                                                    data[trialListCounter, meanCounter] =
                                                        _myMatlabWrapper.GetWorkspaceData("fisherZ");
                                                    _myMatlabWrapper.ClearWorkspace();
                                                    break;

                                                case "Vector correlation fisher-z to r-values":
                                                    _myMatlabWrapper.SetWorkspaceData("vcorr",
                                                        Convert.ToDouble(row["velocity_vector_correlation"]));
                                                    _myMatlabWrapper.Execute(
                                                        "fisherZ = vectorCorrelationFisherZTransform(vcorr);");
                                                    data[trialListCounter, meanCounter] =
                                                        _myMatlabWrapper.GetWorkspaceData("fisherZ");
                                                    _myMatlabWrapper.ClearWorkspace();
                                                    break;

                                                case "Perpendicular distance 300ms - Abs":
                                                    data[trialListCounter, meanCounter] =
                                                        Convert.ToDouble(row["perpendicular_displacement_300ms_abs"]);
                                                    break;

                                                case "Mean perpendicular distance - Abs":
                                                    data[trialListCounter, meanCounter] =
                                                        Convert.ToDouble(row["mean_perpendicular_displacement_abs"]);
                                                    break;

                                                case "Max perpendicular distance - Abs":
                                                    data[trialListCounter, meanCounter] =
                                                        Convert.ToDouble(row["maximal_perpendicular_displacement_abs"]);
                                                    break;

                                                case "Perpendicular distance 300ms - Sign":
                                                    data[trialListCounter, meanCounter] =
                                                        Convert.ToDouble(row["perpendicular_displacement_300ms_sign"]);
                                                    break;

                                                case "Max perpendicular distance - Sign":
                                                    data[trialListCounter, meanCounter] =
                                                        Convert.ToDouble(row["maximal_perpendicular_displacement_sign"]);
                                                    break;

                                                case "Trajectory length abs":
                                                    data[trialListCounter, meanCounter] =
                                                        Convert.ToDouble(row["trajectory_length_abs"]);
                                                    break;

                                                case "Trajectory length ratio":
                                                    data[trialListCounter, meanCounter] =
                                                        Convert.ToDouble(row["trajectory_length_ratio_baseline"]);
                                                    break;

                                                case "Enclosed area":
                                                    data[trialListCounter, meanCounter] =
                                                        Convert.ToDouble(row["enclosed_area"]);
                                                    break;

                                                case "RMSE":
                                                    data[trialListCounter, meanCounter] = Convert.ToDouble(row["rmse"]);
                                                    break;
                                            }
                                            trialListCounter++;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                DataSet baselineDataSet = null;

                                if (tempStatisticPlotContainer.Study == "Study 2" ||
                                    tempStatisticPlotContainer.Study == "Study 1" ||
                                    tempStatisticPlotContainer.Study == "Study 5")
                                {
                                    baselineDataSet = _myDatabaseWrapper.GetBaselineDataSet(tempStatisticPlotContainer.Study,
                                        tempStatisticPlotContainer.Group, "Szenario02",
                                        tempStatisticPlotContainer.Subject);
                                }
                                else if (tempStatisticPlotContainer.Study == "Study 3")
                                {
                                    baselineDataSet = _myDatabaseWrapper.GetBaselineDataSet(tempStatisticPlotContainer.Study,
                                        tempStatisticPlotContainer.Group, "Szenario30",
                                        tempStatisticPlotContainer.Subject);
                                }
                                else if (tempStatisticPlotContainer.Study == "Study 4")
                                {
                                    if (tempStatisticPlotContainer.Szenario.Contains("_R"))
                                    {
                                        baselineDataSet = _myDatabaseWrapper.GetBaselineDataSet(tempStatisticPlotContainer.Study,
                                          tempStatisticPlotContainer.Group, "Szenario42_R",
                                          tempStatisticPlotContainer.Subject);
                                    }
                                    else if (tempStatisticPlotContainer.Szenario.Contains("_N"))
                                    {
                                        baselineDataSet = _myDatabaseWrapper.GetBaselineDataSet(tempStatisticPlotContainer.Study,
                                            tempStatisticPlotContainer.Group, "Szenario42_N",
                                            tempStatisticPlotContainer.Subject);
                                    }
                                }
                                var baselineDataList = new LinkedList<BaselineDataContainer>();

                                for (int baselineSamples = 0;
                                    baselineSamples < baselineDataSet.Tables[0].Rows.Count;
                                    baselineSamples++)
                                {
                                    baselineDataList.AddLast(new BaselineDataContainer(
                                        Convert.ToDateTime(
                                            baselineDataSet.Tables[0].Rows[baselineSamples]["pseudo_time_stamp"]),
                                        Convert.ToDouble(
                                            baselineDataSet.Tables[0].Rows[baselineSamples][
                                                "baseline_position_cartesian_x"]),
                                        Convert.ToDouble(
                                            baselineDataSet.Tables[0].Rows[baselineSamples][
                                                "baseline_position_cartesian_y"]),
                                        Convert.ToDouble(
                                            baselineDataSet.Tables[0].Rows[baselineSamples][
                                                "baseline_position_cartesian_z"]),
                                        Convert.ToDouble(
                                            baselineDataSet.Tables[0].Rows[baselineSamples]["baseline_velocity_x"]),
                                        Convert.ToDouble(
                                            baselineDataSet.Tables[0].Rows[baselineSamples]["baseline_velocity_y"]),
                                        Convert.ToDouble(
                                            baselineDataSet.Tables[0].Rows[baselineSamples]["baseline_velocity_z"]),
                                        Convert.ToInt32(baselineDataSet.Tables[0].Rows[baselineSamples]["target_number"])));
                                }

                                for (int trialCounter = 0; trialCounter < trialList.Count; trialCounter++)
                                {
                                    _myManipAnalysisGui.SetProgressBarValue((100.0/trialList.Count)*trialCounter);

                                    int szenarioTrialNumber = trialList.ElementAt(trialCounter);
                                    int trialId = _myDatabaseWrapper.GetTrailID(tempStatisticPlotContainer.Study,
                                        tempStatisticPlotContainer.Group, tempStatisticPlotContainer.Szenario,
                                        tempStatisticPlotContainer.Subject, turnDateTime, szenarioTrialNumber);

                                    DataSet measureDataSet = _myDatabaseWrapper.GetMeasureDataNormalizedDataSet(trialId);
                                    int targetNumber = _myDatabaseWrapper.GetTargetNumber(trialId);


                                    if (baselineDataList.Count > 0 &
                                        measureDataSet.Tables[0].Rows.Count > 0)
                                    {
                                        int sampleCount = measureDataSet.Tables[0].Rows.Count;
                                        var measureData = new double[sampleCount, 3];
                                        var baselineData = new double[sampleCount, 3];
                                        double[][] baselineDataArray =
                                            baselineDataList.Where(t => t.TargetNumber == targetNumber)
                                                .OrderBy(t => t.PseudoTimeStamp)
                                                .Select(
                                                    t =>
                                                        new[]
                                                        {
                                                            t.BaselinePositionCartesianX,
                                                            t.BaselinePositionCartesianY,
                                                            t.BaselinePositionCartesianZ
                                                        }).ToArray();
                                        var timeStamp = new double[sampleCount];

                                        for (int i = 0; i < sampleCount; i++)
                                        {
                                            timeStamp[i] =
                                                Convert.ToDateTime(measureDataSet.Tables[0].Rows[i]["time_stamp"]).Ticks;

                                            measureData[i, 0] =
                                                Convert.ToDouble(
                                                    measureDataSet.Tables[0].Rows[i]["position_cartesian_x"]);
                                            measureData[i, 1] =
                                                Convert.ToDouble(
                                                    measureDataSet.Tables[0].Rows[i]["position_cartesian_y"]);
                                            measureData[i, 2] =
                                                Convert.ToDouble(
                                                    measureDataSet.Tables[0].Rows[i]["position_cartesian_z"]);

                                            baselineData[i, 0] = baselineDataArray[i][0];
                                            baselineData[i, 1] = baselineDataArray[i][1];
                                            baselineData[i, 2] = baselineDataArray[i][2];
                                        }

                                        List<double> tempTimeList = timeStamp.ToList();
                                        int timeMsIndex =
                                            tempTimeList.IndexOf(
                                                tempTimeList.OrderBy(
                                                    d =>
                                                        Math.Abs(d -
                                                                 (timeStamp[0] + TimeSpan.FromMilliseconds(pdTime).Ticks)))
                                                    .ElementAt(0));

                                        _myMatlabWrapper.SetWorkspaceData("targetNumber", targetNumber);
                                        _myMatlabWrapper.SetWorkspaceData("timeMsIndex", timeMsIndex);
                                        _myMatlabWrapper.SetWorkspaceData("measureData", measureData);
                                        _myMatlabWrapper.SetWorkspaceData("baselineData", baselineData);

                                        _myMatlabWrapper.Execute(
                                            "distanceAbs = distance2curveAbs([measureData(:,1),measureData(:,3)],targetNumber);");
                                        _myMatlabWrapper.Execute(
                                            "distanceSign = distance2curveSign([measureData(:,1),measureData(:,3)],targetNumber);");
                                        _myMatlabWrapper.Execute("distanceMsAbs = distanceAbs(timeMsIndex);");
                                        _myMatlabWrapper.Execute("distanceMsSign = distanceSign(timeMsIndex);");

                                        switch (statisticType)
                                        {
                                            case "Perpendicular distance ?ms - Abs":
                                                data[trialCounter, meanCounter] =
                                                    _myMatlabWrapper.GetWorkspaceData("distanceMsAbs");
                                                break;

                                            case "Perpendicular distance ?ms - Sign":
                                                data[trialCounter, meanCounter] =
                                                    _myMatlabWrapper.GetWorkspaceData("distanceMsSign");
                                                break;
                                        }
                                    }
                                }
                            }
                        }

                        _myMatlabWrapper.SetWorkspaceData("data", data);

                        if (meanCounter > 1)
                        {
                            if (statisticType == "Vector correlation fisher-z to r-values")
                            {
                                _myMatlabWrapper.Execute(
                                    "dataPlot = fisherZVectorCorrelationTransform(mean(transpose(data)));");
                                _myMatlabWrapper.Execute(
                                    "dataStdPlot = fisherZVectorCorrelationTransform(std(transpose(data)));");
                            }
                            else
                            {
                                _myMatlabWrapper.Execute("dataPlot = mean(transpose(data));");
                                _myMatlabWrapper.Execute("dataStdPlot = std(transpose(data));");
                            }
                        }
                        else
                        {
                            if (statisticType == "Vector correlation fisher-z to r-values")
                            {
                                _myMatlabWrapper.Execute("dataPlot = fisherZVectorCorrelationTransform(data);");
                            }
                            else
                            {
                                _myMatlabWrapper.Execute("dataPlot = data;");
                            }
                        }

                        double[,] dataPlot = null;
                        if (Object.ReferenceEquals(_myMatlabWrapper.GetWorkspaceData("dataPlot").GetType(),
                            typeof (double[,])))
                        {
                            dataPlot = _myMatlabWrapper.GetWorkspaceData("dataPlot");
                        }
                        else if (Object.ReferenceEquals(_myMatlabWrapper.GetWorkspaceData("dataPlot").GetType(),
                            typeof (double)))
                        {
                            dataPlot = new double[1, 1];
                            dataPlot[0, 0] = _myMatlabWrapper.GetWorkspaceData("dataPlot");
                        }
                        if (dataPlot != null)
                        {
                            switch (statisticType)
                            {
                                case "Vector correlation":

                                    _myMatlabWrapper.CreateStatisticFigure("Velocity Vector Correlation plot",
                                        "dataPlot",
                                        "fit(transpose([1:1:length(dataPlot)]),transpose(dataPlot),'" +
                                        fitEquation + "')",
                                        "dataStdPlot", "[Trial]",
                                        "Velocity Vector Correlation", 1, dataPlot.Length,
                                        0.5, 1,
                                        plotFit,
                                        plotErrorbars);
                                    break;

                                case "Vector correlation fisher-z":
                                    _myMatlabWrapper.CreateStatisticFigure("Velocity Vector Correlation Fisher Z plot",
                                        "dataPlot",
                                        "fit(transpose([1:1:length(dataPlot)]),transpose(dataPlot),'" +
                                        fitEquation + "')",
                                        "dataStdPlot", "[Trial]",
                                        "Velocity Vector Correlation Fisher Z", 1, dataPlot.Length,
                                        0.0, 2.0,
                                        plotFit,
                                        plotErrorbars);
                                    break;

                                case "Vector correlation fisher-z to r-values":
                                    _myMatlabWrapper.CreateStatisticFigure(
                                        "Velocity Vector Correlation Fisher Z to r-Values  plot", "dataPlot",
                                        "fit(transpose([1:1:length(dataPlot)]),transpose(dataPlot),'" +
                                        fitEquation + "')",
                                        "dataStdPlot", "[Trial]",
                                        "Velocity Vector Correlation Fisher Z", 1, dataPlot.Length,
                                        0.5, 1,
                                        plotFit,
                                        plotErrorbars);
                                    break;

                                case "Perpendicular distance 300ms - Abs":
                                    _myMatlabWrapper.CreateStatisticFigure("PD300 abs plot", "dataPlot",
                                        "fit(transpose([1:1:length(dataPlot)]),transpose(dataPlot),'" +
                                        fitEquation + "')",
                                        "dataStdPlot", "[Trial]", "PD300 [m]", 1,
                                        dataPlot.Length, 0, 0.05,
                                        plotFit,
                                        plotErrorbars);
                                    break;

                                case "Perpendicular distance ?ms - Abs":
                                    _myMatlabWrapper.CreateStatisticFigure("PD" + pdTime + " abs plot", "dataPlot",
                                        "fit(transpose([1:1:length(dataPlot)]),transpose(dataPlot),'" +
                                        fitEquation + "')",
                                        "dataStdPlot", "[Trial]", "PD" + pdTime + " [m]", 1,
                                        dataPlot.Length, 0, 0.05,
                                        plotFit,
                                        plotErrorbars);
                                    break;

                                case "Mean perpendicular distance - Abs":
                                    _myMatlabWrapper.CreateStatisticFigure("MeanPD abs plot", "dataPlot",
                                        "fit(transpose([1:1:length(dataPlot)]),transpose(dataPlot),'" +
                                        fitEquation + "')",
                                        "dataStdPlot", "[Trial]", "MeanPD [m]", 1,
                                        dataPlot.Length, 0, 0.05,
                                        plotFit,
                                        plotErrorbars);
                                    break;

                                case "Max perpendicular distance - Abs":
                                    _myMatlabWrapper.CreateStatisticFigure("MaxPD abs plot", "dataPlot",
                                        "fit(transpose([1:1:length(dataPlot)]),transpose(dataPlot),'" +
                                        fitEquation + "')",
                                        "dataStdPlot", "[Trial]", "MaxPD [m]", 1,
                                        dataPlot.Length, 0, 0.05,
                                        plotFit,
                                        plotErrorbars);
                                    break;

                                case "Perpendicular distance 300ms - Sign":
                                    _myMatlabWrapper.CreateStatisticFigure("PD300 sign plot", "dataPlot",
                                        "fit(transpose([1:1:length(dataPlot)]),transpose(dataPlot),'" +
                                        fitEquation + "')",
                                        "dataStdPlot", "[Trial]", "PD300 [m]", 1,
                                        dataPlot.Length, -0.05, 0.05,
                                        plotFit,
                                        plotErrorbars);
                                    break;

                                case "Perpendicular distance ?ms - Sign":
                                    _myMatlabWrapper.CreateStatisticFigure("PD" + pdTime + " sign plot", "dataPlot",
                                        "fit(transpose([1:1:length(dataPlot)]),transpose(dataPlot),'" +
                                        fitEquation + "')",
                                        "dataStdPlot", "[Trial]", "PD" + pdTime + " [m]", 1,
                                        dataPlot.Length, -0.05, 0.05,
                                        plotFit,
                                        plotErrorbars);
                                    break;

                                case "Max perpendicular distance - Sign":
                                    _myMatlabWrapper.CreateStatisticFigure("MaxPD sign plot", "dataPlot",
                                        "fit(transpose([1:1:length(dataPlot)]),transpose(dataPlot),'" +
                                        fitEquation + "')",
                                        "dataStdPlot", "[Trial]", "MaxPD [m]", 1,
                                        dataPlot.Length, -0.05, 0.05,
                                        plotFit,
                                        plotErrorbars);
                                    break;

                                case "Trajectory length abs":
                                    _myMatlabWrapper.CreateStatisticFigure("Trajectory Length plot", "dataPlot",
                                        "fit(transpose([1:1:length(dataPlot)]),transpose(dataPlot),'" +
                                        fitEquation + "')",
                                        "dataStdPlot", "[Trial]", "Trajectory Length [m]", 1,
                                        dataPlot.Length, 0.07, 0.2,
                                        plotFit,
                                        plotErrorbars);
                                    break;

                                case "Trajectory length ratio":
                                    _myMatlabWrapper.CreateStatisticFigure("Trajectory Length Ratio plot", "dataPlot",
                                        "fit(transpose([1:1:length(dataPlot)]),transpose(dataPlot),'" +
                                        fitEquation + "')",
                                        "dataStdPlot", "[Trial]", "Trajectory Length Ratio",
                                        1, dataPlot.Length, 0.2, 1.8,
                                        plotFit,
                                        plotErrorbars);
                                    break;

                                case "Enclosed area":
                                    _myMatlabWrapper.CreateStatisticFigure("Enclosed area plot", "dataPlot",
                                        "fit(transpose([1:1:length(dataPlot)]),transpose(dataPlot),'" +
                                        fitEquation + "')",
                                        "dataStdPlot", "[Trial]", "Enclosed Area [m²]", 1,
                                        dataPlot.Length, 0, 0.002,
                                        plotFit,
                                        plotErrorbars);
                                    break;

                                case "RMSE":
                                    _myMatlabWrapper.CreateStatisticFigure("Root Mean Square Error plot", "dataPlot",
                                        "fit(transpose([1:1:length(dataPlot)]),transpose(dataPlot),'" +
                                        fitEquation + "')",
                                        "dataStdPlot", "[Trial]", "Root Mean Square Error", 1,
                                        dataPlot.Length, 0, 0.1,
                                        plotFit,
                                        plotErrorbars);

                                    break;
                            }
                        }
                        _myMatlabWrapper.ClearWorkspace();
                    }
                }

                _myManipAnalysisGui.WriteProgressInfo("Ready");
                _myManipAnalysisGui.SetProgressBarValue(0);
                TaskManager.Remove(Task.CurrentId);
            }));
        }

        public IEnumerable<object[]> GetFaultyTrialInformation()
        {
            return _myDatabaseWrapper.GetFaultyTrialInformation();
        }

        public void ExportDescriptiveStatistic1Data(IEnumerable<StatisticPlotContainer> selectedTrials,
            string statisticType, int pdTime, string fileName)
        {
            TaskManager.PushBack(Task.Factory.StartNew(() =>
            {
                bool isValid = true;
                List<StatisticPlotContainer> selectedTrialsList = selectedTrials.ToList();
                List<int> trialList = selectedTrialsList.ElementAt(0).Trials;

                if (selectedTrialsList.Any(temp => !trialList.SequenceEqual(temp.Trials)))
                {
                    _myManipAnalysisGui.WriteToLogBox("Trial selections are not equal!");
                    isValid = false;
                }

                if (isValid)
                {
                    int meanCounter;
                    int trialCounter = 0;
                    var data = new double[trialList.Count, selectedTrialsList.Count()];

                    for (meanCounter = 0;
                        meanCounter < selectedTrialsList.Count() & !TaskManager.Cancel;
                        meanCounter++)
                    {
                        _myManipAnalysisGui.SetProgressBarValue((100.0 / selectedTrialsList.Count()) *
                                                                meanCounter);

                        StatisticPlotContainer tempStatisticPlotContainer = selectedTrialsList.ElementAt(meanCounter);

                        DateTime turnDateTime = _myDatabaseWrapper.GetTurnDateTime(tempStatisticPlotContainer.Study, tempStatisticPlotContainer.Group, tempStatisticPlotContainer.Szenario,
                            tempStatisticPlotContainer.Subject,
                            Convert.ToInt32(
                                tempStatisticPlotContainer.Turn.Substring("Turn".Length)));

                        if (pdTime == -1)
                        {
                            DataSet statisticDataSet = _myDatabaseWrapper.GetStatisticDataSet(tempStatisticPlotContainer.Study, tempStatisticPlotContainer.Group,
                                tempStatisticPlotContainer.Szenario, tempStatisticPlotContainer.Subject,
                                turnDateTime);

                            trialCounter = 0;
                            for (int rowCounter = 0;
                                rowCounter < statisticDataSet.Tables[0].Rows.Count & !TaskManager.Cancel;
                                rowCounter++)
                            {
                                DataRow row = statisticDataSet.Tables[0].Rows[rowCounter];

                                int szenarioTrialNumber = Convert.ToInt32(row["szenario_trial_number"]);
                                if (trialList.Contains(szenarioTrialNumber))
                                {
                                    switch (statisticType)
                                    {
                                        case "Vector correlation":
                                            data[trialCounter, meanCounter] =
                                                Convert.ToDouble(row["velocity_vector_correlation"]);
                                            break;

                                        case "Vector correlation fisher-z":
                                            _myMatlabWrapper.SetWorkspaceData("vcorr",
                                                Convert.ToDouble(row["velocity_vector_correlation"]));
                                            _myMatlabWrapper.Execute("fisherZ = vectorCorrelationFisherZTransform(vcorr);");
                                            data[trialCounter, meanCounter] =
                                                _myMatlabWrapper.GetWorkspaceData("fisherZ");
                                            _myMatlabWrapper.ClearWorkspace();
                                            break;

                                        case "Vector correlation fisher-z to r-values":
                                            _myMatlabWrapper.SetWorkspaceData("vcorr",
                                                Convert.ToDouble(row["velocity_vector_correlation"]));
                                            _myMatlabWrapper.Execute("fisherZ = vectorCorrelationFisherZTransform(vcorr);");
                                            data[trialCounter, meanCounter] =
                                                _myMatlabWrapper.GetWorkspaceData("fisherZ");
                                            _myMatlabWrapper.ClearWorkspace();
                                            break;

                                        case "Perpendicular distance 300ms - Abs":
                                            data[trialCounter, meanCounter] =
                                                Convert.ToDouble(row["perpendicular_displacement_300ms_abs"]);
                                            break;

                                        case "Mean perpendicular distance - Abs":
                                            data[trialCounter, meanCounter] =
                                                Convert.ToDouble(row["mean_perpendicular_displacement_abs"]);
                                            break;

                                        case "Max perpendicular distance - Abs":
                                            data[trialCounter, meanCounter] =
                                                Convert.ToDouble(row["maximal_perpendicular_displacement_abs"]);
                                            break;

                                        case "Perpendicular distance 300ms - Sign":
                                            data[trialCounter, meanCounter] =
                                                Convert.ToDouble(row["perpendicular_displacement_300ms_sign"]);
                                            break;

                                        case "Max perpendicular distance - Sign":
                                            data[trialCounter, meanCounter] =
                                                Convert.ToDouble(row["maximal_perpendicular_displacement_sign"]);
                                            break;

                                        case "Trajectory length abs":
                                            data[trialCounter, meanCounter] =
                                                Convert.ToDouble(row["trajectory_length_abs"]);
                                            break;

                                        case "Trajectory length ratio":
                                            data[trialCounter, meanCounter] =
                                                Convert.ToDouble(row["trajectory_length_ratio_baseline"]);
                                            break;

                                        case "Enclosed area":
                                            data[trialCounter, meanCounter] = Convert.ToDouble(row["enclosed_area"]);
                                            break;

                                        case "RMSE":
                                            data[trialCounter, meanCounter] = Convert.ToDouble(row["rmse"]);
                                            break;
                                    }
                                    trialCounter++;
                                }
                            }
                        }
                        else
                        {
                            DataSet baselineDataSet = null;

                            if (tempStatisticPlotContainer.Study == "Study 2" ||
                                tempStatisticPlotContainer.Study == "Study 1" ||
                                tempStatisticPlotContainer.Study == "Study 5")
                            {
                                baselineDataSet = _myDatabaseWrapper.GetBaselineDataSet(tempStatisticPlotContainer.Study,
                                    tempStatisticPlotContainer.Group, "Szenario02",
                                    tempStatisticPlotContainer.Subject);
                            }
                            else if (tempStatisticPlotContainer.Study == "Study 3")
                            {
                                baselineDataSet = _myDatabaseWrapper.GetBaselineDataSet(tempStatisticPlotContainer.Study,
                                    tempStatisticPlotContainer.Group, "Szenario30",
                                    tempStatisticPlotContainer.Subject);
                            }
                            else if (tempStatisticPlotContainer.Study == "Study 4")
                            {
                                if (tempStatisticPlotContainer.Szenario.Contains("_R"))
                                {
                                    baselineDataSet = _myDatabaseWrapper.GetBaselineDataSet(tempStatisticPlotContainer.Study,
                                      tempStatisticPlotContainer.Group, "Szenario42_R",
                                      tempStatisticPlotContainer.Subject);
                                }
                                else if (tempStatisticPlotContainer.Szenario.Contains("_N"))
                                {
                                    baselineDataSet = _myDatabaseWrapper.GetBaselineDataSet(tempStatisticPlotContainer.Study,
                                        tempStatisticPlotContainer.Group, "Szenario42_N",
                                        tempStatisticPlotContainer.Subject);
                                }
                            }
                            LinkedList<BaselineDataContainer> baselineDataList = new LinkedList<BaselineDataContainer>();

                            for (int baselineSamples = 0; baselineSamples < baselineDataSet.Tables[0].Rows.Count; baselineSamples++)
                            {
                                baselineDataList.AddLast(new BaselineDataContainer(
                                    Convert.ToDateTime(baselineDataSet.Tables[0].Rows[baselineSamples]["pseudo_time_stamp"]),
                                    Convert.ToDouble(baselineDataSet.Tables[0].Rows[baselineSamples]["baseline_position_cartesian_x"]),
                                    Convert.ToDouble(baselineDataSet.Tables[0].Rows[baselineSamples]["baseline_position_cartesian_y"]),
                                    Convert.ToDouble(baselineDataSet.Tables[0].Rows[baselineSamples]["baseline_position_cartesian_z"]),
                                    Convert.ToDouble(baselineDataSet.Tables[0].Rows[baselineSamples]["baseline_velocity_x"]),
                                    Convert.ToDouble(baselineDataSet.Tables[0].Rows[baselineSamples]["baseline_velocity_y"]),
                                    Convert.ToDouble(baselineDataSet.Tables[0].Rows[baselineSamples]["baseline_velocity_z"]),
                                    Convert.ToInt32(baselineDataSet.Tables[0].Rows[baselineSamples]["target_number"])));
                            }

                            for (trialCounter = 0; trialCounter < trialList.Count; trialCounter++)
                            {
                                _myManipAnalysisGui.SetProgressBarValue((100.0 / trialList.Count) * trialCounter);

                                int szenarioTrialNumber = trialList.ElementAt(trialCounter);
                                int trialId = _myDatabaseWrapper.GetTrailID(tempStatisticPlotContainer.Study,
                                    tempStatisticPlotContainer.Group, tempStatisticPlotContainer.Szenario,
                                    tempStatisticPlotContainer.Subject, turnDateTime, szenarioTrialNumber);

                                DataSet measureDataSet = _myDatabaseWrapper.GetMeasureDataNormalizedDataSet(trialId);
                                int targetNumber = _myDatabaseWrapper.GetTargetNumber(trialId);


                                if (baselineDataList.Count > 0 &
                                    measureDataSet.Tables[0].Rows.Count > 0)
                                {

                                    int sampleCount = measureDataSet.Tables[0].Rows.Count;
                                    var measureData = new double[sampleCount, 3];
                                    var baselineData = new double[sampleCount, 3];
                                    var baselineDataArray =
                                        baselineDataList.Where(t => t.TargetNumber == targetNumber)
                                            .OrderBy(t => t.PseudoTimeStamp)
                                            .Select(
                                                t =>
                                                    new double[]
                                                        {
                                                            t.BaselinePositionCartesianX, 
                                                            t.BaselinePositionCartesianY,
                                                            t.BaselinePositionCartesianZ
                                                        }).ToArray();
                                    var timeStamp = new double[sampleCount];

                                    for (int i = 0; i < sampleCount; i++)
                                    {
                                        timeStamp[i] =
                                            Convert.ToDateTime(measureDataSet.Tables[0].Rows[i]["time_stamp"]).Ticks;

                                        measureData[i, 0] =
                                            Convert.ToDouble(
                                                measureDataSet.Tables[0].Rows[i]["position_cartesian_x"]);
                                        measureData[i, 1] =
                                            Convert.ToDouble(
                                                measureDataSet.Tables[0].Rows[i]["position_cartesian_y"]);
                                        measureData[i, 2] =
                                            Convert.ToDouble(
                                                measureDataSet.Tables[0].Rows[i]["position_cartesian_z"]);

                                        baselineData[i, 0] = baselineDataArray[i][0];
                                        baselineData[i, 1] = baselineDataArray[i][1];
                                        baselineData[i, 2] = baselineDataArray[i][2];
                                    }

                                    List<double> tempTimeList = timeStamp.ToList();
                                    int timeMsIndex =
                                        tempTimeList.IndexOf(
                                            tempTimeList.OrderBy(
                                                d =>
                                                    Math.Abs(d -
                                                             (timeStamp[0] + TimeSpan.FromMilliseconds(pdTime).Ticks)))
                                                .ElementAt(0));

                                    _myMatlabWrapper.SetWorkspaceData("targetNumber", targetNumber);
                                    _myMatlabWrapper.SetWorkspaceData("timeMsIndex", timeMsIndex);
                                    _myMatlabWrapper.SetWorkspaceData("measureData", measureData);
                                    _myMatlabWrapper.SetWorkspaceData("baselineData", baselineData);

                                    _myMatlabWrapper.Execute(
                                        "distanceAbs = distance2curveAbs([measureData(:,1),measureData(:,3)],targetNumber);");
                                    _myMatlabWrapper.Execute(
                                        "distanceSign = distance2curveSign([measureData(:,1),measureData(:,3)],targetNumber);");
                                    _myMatlabWrapper.Execute("distanceMsAbs = distanceAbs(timeMsIndex);");
                                    _myMatlabWrapper.Execute("distanceMsSign = distanceSign(timeMsIndex);");

                                    switch (statisticType)
                                    {
                                        case "Perpendicular distance ?ms - Abs":
                                            data[trialCounter, meanCounter] =
                                                _myMatlabWrapper.GetWorkspaceData("distanceMsAbs");
                                            break;

                                        case "Perpendicular distance ?ms - Sign":
                                            data[trialCounter, meanCounter] =
                                                _myMatlabWrapper.GetWorkspaceData("distanceMsSign");
                                            break;

                                    }
                                }
                            }
                        }
                    }

                    _myMatlabWrapper.SetWorkspaceData("data", data);

                    double[,] dataMean;
                    double[,] dataStd;

                    if (meanCounter > 1)
                    {
                        if (statisticType == "Vector correlation fisher-z to r-values")
                        {
                            _myMatlabWrapper.Execute(
                                "dataMean = fisherZVectorCorrelationTransform(mean(transpose(data)));");
                            _myMatlabWrapper.Execute(
                                "dataStd = fisherZVectorCorrelationTransform(std(transpose(data)));");
                        }
                        else
                        {
                            _myMatlabWrapper.Execute("dataMean = mean(transpose(data));");
                            _myMatlabWrapper.Execute("dataStd = std(transpose(data));");
                        }
                        dataMean = _myMatlabWrapper.GetWorkspaceData("dataMean");
                        dataStd = _myMatlabWrapper.GetWorkspaceData("dataStd");
                    }
                    else
                    {
                        dataMean = new double[,] { { 0, 0 } };
                        dataStd = new double[,] { { 0, 0 } };
                    }

                    var cache = new List<string>();
                    var meanDataFileStream = new FileStream(fileName, FileMode.Create,
                        FileAccess.Write);
                    var meanDataFileWriter = new StreamWriter(meanDataFileStream);

                    string personNames = "";
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

                    for (int i = 0; i < trialCounter & !TaskManager.Cancel; i++)
                    {
                        string tempLine = trialList.ElementAt(i) + ";";

                        for (int j = 0; j < meanCounter; j++)
                        {
                            tempLine += DoubleConverter.ToExactString(data[i, j]) + ";";
                        }

                        if (meanCounter > 1)
                        {
                            tempLine += DoubleConverter.ToExactString(dataMean[0, i])
                                        + ";"
                                        + DoubleConverter.ToExactString(dataStd[0, i]);
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

                    _myMatlabWrapper.ClearWorkspace();
                }

                _myManipAnalysisGui.WriteProgressInfo("Ready");
                _myManipAnalysisGui.SetProgressBarValue(0);
                TaskManager.Remove(Task.CurrentId);
            }));
        }

        public void ExportDescriptiveStatistic2Data(IEnumerable<StatisticPlotContainer> selectedTrials,
            string statisticType, string fileName)
        {
            TaskManager.PushBack(Task.Factory.StartNew(() =>
            {
                List<StatisticPlotContainer> selectedTrialsList = selectedTrials.ToList();
                List<int> trialList = selectedTrialsList.ElementAt(0).Trials;

                var data = new double[trialList.Count, selectedTrialsList.Count()];

                for (int meanCounter = 0;
                    meanCounter < selectedTrialsList.Count() & !TaskManager.Cancel;
                    meanCounter++)
                {
                    _myManipAnalysisGui.SetProgressBarValue((100.0/selectedTrialsList.Count())*meanCounter);
                    StatisticPlotContainer temp = selectedTrialsList.ElementAt(meanCounter);

                    DateTime turn = _myDatabaseWrapper.GetTurnDateTime(temp.Study, temp.Group, temp.Szenario,
                        temp.Subject,
                        Convert.ToInt32(temp.Turn.Substring("Turn".Length)));
                    DataSet statisticDataSet = _myDatabaseWrapper.GetStatisticDataSet(temp.Study, temp.Group,
                        temp.Szenario, temp.Subject,
                        turn);

                    int trialListCounter = 0;
                    foreach (DataRow row in statisticDataSet.Tables[0].Rows)
                    {
                        if (TaskManager.Cancel)
                        {
                            break;
                        }
                        int szenarioTrialNumber = Convert.ToInt32(row["szenario_trial_number"]);
                        if (trialList.Contains(szenarioTrialNumber))
                        {
                            switch (statisticType)
                            {
                                case "Vector correlation":
                                    data[trialListCounter, meanCounter] =
                                        Convert.ToDouble(row["velocity_vector_correlation"]);
                                    break;

                                case "Vector correlation fisher-z":
                                    _myMatlabWrapper.SetWorkspaceData("vcorr",
                                        Convert.ToDouble(row["velocity_vector_correlation"]));
                                    _myMatlabWrapper.Execute("fisherZ = vectorCorrelationFisherZTransform(vcorr);");
                                    data[trialListCounter, meanCounter] = _myMatlabWrapper.GetWorkspaceData("fisherZ");
                                    _myMatlabWrapper.ClearWorkspace();
                                    break;

                                case "Vector correlation fisher-z to r-values":
                                    _myMatlabWrapper.SetWorkspaceData("vcorr",
                                        Convert.ToDouble(row["velocity_vector_correlation"]));
                                    _myMatlabWrapper.Execute("fisherZ = vectorCorrelationFisherZTransform(vcorr);");
                                    data[trialListCounter, meanCounter] = _myMatlabWrapper.GetWorkspaceData("fisherZ");
                                    _myMatlabWrapper.ClearWorkspace();
                                    break;

                                case "Perpendicular distance 300ms - Abs":
                                    data[trialListCounter, meanCounter] =
                                        Convert.ToDouble(row["perpendicular_displacement_300ms_abs"]);
                                    break;

                                case "Mean perpendicular distance - Abs":
                                    data[trialListCounter, meanCounter] =
                                        Convert.ToDouble(row["mean_perpendicular_displacement_abs"]);
                                    break;

                                case "Max perpendicular distance - Abs":
                                    data[trialListCounter, meanCounter] =
                                        Convert.ToDouble(row["maximal_perpendicular_displacement_abs"]);
                                    break;

                                case "Perpendicular distance 300ms - Sign":
                                    data[trialListCounter, meanCounter] =
                                        Convert.ToDouble(row["perpendicular_displacement_300ms_sign"]);
                                    break;

                                case "Max perpendicular distance - Sign":
                                    data[trialListCounter, meanCounter] =
                                        Convert.ToDouble(row["maximal_perpendicular_displacement_sign"]);
                                    break;

                                case "Trajectory length abs":
                                    data[trialListCounter, meanCounter] =
                                        Convert.ToDouble(row["trajectory_length_abs"]);
                                    break;

                                case "Trajectory length ratio":
                                    data[trialListCounter, meanCounter] =
                                        Convert.ToDouble(row["trajectory_length_ratio_baseline"]);
                                    break;

                                case "Enclosed area":
                                    data[trialListCounter, meanCounter] = Convert.ToDouble(row["enclosed_area"]);
                                    break;

                                case "RMSE":
                                    data[trialListCounter, meanCounter] = Convert.ToDouble(row["rmse"]);
                                    break;
                            }
                            trialListCounter++;
                        }
                    }
                }

                double[,] dataMean;
                double[,] dataStd;

                if (trialList.Count > 1 & selectedTrialsList.Count() > 1)
                {
                    _myMatlabWrapper.SetWorkspaceData("data", data);

                    if (statisticType == "Vector correlation fisher-z to r-values")
                    {
                        _myMatlabWrapper.Execute("dataMean = fisherZVectorCorrelationTransform(mean(transpose(data)));");
                        _myMatlabWrapper.Execute("dataStd = fisherZVectorCorrelationTransform(std(transpose(data)));");
                    }
                    else
                    {
                        _myMatlabWrapper.Execute("dataMean = mean(transpose(data));");
                        _myMatlabWrapper.Execute("dataStd = std(transpose(data));");
                    }

                    dataMean = _myMatlabWrapper.GetWorkspaceData("dataMean");
                    dataStd = _myMatlabWrapper.GetWorkspaceData("dataStd");
                }
                else
                {
                    dataMean = new[,] {{data[0, 0], 0}};
                    dataStd = new double[,] {{0, 0}};
                }

                var cache = new List<string>();
                var meanDataFileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                var meanDataFileWriter = new StreamWriter(meanDataFileStream);
                cache.Add("Study;Group;Szenario;Subject;Turn;Trials;Mean;Std");

                cache.AddRange(selectedTrialsList.Cast<object>()
                    .Select((t, i) => selectedTrialsList.ElementAt(i))
                    .Select(
                        (tempStatisticPlotContainer, i) =>
                            tempStatisticPlotContainer.Study + ";" +
                            tempStatisticPlotContainer.Group + ";" +
                            tempStatisticPlotContainer.Szenario + ";" +
                            tempStatisticPlotContainer.Subject + ";" +
                            tempStatisticPlotContainer.Turn + ";" +
                            tempStatisticPlotContainer.GetTrialsString() +
                            ";" +
                            DoubleConverter.ToExactString(dataMean[0, i]) +
                            ";" +
                            DoubleConverter.ToExactString(dataStd[0, i])));

                for (int i = 0; i < cache.Count() & !TaskManager.Cancel; i++)
                {
                    meanDataFileWriter.WriteLine(cache[i]);
                }

                meanDataFileWriter.Close();

                _myMatlabWrapper.ClearWorkspace();
                TaskManager.Remove(Task.CurrentId);
            }));
        }

        public void ChangeGroupId(int groupId, int newGroupId)
        {
            _myDatabaseWrapper.ChangeGroupID(groupId, newGroupId);
        }

        public void ChangeGroupGroupName(int groupId, string newGroupName)
        {
            _myDatabaseWrapper.ChangeGroupName(groupId, newGroupName);
        }

        public void ChangeSubjectId(int subjectId, int newSubjectId)
        {
            _myDatabaseWrapper.ChangeSubjectID(subjectId, newSubjectId);
        }

        public void ChangeSubjectSubjectName(int subjectId, string newSubjectName)
        {
            _myDatabaseWrapper.ChangeSubjectName(subjectId, newSubjectName);
        }

        public void ChangeSubjectSubjectId(int subjectId, string newSubjectSubjectId)
        {
            _myDatabaseWrapper.ChangeSubjectSubjectID(subjectId, newSubjectSubjectId);
        }

        public void ImportMeasureFiles(IEnumerable<string> measureFiles, int samplesPerSecond, int butterFilterOrder,
            int butterFilterCutOffPosition, int butterFilterCutOffForce, int percentPeakVelocity,
            int timeNormalizationSamples)
        {
            List<string> measureFilesList = measureFiles.ToList();

            var newTask =
                new Task(
                    () => ImportMeasureFilesThread(measureFilesList, samplesPerSecond, butterFilterOrder,
                        butterFilterCutOffPosition, butterFilterCutOffForce, percentPeakVelocity,
                        timeNormalizationSamples));

            TaskManager.PushBack(newTask);
            newTask.Start();
        }

        private void ImportMeasureFilesThread(List<string> measureFilesList, int samplesPerSecond, int butterFilterOrder,
            int butterFilterCutOffPosition, int butterFilterCutOffForce, int percentPeakVelocity,
            int timeNormalizationSamples)
        {
            while (TaskManager.GetIndex(Task.CurrentId) != 0 & !TaskManager.Cancel)
            {
                Thread.Sleep(100);
            }

            _myManipAnalysisGui.EnableTabPages(false);
            _myManipAnalysisGui.SetProgressBarValue(0);

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

                        if (myParser.ParseFile(filename))
                        {
                            Trial[] trialsContainer = myParser.TrialsContainer;

                            _myManipAnalysisGui.WriteProgressInfo("Running duplicate entry detection...");

                            /*
                            #region Duplicate entry detection

                            DuplicateEntryDetection(trialsContainer);

                            #endregion
                            */
                            _myManipAnalysisGui.WriteProgressInfo("Filtering data...");

                            #region Butterworth filter

                            _myMatlabWrapper.SetWorkspaceData("filterOrder", Convert.ToDouble(butterFilterOrder));
                            _myMatlabWrapper.SetWorkspaceData("cutoffFreqPosition",
                                Convert.ToDouble(butterFilterCutOffPosition));
                            _myMatlabWrapper.SetWorkspaceData("cutoffFreqForce",
                                Convert.ToDouble(butterFilterCutOffForce));
                            _myMatlabWrapper.SetWorkspaceData("samplesPerSecond", Convert.ToDouble(samplesPerSecond));
                            _myMatlabWrapper.Execute(
                                "[bPosition,aPosition] = butter(filterOrder,(cutoffFreqPosition/(samplesPerSecond/2)));");
                            _myMatlabWrapper.Execute(
                                "[bForce,aForce] = butter(filterOrder,(cutoffFreqForce/(samplesPerSecond/2)));");

                            ButterWorthFilter(trialsContainer);

                            _myMatlabWrapper.ClearWorkspace();

                            #endregion

                            _myManipAnalysisGui.WriteProgressInfo("Calculating velocity...");

                            #region Velocity calcultion

                            VelocityCalculation(trialsContainer, samplesPerSecond);

                            _myMatlabWrapper.ClearWorkspace();

                            #endregion

                            _myManipAnalysisGui.WriteProgressInfo("Normalizing data...");

                            #region Time normalization

                            TimeNormalization(trialsContainer, timeNormalizationSamples, percentPeakVelocity);

                            _myMatlabWrapper.ClearWorkspace();

                            #endregion

                            _myManipAnalysisGui.WriteProgressInfo("Calculating baselines...");

                            #region Calculate baselines

                            if (trialsContainer[0].Szenario == "Szenario02" ||
                                trialsContainer[0].Szenario == "Szenario30" || // Szenario 30 == Study 3 EEG-Baseline
                                trialsContainer[0].Szenario == "Szenario42_R" || // Study 4 rotated seat
                                trialsContainer[0].Szenario == "Szenario42_N" || // Study 4 normal seat
                                trialsContainer[0].Szenario == "Szenario42_NKR" // Study 6 KINARM
                                )
                            {
                                CalculateBaselines(trialsContainer);
                            }

                            #endregion

                            _myManipAnalysisGui.WriteProgressInfo("Calculating szenario mean times...");

                            #region Calculate szenario mean times

                            CalculateSzenarioMeanTimes(trialsContainer);

                            _myMatlabWrapper.ClearWorkspace();

                            #endregion

                            #region Uploading data to MongoDB

                            /*
                            int measureFileId =
                                _mySqlWrapper.InsertMeasureFile(
                                    DateTime.Parse(myDataContainter.MeasureFileCreationTime + " " +
                                                   myDataContainter.MeasureFileCreationDate),
                                    myDataContainter.MeasureFileHash);
                            int studyId = _mySqlWrapper.InsertStudy(myDataContainter.StudyName);
                            int szenarioId = _mySqlWrapper.InsertSzenario(myDataContainter.SzenarioName);
                            int groupId = _mySqlWrapper.InsertGroup(myDataContainter.GroupName);
                            int subjectId = _mySqlWrapper.InsertSubject(myDataContainter.SubjectName,
                                myDataContainter.SubjectID);

                            #region Upload trials

                            for (int i = 0; i < szenarioTrialNumbers.Length; i++)
                            {
                                int iVar = i;
                                _myManipAnalysisGui.WriteProgressInfo("Preparing Trial " + (i + 1) + " of " +
                                                                      szenarioTrialNumbers.Length);

                                List<MeasureDataContainer> measureDataRawList =
                                    myDataContainter.MeasureDataRaw.Where(
                                        t => t.SzenarioTrialNumber == szenarioTrialNumbers[iVar])
                                        .OrderBy(t => t.TimeStamp)
                                        .ToList();

                                int targetId =
                                    _mySqlWrapper.InsertTarget(measureDataRawList.ElementAt(0).TargetNumber);

                                int targetTrialNumberId =
                                    _mySqlWrapper.InsertTargetTrialNumber(
                                        measureDataRawList.ElementAt(0).TargetTrialNumber);

                                int szenarioTrialNumberId =
                                    _mySqlWrapper.InsertSzenarioTrialNumber(szenarioTrialNumbers[iVar]);

                                int trialInformationId =
                                    _mySqlWrapper.InsertTrialInformation(
                                        measureDataRawList.ElementAt(0).ContainsDuplicates,
                                        measureDataRawList.ElementAt(0).IsCatchTrial,
                                        measureDataRawList.ElementAt(0).IsErrorclampTrial,
                                        butterFilterOrder,
                                        butterFilterCutOffPosition,
                                        butterFilterCutOffForce,
                                        percentPeakVelocity);

                                int trialId = _mySqlWrapper.InsertTrial(
                                    subjectId,
                                    studyId,
                                    groupId,
                                    szenarioId,
                                    targetId,
                                    targetTrialNumberId,
                                    szenarioTrialNumberId,
                                    measureFileId,
                                    trialInformationId
                                    );


                                List<string> dataFileCache =
                                    measureDataRawList.Select(
                                        t =>
                                            "," + trialId + "," + t.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss.fffffff") +
                                            "," + DoubleConverter.ToExactString(t.ForceActualX) + "," +
                                            DoubleConverter.ToExactString(t.ForceActualY) + "," +
                                            DoubleConverter.ToExactString(t.ForceActualZ) + "," +
                                            DoubleConverter.ToExactString(t.ForceNominalX) + "," +
                                            DoubleConverter.ToExactString(t.ForceNominalY) + "," +
                                            DoubleConverter.ToExactString(t.ForceNominalZ) + "," +
                                            DoubleConverter.ToExactString(t.ForceMomentX) + "," +
                                            DoubleConverter.ToExactString(t.ForceMomentY) + "," +
                                            DoubleConverter.ToExactString(t.ForceMomentZ) + "," +
                                            DoubleConverter.ToExactString(t.PositionCartesianX) + "," +
                                            DoubleConverter.ToExactString(t.PositionCartesianY) + "," +
                                            DoubleConverter.ToExactString(t.PositionCartesianZ) + "," +
                                            t.PositionStatus).ToList();

                                var dataFileStream = new FileStream("C:\\measureDataRaw.dat", FileMode.Append,
                                    FileAccess.Write);

                                var dataFileWriter = new StreamWriter(dataFileStream);

                                for (int cacheWriter = 0; cacheWriter < dataFileCache.Count(); cacheWriter++)
                                {
                                    dataFileWriter.WriteLine(dataFileCache[cacheWriter]);
                                }


                                dataFileWriter.Close();
                                dataFileCache.Clear();

                                if (
                                    myDataContainter.MeasureDataFiltered.Select(t => t.SzenarioTrialNumber)
                                        .Contains(szenarioTrialNumbers[i]))
                                {
                                    List<MeasureDataContainer> measureDataFilteredList =
                                        myDataContainter.MeasureDataFiltered.Where(
                                            t => t.SzenarioTrialNumber == szenarioTrialNumbers[iVar])
                                            .OrderBy(t => t.TimeStamp)
                                            .ToList();

                                    dataFileCache.AddRange(
                                        measureDataFilteredList.Select(
                                            t =>
                                                "," + trialId + "," +
                                                t.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss.fffffff") + "," +
                                                DoubleConverter.ToExactString(t.ForceActualX) + "," +
                                                DoubleConverter.ToExactString(t.ForceActualY) + "," +
                                                DoubleConverter.ToExactString(t.ForceActualZ) + "," +
                                                DoubleConverter.ToExactString(t.ForceNominalX) + "," +
                                                DoubleConverter.ToExactString(t.ForceNominalY) + "," +
                                                DoubleConverter.ToExactString(t.ForceNominalZ) + "," +
                                                DoubleConverter.ToExactString(t.ForceMomentX) + "," +
                                                DoubleConverter.ToExactString(t.ForceMomentY) + "," +
                                                DoubleConverter.ToExactString(t.ForceMomentZ) + "," +
                                                DoubleConverter.ToExactString(t.PositionCartesianX) + "," +
                                                DoubleConverter.ToExactString(t.PositionCartesianY) + "," +
                                                DoubleConverter.ToExactString(t.PositionCartesianZ) + "," +
                                                t.PositionStatus));

                                    dataFileStream = new FileStream("C:\\measureDataFiltered.dat", FileMode.Append,
                                        FileAccess.Write);
                                    dataFileWriter = new StreamWriter(dataFileStream);

                                    for (int cacheWriter = 0; cacheWriter < dataFileCache.Count(); cacheWriter++)
                                    {
                                        dataFileWriter.WriteLine(dataFileCache[cacheWriter]);
                                    }


                                    dataFileWriter.Close();
                                    dataFileCache.Clear();
                                }

                                if (
                                    myDataContainter.MeasureDataNormalized.Select(t => t.SzenarioTrialNumber)
                                        .Contains(szenarioTrialNumbers[i]))
                                {
                                    List<MeasureDataContainer> measureDataNormalizedList =
                                        myDataContainter.MeasureDataNormalized.Where(
                                            t => t.SzenarioTrialNumber == szenarioTrialNumbers[iVar])
                                            .OrderBy(t => t.TimeStamp)
                                            .ToList();

                                    dataFileCache.AddRange(
                                        measureDataNormalizedList.Select(
                                            t =>
                                                "," + trialId + "," +
                                                t.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss.fffffff") + "," +
                                                DoubleConverter.ToExactString(t.ForceActualX) + "," +
                                                DoubleConverter.ToExactString(t.ForceActualY) + "," +
                                                DoubleConverter.ToExactString(t.ForceActualZ) + "," +
                                                DoubleConverter.ToExactString(t.ForceNominalX) + "," +
                                                DoubleConverter.ToExactString(t.ForceNominalY) + "," +
                                                DoubleConverter.ToExactString(t.ForceNominalZ) + "," +
                                                DoubleConverter.ToExactString(t.ForceMomentX) + "," +
                                                DoubleConverter.ToExactString(t.ForceMomentY) + "," +
                                                DoubleConverter.ToExactString(t.ForceMomentZ) + "," +
                                                DoubleConverter.ToExactString(t.PositionCartesianX) + "," +
                                                DoubleConverter.ToExactString(t.PositionCartesianY) + "," +
                                                DoubleConverter.ToExactString(t.PositionCartesianZ) + "," +
                                                t.PositionStatus));

                                    dataFileStream = new FileStream("C:\\measureDataNormalized.dat", FileMode.Append,
                                        FileAccess.Write);
                                    dataFileWriter = new StreamWriter(dataFileStream);

                                    for (int cacheWriter = 0; cacheWriter < dataFileCache.Count(); cacheWriter++)
                                    {
                                        dataFileWriter.WriteLine(dataFileCache[cacheWriter]);
                                    }


                                    dataFileWriter.Close();
                                    dataFileCache.Clear();
                                }

                                if (
                                    myDataContainter.VelocityDataFiltered.Select(t => t.SzenarioTrialNumber)
                                        .Contains(szenarioTrialNumbers[i]))
                                {
                                    List<VelocityDataContainer> velocityDataFilteredList =
                                        myDataContainter.VelocityDataFiltered.Where(
                                            t => t.SzenarioTrialNumber == szenarioTrialNumbers[iVar])
                                            .OrderBy(t => t.TimeStamp)
                                            .ToList();

                                    dataFileCache.AddRange(
                                        velocityDataFilteredList.Select(
                                            t =>
                                                "," + trialId + "," +
                                                t.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss.fffffff") + "," +
                                                DoubleConverter.ToExactString(t.VelocityX) + "," +
                                                DoubleConverter.ToExactString(t.VelocityY) + "," +
                                                DoubleConverter.ToExactString(t.VelocityZ)));

                                    dataFileStream = new FileStream("C:\\velocityDataFiltered.dat", FileMode.Append,
                                        FileAccess.Write);
                                    dataFileWriter = new StreamWriter(dataFileStream);

                                    for (int cacheWriter = 0; cacheWriter < dataFileCache.Count(); cacheWriter++)
                                    {
                                        dataFileWriter.WriteLine(dataFileCache[cacheWriter]);
                                    }


                                    dataFileWriter.Close();
                                    dataFileCache.Clear();
                                }

                                if (
                                    myDataContainter.VelocityDataNormalized.Select(t => t.SzenarioTrialNumber)
                                        .Contains(szenarioTrialNumbers[i]))
                                {
                                    List<VelocityDataContainer> velocityDataNormalizedList =
                                        myDataContainter.VelocityDataNormalized.Where(
                                            t => t.SzenarioTrialNumber == szenarioTrialNumbers[iVar])
                                            .OrderBy(t => t.TimeStamp)
                                            .ToList();

                                    dataFileCache.AddRange(
                                        velocityDataNormalizedList.Select(
                                            t =>
                                                "," + trialId + "," +
                                                t.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss.fffffff") + "," +
                                                DoubleConverter.ToExactString(t.VelocityX) + "," +
                                                DoubleConverter.ToExactString(t.VelocityY) + "," +
                                                DoubleConverter.ToExactString(t.VelocityZ)));

                                    dataFileStream = new FileStream("C:\\velocityDataNormalized.dat",
                                        FileMode.Append, FileAccess.Write);
                                    dataFileWriter = new StreamWriter(dataFileStream);

                                    for (int cacheWriter = 0; cacheWriter < dataFileCache.Count(); cacheWriter++)
                                    {
                                        dataFileWriter.WriteLine(dataFileCache[cacheWriter]);
                                    }


                                    dataFileWriter.Close();
                                    dataFileCache.Clear();
                                }
                            }

                            _myManipAnalysisGui.WriteProgressInfo("Uploading trial data...");

                            _mySqlWrapper.BulkInsertMeasureDataRaw("C:\\measureDataRaw.dat");
                            File.Delete("C:\\measureDataRaw.dat");

                            if (File.Exists("C:\\measureDataFiltered.dat"))
                            {
                                _mySqlWrapper.BulkInsertMeasureDataFiltered("C:\\measureDataFiltered.dat");
                                File.Delete("C:\\measureDataFiltered.dat");
                            }
                            if (File.Exists("C:\\measureDataNormalized.dat"))
                            {
                                _mySqlWrapper.BulkInsertMeasureDataNormalized("C:\\measureDataNormalized.dat");
                                File.Delete("C:\\measureDataNormalized.dat");
                            }
                            if (File.Exists("C:\\velocityDataFiltered.dat"))
                            {
                                _mySqlWrapper.BulkInsertVelocityDataFiltered("C:\\velocityDataFiltered.dat");
                                File.Delete("C:\\velocityDataFiltered.dat");
                            }
                            if (File.Exists("C:\\velocityDataNormalized.dat"))
                            {
                                _mySqlWrapper.BulkInsertVelocityDataNormalized("C:\\velocityDataNormalized.dat");
                                File.Delete("C:\\velocityDataNormalized.dat");
                            }

                            #endregion

                            #region Upload szenario mean times

                            _myManipAnalysisGui.WriteProgressInfo("Uploading szenario mean-time data...");
                            for (int j = 0; j < myDataContainter.SzenarioMeanTimeData.Count; j++)
                            {
                                int targetId =
                                    _mySqlWrapper.InsertTarget(
                                        myDataContainter.SzenarioMeanTimeData[j].TargetNumber);

                                int szenarioMeanTimeId = _mySqlWrapper.InsertSzenarioMeanTime(
                                    subjectId,
                                    studyId,
                                    groupId,
                                    targetId,
                                    szenarioId,
                                    measureFileId
                                    );

                                _mySqlWrapper.InsertSzenarioMeanTimeData(szenarioMeanTimeId,
                                    myDataContainter.SzenarioMeanTimeData[j]
                                        .MeanTime,
                                    myDataContainter.SzenarioMeanTimeData[j]
                                        .MeanTimeStd);
                            }

                            #endregion

                            #region Upload baselines

                            _myManipAnalysisGui.WriteProgressInfo("Uploading baseline data...");
                            if (myDataContainter.BaselineData != null)
                            {
                                for (int j = 0; j < targetNumbers.Length; j++)
                                {
                                    int jVar = j;
                                    int targetId = _mySqlWrapper.InsertTarget(targetNumbers[j]);

                                    int baselineId = _mySqlWrapper.InsertBaseline(
                                        subjectId,
                                        studyId,
                                        groupId,
                                        targetId,
                                        szenarioId,
                                        measureFileId
                                        );

                                    List<BaselineDataContainer> baselineDataList =
                                        myDataContainter.BaselineData.Where(
                                            t => t.TargetNumber == targetNumbers[jVar])
                                            .OrderBy(t => t.PseudoTimeStamp)
                                            .ToList();

                                    for (int k = 0; k < baselineDataList.Count; k++)
                                    {
                                        _mySqlWrapper.InsertBaselineData(
                                            baselineId,
                                            baselineDataList[k].PseudoTimeStamp,
                                            baselineDataList[k].BaselinePositionCartesianX,
                                            baselineDataList[k].BaselinePositionCartesianY,
                                            baselineDataList[k].BaselinePositionCartesianZ,
                                            baselineDataList[k].BaselineVelocityX,
                                            baselineDataList[k].BaselineVelocityY,
                                            baselineDataList[k].BaselineVelocityZ
                                            );
                                    }
                                }
                            }

                            #endregion
                            */

                            #endregion
                        }
                        else
                        {
                            _myManipAnalysisGui.WriteToLogBox("Error parsing \"" + filename + "\"");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox("Error in \"" + measureFilesList.ElementAt(files) + "\":\n" + ex +
                                                      "\nSkipped file.");
                }
            }
            _myManipAnalysisGui.SetProgressBarValue(0);
            _myManipAnalysisGui.WriteProgressInfo("Ready");
            _myManipAnalysisGui.EnableTabPages(true);

            TaskManager.Remove(Task.CurrentId);
        }

        /*
        private void DuplicateEntryDetection(MongoDb.Trial[] trialsContainer)
        {
            if (trialCoreDistribution.Length > coreVar)
            {
                var threadTrials = new List<int>(trialCoreDistribution[coreVar]);

                for (int i = 0; i < threadTrials.Count(); i++)
                {
                    List<MeasureDataContainer> tempRawData;
                    lock (myDataContainter)
                    {
                        int iVar = i;
                        tempRawData =
                            new List<MeasureDataContainer>(
                                myDataContainter.MeasureDataRaw.Where(
                                    t =>
                                        t.SzenarioTrialNumber ==
                                        threadTrials.ElementAt(iVar))
                                    .OrderBy(t => t.TimeStamp));
                    }

                    int entryCount = tempRawData.Select(t => t.TimeStamp.Ticks).Count();
                    int entryUniqueCount =
                        tempRawData.Select(t => t.TimeStamp.Ticks).Distinct().Count();

                    if (entryCount != entryUniqueCount)
                    {
                        lock (myDataContainter)
                        {
                            int iVar = i;
                            List<MeasureDataContainer> tempList =
                                myDataContainter.MeasureDataRaw.Where(
                                    t =>
                                        t.SzenarioTrialNumber ==
                                        threadTrials.ElementAt(iVar))
                                    .ToList();
                            for (int j = 0; j < tempList.Count; j++)
                            {
                                tempList.ElementAt(j).ContainsDuplicates = true;
                            }
                        }
                    }

                    bool errorDetected;
                    do
                    {
                        errorDetected = false;
                        var diffXyz = new List<double>();
                        for (int j = 0; j < (tempRawData.Count - 1); j++)
                        {
                            diffXyz.Add(Math.Sqrt(
                                Math.Pow(
                                    tempRawData[j].PositionCartesianX -
                                    tempRawData[j + 1].PositionCartesianX, 2) +
                                Math.Pow(
                                    tempRawData[j].PositionCartesianY -
                                    tempRawData[j + 1].PositionCartesianY, 2) +
                                Math.Pow(
                                    tempRawData[j].PositionCartesianZ -
                                    tempRawData[j + 1].PositionCartesianZ, 2))/
                                        tempRawData[j + 1].TimeStamp.Subtract(
                                            tempRawData[j].TimeStamp).TotalSeconds);
                        }

                        while (diffXyz.Remove(double.PositiveInfinity) | diffXyz.Remove(double.NegativeInfinity))
                        {
                        }

                        int maxIndex = diffXyz.IndexOf(diffXyz.Max());

                        if (
                            Math.Abs(diffXyz.ElementAt(maxIndex) -
                                     diffXyz.ElementAt(maxIndex - 1)) > 3)
                        {
                            MeasureDataContainer errorEntry =
                                tempRawData.ElementAt(maxIndex + 1);
                            _myManipAnalysisGui.WriteToLogBox(
                                "Fixed error at time-stamp \"" +
                                errorEntry.TimeStamp.ToString(
                                    "hh:mm:ss.fffffff") + "\" in file \"" +
                                filename + "\"");
                            tempRawData.RemoveAt(maxIndex + 1);
                            errorDetected = true;
                        }
                    } while (errorDetected);
                }
            }
        }
         * */

        private void ButterWorthFilter(Trial[] trialsContainer)
        {
            for (int trialCounter = 0; trialCounter < trialsContainer.Length; trialCounter++)
            {
                _myMatlabWrapper.SetWorkspaceData("force_actual_x",
                    trialsContainer[trialCounter].MeasuredForcesRaw.Select(t => t.X).ToArray());
                _myMatlabWrapper.SetWorkspaceData("force_actual_y",
                    trialsContainer[trialCounter].MeasuredForcesRaw.Select(t => t.Y).ToArray());
                _myMatlabWrapper.SetWorkspaceData("force_actual_z",
                    trialsContainer[trialCounter].MeasuredForcesRaw.Select(t => t.Z).ToArray());

                if (trialsContainer[trialCounter].NominalForcesRaw != null)
                {
                    _myMatlabWrapper.SetWorkspaceData("force_nominal_x",
                        trialsContainer[trialCounter].NominalForcesRaw.Select(t => t.X).ToArray());
                    _myMatlabWrapper.SetWorkspaceData("force_nominal_y",
                        trialsContainer[trialCounter].NominalForcesRaw.Select(t => t.Y).ToArray());
                    _myMatlabWrapper.SetWorkspaceData("force_nominal_z",
                        trialsContainer[trialCounter].NominalForcesRaw.Select(t => t.Z).ToArray());
                }

                _myMatlabWrapper.SetWorkspaceData("force_moment_x",
                    trialsContainer[trialCounter].MomentForcesRaw.Select(t => t.X).ToArray());
                _myMatlabWrapper.SetWorkspaceData("force_moment_y",
                    trialsContainer[trialCounter].MomentForcesRaw.Select(t => t.Y).ToArray());
                _myMatlabWrapper.SetWorkspaceData("force_moment_z",
                    trialsContainer[trialCounter].MomentForcesRaw.Select(t => t.Z).ToArray());

                _myMatlabWrapper.SetWorkspaceData("position_cartesian_x",
                    trialsContainer[trialCounter].PositionRaw.Select(t => t.X).ToArray());
                _myMatlabWrapper.SetWorkspaceData("position_cartesian_y",
                    trialsContainer[trialCounter].PositionRaw.Select(t => t.Y).ToArray());
                _myMatlabWrapper.SetWorkspaceData("position_cartesian_z",
                    trialsContainer[trialCounter].PositionRaw.Select(t => t.Z).ToArray());

                _myMatlabWrapper.Execute("force_actual_x = filtfilt(bForce, aForce, force_actual_x);");
                _myMatlabWrapper.Execute("force_actual_y = filtfilt(bForce, aForce, force_actual_y);");
                _myMatlabWrapper.Execute("force_actual_z = filtfilt(bForce, aForce, force_actual_z);");

                _myMatlabWrapper.Execute("force_nominal_x = filtfilt(bForce, aForce,force_nominal_x);");
                _myMatlabWrapper.Execute("force_nominal_y = filtfilt(bForce, aForce,force_nominal_y);");
                _myMatlabWrapper.Execute("force_nominal_z = filtfilt(bForce, aForce,force_nominal_z);");

                _myMatlabWrapper.Execute("force_moment_x = filtfilt(bForce, aForce, force_moment_x);");
                _myMatlabWrapper.Execute("force_moment_y = filtfilt(bForce, aForce, force_moment_y);");
                _myMatlabWrapper.Execute("force_moment_z = filtfilt(bForce, aForce, force_moment_z);");

                _myMatlabWrapper.Execute("position_cartesian_x = filtfilt(bPosition, aPosition, position_cartesian_x);");
                _myMatlabWrapper.Execute("position_cartesian_y = filtfilt(bPosition, aPosition, position_cartesian_y);");
                _myMatlabWrapper.Execute("position_cartesian_z = filtfilt(bPosition, aPosition, position_cartesian_z);");


                double[,] forceActualX =
                    _myMatlabWrapper.GetWorkspaceData("force_actual_x");
                double[,] forceActualY =
                    _myMatlabWrapper.GetWorkspaceData("force_actual_y");
                double[,] forceActualZ =
                    _myMatlabWrapper.GetWorkspaceData("force_actual_z");

                double[,] forceNominalX =
                    _myMatlabWrapper.GetWorkspaceData("force_nominal_x");
                double[,] forceNominalY =
                    _myMatlabWrapper.GetWorkspaceData("force_nominal_y");
                double[,] forceNominalZ =
                    _myMatlabWrapper.GetWorkspaceData("force_nominal_z");

                double[,] forceMomentX =
                    _myMatlabWrapper.GetWorkspaceData("force_moment_x");
                double[,] forceMomentY =
                    _myMatlabWrapper.GetWorkspaceData("force_moment_y");
                double[,] forceMomentZ =
                    _myMatlabWrapper.GetWorkspaceData("force_moment_z");

                double[,] positionCartesianX =
                    _myMatlabWrapper.GetWorkspaceData("position_cartesian_x");
                double[,] positionCartesianY =
                    _myMatlabWrapper.GetWorkspaceData("position_cartesian_y");
                double[,] positionCartesianZ =
                    _myMatlabWrapper.GetWorkspaceData("position_cartesian_z");


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
                    var nominalForcesFiltered = new ForceContainer();
                    var momentForcesFiltered = new ForceContainer();
                    var positionFiltered = new PositionContainer();

                    measuredForcesFiltered.PositionStatus =
                        trialsContainer[trialCounter].MeasuredForcesRaw[frameCount].PositionStatus;
                    measuredForcesFiltered.TimeStamp = trialsContainer[trialCounter].MeasuredForcesRaw[frameCount].TimeStamp;
                    measuredForcesFiltered.X = forceActualX[0, frameCount];
                    measuredForcesFiltered.Y = forceActualY[0, frameCount];
                    measuredForcesFiltered.Z = forceActualZ[0, frameCount];

                    if (trialsContainer[trialCounter].NominalForcesRaw != null)
                    {
                        nominalForcesFiltered.PositionStatus =
                            trialsContainer[trialCounter].NominalForcesRaw[frameCount].PositionStatus;
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

                    trialsContainer[trialCounter].TrialInformation.FilteredDataSampleRate =
                        trialsContainer[trialCounter].TrialInformation.RawDataSampleRate;
                }

                _myMatlabWrapper.ClearWorkspaceData("force_actual_x");
                _myMatlabWrapper.ClearWorkspaceData("force_actual_y");
                _myMatlabWrapper.ClearWorkspaceData("force_actual_z");

                _myMatlabWrapper.ClearWorkspaceData("force_nominal_x");
                _myMatlabWrapper.ClearWorkspaceData("force_nominal_y");
                _myMatlabWrapper.ClearWorkspaceData("force_nominal_z");

                _myMatlabWrapper.ClearWorkspaceData("force_moment_x");
                _myMatlabWrapper.ClearWorkspaceData("force_moment_y");
                _myMatlabWrapper.ClearWorkspaceData("force_moment_z");

                _myMatlabWrapper.ClearWorkspaceData("position_cartesian_x");
                _myMatlabWrapper.ClearWorkspaceData("position_cartesian_y");
                _myMatlabWrapper.ClearWorkspaceData("position_cartesian_z");
            }
        }


        private void VelocityCalculation(Trial[] trialsContainer, int samplesPerSecond)
        {
            for (int trialCounter = 0; trialCounter < trialsContainer.Length; trialCounter++)
            {
                _myMatlabWrapper.SetWorkspaceData("position_cartesian_x",
                    trialsContainer[trialCounter].PositionFiltered.Select(t => t.X).ToArray());
                _myMatlabWrapper.SetWorkspaceData("position_cartesian_y",
                    trialsContainer[trialCounter].PositionFiltered.Select(t => t.Y).ToArray());
                _myMatlabWrapper.SetWorkspaceData("position_cartesian_z",
                    trialsContainer[trialCounter].PositionFiltered.Select(t => t.Z).ToArray());

                _myMatlabWrapper.SetWorkspaceData("sampleRate", samplesPerSecond);

                _myMatlabWrapper.Execute("velocity_x = numDiff(position_cartesian_x, sampleRate;");
                _myMatlabWrapper.Execute("velocity_y = numDiff(position_cartesian_y, sampleRate;");
                _myMatlabWrapper.Execute("velocity_z = numDiff(position_cartesian_z, sampleRate;");

                double[,] velocityX = _myMatlabWrapper.GetWorkspaceData("velocity_x");
                double[,] velocityY = _myMatlabWrapper.GetWorkspaceData("velocity_y");
                double[,] velocityZ = _myMatlabWrapper.GetWorkspaceData("velocity_z");

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

                _myMatlabWrapper.ClearWorkspace();
            }
        }

        private void TimeNormalization(Trial[] trialsContainer, int timeNormalizationSamples, int percentPeakVelocity)
        {
            _myMatlabWrapper.SetWorkspaceData("newSampleRate",
                Convert.ToDouble(
                    timeNormalizationSamples));

            for (int trialCounter = 0; trialCounter < trialsContainer.Length; trialCounter++)
            {
                trialsContainer[trialCounter].TrialInformation.NormalizedDataSampleRate = timeNormalizationSamples;
                trialsContainer[trialCounter].TrialInformation.VelocityTrimThresholdPercent = percentPeakVelocity;
                trialsContainer[trialCounter].TrialInformation.VelocityTrimThresholdPercent = percentPeakVelocity;
                trialsContainer[trialCounter].TrialInformation.VelocityTrimThresholdForTrial =
                    trialsContainer[trialCounter].VelocityFiltered.Max(
                        t => Math.Sqrt(Math.Pow(t.X, 2) + Math.Pow(t.Y, 2) + Math.Pow(t.Z, 2)))/100.0*
                    percentPeakVelocity;

                var startTime = new DateTime(0);
                var stopTime = new DateTime(0);
                try
                {
                    // First element with PositionStatus == 0 and a velocity higher than the threshold
                    startTime =
                        trialsContainer[trialCounter].VelocityFiltered.Where(t => t.PositionStatus == 0)
                            .First(
                                t =>
                                    Math.Sqrt(Math.Pow(t.X, 2) + Math.Pow(t.Y, 2) + Math.Pow(t.Z, 2)) >
                                    trialsContainer[trialCounter].TrialInformation.VelocityTrimThresholdForTrial)
                            .TimeStamp;
                }
                catch
                {
                    // First element with PositionStatus == 1
                    startTime = trialsContainer[trialCounter].VelocityFiltered.First(t => t.PositionStatus == 1).TimeStamp;
                }


                try
                {
                    // First element with PositionStatus == 2 and a velocity lower than the threshold
                    stopTime = trialsContainer[trialCounter].VelocityFiltered.Where(t => t.PositionStatus == 2)
                        .First(
                            t =>
                                Math.Sqrt(Math.Pow(t.X, 2) + Math.Pow(t.Y, 2) + Math.Pow(t.Z, 2)) <
                                trialsContainer[trialCounter].TrialInformation.VelocityTrimThresholdForTrial)
                        .TimeStamp;
                }
                catch
                {
                    // Last element with PositionStatus == 2
                    stopTime = trialsContainer[trialCounter].VelocityFiltered.Last().TimeStamp;
                }

                IEnumerable<ForceContainer> measuredForcesFilteredCut =
                    trialsContainer[trialCounter].MeasuredForcesFiltered.Where(
                        t => t.TimeStamp > startTime && t.TimeStamp < stopTime).OrderBy(t => t.TimeStamp);

                IEnumerable<ForceContainer> momentForcesFilteredCut =
                    trialsContainer[trialCounter].MomentForcesFiltered.Where(
                        t => t.TimeStamp > startTime && t.TimeStamp < stopTime).OrderBy(t => t.TimeStamp);

                IEnumerable<ForceContainer> nominalForcesFilteredCut = null;
                if (trialsContainer[trialCounter].NominalForcesFiltered != null)
                {
                    nominalForcesFilteredCut =
                        trialsContainer[trialCounter].NominalForcesFiltered.Where(
                            t => t.TimeStamp > startTime && t.TimeStamp < stopTime).OrderBy(t => t.TimeStamp);
                }

                IEnumerable<PositionContainer> positionFilteredCut =
                    trialsContainer[trialCounter].PositionFiltered.Where(
                        t => t.TimeStamp > startTime && t.TimeStamp < stopTime).OrderBy(t => t.TimeStamp);

                IEnumerable<VelocityContainer> velocityFilteredCut =
                    trialsContainer[trialCounter].VelocityFiltered.Where(
                        t => t.TimeStamp > startTime && t.TimeStamp < stopTime).OrderBy(t => t.TimeStamp);


                _myMatlabWrapper.SetWorkspaceData("measure_data_time",
                    velocityFilteredCut.Select(t => Convert.ToDouble(t.TimeStamp.Ticks)).ToArray());

                _myMatlabWrapper.SetWorkspaceData("forceActualX", measuredForcesFilteredCut.Select(t => t.X).ToArray());
                _myMatlabWrapper.SetWorkspaceData("forceActualY", measuredForcesFilteredCut.Select(t => t.Y).ToArray());
                _myMatlabWrapper.SetWorkspaceData("forceActualZ", measuredForcesFilteredCut.Select(t => t.Z).ToArray());

                if (nominalForcesFilteredCut != null)
                {
                    _myMatlabWrapper.SetWorkspaceData("forceNominalX", nominalForcesFilteredCut.Select(t => t.X).ToArray());
                    _myMatlabWrapper.SetWorkspaceData("forceNominalY", nominalForcesFilteredCut.Select(t => t.Y).ToArray());
                    _myMatlabWrapper.SetWorkspaceData("forceNominalY", nominalForcesFilteredCut.Select(t => t.Z).ToArray());
                }

                _myMatlabWrapper.SetWorkspaceData("forceMomentX", momentForcesFilteredCut.Select(t => t.X).ToArray());
                _myMatlabWrapper.SetWorkspaceData("forceMomentY", momentForcesFilteredCut.Select(t => t.Y).ToArray());
                _myMatlabWrapper.SetWorkspaceData("forceMomentZ", momentForcesFilteredCut.Select(t => t.Z).ToArray());

                _myMatlabWrapper.SetWorkspaceData("positionCartesianX", positionFilteredCut.Select(t => t.X).ToArray());
                _myMatlabWrapper.SetWorkspaceData("positionCartesianY", positionFilteredCut.Select(t => t.Y).ToArray());
                _myMatlabWrapper.SetWorkspaceData("positionCartesianZ", positionFilteredCut.Select(t => t.Z).ToArray());

                _myMatlabWrapper.SetWorkspaceData("velocityX", velocityFilteredCut.Select(t => t.X).ToArray());
                _myMatlabWrapper.SetWorkspaceData("velocityY", velocityFilteredCut.Select(t => t.Y).ToArray());
                _myMatlabWrapper.SetWorkspaceData("velocityZ", velocityFilteredCut.Select(t => t.Z).ToArray());

                _myMatlabWrapper.SetWorkspaceData("positionStatus", velocityFilteredCut.Select(t => t.PositionStatus).ToArray());

                var errorList = new List<string>();

                _myMatlabWrapper.Execute(
                    "[errorvar1, forceActualX, newMeasureTime] = timeNorm(forceActualX, measure_data_time, newSampleRate);");
                _myMatlabWrapper.Execute(
                    "[errorvar2, forceActualY, newMeasureTime] = timeNorm(forceActualY, measure_data_time, newSampleRate);");
                _myMatlabWrapper.Execute(
                    "[errorvar3, forceActualZ, newMeasureTime] = timeNorm(forceActualZ, measure_data_time, newSampleRate);");

                if (nominalForcesFilteredCut != null)
                {
                    _myMatlabWrapper.Execute(
                        "[errorvar4, forceNominalX, newMeasureTime] = timeNorm(forceNominalX, measure_data_time, newSampleRate);");
                    _myMatlabWrapper.Execute(
                        "[errorvar5, forceNominalY, newMeasureTime] = timeNorm(forceNominalY, measure_data_time, newSampleRate);");
                    _myMatlabWrapper.Execute(
                        "[errorvar6, forceNominalZ, newMeasureTime] = timeNorm(forceNominalZ, measure_data_time, newSampleRate);");
                }

                _myMatlabWrapper.Execute(
                    "[errorvar7, forceMomentX, newMeasureTime] = timeNorm(forceMomentX, measure_data_time, newSampleRate);");
                _myMatlabWrapper.Execute(
                    "[errorvar8, forceMomentY, newMeasureTime] = timeNorm(forceMomentY, measure_data_time, newSampleRate);");
                _myMatlabWrapper.Execute(
                    "[errorvar9, forceMomentZ, newMeasureTime] = timeNorm(forceMomentZ, measure_data_time, newSampleRate);");

                _myMatlabWrapper.Execute(
                    "[errorvar10, positionCartesianX, newMeasureTime] = timeNorm(positionCartesianX, measure_data_time, newSampleRate);");
                _myMatlabWrapper.Execute(
                    "[errorvar11, positionCartesianY, newMeasureTime] = timeNorm(positionCartesianY, measure_data_time, newSampleRate);");
                _myMatlabWrapper.Execute(
                    "[errorvar12, positionCartesianZ, newMeasureTime] = timeNorm(positionCartesianZ, measure_data_time, newSampleRate);");

                _myMatlabWrapper.Execute(
                    "[errorvar13, velocityX, newMeasureTime] = timeNorm(velocityX, measure_data_time, newSampleRate);");
                _myMatlabWrapper.Execute(
                    "[errorvar14, velocityY, newMeasureTime] = timeNorm(velocityY, measure_data_time, newSampleRate);");
                _myMatlabWrapper.Execute(
                    "[errorvar15, velocityZ, newMeasureTime] = timeNorm(velocityZ, measure_data_time, newSampleRate);");

                _myMatlabWrapper.Execute(
                    "[errorvar16, positionStatus, newMeasureTime] = timeNorm(positionStatus, measure_data_time, newSampleRate);");


                for (int errorVarCounterCounter = 1; errorVarCounterCounter <= 16; errorVarCounterCounter++)
                {
                    if (nominalForcesFilteredCut == null && errorVarCounterCounter >= 4 && errorVarCounterCounter <= 6)
                    {
                    }
                    else
                    {
                        errorList.Add(Convert.ToString(_myMatlabWrapper.GetWorkspaceData("errorvar" + errorVarCounterCounter)));
                    }
                }

                if (errorList.Any(t => !string.IsNullOrEmpty(t)))
                {
                    string output =
                        errorList.Where(t => !string.IsNullOrEmpty(t))
                            .Select(
                                t =>
                                    t + " in " + trialsContainer[trialCounter].MeasureFile.FileName + " at szenario-trial-number " +
                                    trialsContainer[trialCounter].TrialNumberInSzenario)
                            .Aggregate("", (current, line) => current + line);
                    _myManipAnalysisGui.WriteToLogBox(output);
                }

                double[,] measureDataTime =
                    _myMatlabWrapper.GetWorkspaceData("newMeasureTime");
                double[,] velocityDataTime =
                    _myMatlabWrapper.GetWorkspaceData("newVelocityTime");

                double[,] forceActualX =
                    _myMatlabWrapper.GetWorkspaceData("forceActualX");
                double[,] forceActualY =
                    _myMatlabWrapper.GetWorkspaceData("forceActualY");
                double[,] forceActualZ =
                    _myMatlabWrapper.GetWorkspaceData("forceActualZ");

                double[,] forceNominalX = null;
                double[,] forceNominalY = null;
                double[,] forceNominalZ = null;
                if (nominalForcesFilteredCut != null)
                {
                    forceNominalX =
                        _myMatlabWrapper.GetWorkspaceData("forceNominalX");
                    forceNominalY =
                        _myMatlabWrapper.GetWorkspaceData("forceNominalY");
                    forceNominalZ =
                        _myMatlabWrapper.GetWorkspaceData("forceNominalZ");
                }
                double[,] forceMomentX =
                    _myMatlabWrapper.GetWorkspaceData("forceMomentX");
                double[,] forceMomentY =
                    _myMatlabWrapper.GetWorkspaceData("forceMomentY");
                double[,] forceMomentZ =
                    _myMatlabWrapper.GetWorkspaceData("forceMomentZ");

                double[,] positionCartesianX =
                    _myMatlabWrapper.GetWorkspaceData("positionCartesianX");
                double[,] positionCartesianY =
                    _myMatlabWrapper.GetWorkspaceData("positionCartesianY");
                double[,] positionCartesianZ =
                    _myMatlabWrapper.GetWorkspaceData("positionCartesianZ");

                double[,] positionStatus =
                    _myMatlabWrapper.GetWorkspaceData("positionStatus");

                double[,] velocityX =
                    _myMatlabWrapper.GetWorkspaceData("velocityX");
                double[,] velocityY =
                    _myMatlabWrapper.GetWorkspaceData("velocityY");
                double[,] velocityZ =
                    _myMatlabWrapper.GetWorkspaceData("velocityZ");

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

                _myMatlabWrapper.ClearWorkspace();
            }
        }

        private Baseline[] CalculateBaselines(Trial[] trialsContainer)
        {
            int minTargetNumber = trialsContainer.Min(t => t.Target.Number);
            int maxTargetNumber = trialsContainer.Max(t => t.Target.Number);

            var baselines = new List<Baseline>();
            for (int targetCounter = minTargetNumber; targetCounter <= maxTargetNumber; targetCounter++)
            {
                // Get Target-trial 2-MAX (6)
                List<Trial> baselineTrials =
                    trialsContainer.Where(t => t.Target.Number == targetCounter && t.TargetTrialNumberInSzenario > 1).ToList();

                List<double[]> measuredForcesX =
                    baselineTrials.Select(t => t.MeasuredForcesNormalized.Select(u => u.X).ToArray()).ToList();
                List<double[]> measuredForcesY =
                    baselineTrials.Select(t => t.MeasuredForcesNormalized.Select(u => u.Y).ToArray()).ToList();
                List<double[]> measuredForcesZ =
                    baselineTrials.Select(t => t.MeasuredForcesNormalized.Select(u => u.Z).ToArray()).ToList();

                List<double[]> nominalForcesX = null;
                List<double[]> nominalForcesY = null;
                List<double[]> nominalForcesZ = null;
                if (baselineTrials[0].NominalForcesNormalized != null)
                {
                    nominalForcesX = baselineTrials.Select(t => t.NominalForcesNormalized.Select(u => u.X).ToArray()).ToList();
                    nominalForcesY = baselineTrials.Select(t => t.NominalForcesNormalized.Select(u => u.Y).ToArray()).ToList();
                    nominalForcesZ = baselineTrials.Select(t => t.NominalForcesNormalized.Select(u => u.Z).ToArray()).ToList();
                }

                List<double[]> momentForcesX =
                    baselineTrials.Select(t => t.MomentForcesNormalized.Select(u => u.X).ToArray()).ToList();
                List<double[]> momentForcesY =
                    baselineTrials.Select(t => t.MomentForcesNormalized.Select(u => u.Y).ToArray()).ToList();
                List<double[]> momentForcesZ =
                    baselineTrials.Select(t => t.MomentForcesNormalized.Select(u => u.Z).ToArray()).ToList();

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
                    baselineTimeStamps[timeSample] = new DateTime(0);
                    baselineTimeStamps[timeSample] =
                        baselineTimeStamps[timeSample].AddSeconds((1.0/baselineTrials[0].TrialInformation.NormalizedDataSampleRate)*
                                                                  timeSample);
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
            return baselines.ToArray();
        }

        private SzenarioMeanTime[] CalculateSzenarioMeanTimes(Trial[] trialsContainer)
        {
            int minTargetNumber = trialsContainer.Min(t => t.Target.Number);
            int maxTargetNumber = trialsContainer.Max(t => t.Target.Number);

            var szenarioMeanTimes = new List<SzenarioMeanTime>();

            for (int targetCounter = minTargetNumber; targetCounter <= maxTargetNumber; targetCounter++)
            {
                var tempSzenarioMeanTime = new SzenarioMeanTime();
                var targetContainer = new TargetContainer();

                targetContainer.Number = targetCounter;

                tempSzenarioMeanTime.Group = trialsContainer[0].Group;
                tempSzenarioMeanTime.MeasureFile = trialsContainer[0].MeasureFile;
                tempSzenarioMeanTime.Study = trialsContainer[0].Study;
                tempSzenarioMeanTime.Subject = trialsContainer[0].Subject;
                tempSzenarioMeanTime.Szenario = trialsContainer[0].Szenario;
                tempSzenarioMeanTime.Target = targetContainer;

                long[] targetDurationTimes = trialsContainer.Where(t => t.Target.Number == targetCounter)
                    .Select(
                        t => t.PositionNormalized.Max(u => u.TimeStamp.Ticks) - t.PositionNormalized.Min(u => u.TimeStamp.Ticks))
                    .ToArray();

                tempSzenarioMeanTime.MeanTime = new TimeSpan(Convert.ToInt64(targetDurationTimes.Average()));
                tempSzenarioMeanTime.MeanTimeStd = new TimeSpan(Convert.ToInt64(targetDurationTimes.StdDev()));

                szenarioMeanTimes.Add(tempSzenarioMeanTime);
            }

            return szenarioMeanTimes.ToArray();
        }

        public void CalculateStatistics()
        {
            var newTask = new Task(CalculateStatisticsThread);
            TaskManager.PushBack(newTask);
            newTask.Start();
        }

        private void CalculateStatisticsThread()
        {
            while (TaskManager.GetIndex(Task.CurrentId) != 0 & !TaskManager.Cancel)
            {
                Thread.Sleep(100);
            }

            _myManipAnalysisGui.EnableTabPages(false);
            _myManipAnalysisGui.WriteProgressInfo("Calculating statistics...");

            List<int[]> trialInfos = _myDatabaseWrapper.GetStatisticCalculationInformation();
            /*
            List<int[]> trialInfos = _mySqlWrapper.GetStatisticUpdateInformation();
            */
            if (trialInfos != null)
            {
                int counter = 1;

                foreach (var trialInfo in trialInfos)
                {
                    if (TaskManager.Cancel)
                    {
                        break;
                    }
                    while (TaskManager.Pause & !TaskManager.Cancel)
                    {
                        Thread.Sleep(100);
                    }

                    _myManipAnalysisGui.SetProgressBarValue((100.0/trialInfos.Count())*counter);
                    counter++;

                    DataSet measureDataSet = _myDatabaseWrapper.GetMeasureDataNormalizedDataSet(trialInfo[0]);
                    DataSet velocityDataSet = _myDatabaseWrapper.GetVelocityDataNormalizedDataSet(trialInfo[0]);
                    DataSet baselineDataSet = _myDatabaseWrapper.GetBaselineDataSet(trialInfo[1], trialInfo[2], trialInfo[3],
                        trialInfo[4]);
                    /*
                    DataSet statisticDataSet = _mySqlWrapper.GetStatisticDataSet(trialInfo[0]);
                    */

                    int targetNumber = trialInfo[5];

                    if (baselineDataSet.Tables[0].Rows.Count > 0)
                    {
                        if ((measureDataSet.Tables[0].Rows.Count == velocityDataSet.Tables[0].Rows.Count) &&
                            (velocityDataSet.Tables[0].Rows.Count == baselineDataSet.Tables[0].Rows.Count))
                        {
                            try
                            {
                                int sampleCount = measureDataSet.Tables[0].Rows.Count;

                                var measureData = new double[sampleCount, 3];
                                var velocityData = new double[sampleCount, 3];
                                var baselineData = new double[sampleCount, 6];
                                var timeStamp = new double[sampleCount];

                                for (int i = 0; i < sampleCount; i++)
                                {
                                    timeStamp[i] =
                                        Convert.ToDateTime(measureDataSet.Tables[0].Rows[i]["time_stamp"]).Ticks;

                                    measureData[i, 0] =
                                        Convert.ToDouble(measureDataSet.Tables[0].Rows[i]["position_cartesian_x"]);
                                    measureData[i, 1] =
                                        Convert.ToDouble(measureDataSet.Tables[0].Rows[i]["position_cartesian_y"]);
                                    measureData[i, 2] =
                                        Convert.ToDouble(measureDataSet.Tables[0].Rows[i]["position_cartesian_z"]);

                                    velocityData[i, 0] =
                                        Convert.ToDouble(velocityDataSet.Tables[0].Rows[i]["velocity_x"]);
                                    velocityData[i, 1] =
                                        Convert.ToDouble(velocityDataSet.Tables[0].Rows[i]["velocity_y"]);
                                    velocityData[i, 2] =
                                        Convert.ToDouble(velocityDataSet.Tables[0].Rows[i]["velocity_z"]);

                                    baselineData[i, 0] =
                                        Convert.ToDouble(
                                            baselineDataSet.Tables[0].Rows[i]["baseline_position_cartesian_x"]);
                                    baselineData[i, 1] =
                                        Convert.ToDouble(
                                            baselineDataSet.Tables[0].Rows[i]["baseline_position_cartesian_y"]);
                                    baselineData[i, 2] =
                                        Convert.ToDouble(
                                            baselineDataSet.Tables[0].Rows[i]["baseline_position_cartesian_z"]);
                                    baselineData[i, 3] =
                                        Convert.ToDouble(baselineDataSet.Tables[0].Rows[i]["baseline_velocity_x"]);
                                    baselineData[i, 4] =
                                        Convert.ToDouble(baselineDataSet.Tables[0].Rows[i]["baseline_velocity_y"]);
                                    baselineData[i, 5] =
                                        Convert.ToDouble(baselineDataSet.Tables[0].Rows[i]["baseline_velocity_z"]);
                                }

                                List<double> tempTimeList = timeStamp.ToList();
                                int time300MsIndex =
                                    tempTimeList.IndexOf(
                                        tempTimeList.OrderBy(
                                            d => Math.Abs(d - (timeStamp[0] + TimeSpan.FromMilliseconds(300).Ticks)))
                                            .ElementAt(0));

                                _myMatlabWrapper.SetWorkspaceData("targetNumber", targetNumber);
                                _myMatlabWrapper.SetWorkspaceData("time300msIndex", time300MsIndex);
                                _myMatlabWrapper.SetWorkspaceData("measureData", measureData);
                                _myMatlabWrapper.SetWorkspaceData("velocityData", velocityData);
                                _myMatlabWrapper.SetWorkspaceData("baselineData", baselineData);

                                _myMatlabWrapper.Execute(
                                    "vector_correlation = vectorCorrelation([velocityData(:,1) velocityData(:,3)],[baselineData(:,4) baselineData(:,6)]);");
                                _myMatlabWrapper.Execute(
                                    "enclosed_area = enclosedArea(measureData(:,1),measureData(:,3));");
                                _myMatlabWrapper.Execute(
                                    "length_abs = trajectLength(measureData(:,1),measureData(:,3));");
                                _myMatlabWrapper.Execute(
                                    "length_ratio = trajectLength(measureData(:,1),measureData(:,3)) / trajectLength(baselineData(:,1),baselineData(:,3));");
                                _myMatlabWrapper.Execute(
                                    "distanceAbs = distance2curveAbs([measureData(:,1),measureData(:,3)],targetNumber);");
                                _myMatlabWrapper.Execute(
                                    "distanceSign = distance2curveSign([measureData(:,1),measureData(:,3)],targetNumber);");
                                _myMatlabWrapper.Execute("distance300msAbs = distanceAbs(time300msIndex);");
                                _myMatlabWrapper.Execute("distance300msSign = distanceSign(time300msIndex);");
                                _myMatlabWrapper.Execute("meanDistanceAbs = mean(distanceAbs);");
                                _myMatlabWrapper.Execute("maxDistanceAbs = max(distanceAbs);");
                                _myMatlabWrapper.Execute("[~, posDistanceSign] = max(abs(distanceSign));");
                                _myMatlabWrapper.Execute("maxDistanceSign = distanceSign(posDistanceSign);");
                                _myMatlabWrapper.Execute(
                                    "rmse = rootMeanSquareError([measureData(:,1) measureData(:,3)], [baselineData(:,1) baselineData(:,3)]);");

                                double vectorCorrelation = _myMatlabWrapper.GetWorkspaceData("vector_correlation");
                                double enclosedArea = _myMatlabWrapper.GetWorkspaceData("enclosed_area");
                                double lengthAbs = _myMatlabWrapper.GetWorkspaceData("length_abs");
                                double lengthRatio = _myMatlabWrapper.GetWorkspaceData("length_ratio");
                                double distance300MsAbs = _myMatlabWrapper.GetWorkspaceData("distance300msAbs");
                                double distance300MsSign = _myMatlabWrapper.GetWorkspaceData("distance300msSign");
                                double meanDistanceAbs = _myMatlabWrapper.GetWorkspaceData("meanDistanceAbs");
                                double maxDistanceAbs = _myMatlabWrapper.GetWorkspaceData("maxDistanceAbs");
                                double maxDistanceSign = _myMatlabWrapper.GetWorkspaceData("maxDistanceSign");
                                double rmse = _myMatlabWrapper.GetWorkspaceData("rmse");


                                _myDatabaseWrapper.InsertStatisticData(trialInfo[0], vectorCorrelation, lengthAbs,
                                    lengthRatio, distance300MsAbs, maxDistanceAbs,
                                    meanDistanceAbs, distance300MsSign, maxDistanceSign,
                                    enclosedArea, rmse);


                                /*
                                _mySqlWrapper.UpdateStatisticData(trialInfo[0], 
                                    Convert.ToDouble(statisticDataSet.Tables[0].Rows[0]["velocity_vector_correlation"]),
                                    Convert.ToDouble(statisticDataSet.Tables[0].Rows[0]["trajectory_length_abs"]),
                                    Convert.ToDouble(statisticDataSet.Tables[0].Rows[0]["trajectory_length_ratio_baseline"]),
                                    Convert.ToDouble(statisticDataSet.Tables[0].Rows[0]["perpendicular_displacement_300ms_abs"]),
                                    Convert.ToDouble(statisticDataSet.Tables[0].Rows[0]["maximal_perpendicular_displacement_abs"]),
                                    Convert.ToDouble(statisticDataSet.Tables[0].Rows[0]["mean_perpendicular_displacement_abs"]),
                                    Convert.ToDouble(statisticDataSet.Tables[0].Rows[0]["perpendicular_displacement_300ms_sign"]),
                                    Convert.ToDouble(statisticDataSet.Tables[0].Rows[0]["maximal_perpendicular_displacement_sign"]),
                                    Convert.ToDouble(statisticDataSet.Tables[0].Rows[0]["enclosed_area"]),
                                    Convert.ToDouble(statisticDataSet.Tables[0].Rows[0]["rmse"]));
                                */
                            }
                            catch (Exception statisticException)
                            {
                                _myManipAnalysisGui.WriteToLogBox("Error in Statistic calculation: " +
                                                                  statisticException);
                            }
                        }
                        else
                        {
                            _myManipAnalysisGui.WriteToLogBox("TrialID: " + trialInfo[0] + " - Data not normalised!");
                        }
                        _myMatlabWrapper.ClearWorkspace();
                    }
                    else
                    {
                        _myManipAnalysisGui.WriteToLogBox("TrialID: " + trialInfo[0] + " - No matching baseline found!");
                    }
                }
            }
            else
            {
                _myManipAnalysisGui.WriteToLogBox("Statistics already calculated!");
            }
            _myManipAnalysisGui.SetProgressBarValue(0);
            _myManipAnalysisGui.WriteProgressInfo("Ready");
            _myManipAnalysisGui.EnableTabPages(true);

            TaskManager.Remove(Task.CurrentId);
        }

        public void FixBrokenTrials()
        {
            var newTask = new Task(FixBrokenTrialsThread);
            TaskManager.PushBack(newTask);
            newTask.Start();
        }

        private void FixBrokenTrialsThread()
        {
            while (TaskManager.GetIndex(Task.CurrentId) != 0 & !TaskManager.Cancel)
            {
                Thread.Sleep(100);
            }

            _myManipAnalysisGui.EnableTabPages(false);
            _myManipAnalysisGui.WriteProgressInfo("Fixing broken Trials...");

            List<object[]> faultyTrialInformation = _myDatabaseWrapper.GetFaultyTrialInformation();

            if (faultyTrialInformation != null)
            {
                if (faultyTrialInformation.Count == 0)
                {
                    _myManipAnalysisGui.WriteToLogBox("Trials already fixed!");
                }
                else
                {
                    for (int trialIdCounter = 0;
                        trialIdCounter < faultyTrialInformation.Count & !TaskManager.Cancel;
                        trialIdCounter++)
                    {
                        while (TaskManager.Pause & !TaskManager.Cancel)
                        {
                            Thread.Sleep(100);
                        }
                        _myManipAnalysisGui.SetProgressBarValue((100.0/faultyTrialInformation.Count)*trialIdCounter);

                        int[] trialFixInformation =
                            _myDatabaseWrapper.GetFaultyTrialFixInformation(
                                Convert.ToInt32(faultyTrialInformation[trialIdCounter][1]),
                                Convert.ToInt32(faultyTrialInformation[trialIdCounter][7]));
                        if (trialFixInformation != null)
                        {
                            DataSet upperStatisticDataSet = _myDatabaseWrapper.GetStatisticDataSet(trialFixInformation[0]);
                            DataSet lowerStatisticDataSet = _myDatabaseWrapper.GetStatisticDataSet(trialFixInformation[1]);

                            double velocityVectorCorrelation =
                                (Convert.ToDouble(upperStatisticDataSet.Tables[0].Rows[0]["velocity_vector_correlation"]) +
                                 Convert.ToDouble(lowerStatisticDataSet.Tables[0].Rows[0]["velocity_vector_correlation"]))/
                                2;
                            double trajectoryLengthAbs =
                                (Convert.ToDouble(upperStatisticDataSet.Tables[0].Rows[0]["trajectory_length_abs"]) +
                                 Convert.ToDouble(lowerStatisticDataSet.Tables[0].Rows[0]["trajectory_length_abs"]))/2;
                            double trajectoryLengthRatioBaseline =
                                (Convert.ToDouble(
                                    upperStatisticDataSet.Tables[0].Rows[0]["trajectory_length_ratio_baseline"]) +
                                 Convert.ToDouble(
                                     lowerStatisticDataSet.Tables[0].Rows[0]["trajectory_length_ratio_baseline"]))/2;
                            double perpendicularDisplacement300MsAbs =
                                (Convert.ToDouble(
                                    upperStatisticDataSet.Tables[0].Rows[0]["perpendicular_displacement_300ms_abs"]) +
                                 Convert.ToDouble(
                                     lowerStatisticDataSet.Tables[0].Rows[0]["perpendicular_displacement_300ms_abs"]))/2;
                            double maximalPerpendicularDisplacementAbs =
                                (Convert.ToDouble(
                                    upperStatisticDataSet.Tables[0].Rows[0]["maximal_perpendicular_displacement_abs"]) +
                                 Convert.ToDouble(
                                     lowerStatisticDataSet.Tables[0].Rows[0]["maximal_perpendicular_displacement_abs"]))/
                                2;
                            double meanPerpendicularDisplacementAbs =
                                (Convert.ToDouble(
                                    upperStatisticDataSet.Tables[0].Rows[0]["mean_perpendicular_displacement_abs"]) +
                                 Convert.ToDouble(
                                     lowerStatisticDataSet.Tables[0].Rows[0]["mean_perpendicular_displacement_abs"]))/2;
                            double perpendicularDisplacement300MsSign =
                                (Convert.ToDouble(
                                    upperStatisticDataSet.Tables[0].Rows[0]["perpendicular_displacement_300ms_sign"]) +
                                 Convert.ToDouble(
                                     lowerStatisticDataSet.Tables[0].Rows[0]["perpendicular_displacement_300ms_sign"]))/
                                2;
                            double maximalPerpendicularDisplacementSign =
                                (Convert.ToDouble(
                                    upperStatisticDataSet.Tables[0].Rows[0]["maximal_perpendicular_displacement_sign"]) +
                                 Convert.ToDouble(
                                     lowerStatisticDataSet.Tables[0].Rows[0]["maximal_perpendicular_displacement_sign"]))/
                                2;
                            double enclosedArea =
                                (Convert.ToDouble(upperStatisticDataSet.Tables[0].Rows[0]["enclosed_area"]) +
                                 Convert.ToDouble(lowerStatisticDataSet.Tables[0].Rows[0]["enclosed_area"]))/2;
                            double rmse = (Convert.ToDouble(upperStatisticDataSet.Tables[0].Rows[0]["rmse"]) +
                                           Convert.ToDouble(lowerStatisticDataSet.Tables[0].Rows[0]["rmse"]))/2;

                            _myDatabaseWrapper.InsertStatisticData(
                                Convert.ToInt32(faultyTrialInformation[trialIdCounter][0]),
                                velocityVectorCorrelation,
                                trajectoryLengthAbs,
                                trajectoryLengthRatioBaseline,
                                perpendicularDisplacement300MsAbs,
                                maximalPerpendicularDisplacementAbs,
                                meanPerpendicularDisplacementAbs,
                                perpendicularDisplacement300MsSign,
                                maximalPerpendicularDisplacementSign,
                                enclosedArea,
                                rmse);
                        }
                    }
                }
            }
            else
            {
                _myManipAnalysisGui.WriteToLogBox("Trials already fixed!");
            }
            _myManipAnalysisGui.SetProgressBarValue(0);
            _myManipAnalysisGui.WriteProgressInfo("Ready");
            _myManipAnalysisGui.EnableTabPages(true);

            TaskManager.Remove(Task.CurrentId);
        }

        public void PlotTrajectory(IEnumerable<TrajectoryVelocityPlotContainer> selectedTrials, string meanIndividual,
            bool showCatchTrials, bool showCatchTrialsExclusivly, bool showErrorclampTrials,
            bool showErrorclampTrialsExclusivly, bool showForceVectors, bool showPdForceVectors)
        {
            TaskManager.PushBack(Task.Factory.StartNew(() =>
            {
                _myManipAnalysisGui.WriteProgressInfo("Getting data...");
                List<TrajectoryVelocityPlotContainer> selectedTrialsList = selectedTrials.ToList();

                if (meanIndividual == "Individual")
                {
                    if (showForceVectors || showPdForceVectors)
                    {
                        _myMatlabWrapper.CreateTrajectoryForceFigure("XZ-Plot");
                    }
                    else
                    {
                        _myMatlabWrapper.CreateTrajectoryFigure("XZ-Plot");
                    }
                    _myMatlabWrapper.DrawTargets(0.005, 0.1, 0, 0);


                    int counter = 0;
                    foreach (TrajectoryVelocityPlotContainer tempContainer in selectedTrialsList)
                    {
                        if (TaskManager.Cancel)
                        {
                            break;
                        }
                        _myManipAnalysisGui.SetProgressBarValue((100.0/selectedTrialsList.Count())*counter);
                        counter++;
                        DateTime turnDateTime = _myDatabaseWrapper.GetTurnDateTime(tempContainer.Study, tempContainer.Group,
                            tempContainer.Szenario,
                            tempContainer.Subject,
                            tempContainer.Turn);
                        foreach (int trial in tempContainer.Trials)
                        {
                            if (TaskManager.Cancel)
                            {
                                break;
                            }
                            int trialId = _myDatabaseWrapper.GetTrailID(tempContainer.Study, tempContainer.Group,
                                tempContainer.Szenario, tempContainer.Subject,
                                turnDateTime, tempContainer.Target, trial);
                            DataSet measureDataSet = _myDatabaseWrapper.GetMeasureDataNormalizedDataSet(trialId);

                            var measureDataX = new List<double>();
                            var measureDataZ = new List<double>();
                            var forceDataX = new List<double>();
                            var forceDataZ = new List<double>();

                            foreach (DataRow row in measureDataSet.Tables[0].Rows)
                            {
                                if (TaskManager.Cancel)
                                {
                                    break;
                                }
                                if (showCatchTrialsExclusivly)
                                {
                                    if (Convert.ToBoolean(row["is_catch_trial"]))
                                    {
                                        measureDataX.Add(Convert.ToDouble(row["position_cartesian_x"]));
                                        measureDataZ.Add(Convert.ToDouble(row["position_cartesian_z"]));
                                        if (showForceVectors || showPdForceVectors)
                                        {
                                            forceDataX.Add(Convert.ToDouble(row["force_actual_x"]));
                                            forceDataZ.Add(Convert.ToDouble(row["force_actual_z"]));
                                        }
                                    }
                                }
                                else if (showErrorclampTrialsExclusivly)
                                {
                                    if (Convert.ToBoolean(row["is_errorclamp_trial"]))
                                    {
                                        measureDataX.Add(Convert.ToDouble(row["position_cartesian_x"]));
                                        measureDataZ.Add(Convert.ToDouble(row["position_cartesian_z"]));
                                        if (showForceVectors || showPdForceVectors)
                                        {
                                            forceDataX.Add(Convert.ToDouble(row["force_actual_x"]));
                                            forceDataZ.Add(Convert.ToDouble(row["force_actual_z"]));
                                        }
                                    }
                                }
                                else if (showCatchTrials & showErrorclampTrials)
                                {
                                    measureDataX.Add(Convert.ToDouble(row["position_cartesian_x"]));
                                    measureDataZ.Add(Convert.ToDouble(row["position_cartesian_z"]));
                                    if (showForceVectors || showPdForceVectors)
                                    {
                                        forceDataX.Add(Convert.ToDouble(row["force_actual_x"]));
                                        forceDataZ.Add(Convert.ToDouble(row["force_actual_z"]));
                                    }
                                }
                                else if (!showCatchTrials & showErrorclampTrials)
                                {
                                    if (Convert.ToBoolean(row["is_catch_trial"]) == false)
                                    {
                                        measureDataX.Add(Convert.ToDouble(row["position_cartesian_x"]));
                                        measureDataZ.Add(Convert.ToDouble(row["position_cartesian_z"]));
                                        if (showForceVectors || showPdForceVectors)
                                        {
                                            forceDataX.Add(Convert.ToDouble(row["force_actual_x"]));
                                            forceDataZ.Add(Convert.ToDouble(row["force_actual_z"]));
                                        }
                                    }
                                }
                                else if (showCatchTrials & !showErrorclampTrials)
                                {
                                    if (Convert.ToBoolean(row["is_errorclamp_trial"]) == false)
                                    {
                                        measureDataX.Add(Convert.ToDouble(row["position_cartesian_x"]));
                                        measureDataZ.Add(Convert.ToDouble(row["position_cartesian_z"]));
                                        if (showForceVectors || showPdForceVectors)
                                        {
                                            forceDataX.Add(Convert.ToDouble(row["force_actual_x"]));
                                            forceDataZ.Add(Convert.ToDouble(row["force_actual_z"]));
                                        }
                                    }
                                }
                                else if (!showCatchTrials & !showErrorclampTrials)
                                {
                                    if (Convert.ToBoolean(row["is_errorclamp_trial"]) == false &
                                        Convert.ToBoolean(row["is_catch_trial"]) == false)
                                    {
                                        measureDataX.Add(Convert.ToDouble(row["position_cartesian_x"]));
                                        measureDataZ.Add(Convert.ToDouble(row["position_cartesian_z"]));
                                        if (showForceVectors || showPdForceVectors)
                                        {
                                            forceDataX.Add(Convert.ToDouble(row["force_actual_x"]));
                                            forceDataZ.Add(Convert.ToDouble(row["force_actual_z"]));
                                        }
                                    }
                                }
                            }

                            _myMatlabWrapper.SetWorkspaceData("X", measureDataX.ToArray());
                            _myMatlabWrapper.SetWorkspaceData("Z", measureDataZ.ToArray());
                            _myMatlabWrapper.Plot("X", "Z", "black", 2);


                            if ((showForceVectors || showPdForceVectors) && measureDataX.Count > 1)
                            {
                                for (int i = 2; i < measureDataX.Count & !TaskManager.Pause; i++)
                                {
                                    _myMatlabWrapper.SetWorkspaceData("vpos1", new[]
                                    {
                                        measureDataX.ElementAt(i - 2),
                                        measureDataZ.ElementAt(i - 2)
                                    });
                                    _myMatlabWrapper.SetWorkspaceData("vpos2", new[]
                                    {
                                        measureDataX.ElementAt(i - 1),
                                        measureDataZ.ElementAt(i - 1)
                                    });
                                    _myMatlabWrapper.SetWorkspaceData("vforce", new[]
                                    {
                                        forceDataX.ElementAt(i - 2)/100.0,
                                        forceDataZ.ElementAt(i - 2)/100.0
                                    });
                                    if (showForceVectors)
                                    {
                                        _myMatlabWrapper.Execute(
                                            "quiver(vpos2(1),vpos2(2),vforce(1),vforce(2),'Color','red');");
                                    }
                                    if (showPdForceVectors)
                                    {
                                        _myMatlabWrapper.Execute("fPD = pdForceLineSegment(vforce, vpos1, vpos2);");
                                        _myMatlabWrapper.Execute(
                                            "quiver(vpos2(1),vpos2(2),fPD(1),fPD(2),'Color','blue');");
                                    }
                                }
                            }
                        }
                    }
                }
                else if (meanIndividual == "Mean")
                {
                    if (
                        selectedTrialsList.Select(t => t.Trials.ToArray())
                            .Distinct(new ArrayComparer()).Count() > 1)
                    {
                        _myManipAnalysisGui.WriteToLogBox("Trial selections are not equal!");
                    }
                    else
                    {
                        _myMatlabWrapper.CreateTrajectoryFigure("XZ-Plot");
                        _myMatlabWrapper.DrawTargets(0.005, 0.1, 0, 0);


                        int[] targetArray = selectedTrialsList.Select(t => t.Target).Distinct().ToArray();
                        int counter = 0;

                        for (int targetCounter = 0;
                            targetCounter < targetArray.Length & !TaskManager.Cancel;
                            targetCounter++)
                        {
                            int targetCounterVar = targetCounter;
                            int meanCounter = 0;
                            var dataX = new List<double>();
                            var dataZ = new List<double>();

                            foreach (
                                TrajectoryVelocityPlotContainer tempContainer in
                                    selectedTrialsList.Where(
                                        t => t.Target == targetArray[targetCounterVar]))
                            {
                                if (TaskManager.Cancel)
                                {
                                    break;
                                }
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
                                    if (TaskManager.Cancel)
                                    {
                                        break;
                                    }
                                    int trialID = _myDatabaseWrapper.GetTrailID(tempContainer.Study, tempContainer.Group,
                                        tempContainer.Szenario,
                                        tempContainer.Subject, turnDateTime,
                                        tempContainer.Target, trial);
                                    DataSet dataSet = _myDatabaseWrapper.GetMeasureDataNormalizedDataSet(trialID);
                                    for (int i = 0; i < dataSet.Tables[0].Rows.Count & !TaskManager.Cancel; i++)
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
                            for (int i = 0; i < dataX.Count; i++)
                            {
                                dataX[i] /= meanCounter;
                                dataZ[i] /= meanCounter;
                            }

                            _myMatlabWrapper.SetWorkspaceData("X", dataX.ToArray());
                            _myMatlabWrapper.SetWorkspaceData("Z", dataZ.ToArray());
                            _myMatlabWrapper.Plot("X", "Z", "black", 2);
                        }
                    }
                }

                _myMatlabWrapper.ClearWorkspace();

                _myManipAnalysisGui.SetProgressBarValue(0);
                _myManipAnalysisGui.WriteProgressInfo("Ready");
                TaskManager.Remove(Task.CurrentId);
            }));
        }

        public void PlotVelocity(IEnumerable<TrajectoryVelocityPlotContainer> selectedTrials, string meanIndividual,
            bool showCatchTrials, bool showCatchTrialsExclusivly, bool showErrorclampTrials,
            bool showErrorclampTrialsExclusivly)
        {
            TaskManager.PushBack(Task.Factory.StartNew(() =>
            {
                _myManipAnalysisGui.WriteProgressInfo("Getting data...");
                List<TrajectoryVelocityPlotContainer> selectedTrialsList = selectedTrials.ToList();

                if (meanIndividual == "Individual")
                {
                    _myMatlabWrapper.CreateVelocityFigure("Velocity plot", 101);

                    int counter = 0;
                    foreach (TrajectoryVelocityPlotContainer tempContainer in selectedTrialsList)
                    {
                        if (TaskManager.Cancel)
                        {
                            break;
                        }
                        _myManipAnalysisGui.SetProgressBarValue((100.0/selectedTrialsList.Count())*counter);
                        counter++;
                        DateTime turnDateTime = _myDatabaseWrapper.GetTurnDateTime(tempContainer.Study, tempContainer.Group,
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
                            var velocityDataXZ = new List<double>();

                            foreach (DataRow row in velocityDataSet.Tables[0].Rows)
                            {
                                if (TaskManager.Cancel)
                                {
                                    break;
                                }
                                if (showCatchTrialsExclusivly)
                                {
                                    if (Convert.ToBoolean(row["is_catch_trial"]))
                                    {
                                        velocityDataXZ.Add(
                                            Math.Sqrt(Math.Pow(Convert.ToDouble(row["velocity_x"]), 2) +
                                                      Math.Pow(Convert.ToDouble(row["velocity_z"]), 2)));
                                    }
                                }
                                else if (showErrorclampTrialsExclusivly)
                                {
                                    if (Convert.ToBoolean(row["is_errorclamp_trial"]))
                                    {
                                        velocityDataXZ.Add(
                                            Math.Sqrt(Math.Pow(Convert.ToDouble(row["velocity_x"]), 2) +
                                                      Math.Pow(Convert.ToDouble(row["velocity_z"]), 2)));
                                    }
                                }
                                else if (showCatchTrials & showErrorclampTrials)
                                {
                                    velocityDataXZ.Add(
                                        Math.Sqrt(Math.Pow(Convert.ToDouble(row["velocity_x"]), 2) +
                                                  Math.Pow(Convert.ToDouble(row["velocity_z"]), 2)));
                                }
                                else if (!showCatchTrials & showErrorclampTrials)
                                {
                                    if (Convert.ToBoolean(row["is_catch_trial"]) == false)
                                    {
                                        velocityDataXZ.Add(
                                            Math.Sqrt(Math.Pow(Convert.ToDouble(row["velocity_x"]), 2) +
                                                      Math.Pow(Convert.ToDouble(row["velocity_z"]), 2)));
                                    }
                                }
                                else if (showCatchTrials & !showErrorclampTrials)
                                {
                                    if (Convert.ToBoolean(row["is_errorclamp_trial"]) == false)
                                    {
                                        velocityDataXZ.Add(
                                            Math.Sqrt(Math.Pow(Convert.ToDouble(row["velocity_x"]), 2) +
                                                      Math.Pow(Convert.ToDouble(row["velocity_z"]), 2)));
                                    }
                                }
                                else if (!showCatchTrials & !showErrorclampTrials)
                                {
                                    if (Convert.ToBoolean(row["is_errorclamp_trial"]) == false &
                                        Convert.ToBoolean(row["is_catch_trial"]) == false)
                                    {
                                        velocityDataXZ.Add(
                                            Math.Sqrt(Math.Pow(Convert.ToDouble(row["velocity_x"]), 2) +
                                                      Math.Pow(Convert.ToDouble(row["velocity_z"]), 2)));
                                    }
                                }
                            }

                            _myMatlabWrapper.SetWorkspaceData("XZ", velocityDataXZ.ToArray());
                            _myMatlabWrapper.Plot("XZ", "black", 2);
                        }
                    }
                }
                else if (meanIndividual == "Mean")
                {
                    if (
                        selectedTrialsList.Select(t => t.Trials.ToArray())
                            .Distinct(new ArrayComparer()).Count() > 1)
                    {
                        _myManipAnalysisGui.WriteToLogBox("Trial selections are not equal!");
                    }
                    else
                    {
                        _myMatlabWrapper.CreateFigure("Velocity plot", "[Samples]", "Velocity [m/s]");


                        int[] targetArray = selectedTrialsList.Select(t => t.Target).Distinct().ToArray();
                        int counter = 0;

                        for (int targetCounter = 0;
                            targetCounter < targetArray.Length & !TaskManager.Cancel;
                            targetCounter++)
                        {
                            int targetCounterVar = targetCounter;
                            int meanCounter = 0;
                            var dataXZ = new List<double>();

                            foreach (
                                TrajectoryVelocityPlotContainer tempContainer in
                                    selectedTrialsList.Where(t => t.Target == targetArray[targetCounterVar]))
                            {
                                if (TaskManager.Cancel)
                                {
                                    break;
                                }
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
                                    if (TaskManager.Cancel)
                                    {
                                        break;
                                    }
                                    int trialID = _myDatabaseWrapper.GetTrailID(tempContainer.Study,
                                        tempContainer.Group,
                                        tempContainer.Szenario,
                                        tempContainer.Subject,
                                        turnDateTime, tempContainer.Target,
                                        trial);
                                    DataSet dataSet = _myDatabaseWrapper.GetVelocityDataNormalizedDataSet(trialID);

                                    for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                                    {
                                        DataRow row = dataSet.Tables[0].Rows[i];

                                        if (dataXZ.Count <= i)
                                        {
                                            dataXZ.Add(
                                                Math.Sqrt(Math.Pow(Convert.ToDouble(row["velocity_x"]), 2) +
                                                          Math.Pow(Convert.ToDouble(row["velocity_z"]), 2)));
                                        }
                                        else
                                        {
                                            dataXZ[i] +=
                                                Math.Sqrt(Math.Pow(Convert.ToDouble(row["velocity_x"]), 2) +
                                                          Math.Pow(Convert.ToDouble(row["velocity_z"]), 2));
                                        }
                                    }
                                    meanCounter++;
                                }
                            }

                            for (int i = 0; i < dataXZ.Count; i++)
                            {
                                dataXZ[i] /= meanCounter;
                            }

                            _myMatlabWrapper.SetWorkspaceData("X", dataXZ.ToArray());
                            _myMatlabWrapper.Plot("X", "black", 2);
                        }
                    }
                }
                _myManipAnalysisGui.SetProgressBarValue(0);
                _myManipAnalysisGui.WriteProgressInfo("Ready");
                TaskManager.Remove(Task.CurrentId);
            }));
        }

        public void ExportTrajectoryData(IEnumerable<TrajectoryVelocityPlotContainer> selectedTrials,
            string meanIndividual, string fileName)
        {
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
                        SubjectInformationContainer[] subjectArray =
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
                                        String.Join<SubjectInformationContainer>(",", subjectArray) + ";" +
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
        }

        public void ExportVelocityData(IEnumerable<TrajectoryVelocityPlotContainer> selectedTrials,
            string meanIndividual, string fileName)
        {
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
                        SubjectInformationContainer[] subjectArray =
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
                                              String.Join<SubjectInformationContainer>(",", subjectArray) +
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
        }

        public void ExportTrajectoryBaseline(string study, string group, string szenario,
            SubjectInformationContainer subject, string fileName)
        {
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
        }

        public void ExportVelocityBaseline(string study, string group, string szenario,
            SubjectInformationContainer subject, string fileName)
        {
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
        }

        public void PlotVelocityBaselines(string study, string group, string szenario,
            SubjectInformationContainer subject)
        {
            TaskManager.PushBack(Task.Factory.StartNew(() =>
            {
                _myMatlabWrapper.CreateVelocityFigure("Velocity baseline plot", 101);
                _myMatlabWrapper.DrawTargets(0.005, 0.1, 0, 0);

                DataSet baseline = _myDatabaseWrapper.GetBaselineDataSet(study, group, szenario, subject);

                List<object[]> baselineData = (from DataRow row in baseline.Tables[0].Rows
                    select new object[]
                    {
                        Convert.ToDouble(row["baseline_velocity_x"]),
                        Convert.ToDouble(row["baseline_velocity_z"]),
                        Convert.ToInt32(row["target_number"])
                    }).ToList();

                int[] targetNumberArray = baselineData.Select(t => Convert.ToInt32(t[2])).Distinct().ToArray();

                for (int i = 0; i < targetNumberArray.Length & !TaskManager.Cancel; i++)
                {
                    double[] tempXZ =
                        baselineData.Where(t => Convert.ToInt32(t[2]) == targetNumberArray[i])
                            .Select(
                                t =>
                                    Math.Sqrt(Math.Pow(Convert.ToDouble(t[0]), 2) + Math.Pow(Convert.ToDouble(t[1]), 2)))
                            .ToArray();
                    _myMatlabWrapper.SetWorkspaceData("XZ", tempXZ);
                    _myMatlabWrapper.Plot("XZ", 2);
                }

                _myMatlabWrapper.ClearWorkspace();
                TaskManager.Remove(Task.CurrentId);
            }));
        }

        public void RecalculateBaselines(IEnumerable<TrajectoryVelocityPlotContainer> selectedTrials)
        {
            List<TrajectoryVelocityPlotContainer> selectedTrialsList = selectedTrials.ToList();
            int[] targetArray = selectedTrialsList.Select(t => t.Target).Distinct().ToArray();
            SubjectInformationContainer subject = selectedTrialsList.Select(t => t.Subject).ElementAt(0);

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
        }

        public void PlotLearningIndex(string study, string group, string szenario,
            IEnumerable<SubjectInformationContainer> subjects, int turn)
        {
            TaskManager.PushBack(Task.Factory.StartNew(() =>
            {
                List<SubjectInformationContainer> subjectList = subjects.ToList(); // Subject list
                string[] szenarioTrials = _myDatabaseWrapper.GetSzenarioTrials(study, szenario, true, false, false, false);
                // List of all trials in the szenario
                int setCount = szenarioTrials.Length/16; // The number of sets in the szenario
                var liData = new List<double>[setCount]; // Array of Lists, size is equal to the number of sets

                // Info output
                _myManipAnalysisGui.WriteProgressInfo("Calculating learning index...");
                _myManipAnalysisGui.SetProgressBarValue(0);

                // Loops over all subjects. Can also be only one.
                for (int subjectCounter = 0;
                    subjectCounter < subjectList.Count() & !TaskManager.Cancel;
                    subjectCounter++)
                {
                    _myManipAnalysisGui.SetProgressBarValue((100.0/subjectList.Count())*subjectCounter);

                    // Gets the statistic-data for the subject
                    DateTime turnDateTime = _myDatabaseWrapper.GetTurnDateTime(study, group, szenario,
                        subjectList[subjectCounter], turn);
                    DataSet statisticDataSet = _myDatabaseWrapper.GetStatisticDataSet(study, group, szenario,
                        subjectList[subjectCounter],
                        turnDateTime);

                    // Checks wether the amount of statistic-data equals the amount of szenario-trials
                    if (statisticDataSet.Tables[0].Rows.Count == szenarioTrials.Length)
                    {
                        // Loops over all sets in the szenario
                        for (int setCounter = 0; setCounter < setCount & !TaskManager.Cancel; setCounter++)
                        {
                            var catchPdSign = new List<double>();
                            var catchPdAbs = new List<double>();
                            var fieldPdAbs = new List<double>();

                            // Saves the perpendicular displacement data depending on wether a catch-trial is present in the set or not
                            for (int rowCounter = (setCounter*16);
                                rowCounter < ((setCounter + 1)*16) & !TaskManager.Cancel;
                                rowCounter++)
                            {
                                DataRow row = statisticDataSet.Tables[0].Rows[rowCounter];
                                int szenarioTrialNumber = Convert.ToInt32(row["szenario_trial_number"]);
                                double pd300Abs = Convert.ToDouble(row["perpendicular_displacement_300ms_abs"]);
                                double pd300Sign = Convert.ToDouble(row["perpendicular_displacement_300ms_sign"]);
                                bool isCatchTrial =
                                    szenarioTrials.Contains("Trial " + szenarioTrialNumber.ToString("000") +
                                                            " - CatchTrial");

                                if (isCatchTrial)
                                {
                                    catchPdAbs.Add(pd300Abs);
                                    catchPdSign.Add(pd300Sign);
                                }
                                else
                                {
                                    fieldPdAbs.Add(pd300Abs);
                                }
                            }

                            // If a catch-trial was present in the set, the learning-index is calculated and saved
                            if (catchPdAbs.Count > 0)
                            {
                                if (liData[setCounter] == null)
                                {
                                    liData[setCounter] = new List<double>();
                                }

                                liData[setCounter].Add(catchPdSign.Average()/
                                                       (catchPdAbs.Average() + fieldPdAbs.Average()));
                            }
                        }
                    }
                }

                // Some necessary Array-magic to leave out the sets with no catch-trials
                int trueSetCount = liData.Count(t => t != null);
                var stdAbwArr = new double[trueSetCount];
                var liDataArr = new double[trueSetCount];
                var setCountArr = new double[trueSetCount];
                trueSetCount = 0;

                // Fill the data-arrays and calculate the StdAbv
                for (int setCounter = 0; setCounter < setCount & !TaskManager.Cancel; setCounter++)
                {
                    if (liData[setCounter] != null)
                    {
                        stdAbwArr[trueSetCount] =
                            Math.Sqrt((liData[setCounter].Sum(t => Math.Pow(t - liData[setCounter].Average(), 2)))
                                      /
                                      (liData[setCounter].Count - 1)
                                );
                        liDataArr[trueSetCount] = liData[setCounter].Average();
                        setCountArr[trueSetCount] = setCounter + 1;
                        trueSetCount++;
                    }
                }

                _myMatlabWrapper.CreateLearningIndexFigure(setCount);
                _myMatlabWrapper.SetWorkspaceData("X", setCountArr);
                _myMatlabWrapper.SetWorkspaceData("Y", liDataArr);
                _myMatlabWrapper.SetWorkspaceData("std", stdAbwArr);
                _myMatlabWrapper.PlotMeanTimeErrorBar("X", "Y", "std");

                _myManipAnalysisGui.WriteProgressInfo("Ready.");
                _myManipAnalysisGui.SetProgressBarValue(0);
                TaskManager.Remove(Task.CurrentId);
            }));
        }

        public void ExportLearningIndex(string fileName, string study, string group, string szenario,
            IEnumerable<SubjectInformationContainer> subjects, int turn)
        {
            TaskManager.PushBack(Task.Factory.StartNew(() =>
            {
                List<SubjectInformationContainer> subjectList = subjects.ToList(); // Subject list
                string[] szenarioTrials = _myDatabaseWrapper.GetSzenarioTrials(study, szenario, true, false, false, false);
                // List of all trials in the szenario
                int setCount = szenarioTrials.Length/16; // The number of sets in the szenario
                var liData = new List<double>[setCount]; // Array of Lists, size is equal to the number of sets

                // Info output
                _myManipAnalysisGui.WriteProgressInfo("Calculating learning index...");
                _myManipAnalysisGui.SetProgressBarValue(0);

                // Loops over all subjects. Can also be only one.
                for (int subjectCounter = 0;
                    subjectCounter < subjectList.Count() & !TaskManager.Cancel;
                    subjectCounter++)
                {
                    _myManipAnalysisGui.SetProgressBarValue((100.0/subjectList.Count())*subjectCounter);

                    // Gets the statistic-data for the subject
                    DateTime turnDateTime = _myDatabaseWrapper.GetTurnDateTime(study, group, szenario,
                        subjectList[subjectCounter], turn);
                    DataSet statisticDataSet = _myDatabaseWrapper.GetStatisticDataSet(study, group, szenario,
                        subjectList[subjectCounter],
                        turnDateTime);

                    // Checks wether the amount of statistic-data equals the amount of szenario-trials
                    if (statisticDataSet.Tables[0].Rows.Count == szenarioTrials.Length)
                    {
                        // Loops over all sets in the szenario
                        for (int setCounter = 0; setCounter < setCount; setCounter++)
                        {
                            var catchPdSign = new List<double>();
                            var catchPdAbs = new List<double>();
                            var fieldPdAbs = new List<double>();

                            // Saves the perpendicular displacement data depending on wether a catch-trial is present in the set or not
                            for (int rowCounter = (setCounter*16);
                                rowCounter < ((setCounter + 1)*16) & !TaskManager.Cancel;
                                rowCounter++)
                            {
                                DataRow row = statisticDataSet.Tables[0].Rows[rowCounter];
                                int szenarioTrialNumber = Convert.ToInt32(row["szenario_trial_number"]);
                                double pd300Abs = Convert.ToDouble(row["perpendicular_displacement_300ms_abs"]);
                                double pd300Sign = Convert.ToDouble(row["perpendicular_displacement_300ms_sign"]);
                                bool isCatchTrial =
                                    szenarioTrials.Contains("Trial " + szenarioTrialNumber.ToString("000") +
                                                            " - CatchTrial");

                                if (isCatchTrial)
                                {
                                    catchPdAbs.Add(pd300Abs);
                                    catchPdSign.Add(pd300Sign);
                                }
                                else
                                {
                                    fieldPdAbs.Add(pd300Abs);
                                }
                            }

                            // If a catch-trial was present in the set, the learning-index is calculated and saved
                            if (catchPdAbs.Count > 0)
                            {
                                if (liData[setCounter] == null)
                                {
                                    liData[setCounter] = new List<double>();
                                }

                                liData[setCounter].Add(catchPdSign.Average()/
                                                       (catchPdAbs.Average() + fieldPdAbs.Average()));
                            }
                        }
                    }
                }

                // Create write-cache and header-line
                var cache = new List<string>();
                string line = "SzenarioSetNumber,";
                for (int subjectCounter = 0; subjectCounter < subjectList.Count & !TaskManager.Cancel; subjectCounter++)
                {
                    line += subjectList.ElementAt(subjectCounter).SubjectID + ",";
                }
                line += "Mean,Std";
                cache.Add(line);


                // Loops over all sets and adds the data to the cache
                for (int setCounter = 0; setCounter < setCount & !TaskManager.Cancel; setCounter++)
                {
                    if (liData[setCounter] != null)
                    {
                        line = (setCounter + 1).ToString(CultureInfo.InvariantCulture) + ",";

                        for (int subjectCounter = 0;
                            subjectCounter < liData[setCounter].Count & !TaskManager.Cancel;
                            subjectCounter++)
                        {
                            line += DoubleConverter.ToExactString(liData[setCounter].ElementAt(subjectCounter)) + ",";
                        }

                        line += DoubleConverter.ToExactString(liData[setCounter].Average()) + ",";

                        line +=
                            DoubleConverter.ToExactString(
                                Math.Sqrt((liData[setCounter].Sum(t => Math.Pow(t - liData[setCounter].Average(), 2)))
                                          /
                                          (liData[setCounter].Count - 1)
                                    ));

                        cache.Add(line);
                    }
                }

                var dataFileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                var dataFileWriter = new StreamWriter(dataFileStream);

                for (int i = 0; i < cache.Count() & !TaskManager.Cancel; i++)
                {
                    dataFileWriter.WriteLine(cache[i]);
                }

                dataFileWriter.Close();

                _myManipAnalysisGui.WriteProgressInfo("Ready.");
                _myManipAnalysisGui.SetProgressBarValue(0);
                TaskManager.Remove(Task.CurrentId);
            }));
        }

        public void ForcefieldCompensationFactor(string study, string group, string szenario,
            SubjectInformationContainer subject, int turn, int msIndex)
        {
            TaskManager.PushBack(Task.Factory.StartNew(() =>
            {
                if (szenario == "Szenario44_N" || szenario == "Szenario44_R" || szenario == "Szenario45_N" ||
                    szenario == "Szenario45_R")
                {
                    // Info output
                    _myManipAnalysisGui.WriteProgressInfo("Getting data...");
                    _myManipAnalysisGui.SetProgressBarValue(0);

                    DateTime turnDateTime = GetTurnDateTime(study, group, szenario, subject, turn);

                    IEnumerable<string> trialStringList = GetTrialsOfSzenario(study, szenario, false, false, true, true);
                    if (trialStringList != null)
                    {
                        var trialList = new List<int>();
                        for (int i = 0; i < trialStringList.Count(); i++)
                        {
                            trialList.Add(Convert.ToInt32(trialStringList.ElementAt(i).Substring(6, 3)));
                        }

                        _myMatlabWrapper.CreateForcefieldCompensationIndexFigure(trialList.Count);

                        for (int trialCounter = 0; trialCounter < trialList.Count & !TaskManager.Cancel; trialCounter++)
                        {
                            _myManipAnalysisGui.SetProgressBarValue((100.0/trialList.Count())*trialCounter);
                            int szenarioTrial = trialList.ElementAt(trialCounter);
                            int trialID = _myDatabaseWrapper.GetTrailID(study, group, szenario, subject, turnDateTime,
                                szenarioTrial);
                            DataSet measureDataSet = _myDatabaseWrapper.GetMeasureDataNormalizedDataSet(trialID);
                            DataSet velocityDataSet = _myDatabaseWrapper.GetVelocityDataNormalizedDataSet(trialID);
                            int sampleCount = measureDataSet.Tables[0].Rows.Count;
                            var positionData = new double[sampleCount, 2];
                            var velocityData = new double[sampleCount, 2];
                            var forceIstData = new double[sampleCount, 2];
                            //var forceSollData = new double[sampleCount, 2];
                            var timeStamp = new double[sampleCount];

                            if (measureDataSet.Tables[0].Rows.Count > 0)
                            {
                                int targetNumber = Convert.ToInt32(measureDataSet.Tables[0].Rows[0]["target_number"]);

                                for (int sample = 0; sample < sampleCount & !TaskManager.Cancel; sample++)
                                {
                                    timeStamp[sample] =
                                        Convert.ToDateTime(measureDataSet.Tables[0].Rows[sample]["time_stamp"]).Ticks;

                                    positionData[sample, 0] =
                                        Convert.ToDouble(measureDataSet.Tables[0].Rows[sample]["position_cartesian_x"]);
                                    positionData[sample, 1] =
                                        Convert.ToDouble(measureDataSet.Tables[0].Rows[sample]["position_cartesian_z"]);

                                    velocityData[sample, 0] =
                                        Convert.ToDouble(velocityDataSet.Tables[0].Rows[sample]["velocity_x"]);
                                    velocityData[sample, 1] =
                                        Convert.ToDouble(velocityDataSet.Tables[0].Rows[sample]["velocity_z"]);

                                    forceIstData[sample, 0] =
                                        Convert.ToDouble(measureDataSet.Tables[0].Rows[sample]["force_actual_x"]);
                                    forceIstData[sample, 1] =
                                        Convert.ToDouble(measureDataSet.Tables[0].Rows[sample]["force_actual_z"]);
                                    /*
                                    forceSollData[sample, 0] =
                                        Convert.ToDouble(measureDataSet.Tables[0].Rows[sample]["force_nominal_x"]);
                                    forceSollData[sample, 1] =
                                        Convert.ToDouble(measureDataSet.Tables[0].Rows[sample]["force_nominal_z"]);
                                     */
                                }

                                List<double> tempTimeList = timeStamp.ToList();
                                int timeMsIndex =
                                    tempTimeList.IndexOf(
                                        tempTimeList.OrderBy(
                                            d => Math.Abs(d - (timeStamp[0] + TimeSpan.FromMilliseconds(msIndex).Ticks)))
                                            .ElementAt(0));

                                _myMatlabWrapper.SetWorkspaceData("timeMsIndex", timeMsIndex);
                                _myMatlabWrapper.SetWorkspaceData("positionData", positionData);
                                _myMatlabWrapper.SetWorkspaceData("velocityData", velocityData);
                                _myMatlabWrapper.SetWorkspaceData("forceIstData", forceIstData);
                                //_myMatlabWrapper.SetWorkspaceData("forceSollDataData", forceSollData);

                                _myMatlabWrapper.Execute("forceSollData = bsxfun(@times, velocityData, [20 -20]);");
                                // CW-ForceField

                                _myMatlabWrapper.Execute("trials(" +
                                                         (trialCounter + 1).ToString(CultureInfo.InvariantCulture) +
                                                         ") = " +
                                                         (trialCounter + 1).ToString(CultureInfo.InvariantCulture) + ";");
                                _myMatlabWrapper.Execute("bars(" +
                                                         (trialCounter + 1).ToString(CultureInfo.InvariantCulture) +
                                                         ",1) = forcefieldCompensationFactor(positionData,velocityData,forceIstData,forceSollData,timeMsIndex);");

                                /*
                                _myMatlabWrapper.Execute("figure;");
                                _myMatlabWrapper.Execute("hold all;");
                                _myMatlabWrapper.Execute("plot(forceSollDataData(:,1),forceSollDataData(:,2),'r');");
                                _myMatlabWrapper.Execute("plot(forceSollData(:,1),forceSollDataData(:,2),'g');");
                                _myMatlabWrapper.Execute("plot(forceIstData(:,1),forceSollDataData(:,2),'b');");
                                _myMatlabWrapper.Execute("legend('ForcefieldRobot', 'ForcefieldCalculated', 'ForceMeasured');");
                                int a = 0;
                                 */
                            }
                        }
                        _myMatlabWrapper.Execute("bar(trials - 0.125,bars(:,1),0.25,'b');");
                        _myMatlabWrapper.Execute("legend('Forcefield compensation factor');");
                    }
                    _myMatlabWrapper.ClearWorkspace();
                    _myManipAnalysisGui.WriteProgressInfo("Ready.");
                    _myManipAnalysisGui.SetProgressBarValue(0);
                }
                else
                {
                    _myManipAnalysisGui.WriteToLogBox("Szenario doesn't contain any ErrorClamp-Trials.");
                }
                TaskManager.Remove(Task.CurrentId);
            }));
        }
    }
}