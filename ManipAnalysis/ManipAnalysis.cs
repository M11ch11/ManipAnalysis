using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Globalization;
using System.Net;
using System.Net.Sockets;

namespace ManipAnalysis
{
    public partial class ManipAnalysis : Form
    {
        private SqlWrapper mySQLWrapper;
        private MatlabWrapper myMatlabWrapper;
        private SplashScreen splash;

        private delegate void myTabControlCallback(bool enable);
        private delegate void myProgressBarCallback(double value);
        private delegate void myProgressLabelCallback(string text);
        private delegate void myLogBoxCallbackAddString(string text);
        private delegate void myLogBoxCallbackAddStringArray(string[] textArray);
        private delegate string[] myLogBoxCallbackGetText();
        private delegate void myLogBoxCallbackClearItems();

        public ManipAnalysis()
        {
            splash = new SplashScreen();
            splash.Show();      

            try
            {
                mySQLWrapper = new SqlWrapper(this);
                myMatlabWrapper = new MatlabWrapper(this);                   
            }
            catch(Exception ex)                
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

        public void writeToLogBox(string text)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new myLogBoxCallbackAddString(writeToLogBox), text);
            }
            else
            {
                listBox_LogBox.Items.Add("[" + DateTime.Now + "] " + text);
            }     
        }

        public void writeToLogBox(string[] textArray)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new myLogBoxCallbackAddStringArray(writeToLogBox), textArray);
            }
            else
            {
                
                for (int i = 0; i < textArray.Length; i++)
                {
                    if (i == 0)
                    {
                        listBox_LogBox.Items.Add("[" + DateTime.Now + "] " + textArray[0]);
                    }
                    else
                    {
                        listBox_LogBox.Items.Add(textArray[i]);
                    }
                }
            }
        }

        public string[] getLogBoxText()
        {
            string[] retVal = null;

            if (InvokeRequired)
            {
                BeginInvoke(new myLogBoxCallbackGetText(getLogBoxText));
            }
            else
            {
                retVal = listBox_LogBox.Items.Cast<string>().ToArray();
            }

            return retVal;
        }

        public void clearLogBox()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new myLogBoxCallbackClearItems(clearLogBox));
            }
            else
            {
                listBox_LogBox.Items.Clear();
            }
        }

        private void button_OpenMeasureFiles_Click(object sender, EventArgs e)
        {
            openFileDialog.Reset();
            openFileDialog.Multiselect = true;
            openFileDialog.Title = "Select measure-file(s)";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog.Filter = "MeasureData-files (*.csv)|*.csv";
            openFileDialog.ShowDialog();

            List<FileInfo> filesList = new List<FileInfo>(openFileDialog.FileNames.Select(t => new FileInfo(t)));

            bool isValid = true;

            filesList.RemoveAll(t => (!t.Name.Contains("Szenario")));
            filesList.RemoveAll(t => (t.Name.Contains("Szenario00")));
            filesList.RemoveAll(t => (t.Name.Contains("Szenario01")));


            for (int i = 0; i < filesList.Count; i++)
            {
                if (filesList[i].Name.Count(t => t == '-') == 6)
                {
                    string tempFileHash = MD5.computeHash(filesList[i].FullName);

                    if (!mySQLWrapper.checkIfMeasureFileHashExists(tempFileHash))
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
                writeToLogBox("One or more filenames are invalid!");
            }
        }

        private void button_OpenMeasureFilesFolder_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.Reset();
            folderBrowserDialog.ShowDialog();

            List<DirectoryInfo> directoriesList = new List<DirectoryInfo>(); ;
            List<FileInfo> filesList = new List<FileInfo>();
            DirectoryInfo rootDir;

            string path = folderBrowserDialog.SelectedPath;

            if (path != "")
            {
                rootDir = new DirectoryInfo(path);

                getSubDirectories(ref directoriesList, rootDir);

                for (int i = 0; i < directoriesList.Count; i++)
                {
                    DirectoryInfo di = directoriesList[i];

                    FileInfo[] files = di.GetFiles("*.csv");

                    if (files != null)
                    {
                        filesList.AddRange(files);
                    }
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
                    if (fi.Name.Count(t => t == '-') == 6)    // Study 2
                    {
                        string tempFileHash = MD5.computeHash(fi.FullName);

                        if (!mySQLWrapper.checkIfMeasureFileHashExists(tempFileHash))
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
                    writeToLogBox("One or more filenames are invalid!");
                }
            }
        }

        private void getSubDirectories(ref List<DirectoryInfo> directoriesList, DirectoryInfo rootDir)
        {
            directoriesList.Add(rootDir);
            DirectoryInfo[] dirs = rootDir.GetDirectories();

            if (dirs != null)
            {
                directoriesList.AddRange(dirs);

                for (int i = 0; i < dirs.Count(); i++)
                {
                    DirectoryInfo di = dirs[i];
                    getSubDirectories(ref directoriesList, di);
                }
            }
        }

        private void ManipAnalysis_FormClosed(object sender, FormClosedEventArgs e)
        {
            mySQLWrapper.closeSqlConnection();
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
            SubjectInformationContainer subject = (SubjectInformationContainer)comboBox_BaselineMeantime_Subject.SelectedItem;
            int turn = Convert.ToInt32(comboBox_BaselineMeantime_Turn.SelectedItem.ToString().Substring("Turn".Length));
            DateTime turnDateTime = mySQLWrapper.getTurnDateTime(study, group, szenario, subject.id, turn);

            DataSet meanTimeDataSet = mySQLWrapper.getMeanTimeDataSet(study, group, szenario, subject.id, turnDateTime);

            myMatlabWrapper.createMeanTimeFigure();            
            
            List<double> meanTimeList = new List<double>();
            List<double> meanTimeStdList = new List<double>();
            List<int> targetList = new List<int>();

            foreach (DataRow row in meanTimeDataSet.Tables[0].Rows)
            {
                meanTimeList.Add(TimeSpan.Parse(Convert.ToString(row["szenario_mean_time"])).TotalSeconds);
                meanTimeStdList.Add(TimeSpan.Parse(Convert.ToString(row["szenario_mean_time_std"])).TotalSeconds);
                targetList.Add(Convert.ToInt32(row["target_number"]));
            }

            meanTimeList.Add(meanTimeList.Average());
            meanTimeStdList.Add(meanTimeStdList.Average());
            targetList.Add(17);

            myMatlabWrapper.setWorkspaceData("target", targetList.ToArray());
            myMatlabWrapper.setWorkspaceData("meanTime", meanTimeList.ToArray());
            myMatlabWrapper.setWorkspaceData("meanTimeStd", meanTimeStdList.ToArray());
            myMatlabWrapper.plotMeanTimeErrorBar("target", "meanTime", "meanTimeStd");

            myMatlabWrapper.clearWorkspace();            
        }

        private void button_ShowMatlabWindow_Click(object sender, EventArgs e)
        {
            myMatlabWrapper.toggleShowCommandWindow();
        }

        private void button_ShowMatlabWorkspace_Click(object sender, EventArgs e)
        {
            myMatlabWrapper.showWorkspaceWindow();
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
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            result = MessageBox.Show("Are you really sure you want to initialise the Database?", "Really?", buttons);

            if (result == DialogResult.Yes)
            {
                mySQLWrapper.initializeDatabase();
            }
        }

        private void button_ChangeSQlServerConnection_Click(object sender, EventArgs e)
        {
            bool _serverAvailable = false;
            comboBox_Start_Database.Items.Clear();

            using (TcpClient tcp = new TcpClient())
            {
                IAsyncResult ar = tcp.BeginConnect(comboBox_Start_SQL_Server.Text, 1433, null, null);
                System.Threading.WaitHandle wh = ar.AsyncWaitHandle;
                try
                {
                    if (!ar.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(2), false))
                    {
                        tcp.Close();
                        throw new TimeoutException();
                    }

                    tcp.EndConnect(ar);
                    _serverAvailable = true;
                }
                catch
                {
                    writeToLogBox("SQL-Server not reachable!");
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

            if (_serverAvailable)
            {
                mySQLWrapper.setSqlServer(comboBox_Start_SQL_Server.Text);
                writeToLogBox("Connected to SQL-Server.");
                comboBox_Start_Database.Items.AddRange(mySQLWrapper.getDatabases());
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
            SubjectInformationContainer subject = (SubjectInformationContainer)comboBox_BaselineMeantime_Subject.SelectedItem;

            myMatlabWrapper.createTrajectoryFigure("Trajectory baseline plot");
            myMatlabWrapper.drawTargets(0.005, 0.1, 0, 0);

            DataSet baseline = mySQLWrapper.getBaselineDataSet(study, group, szenario, subject.id);

            List<object[]> baselineData = new List<object[]>();

            foreach (DataRow row in baseline.Tables[0].Rows)
            {
                baselineData.Add(new object[] { Convert.ToDouble(row["baseline_position_cartesian_x"]), Convert.ToDouble(row["baseline_position_cartesian_z"]), Convert.ToInt32(row["target_number"]) });
            }

            int[] targetNumberArray = baselineData.Select(t => Convert.ToInt32(t[2])).Distinct().ToArray();

            for (int i = 0; i < targetNumberArray.Length; i++)
            {
                double[] tempX = baselineData.Where(t => Convert.ToInt32(t[2]) == targetNumberArray[i]).Select(t => Convert.ToDouble(t[0])).ToArray();
                double[] tempZ = baselineData.Where(t => Convert.ToInt32(t[2]) == targetNumberArray[i]).Select(t => Convert.ToDouble(t[1])).ToArray();

                myMatlabWrapper.setWorkspaceData("X", tempX);
                myMatlabWrapper.setWorkspaceData("Z", tempZ);
                myMatlabWrapper.plot("X", "Z", "black", 2);
            }

            myMatlabWrapper.clearWorkspace();
        }

        private void enableTabPages(bool enable)
        {
            if (tabControl.InvokeRequired)
            {
                myTabControlCallback _enableTabPages = new myTabControlCallback(enableTabPages);
                tabControl.Invoke(_enableTabPages, new object[] { enable });
            }
            else
            {
                tabControl.Enabled = enable;
            }
        }

        private void setProgressBarValue(double value)
        {
            if (progressBar.InvokeRequired)
            {
                myProgressBarCallback _setProgressBarValue = new myProgressBarCallback(setProgressBarValue);
                progressBar.Invoke(_setProgressBarValue, new object[] { value });
            }
            else
            {
                progressBar.Value = Convert.ToInt32(value);
            }
        }

        public void writeProgressInfo(string text)
        {
            if (label_ProgressInfo.InvokeRequired)
            {
                myProgressLabelCallback _writeProgressInfo = new myProgressLabelCallback(writeProgressInfo);
                label_ProgressInfo.Invoke(_writeProgressInfo, new object[] { text });
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

            string[] groupNames = mySQLWrapper.getGroupNames(comboBox_DescriptiveStatistic1_Study.SelectedItem.ToString());
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

                string[] szenarioIntersect = mySQLWrapper.getSzenarioNames(study, groups[0]);
                for (int i = 1; i < groups.Length; i++)
                {
                    szenarioIntersect = szenarioIntersect.Intersect(mySQLWrapper.getSzenarioNames(study, groups[i])).ToArray();
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
                listBox_DescriptiveStatistic1_Subjects.Items.AddRange(mySQLWrapper.getSubjectInformations(study, groups[i], szenario).ToArray());
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
            int[] subjectIDs = listBox_DescriptiveStatistic1_Subjects.SelectedItems.Cast<SubjectInformationContainer>().Select(t => t.id).ToArray();

            string[] turnIntersect = null;
            for (int i = 0; i < groups.Length; i++)
            {
                for (int j = 0; j < subjectIDs.Length; j++)
                {
                    string[] tempTurnString = mySQLWrapper.getTurns(study, groups[i], szenario, subjectIDs[j]);

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
            listBox_DescriptiveStatistic1_Turns.Items.AddRange(turnIntersect);
            listBox_DescriptiveStatistic1_Turns.SelectedIndex = 0;
        }

        private void listBox_DescriptiveStatistic1_Turns_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox_DescriptiveStatistic1_Trials.Items.Clear();

            string study = comboBox_DescriptiveStatistic1_Study.SelectedItem.ToString();
            string szenario = comboBox_DescriptiveStatistic1_Szenario.SelectedItem.ToString();
            bool showCatchTrials = checkBox_DescriptiveStatistic1_ShowCatchTrials.Checked;
            bool showCatchTrialsExclusivly = checkBox_DescriptiveStatistic1_ShowCatchTrialsExclusivly.Checked;

            string[] szenarioTrialNames = mySQLWrapper.getSzenarioTrials(study, szenario, showCatchTrials, showCatchTrialsExclusivly);

            listBox_DescriptiveStatistic1_Trials.Items.AddRange(szenarioTrialNames);
            listBox_DescriptiveStatistic1_Trials.SelectedIndex = 0;
        }

        private void button_StatisticPlots_AddSelected_Click(object sender, EventArgs e)
        {
            if (comboBox_DescriptiveStatistic1_Study.SelectedItem != null)
            {
                string study = comboBox_DescriptiveStatistic1_Study.SelectedItem.ToString();
                string[] groups = listBox_DescriptiveStatistic1_Groups.SelectedItems.Cast<string>().ToArray();
                string szenario = comboBox_DescriptiveStatistic1_Szenario.SelectedItem.ToString();
                SubjectInformationContainer[] subjects = listBox_DescriptiveStatistic1_Subjects.SelectedItems.Cast<SubjectInformationContainer>().ToArray();
                string[] turns = listBox_DescriptiveStatistic1_Turns.SelectedItems.Cast<string>().ToArray();
                string[] trials = listBox_DescriptiveStatistic1_Trials.SelectedItems.Cast<string>().ToArray();

                foreach (string group in groups)
                {
                    foreach (SubjectInformationContainer subject in subjects)
                    {
                        foreach (string turn in turns)
                        {
                            if (mySQLWrapper.getTurns(study, group, szenario, subject.id) != null)
                            {
                                if (listBox_DescriptiveStatistic1_SelectedTrials.Items.Count > 0)
                                {
                                    bool canBeUpdated = false;
                                    foreach (StatisticPlotContainer temp in listBox_DescriptiveStatistic1_SelectedTrials.Items)
                                    {
                                        if (temp.updateStatisticPlotContainer(study, group, szenario, subject, turn, trials))
                                        {
                                            typeof(ListBox).InvokeMember("RefreshItems",
                                            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod,
                                            null, listBox_DescriptiveStatistic1_SelectedTrials, new object[] { });
                                            canBeUpdated = true;
                                        }
                                    }

                                    if (!canBeUpdated)
                                    {
                                        listBox_DescriptiveStatistic1_SelectedTrials.Items.Add(new StatisticPlotContainer(study, group, szenario, subject, turn, trials));
                                    }
                                }
                                else
                                {
                                    listBox_DescriptiveStatistic1_SelectedTrials.Items.Add(new StatisticPlotContainer(study, group, szenario, subject, turn, trials));
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
                listBox_DescriptiveStatistic1_SelectedTrials.Items.Remove(listBox_DescriptiveStatistic1_SelectedTrials.SelectedItem);
            }
        }

        private void button_StatisticPlots_ClearAll_Click(object sender, EventArgs e)
        {
            listBox_DescriptiveStatistic1_SelectedTrials.Items.Clear();
        }

        private void button_StatisticPlots_PlotMeanStd_Click(object sender, EventArgs e)
        {
            writeProgressInfo("Getting data...");
            if (listBox_DescriptiveStatistic1_SelectedTrials.Items.Count > 0)
            {
                bool isValid = true;
                List<int> trialList = listBox_DescriptiveStatistic1_SelectedTrials.Items.Cast<StatisticPlotContainer>().ElementAt(0).Trials;
                foreach (StatisticPlotContainer temp in listBox_DescriptiveStatistic1_SelectedTrials.Items)
                {
                    if (!trialList.SequenceEqual(temp.Trials))
                    {
                        writeToLogBox("Trial selections are not equal!");
                        isValid = false;
                        break;
                    }
                }

                if (isValid)
                {
                    int meanCounter;
                    double[,] data = new double[trialList.Count, listBox_DescriptiveStatistic1_SelectedTrials.Items.Count];

                    for (meanCounter = 0; meanCounter < listBox_DescriptiveStatistic1_SelectedTrials.Items.Count; meanCounter++)
                    {
                        setProgressBarValue((100.0 / listBox_DescriptiveStatistic1_SelectedTrials.Items.Count) * meanCounter);
                        StatisticPlotContainer temp = listBox_DescriptiveStatistic1_SelectedTrials.Items.Cast<StatisticPlotContainer>().ElementAt(meanCounter);

                        DateTime turn = mySQLWrapper.getTurnDateTime(temp.Study, temp.Group, temp.Szenario, temp.Subject.id, Convert.ToInt32(temp.Turn.Substring("Turn".Length)));
                        DataSet statisticDataSet = mySQLWrapper.getStatisticDataSet(temp.Study, temp.Group, temp.Szenario, temp.Subject.id, turn);

                        int trialListCounter = 0;
                        foreach (DataRow row in statisticDataSet.Tables[0].Rows)
                        {
                            int szenarioTrialNumber = Convert.ToInt32(row["szenario_trial_number"]);
                            if (trialList.Contains(szenarioTrialNumber))
                            {
                                switch (comboBox_DescriptiveStatistic1_DataTypeSelect.SelectedItem.ToString())
                                {
                                    case "Vector correlation":
                                        data[trialListCounter, meanCounter] = Convert.ToDouble(row["velocity_vector_correlation"]);
                                        break;

                                    case "Perpendicular distance 300ms - Abs":
                                        data[trialListCounter, meanCounter] = Convert.ToDouble(row["perpendicular_displacement_300ms_abs"]);
                                        break;

                                    case "Mean perpendicular distance - Abs":
                                        data[trialListCounter, meanCounter] = Convert.ToDouble(row["mean_perpendicular_displacement_abs"]);
                                        break;

                                    case "Max perpendicular distance - Abs":
                                        data[trialListCounter, meanCounter] = Convert.ToDouble(row["maximal_perpendicular_displacement_abs"]);
                                        break;

                                    case "Perpendicular distance 300ms - Sign":
                                        data[trialListCounter, meanCounter] = Convert.ToDouble(row["perpendicular_displacement_300ms_sign"]);
                                        break;

                                    case "Max perpendicular distance - Sign":
                                        data[trialListCounter, meanCounter] = Convert.ToDouble(row["maximal_perpendicular_displacement_sign"]);
                                        break;

                                    case "Trajectory length abs":
                                        data[trialListCounter, meanCounter] = Convert.ToDouble(row["trajectory_length_abs"]);
                                        break;

                                    case "Trajectory length ratio":
                                        data[trialListCounter, meanCounter] = Convert.ToDouble(row["trajectory_length_ratio_baseline"]);
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

                    myMatlabWrapper.setWorkspaceData("data", data);

                    double[,] dataStdPlot = null;
                    double[,] dataPlot = null;

                    if (meanCounter > 1)
                    {
                        myMatlabWrapper.execute("dataPlot = mean(transpose(data));");
                        myMatlabWrapper.execute("dataStdPlot = std(transpose(data));");    
                        dataStdPlot = myMatlabWrapper.getWorkspaceData("dataStdPlot");
                    }
                    else
                    {
                        myMatlabWrapper.execute("dataPlot = data;");
                    }

                    dataPlot = myMatlabWrapper.getWorkspaceData("dataPlot");
                   
                    switch (comboBox_DescriptiveStatistic1_DataTypeSelect.SelectedItem.ToString())
                    {
                        case "Vector correlation":
                            myMatlabWrapper.createStatisticFigure("Velocity Vector Correlation plot", "dataPlot", "fit(transpose([1:1:length(dataPlot)]),transpose(dataPlot),'" + textBox_DescriptiveStatistic1_FitEquation.Text + "')", "dataStdPlot", "[Trial]", "Velocity Vector Correlation", 1, dataPlot.Length, 0.5, 0.9, checkBox_DescriptiveStatistic1_PlotFit.Checked, checkBox_DescriptiveStatistic1_PlotErrorbars.Checked);
                            break;

                        case "Perpendicular distance 300ms - Abs":
                            myMatlabWrapper.createStatisticFigure("PD300 abs plot", "dataPlot", "fit(transpose([1:1:length(dataPlot)]),transpose(dataPlot),'" + textBox_DescriptiveStatistic1_FitEquation.Text + "')", "dataStdPlot", "[Trial]", "PD300 [m]", 1, dataPlot.Length, 0, 0.05, checkBox_DescriptiveStatistic1_PlotFit.Checked, checkBox_DescriptiveStatistic1_PlotErrorbars.Checked);
                            break;

                        case "Mean perpendicular distance - Abs":
                            myMatlabWrapper.createStatisticFigure("MeanPD abs plot", "dataPlot", "fit(transpose([1:1:length(dataPlot)]),transpose(dataPlot),'" + textBox_DescriptiveStatistic1_FitEquation.Text + "')", "dataStdPlot", "[Trial]", "MeanPD [m]", 1, dataPlot.Length, 0, 0.05, checkBox_DescriptiveStatistic1_PlotFit.Checked, checkBox_DescriptiveStatistic1_PlotErrorbars.Checked);
                            break;

                        case "Max perpendicular distance - Abs":
                            myMatlabWrapper.createStatisticFigure("MaxPD abs plot", "dataPlot", "fit(transpose([1:1:length(dataPlot)]),transpose(dataPlot),'" + textBox_DescriptiveStatistic1_FitEquation.Text + "')", "dataStdPlot", "[Trial]", "MaxPD [m]", 1, dataPlot.Length, 0, 0.05, checkBox_DescriptiveStatistic1_PlotFit.Checked, checkBox_DescriptiveStatistic1_PlotErrorbars.Checked);
                            break;

                        case "Perpendicular distance 300ms - Sign":
                            myMatlabWrapper.createStatisticFigure("PD300 sign plot", "dataPlot", "fit(transpose([1:1:length(dataPlot)]),transpose(dataPlot),'" + textBox_DescriptiveStatistic1_FitEquation.Text + "')", "dataStdPlot", "[Trial]", "PD300 [m]", 1, dataPlot.Length, -0.05, 0.05, checkBox_DescriptiveStatistic1_PlotFit.Checked, checkBox_DescriptiveStatistic1_PlotErrorbars.Checked);
                            break;

                        case "Max perpendicular distance - Sign":
                            myMatlabWrapper.createStatisticFigure("MaxPD sign plot", "dataPlot", "fit(transpose([1:1:length(dataPlot)]),transpose(dataPlot),'" + textBox_DescriptiveStatistic1_FitEquation.Text + "')", "dataStdPlot", "[Trial]", "MaxPD [m]", 1, dataPlot.Length, -0.05, 0.05, checkBox_DescriptiveStatistic1_PlotFit.Checked, checkBox_DescriptiveStatistic1_PlotErrorbars.Checked);
                            break;

                        case "Trajectory length abs":
                            myMatlabWrapper.createStatisticFigure("Trajectory Length plot", "dataPlot", "fit(transpose([1:1:length(dataPlot)]),transpose(dataPlot),'" + textBox_DescriptiveStatistic1_FitEquation.Text + "')", "dataStdPlot", "[Trial]", "Trajectory Length [m]", 1, dataPlot.Length, 0.07, 0.2, checkBox_DescriptiveStatistic1_PlotFit.Checked, checkBox_DescriptiveStatistic1_PlotErrorbars.Checked);
                            break;

                        case "Trajectory length ratio":
                            myMatlabWrapper.createStatisticFigure("Trajectory Length Ratio plot", "dataPlot", "fit(transpose([1:1:length(dataPlot)]),transpose(dataPlot),'" + textBox_DescriptiveStatistic1_FitEquation.Text + "')", "dataStdPlot", "[Trial]", "Trajectory Length Ratio", 1, dataPlot.Length, 0.2, 1.8, checkBox_DescriptiveStatistic1_PlotFit.Checked, checkBox_DescriptiveStatistic1_PlotErrorbars.Checked);
                            break;

                        case "Enclosed area":
                            myMatlabWrapper.createStatisticFigure("Enclosed area plot", "dataPlot", "fit(transpose([1:1:length(dataPlot)]),transpose(dataPlot),'" + textBox_DescriptiveStatistic1_FitEquation.Text + "')", "dataStdPlot", "[Trial]", "Enclosed Area [m²]", 1, dataPlot.Length, 0, 0.002, checkBox_DescriptiveStatistic1_PlotFit.Checked, checkBox_DescriptiveStatistic1_PlotErrorbars.Checked);
                            break;

                        case "RMSE":
                            myMatlabWrapper.createStatisticFigure("Root Mean Square Error plot", "dataPlot", "fit(transpose([1:1:length(dataPlot)]),transpose(dataPlot),'" + textBox_DescriptiveStatistic1_FitEquation.Text + "')", "dataStdPlot", "[Trial]", "Root Mean Square Error", 1, dataPlot.Length, 0, 0.1, checkBox_DescriptiveStatistic1_PlotFit.Checked, checkBox_DescriptiveStatistic1_PlotErrorbars.Checked);
                            break;
                    }

                    myMatlabWrapper.clearWorkspace();
                }
            }
            writeProgressInfo("Ready");
            setProgressBarValue(0);
        }

        private void button_StatisticPlots_AddAll_Click(object sender, EventArgs e)
        {
            if (comboBox_DescriptiveStatistic1_Study.SelectedItem != null)
            {
                string study = comboBox_DescriptiveStatistic1_Study.SelectedItem.ToString();
                string[] groups = listBox_DescriptiveStatistic1_Groups.SelectedItems.Cast<string>().ToArray();
                string szenario = comboBox_DescriptiveStatistic1_Szenario.SelectedItem.ToString();
                SubjectInformationContainer[] subjects = listBox_DescriptiveStatistic1_Subjects.SelectedItems.Cast<SubjectInformationContainer>().ToArray();
                string[] turns = listBox_DescriptiveStatistic1_Turns.SelectedItems.Cast<string>().ToArray();
                string[] trials = listBox_DescriptiveStatistic1_Trials.Items.Cast<string>().ToArray();

                foreach (string group in groups)
                {
                    foreach (SubjectInformationContainer subject in subjects)
                    {
                        foreach (string turn in turns)
                        {
                            if (mySQLWrapper.getTurns(study, group, szenario, subject.id) != null)
                            {
                                if (listBox_DescriptiveStatistic1_SelectedTrials.Items.Count > 0)
                                {
                                    bool canBeUpdated = false;
                                    foreach (StatisticPlotContainer temp in listBox_DescriptiveStatistic1_SelectedTrials.Items)
                                    {
                                        if (temp.updateStatisticPlotContainer(study, group, szenario, subject, turn, trials))
                                        {
                                            typeof(ListBox).InvokeMember("RefreshItems",
                                            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod,
                                            null, listBox_DescriptiveStatistic1_SelectedTrials, new object[] { });
                                            canBeUpdated = true;
                                        }
                                    }

                                    if (!canBeUpdated)
                                    {
                                        listBox_DescriptiveStatistic1_SelectedTrials.Items.Add(new StatisticPlotContainer(study, group, szenario, subject, turn, trials));
                                    }
                                }
                                else
                                {
                                    listBox_DescriptiveStatistic1_SelectedTrials.Items.Add(new StatisticPlotContainer(study, group, szenario, subject, turn, trials));
                                }
                            }
                        }
                    }
                }
            }
        }

        private void button_showFaultyTrials_Click(object sender, EventArgs e)
        {
            List<object[]> faultyTrialInfo = mySQLWrapper.getFaultyTrialInformation();

            if (faultyTrialInfo != null)
            {
                List<string[]> cache = new List<string[]>();
                for (int i = 0; i < faultyTrialInfo.Count; i++)
                {
                    cache.Add(new string[]{
                                                Convert.ToString(faultyTrialInfo[i][3]),
                                                Convert.ToString(faultyTrialInfo[i][4]),
                                                Convert.ToString(faultyTrialInfo[i][6]),
                                                Convert.ToString(faultyTrialInfo[i][5]),
                                                Convert.ToString(Convert.ToDateTime(faultyTrialInfo[i][7])),
                                                Convert.ToString(Convert.ToInt32(faultyTrialInfo[i][8]))
                                              });
                }
                writeToLogBox("------------------------------------------------------- Faulty trial list -------------------------------------------------------");
                writeToLogBox(cache.OrderBy(t => t[4]).Select(t => t[0] + " - " + t[1] + " - " + t[2] + " - " + t[3] + " - " + t[4] + " - Trial " + t[5]).ToArray());
                writeToLogBox("---------------------------------------------------------------------------------------------------------------------------------");
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

            string[] studyNames = mySQLWrapper.getStudyNames();
            if (studyNames != null)
            {
                comboBox_DescriptiveStatistic1_Study.Items.AddRange(studyNames);
                comboBox_DescriptiveStatistic1_Study.SelectedIndex = 0;
            }
        }

        private void checkBox_DescriptiveStatistic1_ShowCatchTrials_CheckedChanged(object sender, EventArgs e)
        {
            listBox_DescriptiveStatistic1_Turns_SelectedIndexChanged(this, new EventArgs());
            checkBox_DescriptiveStatistic1_ShowCatchTrialsExclusivly.Enabled = checkBox_DescriptiveStatistic1_ShowCatchTrials.Checked;
        }

        private void comboBox_DescriptiveStatistic2_Study_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox_DescriptiveStatistic2_Groups.Items.Clear();
            comboBox_DescriptiveStatistic2_Szenario.Items.Clear();
            listBox_DescriptiveStatistic2_Subjects.Items.Clear();
            listBox_DescriptiveStatistic2_Turns.Items.Clear();
            listBox_DescriptiveStatistic2_Trials.Items.Clear();

            string[] groupNames = mySQLWrapper.getGroupNames(comboBox_DescriptiveStatistic2_Study.SelectedItem.ToString());
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

                string[] szenarioIntersect = mySQLWrapper.getSzenarioNames(study, groups[0]);
                for (int i = 1; i < groups.Length; i++)
                {
                    szenarioIntersect = szenarioIntersect.Intersect(mySQLWrapper.getSzenarioNames(study, groups[i])).ToArray();
                }

                comboBox_DescriptiveStatistic2_Szenario.Items.AddRange(szenarioIntersect);
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
                listBox_DescriptiveStatistic2_Subjects.Items.AddRange(mySQLWrapper.getSubjectInformations(study, groups[i], szenario).ToArray());
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
            SubjectInformationContainer[] subjects = listBox_DescriptiveStatistic2_Subjects.SelectedItems.Cast<SubjectInformationContainer>().ToArray();

            string[] turnIntersect = null;
            for (int i = 0; i < groups.Length; i++)
            {
                for (int j = 0; j < subjects.Length; j++)
                {
                    string[] tempTurnString = mySQLWrapper.getTurns(study, groups[i], szenario, subjects[j].id); ;

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
            listBox_DescriptiveStatistic2_Turns.Items.AddRange(turnIntersect);
            listBox_DescriptiveStatistic2_Turns.SelectedIndex = 0;
        }

        private void listBox_DescriptiveStatistic2_Turns_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox_DescriptiveStatistic2_Trials.Items.Clear();

            string study = comboBox_DescriptiveStatistic2_Study.SelectedItem.ToString();
            string szenario = comboBox_DescriptiveStatistic2_Szenario.SelectedItem.ToString();
            bool showCatchTrials = checkBox_DescriptiveStatistic2_ShowCatchTrials.Checked;
            bool showCatchTrialsExclusivly = checkBox_DescriptiveStatistic2_ShowCatchTrialsExclusivly.Checked;

            string[] szenarioTrialNames = mySQLWrapper.getSzenarioTrials(study, szenario, showCatchTrials, showCatchTrialsExclusivly);

            listBox_DescriptiveStatistic2_Trials.Items.AddRange(szenarioTrialNames);
            listBox_DescriptiveStatistic2_Trials.SelectedIndex = 0;
        }

        private void checkBox_DescriptiveStatistic2_ShowCatchTrials_CheckedChanged(object sender, EventArgs e)
        {
            listBox_DescriptiveStatistic2_Turns_SelectedIndexChanged(this, new EventArgs());
            checkBox_DescriptiveStatistic2_ShowCatchTrialsExclusivly.Enabled = checkBox_DescriptiveStatistic2_ShowCatchTrials.Checked;
        }

        private void button_DescriptiveStatistic2_AddSelected_Click(object sender, EventArgs e)
        {
            if (comboBox_DescriptiveStatistic2_Study.SelectedItem != null)
            {
                string study = comboBox_DescriptiveStatistic2_Study.SelectedItem.ToString();
                string[] groups = listBox_DescriptiveStatistic2_Groups.SelectedItems.Cast<string>().ToArray();
                string szenario = comboBox_DescriptiveStatistic2_Szenario.SelectedItem.ToString();
                SubjectInformationContainer[] subjects = listBox_DescriptiveStatistic2_Subjects.SelectedItems.Cast<SubjectInformationContainer>().ToArray();
                string[] turns = listBox_DescriptiveStatistic2_Turns.SelectedItems.Cast<string>().ToArray();
                string[] trials = listBox_DescriptiveStatistic2_Trials.SelectedItems.Cast<string>().ToArray();

                foreach (string group in groups)
                {
                    foreach (SubjectInformationContainer subject in subjects)
                    {
                        foreach (string turn in turns)
                        {
                            listBox_DescriptiveStatistic2_SelectedTrials.Items.Add(new StatisticPlotContainer(study, group, szenario, subject, turn, trials));
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
                SubjectInformationContainer[] subjects = listBox_DescriptiveStatistic2_Subjects.SelectedItems.Cast<SubjectInformationContainer>().ToArray();
                string[] turns = listBox_DescriptiveStatistic2_Turns.SelectedItems.Cast<string>().ToArray();
                string[] trials = listBox_DescriptiveStatistic2_Trials.Items.Cast<string>().ToArray();

                foreach (string group in groups)
                {
                    foreach (SubjectInformationContainer subject in subjects)
                    {
                        foreach (string turn in turns)
                        {
                            listBox_DescriptiveStatistic2_SelectedTrials.Items.Add(new StatisticPlotContainer(study, group, szenario, subject, turn, trials));
                        }
                    }
                }
            }
        }

        private void button_DescriptiveStatistic2_ClearSelected_Click(object sender, EventArgs e)
        {
            while (listBox_DescriptiveStatistic2_SelectedTrials.SelectedItems.Count > 0)
            {
                listBox_DescriptiveStatistic2_SelectedTrials.Items.Remove(listBox_DescriptiveStatistic2_SelectedTrials.SelectedItem);
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

            string[] studyNames = mySQLWrapper.getStudyNames();
            if (studyNames != null)
            {
                comboBox_DescriptiveStatistic2_Study.Items.AddRange(studyNames);
                comboBox_DescriptiveStatistic2_Study.SelectedIndex = 0;
            }
        }

        private void button_DescriptiveStatistic2_CalculateMeanValues_Click(object sender, EventArgs e)
        {
            writeProgressInfo("Getting data...");
            saveFileDialog = new SaveFileDialog();
            saveFileDialog.Reset();
            saveFileDialog.Title = "Save mean data file";
            saveFileDialog.AddExtension = true;
            saveFileDialog.DefaultExt = ".csv";
            saveFileDialog.Filter = "DataFiles (*.csv)|.csv";
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
                                    + comboBox_DescriptiveStatistic2_DataTypeSelect.SelectedItem.ToString()
                                    + "-data";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (listBox_DescriptiveStatistic2_SelectedTrials.Items.Count > 0)
                {
                    int trialListCounter = 0;
                    List<int> trialList = listBox_DescriptiveStatistic2_SelectedTrials.Items.Cast<StatisticPlotContainer>().ElementAt(0).Trials;
                    double[,] data = new double[trialList.Count, listBox_DescriptiveStatistic2_SelectedTrials.Items.Count];

                    for (int meanCounter = 0; meanCounter < listBox_DescriptiveStatistic2_SelectedTrials.Items.Count; meanCounter++)
                    {
                        setProgressBarValue((100.0 / listBox_DescriptiveStatistic2_SelectedTrials.Items.Count) * meanCounter);
                        StatisticPlotContainer temp = listBox_DescriptiveStatistic2_SelectedTrials.Items.Cast<StatisticPlotContainer>().ElementAt(meanCounter);

                        DateTime turn = mySQLWrapper.getTurnDateTime(temp.Study, temp.Group, temp.Szenario, temp.Subject.id, Convert.ToInt32(temp.Turn.Substring("Turn".Length)));
                        DataSet statisticDataSet = mySQLWrapper.getStatisticDataSet(temp.Study, temp.Group, temp.Szenario, temp.Subject.id, turn);

                        trialListCounter = 0;
                        foreach (DataRow row in statisticDataSet.Tables[0].Rows)
                        {
                            int szenarioTrialNumber = Convert.ToInt32(row["szenario_trial_number"]);
                            if (trialList.Contains(szenarioTrialNumber))
                            {
                                switch (comboBox_DescriptiveStatistic2_DataTypeSelect.SelectedItem.ToString())
                                {
                                    case "Vector correlation":
                                        data[trialListCounter, meanCounter] = Convert.ToDouble(row["velocity_vector_correlation"]);
                                        break;

                                    case "Perpendicular distance 300ms - Abs":
                                        data[trialListCounter, meanCounter] = Convert.ToDouble(row["perpendicular_displacement_300ms_abs"]);
                                        break;

                                    case "Mean perpendicular distance - Abs":
                                        data[trialListCounter, meanCounter] = Convert.ToDouble(row["mean_perpendicular_displacement_abs"]);
                                        break;

                                    case "Max perpendicular distance - Abs":
                                        data[trialListCounter, meanCounter] = Convert.ToDouble(row["maximal_perpendicular_displacement_abs"]);
                                        break;

                                    case "Perpendicular distance 300ms - Sign":
                                        data[trialListCounter, meanCounter] = Convert.ToDouble(row["perpendicular_displacement_300ms_sign"]);
                                        break;

                                    case "Max perpendicular distance - Sign":
                                        data[trialListCounter, meanCounter] = Convert.ToDouble(row["maximal_perpendicular_displacement_sign"]);
                                        break;

                                    case "Trajectory length abs":
                                        data[trialListCounter, meanCounter] = Convert.ToDouble(row["trajectory_length_abs"]);
                                        break;

                                    case "Trajectory length ratio":
                                        data[trialListCounter, meanCounter] = Convert.ToDouble(row["trajectory_length_ratio_baseline"]);
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

                        myMatlabWrapper.setWorkspaceData("data", data);

                        myMatlabWrapper.execute("dataMean = mean(data);");
                        myMatlabWrapper.execute("dataStd = std(data);");

                        dataMean = myMatlabWrapper.getWorkspaceData("dataMean");
                        dataStd = myMatlabWrapper.getWorkspaceData("dataStd");
                    }
                    else
                    {
                        dataMean = new double[,] { { data[0, 0], 0 } };
                        dataStd = new double[,] { { 0, 0 } };
                    }

                    List<string> cache = new List<string>();
                    FileStream meanDataFileStream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
                    StreamWriter meanDataFileWriter = new StreamWriter(meanDataFileStream);
                    cache.Add("Study;Group;Szenario;Subject;Turn;Trials;Mean;Std");

                    for (int i = 0; i < listBox_DescriptiveStatistic2_SelectedTrials.Items.Count; i++)
                    {
                        StatisticPlotContainer tempStatisticPlotContainer = listBox_DescriptiveStatistic2_SelectedTrials.Items.Cast<StatisticPlotContainer>().ElementAt(i);
                        string tempLine = tempStatisticPlotContainer.Study
                                        + ";"
                                        + tempStatisticPlotContainer.Group
                                        + ";"
                                        + tempStatisticPlotContainer.Szenario
                                        + ";"
                                        + tempStatisticPlotContainer.Subject
                                        + ";"
                                        + tempStatisticPlotContainer.Turn
                                        + ";"
                                        + tempStatisticPlotContainer.getTrialsString()
                                        + ";"
                                        + DoubleConverter.ToExactString(dataMean[0, i])
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

                    myMatlabWrapper.clearWorkspace();
                }
            }
            writeProgressInfo("Ready");
            setProgressBarValue(0);
        }

        private void button_DescriptiveStatistic1_ExportData_Click(object sender, EventArgs e)
        {
            writeProgressInfo("Getting data...");
            saveFileDialog = new SaveFileDialog();
            saveFileDialog.Reset();
            saveFileDialog.Title = "Save mean data file";
            saveFileDialog.AddExtension = true;
            saveFileDialog.DefaultExt = ".csv";
            saveFileDialog.Filter = "DataFiles (*.csv)|.csv";
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
                                    + comboBox_DescriptiveStatistic1_DataTypeSelect.SelectedItem.ToString()
                                    + "-data";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {

                if (listBox_DescriptiveStatistic1_SelectedTrials.Items.Count > 0)
                {
                    bool isValid = true;
                    List<int> trialList = listBox_DescriptiveStatistic1_SelectedTrials.Items.Cast<StatisticPlotContainer>().ElementAt(0).Trials;
                    foreach (StatisticPlotContainer temp in listBox_DescriptiveStatistic1_SelectedTrials.Items)
                    {
                        if (!trialList.SequenceEqual(temp.Trials))
                        {
                            writeToLogBox("Trial selections are not equal!");
                            isValid = false;
                            break;
                        }
                    }

                    if (isValid)
                    {
                        int meanCounter;
                        int trialListCounter = 0;
                        double[,] data = new double[trialList.Count, listBox_DescriptiveStatistic1_SelectedTrials.Items.Count];

                        for (meanCounter = 0; meanCounter < listBox_DescriptiveStatistic1_SelectedTrials.Items.Count; meanCounter++)
                        {
                            setProgressBarValue((100.0 / listBox_DescriptiveStatistic1_SelectedTrials.Items.Count) * meanCounter);
                            StatisticPlotContainer temp = listBox_DescriptiveStatistic1_SelectedTrials.Items.Cast<StatisticPlotContainer>().ElementAt(meanCounter);

                            DateTime turn = mySQLWrapper.getTurnDateTime(temp.Study, temp.Group, temp.Szenario, temp.Subject.id, Convert.ToInt32(temp.Turn.Substring("Turn".Length)));
                            DataSet statisticDataSet = mySQLWrapper.getStatisticDataSet(temp.Study, temp.Group, temp.Szenario, temp.Subject.id, turn);

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
                                            data[trialListCounter, meanCounter] = Convert.ToDouble(row["velocity_vector_correlation"]);
                                            break;

                                        case "Perpendicular distance 300ms - Abs":
                                            data[trialListCounter, meanCounter] = Convert.ToDouble(row["perpendicular_displacement_300ms_abs"]);
                                            break;

                                        case "Mean perpendicular distance - Abs":
                                            data[trialListCounter, meanCounter] = Convert.ToDouble(row["mean_perpendicular_displacement_abs"]);
                                            break;

                                        case "Max perpendicular distance - Abs":
                                            data[trialListCounter, meanCounter] = Convert.ToDouble(row["maximal_perpendicular_displacement_abs"]);
                                            break;

                                        case "Perpendicular distance 300ms - Sign":
                                            data[trialListCounter, meanCounter] = Convert.ToDouble(row["perpendicular_displacement_300ms_sign"]);
                                            break;

                                        case "Max perpendicular distance - Sign":
                                            data[trialListCounter, meanCounter] = Convert.ToDouble(row["maximal_perpendicular_displacement_sign"]);
                                            break;

                                        case "Trajectory length abs":
                                            data[trialListCounter, meanCounter] = Convert.ToDouble(row["trajectory_length_abs"]);
                                            break;

                                        case "Trajectory length ratio":
                                            data[trialListCounter, meanCounter] = Convert.ToDouble(row["trajectory_length_ratio_baseline"]);
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

                        myMatlabWrapper.setWorkspaceData("data", data);

                        double[,] dataMean;
                        double[,] dataStd;

                        if (meanCounter > 1)
                        {
                            myMatlabWrapper.execute("dataMean = mean(transpose(data));");
                            myMatlabWrapper.execute("dataStd = std(transpose(data));");
                            dataMean = myMatlabWrapper.getWorkspaceData("dataMean");
                            dataStd = myMatlabWrapper.getWorkspaceData("dataStd");
                        }
                        else
                        {
                            dataMean = new double[,] { { data[0, 0], 0 } };
                            dataStd = new double[,] { { 0, 0 } }; ;
                        }

                        List<string> cache = new List<string>();
                        FileStream meanDataFileStream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
                        StreamWriter meanDataFileWriter = new StreamWriter(meanDataFileStream);

                        string personNames = "";
                        for (int i = 0; i < listBox_DescriptiveStatistic1_SelectedTrials.Items.Count; i++)
                        {
                            StatisticPlotContainer tempStatisticPlotContainer = listBox_DescriptiveStatistic1_SelectedTrials.Items.Cast<StatisticPlotContainer>().ElementAt(i);
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

                            string tempLine = trialList.ElementAt(i).ToString() + ";";

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

                        myMatlabWrapper.clearWorkspace();
                    }
                }
            }
            writeProgressInfo("Ready");
            setProgressBarValue(0);
        }

        private void checkBox_PauseThread_CheckedChanged(object sender, EventArgs e)
        {
            ThreadManager.pause = checkBox_PauseThread.Checked;
        }

        private void button_DataManipulation_UpdateGroupID_Click(object sender, EventArgs e)
        {
            mySQLWrapper.changeGroupID(
                                            Convert.ToInt32(textBox_DataManipulation_OldGroupID.Text),
                                            Convert.ToInt32(textBox_DataManipulation_NewGroupID.Text)
                                            );
        }

        private void button_DataManipulation_UpdateSubjectID_Click(object sender, EventArgs e)
        {
            mySQLWrapper.changeSubjectID(
                                            Convert.ToInt32(textBox_DataManipulation_OldSubjectID.Text),
                                            Convert.ToInt32(textBox_DataManipulation_NewSubjectID.Text)
                                            );
        }

        private void button_DataManipulation_UpdateGroupName_Click(object sender, EventArgs e)
        {
            mySQLWrapper.changeGroupName(
                                            Convert.ToInt32(textBox_DataManipulation_GroupID.Text),
                                            textBox_DataManipulation_NewGroupName.Text
                                            );
        }

        private void button_DataManipulation_UpdateSubjectName_Click(object sender, EventArgs e)
        {
            mySQLWrapper.changeSubjectName(
                                            Convert.ToInt32(textBox_DataManipulation_SubjectID.Text),
                                            textBox_DataManipulation_NewSubjectName.Text
                                            );
        }

        private void button_ImportMeasureFiles_Click(object sender, EventArgs e)
        {
            Thread newThread = null;
            newThread = new Thread(delegate()
            {
                // Debug Christian, can be deleted!
                int debugCounterZero = 0;
                int debugCounterTwo = 0;
                int debugCounterThree = 0;
                //
                while (ThreadManager.getIndex(newThread) != 0)
                {
                    Thread.Sleep(100);
                }
                enableTabPages(false);
                setProgressBarValue(0);

                for (int files = 0; files < listBox_Import_SelectedMeasureFiles.Items.Count; files++)
                {
                    while (ThreadManager.pause)
                    {
                        Thread.Sleep(100);
                    }

                    setProgressBarValue((100.0 / listBox_Import_SelectedMeasureFiles.Items.Count) * files);

                    string filename = listBox_Import_SelectedMeasureFiles.Items[files].ToString();

                    string tempFileHash = MD5.computeHash(filename);

                    if (!mySQLWrapper.checkIfMeasureFileHashExists(tempFileHash))
                    {
                        DataContainer myDataContainter = new DataContainer();
                        MeasureFileParser myParser = new MeasureFileParser(myDataContainter, this);

                        if (myParser.parseFile(filename))
                        {
                            writeProgressInfo("Running multicore-calculation preparation...");
                            #region MultiCore preparation

                            List<Thread> multiCoreThreads = new List<Thread>();

                            int[] szenarioTrialNumbers = myDataContainter.measureDataRaw.Select(t => t.szenario_trial_number).OrderBy(t => t).Distinct().ToArray();
                            int[] targetNumbers = myDataContainter.measureDataRaw.Select(t => t.target_number).OrderBy(t => t).Distinct().ToArray();

                            List<int>[] trialCoreDistribution = new List<int>[Environment.ProcessorCount];
                            List<int>[] targetCoreDistribution = new List<int>[Environment.ProcessorCount];

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

                            writeProgressInfo("Running duplicate entry detection...");
                            #region Duplicate entry detection

                            for (int core = 0; core < Environment.ProcessorCount; core++)
                            {
                                List<int> _threadTrials = trialCoreDistribution[core];

                                multiCoreThreads.Add(new Thread(delegate()
                                {
                                    List<int> threadTrials = new List<int>(_threadTrials);

                                    for (int i = 0; i < threadTrials.Count(); i++)
                                    {
                                        List<MeasureDataContainer> tempRawData;
                                        lock (myDataContainter)
                                        {
                                            tempRawData = new List<MeasureDataContainer>(myDataContainter.measureDataRaw.Where(t => t.szenario_trial_number == threadTrials.ElementAt(i)).OrderBy(t => t.time_stamp));
                                        }

                                        int entryCount = tempRawData.Select(t => t.time_stamp.Ticks).Count();
                                        int entryUniqueCount = tempRawData.Select(t => t.time_stamp.Ticks).Distinct().Count();

                                        if (entryCount != entryUniqueCount)
                                        {
                                            lock (myDataContainter)
                                            {
                                                List<MeasureDataContainer> tempList = myDataContainter.measureDataRaw.Where(t => t.szenario_trial_number == threadTrials.ElementAt(i)).ToList();
                                                for (int j = 0; j < tempList.Count; j++)
                                                {
                                                    tempList.ElementAt(j).contains_duplicates = true;
                                                }
                                            }
                                        }

                                        bool errorDetected;
                                        do
                                        {
                                            errorDetected = false;
                                            List<double> diffXYZ = new List<double>(); ;
                                            for (int j = 0; j < (tempRawData.Count - 1); j++)
                                            {
                                                diffXYZ.Add(Math.Sqrt(Math.Pow(tempRawData[j].position_cartesian_x - tempRawData[j + 1].position_cartesian_x, 2) +
                                                                           Math.Pow(tempRawData[j].position_cartesian_y - tempRawData[j + 1].position_cartesian_y, 2) +
                                                                           Math.Pow(tempRawData[j].position_cartesian_z - tempRawData[j + 1].position_cartesian_z, 2)) /
                                                                           tempRawData[j + 1].time_stamp.Subtract(tempRawData[j].time_stamp).TotalSeconds);
                                            }
                                            
                                            int maxIndex = diffXYZ.IndexOf(diffXYZ.Max());

                                            if (Math.Abs(diffXYZ.ElementAt(maxIndex) - diffXYZ.ElementAt(maxIndex - 1)) > 3)
                                            {                                                
                                                MeasureDataContainer errorEntry = tempRawData.ElementAt(maxIndex + 1);
                                                writeToLogBox("Fixed error at time-stamp \"" + errorEntry.time_stamp.ToString("hh:mm:ss.fffffff") + "\" in file \"" + filename + "\"");
                                                tempRawData.RemoveAt(maxIndex + 1);
                                                errorDetected = true;
                                            }
                                        } while (errorDetected);
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

                            writeProgressInfo("Filtering data...");
                            #region Butterworth filter

                            int samplesPerSecond = Convert.ToInt32(textBox_Import_SamplesPerSec.Text);
                            int filterOrder = Convert.ToInt32(textBox_Import_FilterOrder.Text);
                            int cutoffFreq = Convert.ToInt32(textBox_Import_CutoffFreq.Text);
                            int velocityCuttingThreshold = Convert.ToInt32(textBox_Import_PercentPeakVelocity.Text);

                            myMatlabWrapper.setWorkspaceData("filterOrder",Convert.ToDouble(filterOrder));
                            myMatlabWrapper.setWorkspaceData("cutoffFreq", Convert.ToDouble(cutoffFreq));
                            myMatlabWrapper.setWorkspaceData("samplesPerSecond", Convert.ToDouble(samplesPerSecond));
                            myMatlabWrapper.execute("[b,a] = butter(filterOrder,(cutoffFreq/(samplesPerSecond/2)));");

                            for (int core = 0; core < Environment.ProcessorCount; core++)
                            {
                                List<int> _threadTrials = trialCoreDistribution[core];

                                multiCoreThreads.Add(new Thread(delegate()
                                {
                                    List<int> threadTrials = new List<int>(_threadTrials);

                                    for (int i = 0; i < threadTrials.Count(); i++)
                                    {
                                        List<MeasureDataContainer> tempRawDataEnum;
                                        lock (myDataContainter)
                                        {
                                            tempRawDataEnum = new List<MeasureDataContainer>(myDataContainter.measureDataRaw.Where(t => t.contains_duplicates == false).Where(t => t.szenario_trial_number == threadTrials.ElementAt(i)).OrderBy(t => t.time_stamp));
                                        }
                                        if (tempRawDataEnum.Count > 0)
                                        {
                                            DateTime[] tempTimeStamp = tempRawDataEnum.Select(t => t.time_stamp).ToArray();
                                            int tempTargetNumber = tempRawDataEnum.Select(t => t.target_number).ElementAt(0);
                                            int tempTargetTrialNumber = tempRawDataEnum.Select(t => t.target_trial_number).ElementAt(0);
                                            int tempSzenarioTrialNumber = tempRawDataEnum.Select(t => t.szenario_trial_number).ElementAt(0);
                                            bool tempIsCatchTrial = tempRawDataEnum.Select(t => t.is_catch_trial).ElementAt(0);
                                            int[] tempPositionStatus = tempRawDataEnum.Select(t => t.position_status).ToArray();

                                            myMatlabWrapper.setWorkspaceData("force_actual_x" + threadTrials.ElementAt(i), tempRawDataEnum.Select(t => t.force_actual_x).ToArray());
                                            myMatlabWrapper.setWorkspaceData("force_actual_y" + threadTrials.ElementAt(i), tempRawDataEnum.Select(t => t.force_actual_y).ToArray());
                                            myMatlabWrapper.setWorkspaceData("force_actual_z" + threadTrials.ElementAt(i), tempRawDataEnum.Select(t => t.force_actual_z).ToArray());

                                            myMatlabWrapper.setWorkspaceData("force_nominal_x" + threadTrials.ElementAt(i), tempRawDataEnum.Select(t => t.force_nominal_x).ToArray());
                                            myMatlabWrapper.setWorkspaceData("force_nominal_y" + threadTrials.ElementAt(i), tempRawDataEnum.Select(t => t.force_nominal_y).ToArray());
                                            myMatlabWrapper.setWorkspaceData("force_nominal_z" + threadTrials.ElementAt(i), tempRawDataEnum.Select(t => t.force_nominal_z).ToArray());

                                            myMatlabWrapper.setWorkspaceData("force_moment_x" + threadTrials.ElementAt(i), tempRawDataEnum.Select(t => t.force_moment_x).ToArray());
                                            myMatlabWrapper.setWorkspaceData("force_moment_y" + threadTrials.ElementAt(i), tempRawDataEnum.Select(t => t.force_moment_y).ToArray());
                                            myMatlabWrapper.setWorkspaceData("force_moment_z" + threadTrials.ElementAt(i), tempRawDataEnum.Select(t => t.force_moment_z).ToArray());

                                            myMatlabWrapper.setWorkspaceData("position_cartesian_x" + threadTrials.ElementAt(i), tempRawDataEnum.Select(t => t.position_cartesian_x).ToArray());
                                            myMatlabWrapper.setWorkspaceData("position_cartesian_y" + threadTrials.ElementAt(i), tempRawDataEnum.Select(t => t.position_cartesian_y).ToArray());
                                            myMatlabWrapper.setWorkspaceData("position_cartesian_z" + threadTrials.ElementAt(i), tempRawDataEnum.Select(t => t.position_cartesian_z).ToArray());

                                            myMatlabWrapper.execute("force_actual_x" + threadTrials.ElementAt(i) + " = filtfilt(b, a, force_actual_x" + threadTrials.ElementAt(i) + ");");
                                            myMatlabWrapper.execute("force_actual_y" + threadTrials.ElementAt(i) + " = filtfilt(b, a, force_actual_y" + threadTrials.ElementAt(i) + ");");
                                            myMatlabWrapper.execute("force_actual_z" + threadTrials.ElementAt(i) + " = filtfilt(b, a, force_actual_z" + threadTrials.ElementAt(i) + ");");

                                            myMatlabWrapper.execute("force_nominal_x" + threadTrials.ElementAt(i) + " = filtfilt(b, a,force_nominal_x" + threadTrials.ElementAt(i) + ");");
                                            myMatlabWrapper.execute("force_nominal_y" + threadTrials.ElementAt(i) + " = filtfilt(b, a,force_nominal_y" + threadTrials.ElementAt(i) + ");");
                                            myMatlabWrapper.execute("force_nominal_z" + threadTrials.ElementAt(i) + " = filtfilt(b, a,force_nominal_z" + threadTrials.ElementAt(i) + ");");

                                            myMatlabWrapper.execute("force_moment_x" + threadTrials.ElementAt(i) + " = filtfilt(b, a, force_moment_x" + threadTrials.ElementAt(i) + ");");
                                            myMatlabWrapper.execute("force_moment_y" + threadTrials.ElementAt(i) + " = filtfilt(b, a, force_moment_y" + threadTrials.ElementAt(i) + ");");
                                            myMatlabWrapper.execute("force_moment_z" + threadTrials.ElementAt(i) + " = filtfilt(b, a, force_moment_z" + threadTrials.ElementAt(i) + ");");

                                            myMatlabWrapper.execute("position_cartesian_x" + threadTrials.ElementAt(i) + " = filtfilt(b, a, position_cartesian_x" + threadTrials.ElementAt(i) + ");");
                                            myMatlabWrapper.execute("position_cartesian_y" + threadTrials.ElementAt(i) + " = filtfilt(b, a, position_cartesian_y" + threadTrials.ElementAt(i) + ");");
                                            myMatlabWrapper.execute("position_cartesian_z" + threadTrials.ElementAt(i) + " = filtfilt(b, a, position_cartesian_z" + threadTrials.ElementAt(i) + ");");


                                            double[,] force_actual_x = myMatlabWrapper.getWorkspaceData("force_actual_x" + threadTrials.ElementAt(i));
                                            double[,] force_actual_y = myMatlabWrapper.getWorkspaceData("force_actual_y" + threadTrials.ElementAt(i));
                                            double[,] force_actual_z = myMatlabWrapper.getWorkspaceData("force_actual_z" + threadTrials.ElementAt(i));

                                            double[,] force_nominal_x = myMatlabWrapper.getWorkspaceData("force_nominal_x" + threadTrials.ElementAt(i));
                                            double[,] force_nominal_y = myMatlabWrapper.getWorkspaceData("force_nominal_y" + threadTrials.ElementAt(i));
                                            double[,] force_nominal_z = myMatlabWrapper.getWorkspaceData("force_nominal_z" + threadTrials.ElementAt(i));

                                            double[,] force_moment_x = myMatlabWrapper.getWorkspaceData("force_moment_x" + threadTrials.ElementAt(i));
                                            double[,] force_moment_y = myMatlabWrapper.getWorkspaceData("force_moment_y" + threadTrials.ElementAt(i));
                                            double[,] force_moment_z = myMatlabWrapper.getWorkspaceData("force_moment_z" + threadTrials.ElementAt(i));

                                            double[,] position_cartesian_x = myMatlabWrapper.getWorkspaceData("position_cartesian_x" + threadTrials.ElementAt(i));
                                            double[,] position_cartesian_y = myMatlabWrapper.getWorkspaceData("position_cartesian_y" + threadTrials.ElementAt(i));
                                            double[,] position_cartesian_z = myMatlabWrapper.getWorkspaceData("position_cartesian_z" + threadTrials.ElementAt(i));


                                            for (int j = 0; j < force_actual_x.Length; j++)
                                            {
                                                lock (myDataContainter)
                                                {
                                                    myDataContainter.measureDataFiltered.Add(new MeasureDataContainer(
                                                                                tempTimeStamp[j],
                                                                                force_actual_x[0, j],
                                                                                force_actual_y[0, j],
                                                                                force_actual_z[0, j],
                                                                                force_nominal_x[0, j],
                                                                                force_nominal_y[0, j],
                                                                                force_nominal_z[0, j],
                                                                                force_moment_x[0, j],
                                                                                force_moment_y[0, j],
                                                                                force_moment_z[0, j],
                                                                                position_cartesian_x[0, j],
                                                                                position_cartesian_y[0, j],
                                                                                position_cartesian_z[0, j],
                                                                                tempTargetNumber,
                                                                                tempTargetTrialNumber,
                                                                                tempSzenarioTrialNumber,
                                                                                tempIsCatchTrial,
                                                                                tempPositionStatus[j]
                                                                                ));
                                                }
                                            }

                                            myMatlabWrapper.clearWorkspaceData("force_actual_x" + threadTrials.ElementAt(i));
                                            myMatlabWrapper.clearWorkspaceData("force_actual_y" + threadTrials.ElementAt(i));
                                            myMatlabWrapper.clearWorkspaceData("force_actual_z" + threadTrials.ElementAt(i));

                                            myMatlabWrapper.clearWorkspaceData("force_nominal_x" + threadTrials.ElementAt(i));
                                            myMatlabWrapper.clearWorkspaceData("force_nominal_y" + threadTrials.ElementAt(i));
                                            myMatlabWrapper.clearWorkspaceData("force_nominal_z" + threadTrials.ElementAt(i));

                                            myMatlabWrapper.clearWorkspaceData("force_moment_x" + threadTrials.ElementAt(i));
                                            myMatlabWrapper.clearWorkspaceData("force_moment_y" + threadTrials.ElementAt(i));
                                            myMatlabWrapper.clearWorkspaceData("force_moment_z" + threadTrials.ElementAt(i));

                                            myMatlabWrapper.clearWorkspaceData("position_cartesian_x" + threadTrials.ElementAt(i));
                                            myMatlabWrapper.clearWorkspaceData("position_cartesian_y" + threadTrials.ElementAt(i));
                                            myMatlabWrapper.clearWorkspaceData("position_cartesian_z" + threadTrials.ElementAt(i));
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
                            myMatlabWrapper.clearWorkspace();
                            multiCoreThreads.Clear();

                            #endregion Butterworth Filter

                            writeProgressInfo("Calculating velocity...");
                            #region Velocity calcultion

                            for (int core = 0; core < Environment.ProcessorCount; core++)
                            {
                                List<int> _threadTrials = trialCoreDistribution[core];

                                multiCoreThreads.Add(new Thread(delegate()
                                {
                                    List<int> threadTrials = new List<int>(_threadTrials);

                                    for (int i = 0; i < threadTrials.Count(); i++)
                                    {
                                        List<MeasureDataContainer> tempFilteredDataEnum;
                                        lock (myDataContainter)
                                        {
                                            tempFilteredDataEnum = new List<MeasureDataContainer>(myDataContainter.measureDataFiltered.Where(t => t.szenario_trial_number == threadTrials.ElementAt(i)).OrderBy(t => t.time_stamp));
                                        }
                                        if (tempFilteredDataEnum.Count > 0)
                                        {                                            
                                            myMatlabWrapper.setWorkspaceData("time_stamp" + threadTrials.ElementAt(i), tempFilteredDataEnum.Select(t => Convert.ToDouble(t.time_stamp.Ticks)).ToArray());
                                            myMatlabWrapper.setWorkspaceData("position_cartesian_x" + threadTrials.ElementAt(i), tempFilteredDataEnum.Select(t => t.position_cartesian_x).ToArray());
                                            myMatlabWrapper.setWorkspaceData("position_cartesian_y" + threadTrials.ElementAt(i), tempFilteredDataEnum.Select(t => t.position_cartesian_y).ToArray());
                                            myMatlabWrapper.setWorkspaceData("position_cartesian_z" + threadTrials.ElementAt(i), tempFilteredDataEnum.Select(t => t.position_cartesian_z).ToArray());
                                            myMatlabWrapper.setWorkspaceData("sampleRate", (1.0 / Convert.ToDouble(textBox_Import_SamplesPerSec.Text)));

                                            myMatlabWrapper.execute("time_stamp" + threadTrials.ElementAt(i) + " = time_stamp" + threadTrials.ElementAt(i) + "(1:end-1) +  (diff(time_stamp" + threadTrials.ElementAt(i) + ") ./ 2);");
                                            myMatlabWrapper.execute("velocity_x" + threadTrials.ElementAt(i) + " = diff(position_cartesian_x" + threadTrials.ElementAt(i) + ") ./ sampleRate;");
                                            myMatlabWrapper.execute("velocity_y" + threadTrials.ElementAt(i) + " = diff(position_cartesian_y" + threadTrials.ElementAt(i) + ") ./ sampleRate;");
                                            myMatlabWrapper.execute("velocity_z" + threadTrials.ElementAt(i) + " = diff(position_cartesian_z" + threadTrials.ElementAt(i) + ") ./ sampleRate;");

                                            double[,] time_stamp = myMatlabWrapper.getWorkspaceData("time_stamp" + threadTrials.ElementAt(i));
                                            double[,] velocity_x = myMatlabWrapper.getWorkspaceData("velocity_x" + threadTrials.ElementAt(i));
                                            double[,] velocity_y = myMatlabWrapper.getWorkspaceData("velocity_y" + threadTrials.ElementAt(i));
                                            double[,] velocity_z = myMatlabWrapper.getWorkspaceData("velocity_z" + threadTrials.ElementAt(i));                                            

                                            for (int j = 0; j < velocity_x.Length; j++)
                                            {
                                                lock (myDataContainter)
                                                {
                                                    myDataContainter.velocityDataFiltered.Add(new VelocityDataContainer(
                                                                                    new DateTime(Convert.ToInt64(time_stamp[0, j])),
                                                                                    velocity_x[0, j],
                                                                                    velocity_y[0, j],
                                                                                    velocity_z[0, j],
                                                                                    threadTrials.ElementAt(i),
                                                                                    tempFilteredDataEnum.ElementAt(0).target_number,
                                                                                    tempFilteredDataEnum.ElementAt(j).position_status
                                                                                    ));
                                                }
                                            }

                                            myMatlabWrapper.clearWorkspaceData("time_stamp" + threadTrials.ElementAt(i));
                                            myMatlabWrapper.clearWorkspaceData("velocity_x" + threadTrials.ElementAt(i));
                                            myMatlabWrapper.clearWorkspaceData("velocity_y" + threadTrials.ElementAt(i));
                                            myMatlabWrapper.clearWorkspaceData("velocity_z" + threadTrials.ElementAt(i));
                                            myMatlabWrapper.clearWorkspaceData("position_cartesian_x" + threadTrials.ElementAt(i));
                                            myMatlabWrapper.clearWorkspaceData("position_cartesian_y" + threadTrials.ElementAt(i));
                                            myMatlabWrapper.clearWorkspaceData("position_cartesian_z" + threadTrials.ElementAt(i));
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
                            myMatlabWrapper.clearWorkspace();
                            multiCoreThreads.Clear();

                            #endregion

                            writeProgressInfo("Normalizing data...");
                            #region Time normalization

                            for (int core = 0; core < Environment.ProcessorCount; core++)
                            {
                                List<int> _threadTrials = trialCoreDistribution[core];

                                multiCoreThreads.Add(new Thread(delegate()
                                {
                                    List<int> threadTrials = new List<int>(_threadTrials);
                                    myMatlabWrapper.setWorkspaceData("newSampleCount", Convert.ToDouble(textBox_Import_NewSampleCount.Text));

                                    for (int i = 0; i < threadTrials.Count(); i++)
                                    {
                                        List<MeasureDataContainer> tempFilteredDataEnum;
                                        List<VelocityDataContainer> tempVelocityDataEnum;

                                        lock (myDataContainter)
                                        {                                            
                                            tempFilteredDataEnum = new List<MeasureDataContainer>(myDataContainter.measureDataFiltered.Where(t => t.szenario_trial_number == threadTrials.ElementAt(i)).Where(t => t.position_status == 1).OrderBy(t => t.time_stamp));
                                            if (myDataContainter.measureDataFiltered.Where(t => t.szenario_trial_number == threadTrials.ElementAt(i)).Where(t => t.position_status == 0).Count() > 0)
                                            {
                                                tempFilteredDataEnum.Insert(0, myDataContainter.measureDataFiltered.Where(t => t.szenario_trial_number == threadTrials.ElementAt(i)).Where(t => t.position_status == 0).OrderBy(t => t.time_stamp).Last());
                                            }

                                            tempVelocityDataEnum = new List<VelocityDataContainer>(myDataContainter.velocityDataFiltered.Where(t => t.szenario_trial_number == threadTrials.ElementAt(i)).OrderBy(t => t.time_stamp));
                                        }

                                        if ((tempFilteredDataEnum.Count > 0) && (tempVelocityDataEnum.Count > 0))
                                        {
                                            int trimThreshold = Convert.ToInt32(textBox_Import_PercentPeakVelocity.Text);
                                            List<VelocityDataContainer> tempVelocityDataEnumCropped = new List<VelocityDataContainer>();

                                            if (trimThreshold > 0)
                                            {
                                                double velocityCropThreshold = tempVelocityDataEnum.Max(t => Math.Sqrt(Math.Pow(t.velocity_x, 2) + Math.Pow(t.velocity_z, 2))) / 100.0 * trimThreshold;

                                                if (tempVelocityDataEnum.Where(t => t.position_status == 0).Count(t => Math.Sqrt(Math.Pow(t.velocity_x, 2) + Math.Pow(t.velocity_z, 2)) > velocityCropThreshold) > 0)
                                                {
                                                    DateTime startTime = tempVelocityDataEnum.Where(t => Math.Sqrt(Math.Pow(t.velocity_x, 2) + Math.Pow(t.velocity_z, 2)) > velocityCropThreshold).OrderBy(t => t.time_stamp).First().time_stamp;
                                                    tempVelocityDataEnumCropped.AddRange(tempVelocityDataEnum.Where(t => t.position_status == 0).Where(t => t.time_stamp >= startTime));
                                                }
                                                // Debug Christian, can be deleted!
                                                else
                                                {                                                    
                                                    debugCounterZero++;
                                                }
                                                // ---------------------------------
                                                tempVelocityDataEnumCropped.AddRange(tempVelocityDataEnum.Where(t => t.position_status == 1));
                                                
                                                if (tempVelocityDataEnum.Where(t => t.position_status == 2).Count(t => Math.Sqrt(Math.Pow(t.velocity_x, 2) + Math.Pow(t.velocity_z, 2)) < velocityCropThreshold) > 0)
                                                {
                                                    DateTime stopTime = tempVelocityDataEnum.Where(t => t.position_status == 2).Where(t => Math.Sqrt(Math.Pow(t.velocity_x, 2) + Math.Pow(t.velocity_z, 2)) < velocityCropThreshold).OrderBy(t => t.time_stamp).First().time_stamp;
                                                    tempVelocityDataEnumCropped.AddRange(tempVelocityDataEnum.Where(t => t.position_status > 1).Where(t => t.time_stamp <= stopTime));                                                    
                                                }
                                                else 
                                                {
                                                    if (tempVelocityDataEnum.Exists(t => t.position_status == 3))
                                                    {
                                                        DateTime stopTime = tempVelocityDataEnum.Where(t => t.position_status == 3).OrderBy(t => t.time_stamp).Last().time_stamp;
                                                        tempVelocityDataEnumCropped.AddRange(tempVelocityDataEnum.Where(t => t.time_stamp <= stopTime));
                                                    }
                                                    // Debug Christian, can be deleted!
                                                    else
                                                    {
                                                        debugCounterThree++;
                                                    }
                                                    debugCounterTwo++;
                                                    // ---------------------------------
                                                }

                                                tempVelocityDataEnum = tempVelocityDataEnumCropped.OrderBy(t => t.time_stamp).ToList();
                                            }
                                            else if (trimThreshold < 0)
                                            {
                                                if(tempVelocityDataEnum.Exists(t => t.position_status == 3))
                                                {
                                                    DateTime stopTime = tempVelocityDataEnum.Where(t => t.position_status == 3).OrderBy(t => t.time_stamp).Last().time_stamp;
                                                    tempVelocityDataEnumCropped = tempVelocityDataEnum.Where(t => t.position_status > 0).Where(t => t.time_stamp <= stopTime).ToList();
                                                }
                                                else
                                                {
                                                    tempVelocityDataEnumCropped = tempVelocityDataEnum.Where(t => t.position_status == 1).ToList();                                                    
                                                }

                                                tempVelocityDataEnum = tempVelocityDataEnumCropped.OrderBy(t => t.time_stamp).ToList();
                                            }
                                            

                                            int tempTargetNumber = tempFilteredDataEnum.Select(t => t.target_number).ElementAt(0);
                                            int tempTargetTrialNumber = tempFilteredDataEnum.Select(t => t.target_trial_number).ElementAt(0);
                                            int tempSzenarioTrialNumber = tempFilteredDataEnum.Select(t => t.szenario_trial_number).ElementAt(0);
                                            bool tempIsCatchTrial = tempFilteredDataEnum.Select(t => t.is_catch_trial).ElementAt(0);
                                            List<string> errorList = new List<string>();

                                            myMatlabWrapper.setWorkspaceData("measure_data_time" + threadTrials.ElementAt(i), tempFilteredDataEnum.OrderBy(t => t.time_stamp).Select(t => Convert.ToDouble(t.time_stamp.Ticks)).ToArray());

                                            myMatlabWrapper.setWorkspaceData("forceActualX" + threadTrials.ElementAt(i), tempFilteredDataEnum.OrderBy(t => t.time_stamp).Select(t => t.force_actual_x).ToArray());
                                            myMatlabWrapper.setWorkspaceData("forceActualY" + threadTrials.ElementAt(i), tempFilteredDataEnum.OrderBy(t => t.time_stamp).Select(t => t.force_actual_y).ToArray());
                                            myMatlabWrapper.setWorkspaceData("forceActualZ" + threadTrials.ElementAt(i), tempFilteredDataEnum.OrderBy(t => t.time_stamp).Select(t => t.force_actual_z).ToArray());

                                            myMatlabWrapper.setWorkspaceData("forceNominalX" + threadTrials.ElementAt(i), tempFilteredDataEnum.OrderBy(t => t.time_stamp).Select(t => t.force_nominal_x).ToArray());
                                            myMatlabWrapper.setWorkspaceData("forceNominalY" + threadTrials.ElementAt(i), tempFilteredDataEnum.OrderBy(t => t.time_stamp).Select(t => t.force_nominal_x).ToArray());
                                            myMatlabWrapper.setWorkspaceData("forceNominalZ" + threadTrials.ElementAt(i), tempFilteredDataEnum.OrderBy(t => t.time_stamp).Select(t => t.force_nominal_x).ToArray());

                                            myMatlabWrapper.setWorkspaceData("forceMomentX" + threadTrials.ElementAt(i), tempFilteredDataEnum.OrderBy(t => t.time_stamp).Select(t => t.force_moment_x).ToArray());
                                            myMatlabWrapper.setWorkspaceData("forceMomentY" + threadTrials.ElementAt(i), tempFilteredDataEnum.OrderBy(t => t.time_stamp).Select(t => t.force_moment_x).ToArray());
                                            myMatlabWrapper.setWorkspaceData("forceMomentZ" + threadTrials.ElementAt(i), tempFilteredDataEnum.OrderBy(t => t.time_stamp).Select(t => t.force_moment_x).ToArray());

                                            myMatlabWrapper.setWorkspaceData("positionCartesianX" + threadTrials.ElementAt(i), tempFilteredDataEnum.OrderBy(t => t.time_stamp).Select(t => t.position_cartesian_x).ToArray());
                                            myMatlabWrapper.setWorkspaceData("positionCartesianY" + threadTrials.ElementAt(i), tempFilteredDataEnum.OrderBy(t => t.time_stamp).Select(t => t.position_cartesian_y).ToArray());
                                            myMatlabWrapper.setWorkspaceData("positionCartesianZ" + threadTrials.ElementAt(i), tempFilteredDataEnum.OrderBy(t => t.time_stamp).Select(t => t.position_cartesian_z).ToArray());

                                            myMatlabWrapper.setWorkspaceData("positionStatus" + threadTrials.ElementAt(i), tempFilteredDataEnum.OrderBy(t => t.time_stamp).Select(t => Convert.ToDouble(t.position_status)).ToArray());

                                            myMatlabWrapper.setWorkspaceData("velocity_data_time" + threadTrials.ElementAt(i), tempVelocityDataEnum.OrderBy(t => t.time_stamp).Select(t => Convert.ToDouble(t.time_stamp.Ticks)).ToArray());

                                            myMatlabWrapper.setWorkspaceData("velocityX" + threadTrials.ElementAt(i), tempVelocityDataEnum.OrderBy(t => t.time_stamp).Select(t => t.velocity_x).ToArray());
                                            myMatlabWrapper.setWorkspaceData("velocityY" + threadTrials.ElementAt(i), tempVelocityDataEnum.OrderBy(t => t.time_stamp).Select(t => t.velocity_y).ToArray());
                                            myMatlabWrapper.setWorkspaceData("velocityZ" + threadTrials.ElementAt(i), tempVelocityDataEnum.OrderBy(t => t.time_stamp).Select(t => t.velocity_z).ToArray());

                                            ///

                                            myMatlabWrapper.execute("[errorvar1_" + threadTrials.ElementAt(i) + ", forceActualX" + threadTrials.ElementAt(i) + ",newMeasureTime" + threadTrials.ElementAt(i) + "] = timeNorm(forceActualX" + threadTrials.ElementAt(i) + ",measure_data_time" + threadTrials.ElementAt(i) + ",newSampleCount);");
                                            myMatlabWrapper.execute("[errorvar2_" + threadTrials.ElementAt(i) + ", forceActualY" + threadTrials.ElementAt(i) + ",newMeasureTime" + threadTrials.ElementAt(i) + "] = timeNorm(forceActualY" + threadTrials.ElementAt(i) + ",measure_data_time" + threadTrials.ElementAt(i) + ",newSampleCount);");
                                            myMatlabWrapper.execute("[errorvar3_" + threadTrials.ElementAt(i) + ", forceActualZ" + threadTrials.ElementAt(i) + ",newMeasureTime" + threadTrials.ElementAt(i) + "] = timeNorm(forceActualZ" + threadTrials.ElementAt(i) + ",measure_data_time" + threadTrials.ElementAt(i) + ",newSampleCount);");
                                                                                                                
                                            myMatlabWrapper.execute("[errorvar4_" + threadTrials.ElementAt(i) + ", forceNominalX" + threadTrials.ElementAt(i) + ",newMeasureTime" + threadTrials.ElementAt(i) + "] = timeNorm(forceNominalX" + threadTrials.ElementAt(i) + ",measure_data_time" + threadTrials.ElementAt(i) + ",newSampleCount);");
                                            myMatlabWrapper.execute("[errorvar5_" + threadTrials.ElementAt(i) + ", forceNominalY" + threadTrials.ElementAt(i) + ",newMeasureTime" + threadTrials.ElementAt(i) + "] = timeNorm(forceNominalY" + threadTrials.ElementAt(i) + ",measure_data_time" + threadTrials.ElementAt(i) + ",newSampleCount);");
                                            myMatlabWrapper.execute("[errorvar6_" + threadTrials.ElementAt(i) + ", forceNominalZ" + threadTrials.ElementAt(i) + ",newMeasureTime" + threadTrials.ElementAt(i) + "] = timeNorm(forceNominalZ" + threadTrials.ElementAt(i) + ",measure_data_time" + threadTrials.ElementAt(i) + ",newSampleCount);");
                                                                                                                 
                                            myMatlabWrapper.execute("[errorvar7_" + threadTrials.ElementAt(i) + ", forceMomentX" + threadTrials.ElementAt(i) + ",newMeasureTime" + threadTrials.ElementAt(i) + "] = timeNorm(forceMomentX" + threadTrials.ElementAt(i) + ",measure_data_time" + threadTrials.ElementAt(i) + ",newSampleCount);");
                                            myMatlabWrapper.execute("[errorvar8_" + threadTrials.ElementAt(i) + ", forceMomentY" + threadTrials.ElementAt(i) + ",newMeasureTime" + threadTrials.ElementAt(i) + "] = timeNorm(forceMomentY" + threadTrials.ElementAt(i) + ",measure_data_time" + threadTrials.ElementAt(i) + ",newSampleCount);");
                                            myMatlabWrapper.execute("[errorvar9_" + threadTrials.ElementAt(i) + ", forceMomentZ" + threadTrials.ElementAt(i) + ",newMeasureTime" + threadTrials.ElementAt(i) + "] = timeNorm(forceMomentZ" + threadTrials.ElementAt(i) + ",measure_data_time" + threadTrials.ElementAt(i) + ",newSampleCount);");

                                            myMatlabWrapper.execute("[errorvar10_" + threadTrials.ElementAt(i) + ", positionCartesianX" + threadTrials.ElementAt(i) + ",newMeasureTime" + threadTrials.ElementAt(i) + "] = timeNorm(positionCartesianX" + threadTrials.ElementAt(i) + ",measure_data_time" + threadTrials.ElementAt(i) + ",newSampleCount);");
                                            myMatlabWrapper.execute("[errorvar11_" + threadTrials.ElementAt(i) + ", positionCartesianY" + threadTrials.ElementAt(i) + ",newMeasureTime" + threadTrials.ElementAt(i) + "] = timeNorm(positionCartesianY" + threadTrials.ElementAt(i) + ",measure_data_time" + threadTrials.ElementAt(i) + ",newSampleCount);");
                                            myMatlabWrapper.execute("[errorvar12_" + threadTrials.ElementAt(i) + ", positionCartesianZ" + threadTrials.ElementAt(i) + ",newMeasureTime" + threadTrials.ElementAt(i) + "] = timeNorm(positionCartesianZ" + threadTrials.ElementAt(i) + ",measure_data_time" + threadTrials.ElementAt(i) + ",newSampleCount);");
                                                                                  
                                            myMatlabWrapper.execute("[errorvar13_" + threadTrials.ElementAt(i) + ", positionStatus" + threadTrials.ElementAt(i) + ",newMeasureTime" + threadTrials.ElementAt(i) + "] = timeNorm(positionStatus" + threadTrials.ElementAt(i) + ",measure_data_time" + threadTrials.ElementAt(i) + ",newSampleCount);");
                                                                                  
                                            myMatlabWrapper.execute("[errorvar14_" + threadTrials.ElementAt(i) + ", velocityX" + threadTrials.ElementAt(i) + ",newVelocityTime" + threadTrials.ElementAt(i) + "] = timeNorm(velocityX" + threadTrials.ElementAt(i) + ",velocity_data_time" + threadTrials.ElementAt(i) + ",newSampleCount);");
                                            myMatlabWrapper.execute("[errorvar15_" + threadTrials.ElementAt(i) + ", velocityY" + threadTrials.ElementAt(i) + ",newVelocityTime" + threadTrials.ElementAt(i) + "] = timeNorm(velocityY" + threadTrials.ElementAt(i) + ",velocity_data_time" + threadTrials.ElementAt(i) + ",newSampleCount);");
                                            myMatlabWrapper.execute("[errorvar16_" + threadTrials.ElementAt(i) + ", velocityZ" + threadTrials.ElementAt(i) + ",newVelocityTime" + threadTrials.ElementAt(i) + "] = timeNorm(velocityZ" + threadTrials.ElementAt(i) + ",velocity_data_time" + threadTrials.ElementAt(i) + ",newSampleCount);");

                                            ///

                                            for (int errorVarCounterCounter = 1; errorVarCounterCounter <= 16; errorVarCounterCounter++)
                                            {
                                                errorList.Add(Convert.ToString(myMatlabWrapper.getWorkspaceData("errorvar" + errorVarCounterCounter + "_" + threadTrials.ElementAt(i))));
                                                myMatlabWrapper.clearWorkspaceData("errorvar" + errorVarCounterCounter + "_" + threadTrials.ElementAt(i));
                                            }

                                            if (errorList.Where(t => t != "" && t != null).Count() > 0)
                                            {
                                                writeToLogBox(errorList.Where(t => t != "" && t != null).Select(t => t + " in " + filename + " at szenario-trial-number " + tempSzenarioTrialNumber).ToArray());
                                            }

                                            ///
                                           
                                            double[,] measure_data_time = myMatlabWrapper.getWorkspaceData("newMeasureTime" + threadTrials.ElementAt(i));
                                            double[,] velocity_data_time = myMatlabWrapper.getWorkspaceData("newVelocityTime" + threadTrials.ElementAt(i));

                                            double[,] force_actual_x = myMatlabWrapper.getWorkspaceData("forceActualX" + threadTrials.ElementAt(i));
                                            double[,] force_actual_y = myMatlabWrapper.getWorkspaceData("forceActualY" + threadTrials.ElementAt(i));
                                            double[,] force_actual_z = myMatlabWrapper.getWorkspaceData("forceActualZ" + threadTrials.ElementAt(i));

                                            double[,] force_nominal_x = myMatlabWrapper.getWorkspaceData("forceNominalX" + threadTrials.ElementAt(i));
                                            double[,] force_nominal_y = myMatlabWrapper.getWorkspaceData("forceNominalY" + threadTrials.ElementAt(i));
                                            double[,] force_nominal_z = myMatlabWrapper.getWorkspaceData("forceNominalZ" + threadTrials.ElementAt(i));

                                            double[,] force_moment_x = myMatlabWrapper.getWorkspaceData("forceMomentX" + threadTrials.ElementAt(i));
                                            double[,] force_moment_y = myMatlabWrapper.getWorkspaceData("forceMomentY" + threadTrials.ElementAt(i));
                                            double[,] force_moment_z = myMatlabWrapper.getWorkspaceData("forceMomentZ" + threadTrials.ElementAt(i));

                                            double[,] position_cartesian_x = myMatlabWrapper.getWorkspaceData("positionCartesianX" + threadTrials.ElementAt(i));
                                            double[,] position_cartesian_y = myMatlabWrapper.getWorkspaceData("positionCartesianY" + threadTrials.ElementAt(i));
                                            double[,] position_cartesian_z = myMatlabWrapper.getWorkspaceData("positionCartesianZ" + threadTrials.ElementAt(i));

                                            double[,] position_status = myMatlabWrapper.getWorkspaceData("positionStatus" + threadTrials.ElementAt(i));

                                            double[,] velocity_x = myMatlabWrapper.getWorkspaceData("velocityX" + threadTrials.ElementAt(i));
                                            double[,] velocity_y = myMatlabWrapper.getWorkspaceData("velocityY" + threadTrials.ElementAt(i));
                                            double[,] velocity_z = myMatlabWrapper.getWorkspaceData("velocityZ" + threadTrials.ElementAt(i));

                                            ///

                                            for (int j = 0; j < measure_data_time.Length; j++)
                                            {
                                                lock (myDataContainter)
                                                {
                                                    myDataContainter.measureDataNormalized.Add(new MeasureDataContainer(
                                                                                    new DateTime(Convert.ToInt64(measure_data_time[j, 0])),
                                                                                    force_actual_x[j, 0],
                                                                                    force_actual_y[j, 0],
                                                                                    force_actual_z[j, 0],
                                                                                    force_nominal_x[j, 0],
                                                                                    force_nominal_y[j, 0],
                                                                                    force_nominal_z[j, 0],
                                                                                    force_moment_x[j, 0],
                                                                                    force_moment_y[j, 0],
                                                                                    force_moment_z[j, 0],
                                                                                    position_cartesian_x[j, 0],
                                                                                    position_cartesian_y[j, 0],
                                                                                    position_cartesian_z[j, 0],
                                                                                    tempTargetNumber,
                                                                                    tempTargetTrialNumber,
                                                                                    tempSzenarioTrialNumber,
                                                                                    tempIsCatchTrial,
                                                                                    Convert.ToInt32(position_status[j, 0])
                                                                                    ));
                                                }
                                            }

                                            for (int j = 0; j < velocity_data_time.Length; j++)
                                            {
                                                lock (myDataContainter)
                                                {
                                                    myDataContainter.velocityDataNormalized.Add(new VelocityDataContainer(
                                                                                    new DateTime(Convert.ToInt64(velocity_data_time[j, 0])),
                                                                                    velocity_x[j, 0],
                                                                                    velocity_y[j, 0],
                                                                                    velocity_z[j, 0],
                                                                                    threadTrials.ElementAt(i),
                                                                                    tempFilteredDataEnum.ElementAt(0).target_number,
                                                                                    Convert.ToInt32(position_status[j, 0])
                                                                                    ));
                                                }
                                            }

                                            myMatlabWrapper.clearWorkspaceData("newMeasureTime" + threadTrials.ElementAt(i));
                                            myMatlabWrapper.clearWorkspaceData("measure_data_time" + threadTrials.ElementAt(i));
                                            myMatlabWrapper.clearWorkspaceData("velocity_data_time" + threadTrials.ElementAt(i));

                                            myMatlabWrapper.clearWorkspaceData("forceActualX" + threadTrials.ElementAt(i));
                                            myMatlabWrapper.clearWorkspaceData("forceActualY" + threadTrials.ElementAt(i));
                                            myMatlabWrapper.clearWorkspaceData("forceActualZ" + threadTrials.ElementAt(i));

                                            myMatlabWrapper.clearWorkspaceData("forceNominalX" + threadTrials.ElementAt(i));
                                            myMatlabWrapper.clearWorkspaceData("forceNominalY" + threadTrials.ElementAt(i));
                                            myMatlabWrapper.clearWorkspaceData("forceNominalZ" + threadTrials.ElementAt(i));

                                            myMatlabWrapper.clearWorkspaceData("forceMomentX" + threadTrials.ElementAt(i));
                                            myMatlabWrapper.clearWorkspaceData("forceMomentY" + threadTrials.ElementAt(i));
                                            myMatlabWrapper.clearWorkspaceData("forceMomentZ" + threadTrials.ElementAt(i));

                                            myMatlabWrapper.clearWorkspaceData("positionCartesianX" + threadTrials.ElementAt(i));
                                            myMatlabWrapper.clearWorkspaceData("positionCartesianY" + threadTrials.ElementAt(i));
                                            myMatlabWrapper.clearWorkspaceData("positionCartesianZ" + threadTrials.ElementAt(i));

                                            myMatlabWrapper.clearWorkspaceData("positionStatus" + threadTrials.ElementAt(i));

                                            myMatlabWrapper.clearWorkspaceData("newVelocityTime" + threadTrials.ElementAt(i));

                                            myMatlabWrapper.clearWorkspaceData("velocityX" + threadTrials.ElementAt(i));
                                            myMatlabWrapper.clearWorkspaceData("velocityY" + threadTrials.ElementAt(i));
                                            myMatlabWrapper.clearWorkspaceData("velocityZ" + threadTrials.ElementAt(i));
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
                            myMatlabWrapper.clearWorkspace();
                            multiCoreThreads.Clear();

                            #endregion

                            writeProgressInfo("Calculating baselines...");
                            #region Calculate baselines

                            if (myDataContainter.szenarioName == "Szenario02")
                            {
                                myDataContainter.baselineData = new List<BaselineDataContainer>();

                                for (int core = 0; core < Environment.ProcessorCount; core++)
                                {
                                    List<int> _threadTargets = targetCoreDistribution[core];

                                    multiCoreThreads.Add(new Thread(delegate()
                                    {
                                        List<int> threadTargets = new List<int>(_threadTargets);

                                        for (int targetCount = 0; targetCount < threadTargets.Count(); targetCount++)
                                        {
                                            List<MeasureDataContainer> tempNormalisedDataEnum;
                                            List<VelocityDataContainer> tempVelocityDataNormalisedEnum;

                                            lock (myDataContainter)
                                            {
                                                tempNormalisedDataEnum = new List<MeasureDataContainer>(myDataContainter.measureDataNormalized.Where(t => t.target_number == threadTargets.ElementAt(targetCount)).OrderBy(t => t.time_stamp));
                                                tempVelocityDataNormalisedEnum = new List<VelocityDataContainer>(myDataContainter.velocityDataNormalized.Where(t => t.target_number == threadTargets.ElementAt(targetCount)).OrderBy(t => t.time_stamp));
                                            }

                                            int[] tempSzenarioTrialNumbers = tempNormalisedDataEnum.Where(t => t.target_trial_number > 1).Select(t => t.szenario_trial_number).OrderBy(t => t).Distinct().ToArray();
                                            int measureDataCount = tempNormalisedDataEnum.Where(t => t.szenario_trial_number == tempSzenarioTrialNumbers.ElementAt(0)).Count();

                                            double[] positionCartesianX = new double[measureDataCount];
                                            double[] positionCartesianY = new double[measureDataCount];
                                            double[] positionCartesianZ = new double[measureDataCount];

                                            double[] velocityX = new double[measureDataCount];
                                            double[] velocityY = new double[measureDataCount];
                                            double[] velocityZ = new double[measureDataCount];

                                            for (int i = 0; i < tempSzenarioTrialNumbers.Count(); i++)
                                            {
                                                List<MeasureDataContainer> tempMeasureDataCountainerList = tempNormalisedDataEnum.Where(t => t.szenario_trial_number == tempSzenarioTrialNumbers.ElementAt(i)).OrderBy(t => t.time_stamp).ToList();
                                                List<VelocityDataContainer> tempVelocityDataContainerList = tempVelocityDataNormalisedEnum.Where(t => t.szenario_trial_number == tempSzenarioTrialNumbers.ElementAt(i)).OrderBy(t => t.time_stamp).ToList();

                                                for (int j = 0; j < tempMeasureDataCountainerList.Count(); j++)
                                                {
                                                    positionCartesianX[j] += tempMeasureDataCountainerList.ElementAt(j).position_cartesian_x;
                                                    positionCartesianY[j] += tempMeasureDataCountainerList.ElementAt(j).position_cartesian_y;
                                                    positionCartesianZ[j] += tempMeasureDataCountainerList.ElementAt(j).position_cartesian_z;
                                                    velocityX[j] += tempVelocityDataContainerList.ElementAt(j).velocity_x;
                                                    velocityY[j] += tempVelocityDataContainerList.ElementAt(j).velocity_y;
                                                    velocityZ[j] += tempVelocityDataContainerList.ElementAt(j).velocity_z;
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

                                            DateTime tempFileCreationDateTime = DateTime.Parse("00:00:00 " + myDataContainter.measureFileCreationDate);
                                            for (int i = 0; i < positionCartesianX.Length; i++)
                                            {
                                                lock (myDataContainter)
                                                {
                                                    myDataContainter.baselineData.Add(new BaselineDataContainer(
                                                                                        tempFileCreationDateTime.AddMilliseconds(i * 10),
                                                                                        positionCartesianX[i],
                                                                                        positionCartesianY[i],
                                                                                        positionCartesianZ[i],
                                                                                        velocityX[i],
                                                                                        velocityY[i],
                                                                                        velocityZ[i],
                                                                                        threadTargets.ElementAt(targetCount)
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

                            writeProgressInfo("Calculating szenario mean times...");
                            #region Calculate szenario mean times

                            for (int core = 0; core < Environment.ProcessorCount; core++)
                            {
                                List<int> _threadTargets = targetCoreDistribution[core];

                                multiCoreThreads.Add(new Thread(delegate()
                                {
                                    List<int> threadTargets = new List<int>(_threadTargets);

                                    for (int targetCount = 0; targetCount < threadTargets.Count(); targetCount++)
                                    {
                                        List<MeasureDataContainer> tempNormalisedDataEnum;

                                        lock (myDataContainter)
                                        {
                                            tempNormalisedDataEnum = new List<MeasureDataContainer>(myDataContainter.measureDataNormalized.Where(t => t.target_number == threadTargets.ElementAt(targetCount)).OrderBy(t => t.time_stamp));
                                        }

                                        int[] tempSzenarioTrialNumbers = tempNormalisedDataEnum.Select(t => t.szenario_trial_number).OrderBy(t => t).Distinct().ToArray();

                                        long[] meanTimeStdArray = new long[tempSzenarioTrialNumbers.Length];

                                        for (int i = 0; i < tempSzenarioTrialNumbers.Length; i++)
                                        {
                                            long maxVal = tempNormalisedDataEnum.Where(t => t.szenario_trial_number == tempSzenarioTrialNumbers.ElementAt(i)).Select(t => t.time_stamp.Ticks).Max();
                                            long minVal = tempNormalisedDataEnum.Where(t => t.szenario_trial_number == tempSzenarioTrialNumbers.ElementAt(i)).Select(t => t.time_stamp.Ticks).Min();

                                            meanTimeStdArray[i] = (maxVal - minVal);
                                        }

                                        TimeSpan meanTime = new TimeSpan(meanTimeStdArray.Sum() / meanTimeStdArray.Length);

                                        myMatlabWrapper.setWorkspaceData("timeArray" + threadTargets.ElementAt(targetCount), meanTimeStdArray);
                                        myMatlabWrapper.execute("meanTimeStd" + threadTargets.ElementAt(targetCount) + " = int64(std(double(timeArray" + threadTargets.ElementAt(targetCount) + ")));");
                                        TimeSpan meanTimeStd = new TimeSpan(myMatlabWrapper.getWorkspaceData("meanTimeStd" + threadTargets.ElementAt(targetCount)));
                                        myMatlabWrapper.clearWorkspaceData("timeArray" + threadTargets.ElementAt(targetCount));
                                        myMatlabWrapper.clearWorkspaceData("meanTimeStd" + threadTargets.ElementAt(targetCount));


                                        lock (myDataContainter)
                                        {
                                            myDataContainter.szenarioMeanTimeData.Add(new SzenarioMeanTimeDataContainer(
                                                                                meanTime,
                                                                                meanTimeStd,
                                                                                threadTargets.ElementAt(targetCount)
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
                            myMatlabWrapper.clearWorkspace();
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

                            int measureFileID = mySQLWrapper.insertMeasureFile(DateTime.Parse(myDataContainter.measureFileCreationTime + " " + myDataContainter.measureFileCreationDate), myDataContainter.measureFileHash);
                            int studyID = mySQLWrapper.insertStudy(myDataContainter.studyName);
                            int szenarioID = mySQLWrapper.insertSzenario(myDataContainter.szenarioName);
                            int groupID = mySQLWrapper.insertGroup(myDataContainter.groupName);
                            int subjectID = mySQLWrapper.insertSubject(myDataContainter.subjectName, myDataContainter.subjectID);


                            #region Upload trials

                            for (int i = 0; i < szenarioTrialNumbers.Length; i++)
                            {
                                writeProgressInfo("Preparing Trial " + (i + 1) + " of " + szenarioTrialNumbers.Length);
                                
                                List<MeasureDataContainer> measureDataRawList = myDataContainter.measureDataRaw.Where(t => t.szenario_trial_number == szenarioTrialNumbers[i]).OrderBy(t => t.time_stamp).ToList();
                                List<MeasureDataContainer> measureDataFilteredList = null;
                                List<MeasureDataContainer> measureDataNormalizedList = null;
                                List<VelocityDataContainer> velocityDataFilteredList = null;
                                List<VelocityDataContainer> velocityDataNormalizedList = null;

                                int targetID = mySQLWrapper.insertTarget(measureDataRawList.ElementAt(0).target_number);
                                int targetTrialNumberID = mySQLWrapper.insertTargetTrialNumber(measureDataRawList.ElementAt(0).target_trial_number);
                                int szenarioTrialNumberID = mySQLWrapper.insertSzenarioTrialNumber(szenarioTrialNumbers[i]);
                                int trialInformationID = mySQLWrapper.insertTrialInformation(measureDataRawList.ElementAt(0).contains_duplicates, filterOrder, cutoffFreq, velocityCuttingThreshold);
                                int isCatchTrialID = mySQLWrapper.insertIsCatchTrial(measureDataRawList.ElementAt(0).is_catch_trial);
                                int trialID = mySQLWrapper.insertTrial(
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


                                List<string> dataFileCache = new List<string>();

                                for (int j = 0; j < measureDataRawList.Count; j++)
                                {
                                    string tempLine = "," + trialID + "," +
                                                      measureDataRawList[j].time_stamp.ToString("yyyy-MM-dd HH:mm:ss.fffffff") + "," +
                                                      DoubleConverter.ToExactString(measureDataRawList[j].force_actual_x) + "," +
                                                      DoubleConverter.ToExactString(measureDataRawList[j].force_actual_y) + "," +
                                                      DoubleConverter.ToExactString(measureDataRawList[j].force_actual_z) + "," +
                                                      DoubleConverter.ToExactString(measureDataRawList[j].force_nominal_x) + "," +
                                                      DoubleConverter.ToExactString(measureDataRawList[j].force_nominal_y) + "," +
                                                      DoubleConverter.ToExactString(measureDataRawList[j].force_nominal_z) + "," +
                                                      DoubleConverter.ToExactString(measureDataRawList[j].force_moment_x) + "," +
                                                      DoubleConverter.ToExactString(measureDataRawList[j].force_moment_y) + "," +
                                                      DoubleConverter.ToExactString(measureDataRawList[j].force_moment_z) + "," +
                                                      DoubleConverter.ToExactString(measureDataRawList[j].position_cartesian_x) + "," +
                                                      DoubleConverter.ToExactString(measureDataRawList[j].position_cartesian_y) + "," +
                                                      DoubleConverter.ToExactString(measureDataRawList[j].position_cartesian_z) + "," +
                                                      measureDataRawList[j].position_status;

                                    dataFileCache.Add(tempLine);
                                }


                                FileStream dataFileStream = new FileStream("C:\\measureDataRaw.dat", FileMode.Append, FileAccess.Write);
                                StreamWriter dataFileWriter = new StreamWriter(dataFileStream);

                                for (int cacheWriter = 0; cacheWriter < dataFileCache.Count(); cacheWriter++)
                                {
                                    dataFileWriter.WriteLine(dataFileCache[cacheWriter]);
                                }


                                dataFileWriter.Close();
                                dataFileStream.Close();
                                dataFileCache.Clear();

                                if (myDataContainter.measureDataFiltered.Select(t => t.szenario_trial_number).Contains(szenarioTrialNumbers[i]))
                                {
                                    measureDataFilteredList = myDataContainter.measureDataFiltered.Where(t => t.szenario_trial_number == szenarioTrialNumbers[i]).OrderBy(t => t.time_stamp).ToList();

                                    for (int j = 0; j < measureDataFilteredList.Count; j++)
                                    {
                                        string tempLine = "," + trialID + "," +
                                                          measureDataFilteredList[j].time_stamp.ToString("yyyy-MM-dd HH:mm:ss.fffffff") + "," +
                                                          DoubleConverter.ToExactString(measureDataFilteredList[j].force_actual_x) + "," +
                                                          DoubleConverter.ToExactString(measureDataFilteredList[j].force_actual_y) + "," +
                                                          DoubleConverter.ToExactString(measureDataFilteredList[j].force_actual_z) + "," +
                                                          DoubleConverter.ToExactString(measureDataFilteredList[j].force_nominal_x) + "," +
                                                          DoubleConverter.ToExactString(measureDataFilteredList[j].force_nominal_y) + "," +
                                                          DoubleConverter.ToExactString(measureDataFilteredList[j].force_nominal_z) + "," +
                                                          DoubleConverter.ToExactString(measureDataFilteredList[j].force_moment_x) + "," +
                                                          DoubleConverter.ToExactString(measureDataFilteredList[j].force_moment_y) + "," +
                                                          DoubleConverter.ToExactString(measureDataFilteredList[j].force_moment_z) + "," +
                                                          DoubleConverter.ToExactString(measureDataFilteredList[j].position_cartesian_x) + "," +
                                                          DoubleConverter.ToExactString(measureDataFilteredList[j].position_cartesian_y) + "," +
                                                          DoubleConverter.ToExactString(measureDataFilteredList[j].position_cartesian_z) + "," +
                                                          measureDataFilteredList[j].position_status;

                                        dataFileCache.Add(tempLine);
                                    }

                                    dataFileStream = new FileStream("C:\\measureDataFiltered.dat", FileMode.Append, FileAccess.Write);
                                    dataFileWriter = new StreamWriter(dataFileStream);

                                    for (int cacheWriter = 0; cacheWriter < dataFileCache.Count(); cacheWriter++)
                                    {
                                        dataFileWriter.WriteLine(dataFileCache[cacheWriter]);
                                    }


                                    dataFileWriter.Close();
                                    dataFileStream.Close();
                                    dataFileCache.Clear();
                                }

                                if (myDataContainter.measureDataNormalized.Select(t => t.szenario_trial_number).Contains(szenarioTrialNumbers[i]))
                                {
                                    measureDataNormalizedList = myDataContainter.measureDataNormalized.Where(t => t.szenario_trial_number == szenarioTrialNumbers[i]).OrderBy(t => t.time_stamp).ToList();

                                    for (int j = 0; j < measureDataNormalizedList.Count; j++)
                                    {
                                        string tempLine = "," + trialID + "," +
                                                          measureDataNormalizedList[j].time_stamp.ToString("yyyy-MM-dd HH:mm:ss.fffffff") + "," +
                                                          DoubleConverter.ToExactString(measureDataNormalizedList[j].force_actual_x) + "," +
                                                          DoubleConverter.ToExactString(measureDataNormalizedList[j].force_actual_y) + "," +
                                                          DoubleConverter.ToExactString(measureDataNormalizedList[j].force_actual_z) + "," +
                                                          DoubleConverter.ToExactString(measureDataNormalizedList[j].force_nominal_x) + "," +
                                                          DoubleConverter.ToExactString(measureDataNormalizedList[j].force_nominal_y) + "," +
                                                          DoubleConverter.ToExactString(measureDataNormalizedList[j].force_nominal_z) + "," +
                                                          DoubleConverter.ToExactString(measureDataNormalizedList[j].force_moment_x) + "," +
                                                          DoubleConverter.ToExactString(measureDataNormalizedList[j].force_moment_y) + "," +
                                                          DoubleConverter.ToExactString(measureDataNormalizedList[j].force_moment_z) + "," +
                                                          DoubleConverter.ToExactString(measureDataNormalizedList[j].position_cartesian_x) + "," +
                                                          DoubleConverter.ToExactString(measureDataNormalizedList[j].position_cartesian_y) + "," +
                                                          DoubleConverter.ToExactString(measureDataNormalizedList[j].position_cartesian_z) + "," +
                                                          measureDataNormalizedList[j].position_status;

                                        dataFileCache.Add(tempLine);
                                    }

                                    dataFileStream = new FileStream("C:\\measureDataNormalized.dat", FileMode.Append, FileAccess.Write);
                                    dataFileWriter = new StreamWriter(dataFileStream);

                                    for (int cacheWriter = 0; cacheWriter < dataFileCache.Count(); cacheWriter++)
                                    {
                                        dataFileWriter.WriteLine(dataFileCache[cacheWriter]);
                                    }


                                    dataFileWriter.Close();
                                    dataFileStream.Close();
                                    dataFileCache.Clear();
                                }

                                if (myDataContainter.velocityDataFiltered.Select(t => t.szenario_trial_number).Contains(szenarioTrialNumbers[i]))
                                {
                                    velocityDataFilteredList = myDataContainter.velocityDataFiltered.Where(t => t.szenario_trial_number == szenarioTrialNumbers[i]).OrderBy(t => t.time_stamp).ToList();

                                    for (int j = 0; j < velocityDataFilteredList.Count; j++)
                                    {
                                        string tempLine = "," + trialID + "," +
                                                          velocityDataFilteredList[j].time_stamp.ToString("yyyy-MM-dd HH:mm:ss.fffffff") + "," +
                                                          DoubleConverter.ToExactString(velocityDataFilteredList[j].velocity_x) + "," +
                                                          DoubleConverter.ToExactString(velocityDataFilteredList[j].velocity_y) + "," +
                                                          DoubleConverter.ToExactString(velocityDataFilteredList[j].velocity_z);

                                        dataFileCache.Add(tempLine);
                                    }

                                    dataFileStream = new FileStream("C:\\velocityDataFiltered.dat", FileMode.Append, FileAccess.Write);
                                    dataFileWriter = new StreamWriter(dataFileStream);

                                    for (int cacheWriter = 0; cacheWriter < dataFileCache.Count(); cacheWriter++)
                                    {
                                        dataFileWriter.WriteLine(dataFileCache[cacheWriter]);
                                    }


                                    dataFileWriter.Close();
                                    dataFileStream.Close();
                                    dataFileCache.Clear();
                                }

                                if (myDataContainter.velocityDataNormalized.Select(t => t.szenario_trial_number).Contains(szenarioTrialNumbers[i]))
                                {
                                    velocityDataNormalizedList = myDataContainter.velocityDataNormalized.Where(t => t.szenario_trial_number == szenarioTrialNumbers[i]).OrderBy(t => t.time_stamp).ToList();

                                    for (int j = 0; j < velocityDataNormalizedList.Count; j++)
                                    {
                                        string tempLine = "," + trialID + "," +
                                                          velocityDataNormalizedList[j].time_stamp.ToString("yyyy-MM-dd HH:mm:ss.fffffff") + "," +
                                                          DoubleConverter.ToExactString(velocityDataNormalizedList[j].velocity_x) + "," +
                                                          DoubleConverter.ToExactString(velocityDataNormalizedList[j].velocity_y) + "," +
                                                          DoubleConverter.ToExactString(velocityDataNormalizedList[j].velocity_z);

                                        dataFileCache.Add(tempLine);
                                    }

                                    dataFileStream = new FileStream("C:\\velocityDataNormalized.dat", FileMode.Append, FileAccess.Write);
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

                            writeProgressInfo("Uploading trial data...");

                            mySQLWrapper.bulkInsertMeasureDataRaw("C:\\measureDataRaw.dat");
                            File.Delete("C:\\measureDataRaw.dat");

                            if (File.Exists("C:\\measureDataFiltered.dat"))
                            {
                                mySQLWrapper.bulkInsertMeasureDataFiltered("C:\\measureDataFiltered.dat");
                                File.Delete("C:\\measureDataFiltered.dat");
                            }
                            if (File.Exists("C:\\measureDataNormalized.dat"))
                            {
                                mySQLWrapper.bulkInsertMeasureDataNormalized("C:\\measureDataNormalized.dat");
                                File.Delete("C:\\measureDataNormalized.dat");
                            }
                            if (File.Exists("C:\\velocityDataFiltered.dat"))
                            {
                                mySQLWrapper.bulkInsertVelocityDataFiltered("C:\\velocityDataFiltered.dat");
                                File.Delete("C:\\velocityDataFiltered.dat");
                            }
                            if (File.Exists("C:\\velocityDataNormalized.dat"))
                            {
                                mySQLWrapper.bulkInsertVelocityDataNormalized("C:\\velocityDataNormalized.dat");
                                File.Delete("C:\\velocityDataNormalized.dat");
                            }
                            
                            #endregion

                            #region Upload szenario mean times

                            writeProgressInfo("Uploading szenario mean-time data...");
                            for (int j = 0; j < myDataContainter.szenarioMeanTimeData.Count; j++)
                            {
                               
                                int targetID = mySQLWrapper.insertTarget(myDataContainter.szenarioMeanTimeData[j].target_number);

                                int szenarioMeanTimeID = mySQLWrapper.insertSzenarioMeanTime(
                                                                        subjectID,
                                                                        studyID,
                                                                        groupID,
                                                                        targetID,
                                                                        szenarioID,
                                                                        measureFileID
                                                                    );

                                mySQLWrapper.insertSzenarioMeanTimeData(szenarioMeanTimeID, myDataContainter.szenarioMeanTimeData[j].mean_time, myDataContainter.szenarioMeanTimeData[j].mean_time_std);
                            }

                            #endregion

                            #region Upload baselines

                            writeProgressInfo("Uploading baseline data...");
                            if (myDataContainter.baselineData != null)
                            {
                                for (int j = 0; j < targetNumbers.Length; j++)
                                {
                                    int targetID = mySQLWrapper.insertTarget(targetNumbers[j]);

                                    int baselineID =  mySQLWrapper.insertBaseline(
                                                                    subjectID,
                                                                    studyID,
                                                                    groupID,
                                                                    targetID,
                                                                    szenarioID,
                                                                    measureFileID
                                                                );

                                    List<BaselineDataContainer> baselineDataList = myDataContainter.baselineData.Where(t => t.target_number == targetNumbers[j]).OrderBy(t => t.pseudo_time_stamp).ToList();

                                    for (int k = 0; k < baselineDataList.Count; k++)
                                    {
                                        mySQLWrapper.insertBaselineData(
                                                                            baselineID,
                                                                            baselineDataList[k].pseudo_time_stamp,
                                                                            baselineDataList[k].baseline_position_cartesian_x,
                                                                            baselineDataList[k].baseline_position_cartesian_y,
                                                                            baselineDataList[k].baseline_position_cartesian_z,
                                                                            baselineDataList[k].baseline_velocity_x,
                                                                            baselineDataList[k].baseline_velocity_y,
                                                                            baselineDataList[k].baseline_velocity_z
                                                                        );
                                    }
                                }

                            }

                            #endregion

                            #endregion

                        }
                        else
                        {
                            writeToLogBox("Fehler beim einlesen der Datei \"" + filename + "\"");
                        }
                    }
                }
                setProgressBarValue(0);
                writeProgressInfo("Ready");
                enableTabPages(true);

                writeToLogBox("Counted " + debugCounterZero + " events where velocity threshold in PositionsStatus 0 wasn't exceeded.");
                writeToLogBox("Counted " + debugCounterTwo + " events where velocity threshold in PositionsStatus 2 wasn't undershot,");
                writeToLogBox("of wich " + debugCounterTwo + " times a PositionsStatus 3 was not found.");
                ThreadManager.remove(newThread);
            });
            ThreadManager.pushBack(newThread);
            newThread.Start();
        }

        private void button_CalculateStatistics_Click(object sender, EventArgs e)
        {
            Thread newThread = null;
            newThread = new Thread(delegate()
            {
                while (ThreadManager.getIndex(newThread) != 0)
                {
                    Thread.Sleep(100);
                }

                enableTabPages(false);
                writeProgressInfo("Calculating statistics...");

                List<int[]> trialInfos = mySQLWrapper.getStatisticCalculationInformation();

                if (trialInfos != null)
                {
                    int counter = 1;

                    foreach (int[] trialInfo in trialInfos)
                    {
                        while (ThreadManager.pause)
                        {
                            Thread.Sleep(100);
                        }
                        setProgressBarValue((100.0 / trialInfos.Count()) * counter);
                        counter++;

                        DataSet measureDataSet = mySQLWrapper.getMeasureDataNormalizedDataSet(trialInfo[0]);
                        DataSet velocityDataSet = mySQLWrapper.getVelocityDataNormalizedDataSet(trialInfo[0]);
                        DataSet baselineDataSet = mySQLWrapper.getBaselineDataSet(trialInfo[1], trialInfo[2], trialInfo[3], trialInfo[4]);
                        int targetNumber = trialInfo[5];

                        if (baselineDataSet.Tables[0].Rows.Count > 0)
                        {
                            if ((measureDataSet.Tables[0].Rows.Count == velocityDataSet.Tables[0].Rows.Count) && (velocityDataSet.Tables[0].Rows.Count == baselineDataSet.Tables[0].Rows.Count))
                            {
                                try
                                {
                                    int sampleCount = measureDataSet.Tables[0].Rows.Count;

                                    double[,] measureData = new double[sampleCount, 3];
                                    double[,] velocityData = new double[sampleCount, 3];
                                    double[,] baselineData = new double[sampleCount, 6];
                                    double[] timeStamp = new double[sampleCount];

                                    for (int i = 0; i < sampleCount; i++)
                                    {
                                        timeStamp[i] = Convert.ToDateTime(measureDataSet.Tables[0].Rows[i]["time_stamp"]).Ticks;

                                        measureData[i, 0] = Convert.ToDouble(measureDataSet.Tables[0].Rows[i]["position_cartesian_x"]);
                                        measureData[i, 1] = Convert.ToDouble(measureDataSet.Tables[0].Rows[i]["position_cartesian_y"]);
                                        measureData[i, 2] = Convert.ToDouble(measureDataSet.Tables[0].Rows[i]["position_cartesian_z"]);

                                        velocityData[i, 0] = Convert.ToDouble(velocityDataSet.Tables[0].Rows[i]["velocity_x"]);
                                        velocityData[i, 1] = Convert.ToDouble(velocityDataSet.Tables[0].Rows[i]["velocity_y"]);
                                        velocityData[i, 2] = Convert.ToDouble(velocityDataSet.Tables[0].Rows[i]["velocity_z"]);

                                        baselineData[i, 0] = Convert.ToDouble(baselineDataSet.Tables[0].Rows[i]["baseline_position_cartesian_x"]);
                                        baselineData[i, 1] = Convert.ToDouble(baselineDataSet.Tables[0].Rows[i]["baseline_position_cartesian_y"]);
                                        baselineData[i, 2] = Convert.ToDouble(baselineDataSet.Tables[0].Rows[i]["baseline_position_cartesian_z"]);
                                        baselineData[i, 3] = Convert.ToDouble(baselineDataSet.Tables[0].Rows[i]["baseline_velocity_x"]);
                                        baselineData[i, 4] = Convert.ToDouble(baselineDataSet.Tables[0].Rows[i]["baseline_velocity_y"]);
                                        baselineData[i, 5] = Convert.ToDouble(baselineDataSet.Tables[0].Rows[i]["baseline_velocity_z"]);
                                    }

                                    List<double> tempTimeList = timeStamp.ToList();
                                    int time300msIndex = tempTimeList.IndexOf(tempTimeList.OrderBy(d => Math.Abs(d - (timeStamp[0] + TimeSpan.FromMilliseconds(300).Ticks))).ElementAt(0));

                                    myMatlabWrapper.setWorkspaceData("targetNumber", targetNumber);
                                    myMatlabWrapper.setWorkspaceData("time300msIndex", time300msIndex);
                                    myMatlabWrapper.setWorkspaceData("measureData", measureData);
                                    myMatlabWrapper.setWorkspaceData("velocityData", velocityData);
                                    myMatlabWrapper.setWorkspaceData("baselineData", baselineData);

                                    myMatlabWrapper.execute("vector_correlation = vectorCorrelation([velocityData(:,1) velocityData(:,3)],[baselineData(:,4) baselineData(:,6)]);");
                                    myMatlabWrapper.execute("enclosed_area = enclosedArea(measureData(:,1),measureData(:,3));");
                                    myMatlabWrapper.execute("length_abs = trajectLength(measureData(:,1),measureData(:,3));");
                                    myMatlabWrapper.execute("length_ratio = trajectLength(measureData(:,1),measureData(:,3)) / trajectLength(baselineData(:,1),baselineData(:,3));");
                                    myMatlabWrapper.execute("distanceAbs = distance2curveAbs([measureData(:,1),measureData(:,3)],targetNumber);");
                                    myMatlabWrapper.execute("distanceSign = distance2curveSign([measureData(:,1),measureData(:,3)],targetNumber);");
                                    myMatlabWrapper.execute("distance300msAbs = distanceAbs(time300msIndex);");
                                    myMatlabWrapper.execute("distance300msSign = distanceSign(time300msIndex);");
                                    myMatlabWrapper.execute("meanDistanceAbs = mean(distanceAbs);");
                                    myMatlabWrapper.execute("maxDistanceAbs = max(distanceAbs);");
                                    myMatlabWrapper.execute("[~, posDistanceSign] = max(abs(distanceSign));");
                                    myMatlabWrapper.execute("maxDistanceSign = distanceSign(posDistanceSign);");
                                    myMatlabWrapper.execute("rmse = rootMeanSquareError([measureData(:,1) measureData(:,3)], [baselineData(:,1) baselineData(:,3)]);");

                                    double vector_correlation = myMatlabWrapper.getWorkspaceData("vector_correlation");
                                    double enclosed_area = myMatlabWrapper.getWorkspaceData("enclosed_area");
                                    double length_abs = myMatlabWrapper.getWorkspaceData("length_abs");
                                    double length_ratio = myMatlabWrapper.getWorkspaceData("length_ratio");
                                    double distance300msAbs = myMatlabWrapper.getWorkspaceData("distance300msAbs");
                                    double distance300msSign = myMatlabWrapper.getWorkspaceData("distance300msSign");
                                    double meanDistanceAbs = myMatlabWrapper.getWorkspaceData("meanDistanceAbs");
                                    double maxDistanceAbs = myMatlabWrapper.getWorkspaceData("maxDistanceAbs");
                                    double maxDistanceSign = myMatlabWrapper.getWorkspaceData("maxDistanceSign");
                                    double rmse = myMatlabWrapper.getWorkspaceData("rmse");

                                    int statisticDataID = mySQLWrapper.insertStatisticData(
                                                                                            trialInfo[0],
                                                                                            vector_correlation,
                                                                                            length_abs,length_ratio,
                                                                                            distance300msAbs,
                                                                                            maxDistanceAbs,
                                                                                            meanDistanceAbs,
                                                                                            distance300msSign,
                                                                                            maxDistanceSign,
                                                                                            enclosed_area,
                                                                                            rmse
                                                                                            );                                                                                      
                                }                                                  
                                catch(Exception statisticException)
                                {
                                    writeToLogBox("Error in Statistic calculation: " + statisticException.ToString());
                                }
                            }
                            else
                            {
                                writeToLogBox("TrialID: " + trialInfo[0] + " - Data not normalised!");
                            }
                            myMatlabWrapper.clearWorkspace();
                        }
                        else
                        {
                            writeToLogBox("TrialID: " + trialInfo[0] + " - No matching baseline found!");
                        }
                    }
                }
                else
                {
                    writeToLogBox("Statistics already calculated!");
                }
                setProgressBarValue(0);
                writeProgressInfo("Ready");
                enableTabPages(true);

                ThreadManager.remove(newThread);
            });
            ThreadManager.pushBack(newThread);
            newThread.Start();
        }

        private void button_FixBrokenTrials_Click(object sender, EventArgs e)
        {
            Thread newThread = null;
            newThread = new Thread(delegate()
            {
                while (ThreadManager.getIndex(newThread) != 0)
                {
                    Thread.Sleep(100);
                }

                enableTabPages(false);
                writeProgressInfo("Fixing broken Trials...");

                List<object[]> faultyTrialInformation = mySQLWrapper.getFaultyTrialInformation();

                if (faultyTrialInformation != null)
                {
                    if (faultyTrialInformation.Count == 0)
                    {
                        writeToLogBox("Trials already fixed!");
                    }
                    else
                    {
                        for (int trialIDCounter = 0; trialIDCounter < faultyTrialInformation.Count; trialIDCounter++)
                        {
                            while (ThreadManager.pause)
                            {
                                Thread.Sleep(100);
                            }
                            setProgressBarValue((100.0 / faultyTrialInformation.Count) * trialIDCounter);

                            int[] trialFixInformation = mySQLWrapper.getFaultyTrialFixInformation(Convert.ToInt32(faultyTrialInformation[trialIDCounter][1]), Convert.ToInt32(faultyTrialInformation[trialIDCounter][7]));

                            DataSet upperStatisticDataSet = mySQLWrapper.getStatisticDataSet(trialFixInformation[0]);
                            DataSet lowerStatisticDataSet = mySQLWrapper.getStatisticDataSet(trialFixInformation[1]);

                            double velocity_vector_correlation = (Convert.ToDouble(upperStatisticDataSet.Tables[0].Rows[0]["velocity_vector_correlation"]) + Convert.ToDouble(lowerStatisticDataSet.Tables[0].Rows[0]["velocity_vector_correlation"])) / 2;
                            double trajectory_length_abs = (Convert.ToDouble(upperStatisticDataSet.Tables[0].Rows[0]["trajectory_length_abs"]) + Convert.ToDouble(lowerStatisticDataSet.Tables[0].Rows[0]["trajectory_length_abs"])) / 2;
                            double trajectory_length_ratio_baseline = (Convert.ToDouble(upperStatisticDataSet.Tables[0].Rows[0]["trajectory_length_ratio_baseline"]) + Convert.ToDouble(lowerStatisticDataSet.Tables[0].Rows[0]["trajectory_length_ratio_baseline"])) / 2;
                            double perpendicular_displacement_300ms_abs = (Convert.ToDouble(upperStatisticDataSet.Tables[0].Rows[0]["perpendicular_displacement_300ms_abs"]) + Convert.ToDouble(lowerStatisticDataSet.Tables[0].Rows[0]["perpendicular_displacement_300ms_abs"])) / 2;
                            double maximal_perpendicular_displacement_abs = (Convert.ToDouble(upperStatisticDataSet.Tables[0].Rows[0]["maximal_perpendicular_displacement_abs"]) + Convert.ToDouble(lowerStatisticDataSet.Tables[0].Rows[0]["maximal_perpendicular_displacement_abs"])) / 2;
                            double mean_perpendicular_displacement_abs = (Convert.ToDouble(upperStatisticDataSet.Tables[0].Rows[0]["mean_perpendicular_displacement_abs"]) + Convert.ToDouble(lowerStatisticDataSet.Tables[0].Rows[0]["mean_perpendicular_displacement_abs"])) / 2;
                            double perpendicular_displacement_300ms_sign = (Convert.ToDouble(upperStatisticDataSet.Tables[0].Rows[0]["perpendicular_displacement_300ms_sign"]) + Convert.ToDouble(lowerStatisticDataSet.Tables[0].Rows[0]["perpendicular_displacement_300ms_sign"])) / 2;
                            double maximal_perpendicular_displacement_sign = (Convert.ToDouble(upperStatisticDataSet.Tables[0].Rows[0]["maximal_perpendicular_displacement_sign"]) + Convert.ToDouble(lowerStatisticDataSet.Tables[0].Rows[0]["maximal_perpendicular_displacement_sign"])) / 2;
                            double enclosed_area = (Convert.ToDouble(upperStatisticDataSet.Tables[0].Rows[0]["enclosed_area"]) + Convert.ToDouble(lowerStatisticDataSet.Tables[0].Rows[0]["enclosed_area"])) / 2;
                            double rmse = (Convert.ToDouble(upperStatisticDataSet.Tables[0].Rows[0]["rmse"]) + Convert.ToDouble(lowerStatisticDataSet.Tables[0].Rows[0]["rmse"])) / 2;

                            mySQLWrapper.insertStatisticData(
                                                              Convert.ToInt32(faultyTrialInformation[trialIDCounter][0]),
                                                              velocity_vector_correlation,
                                                              trajectory_length_abs,
                                                              trajectory_length_ratio_baseline,
                                                              perpendicular_displacement_300ms_abs,
                                                              maximal_perpendicular_displacement_abs,
                                                              mean_perpendicular_displacement_abs,
                                                              perpendicular_displacement_300ms_sign,
                                                              maximal_perpendicular_displacement_sign,
                                                              enclosed_area,
                                                              rmse
                                                            );
                        }
                    }
                }
                else
                {
                    writeToLogBox("Trials already fixed!");
                }
                setProgressBarValue(0);
                writeProgressInfo("Ready");
                enableTabPages(true);

                ThreadManager.remove(newThread);
            });
            ThreadManager.pushBack(newThread);
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
            saveFileDialog.Title = "Save log file";
            saveFileDialog.AddExtension = true;
            saveFileDialog.DefaultExt = ".txt";
            saveFileDialog.Filter = "LogFiles (*.txt)|.txt";
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
                FileStream logFileStream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
                StreamWriter logFileWriter = new StreamWriter(logFileStream);

                string[] logText = getLogBoxText();
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
            clearLogBox();
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

            string[] studyNames = mySQLWrapper.getStudyNames();
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

            string[] groupNames = mySQLWrapper.getGroupNames(comboBox_TrajectoryVelocity_Study.SelectedItem.ToString());
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

                string[] szenarioIntersect = mySQLWrapper.getSzenarioNames(study, groups[0]);
                for (int i = 1; i < groups.Length; i++)
                {
                    szenarioIntersect = szenarioIntersect.Intersect(mySQLWrapper.getSzenarioNames(study, groups[i])).ToArray();
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
                listBox_TrajectoryVelocity_Subjects.Items.AddRange(mySQLWrapper.getSubjectInformations(study, groups[i], szenario).ToArray());
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
            SubjectInformationContainer[] subjects = listBox_TrajectoryVelocity_Subjects.SelectedItems.Cast<SubjectInformationContainer>().ToArray();

            string[] turnIntersect = null;
            for (int i = 0; i < groups.Length; i++)
            {
                for (int j = 0; j < subjects.Length; j++)
                {
                    string[] tempTurnString = mySQLWrapper.getTurns(study, groups[i], szenario, subjects[j].id); ;

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

            listBox_TrajectoryVelocity_Turns.Items.AddRange(turnIntersect);
            listBox_TrajectoryVelocity_Turns.SelectedIndex = 0;            
        }

        private void listBox_TrajectoryVelocity_Turns_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox_TrajectoryVelocity_Targets.Items.Clear();
            listBox_TrajectoryVelocity_Trials.Items.Clear();

            string study = comboBox_TrajectoryVelocity_Study.SelectedItem.ToString();
            string szenario = comboBox_TrajectoryVelocity_Szenario.SelectedItem.ToString();

            string[] targets = mySQLWrapper.getTargets(study, szenario).OrderBy(t => t).ToArray();
            string[] trials = mySQLWrapper.getTrials(study, szenario).OrderBy(t => t).ToArray();

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
                SubjectInformationContainer[] subjects = listBox_TrajectoryVelocity_Subjects.SelectedItems.Cast<SubjectInformationContainer>().ToArray();
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
                                if (mySQLWrapper.getTurns(study, group, szenario, subject.id) != null)
                                {
                                    if (listBox_TrajectoryVelocity_SelectedTrials.Items.Count > 0)
                                    {
                                        bool canBeUpdated = false;
                                        foreach (TrajectoryVelocityPlotContainer temp in listBox_TrajectoryVelocity_SelectedTrials.Items)
                                        {
                                            if (temp.updateTrajectoryVelocityPlotContainer(study, group, szenario, subject, turn, target, trials))
                                            {
                                                typeof(ListBox).InvokeMember("RefreshItems",
                                                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod,
                                                null, listBox_TrajectoryVelocity_SelectedTrials, new object[] { });
                                                canBeUpdated = true;
                                            }
                                        }

                                        if (!canBeUpdated)
                                        {
                                            listBox_TrajectoryVelocity_SelectedTrials.Items.Add(new TrajectoryVelocityPlotContainer(study, group, szenario, subject, turn, target, trials));
                                        }
                                    }
                                    else
                                    {
                                        listBox_TrajectoryVelocity_SelectedTrials.Items.Add(new TrajectoryVelocityPlotContainer(study, group, szenario, subject, turn, target, trials));
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
                SubjectInformationContainer[] subjects = listBox_TrajectoryVelocity_Subjects.SelectedItems.Cast<SubjectInformationContainer>().ToArray();
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
                                if (mySQLWrapper.getTurns(study, group, szenario, subject.id) != null)
                                {
                                    if (listBox_TrajectoryVelocity_SelectedTrials.Items.Count > 0)
                                    {
                                        bool canBeUpdated = false;
                                        foreach (TrajectoryVelocityPlotContainer temp in listBox_TrajectoryVelocity_SelectedTrials.Items)
                                        {
                                            if (temp.updateTrajectoryVelocityPlotContainer(study, group, szenario, subject, turn, target, trials))
                                            {
                                                typeof(ListBox).InvokeMember("RefreshItems",
                                                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod,
                                                null, listBox_TrajectoryVelocity_SelectedTrials, new object[] { });
                                                canBeUpdated = true;
                                            }
                                        }

                                        if (!canBeUpdated)
                                        {
                                            listBox_TrajectoryVelocity_SelectedTrials.Items.Add(new TrajectoryVelocityPlotContainer(study, group, szenario, subject, turn, target, trials));
                                        }
                                    }
                                    else
                                    {
                                        listBox_TrajectoryVelocity_SelectedTrials.Items.Add(new TrajectoryVelocityPlotContainer(study, group, szenario, subject, turn, target, trials));
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
                listBox_TrajectoryVelocity_SelectedTrials.Items.Remove(listBox_TrajectoryVelocity_SelectedTrials.SelectedItem);
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
                writeProgressInfo("Getting data...");
                if (comboBox_TrajectoryVelocity_IndividualMean.SelectedItem.ToString() == "Individual")
                {
                    if (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString() == "Trajectory")
                    {
                        myMatlabWrapper.createTrajectoryFigure("XZ-Plot");
                        myMatlabWrapper.drawTargets(0.005, 0.1, 0, 0);
                    }
                    else if (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString() == "Velocity")
                    {
                        myMatlabWrapper.createVelocityFigure("Velocity plot", 101);
                    }

                    int counter = 0;
                    foreach (TrajectoryVelocityPlotContainer tempContainer in listBox_TrajectoryVelocity_SelectedTrials.Items)
                    {
                        setProgressBarValue((100.0 / listBox_TrajectoryVelocity_SelectedTrials.Items.Count) * counter);
                        counter++;
                        DateTime turnDateTime = mySQLWrapper.getTurnDateTime(tempContainer.Study, tempContainer.Group, tempContainer.Szenario, tempContainer.Subject.id, tempContainer.Turn);
                        foreach (int trial in tempContainer.Trials)
                        {
                            if (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString() == "Trajectory")
                            {
                                int trialID = mySQLWrapper.getTrailID(tempContainer.Study, tempContainer.Group, tempContainer.Szenario, tempContainer.Subject.id, turnDateTime, tempContainer.Target, trial);
                                DataSet measureDataSet = mySQLWrapper.getMeasureDataNormalizedDataSet(trialID);

                                List<double> measureData_x = new List<double>();
                                List<double> measureData_z = new List<double>();

                                foreach (DataRow row in measureDataSet.Tables[0].Rows)
                                {
                                    if (checkBox_TrajectoryVelocity_IgnoreCatchTrials.Checked)
                                    {
                                        if (Convert.ToInt32(row["is_catch_trial"]) == 0)
                                        {
                                            measureData_x.Add(Convert.ToDouble(row["position_cartesian_x"]));
                                            measureData_z.Add(Convert.ToDouble(row["position_cartesian_z"]));
                                        }
                                    }
                                    else
                                    {
                                        measureData_x.Add(Convert.ToDouble(row["position_cartesian_x"]));
                                        measureData_z.Add(Convert.ToDouble(row["position_cartesian_z"]));
                                    }
                                }

                                myMatlabWrapper.setWorkspaceData("X", measureData_x.ToArray());
                                myMatlabWrapper.setWorkspaceData("Z", measureData_z.ToArray());
                                myMatlabWrapper.plot("X", "Z", "black", 2);
                            }

                            else if (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString() == "Velocity")
                            {
                                int trialID = mySQLWrapper.getTrailID(tempContainer.Study, tempContainer.Group, tempContainer.Szenario, tempContainer.Subject.id, turnDateTime, tempContainer.Target, trial);
                                DataSet velocityDataSet = mySQLWrapper.getVelocityDataNormalizedDataSet(trialID);
                                List<double> velocityData_xz = new List<double>();

                                foreach (DataRow row in velocityDataSet.Tables[0].Rows)
                                {
                                    if (checkBox_TrajectoryVelocity_IgnoreCatchTrials.Checked)
                                    {
                                        if (Convert.ToInt32(row["is_catch_trial"]) == 0)
                                        {
                                            velocityData_xz.Add(Math.Sqrt(Math.Pow(Convert.ToDouble(row["velocity_x"]), 2) + Math.Pow(Convert.ToDouble(row["velocity_z"]), 2)));
                                        }
                                    }
                                    else
                                    {
                                        velocityData_xz.Add(Math.Sqrt(Math.Pow(Convert.ToDouble(row["velocity_x"]), 2) + Math.Pow(Convert.ToDouble(row["velocity_z"]), 2)));
                                    }                                    
                                }

                                myMatlabWrapper.setWorkspaceData("XZ", velocityData_xz.ToArray());
                                myMatlabWrapper.plot("X", "Z", "black", 2);
                            }
                        }
                    }
                }

                else if (comboBox_TrajectoryVelocity_IndividualMean.SelectedItem.ToString() == "Mean")
                {
                    List<TrajectoryVelocityPlotContainer> tempTrajectoryVelocityPlotContainerList = new List<TrajectoryVelocityPlotContainer>(listBox_TrajectoryVelocity_SelectedTrials.Items.Cast<TrajectoryVelocityPlotContainer>().ToList());

                    if (tempTrajectoryVelocityPlotContainerList.Select(t => t.Trials.ToArray()).Distinct(new ArrayComparer()).ToList().Count() > 1)
                    {
                        writeToLogBox("Trial selections are not equal!");
                    }
                    else
                    {
                        if (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString() == "Trajectory")
                        {
                            myMatlabWrapper.createTrajectoryFigure("XZ-Plot");
                            myMatlabWrapper.drawTargets(0.005, 0.1, 0, 0);
                        }
                        else if (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString() == "Velocity")
                        {
                            myMatlabWrapper.createFigure("Velocity plot", "[Samples]", "Velocity [m/s]");
                        }

                        int[] targetArray = tempTrajectoryVelocityPlotContainerList.Select(t => t.Target).Distinct().ToArray();
                        int counter = 0;
                        
                        for (int targetCounter = 0; targetCounter < targetArray.Length; targetCounter++)
                        {
                            DataSet dataSet;
                            int meanCounter = 0;
                            List<double> data_x = new List<double>();
                            List<double> data_z = new List<double>();

                            foreach (TrajectoryVelocityPlotContainer tempContainer in tempTrajectoryVelocityPlotContainerList.Where(t => t.Target == targetArray[targetCounter]))
                            {
                                setProgressBarValue((100.0 / listBox_TrajectoryVelocity_SelectedTrials.Items.Count) * counter);
                                counter++;
                                DateTime turnDateTime = mySQLWrapper.getTurnDateTime(tempContainer.Study, tempContainer.Group, tempContainer.Szenario, tempContainer.Subject.id, tempContainer.Turn);

                                if (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString() == "Trajectory")
                                {
                                    foreach (int trial in tempContainer.Trials)
                                    {
                                        int trialID = mySQLWrapper.getTrailID(tempContainer.Study, tempContainer.Group, tempContainer.Szenario, tempContainer.Subject.id, turnDateTime, tempContainer.Target, trial);
                                        dataSet = mySQLWrapper.getMeasureDataNormalizedDataSet(trialID);
                                        for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                                        {
                                            DataRow row = dataSet.Tables[0].Rows[i];

                                            if (data_x.Count <= i)
                                            {
                                                data_x.Add(Convert.ToDouble(row["position_cartesian_x"]));
                                            }
                                            else
                                            {
                                                data_x[i] += Convert.ToDouble(row["position_cartesian_x"]);
                                            }

                                            if (data_z.Count <= i)
                                            {
                                                data_z.Add(Convert.ToDouble(row["position_cartesian_z"]));
                                            }
                                            else
                                            {
                                                data_z[i] += Convert.ToDouble(row["position_cartesian_z"]);
                                            }
                                        }
                                        meanCounter++;
                                    }
                                }
                                else if (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString() == "Velocity")
                                {
                                    foreach (int trial in tempContainer.Trials)
                                    {
                                        int trialID = mySQLWrapper.getTrailID(tempContainer.Study, tempContainer.Group, tempContainer.Szenario, tempContainer.Subject.id, turnDateTime, tempContainer.Target, trial);
                                        dataSet = mySQLWrapper.getVelocityDataNormalizedDataSet(trialID);
                                        
                                        for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                                        {
                                            DataRow row = dataSet.Tables[0].Rows[i];

                                            if (data_x.Count <= i)
                                            {
                                                data_x.Add(Math.Sqrt(Math.Pow(Convert.ToDouble(row["velocity_x"]), 2) + Math.Pow(Convert.ToDouble(row["velocity_z"]), 2)));
                                            }
                                            else
                                            {
                                                data_x[i] += Math.Sqrt(Math.Pow(Convert.ToDouble(row["velocity_x"]), 2) + Math.Pow(Convert.ToDouble(row["velocity_z"]), 2));
                                            }
                                        }
                                        meanCounter++;
                                    }
                                }
                            }

                            if (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString() == "Trajectory")
                            {
                                for (int i = 0; i < data_x.Count; i++)
                                {
                                    data_x[i] /= meanCounter;
                                    data_z[i] /= meanCounter;
                                }

                                myMatlabWrapper.setWorkspaceData("X", data_x.ToArray());
                                myMatlabWrapper.setWorkspaceData("Z", data_z.ToArray());
                                myMatlabWrapper.plot("X", "Z", "black", 2);
                            }
                            else if (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString() == "Velocity")
                            {
                                for (int i = 0; i < data_x.Count; i++)
                                {
                                    data_x[i] /= meanCounter;
                                }

                                myMatlabWrapper.setWorkspaceData("X", data_x.ToArray());
                                myMatlabWrapper.plot("X", "black", 2);
                            }
                        }
                    }
                }

                myMatlabWrapper.clearWorkspace();
            }
            else
            {
                writeToLogBox("Please add data to plot!");
            }
            setProgressBarValue(0);
            writeProgressInfo("Ready");
        }

        private void tabPage_BaselineMeantime_Enter(object sender, EventArgs e)
        {
            comboBox_BaselineMeantime_Study.Items.Clear();
            comboBox_BaselineMeantime_Group.Items.Clear();
            comboBox_BaselineMeantime_Szenario.Items.Clear();
            comboBox_BaselineMeantime_Subject.Items.Clear();
            comboBox_BaselineMeantime_Turn.Items.Clear();

            string[] studyNames = mySQLWrapper.getStudyNames();
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

            string[] groupNames = mySQLWrapper.getGroupNames(comboBox_BaselineMeantime_Study.SelectedItem.ToString());
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
            comboBox_BaselineMeantime_Turn.Items.Clear(); ;

            string study = comboBox_BaselineMeantime_Study.SelectedItem.ToString();
            string group = comboBox_BaselineMeantime_Group.SelectedItem.ToString();

            string[] szenarioNames = mySQLWrapper.getSzenarioNames(study, group);
            if (szenarioNames != null)
            {
                comboBox_BaselineMeantime_Szenario.Items.AddRange(szenarioNames);
                comboBox_BaselineMeantime_Szenario.SelectedIndex = 0;
            }
        }

        private void comboBox_BaselineMeantime_Szenario_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox_BaselineMeantime_Subject.Items.Clear();
            comboBox_BaselineMeantime_Turn.Items.Clear(); ;

            string study = comboBox_BaselineMeantime_Study.SelectedItem.ToString();
            string group = comboBox_BaselineMeantime_Group.SelectedItem.ToString();
            string szenario = comboBox_BaselineMeantime_Szenario.SelectedItem.ToString();

            SubjectInformationContainer[] subjectNames = mySQLWrapper.getSubjectInformations(study, group, szenario);
            if (subjectNames != null)
            {
                comboBox_BaselineMeantime_Subject.Items.AddRange(subjectNames);
                comboBox_BaselineMeantime_Subject.SelectedIndex = 0;
            }
        }

        private void comboBox_BaselineMeantime_Subject_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox_BaselineMeantime_Turn.Items.Clear(); ;

            string study = comboBox_BaselineMeantime_Study.SelectedItem.ToString();
            string group = comboBox_BaselineMeantime_Group.SelectedItem.ToString();
            string szenario = comboBox_BaselineMeantime_Szenario.SelectedItem.ToString();
            SubjectInformationContainer subject = (SubjectInformationContainer)comboBox_BaselineMeantime_Subject.SelectedItem;

            string[] turnNames = mySQLWrapper.getTurns(study, group, szenario, subject.id);
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
            writeProgressInfo("Getting data...");

            saveFileDialog = new SaveFileDialog();
            saveFileDialog.Reset();
            saveFileDialog.Title = "Save trajectory / velocity file";
            saveFileDialog.AddExtension = true;
            saveFileDialog.DefaultExt = ".csv";
            saveFileDialog.Filter = "DataFiles (*.csv)|.csv";
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
                                    + comboBox_TrajectoryVelocity_IndividualMean.SelectedItem.ToString()
                                    + "-"
                                    + comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString()
                                    + "-data";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (listBox_TrajectoryVelocity_SelectedTrials.Items.Count != 0)
                {
                    if (comboBox_TrajectoryVelocity_IndividualMean.SelectedItem.ToString() == "Individual")
                    {
                        int counter = 0;
                        foreach (TrajectoryVelocityPlotContainer tempContainer in listBox_TrajectoryVelocity_SelectedTrials.Items)
                        {
                            setProgressBarValue((100.0 / listBox_TrajectoryVelocity_SelectedTrials.Items.Count) * counter);
                            counter++;
                            DateTime turnDateTime = mySQLWrapper.getTurnDateTime(tempContainer.Study, tempContainer.Group, tempContainer.Szenario, tempContainer.Subject.id, tempContainer.Turn);
                            foreach (int trial in tempContainer.Trials)
                            {
                                if (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString() == "Trajectory")
                                {
                                    int trialID = mySQLWrapper.getTrailID(tempContainer.Study, tempContainer.Group, tempContainer.Szenario, tempContainer.Subject.id, turnDateTime, tempContainer.Target, trial);
                                    DataSet measureDataSet = mySQLWrapper.getMeasureDataNormalizedDataSet(trialID);

                                    List<string> cache = new List<string>();
                                    cache.Add("Study;Group;Szenario;Subject;Turn;Target;Trial;TimeStamp;PositionCartesianX;PositionCartesianZ");

                                    foreach (DataRow row in measureDataSet.Tables[0].Rows)
                                    {
                                        cache.Add(
                                                    tempContainer.Study 
                                                    + ";" 
                                                    + tempContainer.Group 
                                                    + ";" 
                                                    + tempContainer.Szenario 
                                                    + ";" 
                                                    + tempContainer.Subject 
                                                    + ";" 
                                                    + tempContainer.Turn 
                                                    + ";" 
                                                    + tempContainer.Target 
                                                    + ";" 
                                                    + trial
                                                    + ";"
                                                    + Convert.ToDateTime(row["time_stamp"]).ToString("dd.MM.yyyy HH:mm:ss.fffffff")
                                                    + ";"
                                                    + DoubleConverter.ToExactString(Convert.ToDouble(row["position_cartesian_x"]))
                                                    + ";"
                                                    + DoubleConverter.ToExactString(Convert.ToDouble(row["position_cartesian_z"]))
                                                    );
                                    }

                                    FileStream dataFileStream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
                                    StreamWriter dataFileWriter = new StreamWriter(dataFileStream);

                                    for (int i = 0; i < cache.Count(); i++)
                                    {
                                        dataFileWriter.WriteLine(cache[i]);
                                    }

                                    dataFileWriter.Close();
                                    dataFileStream.Close();
                                }

                                else if (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString() == "Velocity")
                                {
                                    int trialID = mySQLWrapper.getTrailID(tempContainer.Study, tempContainer.Group, tempContainer.Szenario, tempContainer.Subject.id, turnDateTime, tempContainer.Target, trial);
                                    DataSet velocityDataSet = mySQLWrapper.getVelocityDataNormalizedDataSet(trialID);
                                   
                                    List<string> cache = new List<string>();
                                    cache.Add("Study;Group;Szenario;Subject;Turn;Target;Trial;TimeStamp;VelocityXZ");

                                    foreach (DataRow row in velocityDataSet.Tables[0].Rows)
                                    {
                                        cache.Add(
                                                    tempContainer.Study
                                                    + ";"
                                                    + tempContainer.Group
                                                    + ";"
                                                    + tempContainer.Szenario
                                                    + ";"
                                                    + tempContainer.Subject
                                                    + ";"
                                                    + tempContainer.Turn
                                                    + ";"
                                                    + tempContainer.Target
                                                    + ";"
                                                    + trial
                                                    + ";"
                                                    + Convert.ToDateTime(row["time_stamp"]).ToString("dd.MM.yyyy HH:mm:ss.fffffff")
                                                    + ";"
                                                    + DoubleConverter.ToExactString(Convert.ToDouble(Math.Sqrt(Math.Pow(Convert.ToDouble(row["velocity_x"]), 2) + Math.Pow(Convert.ToDouble(row["velocity_z"]), 2))))
                                                    );
                                    }

                                    FileStream dataFileStream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
                                    StreamWriter dataFileWriter = new StreamWriter(dataFileStream);

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
                        List<TrajectoryVelocityPlotContainer> tempTrajectoryVelocityPlotContainerList = new List<TrajectoryVelocityPlotContainer>(listBox_TrajectoryVelocity_SelectedTrials.Items.Cast<TrajectoryVelocityPlotContainer>().ToList());

                        if (tempTrajectoryVelocityPlotContainerList.Select(t => t.Trials.ToArray()).Distinct(new ArrayComparer()).ToList().Count() > 1)
                        {
                            writeToLogBox("Trial selections are not equal!");
                        }
                        else
                        {
                            string[] studyArray = tempTrajectoryVelocityPlotContainerList.Select(t => t.Study).Distinct().ToArray();
                            string[] groupArray = tempTrajectoryVelocityPlotContainerList.Select(t => t.Group).Distinct().ToArray();
                            string[] szenarioArray = tempTrajectoryVelocityPlotContainerList.Select(t => t.Szenario).Distinct().ToArray();
                            SubjectInformationContainer[] subjectArray = tempTrajectoryVelocityPlotContainerList.Select(t => t.Subject).Distinct().ToArray();
                            string[] turnArray = tempTrajectoryVelocityPlotContainerList.Select(t => t.Turn.ToString()).Distinct().ToArray();
                            int[] targetArray = tempTrajectoryVelocityPlotContainerList.Select(t => t.Target).Distinct().ToArray();
                            string trials = tempTrajectoryVelocityPlotContainerList.ElementAt(0).getTrialsString();

                            List<string> cache = new List<string>();
                            if (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString() == "Trajectory")
                            {
                                cache.Add("Study;Group;Szenario;Subject;Turn;Target;Trial;DataPoint;PositionCartesianX;PositionCartesianZ");
                            }
                            else if (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString() == "Velocity")
                            {
                                cache.Add("Study;Group;Szenario;Subject;Turn;Target;Trial;DataPoint;VelocityXZ");
                            }

                            int counter = 0;
                            for (int targetCounter = 0; targetCounter < targetArray.Length; targetCounter++)
                            {
                                DataSet dataSet;
                                int meanCounter = 0;
                                List<double> data_x = new List<double>();
                                List<double> data_z = new List<double>();

                                foreach (TrajectoryVelocityPlotContainer tempContainer in tempTrajectoryVelocityPlotContainerList.Where(t => t.Target == targetArray[targetCounter]))
                                {
                                    setProgressBarValue((100.0 / listBox_TrajectoryVelocity_SelectedTrials.Items.Count) * counter);
                                    counter++;
                                    DateTime turnDateTime = mySQLWrapper.getTurnDateTime(tempContainer.Study, tempContainer.Group, tempContainer.Szenario, tempContainer.Subject.id, tempContainer.Turn);

                                    if (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString() == "Trajectory")
                                    {
                                        foreach (int trial in tempContainer.Trials)
                                        {
                                            int trialID = mySQLWrapper.getTrailID(tempContainer.Study, tempContainer.Group, tempContainer.Szenario, tempContainer.Subject.id, turnDateTime, tempContainer.Target, trial);
                                            dataSet = mySQLWrapper.getMeasureDataNormalizedDataSet(trialID);
                                            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                                            {
                                                DataRow row = dataSet.Tables[0].Rows[i];

                                                if (data_x.Count <= i)
                                                {
                                                    data_x.Add(Convert.ToDouble(row["position_cartesian_x"]));
                                                }
                                                else
                                                {
                                                    data_x[i] += Convert.ToDouble(row["position_cartesian_x"]);
                                                }

                                                if (data_z.Count <= i)
                                                {
                                                    data_z.Add(Convert.ToDouble(row["position_cartesian_z"]));
                                                }
                                                else
                                                {
                                                    data_z[i] += Convert.ToDouble(row["position_cartesian_z"]);
                                                }
                                            }
                                            meanCounter++;
                                        }
                                    }
                                    else if (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString() == "Velocity")
                                    {
                                        foreach (int trial in tempContainer.Trials)
                                        {
                                            int trialID = mySQLWrapper.getTrailID(tempContainer.Study, tempContainer.Group, tempContainer.Szenario, tempContainer.Subject.id, turnDateTime, tempContainer.Target, trial);
                                            dataSet = mySQLWrapper.getVelocityDataNormalizedDataSet(trialID);
                                            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                                            {
                                                DataRow row = dataSet.Tables[0].Rows[i];

                                                if (data_x.Count <= i)
                                                {
                                                    data_x.Add(Math.Sqrt(Math.Pow(Convert.ToDouble(row["velocity_x"]), 2) + Math.Pow(Convert.ToDouble(row["velocity_z"]), 2)));
                                                }
                                                else
                                                {
                                                    data_x[i] += Math.Sqrt(Math.Pow(Convert.ToDouble(row["velocity_x"]), 2) + Math.Pow(Convert.ToDouble(row["velocity_z"]), 2));
                                                }
                                            }
                                            meanCounter++;
                                        }
                                    }
                                }

                                if (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString() == "Trajectory")
                                {
                                    for (int i = 0; i < data_x.Count; i++)
                                    {
                                        data_x[i] /= meanCounter;
                                        data_z[i] /= meanCounter;
                                    }

                                    for (int i = 0; i < data_x.Count; i++)
                                    {
                                        cache.Add(
                                                    String.Join(",", studyArray)
                                                    + ";"
                                                    + String.Join(",", groupArray)
                                                    + ";"
                                                    + String.Join(",", szenarioArray)
                                                    + ";"
                                                    + String.Join<SubjectInformationContainer>(",", subjectArray)
                                                    + ";"
                                                    + String.Join(",", turnArray)
                                                    + ";"
                                                    + targetArray[targetCounter]
                                                    + ";"
                                                    + trials
                                                    + ";"
                                                    + i
                                                    + ";"
                                                    + DoubleConverter.ToExactString(data_x[i])
                                                    + ";"
                                                    + DoubleConverter.ToExactString(data_z[i])
                                                    );
                                    }

                                    FileStream dataFileStream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
                                    StreamWriter dataFileWriter = new StreamWriter(dataFileStream);

                                    for (int i = 0; i < cache.Count(); i++)
                                    {
                                        dataFileWriter.WriteLine(cache[i]);
                                    }

                                    dataFileWriter.Close();
                                    dataFileStream.Close();
                                }
                                else if (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString() == "Velocity")
                                {
                                    for (int i = 0; i < data_x.Count; i++)
                                    {
                                        data_x[i] /= meanCounter;
                                    }

                                    for (int i = 0; i < data_x.Count; i++)
                                    {
                                        cache.Add(
                                                    String.Join(",", studyArray)
                                                    + ";"
                                                    + String.Join(",", groupArray)
                                                    + ";"
                                                    + String.Join(",", szenarioArray)
                                                    + ";"
                                                    + String.Join<SubjectInformationContainer>(",", subjectArray)
                                                    + ";"
                                                    + String.Join(",", turnArray)
                                                    + ";"
                                                    + targetArray[targetCounter]
                                                    + ";"
                                                    + trials
                                                    + ";"
                                                    + i
                                                    + ";"
                                                    + DoubleConverter.ToExactString(data_x[i])
                                                    );
                                    }

                                    FileStream dataFileStream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
                                    StreamWriter dataFileWriter = new StreamWriter(dataFileStream);

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

                    myMatlabWrapper.clearWorkspace();
                }
                else
                {
                    writeToLogBox("Please add data to export!");
                }
            }
            writeProgressInfo("Ready");
            setProgressBarValue(0);
        }

        private void button_BaselineMeantime_ExportBaseline_Click(object sender, EventArgs e)
        {
            saveFileDialog = new SaveFileDialog();
            saveFileDialog.Reset();
            saveFileDialog.Title = "Save trajectory / velocity file";
            saveFileDialog.AddExtension = true;
            saveFileDialog.DefaultExt = ".csv";
            saveFileDialog.Filter = "DataFiles (*.csv)|.csv";
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
                                    + comboBox_BaselineMeantime_Subject.SelectedItem.ToString()
                                    + "trajectory-baseline-data";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string study = comboBox_BaselineMeantime_Study.SelectedItem.ToString();
                string group = comboBox_BaselineMeantime_Group.SelectedItem.ToString();
                string szenario = comboBox_BaselineMeantime_Szenario.SelectedItem.ToString();
                SubjectInformationContainer subject = (SubjectInformationContainer)comboBox_BaselineMeantime_Subject.SelectedItem;

                DataSet baseline = mySQLWrapper.getBaselineDataSet(study, group, szenario, subject.id);

                List<object[]> baselineData = new List<object[]>();

                foreach (DataRow row in baseline.Tables[0].Rows)
                {
                    baselineData.Add(new object[] { Convert.ToDouble(row["baseline_position_cartesian_x"]), Convert.ToDouble(row["baseline_position_cartesian_z"]), Convert.ToInt32(row["target_number"]) });
                }

                int[] targetNumberArray = baselineData.Select(t => Convert.ToInt32(t[2])).Distinct().ToArray();

                List<string> cache = new List<string>();
                cache.Add("Study;Group;Szenario;Subject;Target;DataPoint;PositionCartesianX;PositionCartesianZ");

                for (int i = 0; i < targetNumberArray.Length; i++)
                {
                    double[] tempX = baselineData.Where(t => Convert.ToInt32(t[2]) == targetNumberArray[i]).Select(t => Convert.ToDouble(t[0])).ToArray();
                    double[] tempZ = baselineData.Where(t => Convert.ToInt32(t[2]) == targetNumberArray[i]).Select(t => Convert.ToDouble(t[1])).ToArray();

                    for (int j = 0; j < tempX.Length; j++)
                    {
                        cache.Add(
                                    study
                                    + ";"
                                    + group
                                    + ";"
                                    + szenario
                                    + ";"
                                    + subject
                                    + ";"
                                    + targetNumberArray[i]
                                    + ";"
                                    + j
                                    + ";"
                                    + DoubleConverter.ToExactString(tempX[j])
                                    + ";"
                                    + DoubleConverter.ToExactString(tempZ[j])
                                    );
                    }
                }

                FileStream dataFileStream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
                StreamWriter dataFileWriter = new StreamWriter(dataFileStream);

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
            saveFileDialog.Title = "Save trajectory / velocity file";
            saveFileDialog.AddExtension = true;
            saveFileDialog.DefaultExt = ".csv";
            saveFileDialog.Filter = "DataFiles (*.csv)|.csv";
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
                                    + comboBox_BaselineMeantime_Subject.SelectedItem.ToString()
                                    + "-szenario-mean-time-data";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string study = comboBox_BaselineMeantime_Study.SelectedItem.ToString();
                string group = comboBox_BaselineMeantime_Group.SelectedItem.ToString();
                string szenario = comboBox_BaselineMeantime_Szenario.SelectedItem.ToString();
                SubjectInformationContainer subject = (SubjectInformationContainer)comboBox_BaselineMeantime_Subject.SelectedItem;
                int turn = Convert.ToInt32(comboBox_BaselineMeantime_Turn.SelectedItem.ToString().Substring("Turn".Length));
                DateTime turnDateTime = mySQLWrapper.getTurnDateTime(study, group, szenario, subject.id, turn);

                DataSet meanTimeDataSet = mySQLWrapper.getMeanTimeDataSet(study, group, szenario, subject.id, turnDateTime);

                List<TimeSpan> meanTimeList = new List<TimeSpan>();
                List<TimeSpan> meanTimeStdList = new List<TimeSpan>();
                List<int> targetList = new List<int>();

                List<string> cache = new List<string>();
                cache.Add("Study;Group;Szenario;Subject;Turn;Target;SzenarioMeanTime;SzenarioMeanTimeStd");

                foreach (DataRow row in meanTimeDataSet.Tables[0].Rows)
                {
                    cache.Add(
                                    study
                                    + ";"
                                    + group
                                    + ";"
                                    + szenario
                                    + ";"
                                    + subject
                                    + ";"
                                    + turn
                                    + ";"
                                    + Convert.ToInt32(row["target_number"])
                                    + ";"
                                    + DoubleConverter.ToExactString(TimeSpan.Parse(Convert.ToString(row["szenario_mean_time"])).TotalMilliseconds)
                                    + ";"
                                    + DoubleConverter.ToExactString(TimeSpan.Parse(Convert.ToString(row["szenario_mean_time_std"])).TotalMilliseconds)
                                    );
                }

                FileStream dataFileStream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
                StreamWriter dataFileWriter = new StreamWriter(dataFileStream);

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
            writeProgressInfo("Deleting measure file...");
            mySQLWrapper.deleteMeasureFile(Convert.ToInt32(textBox_DataManipulation_MeasureFileID));
            writeProgressInfo("Ready");
        }

        private void button_Debug_CleanOrphanedEntries_Click(object sender, EventArgs e)
        {
            writeProgressInfo("Cleaning orphaned entries...");
            mySQLWrapper.cleanOrphanedEntries();
            writeProgressInfo("Ready");
        }

        private void button_Start_SelectDatabase_Click(object sender, EventArgs e)
        {
            mySQLWrapper.setDatabase(comboBox_Start_Database.SelectedItem.ToString());

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
            SubjectInformationContainer subject = (SubjectInformationContainer)comboBox_BaselineMeantime_Subject.SelectedItem;

            myMatlabWrapper.createVelocityFigure("Velocity baseline plot", 101);
            myMatlabWrapper.drawTargets(0.005, 0.1, 0, 0);

            DataSet baseline = mySQLWrapper.getBaselineDataSet(study, group, szenario, subject.id);

            List<object[]> baselineData = new List<object[]>();

            foreach (DataRow row in baseline.Tables[0].Rows)
            {
                baselineData.Add(new object[] { Convert.ToDouble(row["baseline_velocity_x"]), Convert.ToDouble(row["baseline_velocity_z"]), Convert.ToInt32(row["target_number"]) });
            }

            int[] targetNumberArray = baselineData.Select(t => Convert.ToInt32(t[2])).Distinct().ToArray();

            for (int i = 0; i < targetNumberArray.Length; i++)
            {
                double[] tempXZ = baselineData.Where(t => Convert.ToInt32(t[2]) == targetNumberArray[i]).Select(t => Math.Sqrt(Math.Pow(Convert.ToDouble(t[0]), 2) + Math.Pow(Convert.ToDouble(t[1]), 2))).ToArray();
                myMatlabWrapper.setWorkspaceData("XZ", tempXZ);
                myMatlabWrapper.plot("XZ",2);
            }

            myMatlabWrapper.clearWorkspace();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            saveFileDialog = new SaveFileDialog();
            saveFileDialog.Reset();
            saveFileDialog.Title = "Save trajectory / velocity file";
            saveFileDialog.AddExtension = true;
            saveFileDialog.DefaultExt = ".csv";
            saveFileDialog.Filter = "DataFiles (*.csv)|.csv";
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
                                    + comboBox_BaselineMeantime_Subject.SelectedItem.ToString()
                                    + "-velocity-baseline-data";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string study = comboBox_BaselineMeantime_Study.SelectedItem.ToString();
                string group = comboBox_BaselineMeantime_Group.SelectedItem.ToString();
                string szenario = comboBox_BaselineMeantime_Szenario.SelectedItem.ToString();
                SubjectInformationContainer subject = (SubjectInformationContainer)comboBox_BaselineMeantime_Subject.SelectedItem;

                DataSet baseline = mySQLWrapper.getBaselineDataSet(study, group, szenario, subject.id);

                List<object[]> baselineData = new List<object[]>();

                foreach (DataRow row in baseline.Tables[0].Rows)
                {
                    baselineData.Add(new object[] { Convert.ToDouble(row["baseline_velocity_x"]), Convert.ToDouble(row["baseline_velocity_z"]), Convert.ToInt32(row["target_number"]) });
                }

                int[] targetNumberArray = baselineData.Select(t => Convert.ToInt32(t[2])).Distinct().ToArray();

                List<string> cache = new List<string>();
                cache.Add("Study;Group;Szenario;Subject;Target;DataPoint;VelocityX;VelocityZ;VelocityXZ");

                for (int i = 0; i < targetNumberArray.Length; i++)
                {
                    double[] tempX = baselineData.Where(t => Convert.ToInt32(t[2]) == targetNumberArray[i]).Select(t => Convert.ToDouble(t[0])).ToArray();
                    double[] tempZ = baselineData.Where(t => Convert.ToInt32(t[2]) == targetNumberArray[i]).Select(t => Convert.ToDouble(t[1])).ToArray();
                    double[] tempXZ = baselineData.Where(t => Convert.ToInt32(t[2]) == targetNumberArray[i]).Select(t => Math.Sqrt(Math.Pow(Convert.ToDouble(t[0]), 2) + Math.Pow(Convert.ToDouble(t[1]), 2))).ToArray();

                    for (int j = 0; j < tempX.Length; j++)
                    {
                        cache.Add(
                                    study
                                    + ";"
                                    + group
                                    + ";"
                                    + szenario
                                    + ";"
                                    + subject
                                    + ";"
                                    + targetNumberArray[i]
                                    + ";"
                                    + j
                                    + ";"
                                    + DoubleConverter.ToExactString(tempX[j])
                                    + ";"
                                    + DoubleConverter.ToExactString(tempZ[j])
                                    );
                    }
                }

                FileStream dataFileStream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
                StreamWriter dataFileWriter = new StreamWriter(dataFileStream);

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