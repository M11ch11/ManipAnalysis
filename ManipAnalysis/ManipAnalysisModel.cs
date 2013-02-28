using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ManipAnalysis.Container;

namespace ManipAnalysis
{
    public class ManipAnalysisModel
    {
        private readonly ManipAnalysisGui _myManipAnalysisGui;
        private readonly MatlabWrapper _myMatlabWrapper;
        private readonly SqlWrapper _mySqlWrapper;

        public ManipAnalysisModel(ManipAnalysisGui myManipAnalysisGui, MatlabWrapper myMatlabWrapper,
                                  SqlWrapper mySqlWrapper)
        {
            _myMatlabWrapper = myMatlabWrapper;
            _mySqlWrapper = mySqlWrapper;
            _myManipAnalysisGui = myManipAnalysisGui;
        }

        private bool CheckSqlServerAvailability(string server)
        {
            bool serverAvailable = false;

            using (var tcp = new TcpClient())
            {
                IAsyncResult ar = tcp.BeginConnect(server, 1433, null, null);
                WaitHandle wh = ar.AsyncWaitHandle;
                try
                {
                    if (!ar.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(2), false))
                    {
                        tcp.Close();
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

        public bool ConnectToSqlServer(string server)
        {
            bool retVal = false;

            if (CheckSqlServerAvailability(server))
            {
                _myManipAnalysisGui.WriteToLogBox("Connected to SQL-Server.");
                _mySqlWrapper.SetSqlServer(server);
                _myManipAnalysisGui.SetSqlDatabases(_mySqlWrapper.GetDatabases());
                retVal = true;
            }
            else
            {
                _myManipAnalysisGui.WriteToLogBox("SQL-Server not reachable!");
            }

            return retVal;
        }

        public void SetSqlDatabase(string database)
        {
            _mySqlWrapper.SetDatabase(database);
        }

        public IEnumerable<string> GetStudys()
        {
            return _mySqlWrapper.GetStudyNames();
        }

        public IEnumerable<string> GetGroups(string study)
        {
            return _mySqlWrapper.GetGroupNames(study);
        }

        public IEnumerable<string> GetSzenarios(string study, string group)
        {
            return _mySqlWrapper.GetSzenarioNames(study, group);
        }

        public IEnumerable<SubjectInformationContainer> GetSubjects(string study, string group, string szenario)
        {
            return _mySqlWrapper.GetSubjectInformations(study, group, szenario);
        }

        public IEnumerable<string> GetTurns(string study, string group, string szenario,
                                            SubjectInformationContainer subject)
        {
            return _mySqlWrapper.GetTurns(study, group, szenario, subject);
        }

        public IEnumerable<string> GetTargets(string study, string szenario)
        {
            return _mySqlWrapper.GetTargets(study, szenario);
        }

        public IEnumerable<string> GetTrials(string study, string szenario)
        {
            return _mySqlWrapper.GetTrials(study, szenario);
        }

        public bool CheckIfMeasureFileHashAlreadyExists(string hash)
        {
            return _mySqlWrapper.CheckIfMeasureFileHashExists(hash);
        }

        private DateTime GetTurnDateTime(string study, string group, string szenario,
                                         SubjectInformationContainer subject,
                                         int turn)
        {
            return _mySqlWrapper.GetTurnDateTime(study, group, szenario, subject, turn);
        }

        public void DeleteMeasureFile(int measureFileID)
        {
            _mySqlWrapper.DeleteMeasureFile(measureFileID);
        }

        public void PlotSzenarioMeanTimes(string study, string group, string szenario,
                                          SubjectInformationContainer subject, int turn)
        {
            DateTime turnDateTime = GetTurnDateTime(study, group, szenario, subject, turn);
            DataSet meanTimeDataSet = _mySqlWrapper.GetMeanTimeDataSet(study, group, szenario, subject, turnDateTime);

            _myMatlabWrapper.CreateMeanTimeFigure();

            var meanTimeList = new List<double>();
            var meanTimeStdList = new List<double>();
            var targetList = new List<int>();

            foreach (DataRow row in meanTimeDataSet.Tables[0].Rows)
            {
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
        }

        public void ExportSzenarioMeanTimes(string study, string group, string szenario,
                                            SubjectInformationContainer subject, int turn, string fileName)
        {
            DateTime turnDateTime = _mySqlWrapper.GetTurnDateTime(study, group, szenario, subject, turn);

            DataSet meanTimeDataSet = _mySqlWrapper.GetMeanTimeDataSet(study, group, szenario, subject, turnDateTime);

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
            dataFileStream.Close();
        }

        public void ToggleMatlabCommandWindow()
        {
            _myMatlabWrapper.ToggleShowCommandWindow();
        }

        public void ShowMatlabWorkspace()
        {
            _myMatlabWrapper.ShowWorkspaceWindow();
        }

        public void InitializeSqlDatabase()
        {
            _mySqlWrapper.InitializeDatabase();
        }

        public void PlotBaseline(string study, string group, string szenario, SubjectInformationContainer subject)
        {
            _myMatlabWrapper.CreateTrajectoryFigure("Trajectory baseline plot");
            _myMatlabWrapper.DrawTargets(0.005, 0.1, 0, 0);

            DataSet baseline = _mySqlWrapper.GetBaselineDataSet(study, group, szenario, subject);

            List<object[]> baselineData = (from DataRow row in baseline.Tables[0].Rows
                                           select new object[]
                                               {
                                                   Convert.ToDouble(row["baseline_position_cartesian_x"]),
                                                   Convert.ToDouble(row["baseline_position_cartesian_z"]),
                                                   Convert.ToInt32(row["target_number"])
                                               }).ToList();

            int[] targetNumberArray = baselineData.Select(t => Convert.ToInt32(t[2])).Distinct().ToArray();

            for (int i = 0; i < targetNumberArray.Length; i++)
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
        }

        public IEnumerable<string> GetTrialsOfSzenario(string study, string szenario, bool showCatchTrials,
                                                       bool showCatchTrialsExclusivly)
        {
            return _mySqlWrapper.GetSzenarioTrials(study, szenario, showCatchTrials, showCatchTrialsExclusivly);
        }

        public void PlotDescriptiveStatistic1(IEnumerable<StatisticPlotContainer> selectedTrials, string statisticType,
                                              string fitEquation, bool plotFit, bool plotErrorbars)
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
                    var data = new double[trialList.Count,selectedTrialsList.Count()];

                    for (meanCounter = 0;
                         meanCounter < selectedTrialsList.Count();
                         meanCounter++)
                    {
                        _myManipAnalysisGui.SetProgressBarValue((100.0/selectedTrialsList.Count())*meanCounter);
                        StatisticPlotContainer temp = selectedTrialsList.ElementAt(meanCounter);

                        DateTime turn = GetTurnDateTime(temp.Study, temp.Group, temp.Szenario,
                                                        temp.Subject,
                                                        Convert.ToInt32(
                                                            temp.Turn.Substring("Turn".Length)));

                        DataSet statisticDataSet = _mySqlWrapper.GetStatisticDataSet(temp.Study, temp.Group,
                                                                                     temp.Szenario, temp.Subject,
                                                                                     turn);

                        int trialListCounter = 0;
                        foreach (DataRow row in statisticDataSet.Tables[0].Rows)
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

                    _myMatlabWrapper.SetWorkspaceData("data", data);

                    if (meanCounter > 1)
                    {
                        _myMatlabWrapper.Execute("dataPlot = mean(transpose(data));");
                        _myMatlabWrapper.Execute("dataStdPlot = std(transpose(data));");
                        _myMatlabWrapper.GetWorkspaceData("dataStdPlot");
                    }
                    else
                    {
                        _myMatlabWrapper.Execute("dataPlot = data;");
                    }

                    double[,] dataPlot = _myMatlabWrapper.GetWorkspaceData("dataPlot");

                    switch (statisticType)
                    {
                        case "Vector correlation":
                            _myMatlabWrapper.CreateStatisticFigure("Velocity Vector Correlation plot", "dataPlot",
                                                                   "fit(transpose([1:1:length(dataPlot)]),transpose(dataPlot),'" +
                                                                   fitEquation + "')",
                                                                   "dataStdPlot", "[Trial]",
                                                                   "Velocity Vector Correlation", 1, dataPlot.Length,
                                                                   0.5, 0.9,
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

                    _myMatlabWrapper.ClearWorkspace();
                }
            }

            _myManipAnalysisGui.WriteProgressInfo("Ready");
            _myManipAnalysisGui.SetProgressBarValue(0);
        }

        public IEnumerable<object[]> GetFaultyTrialInformation()
        {
            return _mySqlWrapper.GetFaultyTrialInformation();
        }

        public void ExportDescriptiveStatistic1Data(IEnumerable<StatisticPlotContainer> selectedTrials,
                                                    string statisticType, string fileName)
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
                int trialListCounter = 0;
                var data = new double[trialList.Count,selectedTrialsList.Count()];

                for (meanCounter = 0;
                     meanCounter < selectedTrialsList.Count();
                     meanCounter++)
                {
                    _myManipAnalysisGui.SetProgressBarValue((100.0/selectedTrialsList.Count())*
                                                            meanCounter);

                    StatisticPlotContainer temp = selectedTrialsList.ElementAt(meanCounter);

                    DateTime turn = _mySqlWrapper.GetTurnDateTime(temp.Study, temp.Group, temp.Szenario,
                                                                  temp.Subject,
                                                                  Convert.ToInt32(
                                                                      temp.Turn.Substring("Turn".Length)));

                    DataSet statisticDataSet = _mySqlWrapper.GetStatisticDataSet(temp.Study, temp.Group,
                                                                                 temp.Szenario, temp.Subject,
                                                                                 turn);

                    trialListCounter = 0;
                    for (int rowCounter = 0; rowCounter < statisticDataSet.Tables[0].Rows.Count; rowCounter++)
                    {
                        DataRow row = statisticDataSet.Tables[0].Rows[rowCounter];

                        int szenarioTrialNumber = Convert.ToInt32(row["szenario_trial_number"]);
                        if (trialList.Contains(szenarioTrialNumber))
                        {
                            switch (statisticType)
                            {
                                case "Vector correlation":
                                    data[trialListCounter, meanCounter] =
                                        Convert.ToDouble(row["velocity_vector_correlation"]);
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

                _myMatlabWrapper.SetWorkspaceData("data", data);

                double[,] dataMean;
                double[,] dataStd;

                if (meanCounter > 1)
                {
                    _myMatlabWrapper.Execute("dataMean = mean(transpose(data));");
                    _myMatlabWrapper.Execute("dataStd = std(transpose(data));");
                    dataMean = _myMatlabWrapper.GetWorkspaceData("dataMean");
                    dataStd = _myMatlabWrapper.GetWorkspaceData("dataStd");
                }
                else
                {
                    dataMean = new double[,] {{data[0, 0], 0}};
                    dataStd = new double[,] {{0, 0}};
                }

                var cache = new List<string>();
                var meanDataFileStream = new FileStream(fileName, FileMode.Create,
                                                        FileAccess.Write);
                var meanDataFileWriter = new StreamWriter(meanDataFileStream);

                string personNames = "";
                for (int i = 0; i < selectedTrialsList.Count(); i++)
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

                for (int i = 0; i < trialListCounter; i++)
                {
                    string tempLine = trialList.ElementAt(i) + ";";

                    for (int j = 0; j < meanCounter; j++)
                    {
                        tempLine += DoubleConverter.ToExactString(data[i, j]) + ";";
                    }

                    tempLine += DoubleConverter.ToExactString(dataMean[0, i])
                                + ";"
                                + DoubleConverter.ToExactString(dataStd[0, i]);

                    cache.Add(tempLine);
                }

                for (int i = 0; i < cache.Count(); i++)
                {
                    meanDataFileWriter.WriteLine(cache[i]);
                }

                meanDataFileWriter.Close();
                meanDataFileStream.Close();

                _myMatlabWrapper.ClearWorkspace();
            }
        }

        public void ExportDescriptiveStatistic2Data(IEnumerable<StatisticPlotContainer> selectedTrials,
                                                    string statisticType, string fileName)
        {
            int trialListCounter = 0;
            List<StatisticPlotContainer> selectedTrialsList = selectedTrials.ToList();
            List<int> trialList = selectedTrialsList.ElementAt(0).Trials;

            var data = new double[trialList.Count,selectedTrialsList.Count()];

            for (int meanCounter = 0;
                 meanCounter < selectedTrialsList.Count();
                 meanCounter++)
            {
                _myManipAnalysisGui.SetProgressBarValue((100.0/selectedTrialsList.Count())*meanCounter);
                StatisticPlotContainer temp = selectedTrialsList.ElementAt(meanCounter);

                DateTime turn = _mySqlWrapper.GetTurnDateTime(temp.Study, temp.Group, temp.Szenario,
                                                              temp.Subject,
                                                              Convert.ToInt32(temp.Turn.Substring("Turn".Length)));
                DataSet statisticDataSet = _mySqlWrapper.GetStatisticDataSet(temp.Study, temp.Group,
                                                                             temp.Szenario, temp.Subject,
                                                                             turn);

                trialListCounter = 0;
                foreach (DataRow row in statisticDataSet.Tables[0].Rows)
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

            if (trialListCounter > 1)
            {
                _myMatlabWrapper.SetWorkspaceData("data", data);

                _myMatlabWrapper.Execute("dataMean = mean(data);");
                _myMatlabWrapper.Execute("dataStd = std(data);");

                dataMean = _myMatlabWrapper.GetWorkspaceData("dataMean");
                dataStd = _myMatlabWrapper.GetWorkspaceData("dataStd");
            }
            else
            {
                dataMean = new double[,] {{data[0, 0], 0}};
                dataStd = new double[,] {{0, 0}};
            }

            var cache = new List<string>();
            var meanDataFileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            var meanDataFileWriter = new StreamWriter(meanDataFileStream);
            cache.Add("Study;Group;Szenario;Subject;Turn;Trials;Mean;Std");

            cache.AddRange(selectedTrialsList.Cast<object>().Select((t, i) => selectedTrialsList.ElementAt(i)).Select(
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

            for (int i = 0; i < cache.Count(); i++)
            {
                meanDataFileWriter.WriteLine(cache[i]);
            }

            meanDataFileWriter.Close();
            meanDataFileStream.Close();

            _myMatlabWrapper.ClearWorkspace();
        }

        public void ChangeGroupID(int groupID, int newGroupID)
        {
            _mySqlWrapper.ChangeGroupID(groupID, newGroupID);
        }

        public void ChangeGroupGroupName(int groupID, string newGroupName)
        {
            _mySqlWrapper.ChangeGroupName(groupID, newGroupName);
        }

        public void ChangeSubjectID(int subjectID, int newSubjectID)
        {
            _mySqlWrapper.ChangeSubjectID(subjectID, newSubjectID);
        }

        public void ChangeSubjectSubjectName(int subjectID, string newSubjectName)
        {
            _mySqlWrapper.ChangeSubjectName(subjectID, newSubjectName);
        }

        public void ChangeSubjectSubjectID(int subjectID, string newSubjectSubjectID)
        {
            _mySqlWrapper.ChangeSubjectSubjectID(subjectID, newSubjectSubjectID);
        }

        public void ImportMeasureFiles(IEnumerable<string> measureFiles, int samplesPerSecond, int butterFilterOrder,
                                       int butterFilterCutOff, int percentPeakVelocity, int timeNormalizationSamples)
        {
            List<string> measureFilesList = measureFiles.ToList();

            var newTask =
                new Task(
                    () => ImportMeasureFilesThread(measureFilesList, samplesPerSecond, butterFilterOrder,
                                                   butterFilterCutOff, percentPeakVelocity, timeNormalizationSamples));

            TaskManager.PushBack(newTask);
            newTask.Start();
        }

        private void ImportMeasureFilesThread(List<string> measureFilesList, int samplesPerSecond, int butterFilterOrder,
                                              int butterFilterCutOff, int percentPeakVelocity,
                                              int timeNormalizationSamples)
        {
            while (TaskManager.GetIndex(Task.CurrentId) != 0)
            {
                Thread.Sleep(100);
            }
            _myManipAnalysisGui.EnableTabPages(false);
            _myManipAnalysisGui.SetProgressBarValue(0);

            for (int files = 0; files < measureFilesList.Count; files++)
            {
                try
                {
                    while (TaskManager.Pause)
                    {
                        Thread.Sleep(100);
                    }

                    _myManipAnalysisGui.SetProgressBarValue((100.0/measureFilesList.Count)*files);

                    string filename = measureFilesList.ElementAt(files);

                    string tempFileHash = Md5.ComputeHash(filename);

                    if (!_mySqlWrapper.CheckIfMeasureFileHashExists(tempFileHash))
                    {
                        var myDataContainter = new DataContainer();
                        var myParser = new MeasureFileParser(myDataContainter, _myManipAnalysisGui);

                        if (myParser.ParseFile(filename))
                        {
                            _myManipAnalysisGui.WriteProgressInfo("Running multicore-calculation preparation...");

                            #region MultiCore preparation

                            var multiCoreTasks = new List<Task>();

                            int[] szenarioTrialNumbers =
                                myDataContainter.MeasureDataRaw.Select(t => t.SzenarioTrialNumber)
                                                .OrderBy(t => t)
                                                .Distinct()
                                                .ToArray();
                            int[] targetNumbers =
                                myDataContainter.MeasureDataRaw.Select(t => t.TargetNumber)
                                                .OrderBy(t => t)
                                                .Distinct()
                                                .ToArray();

                            var trialCoreDistribution = new List<int>[Environment.ProcessorCount];
                            var targetCoreDistribution = new List<int>[Environment.ProcessorCount];

                            int coreCounter = 0;
                            for (int i = 0; i < szenarioTrialNumbers.Count(); i++)
                            {
                                if (trialCoreDistribution[coreCounter] == null)
                                {
                                    trialCoreDistribution[coreCounter] = new List<int>();
                                }
                                trialCoreDistribution[coreCounter].Add(szenarioTrialNumbers[i]);

                                coreCounter++;
                                if (coreCounter >= Environment.ProcessorCount)
                                {
                                    coreCounter = 0;
                                }
                            }

                            coreCounter = 0;
                            for (int i = 0; i < targetNumbers.Count(); i++)
                            {
                                if (targetCoreDistribution[coreCounter] == null)
                                {
                                    targetCoreDistribution[coreCounter] = new List<int>();
                                }
                                targetCoreDistribution[coreCounter].Add(targetNumbers[i]);

                                coreCounter++;
                                if (coreCounter >= Environment.ProcessorCount)
                                {
                                    coreCounter = 0;
                                }
                            }

                            #endregion

                            _myManipAnalysisGui.WriteProgressInfo("Running duplicate entry detection...");

                            #region Duplicate entry detection

                            for (int core = 0; core < Environment.ProcessorCount; core++)
                            {
                                int coreVar = core;
                                multiCoreTasks.Add(new Task(
                                                       () =>
                                                       DuplicateEntryDetectionThread(trialCoreDistribution, coreVar,
                                                                                     filename,
                                                                                     myDataContainter)
                                                       ));

                                foreach (Task t in multiCoreTasks)
                                {
                                    t.Start();
                                }

                                foreach (Task t in multiCoreTasks)
                                {
                                    t.Wait();
                                }

                                multiCoreTasks.Clear();
                            }

                            #endregion

                            _myManipAnalysisGui.WriteProgressInfo("Filtering data...");

                            #region Butterworth filter

                            _myMatlabWrapper.SetWorkspaceData("filterOrder", Convert.ToDouble(butterFilterOrder));
                            _myMatlabWrapper.SetWorkspaceData("cutoffFreq", Convert.ToDouble(butterFilterCutOff));
                            _myMatlabWrapper.SetWorkspaceData("samplesPerSecond", Convert.ToDouble(samplesPerSecond));
                            _myMatlabWrapper.Execute(
                                "[b,a] = butter(filterOrder,(cutoffFreq/(samplesPerSecond/2)));");

                            for (int core = 0; core < Environment.ProcessorCount; core++)
                            {
                                int coreVar = core;
                                multiCoreTasks.Add(
                                    new Task(
                                        () => ButterWorthFilterThread(trialCoreDistribution, coreVar, myDataContainter)
                                        ));
                            }

                            foreach (Task t in multiCoreTasks)
                            {
                                t.Start();
                            }

                            foreach (Task t in multiCoreTasks)
                            {
                                t.Wait();
                            }
                            _myMatlabWrapper.ClearWorkspace();
                            multiCoreTasks.Clear();

                            #endregion Butterworth Filter

                            _myManipAnalysisGui.WriteProgressInfo("Calculating velocity...");

                            #region Velocity calcultion

                            for (int core = 0; core < Environment.ProcessorCount; core++)
                            {
                                int coreVar = core;

                                multiCoreTasks.Add(
                                    new Task(
                                        () =>
                                        VelocityCalculationThread(trialCoreDistribution, coreVar, myDataContainter,
                                                                  samplesPerSecond)
                                        ));
                            }

                            foreach (Task t in multiCoreTasks)
                            {
                                t.Start();
                            }

                            foreach (Task t in multiCoreTasks)
                            {
                                t.Wait();
                            }
                            _myMatlabWrapper.ClearWorkspace();
                            multiCoreTasks.Clear();

                            #endregion

                            _myManipAnalysisGui.WriteProgressInfo("Normalizing data...");

                            #region Time normalization

                            for (int core = 0; core < Environment.ProcessorCount; core++)
                            {
                                int coreVar = core;
                                multiCoreTasks.Add(
                                    new Task(
                                        () => TimeNormalizationThread(trialCoreDistribution, coreVar, myDataContainter,
                                                                      timeNormalizationSamples, percentPeakVelocity,
                                                                      filename)
                                        ));
                            }

                            foreach (Task t in multiCoreTasks)
                            {
                                t.Start();
                            }

                            foreach (Task t in multiCoreTasks)
                            {
                                t.Wait();
                            }
                            _myMatlabWrapper.ClearWorkspace();
                            multiCoreTasks.Clear();

                            #endregion

                            _myManipAnalysisGui.WriteProgressInfo("Calculating baselines...");

                            #region Calculate baselines

                            if (myDataContainter.SzenarioName == "Szenario02")
                            {
                                myDataContainter.BaselineData = new List<BaselineDataContainer>();

                                for (int core = 0; core < Environment.ProcessorCount; core++)
                                {
                                    int coreVar = core;
                                    multiCoreTasks.Add(
                                        new Task(
                                            () => CalculateBaselinesThread(targetCoreDistribution, coreVar,
                                                                           myDataContainter)
                                            ));
                                }

                                foreach (Task t in multiCoreTasks)
                                {
                                    t.Start();
                                }

                                foreach (Task t in multiCoreTasks)
                                {
                                    t.Wait();
                                }

                                multiCoreTasks.Clear();
                            }

                            #endregion

                            _myManipAnalysisGui.WriteProgressInfo("Calculating szenario mean times...");

                            #region Calculate szenario mean times

                            for (int core = 0; core < Environment.ProcessorCount; core++)
                            {
                                int coreVar = core;
                                multiCoreTasks.Add(
                                    new Task(
                                        () => CalculateSzenarioMeanTimesThread(targetCoreDistribution, coreVar,
                                                                               myDataContainter)
                                        ));
                            }

                            foreach (Task t in multiCoreTasks)
                            {
                                t.Start();
                            }

                            foreach (Task t in multiCoreTasks)
                            {
                                t.Wait();
                            }
                            _myMatlabWrapper.ClearWorkspace();
                            multiCoreTasks.Clear();

                            #endregion

                            #region Uploading data to SQL server

                            if (File.Exists("C:\\measureDataRaw.dat"))
                            {
                                File.Delete("C:\\measureDataRaw.dat");
                            }
                            if (File.Exists("C:\\measureDataFiltered.dat"))
                            {
                                File.Delete("C:\\measureDataFiltered.dat");
                            }
                            if (File.Exists("C:\\measureDataNormalized.dat"))
                            {
                                File.Delete("C:\\measureDataNormalized.dat");
                            }
                            if (File.Exists("C:\\velocityDataFiltered.dat"))
                            {
                                File.Delete("C:\\velocityDataFiltered.dat");
                            }
                            if (File.Exists("C:\\velocityDataNormalized.dat"))
                            {
                                File.Delete("C:\\velocityDataNormalized.dat");
                            }

                            int measureFileID =
                                _mySqlWrapper.InsertMeasureFile(
                                    DateTime.Parse(myDataContainter.MeasureFileCreationTime + " " +
                                                   myDataContainter.MeasureFileCreationDate),
                                    myDataContainter.MeasureFileHash);
                            int studyID = _mySqlWrapper.InsertStudy(myDataContainter.StudyName);
                            int szenarioID = _mySqlWrapper.InsertSzenario(myDataContainter.SzenarioName);
                            int groupID = _mySqlWrapper.InsertGroup(myDataContainter.GroupName);
                            int subjectID = _mySqlWrapper.InsertSubject(myDataContainter.SubjectName,
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

                                int targetID =
                                    _mySqlWrapper.InsertTarget(measureDataRawList.ElementAt(0).TargetNumber);

                                int targetTrialNumberID =
                                    _mySqlWrapper.InsertTargetTrialNumber(
                                        measureDataRawList.ElementAt(0).TargetTrialNumber);

                                int szenarioTrialNumberID =
                                    _mySqlWrapper.InsertSzenarioTrialNumber(szenarioTrialNumbers[iVar]);

                                int trialInformationID =
                                    _mySqlWrapper.InsertTrialInformation(
                                        measureDataRawList.ElementAt(0).ContainsDuplicates, butterFilterOrder,
                                        butterFilterCutOff,
                                        percentPeakVelocity);

                                int isCatchTrialID =
                                    _mySqlWrapper.InsertIsCatchTrial(measureDataRawList.ElementAt(0).IsCatchTrial);

                                int trialID = _mySqlWrapper.InsertTrial(
                                    subjectID,
                                    studyID,
                                    groupID,
                                    isCatchTrialID,
                                    szenarioID,
                                    targetID,
                                    targetTrialNumberID,
                                    szenarioTrialNumberID,
                                    measureFileID,
                                    trialInformationID
                                    );


                                List<string> dataFileCache =
                                    measureDataRawList.Select(
                                        t =>
                                        "," + trialID + "," + t.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss.fffffff") +
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
                                dataFileStream.Close();
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
                                            "," + trialID + "," +
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
                                    dataFileStream.Close();
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
                                            "," + trialID + "," +
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
                                    dataFileStream.Close();
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
                                            "," + trialID + "," +
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
                                    dataFileStream.Close();
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
                                            "," + trialID + "," +
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
                                    dataFileStream.Close();
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
                                int targetID =
                                    _mySqlWrapper.InsertTarget(
                                        myDataContainter.SzenarioMeanTimeData[j].TargetNumber);

                                int szenarioMeanTimeID = _mySqlWrapper.InsertSzenarioMeanTime(
                                    subjectID,
                                    studyID,
                                    groupID,
                                    targetID,
                                    szenarioID,
                                    measureFileID
                                    );

                                _mySqlWrapper.InsertSzenarioMeanTimeData(szenarioMeanTimeID,
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
                                    int targetID = _mySqlWrapper.InsertTarget(targetNumbers[j]);

                                    int baselineID = _mySqlWrapper.InsertBaseline(
                                        subjectID,
                                        studyID,
                                        groupID,
                                        targetID,
                                        szenarioID,
                                        measureFileID
                                        );

                                    List<BaselineDataContainer> baselineDataList =
                                        myDataContainter.BaselineData.Where(
                                            t => t.TargetNumber == targetNumbers[jVar])
                                                        .OrderBy(t => t.PseudoTimeStamp)
                                                        .ToList();

                                    for (int k = 0; k < baselineDataList.Count; k++)
                                    {
                                        _mySqlWrapper.InsertBaselineData(
                                            baselineID,
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

        private void DuplicateEntryDetectionThread(List<int>[] trialCoreDistribution, int coreVar, string filename,
                                                   DataContainer myDataContainter)
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
                        var diffXYZ = new List<double>();
                        for (int j = 0; j < (tempRawData.Count - 1); j++)
                        {
                            diffXYZ.Add(Math.Sqrt(
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

                        int maxIndex = diffXYZ.IndexOf(diffXYZ.Max());

                        if (
                            Math.Abs(diffXYZ.ElementAt(maxIndex) -
                                     diffXYZ.ElementAt(maxIndex - 1)) > 3)
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

        private void ButterWorthFilterThread(List<int>[] trialCoreDistribution, int coreVar,
                                             DataContainer myDataContainter)
        {
            var threadTrials = new List<int>(trialCoreDistribution[coreVar]);

            for (int i = 0; i < threadTrials.Count(); i++)
            {
                List<MeasureDataContainer> tempRawDataEnum;
                lock (myDataContainter)
                {
                    int iVar = i;
                    tempRawDataEnum =
                        new List<MeasureDataContainer>(
                            myDataContainter.MeasureDataRaw.Where(
                                t => t.ContainsDuplicates == false)
                                            .Where(
                                                t =>
                                                t.SzenarioTrialNumber ==
                                                threadTrials.ElementAt(iVar))
                                            .OrderBy(t => t.TimeStamp));
                }
                if (tempRawDataEnum.Count > 0)
                {
                    DateTime[] tempTimeStamp =
                        tempRawDataEnum.Select(t => t.TimeStamp).ToArray();
                    int tempTargetNumber =
                        tempRawDataEnum.Select(t => t.TargetNumber).ElementAt(0);
                    int tempTargetTrialNumber =
                        tempRawDataEnum.Select(t => t.TargetTrialNumber).ElementAt(0);
                    int tempSzenarioTrialNumber =
                        tempRawDataEnum.Select(t => t.SzenarioTrialNumber)
                                       .ElementAt(0);
                    bool tempIsCatchTrial =
                        tempRawDataEnum.Select(t => t.IsCatchTrial).ElementAt(0);
                    int[] tempPositionStatus =
                        tempRawDataEnum.Select(t => t.PositionStatus).ToArray();

                    _myMatlabWrapper.SetWorkspaceData(
                        "force_actual_x" + threadTrials.ElementAt(i),
                        tempRawDataEnum.Select(t => t.ForceActualX).ToArray());
                    _myMatlabWrapper.SetWorkspaceData(
                        "force_actual_y" + threadTrials.ElementAt(i),
                        tempRawDataEnum.Select(t => t.ForceActualY).ToArray());
                    _myMatlabWrapper.SetWorkspaceData(
                        "force_actual_z" + threadTrials.ElementAt(i),
                        tempRawDataEnum.Select(t => t.ForceActualZ).ToArray());

                    _myMatlabWrapper.SetWorkspaceData(
                        "force_nominal_x" + threadTrials.ElementAt(i),
                        tempRawDataEnum.Select(t => t.ForceNominalX).ToArray());
                    _myMatlabWrapper.SetWorkspaceData(
                        "force_nominal_y" + threadTrials.ElementAt(i),
                        tempRawDataEnum.Select(t => t.ForceNominalY).ToArray());
                    _myMatlabWrapper.SetWorkspaceData(
                        "force_nominal_z" + threadTrials.ElementAt(i),
                        tempRawDataEnum.Select(t => t.ForceNominalZ).ToArray());

                    _myMatlabWrapper.SetWorkspaceData(
                        "force_moment_x" + threadTrials.ElementAt(i),
                        tempRawDataEnum.Select(t => t.ForceMomentX).ToArray());
                    _myMatlabWrapper.SetWorkspaceData(
                        "force_moment_y" + threadTrials.ElementAt(i),
                        tempRawDataEnum.Select(t => t.ForceMomentY).ToArray());
                    _myMatlabWrapper.SetWorkspaceData(
                        "force_moment_z" + threadTrials.ElementAt(i),
                        tempRawDataEnum.Select(t => t.ForceMomentZ).ToArray());

                    _myMatlabWrapper.SetWorkspaceData(
                        "position_cartesian_x" + threadTrials.ElementAt(i),
                        tempRawDataEnum.Select(t => t.PositionCartesianX).ToArray());
                    _myMatlabWrapper.SetWorkspaceData(
                        "position_cartesian_y" + threadTrials.ElementAt(i),
                        tempRawDataEnum.Select(t => t.PositionCartesianY).ToArray());
                    _myMatlabWrapper.SetWorkspaceData(
                        "position_cartesian_z" + threadTrials.ElementAt(i),
                        tempRawDataEnum.Select(t => t.PositionCartesianZ).ToArray());

                    _myMatlabWrapper.Execute("force_actual_x" +
                                             threadTrials.ElementAt(i) +
                                             " = filtfilt(b, a, force_actual_x" +
                                             threadTrials.ElementAt(i) + ");");
                    _myMatlabWrapper.Execute("force_actual_y" +
                                             threadTrials.ElementAt(i) +
                                             " = filtfilt(b, a, force_actual_y" +
                                             threadTrials.ElementAt(i) + ");");
                    _myMatlabWrapper.Execute("force_actual_z" +
                                             threadTrials.ElementAt(i) +
                                             " = filtfilt(b, a, force_actual_z" +
                                             threadTrials.ElementAt(i) + ");");

                    _myMatlabWrapper.Execute("force_nominal_x" +
                                             threadTrials.ElementAt(i) +
                                             " = filtfilt(b, a,force_nominal_x" +
                                             threadTrials.ElementAt(i) + ");");
                    _myMatlabWrapper.Execute("force_nominal_y" +
                                             threadTrials.ElementAt(i) +
                                             " = filtfilt(b, a,force_nominal_y" +
                                             threadTrials.ElementAt(i) + ");");
                    _myMatlabWrapper.Execute("force_nominal_z" +
                                             threadTrials.ElementAt(i) +
                                             " = filtfilt(b, a,force_nominal_z" +
                                             threadTrials.ElementAt(i) + ");");

                    _myMatlabWrapper.Execute("force_moment_x" +
                                             threadTrials.ElementAt(i) +
                                             " = filtfilt(b, a, force_moment_x" +
                                             threadTrials.ElementAt(i) + ");");
                    _myMatlabWrapper.Execute("force_moment_y" +
                                             threadTrials.ElementAt(i) +
                                             " = filtfilt(b, a, force_moment_y" +
                                             threadTrials.ElementAt(i) + ");");
                    _myMatlabWrapper.Execute("force_moment_z" +
                                             threadTrials.ElementAt(i) +
                                             " = filtfilt(b, a, force_moment_z" +
                                             threadTrials.ElementAt(i) + ");");

                    _myMatlabWrapper.Execute("position_cartesian_x" +
                                             threadTrials.ElementAt(i) +
                                             " = filtfilt(b, a, position_cartesian_x" +
                                             threadTrials.ElementAt(i) + ");");
                    _myMatlabWrapper.Execute("position_cartesian_y" +
                                             threadTrials.ElementAt(i) +
                                             " = filtfilt(b, a, position_cartesian_y" +
                                             threadTrials.ElementAt(i) + ");");
                    _myMatlabWrapper.Execute("position_cartesian_z" +
                                             threadTrials.ElementAt(i) +
                                             " = filtfilt(b, a, position_cartesian_z" +
                                             threadTrials.ElementAt(i) + ");");


                    double[,] forceActualX =
                        _myMatlabWrapper.GetWorkspaceData("force_actual_x" +
                                                          threadTrials.ElementAt(i));
                    double[,] forceActualY =
                        _myMatlabWrapper.GetWorkspaceData("force_actual_y" +
                                                          threadTrials.ElementAt(i));
                    double[,] forceActualZ =
                        _myMatlabWrapper.GetWorkspaceData("force_actual_z" +
                                                          threadTrials.ElementAt(i));

                    double[,] forceNominalX =
                        _myMatlabWrapper.GetWorkspaceData("force_nominal_x" +
                                                          threadTrials.ElementAt(i));
                    double[,] forceNominalY =
                        _myMatlabWrapper.GetWorkspaceData("force_nominal_y" +
                                                          threadTrials.ElementAt(i));
                    double[,] forceNominalZ =
                        _myMatlabWrapper.GetWorkspaceData("force_nominal_z" +
                                                          threadTrials.ElementAt(i));

                    double[,] forceMomentX =
                        _myMatlabWrapper.GetWorkspaceData("force_moment_x" +
                                                          threadTrials.ElementAt(i));
                    double[,] forceMomentY =
                        _myMatlabWrapper.GetWorkspaceData("force_moment_y" +
                                                          threadTrials.ElementAt(i));
                    double[,] forceMomentZ =
                        _myMatlabWrapper.GetWorkspaceData("force_moment_z" +
                                                          threadTrials.ElementAt(i));

                    double[,] positionCartesianX =
                        _myMatlabWrapper.GetWorkspaceData("position_cartesian_x" +
                                                          threadTrials.ElementAt(i));
                    double[,] positionCartesianY =
                        _myMatlabWrapper.GetWorkspaceData("position_cartesian_y" +
                                                          threadTrials.ElementAt(i));
                    double[,] positionCartesianZ =
                        _myMatlabWrapper.GetWorkspaceData("position_cartesian_z" +
                                                          threadTrials.ElementAt(i));


                    for (int j = 0; j < forceActualX.Length; j++)
                    {
                        lock (myDataContainter)
                        {
                            myDataContainter.MeasureDataFiltered.Add(new MeasureDataContainer
                                                                         (
                                                                         tempTimeStamp[j
                                                                             ],
                                                                         forceActualX[
                                                                             0, j],
                                                                         forceActualY[
                                                                             0, j],
                                                                         forceActualZ[
                                                                             0, j],
                                                                         forceNominalX[
                                                                             0, j],
                                                                         forceNominalY[
                                                                             0, j],
                                                                         forceNominalZ[
                                                                             0, j],
                                                                         forceMomentX[
                                                                             0, j],
                                                                         forceMomentY[
                                                                             0, j],
                                                                         forceMomentZ[
                                                                             0, j],
                                                                         positionCartesianX
                                                                             [0, j],
                                                                         positionCartesianY
                                                                             [0, j],
                                                                         positionCartesianZ
                                                                             [0, j],
                                                                         tempTargetNumber,
                                                                         tempTargetTrialNumber,
                                                                         tempSzenarioTrialNumber,
                                                                         tempIsCatchTrial,
                                                                         tempPositionStatus
                                                                             [j]
                                                                         ));
                        }
                    }

                    _myMatlabWrapper.ClearWorkspaceData("force_actual_x" +
                                                        threadTrials.ElementAt(i));
                    _myMatlabWrapper.ClearWorkspaceData("force_actual_y" +
                                                        threadTrials.ElementAt(i));
                    _myMatlabWrapper.ClearWorkspaceData("force_actual_z" +
                                                        threadTrials.ElementAt(i));

                    _myMatlabWrapper.ClearWorkspaceData("force_nominal_x" +
                                                        threadTrials.ElementAt(i));
                    _myMatlabWrapper.ClearWorkspaceData("force_nominal_y" +
                                                        threadTrials.ElementAt(i));
                    _myMatlabWrapper.ClearWorkspaceData("force_nominal_z" +
                                                        threadTrials.ElementAt(i));

                    _myMatlabWrapper.ClearWorkspaceData("force_moment_x" +
                                                        threadTrials.ElementAt(i));
                    _myMatlabWrapper.ClearWorkspaceData("force_moment_y" +
                                                        threadTrials.ElementAt(i));
                    _myMatlabWrapper.ClearWorkspaceData("force_moment_z" +
                                                        threadTrials.ElementAt(i));

                    _myMatlabWrapper.ClearWorkspaceData("position_cartesian_x" +
                                                        threadTrials.ElementAt(i));
                    _myMatlabWrapper.ClearWorkspaceData("position_cartesian_y" +
                                                        threadTrials.ElementAt(i));
                    _myMatlabWrapper.ClearWorkspaceData("position_cartesian_z" +
                                                        threadTrials.ElementAt(i));
                }
            }
        }

        private void VelocityCalculationThread(List<int>[] trialCoreDistribution, int coreVar,
                                               DataContainer myDataContainter, int samplesPerSecond)
        {
            var threadTrials = new List<int>(trialCoreDistribution[coreVar]);

            for (int i = 0; i < threadTrials.Count(); i++)
            {
                List<MeasureDataContainer> tempFilteredDataEnum;
                lock (myDataContainter)
                {
                    int iVar = i;
                    tempFilteredDataEnum =
                        new List<MeasureDataContainer>(
                            myDataContainter.MeasureDataFiltered.Where(
                                t =>
                                t.SzenarioTrialNumber == threadTrials.ElementAt(iVar))
                                            .OrderBy(t => t.TimeStamp));
                }
                if (tempFilteredDataEnum.Count > 0)
                {
                    _myMatlabWrapper.SetWorkspaceData(
                        "time_stamp" + threadTrials.ElementAt(i),
                        tempFilteredDataEnum.Select(
                            t => Convert.ToDouble(t.TimeStamp.Ticks)).ToArray());
                    _myMatlabWrapper.SetWorkspaceData(
                        "position_cartesian_x" + threadTrials.ElementAt(i),
                        tempFilteredDataEnum.Select(t => t.PositionCartesianX)
                                            .ToArray());
                    _myMatlabWrapper.SetWorkspaceData(
                        "position_cartesian_y" + threadTrials.ElementAt(i),
                        tempFilteredDataEnum.Select(t => t.PositionCartesianY)
                                            .ToArray());
                    _myMatlabWrapper.SetWorkspaceData(
                        "position_cartesian_z" + threadTrials.ElementAt(i),
                        tempFilteredDataEnum.Select(t => t.PositionCartesianZ)
                                            .ToArray());
                    _myMatlabWrapper.SetWorkspaceData("sampleRate",
                                                      (1.0/
                                                       Convert.ToDouble(
                                                           samplesPerSecond)));

                    _myMatlabWrapper.Execute("time_stamp" + threadTrials.ElementAt(i) +
                                             " = time_stamp" + threadTrials.ElementAt(i) +
                                             "(1:end-1) +  (diff(time_stamp" +
                                             threadTrials.ElementAt(i) + ") ./ 2);");
                    _myMatlabWrapper.Execute("velocity_x" + threadTrials.ElementAt(i) +
                                             " = diff(position_cartesian_x" +
                                             threadTrials.ElementAt(i) +
                                             ") ./ sampleRate;");
                    _myMatlabWrapper.Execute("velocity_y" + threadTrials.ElementAt(i) +
                                             " = diff(position_cartesian_y" +
                                             threadTrials.ElementAt(i) +
                                             ") ./ sampleRate;");
                    _myMatlabWrapper.Execute("velocity_z" + threadTrials.ElementAt(i) +
                                             " = diff(position_cartesian_z" +
                                             threadTrials.ElementAt(i) +
                                             ") ./ sampleRate;");

                    double[,] timeStamp =
                        _myMatlabWrapper.GetWorkspaceData("time_stamp" +
                                                          threadTrials.ElementAt(i));
                    double[,] velocityX =
                        _myMatlabWrapper.GetWorkspaceData("velocity_x" +
                                                          threadTrials.ElementAt(i));
                    double[,] velocityY =
                        _myMatlabWrapper.GetWorkspaceData("velocity_y" +
                                                          threadTrials.ElementAt(i));
                    double[,] velocityZ =
                        _myMatlabWrapper.GetWorkspaceData("velocity_z" +
                                                          threadTrials.ElementAt(i));

                    for (int j = 0; j < velocityX.Length; j++)
                    {
                        lock (myDataContainter)
                        {
                            myDataContainter.VelocityDataFiltered.Add(new VelocityDataContainer
                                                                          (
                                                                          new DateTime(
                                                                              Convert
                                                                                  .ToInt64
                                                                                  (timeStamp
                                                                                       [
                                                                                           0,
                                                                                           j
                                                                                       ])),
                                                                          velocityX[0, j
                                                                              ],
                                                                          velocityY[0, j
                                                                              ],
                                                                          velocityZ[0, j
                                                                              ],
                                                                          threadTrials
                                                                              .ElementAt
                                                                              (i),
                                                                          tempFilteredDataEnum
                                                                              .ElementAt
                                                                              (0)
                                                                              .TargetNumber,
                                                                          tempFilteredDataEnum
                                                                              .ElementAt
                                                                              (j)
                                                                              .PositionStatus
                                                                          ));
                        }
                    }

                    _myMatlabWrapper.ClearWorkspaceData("time_stamp" +
                                                        threadTrials.ElementAt(i));
                    _myMatlabWrapper.ClearWorkspaceData("velocity_x" +
                                                        threadTrials.ElementAt(i));
                    _myMatlabWrapper.ClearWorkspaceData("velocity_y" +
                                                        threadTrials.ElementAt(i));
                    _myMatlabWrapper.ClearWorkspaceData("velocity_z" +
                                                        threadTrials.ElementAt(i));
                    _myMatlabWrapper.ClearWorkspaceData("position_cartesian_x" +
                                                        threadTrials.ElementAt(i));
                    _myMatlabWrapper.ClearWorkspaceData("position_cartesian_y" +
                                                        threadTrials.ElementAt(i));
                    _myMatlabWrapper.ClearWorkspaceData("position_cartesian_z" +
                                                        threadTrials.ElementAt(i));
                }
            }
        }

        private void TimeNormalizationThread(List<int>[] trialCoreDistribution, int coreVar,
                                             DataContainer myDataContainter, int timeNormalizationSamples,
                                             int percentPeakVelocity, string filename)
        {
            var threadTrials = new List<int>(trialCoreDistribution[coreVar]);
            _myMatlabWrapper.SetWorkspaceData("newSampleCount",
                                              Convert.ToDouble(
                                                  timeNormalizationSamples));

            for (int i = 0; i < threadTrials.Count(); i++)
            {
                List<MeasureDataContainer> tempFilteredDataEnum;
                List<VelocityDataContainer> tempVelocityDataEnum;

                lock (myDataContainter)
                {
                    int iVar = i;
                    tempFilteredDataEnum =
                        new List<MeasureDataContainer>(
                            myDataContainter.MeasureDataFiltered.Where(
                                t =>
                                t.SzenarioTrialNumber == threadTrials.ElementAt(iVar))
                                            .Where(t => t.PositionStatus == 1)
                                            .OrderBy(t => t.TimeStamp));
                    if (
                        myDataContainter.MeasureDataFiltered.Where(
                            t => t.SzenarioTrialNumber == threadTrials.ElementAt(iVar))
                                        .Any(t => t.PositionStatus == 0))
                    {
                        tempFilteredDataEnum.Insert(0,
                                                    myDataContainter.MeasureDataFiltered
                                                                    .Where(
                                                                        t =>
                                                                        t
                                                                            .SzenarioTrialNumber ==
                                                                        threadTrials
                                                                            .ElementAt(i))
                                                                    .Where(
                                                                        t =>
                                                                        t
                                                                            .PositionStatus ==
                                                                        0)
                                                                    .OrderBy(
                                                                        t =>
                                                                        t.TimeStamp)
                                                                    .Last());
                    }

                    tempVelocityDataEnum =
                        new List<VelocityDataContainer>(
                            myDataContainter.VelocityDataFiltered.Where(
                                t =>
                                t.SzenarioTrialNumber == threadTrials.ElementAt(iVar))
                                            .OrderBy(t => t.TimeStamp));
                }

                if ((tempFilteredDataEnum.Count > 0) && (tempVelocityDataEnum.Count > 0))
                {
                    var tempVelocityDataEnumCropped = new List<VelocityDataContainer>();

                    if (percentPeakVelocity > 0)
                    {
                        double velocityCropThreshold =
                            tempVelocityDataEnum.Max(
                                t =>
                                Math.Sqrt(Math.Pow(t.VelocityX, 2) +
                                          Math.Pow(t.VelocityZ, 2)))/100.0*
                            percentPeakVelocity;

                        if (
                            tempVelocityDataEnum.Where(t => t.PositionStatus == 0)
                                                .Count(
                                                    t =>
                                                    Math.Sqrt(
                                                        Math.Pow(t.VelocityX, 2) +
                                                        Math.Pow(t.VelocityZ, 2)) >
                                                    velocityCropThreshold) > 0)
                        {
                            DateTime startTime =
                                tempVelocityDataEnum.Where(
                                    t =>
                                    Math.Sqrt(Math.Pow(t.VelocityX, 2) +
                                              Math.Pow(t.VelocityZ, 2)) >
                                    velocityCropThreshold)
                                                    .OrderBy(t => t.TimeStamp)
                                                    .First()
                                                    .TimeStamp;
                            tempVelocityDataEnumCropped.AddRange(
                                tempVelocityDataEnum.Where(t => t.PositionStatus == 0)
                                                    .Where(
                                                        t => t.TimeStamp >= startTime));
                        }

                        tempVelocityDataEnumCropped.AddRange(
                            tempVelocityDataEnum.Where(t => t.PositionStatus == 1));

                        if (
                            tempVelocityDataEnum.Where(t => t.PositionStatus == 2)
                                                .Count(
                                                    t =>
                                                    Math.Sqrt(
                                                        Math.Pow(t.VelocityX, 2) +
                                                        Math.Pow(t.VelocityZ, 2)) <
                                                    velocityCropThreshold) > 0)
                        {
                            DateTime stopTime =
                                tempVelocityDataEnum.Where(t => t.PositionStatus == 2)
                                                    .Where(
                                                        t =>
                                                        Math.Sqrt(
                                                            Math.Pow(t.VelocityX, 2) +
                                                            Math.Pow(t.VelocityZ, 2)) <
                                                        velocityCropThreshold)
                                                    .OrderBy(t => t.TimeStamp)
                                                    .First()
                                                    .TimeStamp;
                            tempVelocityDataEnumCropped.AddRange(
                                tempVelocityDataEnum.Where(t => t.PositionStatus > 1)
                                                    .Where(t => t.TimeStamp <= stopTime));
                        }
                        else
                        {
                            if (tempVelocityDataEnum.Exists(t => t.PositionStatus == 3))
                            {
                                DateTime stopTime =
                                    tempVelocityDataEnum.Where(
                                        t => t.PositionStatus == 3)
                                                        .OrderBy(t => t.TimeStamp)
                                                        .Last()
                                                        .TimeStamp;
                                tempVelocityDataEnumCropped.AddRange(
                                    tempVelocityDataEnum.Where(
                                        t => t.TimeStamp <= stopTime));
                            }
                        }

                        tempVelocityDataEnum =
                            tempVelocityDataEnumCropped.OrderBy(t => t.TimeStamp)
                                                       .ToList();
                    }
                    else if (percentPeakVelocity < 0)
                    {
                        if (tempVelocityDataEnum.Exists(t => t.PositionStatus == 3))
                        {
                            DateTime stopTime =
                                tempVelocityDataEnum.Where(t => t.PositionStatus == 3)
                                                    .OrderBy(t => t.TimeStamp)
                                                    .Last()
                                                    .TimeStamp;
                            tempVelocityDataEnumCropped =
                                tempVelocityDataEnum.Where(t => t.PositionStatus > 0)
                                                    .Where(t => t.TimeStamp <= stopTime)
                                                    .ToList();
                        }
                        else
                        {
                            tempVelocityDataEnumCropped =
                                tempVelocityDataEnum.Where(t => t.PositionStatus == 1)
                                                    .ToList();
                        }

                        tempVelocityDataEnum =
                            tempVelocityDataEnumCropped.OrderBy(t => t.TimeStamp)
                                                       .ToList();
                    }


                    int tempTargetNumber =
                        tempFilteredDataEnum.Select(t => t.TargetNumber).ElementAt(0);
                    int tempTargetTrialNumber =
                        tempFilteredDataEnum.Select(t => t.TargetTrialNumber)
                                            .ElementAt(0);
                    int tempSzenarioTrialNumber =
                        tempFilteredDataEnum.Select(t => t.SzenarioTrialNumber)
                                            .ElementAt(0);
                    bool tempIsCatchTrial =
                        tempFilteredDataEnum.Select(t => t.IsCatchTrial).ElementAt(0);
                    var errorList = new List<string>();

                    _myMatlabWrapper.SetWorkspaceData(
                        "measure_data_time" + threadTrials.ElementAt(i),
                        tempFilteredDataEnum.OrderBy(t => t.TimeStamp)
                                            .Select(
                                                t =>
                                                Convert.ToDouble(t.TimeStamp.Ticks))
                                            .ToArray());

                    _myMatlabWrapper.SetWorkspaceData(
                        "forceActualX" + threadTrials.ElementAt(i),
                        tempFilteredDataEnum.OrderBy(t => t.TimeStamp)
                                            .Select(t => t.ForceActualX)
                                            .ToArray());
                    _myMatlabWrapper.SetWorkspaceData(
                        "forceActualY" + threadTrials.ElementAt(i),
                        tempFilteredDataEnum.OrderBy(t => t.TimeStamp)
                                            .Select(t => t.ForceActualY)
                                            .ToArray());
                    _myMatlabWrapper.SetWorkspaceData(
                        "forceActualZ" + threadTrials.ElementAt(i),
                        tempFilteredDataEnum.OrderBy(t => t.TimeStamp)
                                            .Select(t => t.ForceActualZ)
                                            .ToArray());

                    _myMatlabWrapper.SetWorkspaceData(
                        "forceNominalX" + threadTrials.ElementAt(i),
                        tempFilteredDataEnum.OrderBy(t => t.TimeStamp)
                                            .Select(t => t.ForceNominalX)
                                            .ToArray());
                    _myMatlabWrapper.SetWorkspaceData(
                        "forceNominalY" + threadTrials.ElementAt(i),
                        tempFilteredDataEnum.OrderBy(t => t.TimeStamp)
                                            .Select(t => t.ForceNominalX)
                                            .ToArray());
                    _myMatlabWrapper.SetWorkspaceData(
                        "forceNominalZ" + threadTrials.ElementAt(i),
                        tempFilteredDataEnum.OrderBy(t => t.TimeStamp)
                                            .Select(t => t.ForceNominalX)
                                            .ToArray());

                    _myMatlabWrapper.SetWorkspaceData(
                        "forceMomentX" + threadTrials.ElementAt(i),
                        tempFilteredDataEnum.OrderBy(t => t.TimeStamp)
                                            .Select(t => t.ForceMomentX)
                                            .ToArray());
                    _myMatlabWrapper.SetWorkspaceData(
                        "forceMomentY" + threadTrials.ElementAt(i),
                        tempFilteredDataEnum.OrderBy(t => t.TimeStamp)
                                            .Select(t => t.ForceMomentX)
                                            .ToArray());
                    _myMatlabWrapper.SetWorkspaceData(
                        "forceMomentZ" + threadTrials.ElementAt(i),
                        tempFilteredDataEnum.OrderBy(t => t.TimeStamp)
                                            .Select(t => t.ForceMomentX)
                                            .ToArray());

                    _myMatlabWrapper.SetWorkspaceData(
                        "positionCartesianX" + threadTrials.ElementAt(i),
                        tempFilteredDataEnum.OrderBy(t => t.TimeStamp)
                                            .Select(t => t.PositionCartesianX)
                                            .ToArray());
                    _myMatlabWrapper.SetWorkspaceData(
                        "positionCartesianY" + threadTrials.ElementAt(i),
                        tempFilteredDataEnum.OrderBy(t => t.TimeStamp)
                                            .Select(t => t.PositionCartesianY)
                                            .ToArray());
                    _myMatlabWrapper.SetWorkspaceData(
                        "positionCartesianZ" + threadTrials.ElementAt(i),
                        tempFilteredDataEnum.OrderBy(t => t.TimeStamp)
                                            .Select(t => t.PositionCartesianZ)
                                            .ToArray());

                    _myMatlabWrapper.SetWorkspaceData(
                        "positionStatus" + threadTrials.ElementAt(i),
                        tempFilteredDataEnum.OrderBy(t => t.TimeStamp)
                                            .Select(
                                                t => Convert.ToDouble(t.PositionStatus))
                                            .ToArray());

                    _myMatlabWrapper.SetWorkspaceData(
                        "velocity_data_time" + threadTrials.ElementAt(i),
                        tempVelocityDataEnum.OrderBy(t => t.TimeStamp)
                                            .Select(
                                                t =>
                                                Convert.ToDouble(t.TimeStamp.Ticks))
                                            .ToArray());

                    _myMatlabWrapper.SetWorkspaceData(
                        "velocityX" + threadTrials.ElementAt(i),
                        tempVelocityDataEnum.OrderBy(t => t.TimeStamp)
                                            .Select(t => t.VelocityX)
                                            .ToArray());
                    _myMatlabWrapper.SetWorkspaceData(
                        "velocityY" + threadTrials.ElementAt(i),
                        tempVelocityDataEnum.OrderBy(t => t.TimeStamp)
                                            .Select(t => t.VelocityY)
                                            .ToArray());
                    _myMatlabWrapper.SetWorkspaceData(
                        "velocityZ" + threadTrials.ElementAt(i),
                        tempVelocityDataEnum.OrderBy(t => t.TimeStamp)
                                            .Select(t => t.VelocityZ)
                                            .ToArray());

                    //-----

                    _myMatlabWrapper.Execute("[errorvar1_" + threadTrials.ElementAt(i) +
                                             ", forceActualX" +
                                             threadTrials.ElementAt(i) +
                                             ",newMeasureTime" +
                                             threadTrials.ElementAt(i) +
                                             "] = timeNorm(forceActualX" +
                                             threadTrials.ElementAt(i) +
                                             ",measure_data_time" +
                                             threadTrials.ElementAt(i) +
                                             ",newSampleCount);");
                    _myMatlabWrapper.Execute("[errorvar2_" + threadTrials.ElementAt(i) +
                                             ", forceActualY" +
                                             threadTrials.ElementAt(i) +
                                             ",newMeasureTime" +
                                             threadTrials.ElementAt(i) +
                                             "] = timeNorm(forceActualY" +
                                             threadTrials.ElementAt(i) +
                                             ",measure_data_time" +
                                             threadTrials.ElementAt(i) +
                                             ",newSampleCount);");
                    _myMatlabWrapper.Execute("[errorvar3_" + threadTrials.ElementAt(i) +
                                             ", forceActualZ" +
                                             threadTrials.ElementAt(i) +
                                             ",newMeasureTime" +
                                             threadTrials.ElementAt(i) +
                                             "] = timeNorm(forceActualZ" +
                                             threadTrials.ElementAt(i) +
                                             ",measure_data_time" +
                                             threadTrials.ElementAt(i) +
                                             ",newSampleCount);");

                    _myMatlabWrapper.Execute("[errorvar4_" + threadTrials.ElementAt(i) +
                                             ", forceNominalX" +
                                             threadTrials.ElementAt(i) +
                                             ",newMeasureTime" +
                                             threadTrials.ElementAt(i) +
                                             "] = timeNorm(forceNominalX" +
                                             threadTrials.ElementAt(i) +
                                             ",measure_data_time" +
                                             threadTrials.ElementAt(i) +
                                             ",newSampleCount);");
                    _myMatlabWrapper.Execute("[errorvar5_" + threadTrials.ElementAt(i) +
                                             ", forceNominalY" +
                                             threadTrials.ElementAt(i) +
                                             ",newMeasureTime" +
                                             threadTrials.ElementAt(i) +
                                             "] = timeNorm(forceNominalY" +
                                             threadTrials.ElementAt(i) +
                                             ",measure_data_time" +
                                             threadTrials.ElementAt(i) +
                                             ",newSampleCount);");
                    _myMatlabWrapper.Execute("[errorvar6_" + threadTrials.ElementAt(i) +
                                             ", forceNominalZ" +
                                             threadTrials.ElementAt(i) +
                                             ",newMeasureTime" +
                                             threadTrials.ElementAt(i) +
                                             "] = timeNorm(forceNominalZ" +
                                             threadTrials.ElementAt(i) +
                                             ",measure_data_time" +
                                             threadTrials.ElementAt(i) +
                                             ",newSampleCount);");

                    _myMatlabWrapper.Execute("[errorvar7_" + threadTrials.ElementAt(i) +
                                             ", forceMomentX" +
                                             threadTrials.ElementAt(i) +
                                             ",newMeasureTime" +
                                             threadTrials.ElementAt(i) +
                                             "] = timeNorm(forceMomentX" +
                                             threadTrials.ElementAt(i) +
                                             ",measure_data_time" +
                                             threadTrials.ElementAt(i) +
                                             ",newSampleCount);");
                    _myMatlabWrapper.Execute("[errorvar8_" + threadTrials.ElementAt(i) +
                                             ", forceMomentY" +
                                             threadTrials.ElementAt(i) +
                                             ",newMeasureTime" +
                                             threadTrials.ElementAt(i) +
                                             "] = timeNorm(forceMomentY" +
                                             threadTrials.ElementAt(i) +
                                             ",measure_data_time" +
                                             threadTrials.ElementAt(i) +
                                             ",newSampleCount);");
                    _myMatlabWrapper.Execute("[errorvar9_" + threadTrials.ElementAt(i) +
                                             ", forceMomentZ" +
                                             threadTrials.ElementAt(i) +
                                             ",newMeasureTime" +
                                             threadTrials.ElementAt(i) +
                                             "] = timeNorm(forceMomentZ" +
                                             threadTrials.ElementAt(i) +
                                             ",measure_data_time" +
                                             threadTrials.ElementAt(i) +
                                             ",newSampleCount);");

                    _myMatlabWrapper.Execute("[errorvar10_" + threadTrials.ElementAt(i) +
                                             ", positionCartesianX" +
                                             threadTrials.ElementAt(i) +
                                             ",newMeasureTime" +
                                             threadTrials.ElementAt(i) +
                                             "] = timeNorm(positionCartesianX" +
                                             threadTrials.ElementAt(i) +
                                             ",measure_data_time" +
                                             threadTrials.ElementAt(i) +
                                             ",newSampleCount);");
                    _myMatlabWrapper.Execute("[errorvar11_" + threadTrials.ElementAt(i) +
                                             ", positionCartesianY" +
                                             threadTrials.ElementAt(i) +
                                             ",newMeasureTime" +
                                             threadTrials.ElementAt(i) +
                                             "] = timeNorm(positionCartesianY" +
                                             threadTrials.ElementAt(i) +
                                             ",measure_data_time" +
                                             threadTrials.ElementAt(i) +
                                             ",newSampleCount);");
                    _myMatlabWrapper.Execute("[errorvar12_" + threadTrials.ElementAt(i) +
                                             ", positionCartesianZ" +
                                             threadTrials.ElementAt(i) +
                                             ",newMeasureTime" +
                                             threadTrials.ElementAt(i) +
                                             "] = timeNorm(positionCartesianZ" +
                                             threadTrials.ElementAt(i) +
                                             ",measure_data_time" +
                                             threadTrials.ElementAt(i) +
                                             ",newSampleCount);");

                    _myMatlabWrapper.Execute("[errorvar13_" + threadTrials.ElementAt(i) +
                                             ", positionStatus" +
                                             threadTrials.ElementAt(i) +
                                             ",newMeasureTime" +
                                             threadTrials.ElementAt(i) +
                                             "] = timeNorm(positionStatus" +
                                             threadTrials.ElementAt(i) +
                                             ",measure_data_time" +
                                             threadTrials.ElementAt(i) +
                                             ",newSampleCount);");

                    _myMatlabWrapper.Execute("[errorvar14_" + threadTrials.ElementAt(i) +
                                             ", velocityX" + threadTrials.ElementAt(i) +
                                             ",newVelocityTime" +
                                             threadTrials.ElementAt(i) +
                                             "] = timeNorm(velocityX" +
                                             threadTrials.ElementAt(i) +
                                             ",velocity_data_time" +
                                             threadTrials.ElementAt(i) +
                                             ",newSampleCount);");
                    _myMatlabWrapper.Execute("[errorvar15_" + threadTrials.ElementAt(i) +
                                             ", velocityY" + threadTrials.ElementAt(i) +
                                             ",newVelocityTime" +
                                             threadTrials.ElementAt(i) +
                                             "] = timeNorm(velocityY" +
                                             threadTrials.ElementAt(i) +
                                             ",velocity_data_time" +
                                             threadTrials.ElementAt(i) +
                                             ",newSampleCount);");
                    _myMatlabWrapper.Execute("[errorvar16_" + threadTrials.ElementAt(i) +
                                             ", velocityZ" + threadTrials.ElementAt(i) +
                                             ",newVelocityTime" +
                                             threadTrials.ElementAt(i) +
                                             "] = timeNorm(velocityZ" +
                                             threadTrials.ElementAt(i) +
                                             ",velocity_data_time" +
                                             threadTrials.ElementAt(i) +
                                             ",newSampleCount);");

                    //-----

                    for (int errorVarCounterCounter = 1;
                         errorVarCounterCounter <= 16;
                         errorVarCounterCounter++)
                    {
                        errorList.Add(
                            Convert.ToString(
                                _myMatlabWrapper.GetWorkspaceData("errorvar" +
                                                                  errorVarCounterCounter +
                                                                  "_" +
                                                                  threadTrials.ElementAt
                                                                      (i))));
                        _myMatlabWrapper.ClearWorkspaceData("errorvar" +
                                                            errorVarCounterCounter + "_" +
                                                            threadTrials.ElementAt(i));
                    }

                    if (errorList.Any(t => !string.IsNullOrEmpty(t)))
                    {
                        string output =
                            errorList.Where(t => !string.IsNullOrEmpty(t))
                                     .Select(
                                         t =>
                                         t + " in " + filename + " at szenario-trial-number " + tempSzenarioTrialNumber)
                                     .Aggregate("", (current, line) => current + line);
                        _myManipAnalysisGui.WriteToLogBox(output);
                    }

                    //-----

                    double[,] measureDataTime =
                        _myMatlabWrapper.GetWorkspaceData("newMeasureTime" +
                                                          threadTrials.ElementAt(i));
                    double[,] velocityDataTime =
                        _myMatlabWrapper.GetWorkspaceData("newVelocityTime" +
                                                          threadTrials.ElementAt(i));

                    double[,] forceActualX =
                        _myMatlabWrapper.GetWorkspaceData("forceActualX" +
                                                          threadTrials.ElementAt(i));
                    double[,] forceActualY =
                        _myMatlabWrapper.GetWorkspaceData("forceActualY" +
                                                          threadTrials.ElementAt(i));
                    double[,] forceActualZ =
                        _myMatlabWrapper.GetWorkspaceData("forceActualZ" +
                                                          threadTrials.ElementAt(i));

                    double[,] forceNominalX =
                        _myMatlabWrapper.GetWorkspaceData("forceNominalX" +
                                                          threadTrials.ElementAt(i));
                    double[,] forceNominalY =
                        _myMatlabWrapper.GetWorkspaceData("forceNominalY" +
                                                          threadTrials.ElementAt(i));
                    double[,] forceNominalZ =
                        _myMatlabWrapper.GetWorkspaceData("forceNominalZ" +
                                                          threadTrials.ElementAt(i));

                    double[,] forceMomentX =
                        _myMatlabWrapper.GetWorkspaceData("forceMomentX" +
                                                          threadTrials.ElementAt(i));
                    double[,] forceMomentY =
                        _myMatlabWrapper.GetWorkspaceData("forceMomentY" +
                                                          threadTrials.ElementAt(i));
                    double[,] forceMomentZ =
                        _myMatlabWrapper.GetWorkspaceData("forceMomentZ" +
                                                          threadTrials.ElementAt(i));

                    double[,] positionCartesianX =
                        _myMatlabWrapper.GetWorkspaceData("positionCartesianX" +
                                                          threadTrials.ElementAt(i));
                    double[,] positionCartesianY =
                        _myMatlabWrapper.GetWorkspaceData("positionCartesianY" +
                                                          threadTrials.ElementAt(i));
                    double[,] positionCartesianZ =
                        _myMatlabWrapper.GetWorkspaceData("positionCartesianZ" +
                                                          threadTrials.ElementAt(i));

                    double[,] positionStatus =
                        _myMatlabWrapper.GetWorkspaceData("positionStatus" +
                                                          threadTrials.ElementAt(i));

                    double[,] velocityX =
                        _myMatlabWrapper.GetWorkspaceData("velocityX" +
                                                          threadTrials.ElementAt(i));
                    double[,] velocityY =
                        _myMatlabWrapper.GetWorkspaceData("velocityY" +
                                                          threadTrials.ElementAt(i));
                    double[,] velocityZ =
                        _myMatlabWrapper.GetWorkspaceData("velocityZ" +
                                                          threadTrials.ElementAt(i));

                    //-----

                    for (int j = 0; j < measureDataTime.Length; j++)
                    {
                        lock (myDataContainter)
                        {
                            myDataContainter.MeasureDataNormalized.Add(new MeasureDataContainer
                                                                           (
                                                                           new DateTime(
                                                                               Convert
                                                                                   .ToInt64
                                                                                   (measureDataTime
                                                                                        [
                                                                                            j,
                                                                                            0
                                                                                        ])),
                                                                           forceActualX[
                                                                               j, 0],
                                                                           forceActualY[
                                                                               j, 0],
                                                                           forceActualZ[
                                                                               j, 0],
                                                                           forceNominalX
                                                                               [j, 0],
                                                                           forceNominalY
                                                                               [j, 0],
                                                                           forceNominalZ
                                                                               [j, 0],
                                                                           forceMomentX[
                                                                               j, 0],
                                                                           forceMomentY[
                                                                               j, 0],
                                                                           forceMomentZ[
                                                                               j, 0],
                                                                           positionCartesianX
                                                                               [j, 0],
                                                                           positionCartesianY
                                                                               [j, 0],
                                                                           positionCartesianZ
                                                                               [j, 0],
                                                                           tempTargetNumber,
                                                                           tempTargetTrialNumber,
                                                                           tempSzenarioTrialNumber,
                                                                           tempIsCatchTrial,
                                                                           Convert
                                                                               .ToInt32(
                                                                                   positionStatus
                                                                                       [
                                                                                           j,
                                                                                           0
                                                                                       ])
                                                                           ));
                        }
                    }

                    for (int j = 0; j < velocityDataTime.Length; j++)
                    {
                        lock (myDataContainter)
                        {
                            myDataContainter.VelocityDataNormalized.Add(new VelocityDataContainer
                                                                            (
                                                                            new DateTime
                                                                                (Convert
                                                                                     .ToInt64
                                                                                     (velocityDataTime
                                                                                          [
                                                                                              j,
                                                                                              0
                                                                                          ])),
                                                                            velocityX[
                                                                                j, 0],
                                                                            velocityY[
                                                                                j, 0],
                                                                            velocityZ[
                                                                                j, 0],
                                                                            threadTrials
                                                                                .ElementAt
                                                                                (i),
                                                                            tempFilteredDataEnum
                                                                                .ElementAt
                                                                                (0)
                                                                                .TargetNumber,
                                                                            Convert
                                                                                .ToInt32
                                                                                (positionStatus
                                                                                     [
                                                                                         j,
                                                                                         0
                                                                                     ])
                                                                            ));
                        }
                    }

                    _myMatlabWrapper.ClearWorkspaceData("newMeasureTime" +
                                                        threadTrials.ElementAt(i));
                    _myMatlabWrapper.ClearWorkspaceData("measure_data_time" +
                                                        threadTrials.ElementAt(i));
                    _myMatlabWrapper.ClearWorkspaceData("velocity_data_time" +
                                                        threadTrials.ElementAt(i));

                    _myMatlabWrapper.ClearWorkspaceData("forceActualX" +
                                                        threadTrials.ElementAt(i));
                    _myMatlabWrapper.ClearWorkspaceData("forceActualY" +
                                                        threadTrials.ElementAt(i));
                    _myMatlabWrapper.ClearWorkspaceData("forceActualZ" +
                                                        threadTrials.ElementAt(i));

                    _myMatlabWrapper.ClearWorkspaceData("forceNominalX" +
                                                        threadTrials.ElementAt(i));
                    _myMatlabWrapper.ClearWorkspaceData("forceNominalY" +
                                                        threadTrials.ElementAt(i));
                    _myMatlabWrapper.ClearWorkspaceData("forceNominalZ" +
                                                        threadTrials.ElementAt(i));

                    _myMatlabWrapper.ClearWorkspaceData("forceMomentX" +
                                                        threadTrials.ElementAt(i));
                    _myMatlabWrapper.ClearWorkspaceData("forceMomentY" +
                                                        threadTrials.ElementAt(i));
                    _myMatlabWrapper.ClearWorkspaceData("forceMomentZ" +
                                                        threadTrials.ElementAt(i));

                    _myMatlabWrapper.ClearWorkspaceData("positionCartesianX" +
                                                        threadTrials.ElementAt(i));
                    _myMatlabWrapper.ClearWorkspaceData("positionCartesianY" +
                                                        threadTrials.ElementAt(i));
                    _myMatlabWrapper.ClearWorkspaceData("positionCartesianZ" +
                                                        threadTrials.ElementAt(i));

                    _myMatlabWrapper.ClearWorkspaceData("positionStatus" +
                                                        threadTrials.ElementAt(i));

                    _myMatlabWrapper.ClearWorkspaceData("newVelocityTime" +
                                                        threadTrials.ElementAt(i));

                    _myMatlabWrapper.ClearWorkspaceData("velocityX" +
                                                        threadTrials.ElementAt(i));
                    _myMatlabWrapper.ClearWorkspaceData("velocityY" +
                                                        threadTrials.ElementAt(i));
                    _myMatlabWrapper.ClearWorkspaceData("velocityZ" +
                                                        threadTrials.ElementAt(i));
                }
            }
        }

        private void CalculateBaselinesThread(List<int>[] targetCoreDistribution, int coreVar,
                                              DataContainer myDataContainter)
        {
            var threadTargets = new List<int>(targetCoreDistribution[coreVar]);

            for (int targetCount = 0;
                 targetCount < threadTargets.Count();
                 targetCount++)
            {
                List<MeasureDataContainer> tempNormalisedDataEnum;
                List<VelocityDataContainer> tempVelocityDataNormalisedEnum;

                lock (myDataContainter)
                {
                    int targetCountVar = targetCount;
                    tempNormalisedDataEnum =
                        new List<MeasureDataContainer>(
                            myDataContainter.MeasureDataNormalized.Where(
                                t =>
                                t.TargetNumber ==
                                threadTargets.ElementAt(targetCountVar))
                                            .OrderBy(t => t.TimeStamp));
                    tempVelocityDataNormalisedEnum =
                        new List<VelocityDataContainer>(
                            myDataContainter.VelocityDataNormalized.Where(
                                t =>
                                t.TargetNumber ==
                                threadTargets.ElementAt(targetCountVar))
                                            .OrderBy(t => t.TimeStamp));
                }

                int[] tempSzenarioTrialNumbers =
                    tempNormalisedDataEnum.Where(t => t.TargetTrialNumber > 1)
                                          .Select(t => t.SzenarioTrialNumber)
                                          .OrderBy(t => t)
                                          .Distinct()
                                          .ToArray();
                int measureDataCount =
                    tempNormalisedDataEnum.Count(
                        t =>
                        t.SzenarioTrialNumber ==
                        tempSzenarioTrialNumbers.ElementAt(0));

                var positionCartesianX = new double[measureDataCount];
                var positionCartesianY = new double[measureDataCount];
                var positionCartesianZ = new double[measureDataCount];

                var velocityX = new double[measureDataCount];
                var velocityY = new double[measureDataCount];
                var velocityZ = new double[measureDataCount];

                for (int i = 0; i < tempSzenarioTrialNumbers.Count(); i++)
                {
                    int iVar = i;
                    List<MeasureDataContainer> tempMeasureDataCountainerList =
                        tempNormalisedDataEnum.Where(
                            t =>
                            t.SzenarioTrialNumber ==
                            tempSzenarioTrialNumbers.ElementAt(iVar))
                                              .OrderBy(t => t.TimeStamp)
                                              .ToList();
                    List<VelocityDataContainer> tempVelocityDataContainerList =
                        tempVelocityDataNormalisedEnum.Where(
                            t =>
                            t.SzenarioTrialNumber ==
                            tempSzenarioTrialNumbers.ElementAt(iVar))
                                                      .OrderBy(t => t.TimeStamp)
                                                      .ToList();

                    for (int j = 0; j < tempMeasureDataCountainerList.Count(); j++)
                    {
                        positionCartesianX[j] +=
                            tempMeasureDataCountainerList.ElementAt(j)
                                                         .PositionCartesianX;
                        positionCartesianY[j] +=
                            tempMeasureDataCountainerList.ElementAt(j)
                                                         .PositionCartesianY;
                        positionCartesianZ[j] +=
                            tempMeasureDataCountainerList.ElementAt(j)
                                                         .PositionCartesianZ;
                        velocityX[j] +=
                            tempVelocityDataContainerList.ElementAt(j).VelocityX;
                        velocityY[j] +=
                            tempVelocityDataContainerList.ElementAt(j).VelocityY;
                        velocityZ[j] +=
                            tempVelocityDataContainerList.ElementAt(j).VelocityZ;
                    }
                }

                for (int i = 0; i < positionCartesianX.Length; i++)
                {
                    positionCartesianX[i] /= tempSzenarioTrialNumbers.Count();
                    positionCartesianY[i] /= tempSzenarioTrialNumbers.Count();
                    positionCartesianZ[i] /= tempSzenarioTrialNumbers.Count();
                    velocityX[i] /= tempSzenarioTrialNumbers.Count();
                    velocityY[i] /= tempSzenarioTrialNumbers.Count();
                    velocityZ[i] /= tempSzenarioTrialNumbers.Count();
                }

                DateTime tempFileCreationDateTime =
                    DateTime.Parse("00:00:00 " +
                                   myDataContainter.MeasureFileCreationDate);
                for (int i = 0; i < positionCartesianX.Length; i++)
                {
                    lock (myDataContainter)
                    {
                        myDataContainter.BaselineData.Add(new BaselineDataContainer(
                                                              tempFileCreationDateTime
                                                                  .AddMilliseconds(
                                                                      i*10),
                                                              positionCartesianX[i],
                                                              positionCartesianY[i],
                                                              positionCartesianZ[i],
                                                              velocityX[i],
                                                              velocityY[i],
                                                              velocityZ[i],
                                                              threadTargets
                                                                  .ElementAt(
                                                                      targetCount)
                                                              ));
                    }
                }
            }
        }

        private void CalculateSzenarioMeanTimesThread(List<int>[] targetCoreDistribution, int coreVar,
                                                      DataContainer myDataContainter)
        {
            var threadTargets = new List<int>(targetCoreDistribution[coreVar]);

            for (int targetCount = 0;
                 targetCount < threadTargets.Count();
                 targetCount++)
            {
                List<MeasureDataContainer> tempNormalisedDataEnum;

                lock (myDataContainter)
                {
                    int targetCountVar = targetCount;
                    tempNormalisedDataEnum =
                        new List<MeasureDataContainer>(
                            myDataContainter.MeasureDataNormalized.Where(
                                t =>
                                t.TargetNumber ==
                                threadTargets.ElementAt(targetCountVar))
                                            .OrderBy(t => t.TimeStamp));
                }

                int[] tempSzenarioTrialNumbers =
                    tempNormalisedDataEnum.Select(t => t.SzenarioTrialNumber)
                                          .OrderBy(t => t)
                                          .Distinct()
                                          .ToArray();

                var meanTimeStdArray = new long[tempSzenarioTrialNumbers.Length];

                for (int i = 0; i < tempSzenarioTrialNumbers.Length; i++)
                {
                    long maxVal =
                        tempNormalisedDataEnum.Where(
                            t =>
                            t.SzenarioTrialNumber ==
                            tempSzenarioTrialNumbers.ElementAt(i))
                                              .Select(t => t.TimeStamp.Ticks)
                                              .Max();
                    long minVal =
                        tempNormalisedDataEnum.Where(
                            t =>
                            t.SzenarioTrialNumber ==
                            tempSzenarioTrialNumbers.ElementAt(i))
                                              .Select(t => t.TimeStamp.Ticks)
                                              .Min();

                    meanTimeStdArray[i] = (maxVal - minVal);
                }

                var meanTime =
                    new TimeSpan(meanTimeStdArray.Sum()/meanTimeStdArray.Length);

                _myMatlabWrapper.SetWorkspaceData(
                    "timeArray" + threadTargets.ElementAt(targetCount), meanTimeStdArray);
                _myMatlabWrapper.Execute("meanTimeStd" +
                                         threadTargets.ElementAt(targetCount) +
                                         " = int64(std(double(timeArray" +
                                         threadTargets.ElementAt(targetCount) + ")));");
                var meanTimeStd =
                    new TimeSpan(
                        _myMatlabWrapper.GetWorkspaceData("meanTimeStd" +
                                                          threadTargets.ElementAt(
                                                              targetCount)));
                _myMatlabWrapper.ClearWorkspaceData("timeArray" +
                                                    threadTargets.ElementAt(targetCount));
                _myMatlabWrapper.ClearWorkspaceData("meanTimeStd" +
                                                    threadTargets.ElementAt(targetCount));


                lock (myDataContainter)
                {
                    myDataContainter.SzenarioMeanTimeData.Add(new SzenarioMeanTimeDataContainer
                                                                  (
                                                                  meanTime,
                                                                  meanTimeStd,
                                                                  threadTargets
                                                                      .ElementAt(
                                                                          targetCount)
                                                                  ));
                }
            }
        }

        public void CalculateStatistics()
        {
            var newTask = new Task(CalculateStatisticsThread);
            TaskManager.PushBack(newTask);
            newTask.Start();
        }

        private void CalculateStatisticsThread()
        {
            while (TaskManager.GetIndex(Task.CurrentId) != 0)
            {
                Thread.Sleep(100);
            }

            _myManipAnalysisGui.EnableTabPages(false);
            _myManipAnalysisGui.WriteProgressInfo("Calculating statistics...");

            List<int[]> trialInfos = _mySqlWrapper.GetStatisticCalculationInformation();

            if (trialInfos != null)
            {
                int counter = 1;

                foreach (var trialInfo in trialInfos)
                {
                    while (TaskManager.Pause)
                    {
                        Thread.Sleep(100);
                    }
                    _myManipAnalysisGui.SetProgressBarValue((100.0/trialInfos.Count())*counter);
                    counter++;

                    DataSet measureDataSet = _mySqlWrapper.GetMeasureDataNormalizedDataSet(trialInfo[0]);
                    DataSet velocityDataSet = _mySqlWrapper.GetVelocityDataNormalizedDataSet(trialInfo[0]);
                    DataSet baselineDataSet = _mySqlWrapper.GetBaselineDataSet(trialInfo[1], trialInfo[2], trialInfo[3],
                                                                               trialInfo[4]);
                    int targetNumber = trialInfo[5];

                    if (baselineDataSet.Tables[0].Rows.Count > 0)
                    {
                        if ((measureDataSet.Tables[0].Rows.Count == velocityDataSet.Tables[0].Rows.Count) &&
                            (velocityDataSet.Tables[0].Rows.Count == baselineDataSet.Tables[0].Rows.Count))
                        {
                            try
                            {
                                int sampleCount = measureDataSet.Tables[0].Rows.Count;

                                var measureData = new double[sampleCount,3];
                                var velocityData = new double[sampleCount,3];
                                var baselineData = new double[sampleCount,6];
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

                                _mySqlWrapper.InsertStatisticData(trialInfo[0], vectorCorrelation, lengthAbs,
                                                                  lengthRatio, distance300MsAbs, maxDistanceAbs,
                                                                  meanDistanceAbs, distance300MsSign, maxDistanceSign,
                                                                  enclosedArea, rmse);
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
            while (TaskManager.GetIndex(Task.CurrentId) != 0)
            {
                Thread.Sleep(100);
            }

            _myManipAnalysisGui.EnableTabPages(false);
            _myManipAnalysisGui.WriteProgressInfo("Fixing broken Trials...");

            List<object[]> faultyTrialInformation = _mySqlWrapper.GetFaultyTrialInformation();

            if (faultyTrialInformation != null)
            {
                if (faultyTrialInformation.Count == 0)
                {
                    _myManipAnalysisGui.WriteToLogBox("Trials already fixed!");
                }
                else
                {
                    for (int trialIDCounter = 0; trialIDCounter < faultyTrialInformation.Count; trialIDCounter++)
                    {
                        while (TaskManager.Pause)
                        {
                            Thread.Sleep(100);
                        }
                        _myManipAnalysisGui.SetProgressBarValue((100.0/faultyTrialInformation.Count)*trialIDCounter);

                        int[] trialFixInformation =
                            _mySqlWrapper.GetFaultyTrialFixInformation(
                                Convert.ToInt32(faultyTrialInformation[trialIDCounter][1]),
                                Convert.ToInt32(faultyTrialInformation[trialIDCounter][7]));

                        DataSet upperStatisticDataSet = _mySqlWrapper.GetStatisticDataSet(trialFixInformation[0]);
                        DataSet lowerStatisticDataSet = _mySqlWrapper.GetStatisticDataSet(trialFixInformation[1]);

                        double velocityVectorCorrelation =
                            (Convert.ToDouble(upperStatisticDataSet.Tables[0].Rows[0]["velocity_vector_correlation"]) +
                             Convert.ToDouble(lowerStatisticDataSet.Tables[0].Rows[0]["velocity_vector_correlation"]))/2;
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
                                 lowerStatisticDataSet.Tables[0].Rows[0]["maximal_perpendicular_displacement_abs"]))/2;
                        double meanPerpendicularDisplacementAbs =
                            (Convert.ToDouble(
                                upperStatisticDataSet.Tables[0].Rows[0]["mean_perpendicular_displacement_abs"]) +
                             Convert.ToDouble(
                                 lowerStatisticDataSet.Tables[0].Rows[0]["mean_perpendicular_displacement_abs"]))/2;
                        double perpendicularDisplacement300MsSign =
                            (Convert.ToDouble(
                                upperStatisticDataSet.Tables[0].Rows[0]["perpendicular_displacement_300ms_sign"]) +
                             Convert.ToDouble(
                                 lowerStatisticDataSet.Tables[0].Rows[0]["perpendicular_displacement_300ms_sign"]))/2;
                        double maximalPerpendicularDisplacementSign =
                            (Convert.ToDouble(
                                upperStatisticDataSet.Tables[0].Rows[0]["maximal_perpendicular_displacement_sign"]) +
                             Convert.ToDouble(
                                 lowerStatisticDataSet.Tables[0].Rows[0]["maximal_perpendicular_displacement_sign"]))/2;
                        double enclosedArea =
                            (Convert.ToDouble(upperStatisticDataSet.Tables[0].Rows[0]["enclosed_area"]) +
                             Convert.ToDouble(lowerStatisticDataSet.Tables[0].Rows[0]["enclosed_area"]))/2;
                        double rmse = (Convert.ToDouble(upperStatisticDataSet.Tables[0].Rows[0]["rmse"]) +
                                       Convert.ToDouble(lowerStatisticDataSet.Tables[0].Rows[0]["rmse"]))/2;

                        _mySqlWrapper.InsertStatisticData(Convert.ToInt32(faultyTrialInformation[trialIDCounter][0]),
                                                          velocityVectorCorrelation, trajectoryLengthAbs,
                                                          trajectoryLengthRatioBaseline,
                                                          perpendicularDisplacement300MsAbs,
                                                          maximalPerpendicularDisplacementAbs,
                                                          meanPerpendicularDisplacementAbs,
                                                          perpendicularDisplacement300MsSign,
                                                          maximalPerpendicularDisplacementSign, enclosedArea, rmse);
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
                                   bool ignoreCatchTrials)
        {
            _myManipAnalysisGui.WriteProgressInfo("Getting data...");
            List<TrajectoryVelocityPlotContainer> selectedTrialsList = selectedTrials.ToList();

            if (meanIndividual == "Individual")
            {
                _myMatlabWrapper.CreateTrajectoryFigure("XZ-Plot");
                _myMatlabWrapper.DrawTargets(0.005, 0.1, 0, 0);


                int counter = 0;
                foreach (TrajectoryVelocityPlotContainer tempContainer in selectedTrialsList)
                {
                    _myManipAnalysisGui.SetProgressBarValue((100.0/selectedTrialsList.Count())*counter);
                    counter++;
                    DateTime turnDateTime = _mySqlWrapper.GetTurnDateTime(tempContainer.Study, tempContainer.Group,
                                                                          tempContainer.Szenario,
                                                                          tempContainer.Subject,
                                                                          tempContainer.Turn);
                    foreach (int trial in tempContainer.Trials)
                    {
                        int trialID = _mySqlWrapper.GetTrailID(tempContainer.Study, tempContainer.Group,
                                                               tempContainer.Szenario, tempContainer.Subject,
                                                               turnDateTime, tempContainer.Target, trial);
                        DataSet measureDataSet = _mySqlWrapper.GetMeasureDataNormalizedDataSet(trialID);

                        var measureDataX = new List<double>();
                        var measureDataZ = new List<double>();

                        foreach (DataRow row in measureDataSet.Tables[0].Rows)
                        {
                            if (ignoreCatchTrials)
                            {
                                if (Convert.ToInt32(row["is_catch_trial"]) == 0)
                                {
                                    measureDataX.Add(Convert.ToDouble(row["position_cartesian_x"]));
                                    measureDataZ.Add(Convert.ToDouble(row["position_cartesian_z"]));
                                }
                            }
                            else
                            {
                                measureDataX.Add(Convert.ToDouble(row["position_cartesian_x"]));
                                measureDataZ.Add(Convert.ToDouble(row["position_cartesian_z"]));
                            }
                        }

                        _myMatlabWrapper.SetWorkspaceData("X", measureDataX.ToArray());
                        _myMatlabWrapper.SetWorkspaceData("Z", measureDataZ.ToArray());
                        _myMatlabWrapper.Plot("X", "Z", "black", 2);
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

                    for (int targetCounter = 0; targetCounter < targetArray.Length; targetCounter++)
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
                            _myManipAnalysisGui.SetProgressBarValue((100.0/selectedTrialsList.Count())*
                                                                    counter);
                            counter++;
                            DateTime turnDateTime = _mySqlWrapper.GetTurnDateTime(tempContainer.Study,
                                                                                  tempContainer.Group,
                                                                                  tempContainer.Szenario,
                                                                                  tempContainer.Subject,
                                                                                  tempContainer.Turn);


                            foreach (int trial in tempContainer.Trials)
                            {
                                int trialID = _mySqlWrapper.GetTrailID(tempContainer.Study, tempContainer.Group,
                                                                       tempContainer.Szenario,
                                                                       tempContainer.Subject, turnDateTime,
                                                                       tempContainer.Target, trial);
                                DataSet dataSet = _mySqlWrapper.GetMeasureDataNormalizedDataSet(trialID);
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
        }

        public void PlotVelocity(IEnumerable<TrajectoryVelocityPlotContainer> selectedTrials, string meanIndividual,
                                 bool ignoreCatchTrials)
        {
            _myManipAnalysisGui.WriteProgressInfo("Getting data...");
            List<TrajectoryVelocityPlotContainer> selectedTrialsList = selectedTrials.ToList();

            if (meanIndividual == "Individual")
            {
                _myMatlabWrapper.CreateVelocityFigure("Velocity plot", 101);

                int counter = 0;
                foreach (TrajectoryVelocityPlotContainer tempContainer in selectedTrialsList)
                {
                    _myManipAnalysisGui.SetProgressBarValue((100.0/selectedTrialsList.Count())*counter);
                    counter++;
                    DateTime turnDateTime = _mySqlWrapper.GetTurnDateTime(tempContainer.Study, tempContainer.Group,
                                                                          tempContainer.Szenario,
                                                                          tempContainer.Subject,
                                                                          tempContainer.Turn);
                    foreach (int trial in tempContainer.Trials)
                    {
                        int trialID = _mySqlWrapper.GetTrailID(tempContainer.Study, tempContainer.Group,
                                                               tempContainer.Szenario,
                                                               tempContainer.Subject, turnDateTime,
                                                               tempContainer.Target, trial);
                        DataSet velocityDataSet = _mySqlWrapper.GetVelocityDataNormalizedDataSet(trialID);
                        var velocityDataXZ = new List<double>();

                        foreach (DataRow row in velocityDataSet.Tables[0].Rows)
                        {
                            if (ignoreCatchTrials)
                            {
                                if (Convert.ToInt32(row["is_catch_trial"]) == 0)
                                {
                                    velocityDataXZ.Add(
                                        Math.Sqrt(Math.Pow(Convert.ToDouble(row["velocity_x"]), 2) +
                                                  Math.Pow(Convert.ToDouble(row["velocity_z"]), 2)));
                                }
                            }
                            else
                            {
                                velocityDataXZ.Add(
                                    Math.Sqrt(Math.Pow(Convert.ToDouble(row["velocity_x"]), 2) +
                                              Math.Pow(Convert.ToDouble(row["velocity_z"]), 2)));
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

                    for (int targetCounter = 0; targetCounter < targetArray.Length; targetCounter++)
                    {
                        int targetCounterVar = targetCounter;
                        int meanCounter = 0;
                        var dataXZ = new List<double>();

                        foreach (
                            TrajectoryVelocityPlotContainer tempContainer in
                                selectedTrialsList.Where(t => t.Target == targetArray[targetCounterVar]))
                        {
                            _myManipAnalysisGui.SetProgressBarValue((100.0/selectedTrialsList.Count())*
                                                                    counter);
                            counter++;
                            DateTime turnDateTime = _mySqlWrapper.GetTurnDateTime(tempContainer.Study,
                                                                                  tempContainer.Group,
                                                                                  tempContainer.Szenario,
                                                                                  tempContainer.Subject,
                                                                                  tempContainer.Turn);

                            foreach (int trial in tempContainer.Trials)
                            {
                                int trialID = _mySqlWrapper.GetTrailID(tempContainer.Study,
                                                                       tempContainer.Group,
                                                                       tempContainer.Szenario,
                                                                       tempContainer.Subject,
                                                                       turnDateTime, tempContainer.Target,
                                                                       trial);
                                DataSet dataSet = _mySqlWrapper.GetVelocityDataNormalizedDataSet(trialID);

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
        }

        public void ExportTrajectoryData(IEnumerable<TrajectoryVelocityPlotContainer> selectedTrials,
                                         string meanIndividual, string fileName)
        {
            List<TrajectoryVelocityPlotContainer> selectedTrailsList = selectedTrials.ToList();
            if (meanIndividual == "Individual")
            {
                int counter = 0;
                foreach (TrajectoryVelocityPlotContainer tempContainer in selectedTrailsList)
                {
                    _myManipAnalysisGui.SetProgressBarValue((100.0/selectedTrailsList.Count)*counter);
                    counter++;
                    DateTime turnDateTime = _mySqlWrapper.GetTurnDateTime(tempContainer.Study,
                                                                          tempContainer.Group,
                                                                          tempContainer.Szenario,
                                                                          tempContainer.Subject,
                                                                          tempContainer.Turn);
                    foreach (int trial in tempContainer.Trials)
                    {
                        int trialID = _mySqlWrapper.GetTrailID(tempContainer.Study, tempContainer.Group,
                                                               tempContainer.Szenario,
                                                               tempContainer.Subject, turnDateTime,
                                                               tempContainer.Target, trial);
                        DataSet measureDataSet = _mySqlWrapper.GetMeasureDataNormalizedDataSet(trialID);

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
                        dataFileStream.Close();
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
                    for (int targetCounter = 0; targetCounter < targetArray.Length; targetCounter++)
                    {
                        int meanCounter = 0;
                        var dataX = new List<double>();
                        var dataZ = new List<double>();

                        int counterVar = targetCounter;
                        foreach (TrajectoryVelocityPlotContainer tempContainer in selectedTrailsList)
                        {
                            if (tempContainer.Target == targetArray[counterVar])
                            {
                                _myManipAnalysisGui.SetProgressBarValue((100.0/selectedTrailsList.Count)*counter);
                                counter++;
                                DateTime turnDateTime = _mySqlWrapper.GetTurnDateTime(tempContainer.Study,
                                                                                      tempContainer.Group,
                                                                                      tempContainer.Szenario,
                                                                                      tempContainer.Subject,
                                                                                      tempContainer.Turn);


                                foreach (int trial in tempContainer.Trials)
                                {
                                    int trialID = _mySqlWrapper.GetTrailID(tempContainer.Study,
                                                                           tempContainer.Group,
                                                                           tempContainer.Szenario,
                                                                           tempContainer.Subject,
                                                                           turnDateTime,
                                                                           tempContainer.Target, trial);
                                    DataSet dataSet = _mySqlWrapper.GetMeasureDataNormalizedDataSet(trialID);
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
                        dataFileStream.Close();
                    }
                }
            }

            _myMatlabWrapper.ClearWorkspace();
        }

        public void ExportVelocityData(IEnumerable<TrajectoryVelocityPlotContainer> selectedTrials,
                                       string meanIndividual, string fileName)
        {
            List<TrajectoryVelocityPlotContainer> selectedTrailsList = selectedTrials.ToList();
            if (meanIndividual == "Individual")
            {
                int counter = 0;
                foreach (TrajectoryVelocityPlotContainer tempContainer in selectedTrailsList)
                {
                    _myManipAnalysisGui.SetProgressBarValue((100.0/selectedTrailsList.Count)*counter);
                    counter++;
                    DateTime turnDateTime = _mySqlWrapper.GetTurnDateTime(tempContainer.Study,
                                                                          tempContainer.Group,
                                                                          tempContainer.Szenario,
                                                                          tempContainer.Subject,
                                                                          tempContainer.Turn);
                    foreach (int trial in tempContainer.Trials)
                    {
                        int trialID = _mySqlWrapper.GetTrailID(tempContainer.Study, tempContainer.Group,
                                                               tempContainer.Szenario,
                                                               tempContainer.Subject, turnDateTime,
                                                               tempContainer.Target, trial);
                        DataSet velocityDataSet = _mySqlWrapper.GetVelocityDataNormalizedDataSet(trialID);

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
                        dataFileStream.Close();
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

                    var cache = new List<string> {"Study;Group;Szenario;Subject;Turn;Target;Trial;DataPoint;VelocityXZ"};


                    int counter = 0;
                    for (int targetCounter = 0; targetCounter < targetArray.Length; targetCounter++)
                    {
                        int meanCounter = 0;
                        var dataXZ = new List<double>();

                        int counterVar = targetCounter;
                        foreach (TrajectoryVelocityPlotContainer tempContainer in selectedTrailsList)
                        {
                            if (tempContainer.Target == targetArray[counterVar])
                            {
                                _myManipAnalysisGui.SetProgressBarValue((100.0/selectedTrailsList.Count)*counter);
                                counter++;
                                DateTime turnDateTime = _mySqlWrapper.GetTurnDateTime(tempContainer.Study,
                                                                                      tempContainer.Group,
                                                                                      tempContainer.Szenario,
                                                                                      tempContainer.Subject,
                                                                                      tempContainer.Turn);


                                foreach (int trial in tempContainer.Trials)
                                {
                                    int trialID = _mySqlWrapper.GetTrailID(tempContainer.Study,
                                                                           tempContainer.Group,
                                                                           tempContainer.Szenario,
                                                                           tempContainer.Subject,
                                                                           turnDateTime,
                                                                           tempContainer.Target, trial);

                                    DataSet dataSet =
                                        _mySqlWrapper.GetVelocityDataNormalizedDataSet(trialID);
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
                        dataFileStream.Close();
                    }
                }
            }

            _myMatlabWrapper.ClearWorkspace();
        }

        public void ExportTrajectoryBaseline(string study, string group, string szenario,
                                             SubjectInformationContainer subject, string fileName)
        {
            DataSet baseline = _mySqlWrapper.GetBaselineDataSet(study, group, szenario, subject);

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

            for (int i = 0; i < targetNumberArray.Length; i++)
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
                        study + ";" + @group + ";" + szenario + ";" + subject + ";" + targetNumberArray[iVar] + ";" +
                        j + ";" + DoubleConverter.ToExactString(t) + ";" + DoubleConverter.ToExactString(tempZ[j])));
            }

            var dataFileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            var dataFileWriter = new StreamWriter(dataFileStream);

            for (int i = 0; i < cache.Count(); i++)
            {
                dataFileWriter.WriteLine(cache[i]);
            }

            dataFileWriter.Close();
            dataFileStream.Close();
        }

        public void ExportVelocityBaseline(string study, string group, string szenario,
                                           SubjectInformationContainer subject, string fileName)
        {
            DataSet baseline = _mySqlWrapper.GetBaselineDataSet(study, group, szenario, subject);

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

            for (int i = 0; i < targetNumberArray.Length; i++)
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
                        study + ";" + @group + ";" + szenario + ";" + subject + ";" + targetNumberArray[iVar] + ";" +
                        j + ";" + DoubleConverter.ToExactString(t) + ";" + DoubleConverter.ToExactString(tempZ[j])));
            }

            var dataFileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            var dataFileWriter = new StreamWriter(dataFileStream);

            for (int i = 0; i < cache.Count(); i++)
            {
                dataFileWriter.WriteLine(cache[i]);
            }

            dataFileWriter.Close();
            dataFileStream.Close();
        }

        public void PlotVelocityBaselines(string study, string group, string szenario,
                                          SubjectInformationContainer subject)
        {
            _myMatlabWrapper.CreateVelocityFigure("Velocity baseline plot", 101);
            _myMatlabWrapper.DrawTargets(0.005, 0.1, 0, 0);

            DataSet baseline = _mySqlWrapper.GetBaselineDataSet(study, group, szenario, subject);

            List<object[]> baselineData = (from DataRow row in baseline.Tables[0].Rows
                                           select new object[]
                                               {
                                                   Convert.ToDouble(row["baseline_velocity_x"]),
                                                   Convert.ToDouble(row["baseline_velocity_z"]),
                                                   Convert.ToInt32(row["target_number"])
                                               }).ToList();

            int[] targetNumberArray = baselineData.Select(t => Convert.ToInt32(t[2])).Distinct().ToArray();

            for (int i = 0; i < targetNumberArray.Length; i++)
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
        }

        public void RecalculateBaselines(IEnumerable<TrajectoryVelocityPlotContainer> selectedTrials)
        {
            // (1) Recalculate subjects baselines
            // (2) Delete subjects baselines
            //      -> Get baseline_id, delete entries with this id in _baseline_data, insert new data with this id.
            // (3) Delete subjects statistics
            //      -> Get trial_id, delete entried with this id in _statistic_data
            // (4) Upload new baselines
            // (5) Recalculate missing statistics

            List<TrajectoryVelocityPlotContainer> selectedTrialsList = selectedTrials.ToList();
            int[] targetArray = selectedTrialsList.Select(t => t.Target).Distinct().ToArray();
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

                int baselineID = _mySqlWrapper.GetBaselineID(tempContainer.Study,
                                                                     tempContainer.Group,
                                                                     tempContainer.Szenario,
                                                                     tempContainer.Subject,
                                                                     tempContainer.Target);
                

                    _myManipAnalysisGui.SetProgressBarValue((100.0/selectedTrialsList.Count())*
                                                            counter);
                    counter++;
                    DateTime turnDateTime = _mySqlWrapper.GetTurnDateTime(tempContainer.Study,
                                                                          tempContainer.Group,
                                                                          tempContainer.Szenario,
                                                                          tempContainer.Subject,
                                                                          tempContainer.Turn);


                foreach (int trial in tempContainer.Trials)
                {
                    int trialID = _mySqlWrapper.GetTrailID(tempContainer.Study,
                                                           tempContainer.Group,
                                                           tempContainer.Szenario,
                                                           tempContainer.Subject,
                                                           turnDateTime,
                                                           tempContainer.Target,
                                                           trial);

                    DataSet measureDataNormalizedDataSet = _mySqlWrapper.GetMeasureDataNormalizedDataSet(trialID);
                    DataSet velocityDataNormalizedDataSet = _mySqlWrapper.GetVelocityDataNormalizedDataSet(trialID);

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

                _myManipAnalysisGui.WriteToLogBox("BaselineID: " + baselineID + " - Target: " + targetArray[targetCounterVar] + " - " + meanCounter + " Values used.");
            }

            _myManipAnalysisGui.WriteProgressInfo("Ready.");
            _myManipAnalysisGui.SetProgressBarValue(0);
        }
    }
}