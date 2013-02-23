using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using ManipAnalysis.Container;

namespace ManipAnalysis
{
    public partial class ManipAnalysis : Form
    {
        private readonly MatlabWrapper _myMatlabWrapper;
        private readonly SqlWrapper _mySqlWrapper;

        public ManipAnalysis()
        {
            var splash = new SplashScreen();
            splash.Show();

            try
            {
                _mySqlWrapper = new SqlWrapper(this);
                _myMatlabWrapper = new MatlabWrapper();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            InitializeComponent();

            checkBox_Start_ManualMode.Enabled = false;
            tabControl.TabPages.Remove(tabPage_VisualizationExport);
            tabControl.TabPages.Remove(tabPage_ImportCalculations);
            tabControl.TabPages.Remove(tabPage_Debug);
            comboBox_Start_SQL_Server.SelectedIndex = 0;

            splash.Close();
        }

        public void WriteToLogBox(string text)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new LogBoxCallbackAddString(WriteToLogBox), text);
            }
            else
            {
                listBox_LogBox.Items.Add("[" + DateTime.Now + "] " + text);
                listBox_LogBox.TopIndex = listBox_LogBox.Items.Count - 1;
            }
        }

        private void WriteToLogBox(string[] textArray)
        {
            if (InvokeRequired)
            {
                if (textArray != null)
                {
                    BeginInvoke(new LogBoxCallbackAddStringArray(WriteToLogBox), textArray);
                }
            }
            else
            {
                for (int i = 0; i < textArray.Length; i++)
                {
                    if (i == 0)
                    {
                        listBox_LogBox.Items.Add("[" + DateTime.Now + "] " + textArray[0]);
                        listBox_LogBox.TopIndex = listBox_LogBox.Items.Count - 1;
                    }
                    else
                    {
                        listBox_LogBox.Items.Add(textArray[i]);
                        listBox_LogBox.TopIndex = listBox_LogBox.Items.Count - 1;
                    }
                }
            }
        }

        private string[] GetLogBoxText()
        {
            string[] retVal = null;

            if (InvokeRequired)
            {
                BeginInvoke(new LogBoxCallbackGetText(GetLogBoxText));
            }
            else
            {
                retVal = listBox_LogBox.Items.Cast<string>().ToArray();
            }

            return retVal;
        }

        private void ClearLogBox()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new LogBoxCallbackClearItems(ClearLogBox));
            }
            else
            {
                listBox_LogBox.Items.Clear();
            }
        }

        private void button_OpenMeasureFiles_Click(object sender, EventArgs e)
        {
            bool isValid = true;

            openFileDialog.Reset();
            openFileDialog.Multiselect = true;
            openFileDialog.Title = @"Select measure-file(s)";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog.Filter = @"MeasureData-files (*.csv)|*.csv";
            openFileDialog.ShowDialog();

            var filesList = new List<FileInfo>(openFileDialog.FileNames.Select(t => new FileInfo(t)));

            filesList.RemoveAll(t => (!t.Name.Contains("Szenario")));
            filesList.RemoveAll(t => (t.Name.Contains("Szenario00")));
            filesList.RemoveAll(t => (t.Name.Contains("Szenario01")));


            for (int i = 0; i < filesList.Count; i++)
            {
                if (filesList[i].Name.Count(t => t == '-') == 6)
                {
                    string tempFileHash = Md5.ComputeHash(filesList[i].FullName);

                    if (!_mySqlWrapper.CheckIfMeasureFileHashExists(tempFileHash))
                    {
                        listBox_Import_SelectedMeasureFiles.Items.Add(filesList[i].FullName);
                    }
                }
                else
                {
                    isValid = false;
                }
            }

            if (!isValid)
            {
                WriteToLogBox("One or more filenames are invalid!");
            }
        }

        private void button_OpenMeasureFilesFolder_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.Reset();
            folderBrowserDialog.ShowDialog();

            var directoriesList = new List<DirectoryInfo>();
            var filesList = new List<FileInfo>();

            string path = folderBrowserDialog.SelectedPath;

            if (path != "")
            {
                var rootDir = new DirectoryInfo(path);

                GetSubDirectories(ref directoriesList, rootDir);

                for (int i = 0; i < directoriesList.Count; i++)
                {
                    DirectoryInfo di = directoriesList[i];

                    FileInfo[] files = di.GetFiles("*.csv");

                    filesList.AddRange(files);
                }
                directoriesList.Clear();
                filesList.RemoveAll(t => (!t.Name.Contains("Szenario")));
                filesList.RemoveAll(t => (t.Name.Contains("Szenario00")));
                filesList.RemoveAll(t => (t.Name.Contains("Szenario01")));

                bool isValid = true;

                for (int i = 0; i < filesList.Count; i++)
                {
                    FileInfo fi = filesList[i];
                    //if (fi.Name.Count(t => t == '-') == 5)  // Study 1
                    if (fi.Name.Count(t => t == '-') == 6) // Study 2
                    {
                        string tempFileHash = Md5.ComputeHash(fi.FullName);

                        if (!_mySqlWrapper.CheckIfMeasureFileHashExists(tempFileHash))
                        {
                            listBox_Import_SelectedMeasureFiles.Items.Add(fi.FullName);
                        }
                    }
                    else
                    {
                        isValid = false;
                    }
                }

                if (!isValid)
                {
                    WriteToLogBox("One or more filenames are invalid!");
                }
            }
        }

        private void ManipAnalysis_FormClosed(object sender, FormClosedEventArgs e)
        {
            _mySqlWrapper.CloseSqlConnection();
            try
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button_StatisticPlots_SzenarioMeanTime_Click(object sender, EventArgs e)
        {
            string study = comboBox_BaselineMeantime_Study.SelectedItem.ToString();
            string group = comboBox_BaselineMeantime_Group.SelectedItem.ToString();
            string szenario = comboBox_BaselineMeantime_Szenario.SelectedItem.ToString();
            var subject = (SubjectInformationContainer) comboBox_BaselineMeantime_Subject.SelectedItem;
            int turn = Convert.ToInt32(comboBox_BaselineMeantime_Turn.SelectedItem.ToString().Substring("Turn".Length));
            DateTime turnDateTime = _mySqlWrapper.GetTurnDateTime(study, group, szenario, subject.ID, turn);

            DataSet meanTimeDataSet = _mySqlWrapper.GetMeanTimeDataSet(study, group, szenario, subject.ID, turnDateTime);

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
            targetList.Add(17);

            _myMatlabWrapper.SetWorkspaceData("target", targetList.ToArray());
            _myMatlabWrapper.SetWorkspaceData("meanTime", meanTimeList.ToArray());
            _myMatlabWrapper.SetWorkspaceData("meanTimeStd", meanTimeStdList.ToArray());
            _myMatlabWrapper.PlotMeanTimeErrorBar("target", "meanTime", "meanTimeStd");

            _myMatlabWrapper.ClearWorkspace();
        }

        private void button_ShowMatlabWindow_Click(object sender, EventArgs e)
        {
            _myMatlabWrapper.ToggleShowCommandWindow();
        }

        private void button_ShowMatlabWorkspace_Click(object sender, EventArgs e)
        {
            _myMatlabWrapper.ShowWorkspaceWindow();
        }

        private void checkBox_ManualMode_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_Start_ManualMode.Checked)
            {
                tabControl.TabPages.Remove(tabPage_Impressum);
                tabControl.TabPages.Add(tabPage_ImportCalculations);
                tabControl.TabPages.Add(tabPage_Debug);
                tabControl.TabPages.Add(tabPage_Impressum);
            }
            else
            {
                tabControl.TabPages.Remove(tabPage_ImportCalculations);
                tabControl.TabPages.Remove(tabPage_Debug);
            }
        }

        private void button_Debug_InitialiseDatabase_Click(object sender, EventArgs e)
        {
            const MessageBoxButtons buttons = MessageBoxButtons.YesNo;

            DialogResult result = MessageBox.Show(@"Are you really sure you want to initialise the Database?",
                                                  @"Really?", buttons);

            if (result == DialogResult.Yes)
            {
                _mySqlWrapper.InitializeDatabase();
            }
        }

        private void button_ChangeSQlServerConnection_Click(object sender, EventArgs e)
        {
            bool serverAvailable = false;
            comboBox_Start_Database.Items.Clear();

            using (var tcp = new TcpClient())
            {
                IAsyncResult ar = tcp.BeginConnect(comboBox_Start_SQL_Server.Text, 1433, null, null);
                WaitHandle wh = ar.AsyncWaitHandle;
                try
                {
                    if (!ar.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(2), false))
                    {
                        tcp.Close();
                        throw new TimeoutException();
                    }

                    tcp.EndConnect(ar);
                    serverAvailable = true;
                }
                catch
                {
                    WriteToLogBox("SQL-Server not reachable!");
                    checkBox_Start_ManualMode.Enabled = false;
                    tabControl.TabPages.Remove(tabPage_VisualizationExport);
                    tabControl.TabPages.Remove(tabPage_ImportCalculations);
                    tabControl.TabPages.Remove(tabPage_Debug);
                }
                finally
                {
                    wh.Close();
                }
            }

            if (serverAvailable)
            {
                _mySqlWrapper.SetSqlServer(comboBox_Start_SQL_Server.Text);
                WriteToLogBox("Connected to SQL-Server.");
                comboBox_Start_Database.Items.AddRange(_mySqlWrapper.GetDatabases());
                comboBox_Start_Database.SelectedIndex = 0;
                comboBox_Start_Database.Enabled = true;
                button_Start_SelectDatabase.Enabled = true;
            }
            else
            {
                tabControl.TabPages.Remove(tabPage_VisualizationExport);
                tabControl.TabPages.Remove(tabPage_ImportCalculations);
                tabControl.TabPages.Remove(tabPage_Debug);
                comboBox_Start_Database.Enabled = false;
                button_Start_SelectDatabase.Enabled = false;
            }
        }

        private void button_PlotBaseline_Click(object sender, EventArgs e)
        {
            string study = comboBox_BaselineMeantime_Study.SelectedItem.ToString();
            string group = comboBox_BaselineMeantime_Group.SelectedItem.ToString();
            string szenario = comboBox_BaselineMeantime_Szenario.SelectedItem.ToString();
            var subject = (SubjectInformationContainer) comboBox_BaselineMeantime_Subject.SelectedItem;

            _myMatlabWrapper.CreateTrajectoryFigure("Trajectory baseline plot");
            _myMatlabWrapper.DrawTargets(0.005, 0.1, 0, 0);

            DataSet baseline = _mySqlWrapper.GetBaselineDataSet(study, group, szenario, subject.ID);

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

        private void EnableTabPages(bool enable)
        {
            if (tabControl.InvokeRequired)
            {
                TabControlCallback enableTabPages = EnableTabPages;
                tabControl.Invoke(enableTabPages, new object[] {enable});
            }
            else
            {
                tabControl.Enabled = enable;
            }
        }

        private void SetProgressBarValue(double value)
        {
            if (progressBar.InvokeRequired)
            {
                ProgressBarCallback setProgressBarValue = SetProgressBarValue;
                progressBar.Invoke(setProgressBarValue, new object[] {value});
            }
            else
            {
                progressBar.Value = Convert.ToInt32(value);
            }
        }

        public void WriteProgressInfo(string text)
        {
            if (label_ProgressInfo.InvokeRequired)
            {
                ProgressLabelCallback writeProgressInfo = WriteProgressInfo;
                label_ProgressInfo.Invoke(writeProgressInfo, new object[] {text});
            }
            else
            {
                label_ProgressInfo.Text = text;
            }
        }

        private void comboBox_StatisticPlots_Study_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox_DescriptiveStatistic1_Groups.Items.Clear();
            comboBox_DescriptiveStatistic1_Szenario.Items.Clear();
            listBox_DescriptiveStatistic1_Subjects.Items.Clear();
            listBox_DescriptiveStatistic1_Turns.Items.Clear();
            listBox_DescriptiveStatistic1_Trials.Items.Clear();

            string[] groupNames =
                _mySqlWrapper.GetGroupNames(comboBox_DescriptiveStatistic1_Study.SelectedItem.ToString());
            if (groupNames != null)
            {
                listBox_DescriptiveStatistic1_Groups.Items.AddRange(groupNames);
                listBox_DescriptiveStatistic1_Groups.SelectedIndex = 0;
            }
        }

        private void listBox_StatisticPlots_Group_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox_DescriptiveStatistic1_Groups.SelectedItems.Count > 0)
            {
                comboBox_DescriptiveStatistic1_Szenario.Items.Clear();
                listBox_DescriptiveStatistic1_Subjects.Items.Clear();
                listBox_DescriptiveStatistic1_Turns.Items.Clear();
                listBox_DescriptiveStatistic1_Trials.Items.Clear();

                string study = comboBox_DescriptiveStatistic1_Study.SelectedItem.ToString();
                string[] groups = listBox_DescriptiveStatistic1_Groups.SelectedItems.Cast<string>().ToArray();

                string[] szenarioIntersect = _mySqlWrapper.GetSzenarioNames(study, groups[0]);
                for (int i = 1; i < groups.Length; i++)
                {
                    szenarioIntersect =
                        szenarioIntersect.Intersect(_mySqlWrapper.GetSzenarioNames(study, groups[i])).ToArray();
                }

                comboBox_DescriptiveStatistic1_Szenario.Items.AddRange(szenarioIntersect);
                comboBox_DescriptiveStatistic1_Szenario.SelectedIndex = 0;
            }
        }

        private void comboBox_StatisticPlots_Szenario_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox_DescriptiveStatistic1_Subjects.Items.Clear();
            listBox_DescriptiveStatistic1_Turns.Items.Clear();
            listBox_DescriptiveStatistic1_Trials.Items.Clear();

            string study = comboBox_DescriptiveStatistic1_Study.SelectedItem.ToString();
            string[] groups = listBox_DescriptiveStatistic1_Groups.SelectedItems.Cast<string>().ToArray();
            string szenario = comboBox_DescriptiveStatistic1_Szenario.SelectedItem.ToString();

            for (int i = 0; i < groups.Length; i++)
            {
                if (_mySqlWrapper != null)
                {
                    listBox_DescriptiveStatistic1_Subjects.Items.AddRange(
                        _mySqlWrapper.GetSubjectInformations(study, groups[i], szenario).ToArray());
                }
            }
            listBox_DescriptiveStatistic1_Subjects.SelectedIndex = 0;
        }

        private void listBox_StatisticPlots_Subject_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox_DescriptiveStatistic1_Turns.Items.Clear();
            listBox_DescriptiveStatistic1_Trials.Items.Clear();

            string study = comboBox_DescriptiveStatistic1_Study.SelectedItem.ToString();
            string[] groups = listBox_DescriptiveStatistic1_Groups.SelectedItems.Cast<string>().ToArray();
            string szenario = comboBox_DescriptiveStatistic1_Szenario.SelectedItem.ToString();
            int[] subjectIDs =
                listBox_DescriptiveStatistic1_Subjects.SelectedItems.Cast<SubjectInformationContainer>()
                                                      .Select(t => t.ID)
                                                      .ToArray();

            string[] turnIntersect = null;
            for (int i = 0; i < groups.Length; i++)
            {
                for (int j = 0; j < subjectIDs.Length; j++)
                {
                    string[] tempTurnString = _mySqlWrapper.GetTurns(study, groups[i], szenario, subjectIDs[j]);

                    if (tempTurnString != null)
                    {
                        if (turnIntersect == null)
                        {
                            turnIntersect = tempTurnString;
                        }
                        else
                        {
                            turnIntersect = turnIntersect.Intersect(tempTurnString).ToArray();
                        }
                    }
                }
            }

            if (turnIntersect != null)
            {
                listBox_DescriptiveStatistic1_Turns.Items.AddRange(turnIntersect);
            }

            listBox_DescriptiveStatistic1_Turns.SelectedIndex = 0;
        }

        private void listBox_DescriptiveStatistic1_Turns_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox_DescriptiveStatistic1_Trials.Items.Clear();

            string study = comboBox_DescriptiveStatistic1_Study.SelectedItem.ToString();
            string szenario = comboBox_DescriptiveStatistic1_Szenario.SelectedItem.ToString();
            bool showCatchTrials = checkBox_DescriptiveStatistic1_ShowCatchTrials.Checked;
            bool showCatchTrialsExclusivly = checkBox_DescriptiveStatistic1_ShowCatchTrialsExclusivly.Checked;

            string[] szenarioTrialNames = _mySqlWrapper.GetSzenarioTrials(study, szenario, showCatchTrials,
                                                                          showCatchTrialsExclusivly);

            if (szenarioTrialNames != null)
            {
                listBox_DescriptiveStatistic1_Trials.Items.AddRange(szenarioTrialNames);
            }

            listBox_DescriptiveStatistic1_Trials.SelectedIndex = 0;
        }

        private void button_StatisticPlots_AddSelected_Click(object sender, EventArgs e)
        {
            if (comboBox_DescriptiveStatistic1_Study.SelectedItem != null)
            {
                string study = comboBox_DescriptiveStatistic1_Study.SelectedItem.ToString();
                string[] groups = listBox_DescriptiveStatistic1_Groups.SelectedItems.Cast<string>().ToArray();
                string szenario = comboBox_DescriptiveStatistic1_Szenario.SelectedItem.ToString();
                SubjectInformationContainer[] subjects =
                    listBox_DescriptiveStatistic1_Subjects.SelectedItems.Cast<SubjectInformationContainer>().ToArray();
                string[] turns = listBox_DescriptiveStatistic1_Turns.SelectedItems.Cast<string>().ToArray();
                string[] trials = listBox_DescriptiveStatistic1_Trials.SelectedItems.Cast<string>().ToArray();

                foreach (string group in groups)
                {
                    foreach (SubjectInformationContainer subject in subjects)
                    {
                        foreach (string turn in turns)
                        {
                            if (_mySqlWrapper.GetTurns(study, group, szenario, subject.ID) != null)
                            {
                                if (listBox_DescriptiveStatistic1_SelectedTrials.Items.Count > 0)
                                {
                                    bool canBeUpdated = false;
                                    foreach (
                                        StatisticPlotContainer temp in
                                            listBox_DescriptiveStatistic1_SelectedTrials.Items)
                                    {
                                        if (temp.UpdateStatisticPlotContainer(study, group, szenario, subject, turn,
                                                                              trials))
                                        {
                                            typeof (ListBox).InvokeMember("RefreshItems",
                                                                          BindingFlags.NonPublic | BindingFlags.Instance |
                                                                          BindingFlags.InvokeMethod,
                                                                          null,
                                                                          listBox_DescriptiveStatistic1_SelectedTrials,
                                                                          new object[] {});
                                            canBeUpdated = true;
                                        }
                                    }

                                    if (!canBeUpdated)
                                    {
                                        listBox_DescriptiveStatistic1_SelectedTrials.Items.Add(
                                            new StatisticPlotContainer(study, group, szenario, subject, turn, trials));
                                    }
                                }
                                else
                                {
                                    listBox_DescriptiveStatistic1_SelectedTrials.Items.Add(
                                        new StatisticPlotContainer(study, group, szenario, subject, turn, trials));
                                }
                            }
                        }
                    }
                }
            }
        }

        private void button_StatisticPlots_ClearSelected_Click(object sender, EventArgs e)
        {
            while (listBox_DescriptiveStatistic1_SelectedTrials.SelectedItems.Count > 0)
            {
                listBox_DescriptiveStatistic1_SelectedTrials.Items.Remove(
                    listBox_DescriptiveStatistic1_SelectedTrials.SelectedItem);
            }
        }

        private void button_StatisticPlots_ClearAll_Click(object sender, EventArgs e)
        {
            listBox_DescriptiveStatistic1_SelectedTrials.Items.Clear();
        }

        private void button_StatisticPlots_PlotMeanStd_Click(object sender, EventArgs e)
        {
            WriteProgressInfo("Getting data...");
            if (listBox_DescriptiveStatistic1_SelectedTrials.Items.Count > 0)
            {
                bool isValid = true;
                List<int> trialList =
                    listBox_DescriptiveStatistic1_SelectedTrials.Items.Cast<StatisticPlotContainer>()
                                                                .ElementAt(0)
                                                                .Trials;

                if (
                    listBox_DescriptiveStatistic1_SelectedTrials.Items.Cast<StatisticPlotContainer>()
                                                                .Any(temp => !trialList.SequenceEqual(temp.Trials)))
                {
                    WriteToLogBox("Trial selections are not equal!");
                    isValid = false;
                }

                if (isValid)
                {
                    int meanCounter;
                    var data = new double[trialList.Count,listBox_DescriptiveStatistic1_SelectedTrials.Items.Count];

                    for (meanCounter = 0;
                         meanCounter < listBox_DescriptiveStatistic1_SelectedTrials.Items.Count;
                         meanCounter++)
                    {
                        SetProgressBarValue((100.0/listBox_DescriptiveStatistic1_SelectedTrials.Items.Count)*meanCounter);
                        StatisticPlotContainer temp =
                            listBox_DescriptiveStatistic1_SelectedTrials.Items.Cast<StatisticPlotContainer>()
                                                                        .ElementAt(meanCounter);

                        DateTime turn = _mySqlWrapper.GetTurnDateTime(temp.Study, temp.Group, temp.Szenario,
                                                                      temp.Subject.ID,
                                                                      Convert.ToInt32(temp.Turn.Substring("Turn".Length)));
                        DataSet statisticDataSet = _mySqlWrapper.GetStatisticDataSet(temp.Study, temp.Group,
                                                                                     temp.Szenario, temp.Subject.ID,
                                                                                     turn);

                        int trialListCounter = 0;
                        foreach (DataRow row in statisticDataSet.Tables[0].Rows)
                        {
                            int szenarioTrialNumber = Convert.ToInt32(row["szenario_trial_number"]);
                            if (trialList.Contains(szenarioTrialNumber))
                            {
                                switch (comboBox_DescriptiveStatistic1_DataTypeSelect.SelectedItem.ToString())
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

                    switch (comboBox_DescriptiveStatistic1_DataTypeSelect.SelectedItem.ToString())
                    {
                        case "Vector correlation":
                            _myMatlabWrapper.CreateStatisticFigure("Velocity Vector Correlation plot", "dataPlot",
                                                                   "fit(transpose([1:1:length(dataPlot)]),transpose(dataPlot),'" +
                                                                   textBox_DescriptiveStatistic1_FitEquation.Text + "')",
                                                                   "dataStdPlot", "[Trial]",
                                                                   "Velocity Vector Correlation", 1, dataPlot.Length,
                                                                   0.5, 0.9,
                                                                   checkBox_DescriptiveStatistic1_PlotFit.Checked,
                                                                   checkBox_DescriptiveStatistic1_PlotErrorbars.Checked);
                            break;

                        case "Perpendicular distance 300ms - Abs":
                            _myMatlabWrapper.CreateStatisticFigure("PD300 abs plot", "dataPlot",
                                                                   "fit(transpose([1:1:length(dataPlot)]),transpose(dataPlot),'" +
                                                                   textBox_DescriptiveStatistic1_FitEquation.Text + "')",
                                                                   "dataStdPlot", "[Trial]", "PD300 [m]", 1,
                                                                   dataPlot.Length, 0, 0.05,
                                                                   checkBox_DescriptiveStatistic1_PlotFit.Checked,
                                                                   checkBox_DescriptiveStatistic1_PlotErrorbars.Checked);
                            break;

                        case "Mean perpendicular distance - Abs":
                            _myMatlabWrapper.CreateStatisticFigure("MeanPD abs plot", "dataPlot",
                                                                   "fit(transpose([1:1:length(dataPlot)]),transpose(dataPlot),'" +
                                                                   textBox_DescriptiveStatistic1_FitEquation.Text + "')",
                                                                   "dataStdPlot", "[Trial]", "MeanPD [m]", 1,
                                                                   dataPlot.Length, 0, 0.05,
                                                                   checkBox_DescriptiveStatistic1_PlotFit.Checked,
                                                                   checkBox_DescriptiveStatistic1_PlotErrorbars.Checked);
                            break;

                        case "Max perpendicular distance - Abs":
                            _myMatlabWrapper.CreateStatisticFigure("MaxPD abs plot", "dataPlot",
                                                                   "fit(transpose([1:1:length(dataPlot)]),transpose(dataPlot),'" +
                                                                   textBox_DescriptiveStatistic1_FitEquation.Text + "')",
                                                                   "dataStdPlot", "[Trial]", "MaxPD [m]", 1,
                                                                   dataPlot.Length, 0, 0.05,
                                                                   checkBox_DescriptiveStatistic1_PlotFit.Checked,
                                                                   checkBox_DescriptiveStatistic1_PlotErrorbars.Checked);
                            break;

                        case "Perpendicular distance 300ms - Sign":
                            _myMatlabWrapper.CreateStatisticFigure("PD300 sign plot", "dataPlot",
                                                                   "fit(transpose([1:1:length(dataPlot)]),transpose(dataPlot),'" +
                                                                   textBox_DescriptiveStatistic1_FitEquation.Text + "')",
                                                                   "dataStdPlot", "[Trial]", "PD300 [m]", 1,
                                                                   dataPlot.Length, -0.05, 0.05,
                                                                   checkBox_DescriptiveStatistic1_PlotFit.Checked,
                                                                   checkBox_DescriptiveStatistic1_PlotErrorbars.Checked);
                            break;

                        case "Max perpendicular distance - Sign":
                            _myMatlabWrapper.CreateStatisticFigure("MaxPD sign plot", "dataPlot",
                                                                   "fit(transpose([1:1:length(dataPlot)]),transpose(dataPlot),'" +
                                                                   textBox_DescriptiveStatistic1_FitEquation.Text + "')",
                                                                   "dataStdPlot", "[Trial]", "MaxPD [m]", 1,
                                                                   dataPlot.Length, -0.05, 0.05,
                                                                   checkBox_DescriptiveStatistic1_PlotFit.Checked,
                                                                   checkBox_DescriptiveStatistic1_PlotErrorbars.Checked);
                            break;

                        case "Trajectory length abs":
                            _myMatlabWrapper.CreateStatisticFigure("Trajectory Length plot", "dataPlot",
                                                                   "fit(transpose([1:1:length(dataPlot)]),transpose(dataPlot),'" +
                                                                   textBox_DescriptiveStatistic1_FitEquation.Text + "')",
                                                                   "dataStdPlot", "[Trial]", "Trajectory Length [m]", 1,
                                                                   dataPlot.Length, 0.07, 0.2,
                                                                   checkBox_DescriptiveStatistic1_PlotFit.Checked,
                                                                   checkBox_DescriptiveStatistic1_PlotErrorbars.Checked);
                            break;

                        case "Trajectory length ratio":
                            _myMatlabWrapper.CreateStatisticFigure("Trajectory Length Ratio plot", "dataPlot",
                                                                   "fit(transpose([1:1:length(dataPlot)]),transpose(dataPlot),'" +
                                                                   textBox_DescriptiveStatistic1_FitEquation.Text + "')",
                                                                   "dataStdPlot", "[Trial]", "Trajectory Length Ratio",
                                                                   1, dataPlot.Length, 0.2, 1.8,
                                                                   checkBox_DescriptiveStatistic1_PlotFit.Checked,
                                                                   checkBox_DescriptiveStatistic1_PlotErrorbars.Checked);
                            break;

                        case "Enclosed area":
                            _myMatlabWrapper.CreateStatisticFigure("Enclosed area plot", "dataPlot",
                                                                   "fit(transpose([1:1:length(dataPlot)]),transpose(dataPlot),'" +
                                                                   textBox_DescriptiveStatistic1_FitEquation.Text + "')",
                                                                   "dataStdPlot", "[Trial]", "Enclosed Area [m²]", 1,
                                                                   dataPlot.Length, 0, 0.002,
                                                                   checkBox_DescriptiveStatistic1_PlotFit.Checked,
                                                                   checkBox_DescriptiveStatistic1_PlotErrorbars.Checked);
                            break;

                        case "RMSE":
                            _myMatlabWrapper.CreateStatisticFigure("Root Mean Square Error plot", "dataPlot",
                                                                   "fit(transpose([1:1:length(dataPlot)]),transpose(dataPlot),'" +
                                                                   textBox_DescriptiveStatistic1_FitEquation.Text + "')",
                                                                   "dataStdPlot", "[Trial]", "Root Mean Square Error", 1,
                                                                   dataPlot.Length, 0, 0.1,
                                                                   checkBox_DescriptiveStatistic1_PlotFit.Checked,
                                                                   checkBox_DescriptiveStatistic1_PlotErrorbars.Checked);
                            break;
                    }

                    _myMatlabWrapper.ClearWorkspace();
                }
            }
            WriteProgressInfo("Ready");
            SetProgressBarValue(0);
        }

        private void button_StatisticPlots_AddAll_Click(object sender, EventArgs e)
        {
            if (comboBox_DescriptiveStatistic1_Study.SelectedItem != null)
            {
                string study = comboBox_DescriptiveStatistic1_Study.SelectedItem.ToString();
                string[] groups = listBox_DescriptiveStatistic1_Groups.SelectedItems.Cast<string>().ToArray();
                string szenario = comboBox_DescriptiveStatistic1_Szenario.SelectedItem.ToString();
                SubjectInformationContainer[] subjects =
                    listBox_DescriptiveStatistic1_Subjects.SelectedItems.Cast<SubjectInformationContainer>().ToArray();
                string[] turns = listBox_DescriptiveStatistic1_Turns.SelectedItems.Cast<string>().ToArray();
                string[] trials = listBox_DescriptiveStatistic1_Trials.Items.Cast<string>().ToArray();

                foreach (string group in groups)
                {
                    foreach (SubjectInformationContainer subject in subjects)
                    {
                        foreach (string turn in turns)
                        {
                            if (_mySqlWrapper.GetTurns(study, group, szenario, subject.ID) != null)
                            {
                                if (listBox_DescriptiveStatistic1_SelectedTrials.Items.Count > 0)
                                {
                                    bool canBeUpdated = false;
                                    foreach (
                                        StatisticPlotContainer temp in
                                            listBox_DescriptiveStatistic1_SelectedTrials.Items)
                                    {
                                        if (temp.UpdateStatisticPlotContainer(study, group, szenario, subject, turn,
                                                                              trials))
                                        {
                                            typeof (ListBox).InvokeMember("RefreshItems",
                                                                          BindingFlags.NonPublic | BindingFlags.Instance |
                                                                          BindingFlags.InvokeMethod,
                                                                          null,
                                                                          listBox_DescriptiveStatistic1_SelectedTrials,
                                                                          new object[] {});
                                            canBeUpdated = true;
                                        }
                                    }

                                    if (!canBeUpdated)
                                    {
                                        listBox_DescriptiveStatistic1_SelectedTrials.Items.Add(
                                            new StatisticPlotContainer(study, group, szenario, subject, turn, trials));
                                    }
                                }
                                else
                                {
                                    listBox_DescriptiveStatistic1_SelectedTrials.Items.Add(
                                        new StatisticPlotContainer(study, group, szenario, subject, turn, trials));
                                }
                            }
                        }
                    }
                }
            }
        }

        private void button_showFaultyTrials_Click(object sender, EventArgs e)
        {
            List<object[]> faultyTrialInfo = _mySqlWrapper.GetFaultyTrialInformation();

            if (faultyTrialInfo != null)
            {
                List<string[]> cache = faultyTrialInfo.Select(t => new string[]
                    {
                        Convert.ToString(t[3]), Convert.ToString(t[4]), Convert.ToString(t[6]), Convert.ToString(t[5]),
                        Convert.ToString(Convert.ToDateTime(t[7])), Convert.ToString(Convert.ToInt32(t[8]))
                    }).ToList();

                WriteToLogBox(
                    "------------------------------------------------------- Faulty trial list -------------------------------------------------------");
                WriteToLogBox(
                    cache.OrderBy(t => t[4])
                         .Select(
                             t => t[0] + " - " + t[1] + " - " + t[2] + " - " + t[3] + " - " + t[4] + " - Trial " + t[5])
                         .ToArray());
                WriteToLogBox(
                    "---------------------------------------------------------------------------------------------------------------------------------");
            }
        }

        private void tabPage_DescriptiveStatistic1_Enter(object sender, EventArgs e)
        {
            comboBox_DescriptiveStatistic1_DataTypeSelect.SelectedIndex = 0;

            comboBox_DescriptiveStatistic1_Study.Items.Clear();
            listBox_DescriptiveStatistic1_Groups.Items.Clear();
            comboBox_DescriptiveStatistic1_Szenario.Items.Clear();
            listBox_DescriptiveStatistic1_Subjects.Items.Clear();
            listBox_DescriptiveStatistic1_Turns.Items.Clear();
            listBox_DescriptiveStatistic1_Trials.Items.Clear();

            string[] studyNames = _mySqlWrapper.GetStudyNames();
            if (studyNames != null)
            {
                comboBox_DescriptiveStatistic1_Study.Items.AddRange(studyNames);
                comboBox_DescriptiveStatistic1_Study.SelectedIndex = 0;
            }
        }

        private void checkBox_DescriptiveStatistic1_ShowCatchTrials_CheckedChanged(object sender, EventArgs e)
        {
            listBox_DescriptiveStatistic1_Turns_SelectedIndexChanged(this, new EventArgs());
            checkBox_DescriptiveStatistic1_ShowCatchTrialsExclusivly.Enabled =
                checkBox_DescriptiveStatistic1_ShowCatchTrials.Checked;
        }

        private void comboBox_DescriptiveStatistic2_Study_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox_DescriptiveStatistic2_Groups.Items.Clear();
            comboBox_DescriptiveStatistic2_Szenario.Items.Clear();
            listBox_DescriptiveStatistic2_Subjects.Items.Clear();
            listBox_DescriptiveStatistic2_Turns.Items.Clear();
            listBox_DescriptiveStatistic2_Trials.Items.Clear();

            string[] groupNames =
                _mySqlWrapper.GetGroupNames(comboBox_DescriptiveStatistic2_Study.SelectedItem.ToString());
            if (groupNames != null)
            {
                listBox_DescriptiveStatistic2_Groups.Items.AddRange(groupNames);
                listBox_DescriptiveStatistic2_Groups.SelectedIndex = 0;
            }
        }

        private void listBox_DescriptiveStatistic2_Groups_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox_DescriptiveStatistic2_Groups.SelectedItems.Count > 0)
            {
                comboBox_DescriptiveStatistic2_Szenario.Items.Clear();
                listBox_DescriptiveStatistic2_Subjects.Items.Clear();
                listBox_DescriptiveStatistic2_Turns.Items.Clear();
                listBox_DescriptiveStatistic2_Trials.Items.Clear();

                string study = comboBox_DescriptiveStatistic2_Study.SelectedItem.ToString();
                string[] groups = listBox_DescriptiveStatistic2_Groups.SelectedItems.Cast<string>().ToArray();

                string[] szenarioIntersect = _mySqlWrapper.GetSzenarioNames(study, groups[0]);
                for (int i = 1; i < groups.Length; i++)
                {
                    szenarioIntersect =
                        szenarioIntersect.Intersect(_mySqlWrapper.GetSzenarioNames(study, groups[i])).ToArray();
                }

                if (szenarioIntersect != null)
                {
                    comboBox_DescriptiveStatistic2_Szenario.Items.AddRange(szenarioIntersect);
                }

                comboBox_DescriptiveStatistic2_Szenario.SelectedIndex = 0;
            }
        }

        private void comboBox_DescriptiveStatistic2_Szenario_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox_DescriptiveStatistic2_Subjects.Items.Clear();
            listBox_DescriptiveStatistic2_Turns.Items.Clear();
            listBox_DescriptiveStatistic2_Trials.Items.Clear();

            string study = comboBox_DescriptiveStatistic2_Study.SelectedItem.ToString();
            string[] groups = listBox_DescriptiveStatistic2_Groups.SelectedItems.Cast<string>().ToArray();
            string szenario = comboBox_DescriptiveStatistic2_Szenario.SelectedItem.ToString();

            for (int i = 0; i < groups.Length; i++)
            {
                if (_mySqlWrapper != null)
                {
                    listBox_DescriptiveStatistic2_Subjects.Items.AddRange(
                        _mySqlWrapper.GetSubjectInformations(study, groups[i], szenario).ToArray());
                }
            }
            listBox_DescriptiveStatistic2_Subjects.SelectedIndex = 0;
        }

        private void listBox_DescriptiveStatistic2_Subject_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox_DescriptiveStatistic2_Turns.Items.Clear();
            listBox_DescriptiveStatistic2_Trials.Items.Clear();

            string study = comboBox_DescriptiveStatistic2_Study.SelectedItem.ToString();
            string[] groups = listBox_DescriptiveStatistic2_Groups.SelectedItems.Cast<string>().ToArray();
            string szenario = comboBox_DescriptiveStatistic2_Szenario.SelectedItem.ToString();
            SubjectInformationContainer[] subjects =
                listBox_DescriptiveStatistic2_Subjects.SelectedItems.Cast<SubjectInformationContainer>().ToArray();

            string[] turnIntersect = null;
            for (int i = 0; i < groups.Length; i++)
            {
                for (int j = 0; j < subjects.Length; j++)
                {
                    string[] tempTurnString = _mySqlWrapper.GetTurns(study, groups[i], szenario, subjects[j].ID);

                    if (tempTurnString != null)
                    {
                        if (turnIntersect == null)
                        {
                            turnIntersect = tempTurnString;
                        }
                        else
                        {
                            turnIntersect = turnIntersect.Intersect(tempTurnString).ToArray();
                        }
                    }
                }
            }

            if (turnIntersect != null)
            {
                listBox_DescriptiveStatistic2_Turns.Items.AddRange(turnIntersect);
            }

            listBox_DescriptiveStatistic2_Turns.SelectedIndex = 0;
        }

        private void listBox_DescriptiveStatistic2_Turns_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox_DescriptiveStatistic2_Trials.Items.Clear();

            string study = comboBox_DescriptiveStatistic2_Study.SelectedItem.ToString();
            string szenario = comboBox_DescriptiveStatistic2_Szenario.SelectedItem.ToString();
            bool showCatchTrials = checkBox_DescriptiveStatistic2_ShowCatchTrials.Checked;
            bool showCatchTrialsExclusivly = checkBox_DescriptiveStatistic2_ShowCatchTrialsExclusivly.Checked;

            string[] szenarioTrialNames = _mySqlWrapper.GetSzenarioTrials(study, szenario, showCatchTrials,
                                                                          showCatchTrialsExclusivly);

            if (szenarioTrialNames != null)
            {
                listBox_DescriptiveStatistic2_Trials.Items.AddRange(szenarioTrialNames);
            }

            listBox_DescriptiveStatistic2_Trials.SelectedIndex = 0;
        }

        private void checkBox_DescriptiveStatistic2_ShowCatchTrials_CheckedChanged(object sender, EventArgs e)
        {
            listBox_DescriptiveStatistic2_Turns_SelectedIndexChanged(this, new EventArgs());
            checkBox_DescriptiveStatistic2_ShowCatchTrialsExclusivly.Enabled =
                checkBox_DescriptiveStatistic2_ShowCatchTrials.Checked;
        }

        private void button_DescriptiveStatistic2_AddSelected_Click(object sender, EventArgs e)
        {
            if (comboBox_DescriptiveStatistic2_Study.SelectedItem != null)
            {
                string study = comboBox_DescriptiveStatistic2_Study.SelectedItem.ToString();
                string[] groups = listBox_DescriptiveStatistic2_Groups.SelectedItems.Cast<string>().ToArray();
                string szenario = comboBox_DescriptiveStatistic2_Szenario.SelectedItem.ToString();
                SubjectInformationContainer[] subjects =
                    listBox_DescriptiveStatistic2_Subjects.SelectedItems.Cast<SubjectInformationContainer>().ToArray();
                string[] turns = listBox_DescriptiveStatistic2_Turns.SelectedItems.Cast<string>().ToArray();
                string[] trials = listBox_DescriptiveStatistic2_Trials.SelectedItems.Cast<string>().ToArray();

                foreach (string group in groups)
                {
                    foreach (SubjectInformationContainer subject in subjects)
                    {
                        foreach (string turn in turns)
                        {
                            listBox_DescriptiveStatistic2_SelectedTrials.Items.Add(new StatisticPlotContainer(study,
                                                                                                              group,
                                                                                                              szenario,
                                                                                                              subject,
                                                                                                              turn,
                                                                                                              trials));
                        }
                    }
                }
            }
        }

        private void button_DescriptiveStatistic2_AddAll_Click(object sender, EventArgs e)
        {
            if (comboBox_DescriptiveStatistic2_Study.SelectedItem != null)
            {
                string study = comboBox_DescriptiveStatistic2_Study.SelectedItem.ToString();
                string[] groups = listBox_DescriptiveStatistic2_Groups.SelectedItems.Cast<string>().ToArray();
                string szenario = comboBox_DescriptiveStatistic2_Szenario.SelectedItem.ToString();
                SubjectInformationContainer[] subjects =
                    listBox_DescriptiveStatistic2_Subjects.SelectedItems.Cast<SubjectInformationContainer>().ToArray();
                string[] turns = listBox_DescriptiveStatistic2_Turns.SelectedItems.Cast<string>().ToArray();
                string[] trials = listBox_DescriptiveStatistic2_Trials.Items.Cast<string>().ToArray();

                foreach (string group in groups)
                {
                    foreach (SubjectInformationContainer subject in subjects)
                    {
                        foreach (string turn in turns)
                        {
                            listBox_DescriptiveStatistic2_SelectedTrials.Items.Add(new StatisticPlotContainer(study,
                                                                                                              group,
                                                                                                              szenario,
                                                                                                              subject,
                                                                                                              turn,
                                                                                                              trials));
                        }
                    }
                }
            }
        }

        private void button_DescriptiveStatistic2_ClearSelected_Click(object sender, EventArgs e)
        {
            while (listBox_DescriptiveStatistic2_SelectedTrials.SelectedItems.Count > 0)
            {
                listBox_DescriptiveStatistic2_SelectedTrials.Items.Remove(
                    listBox_DescriptiveStatistic2_SelectedTrials.SelectedItem);
            }
        }

        private void button_DescriptiveStatistic2_ClearAll_Click(object sender, EventArgs e)
        {
            listBox_DescriptiveStatistic2_SelectedTrials.Items.Clear();
        }

        private void tabPage_DescriptiveStatistic2_Enter(object sender, EventArgs e)
        {
            comboBox_DescriptiveStatistic2_DataTypeSelect.SelectedIndex = 0;

            comboBox_DescriptiveStatistic2_Study.Items.Clear();
            listBox_DescriptiveStatistic2_Groups.Items.Clear();
            comboBox_DescriptiveStatistic2_Szenario.Items.Clear();
            listBox_DescriptiveStatistic2_Subjects.Items.Clear();
            listBox_DescriptiveStatistic2_Turns.Items.Clear();
            listBox_DescriptiveStatistic2_Trials.Items.Clear();

            string[] studyNames = _mySqlWrapper.GetStudyNames();

            if (studyNames != null)
            {
                comboBox_DescriptiveStatistic2_Study.Items.AddRange(studyNames);
                comboBox_DescriptiveStatistic2_Study.SelectedIndex = 0;
            }
        }

        private void button_DescriptiveStatistic2_CalculateMeanValues_Click(object sender, EventArgs e)
        {
            WriteProgressInfo("Getting data...");
            saveFileDialog = new SaveFileDialog();
            saveFileDialog.Reset();
            saveFileDialog.Title = @"Save mean data file";
            saveFileDialog.AddExtension = true;
            saveFileDialog.DefaultExt = ".csv";
            saveFileDialog.Filter = @"DataFiles (*.csv)|.csv";
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.FileName = DateTime.Now.Year.ToString("0000")
                                      + "."
                                      + DateTime.Now.Month.ToString("00")
                                      + "."
                                      + DateTime.Now.Day.ToString("00")
                                      + "-"
                                      + DateTime.Now.Hour.ToString("00")
                                      + "."
                                      + DateTime.Now.Minute.ToString("00")
                                      + "-mean-"
                                      + comboBox_DescriptiveStatistic2_DataTypeSelect.SelectedItem
                                      + "-data";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (listBox_DescriptiveStatistic2_SelectedTrials.Items.Count > 0)
                {
                    int trialListCounter = 0;
                    List<int> trialList =
                        listBox_DescriptiveStatistic2_SelectedTrials.Items.Cast<StatisticPlotContainer>()
                                                                    .ElementAt(0)
                                                                    .Trials;
                    var data = new double[trialList.Count,listBox_DescriptiveStatistic2_SelectedTrials.Items.Count];

                    for (int meanCounter = 0;
                         meanCounter < listBox_DescriptiveStatistic2_SelectedTrials.Items.Count;
                         meanCounter++)
                    {
                        SetProgressBarValue((100.0/listBox_DescriptiveStatistic2_SelectedTrials.Items.Count)*meanCounter);
                        StatisticPlotContainer temp =
                            listBox_DescriptiveStatistic2_SelectedTrials.Items.Cast<StatisticPlotContainer>()
                                                                        .ElementAt(meanCounter);

                        DateTime turn = _mySqlWrapper.GetTurnDateTime(temp.Study, temp.Group, temp.Szenario,
                                                                      temp.Subject.ID,
                                                                      Convert.ToInt32(temp.Turn.Substring("Turn".Length)));
                        DataSet statisticDataSet = _mySqlWrapper.GetStatisticDataSet(temp.Study, temp.Group,
                                                                                     temp.Szenario, temp.Subject.ID,
                                                                                     turn);

                        trialListCounter = 0;
                        foreach (DataRow row in statisticDataSet.Tables[0].Rows)
                        {
                            int szenarioTrialNumber = Convert.ToInt32(row["szenario_trial_number"]);
                            if (trialList.Contains(szenarioTrialNumber))
                            {
                                switch (comboBox_DescriptiveStatistic2_DataTypeSelect.SelectedItem.ToString())
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
                    var meanDataFileStream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
                    var meanDataFileWriter = new StreamWriter(meanDataFileStream);
                    cache.Add("Study;Group;Szenario;Subject;Turn;Trials;Mean;Std");

                    cache.AddRange(
                        listBox_DescriptiveStatistic2_SelectedTrials.Items.Cast<object>()
                                                                    .Select(
                                                                        (t, i) =>
                                                                        listBox_DescriptiveStatistic2_SelectedTrials
                                                                            .Items.Cast<StatisticPlotContainer>()
                                                                            .ElementAt(i))
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

                    for (int i = 0; i < cache.Count(); i++)
                    {
                        meanDataFileWriter.WriteLine(cache[i]);
                    }

                    meanDataFileWriter.Close();
                    meanDataFileStream.Close();

                    _myMatlabWrapper.ClearWorkspace();
                }
            }
            WriteProgressInfo("Ready");
            SetProgressBarValue(0);
        }

        private void button_DescriptiveStatistic1_ExportData_Click(object sender, EventArgs e)
        {
            WriteProgressInfo("Getting data...");
            saveFileDialog = new SaveFileDialog();
            saveFileDialog.Reset();
            saveFileDialog.Title = @"Save mean data file";
            saveFileDialog.AddExtension = true;
            saveFileDialog.DefaultExt = ".csv";
            saveFileDialog.Filter = @"DataFiles (*.csv)|.csv";
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.FileName = DateTime.Now.Year.ToString("0000")
                                      + "."
                                      + DateTime.Now.Month.ToString("00")
                                      + "."
                                      + DateTime.Now.Day.ToString("00")
                                      + "-"
                                      + DateTime.Now.Hour.ToString("00")
                                      + "."
                                      + DateTime.Now.Minute.ToString("00")
                                      + "-mean-"
                                      + comboBox_DescriptiveStatistic1_DataTypeSelect.SelectedItem
                                      + "-data";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (listBox_DescriptiveStatistic1_SelectedTrials.Items.Count > 0)
                {
                    bool isValid = true;
                    List<int> trialList =
                        listBox_DescriptiveStatistic1_SelectedTrials.Items.Cast<StatisticPlotContainer>()
                                                                    .ElementAt(0)
                                                                    .Trials;

                    if (
                        listBox_DescriptiveStatistic1_SelectedTrials.Items.Cast<StatisticPlotContainer>()
                                                                    .Any(temp => !trialList.SequenceEqual(temp.Trials)))
                    {
                        WriteToLogBox("Trial selections are not equal!");
                        isValid = false;
                    }

                    if (isValid)
                    {
                        int meanCounter;
                        int trialListCounter = 0;
                        var data = new double[trialList.Count,listBox_DescriptiveStatistic1_SelectedTrials.Items.Count];

                        for (meanCounter = 0;
                             meanCounter < listBox_DescriptiveStatistic1_SelectedTrials.Items.Count;
                             meanCounter++)
                        {
                            SetProgressBarValue((100.0/listBox_DescriptiveStatistic1_SelectedTrials.Items.Count)*
                                                meanCounter);
                            StatisticPlotContainer temp =
                                listBox_DescriptiveStatistic1_SelectedTrials.Items.Cast<StatisticPlotContainer>()
                                                                            .ElementAt(meanCounter);

                            DateTime turn = _mySqlWrapper.GetTurnDateTime(temp.Study, temp.Group, temp.Szenario,
                                                                          temp.Subject.ID,
                                                                          Convert.ToInt32(
                                                                              temp.Turn.Substring("Turn".Length)));
                            DataSet statisticDataSet = _mySqlWrapper.GetStatisticDataSet(temp.Study, temp.Group,
                                                                                         temp.Szenario, temp.Subject.ID,
                                                                                         turn);

                            trialListCounter = 0;
                            for (int rowCounter = 0; rowCounter < statisticDataSet.Tables[0].Rows.Count; rowCounter++)
                            {
                                DataRow row = statisticDataSet.Tables[0].Rows[rowCounter];

                                int szenarioTrialNumber = Convert.ToInt32(row["szenario_trial_number"]);
                                if (trialList.Contains(szenarioTrialNumber))
                                {
                                    switch (comboBox_DescriptiveStatistic1_DataTypeSelect.SelectedItem.ToString())
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
                        var meanDataFileStream = new FileStream(saveFileDialog.FileName, FileMode.Create,
                                                                FileAccess.Write);
                        var meanDataFileWriter = new StreamWriter(meanDataFileStream);

                        string personNames = "";
                        for (int i = 0; i < listBox_DescriptiveStatistic1_SelectedTrials.Items.Count; i++)
                        {
                            StatisticPlotContainer tempStatisticPlotContainer =
                                listBox_DescriptiveStatistic1_SelectedTrials.Items.Cast<StatisticPlotContainer>()
                                                                            .ElementAt(i);
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
            }
            WriteProgressInfo("Ready");
            SetProgressBarValue(0);
        }

        private void checkBox_PauseThread_CheckedChanged(object sender, EventArgs e)
        {
            ThreadManager.Pause = checkBox_PauseThread.Checked;
        }

        private void button_DataManipulation_UpdateGroupID_Click(object sender, EventArgs e)
        {
            _mySqlWrapper.ChangeGroupID(
                Convert.ToInt32(textBox_DataManipulation_OldGroupID.Text),
                Convert.ToInt32(textBox_DataManipulation_NewGroupID.Text)
                );
        }

        private void button_DataManipulation_UpdateSubjectID_Click(object sender, EventArgs e)
        {
            _mySqlWrapper.ChangeSubjectID(
                Convert.ToInt32(textBox_DataManipulation_OldSubjectID.Text),
                Convert.ToInt32(textBox_DataManipulation_NewSubjectID.Text)
                );
        }

        private void button_DataManipulation_UpdateGroupName_Click(object sender, EventArgs e)
        {
            _mySqlWrapper.ChangeGroupName(
                Convert.ToInt32(textBox_DataManipulation_GroupID.Text),
                textBox_DataManipulation_NewGroupName.Text
                );
        }

        private void button_DataManipulation_UpdateSubjectName_Click(object sender, EventArgs e)
        {
            _mySqlWrapper.ChangeSubjectName(
                Convert.ToInt32(textBox_DataManipulation_SubjectID.Text),
                textBox_DataManipulation_NewSubjectName.Text
                );
        }

        private void button_ImportMeasureFiles_Click(object sender, EventArgs e)
        {
            var newThread = new Thread(delegate()
                {
                    while (ThreadManager.GetIndex(Thread.CurrentThread) != 0)
                    {
                        Thread.Sleep(100);
                    }
                    EnableTabPages(false);
                    SetProgressBarValue(0);

                    for (int files = 0; files < listBox_Import_SelectedMeasureFiles.Items.Count; files++)
                    {
                        while (ThreadManager.Pause)
                        {
                            Thread.Sleep(100);
                        }

                        SetProgressBarValue((100.0/listBox_Import_SelectedMeasureFiles.Items.Count)*files);

                        string filename = listBox_Import_SelectedMeasureFiles.Items[files].ToString();

                        string tempFileHash = Md5.ComputeHash(filename);

                        if (!_mySqlWrapper.CheckIfMeasureFileHashExists(tempFileHash))
                        {
                            var myDataContainter = new DataContainer();
                            var myParser = new MeasureFileParser(myDataContainter, this);

                            if (myParser.ParseFile(filename))
                            {
                                WriteProgressInfo("Running multicore-calculation preparation...");

                                #region MultiCore preparation

                                var multiCoreThreads = new List<Thread>();

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

                                WriteProgressInfo("Running duplicate entry detection...");

                                #region Duplicate entry detection

                                for (int core = 0; core < Environment.ProcessorCount; core++)
                                {
                                    int coreVar = core;
                                    multiCoreThreads.Add(new Thread(delegate()
                                        {
                                            if (trialCoreDistribution.Length > coreVar)
                                            {
                                                var threadTrials = new List<int>(trialCoreDistribution[coreVar]);

                                                for (int i = 0; i < threadTrials.Count(); i++)
                                                {
                                                    List<MeasureDataContainer> tempRawData;
                                                    lock (myDataContainter)
                                                    {
                                                        tempRawData =
                                                            new List<MeasureDataContainer>(
                                                                myDataContainter.MeasureDataRaw.Where(
                                                                    t =>
                                                                    t.SzenarioTrialNumber == threadTrials.ElementAt(i))
                                                                                .OrderBy(t => t.TimeStamp));
                                                    }

                                                    int entryCount = tempRawData.Select(t => t.TimeStamp.Ticks).Count();
                                                    int entryUniqueCount =
                                                        tempRawData.Select(t => t.TimeStamp.Ticks).Distinct().Count();

                                                    if (entryCount != entryUniqueCount)
                                                    {
                                                        lock (myDataContainter)
                                                        {
                                                            List<MeasureDataContainer> tempList =
                                                                myDataContainter.MeasureDataRaw.Where(
                                                                    t =>
                                                                    t.SzenarioTrialNumber == threadTrials.ElementAt(i))
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
                                                            WriteToLogBox("Fixed error at time-stamp \"" +
                                                                          errorEntry.TimeStamp.ToString(
                                                                              "hh:mm:ss.fffffff") + "\" in file \"" +
                                                                          filename + "\"");
                                                            tempRawData.RemoveAt(maxIndex + 1);
                                                            errorDetected = true;
                                                        }
                                                    } while (errorDetected);
                                                }
                                            }
                                        }));

                                    foreach (Thread t in multiCoreThreads)
                                    {
                                        t.Start();
                                    }

                                    foreach (Thread t in multiCoreThreads)
                                    {
                                        t.Join();
                                    }

                                    multiCoreThreads.Clear();
                                }

                                #endregion

                                WriteProgressInfo("Filtering data...");

                                #region Butterworth filter

                                int samplesPerSecond = Convert.ToInt32(textBox_Import_SamplesPerSec.Text);
                                int filterOrder = Convert.ToInt32(textBox_Import_FilterOrder.Text);
                                int cutoffFreq = Convert.ToInt32(textBox_Import_CutoffFreq.Text);
                                int velocityCuttingThreshold = Convert.ToInt32(textBox_Import_PercentPeakVelocity.Text);

                                _myMatlabWrapper.SetWorkspaceData("filterOrder", Convert.ToDouble(filterOrder));
                                _myMatlabWrapper.SetWorkspaceData("cutoffFreq", Convert.ToDouble(cutoffFreq));
                                _myMatlabWrapper.SetWorkspaceData("samplesPerSecond", Convert.ToDouble(samplesPerSecond));
                                _myMatlabWrapper.Execute(
                                    "[b,a] = butter(filterOrder,(cutoffFreq/(samplesPerSecond/2)));");

                                for (int core = 0; core < Environment.ProcessorCount; core++)
                                {
                                    int coreVar = core;
                                    multiCoreThreads.Add(new Thread(delegate()
                                        {
                                            var threadTrials = new List<int>(trialCoreDistribution[coreVar]);

                                            for (int i = 0; i < threadTrials.Count(); i++)
                                            {
                                                List<MeasureDataContainer> tempRawDataEnum;
                                                lock (myDataContainter)
                                                {
                                                    tempRawDataEnum =
                                                        new List<MeasureDataContainer>(
                                                            myDataContainter.MeasureDataRaw.Where(
                                                                t => t.ContainsDuplicates == false)
                                                                            .Where(
                                                                                t =>
                                                                                t.SzenarioTrialNumber ==
                                                                                threadTrials.ElementAt(i))
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
                                        }));
                                }

                                foreach (Thread t in multiCoreThreads)
                                {
                                    t.Start();
                                }

                                foreach (Thread t in multiCoreThreads)
                                {
                                    t.Join();
                                }
                                _myMatlabWrapper.ClearWorkspace();
                                multiCoreThreads.Clear();

                                #endregion Butterworth Filter

                                WriteProgressInfo("Calculating velocity...");

                                #region Velocity calcultion

                                for (int core = 0; core < Environment.ProcessorCount; core++)
                                {
                                    int coreVar = core;

                                    multiCoreThreads.Add(new Thread(delegate()
                                        {
                                            var threadTrials = new List<int>(trialCoreDistribution[coreVar]);

                                            for (int i = 0; i < threadTrials.Count(); i++)
                                            {
                                                List<MeasureDataContainer> tempFilteredDataEnum;
                                                lock (myDataContainter)
                                                {
                                                    tempFilteredDataEnum =
                                                        new List<MeasureDataContainer>(
                                                            myDataContainter.MeasureDataFiltered.Where(
                                                                t =>
                                                                t.SzenarioTrialNumber == threadTrials.ElementAt(i))
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
                                                                                           textBox_Import_SamplesPerSec
                                                                                               .Text)));

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
                                        }));
                                }

                                foreach (Thread t in multiCoreThreads)
                                {
                                    t.Start();
                                }

                                foreach (Thread t in multiCoreThreads)
                                {
                                    t.Join();
                                }
                                _myMatlabWrapper.ClearWorkspace();
                                multiCoreThreads.Clear();

                                #endregion

                                WriteProgressInfo("Normalizing data...");

                                #region Time normalization

                                for (int core = 0; core < Environment.ProcessorCount; core++)
                                {
                                    int coreVar = core;
                                    multiCoreThreads.Add(new Thread(delegate()
                                        {
                                            var threadTrials = new List<int>(trialCoreDistribution[coreVar]);
                                            _myMatlabWrapper.SetWorkspaceData("newSampleCount",
                                                                              Convert.ToDouble(
                                                                                  textBox_Import_NewSampleCount.Text));

                                            for (int i = 0; i < threadTrials.Count(); i++)
                                            {
                                                List<MeasureDataContainer> tempFilteredDataEnum;
                                                List<VelocityDataContainer> tempVelocityDataEnum;

                                                lock (myDataContainter)
                                                {
                                                    tempFilteredDataEnum =
                                                        new List<MeasureDataContainer>(
                                                            myDataContainter.MeasureDataFiltered.Where(
                                                                t =>
                                                                t.SzenarioTrialNumber == threadTrials.ElementAt(i))
                                                                            .Where(t => t.PositionStatus == 1)
                                                                            .OrderBy(t => t.TimeStamp));
                                                    if (
                                                        myDataContainter.MeasureDataFiltered.Where(
                                                            t => t.SzenarioTrialNumber == threadTrials.ElementAt(i))
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
                                                                t.SzenarioTrialNumber == threadTrials.ElementAt(i))
                                                                            .OrderBy(t => t.TimeStamp));
                                                }

                                                if ((tempFilteredDataEnum.Count > 0) && (tempVelocityDataEnum.Count > 0))
                                                {
                                                    int trimThreshold =
                                                        Convert.ToInt32(textBox_Import_PercentPeakVelocity.Text);
                                                    var tempVelocityDataEnumCropped = new List<VelocityDataContainer>();

                                                    if (trimThreshold > 0)
                                                    {
                                                        double velocityCropThreshold =
                                                            tempVelocityDataEnum.Max(
                                                                t =>
                                                                Math.Sqrt(Math.Pow(t.VelocityX, 2) +
                                                                          Math.Pow(t.VelocityZ, 2)))/100.0*
                                                            trimThreshold;

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
                                                    else if (trimThreshold < 0)
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
                                                        WriteToLogBox(
                                                            errorList.Where(t => !string.IsNullOrEmpty(t))
                                                                     .Select(
                                                                         t =>
                                                                         t + " in " + filename +
                                                                         " at szenario-trial-number " +
                                                                         tempSzenarioTrialNumber)
                                                                     .ToArray());
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
                                        }));
                                }

                                foreach (Thread t in multiCoreThreads)
                                {
                                    t.Start();
                                }

                                foreach (Thread t in multiCoreThreads)
                                {
                                    t.Join();
                                }
                                _myMatlabWrapper.ClearWorkspace();
                                multiCoreThreads.Clear();

                                #endregion

                                WriteProgressInfo("Calculating baselines...");

                                #region Calculate baselines

                                if (myDataContainter.SzenarioName == "Szenario02")
                                {
                                    myDataContainter.BaselineData = new List<BaselineDataContainer>();

                                    for (int core = 0; core < Environment.ProcessorCount; core++)
                                    {
                                        int coreVar = core;
                                        multiCoreThreads.Add(new Thread(delegate()
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
                                                        tempNormalisedDataEnum =
                                                            new List<MeasureDataContainer>(
                                                                myDataContainter.MeasureDataNormalized.Where(
                                                                    t =>
                                                                    t.TargetNumber ==
                                                                    threadTargets.ElementAt(targetCount))
                                                                                .OrderBy(t => t.TimeStamp));
                                                        tempVelocityDataNormalisedEnum =
                                                            new List<VelocityDataContainer>(
                                                                myDataContainter.VelocityDataNormalized.Where(
                                                                    t =>
                                                                    t.TargetNumber ==
                                                                    threadTargets.ElementAt(targetCount))
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
                                                        List<MeasureDataContainer> tempMeasureDataCountainerList =
                                                            tempNormalisedDataEnum.Where(
                                                                t =>
                                                                t.SzenarioTrialNumber ==
                                                                tempSzenarioTrialNumbers.ElementAt(i))
                                                                                  .OrderBy(t => t.TimeStamp)
                                                                                  .ToList();
                                                        List<VelocityDataContainer> tempVelocityDataContainerList =
                                                            tempVelocityDataNormalisedEnum.Where(
                                                                t =>
                                                                t.SzenarioTrialNumber ==
                                                                tempSzenarioTrialNumbers.ElementAt(i))
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
                                            }));
                                    }

                                    foreach (Thread t in multiCoreThreads)
                                    {
                                        t.Start();
                                    }

                                    foreach (Thread t in multiCoreThreads)
                                    {
                                        t.Join();
                                    }

                                    multiCoreThreads.Clear();
                                }

                                #endregion

                                WriteProgressInfo("Calculating szenario mean times...");

                                #region Calculate szenario mean times

                                for (int core = 0; core < Environment.ProcessorCount; core++)
                                {
                                    int coreVar = core;
                                    multiCoreThreads.Add(new Thread(delegate()
                                        {
                                            var threadTargets = new List<int>(targetCoreDistribution[coreVar]);

                                            for (int targetCount = 0;
                                                 targetCount < threadTargets.Count();
                                                 targetCount++)
                                            {
                                                List<MeasureDataContainer> tempNormalisedDataEnum;

                                                lock (myDataContainter)
                                                {
                                                    tempNormalisedDataEnum =
                                                        new List<MeasureDataContainer>(
                                                            myDataContainter.MeasureDataNormalized.Where(
                                                                t =>
                                                                t.TargetNumber == threadTargets.ElementAt(targetCount))
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
                                        }));
                                }

                                foreach (Thread t in multiCoreThreads)
                                {
                                    t.Start();
                                }

                                foreach (Thread t in multiCoreThreads)
                                {
                                    t.Join();
                                }
                                _myMatlabWrapper.ClearWorkspace();
                                multiCoreThreads.Clear();

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
                                    WriteProgressInfo("Preparing Trial " + (i + 1) + " of " +
                                                      szenarioTrialNumbers.Length);

                                    List<MeasureDataContainer> measureDataRawList =
                                        myDataContainter.MeasureDataRaw.Where(
                                            t => t.SzenarioTrialNumber == szenarioTrialNumbers[i])
                                                        .OrderBy(t => t.TimeStamp)
                                                        .ToList();

                                    int targetID =
                                        _mySqlWrapper.InsertTarget(measureDataRawList.ElementAt(0).TargetNumber);

                                    int targetTrialNumberID =
                                        _mySqlWrapper.InsertTargetTrialNumber(
                                            measureDataRawList.ElementAt(0).TargetTrialNumber);

                                    int szenarioTrialNumberID =
                                        _mySqlWrapper.InsertSzenarioTrialNumber(szenarioTrialNumbers[i]);

                                    int trialInformationID =
                                        _mySqlWrapper.InsertTrialInformation(
                                            measureDataRawList.ElementAt(0).ContainsDuplicates, filterOrder, cutoffFreq,
                                            velocityCuttingThreshold);

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
                                                t => t.SzenarioTrialNumber == szenarioTrialNumbers[i])
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
                                                t => t.SzenarioTrialNumber == szenarioTrialNumbers[i])
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
                                                t => t.SzenarioTrialNumber == szenarioTrialNumbers[i])
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
                                                t => t.SzenarioTrialNumber == szenarioTrialNumbers[i])
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

                                WriteProgressInfo("Uploading trial data...");

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

                                WriteProgressInfo("Uploading szenario mean-time data...");
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

                                WriteProgressInfo("Uploading baseline data...");
                                if (myDataContainter.BaselineData != null)
                                {
                                    for (int j = 0; j < targetNumbers.Length; j++)
                                    {
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
                                            myDataContainter.BaselineData.Where(t => t.TargetNumber == targetNumbers[j])
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
                                WriteToLogBox("Fehler beim einlesen der Datei \"" + filename + "\"");
                            }
                        }
                    }
                    SetProgressBarValue(0);
                    WriteProgressInfo("Ready");
                    EnableTabPages(true);

                    ThreadManager.Remove(Thread.CurrentThread);
                });
            ThreadManager.PushBack(newThread);
            newThread.Start();
        }

        private void button_CalculateStatistics_Click(object sender, EventArgs e)
        {
            var newThread = new Thread(delegate()
                {
                    while (ThreadManager.GetIndex(Thread.CurrentThread) != 0)
                    {
                        Thread.Sleep(100);
                    }

                    EnableTabPages(false);
                    WriteProgressInfo("Calculating statistics...");

                    List<int[]> trialInfos = _mySqlWrapper.GetStatisticCalculationInformation();

                    if (trialInfos != null)
                    {
                        int counter = 1;

                        foreach (var trialInfo in trialInfos)
                        {
                            while (ThreadManager.Pause)
                            {
                                Thread.Sleep(100);
                            }
                            SetProgressBarValue((100.0/trialInfos.Count())*counter);
                            counter++;

                            DataSet measureDataSet = _mySqlWrapper.GetMeasureDataNormalizedDataSet(trialInfo[0]);
                            DataSet velocityDataSet = _mySqlWrapper.GetVelocityDataNormalizedDataSet(trialInfo[0]);
                            DataSet baselineDataSet = _mySqlWrapper.GetBaselineDataSet(trialInfo[1], trialInfo[2],
                                                                                       trialInfo[3], trialInfo[4]);
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
                                                Convert.ToDouble(
                                                    measureDataSet.Tables[0].Rows[i]["position_cartesian_x"]);
                                            measureData[i, 1] =
                                                Convert.ToDouble(
                                                    measureDataSet.Tables[0].Rows[i]["position_cartesian_y"]);
                                            measureData[i, 2] =
                                                Convert.ToDouble(
                                                    measureDataSet.Tables[0].Rows[i]["position_cartesian_z"]);

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
                                                Convert.ToDouble(
                                                    baselineDataSet.Tables[0].Rows[i]["baseline_velocity_x"]);
                                            baselineData[i, 4] =
                                                Convert.ToDouble(
                                                    baselineDataSet.Tables[0].Rows[i]["baseline_velocity_y"]);
                                            baselineData[i, 5] =
                                                Convert.ToDouble(
                                                    baselineDataSet.Tables[0].Rows[i]["baseline_velocity_z"]);
                                        }

                                        List<double> tempTimeList = timeStamp.ToList();
                                        int time300MsIndex =
                                            tempTimeList.IndexOf(
                                                tempTimeList.OrderBy(
                                                    d =>
                                                    Math.Abs(d - (timeStamp[0] + TimeSpan.FromMilliseconds(300).Ticks)))
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

                                        double vectorCorrelation =
                                            _myMatlabWrapper.GetWorkspaceData("vector_correlation");
                                        double enclosedArea = _myMatlabWrapper.GetWorkspaceData("enclosed_area");
                                        double lengthAbs = _myMatlabWrapper.GetWorkspaceData("length_abs");
                                        double lengthRatio = _myMatlabWrapper.GetWorkspaceData("length_ratio");
                                        double distance300MsAbs = _myMatlabWrapper.GetWorkspaceData("distance300msAbs");
                                        double distance300MsSign = _myMatlabWrapper.GetWorkspaceData("distance300msSign");
                                        double meanDistanceAbs = _myMatlabWrapper.GetWorkspaceData("meanDistanceAbs");
                                        double maxDistanceAbs = _myMatlabWrapper.GetWorkspaceData("maxDistanceAbs");
                                        double maxDistanceSign = _myMatlabWrapper.GetWorkspaceData("maxDistanceSign");
                                        double rmse = _myMatlabWrapper.GetWorkspaceData("rmse");

                                        _mySqlWrapper.InsertStatisticData(
                                            trialInfo[0],
                                            vectorCorrelation,
                                            lengthAbs, lengthRatio,
                                            distance300MsAbs,
                                            maxDistanceAbs,
                                            meanDistanceAbs,
                                            distance300MsSign,
                                            maxDistanceSign,
                                            enclosedArea,
                                            rmse
                                            );
                                    }
                                    catch (Exception statisticException)
                                    {
                                        WriteToLogBox("Error in Statistic calculation: " + statisticException);
                                    }
                                }
                                else
                                {
                                    WriteToLogBox("TrialID: " + trialInfo[0] + " - Data not normalised!");
                                }
                                _myMatlabWrapper.ClearWorkspace();
                            }
                            else
                            {
                                WriteToLogBox("TrialID: " + trialInfo[0] + " - No matching baseline found!");
                            }
                        }
                    }
                    else
                    {
                        WriteToLogBox("Statistics already calculated!");
                    }
                    SetProgressBarValue(0);
                    WriteProgressInfo("Ready");
                    EnableTabPages(true);

                    ThreadManager.Remove(Thread.CurrentThread);
                });
            ThreadManager.PushBack(newThread);
            newThread.Start();
        }

        private void button_FixBrokenTrials_Click(object sender, EventArgs e)
        {
            var newThread = new Thread(delegate()
                {
                    while (ThreadManager.GetIndex(Thread.CurrentThread) != 0)
                    {
                        Thread.Sleep(100);
                    }

                    EnableTabPages(false);
                    WriteProgressInfo("Fixing broken Trials...");

                    List<object[]> faultyTrialInformation = _mySqlWrapper.GetFaultyTrialInformation();

                    if (faultyTrialInformation != null)
                    {
                        if (faultyTrialInformation.Count == 0)
                        {
                            WriteToLogBox("Trials already fixed!");
                        }
                        else
                        {
                            for (int trialIDCounter = 0;
                                 trialIDCounter < faultyTrialInformation.Count;
                                 trialIDCounter++)
                            {
                                while (ThreadManager.Pause)
                                {
                                    Thread.Sleep(100);
                                }
                                SetProgressBarValue((100.0/faultyTrialInformation.Count)*trialIDCounter);

                                int[] trialFixInformation =
                                    _mySqlWrapper.GetFaultyTrialFixInformation(
                                        Convert.ToInt32(faultyTrialInformation[trialIDCounter][1]),
                                        Convert.ToInt32(faultyTrialInformation[trialIDCounter][7]));

                                DataSet upperStatisticDataSet = _mySqlWrapper.GetStatisticDataSet(trialFixInformation[0]);
                                DataSet lowerStatisticDataSet = _mySqlWrapper.GetStatisticDataSet(trialFixInformation[1]);

                                double velocityVectorCorrelation =
                                    (Convert.ToDouble(
                                        upperStatisticDataSet.Tables[0].Rows[0]["velocity_vector_correlation"]) +
                                     Convert.ToDouble(
                                         lowerStatisticDataSet.Tables[0].Rows[0]["velocity_vector_correlation"]))/2;
                                double trajectoryLengthAbs =
                                    (Convert.ToDouble(upperStatisticDataSet.Tables[0].Rows[0]["trajectory_length_abs"]) +
                                     Convert.ToDouble(lowerStatisticDataSet.Tables[0].Rows[0]["trajectory_length_abs"]))/
                                    2;
                                double trajectoryLengthRatioBaseline =
                                    (Convert.ToDouble(
                                        upperStatisticDataSet.Tables[0].Rows[0]["trajectory_length_ratio_baseline"]) +
                                     Convert.ToDouble(
                                         lowerStatisticDataSet.Tables[0].Rows[0]["trajectory_length_ratio_baseline"]))/2;
                                double perpendicularDisplacement300MsAbs =
                                    (Convert.ToDouble(
                                        upperStatisticDataSet.Tables[0].Rows[0]["perpendicular_displacement_300ms_abs"]) +
                                     Convert.ToDouble(
                                         lowerStatisticDataSet.Tables[0].Rows[0]["perpendicular_displacement_300ms_abs"]))/
                                    2;
                                double maximalPerpendicularDisplacementAbs =
                                    (Convert.ToDouble(
                                        upperStatisticDataSet.Tables[0].Rows[0]["maximal_perpendicular_displacement_abs"
                                            ]) +
                                     Convert.ToDouble(
                                         lowerStatisticDataSet.Tables[0].Rows[0][
                                             "maximal_perpendicular_displacement_abs"]))/2;
                                double meanPerpendicularDisplacementAbs =
                                    (Convert.ToDouble(
                                        upperStatisticDataSet.Tables[0].Rows[0]["mean_perpendicular_displacement_abs"]) +
                                     Convert.ToDouble(
                                         lowerStatisticDataSet.Tables[0].Rows[0]["mean_perpendicular_displacement_abs"]))/
                                    2;
                                double perpendicularDisplacement300MsSign =
                                    (Convert.ToDouble(
                                        upperStatisticDataSet.Tables[0].Rows[0]["perpendicular_displacement_300ms_sign"]) +
                                     Convert.ToDouble(
                                         lowerStatisticDataSet.Tables[0].Rows[0]["perpendicular_displacement_300ms_sign"
                                             ]))/2;
                                double maximalPerpendicularDisplacementSign =
                                    (Convert.ToDouble(
                                        upperStatisticDataSet.Tables[0].Rows[0][
                                            "maximal_perpendicular_displacement_sign"]) +
                                     Convert.ToDouble(
                                         lowerStatisticDataSet.Tables[0].Rows[0][
                                             "maximal_perpendicular_displacement_sign"]))/2;
                                double enclosedArea =
                                    (Convert.ToDouble(upperStatisticDataSet.Tables[0].Rows[0]["enclosed_area"]) +
                                     Convert.ToDouble(lowerStatisticDataSet.Tables[0].Rows[0]["enclosed_area"]))/2;
                                double rmse = (Convert.ToDouble(upperStatisticDataSet.Tables[0].Rows[0]["rmse"]) +
                                               Convert.ToDouble(lowerStatisticDataSet.Tables[0].Rows[0]["rmse"]))/2;

                                _mySqlWrapper.InsertStatisticData(
                                    Convert.ToInt32(faultyTrialInformation[trialIDCounter][0]),
                                    velocityVectorCorrelation,
                                    trajectoryLengthAbs,
                                    trajectoryLengthRatioBaseline,
                                    perpendicularDisplacement300MsAbs,
                                    maximalPerpendicularDisplacementAbs,
                                    meanPerpendicularDisplacementAbs,
                                    perpendicularDisplacement300MsSign,
                                    maximalPerpendicularDisplacementSign,
                                    enclosedArea,
                                    rmse
                                    );
                            }
                        }
                    }
                    else
                    {
                        WriteToLogBox("Trials already fixed!");
                    }
                    SetProgressBarValue(0);
                    WriteProgressInfo("Ready");
                    EnableTabPages(true);

                    ThreadManager.Remove(Thread.CurrentThread);
                });
            ThreadManager.PushBack(newThread);
            newThread.Start();
        }

        private void button_Auto_Click(object sender, EventArgs e)
        {
            button_ImportMeasureFiles_Click(sender, e);
            button_CalculateStatistics_Click(sender, e);
            button_FixBrokenTrials_Click(sender, e);
        }

        private void button_Debug_SaveLogToFile_Click(object sender, EventArgs e)
        {
            saveFileDialog = new SaveFileDialog();
            saveFileDialog.Reset();
            saveFileDialog.Title = @"Save log file";
            saveFileDialog.AddExtension = true;
            saveFileDialog.DefaultExt = ".txt";
            saveFileDialog.Filter = @"LogFiles (*.txt)|.txt";
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.FileName = DateTime.Now.Year.ToString("0000")
                                      + "."
                                      + DateTime.Now.Month.ToString("00")
                                      + "."
                                      + DateTime.Now.Day.ToString("00")
                                      + "-"
                                      + DateTime.Now.Hour.ToString("00")
                                      + "."
                                      + DateTime.Now.Minute.ToString("00")
                                      + "-LogFile";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var logFileStream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
                var logFileWriter = new StreamWriter(logFileStream);

                string[] logText = GetLogBoxText();
                for (int i = 0; i < logText.Length; i++)
                {
                    logFileWriter.WriteLine(logText[i]);
                }

                logFileWriter.Close();
                logFileStream.Close();
            }
        }

        private void button_ClearLog_Click(object sender, EventArgs e)
        {
            ClearLogBox();
        }

        private void tabPage_TrajectoryVelocity_Enter(object sender, EventArgs e)
        {
            comboBox_TrajectoryVelocity_Study.Items.Clear();
            listBox_TrajectoryVelocity_Groups.Items.Clear();
            comboBox_TrajectoryVelocity_Szenario.Items.Clear();
            listBox_TrajectoryVelocity_Subjects.Items.Clear();
            listBox_TrajectoryVelocity_Turns.Items.Clear();
            listBox_TrajectoryVelocity_Targets.Items.Clear();
            listBox_TrajectoryVelocity_Trials.Items.Clear();

            comboBox_TrajectoryVelocity_IndividualMean.SelectedIndex = 0;
            comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedIndex = 0;

            string[] studyNames = _mySqlWrapper.GetStudyNames();
            if (studyNames != null)
            {
                comboBox_TrajectoryVelocity_Study.Items.AddRange(studyNames);
                comboBox_TrajectoryVelocity_Study.SelectedIndex = 0;
            }
        }

        private void comboBox_TrajectoryVelocity_Study_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox_TrajectoryVelocity_Groups.Items.Clear();
            comboBox_TrajectoryVelocity_Szenario.Items.Clear();
            listBox_TrajectoryVelocity_Subjects.Items.Clear();
            listBox_TrajectoryVelocity_Turns.Items.Clear();
            listBox_TrajectoryVelocity_Targets.Items.Clear();
            listBox_TrajectoryVelocity_Trials.Items.Clear();

            string[] groupNames = _mySqlWrapper.GetGroupNames(comboBox_TrajectoryVelocity_Study.SelectedItem.ToString());
            if (groupNames != null)
            {
                listBox_TrajectoryVelocity_Groups.Items.AddRange(groupNames);
                listBox_TrajectoryVelocity_Groups.SelectedIndex = 0;
            }
        }

        private void listBox_TrajectoryVelocity_Groups_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox_TrajectoryVelocity_Groups.SelectedItems.Count > 0)
            {
                comboBox_TrajectoryVelocity_Szenario.Items.Clear();
                listBox_TrajectoryVelocity_Subjects.Items.Clear();
                listBox_TrajectoryVelocity_Turns.Items.Clear();
                listBox_TrajectoryVelocity_Targets.Items.Clear();
                listBox_TrajectoryVelocity_Trials.Items.Clear();

                string study = comboBox_TrajectoryVelocity_Study.SelectedItem.ToString();
                string[] groups = listBox_TrajectoryVelocity_Groups.SelectedItems.Cast<string>().ToArray();

                string[] szenarioIntersect = _mySqlWrapper.GetSzenarioNames(study, groups[0]);
                for (int i = 1; i < groups.Length; i++)
                {
                    szenarioIntersect =
                        szenarioIntersect.Intersect(_mySqlWrapper.GetSzenarioNames(study, groups[i])).ToArray();
                }

                comboBox_TrajectoryVelocity_Szenario.Items.AddRange(szenarioIntersect);
                comboBox_TrajectoryVelocity_Szenario.SelectedIndex = 0;
            }
        }

        private void comboBox_TrajectoryVelocity_Szenario_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox_TrajectoryVelocity_Subjects.Items.Clear();
            listBox_TrajectoryVelocity_Turns.Items.Clear();
            listBox_TrajectoryVelocity_Targets.Items.Clear();
            listBox_TrajectoryVelocity_Trials.Items.Clear();

            string study = comboBox_TrajectoryVelocity_Study.SelectedItem.ToString();
            string[] groups = listBox_TrajectoryVelocity_Groups.SelectedItems.Cast<string>().ToArray();
            string szenario = comboBox_TrajectoryVelocity_Szenario.SelectedItem.ToString();

            for (int i = 0; i < groups.Length; i++)
            {
                listBox_TrajectoryVelocity_Subjects.Items.AddRange(
                    _mySqlWrapper.GetSubjectInformations(study, groups[i], szenario).ToArray());
            }

            listBox_TrajectoryVelocity_Subjects.SelectedIndex = 0;
        }

        private void listBox_TrajectoryVelocity_Subjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox_TrajectoryVelocity_Turns.Items.Clear();
            listBox_TrajectoryVelocity_Targets.Items.Clear();
            listBox_TrajectoryVelocity_Trials.Items.Clear();

            string study = comboBox_TrajectoryVelocity_Study.SelectedItem.ToString();
            string[] groups = listBox_TrajectoryVelocity_Groups.SelectedItems.Cast<string>().ToArray();
            string szenario = comboBox_TrajectoryVelocity_Szenario.SelectedItem.ToString();
            SubjectInformationContainer[] subjects =
                listBox_TrajectoryVelocity_Subjects.SelectedItems.Cast<SubjectInformationContainer>().ToArray();

            string[] turnIntersect = null;
            for (int i = 0; i < groups.Length; i++)
            {
                for (int j = 0; j < subjects.Length; j++)
                {
                    string[] tempTurnString = _mySqlWrapper.GetTurns(study, groups[i], szenario, subjects[j].ID);

                    if (tempTurnString != null)
                    {
                        if (turnIntersect == null)
                        {
                            turnIntersect = tempTurnString;
                        }
                        else
                        {
                            turnIntersect = turnIntersect.Intersect(tempTurnString).ToArray();
                        }
                    }
                }
            }

            if (turnIntersect != null)
            {
                listBox_TrajectoryVelocity_Turns.Items.AddRange(turnIntersect);
            }
            listBox_TrajectoryVelocity_Turns.SelectedIndex = 0;
        }

        private void listBox_TrajectoryVelocity_Turns_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox_TrajectoryVelocity_Targets.Items.Clear();
            listBox_TrajectoryVelocity_Trials.Items.Clear();

            string study = comboBox_TrajectoryVelocity_Study.SelectedItem.ToString();
            string szenario = comboBox_TrajectoryVelocity_Szenario.SelectedItem.ToString();

            string[] targets = _mySqlWrapper.GetTargets(study, szenario).OrderBy(t => t).ToArray();
            string[] trials = _mySqlWrapper.GetTrials(study, szenario).OrderBy(t => t).ToArray();

            listBox_TrajectoryVelocity_Targets.Items.AddRange(targets);
            listBox_TrajectoryVelocity_Targets.SelectedIndex = 0;

            listBox_TrajectoryVelocity_Trials.Items.AddRange(trials);
            listBox_TrajectoryVelocity_Trials.SelectedIndex = 0;
        }

        private void button_TrajectoryVelocity_AddSelected_Click(object sender, EventArgs e)
        {
            if (comboBox_TrajectoryVelocity_Study.SelectedItem != null)
            {
                string study = comboBox_TrajectoryVelocity_Study.SelectedItem.ToString();
                string[] groups = listBox_TrajectoryVelocity_Groups.SelectedItems.Cast<string>().ToArray();
                string szenario = comboBox_TrajectoryVelocity_Szenario.SelectedItem.ToString();
                SubjectInformationContainer[] subjects =
                    listBox_TrajectoryVelocity_Subjects.SelectedItems.Cast<SubjectInformationContainer>().ToArray();
                string[] turns = listBox_TrajectoryVelocity_Turns.SelectedItems.Cast<string>().ToArray();
                string[] targets = listBox_TrajectoryVelocity_Targets.SelectedItems.Cast<string>().ToArray();
                string[] trials = listBox_TrajectoryVelocity_Trials.SelectedItems.Cast<string>().ToArray();

                foreach (string group in groups)
                {
                    foreach (SubjectInformationContainer subject in subjects)
                    {
                        foreach (string turn in turns)
                        {
                            foreach (string target in targets)
                            {
                                if (_mySqlWrapper.GetTurns(study, group, szenario, subject.ID) != null)
                                {
                                    if (listBox_TrajectoryVelocity_SelectedTrials.Items.Count > 0)
                                    {
                                        bool canBeUpdated = false;
                                        foreach (
                                            TrajectoryVelocityPlotContainer temp in
                                                listBox_TrajectoryVelocity_SelectedTrials.Items)
                                        {
                                            if (temp.UpdateTrajectoryVelocityPlotContainer(study, group, szenario,
                                                                                           subject, turn, target, trials))
                                            {
                                                typeof (ListBox).InvokeMember("RefreshItems",
                                                                              BindingFlags.NonPublic |
                                                                              BindingFlags.Instance |
                                                                              BindingFlags.InvokeMethod,
                                                                              null,
                                                                              listBox_TrajectoryVelocity_SelectedTrials,
                                                                              new object[] {});
                                                canBeUpdated = true;
                                            }
                                        }

                                        if (!canBeUpdated)
                                        {
                                            listBox_TrajectoryVelocity_SelectedTrials.Items.Add(
                                                new TrajectoryVelocityPlotContainer(study, group, szenario, subject,
                                                                                    turn, target, trials));
                                        }
                                    }
                                    else
                                    {
                                        listBox_TrajectoryVelocity_SelectedTrials.Items.Add(
                                            new TrajectoryVelocityPlotContainer(study, group, szenario, subject, turn,
                                                                                target, trials));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void button_TrajectoryVelocity_AddAll_Click(object sender, EventArgs e)
        {
            if (comboBox_TrajectoryVelocity_Study.SelectedItem != null)
            {
                string study = comboBox_TrajectoryVelocity_Study.SelectedItem.ToString();
                string[] groups = listBox_TrajectoryVelocity_Groups.SelectedItems.Cast<string>().ToArray();
                string szenario = comboBox_TrajectoryVelocity_Szenario.SelectedItem.ToString();
                SubjectInformationContainer[] subjects =
                    listBox_TrajectoryVelocity_Subjects.SelectedItems.Cast<SubjectInformationContainer>().ToArray();
                string[] turns = listBox_TrajectoryVelocity_Turns.SelectedItems.Cast<string>().ToArray();
                string[] targets = listBox_TrajectoryVelocity_Targets.SelectedItems.Cast<string>().ToArray();
                string[] trials = listBox_TrajectoryVelocity_Trials.Items.Cast<string>().ToArray();

                foreach (string group in groups)
                {
                    foreach (SubjectInformationContainer subject in subjects)
                    {
                        foreach (string turn in turns)
                        {
                            foreach (string target in targets)
                            {
                                if (_mySqlWrapper.GetTurns(study, group, szenario, subject.ID) != null)
                                {
                                    if (listBox_TrajectoryVelocity_SelectedTrials.Items.Count > 0)
                                    {
                                        bool canBeUpdated = false;
                                        foreach (
                                            TrajectoryVelocityPlotContainer temp in
                                                listBox_TrajectoryVelocity_SelectedTrials.Items)
                                        {
                                            if (temp.UpdateTrajectoryVelocityPlotContainer(study, group, szenario,
                                                                                           subject, turn, target, trials))
                                            {
                                                typeof (ListBox).InvokeMember("RefreshItems",
                                                                              BindingFlags.NonPublic |
                                                                              BindingFlags.Instance |
                                                                              BindingFlags.InvokeMethod,
                                                                              null,
                                                                              listBox_TrajectoryVelocity_SelectedTrials,
                                                                              new object[] {});
                                                canBeUpdated = true;
                                            }
                                        }

                                        if (!canBeUpdated)
                                        {
                                            listBox_TrajectoryVelocity_SelectedTrials.Items.Add(
                                                new TrajectoryVelocityPlotContainer(study, group, szenario, subject,
                                                                                    turn, target, trials));
                                        }
                                    }
                                    else
                                    {
                                        listBox_TrajectoryVelocity_SelectedTrials.Items.Add(
                                            new TrajectoryVelocityPlotContainer(study, group, szenario, subject, turn,
                                                                                target, trials));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void button_TrajectoryVelocity_ClearSelected_Click(object sender, EventArgs e)
        {
            while (listBox_TrajectoryVelocity_SelectedTrials.SelectedItems.Count > 0)
            {
                listBox_TrajectoryVelocity_SelectedTrials.Items.Remove(
                    listBox_TrajectoryVelocity_SelectedTrials.SelectedItem);
            }
        }

        private void button_TrajectoryVelocity_ClearAll_Click(object sender, EventArgs e)
        {
            listBox_TrajectoryVelocity_SelectedTrials.Items.Clear();
        }

        private void button_TrajectoryVelocity_Plot_Click(object sender, EventArgs e)
        {
            if (listBox_TrajectoryVelocity_SelectedTrials.Items.Count != 0)
            {
                WriteProgressInfo("Getting data...");
                if (comboBox_TrajectoryVelocity_IndividualMean.SelectedItem.ToString() == "Individual")
                {
                    if (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString() == "Trajectory")
                    {
                        _myMatlabWrapper.CreateTrajectoryFigure("XZ-Plot");
                        _myMatlabWrapper.DrawTargets(0.005, 0.1, 0, 0);
                    }
                    else if (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString() == "Velocity")
                    {
                        _myMatlabWrapper.CreateVelocityFigure("Velocity plot", 101);
                    }

                    int counter = 0;
                    foreach (
                        TrajectoryVelocityPlotContainer tempContainer in listBox_TrajectoryVelocity_SelectedTrials.Items
                        )
                    {
                        SetProgressBarValue((100.0/listBox_TrajectoryVelocity_SelectedTrials.Items.Count)*counter);
                        counter++;
                        DateTime turnDateTime = _mySqlWrapper.GetTurnDateTime(tempContainer.Study, tempContainer.Group,
                                                                              tempContainer.Szenario,
                                                                              tempContainer.Subject.ID,
                                                                              tempContainer.Turn);
                        foreach (int trial in tempContainer.Trials)
                        {
                            if (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString() == "Trajectory")
                            {
                                int trialID = _mySqlWrapper.GetTrailID(tempContainer.Study, tempContainer.Group,
                                                                       tempContainer.Szenario, tempContainer.Subject.ID,
                                                                       turnDateTime, tempContainer.Target, trial);
                                DataSet measureDataSet = _mySqlWrapper.GetMeasureDataNormalizedDataSet(trialID);

                                var measureDataX = new List<double>();
                                var measureDataZ = new List<double>();

                                foreach (DataRow row in measureDataSet.Tables[0].Rows)
                                {
                                    if (checkBox_TrajectoryVelocity_IgnoreCatchTrials.Checked)
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

                            else if (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString() == "Velocity")
                            {
                                int trialID = _mySqlWrapper.GetTrailID(tempContainer.Study, tempContainer.Group,
                                                                       tempContainer.Szenario,
                                                                       tempContainer.Subject.ID, turnDateTime,
                                                                       tempContainer.Target, trial);
                                DataSet velocityDataSet = _mySqlWrapper.GetVelocityDataNormalizedDataSet(trialID);
                                var velocityDataXZ = new List<double>();

                                foreach (DataRow row in velocityDataSet.Tables[0].Rows)
                                {
                                    if (checkBox_TrajectoryVelocity_IgnoreCatchTrials.Checked)
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
                                _myMatlabWrapper.Plot("X", "Z", "black", 2);
                            }
                        }
                    }
                }

                else if (comboBox_TrajectoryVelocity_IndividualMean.SelectedItem.ToString() == "Mean")
                {
                    var tempTrajectoryVelocityPlotContainerList =
                        new List<TrajectoryVelocityPlotContainer>(
                            listBox_TrajectoryVelocity_SelectedTrials.Items.Cast<TrajectoryVelocityPlotContainer>()
                                                                     .ToList());

                    if (
                        tempTrajectoryVelocityPlotContainerList.Select(t => t.Trials.ToArray())
                                                               .Distinct(new ArrayComparer()).Count() > 1)
                    {
                        WriteToLogBox("Trial selections are not equal!");
                    }
                    else
                    {
                        if (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString() == "Trajectory")
                        {
                            _myMatlabWrapper.CreateTrajectoryFigure("XZ-Plot");
                            _myMatlabWrapper.DrawTargets(0.005, 0.1, 0, 0);
                        }
                        else if (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString() == "Velocity")
                        {
                            _myMatlabWrapper.CreateFigure("Velocity plot", "[Samples]", "Velocity [m/s]");
                        }

                        int[] targetArray =
                            tempTrajectoryVelocityPlotContainerList.Select(t => t.Target).Distinct().ToArray();
                        int counter = 0;

                        for (int targetCounter = 0; targetCounter < targetArray.Length; targetCounter++)
                        {
                            int targetCounterVar = targetCounter;
                            int meanCounter = 0;
                            var dataX = new List<double>();
                            var dataZ = new List<double>();

                            foreach (
                                TrajectoryVelocityPlotContainer tempContainer in
                                    tempTrajectoryVelocityPlotContainerList.Where(
                                        t => t.Target == targetArray[targetCounterVar]))
                            {
                                SetProgressBarValue((100.0/listBox_TrajectoryVelocity_SelectedTrials.Items.Count)*
                                                    counter);
                                counter++;
                                DateTime turnDateTime = _mySqlWrapper.GetTurnDateTime(tempContainer.Study,
                                                                                      tempContainer.Group,
                                                                                      tempContainer.Szenario,
                                                                                      tempContainer.Subject.ID,
                                                                                      tempContainer.Turn);

                                if (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString() ==
                                    "Trajectory")
                                {
                                    foreach (int trial in tempContainer.Trials)
                                    {
                                        int trialID = _mySqlWrapper.GetTrailID(tempContainer.Study, tempContainer.Group,
                                                                               tempContainer.Szenario,
                                                                               tempContainer.Subject.ID, turnDateTime,
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
                                else if (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString() ==
                                         "Velocity")
                                {
                                    foreach (int trial in tempContainer.Trials)
                                    {
                                        int trialID = _mySqlWrapper.GetTrailID(tempContainer.Study,
                                                                               tempContainer.Group,
                                                                               tempContainer.Szenario,
                                                                               tempContainer.Subject.ID,
                                                                               turnDateTime, tempContainer.Target,
                                                                               trial);
                                        DataSet dataSet = _mySqlWrapper.GetVelocityDataNormalizedDataSet(trialID);

                                        for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                                        {
                                            DataRow row = dataSet.Tables[0].Rows[i];

                                            if (dataX.Count <= i)
                                            {
                                                dataX.Add(
                                                    Math.Sqrt(Math.Pow(Convert.ToDouble(row["velocity_x"]), 2) +
                                                              Math.Pow(Convert.ToDouble(row["velocity_z"]), 2)));
                                            }
                                            else
                                            {
                                                dataX[i] +=
                                                    Math.Sqrt(Math.Pow(Convert.ToDouble(row["velocity_x"]), 2) +
                                                              Math.Pow(Convert.ToDouble(row["velocity_z"]), 2));
                                            }
                                        }
                                        meanCounter++;
                                    }
                                }
                            }

                            if (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString() == "Trajectory")
                            {
                                for (int i = 0; i < dataX.Count; i++)
                                {
                                    dataX[i] /= meanCounter;
                                    dataZ[i] /= meanCounter;
                                }

                                _myMatlabWrapper.SetWorkspaceData("X", dataX.ToArray());
                                _myMatlabWrapper.SetWorkspaceData("Z", dataZ.ToArray());
                                _myMatlabWrapper.Plot("X", "Z", "black", 2);
                            }
                            else if (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString() == "Velocity")
                            {
                                for (int i = 0; i < dataX.Count; i++)
                                {
                                    dataX[i] /= meanCounter;
                                }

                                _myMatlabWrapper.SetWorkspaceData("X", dataX.ToArray());
                                _myMatlabWrapper.Plot("X", "black", 2);
                            }
                        }
                    }
                }

                _myMatlabWrapper.ClearWorkspace();
            }
            else
            {
                WriteToLogBox("Please add data to plot!");
            }
            SetProgressBarValue(0);
            WriteProgressInfo("Ready");
        }

        private void tabPage_BaselineMeantime_Enter(object sender, EventArgs e)
        {
            comboBox_BaselineMeantime_Study.Items.Clear();
            comboBox_BaselineMeantime_Group.Items.Clear();
            comboBox_BaselineMeantime_Szenario.Items.Clear();
            comboBox_BaselineMeantime_Subject.Items.Clear();
            comboBox_BaselineMeantime_Turn.Items.Clear();

            string[] studyNames = _mySqlWrapper.GetStudyNames();
            if (studyNames != null)
            {
                comboBox_BaselineMeantime_Study.Items.AddRange(studyNames);
                comboBox_BaselineMeantime_Study.SelectedIndex = 0;
            }
        }

        private void comboBox_BaselineMeantime_Study_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox_BaselineMeantime_Group.Items.Clear();
            comboBox_BaselineMeantime_Szenario.Items.Clear();
            comboBox_BaselineMeantime_Subject.Items.Clear();
            comboBox_BaselineMeantime_Turn.Items.Clear();

            string[] groupNames = _mySqlWrapper.GetGroupNames(comboBox_BaselineMeantime_Study.SelectedItem.ToString());
            if (groupNames != null)
            {
                comboBox_BaselineMeantime_Group.Items.AddRange(groupNames);
                comboBox_BaselineMeantime_Group.SelectedIndex = 0;
            }
        }

        private void comboBox_BaselineMeantime_Group_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox_BaselineMeantime_Szenario.Items.Clear();
            comboBox_BaselineMeantime_Subject.Items.Clear();
            comboBox_BaselineMeantime_Turn.Items.Clear();

            string study = comboBox_BaselineMeantime_Study.SelectedItem.ToString();
            string group = comboBox_BaselineMeantime_Group.SelectedItem.ToString();

            string[] szenarioNames = _mySqlWrapper.GetSzenarioNames(study, group);
            if (szenarioNames != null)
            {
                comboBox_BaselineMeantime_Szenario.Items.AddRange(szenarioNames);
                comboBox_BaselineMeantime_Szenario.SelectedIndex = 0;
            }
        }

        private void comboBox_BaselineMeantime_Szenario_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox_BaselineMeantime_Subject.Items.Clear();
            comboBox_BaselineMeantime_Turn.Items.Clear();

            string study = comboBox_BaselineMeantime_Study.SelectedItem.ToString();
            string group = comboBox_BaselineMeantime_Group.SelectedItem.ToString();
            string szenario = comboBox_BaselineMeantime_Szenario.SelectedItem.ToString();

            SubjectInformationContainer[] subjectNames = _mySqlWrapper.GetSubjectInformations(study, group, szenario);
            if (subjectNames != null)
            {
                comboBox_BaselineMeantime_Subject.Items.AddRange(subjectNames);
                comboBox_BaselineMeantime_Subject.SelectedIndex = 0;
            }
        }

        private void comboBox_BaselineMeantime_Subject_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox_BaselineMeantime_Turn.Items.Clear();

            string study = comboBox_BaselineMeantime_Study.SelectedItem.ToString();
            string group = comboBox_BaselineMeantime_Group.SelectedItem.ToString();
            string szenario = comboBox_BaselineMeantime_Szenario.SelectedItem.ToString();
            var subject = (SubjectInformationContainer) comboBox_BaselineMeantime_Subject.SelectedItem;

            string[] turnNames = _mySqlWrapper.GetTurns(study, group, szenario, subject.ID);
            if (turnNames != null)
            {
                comboBox_BaselineMeantime_Turn.Items.AddRange(turnNames);
                comboBox_BaselineMeantime_Turn.SelectedIndex = 0;
            }
        }

        private void tabPage_VisualizationExport_Enter(object sender, EventArgs e)
        {
            tabPage_TrajectoryVelocity_Enter(sender, e);
        }

        private void button_TrajectoryVelocity_Export_Click(object sender, EventArgs e)
        {
            WriteProgressInfo("Getting data...");

            saveFileDialog = new SaveFileDialog();
            saveFileDialog.Reset();
            saveFileDialog.Title = @"Save trajectory / velocity file";
            saveFileDialog.AddExtension = true;
            saveFileDialog.DefaultExt = ".csv";
            saveFileDialog.Filter = @"DataFiles (*.csv)|.csv";
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.FileName = DateTime.Now.Year.ToString("0000")
                                      + "."
                                      + DateTime.Now.Month.ToString("00")
                                      + "."
                                      + DateTime.Now.Day.ToString("00")
                                      + "-"
                                      + DateTime.Now.Hour.ToString("00")
                                      + "."
                                      + DateTime.Now.Minute.ToString("00")
                                      + "-"
                                      + comboBox_TrajectoryVelocity_IndividualMean.SelectedItem
                                      + "-"
                                      + comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem
                                      + "-data";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (listBox_TrajectoryVelocity_SelectedTrials.Items.Count != 0)
                {
                    if (comboBox_TrajectoryVelocity_IndividualMean.SelectedItem.ToString() == "Individual")
                    {
                        int counter = 0;
                        foreach (
                            TrajectoryVelocityPlotContainer tempContainer in
                                listBox_TrajectoryVelocity_SelectedTrials.Items)
                        {
                            SetProgressBarValue((100.0/listBox_TrajectoryVelocity_SelectedTrials.Items.Count)*counter);
                            counter++;
                            DateTime turnDateTime = _mySqlWrapper.GetTurnDateTime(tempContainer.Study,
                                                                                  tempContainer.Group,
                                                                                  tempContainer.Szenario,
                                                                                  tempContainer.Subject.ID,
                                                                                  tempContainer.Turn);
                            foreach (int trial in tempContainer.Trials)
                            {
                                if (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString() ==
                                    "Trajectory")
                                {
                                    int trialID = _mySqlWrapper.GetTrailID(tempContainer.Study, tempContainer.Group,
                                                                           tempContainer.Szenario,
                                                                           tempContainer.Subject.ID, turnDateTime,
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

                                    var dataFileStream = new FileStream(saveFileDialog.FileName, FileMode.Create,
                                                                        FileAccess.Write);
                                    var dataFileWriter = new StreamWriter(dataFileStream);

                                    for (int i = 0; i < cache.Count(); i++)
                                    {
                                        dataFileWriter.WriteLine(cache[i]);
                                    }

                                    dataFileWriter.Close();
                                    dataFileStream.Close();
                                }

                                else if (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString() ==
                                         "Velocity")
                                {
                                    int trialID = _mySqlWrapper.GetTrailID(tempContainer.Study, tempContainer.Group,
                                                                           tempContainer.Szenario,
                                                                           tempContainer.Subject.ID, turnDateTime,
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

                                    var dataFileStream = new FileStream(saveFileDialog.FileName, FileMode.Create,
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
                    }

                    else if (comboBox_TrajectoryVelocity_IndividualMean.SelectedItem.ToString() == "Mean")
                    {
                        var tempTrajectoryVelocityPlotContainerList =
                            new List<TrajectoryVelocityPlotContainer>(
                                listBox_TrajectoryVelocity_SelectedTrials.Items.Cast<TrajectoryVelocityPlotContainer>()
                                                                         .ToList());

                        if (
                            tempTrajectoryVelocityPlotContainerList.Select(t => t.Trials.ToArray())
                                                                   .Distinct(new ArrayComparer())
                                                                   .Count() > 1)
                        {
                            WriteToLogBox("Trial selections are not equal!");
                        }
                        else
                        {
                            string[] studyArray =
                                tempTrajectoryVelocityPlotContainerList.Select(t => t.Study).Distinct().ToArray();
                            string[] groupArray =
                                tempTrajectoryVelocityPlotContainerList.Select(t => t.Group).Distinct().ToArray();
                            string[] szenarioArray =
                                tempTrajectoryVelocityPlotContainerList.Select(t => t.Szenario).Distinct().ToArray();
                            SubjectInformationContainer[] subjectArray =
                                tempTrajectoryVelocityPlotContainerList.Select(t => t.Subject).Distinct().ToArray();
                            int[] turnArray =
                                tempTrajectoryVelocityPlotContainerList.Select(t => t.Turn)
                                                                       .Distinct()
                                                                       .ToArray();
                            int[] targetArray =
                                tempTrajectoryVelocityPlotContainerList.Select(t => t.Target).Distinct().ToArray();
                            string trials = tempTrajectoryVelocityPlotContainerList.ElementAt(0).GetTrialsString();

                            var cache = new List<string>();
                            if (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString() == "Trajectory")
                            {
                                cache.Add(
                                    "Study;Group;Szenario;Subject;Turn;Target;Trial;DataPoint;PositionCartesianX;PositionCartesianZ");
                            }
                            else if (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString() == "Velocity")
                            {
                                cache.Add("Study;Group;Szenario;Subject;Turn;Target;Trial;DataPoint;VelocityXZ");
                            }

                            int counter = 0;
                            for (int targetCounter = 0; targetCounter < targetArray.Length; targetCounter++)
                            {
                                int meanCounter = 0;
                                var dataX = new List<double>();
                                var dataZ = new List<double>();

                                int counterVar = targetCounter;
                                foreach (
                                    TrajectoryVelocityPlotContainer tempContainer in
                                        tempTrajectoryVelocityPlotContainerList)
                                {
                                    if (tempContainer.Target == targetArray[counterVar])
                                    {
                                        SetProgressBarValue((100.0/listBox_TrajectoryVelocity_SelectedTrials.Items.Count)*
                                                            counter);
                                        counter++;
                                        DateTime turnDateTime = _mySqlWrapper.GetTurnDateTime(tempContainer.Study,
                                                                                              tempContainer.Group,
                                                                                              tempContainer.Szenario,
                                                                                              tempContainer.Subject.ID,
                                                                                              tempContainer.Turn);

                                        if (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString() ==
                                            "Trajectory")
                                        {
                                            foreach (int trial in tempContainer.Trials)
                                            {
                                                int trialID = _mySqlWrapper.GetTrailID(tempContainer.Study,
                                                                                       tempContainer.Group,
                                                                                       tempContainer.Szenario,
                                                                                       tempContainer.Subject.ID,
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
                                        else if (
                                            comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString() ==
                                            "Velocity")
                                        {
                                            foreach (int trial in tempContainer.Trials)
                                            {
                                                int trialID = _mySqlWrapper.GetTrailID(tempContainer.Study,
                                                                                       tempContainer.Group,
                                                                                       tempContainer.Szenario,
                                                                                       tempContainer.Subject.ID,
                                                                                       turnDateTime,
                                                                                       tempContainer.Target, trial);

                                                DataSet dataSet =
                                                    _mySqlWrapper.GetVelocityDataNormalizedDataSet(trialID);
                                                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                                                {
                                                    DataRow row = dataSet.Tables[0].Rows[i];

                                                    if (dataX.Count <= i)
                                                    {
                                                        dataX.Add(
                                                            Math.Sqrt(
                                                                Math.Pow(Convert.ToDouble(row["velocity_x"]), 2) +
                                                                Math.Pow(Convert.ToDouble(row["velocity_z"]), 2)));
                                                    }
                                                    else
                                                    {
                                                        dataX[i] +=
                                                            Math.Sqrt(
                                                                Math.Pow(Convert.ToDouble(row["velocity_x"]), 2) +
                                                                Math.Pow(Convert.ToDouble(row["velocity_z"]), 2));
                                                    }
                                                }
                                                meanCounter++;
                                            }
                                        }
                                    }
                                }

                                if (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString() ==
                                    "Trajectory")
                                {
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

                                    var dataFileStream = new FileStream(saveFileDialog.FileName, FileMode.Create,
                                                                        FileAccess.Write);
                                    var dataFileWriter = new StreamWriter(dataFileStream);

                                    for (int i = 0; i < cache.Count(); i++)
                                    {
                                        dataFileWriter.WriteLine(cache[i]);
                                    }

                                    dataFileWriter.Close();
                                    dataFileStream.Close();
                                }
                                else if (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString() ==
                                         "Velocity")
                                {
                                    for (int i = 0; i < dataX.Count; i++)
                                    {
                                        dataX[i] /= meanCounter;
                                    }

                                    counterVar = targetCounter;
                                    cache.AddRange(
                                        dataX.Select(
                                            (t, i) => String.Join(",", studyArray) + ";" +
                                                      String.Join(",", groupArray) + ";" +
                                                      String.Join(",", szenarioArray) + ";" +
                                                      String.Join<SubjectInformationContainer>(",", subjectArray) +
                                                      ";" +
                                                      String.Join(",", turnArray) + ";" + targetArray[counterVar] +
                                                      ";" + trials +
                                                      ";" + i + ";" + DoubleConverter.ToExactString(t)));

                                    var dataFileStream = new FileStream(saveFileDialog.FileName, FileMode.Create,
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
                    }

                    _myMatlabWrapper.ClearWorkspace();
                }
                else
                {
                    WriteToLogBox("Please add data to export!");
                }
            }
            WriteProgressInfo("Ready");
            SetProgressBarValue(0);
        }

        private void button_BaselineMeantime_ExportBaseline_Click(object sender, EventArgs e)
        {
            saveFileDialog = new SaveFileDialog();
            saveFileDialog.Reset();
            saveFileDialog.Title = @"Save trajectory / velocity file";
            saveFileDialog.AddExtension = true;
            saveFileDialog.DefaultExt = ".csv";
            saveFileDialog.Filter = @"DataFiles (*.csv)|.csv";
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.FileName = DateTime.Now.Year.ToString("0000")
                                      + "."
                                      + DateTime.Now.Month.ToString("00")
                                      + "."
                                      + DateTime.Now.Day.ToString("00")
                                      + "-"
                                      + DateTime.Now.Hour.ToString("00")
                                      + "."
                                      + DateTime.Now.Minute.ToString("00")
                                      + "-"
                                      + comboBox_BaselineMeantime_Subject.SelectedItem
                                      + "-TrajectoryBaselineData";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string study = comboBox_BaselineMeantime_Study.SelectedItem.ToString();
                string group = comboBox_BaselineMeantime_Group.SelectedItem.ToString();
                string szenario = comboBox_BaselineMeantime_Szenario.SelectedItem.ToString();
                var subject = (SubjectInformationContainer) comboBox_BaselineMeantime_Subject.SelectedItem;

                DataSet baseline = _mySqlWrapper.GetBaselineDataSet(study, group, szenario, subject.ID);

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

                var dataFileStream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
                var dataFileWriter = new StreamWriter(dataFileStream);

                for (int i = 0; i < cache.Count(); i++)
                {
                    dataFileWriter.WriteLine(cache[i]);
                }

                dataFileWriter.Close();
                dataFileStream.Close();
            }
        }

        private void button_BaselineMeantime_ExportSzenarioMeanTimes_Click(object sender, EventArgs e)
        {
            saveFileDialog = new SaveFileDialog();
            saveFileDialog.Reset();
            saveFileDialog.Title = @"Save trajectory / velocity file";
            saveFileDialog.AddExtension = true;
            saveFileDialog.DefaultExt = ".csv";
            saveFileDialog.Filter = @"DataFiles (*.csv)|.csv";
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.FileName = DateTime.Now.Year.ToString("0000")
                                      + "."
                                      + DateTime.Now.Month.ToString("00")
                                      + "."
                                      + DateTime.Now.Day.ToString("00")
                                      + "-"
                                      + DateTime.Now.Hour.ToString("00")
                                      + "."
                                      + DateTime.Now.Minute.ToString("00")
                                      + "-"
                                      + comboBox_BaselineMeantime_Subject.SelectedItem
                                      + "-SzenarioMeanTimeData";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string study = comboBox_BaselineMeantime_Study.SelectedItem.ToString();
                string group = comboBox_BaselineMeantime_Group.SelectedItem.ToString();
                string szenario = comboBox_BaselineMeantime_Szenario.SelectedItem.ToString();
                var subject = (SubjectInformationContainer) comboBox_BaselineMeantime_Subject.SelectedItem;
                int turn =
                    Convert.ToInt32(comboBox_BaselineMeantime_Turn.SelectedItem.ToString().Substring("Turn".Length));
                DateTime turnDateTime = _mySqlWrapper.GetTurnDateTime(study, group, szenario, subject.ID, turn);

                DataSet meanTimeDataSet = _mySqlWrapper.GetMeanTimeDataSet(study, group, szenario, subject.ID,
                                                                           turnDateTime);

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

                var dataFileStream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
                var dataFileWriter = new StreamWriter(dataFileStream);

                for (int i = 0; i < cache.Count(); i++)
                {
                    dataFileWriter.WriteLine(cache[i]);
                }

                dataFileWriter.Close();
                dataFileStream.Close();
            }
        }

        private void button_Import_ClearMeasureFileList_Click(object sender, EventArgs e)
        {
            listBox_Import_SelectedMeasureFiles.Items.Clear();
        }

        private void checkBox_DescriptiveStatistic1_ShowCatchTrialsExclusivly_CheckedChanged(object sender, EventArgs e)
        {
            listBox_DescriptiveStatistic1_Turns_SelectedIndexChanged(this, new EventArgs());
        }

        private void checkBox_DescriptiveStatistic2_ShowCatchTrialsExclusivly_CheckedChanged(object sender, EventArgs e)
        {
            listBox_DescriptiveStatistic2_Turns_SelectedIndexChanged(this, new EventArgs());
        }

        private void button_DataManipulation_DeleteMeasureFile_Click(object sender, EventArgs e)
        {
            WriteProgressInfo("Deleting measure file...");
            _mySqlWrapper.DeleteMeasureFile(Convert.ToInt32(textBox_DataManipulation_MeasureFileID));
            WriteProgressInfo("Ready");
        }

        private void button_Debug_CleanOrphanedEntries_Click(object sender, EventArgs e)
        {
            WriteProgressInfo("Cleaning orphaned entries...");
            _mySqlWrapper.CleanOrphanedEntries();
            WriteProgressInfo("Ready");
        }

        private void button_Start_SelectDatabase_Click(object sender, EventArgs e)
        {
            _mySqlWrapper.SetDatabase(comboBox_Start_Database.SelectedItem.ToString());

            if (!tabControl.TabPages.Contains(tabPage_VisualizationExport))
            {
                tabControl.TabPages.Remove(tabPage_Impressum);
                tabControl.TabPages.Add(tabPage_VisualizationExport);
                tabControl.TabPages.Add(tabPage_Impressum);
            }
            checkBox_Start_ManualMode.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string study = comboBox_BaselineMeantime_Study.SelectedItem.ToString();
            string group = comboBox_BaselineMeantime_Group.SelectedItem.ToString();
            string szenario = comboBox_BaselineMeantime_Szenario.SelectedItem.ToString();
            var subject = (SubjectInformationContainer) comboBox_BaselineMeantime_Subject.SelectedItem;

            _myMatlabWrapper.CreateVelocityFigure("Velocity baseline plot", 101);
            _myMatlabWrapper.DrawTargets(0.005, 0.1, 0, 0);

            DataSet baseline = _mySqlWrapper.GetBaselineDataSet(study, group, szenario, subject.ID);

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

        private void button1_Click(object sender, EventArgs e)
        {
            saveFileDialog = new SaveFileDialog();
            saveFileDialog.Reset();
            saveFileDialog.Title = @"Save trajectory / velocity file";
            saveFileDialog.AddExtension = true;
            saveFileDialog.DefaultExt = ".csv";
            saveFileDialog.Filter = @"DataFiles (*.csv)|.csv";
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.FileName = DateTime.Now.Year.ToString("0000")
                                      + "."
                                      + DateTime.Now.Month.ToString("00")
                                      + "."
                                      + DateTime.Now.Day.ToString("00")
                                      + "-"
                                      + DateTime.Now.Hour.ToString("00")
                                      + "."
                                      + DateTime.Now.Minute.ToString("00")
                                      + "-"
                                      + comboBox_BaselineMeantime_Subject.SelectedItem
                                      + "-VelocityBaselineData";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string study = comboBox_BaselineMeantime_Study.SelectedItem.ToString();
                string group = comboBox_BaselineMeantime_Group.SelectedItem.ToString();
                string szenario = comboBox_BaselineMeantime_Szenario.SelectedItem.ToString();
                var subject = (SubjectInformationContainer) comboBox_BaselineMeantime_Subject.SelectedItem;

                DataSet baseline = _mySqlWrapper.GetBaselineDataSet(study, group, szenario, subject.ID);

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

                var dataFileStream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
                var dataFileWriter = new StreamWriter(dataFileStream);

                for (int i = 0; i < cache.Count(); i++)
                {
                    dataFileWriter.WriteLine(cache[i]);
                }

                dataFileWriter.Close();
                dataFileStream.Close();
            }
        }

        private delegate void LogBoxCallbackAddString(string text);

        private delegate void LogBoxCallbackAddStringArray(string[] textArray);

        private delegate void LogBoxCallbackClearItems();

        private delegate string[] LogBoxCallbackGetText();

        private delegate void ProgressBarCallback(double value);

        private delegate void ProgressLabelCallback(string text);

        private delegate void TabControlCallback(bool enable);
    }
}