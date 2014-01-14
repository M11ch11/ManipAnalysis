using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using ManipAnalysis.Container;

namespace ManipAnalysis
{
    public partial class ManipAnalysisGui : Form
    {
        private ManipAnalysisFunctions _manipAnalysisFunctions;

        public ManipAnalysisGui()
        {
            InitializeComponent();

            checkBox_Start_ManualMode.Enabled = false;
            tabControl.TabPages.Remove(tabPage_VisualizationExport);
            tabControl.TabPages.Remove(tabPage_ImportCalculations);
            tabControl.TabPages.Remove(tabPage_Debug);

            label_Impressum_Text.Text += Assembly.GetExecutingAssembly().GetName().Version;
        }

        public void SetManipAnalysisModel(ManipAnalysisFunctions manipAnalysisModel)
        {
            _manipAnalysisFunctions = manipAnalysisModel;
        }

        public void WriteToLogBox(string text)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new LogBoxCallbackAddString(WriteToLogBox), text);
            }
            else
            {
                string[] textArray = text.Split('\n');

                for (int i = 0; i < textArray.Length; i++)
                {
                    if (i == 0)
                    {
                        listBox_LogBox.Items.Add("[" + DateTime.Now + "] " + textArray[i]);
                    }
                    else
                    {
                        listBox_LogBox.Items.Add(textArray[i]);
                    }

                    if (listBox_LogBox.HorizontalExtent <
                        TextRenderer.MeasureText(listBox_LogBox.Items[listBox_LogBox.Items.Count - 1].ToString(),
                            listBox_LogBox.Font, listBox_LogBox.ClientSize,
                            TextFormatFlags.NoPrefix).Width)
                    {
                        listBox_LogBox.HorizontalExtent =
                            TextRenderer.MeasureText(listBox_LogBox.Items[listBox_LogBox.Items.Count - 1].ToString(),
                                listBox_LogBox.Font, listBox_LogBox.ClientSize,
                                TextFormatFlags.NoPrefix).Width;
                    }
                }
                listBox_LogBox.TopIndex = listBox_LogBox.Items.Count - 1;
                listBox_LogBox.Invalidate();
                listBox_LogBox.Update();
                listBox_LogBox.Refresh();
                Application.DoEvents();
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
            filesList.RemoveAll(t => (t.Name.Contains("Szenario40")));
            filesList.RemoveAll(t => (t.Name.Contains("Szenario41")));


            for (int i = 0; i < filesList.Count; i++)
            {
                if (filesList[i].Name.Count(t => t == '-') == 6)
                {
                    string tempFileHash = Md5.ComputeHash(filesList[i].FullName);

                    if (!_manipAnalysisFunctions.CheckIfMeasureFileHashAlreadyExists(tempFileHash))
                    {
                        if (!listBox_Import_SelectedMeasureFiles.Items.Contains(filesList[i].FullName))
                        {
                            listBox_Import_SelectedMeasureFiles.Items.Add(filesList[i].FullName);
                        }
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

            WriteProgressInfo("Checking for new files...");

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
                filesList.RemoveAll(t => (t.Name.Contains("Szenario40")));
                filesList.RemoveAll(t => (t.Name.Contains("Szenario41")));

                bool isValid = true;

                for (int i = 0; i < filesList.Count; i++)
                {
                    FileInfo fi = filesList[i];
                    //if (fi.Name.Count(t => t == '-') == 5)  // Study 1
                    if (fi.Name.Count(t => t == '-') == 6) // Study 2
                    {
                        string tempFileHash = Md5.ComputeHash(fi.FullName);

                        if (!_manipAnalysisFunctions.CheckIfMeasureFileHashAlreadyExists(tempFileHash))
                        {
                            if (!listBox_Import_SelectedMeasureFiles.Items.Contains(fi.FullName))
                            {
                                listBox_Import_SelectedMeasureFiles.Items.Add(fi.FullName);
                            }
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

            WriteProgressInfo("Ready.");
        }

        private void button_ShowMatlabWindow_Click(object sender, EventArgs e)
        {
            _manipAnalysisFunctions.ToggleMatlabCommandWindow();
        }

        private void button_ShowMatlabWorkspace_Click(object sender, EventArgs e)
        {
            _manipAnalysisFunctions.ShowMatlabWorkspace();
        }

        private void checkBox_ManualMode_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_Start_ManualMode.Checked)
            {
                tabControl.TabPages.Remove(tabPage_Impressum);
                if (Environment.MachineName != textBox_Start_SqlServer.Text)
                {
                    MessageBox.Show(@"Import and Calculations only possible when running on ManipServer!");
                }
                else
                {
                    tabControl.TabPages.Add(tabPage_ImportCalculations);
                }
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
                _manipAnalysisFunctions.InitializeSqlDatabase();
            }
        }

        private void button_PlotBaseline_Click(object sender, EventArgs e)
        {
            _manipAnalysisFunctions.PlotBaseline(comboBox_Others_Study.SelectedItem.ToString(),
                comboBox_Others_Group.SelectedItem.ToString(),
                comboBox_Others_Szenario.SelectedItem.ToString(),
                (SubjectInformationContainer)
                    comboBox_Others_Subject.SelectedItem);
        }

        public void EnableTabPages(bool enable)
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

        public void SetProgressBarValue(double value)
        {
            if (progressBar.InvokeRequired)
            {
                ProgressBarCallback setProgressBarValue = SetProgressBarValue;
                progressBar.Invoke(setProgressBarValue, new object[] {value});
            }
            else
            {
                progressBar.Value = Convert.ToInt32(value);
                progressBar.Invalidate();
                progressBar.Update();
                progressBar.Refresh();
                Application.DoEvents();
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
                label_ProgressInfo.Invalidate();
                label_ProgressInfo.Update();
                label_ProgressInfo.Refresh();
                Application.DoEvents();
            }
        }

        private void comboBox_StatisticPlots_Study_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox_DescriptiveStatistic1_Groups.Items.Clear();
            comboBox_DescriptiveStatistic1_Szenario.Items.Clear();
            listBox_DescriptiveStatistic1_Subjects.Items.Clear();
            listBox_DescriptiveStatistic1_Turns.Items.Clear();
            listBox_DescriptiveStatistic1_Trials.Items.Clear();

            IEnumerable<string> groupNames =
                _manipAnalysisFunctions.GetGroups(comboBox_DescriptiveStatistic1_Study.SelectedItem.ToString());
            if (groupNames != null)
            {
                listBox_DescriptiveStatistic1_Groups.Items.AddRange(groupNames.ToArray());
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

                IEnumerable<string> szenarioIntersect = _manipAnalysisFunctions.GetSzenarios(study, groups[0]);
                if (szenarioIntersect != null)
                {
                    for (int i = 1; i < groups.Length; i++)
                    {
                        szenarioIntersect =
                            szenarioIntersect.Intersect(_manipAnalysisFunctions.GetSzenarios(study, groups[i]))
                                .ToArray();
                    }

                    comboBox_DescriptiveStatistic1_Szenario.Items.AddRange(szenarioIntersect.ToArray());
                    comboBox_DescriptiveStatistic1_Szenario.SelectedIndex = 0;
                }
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
                IEnumerable<SubjectInformationContainer> tempSubjects = _manipAnalysisFunctions.GetSubjects(study,
                    groups[i],
                    szenario);
                if (tempSubjects != null)
                {
                    listBox_DescriptiveStatistic1_Subjects.Items.AddRange(tempSubjects.ToArray());
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
            SubjectInformationContainer[] subjects =
                listBox_DescriptiveStatistic1_Subjects.SelectedItems.Cast<SubjectInformationContainer>().ToArray();

            string[] turnIntersect = null;
            for (int i = 0; i < groups.Length; i++)
            {
                for (int j = 0; j < subjects.Length; j++)
                {
                    IEnumerable<string> tempTurnString = _manipAnalysisFunctions.GetTurns(study, groups[i], szenario,
                        subjects[j]);

                    if (tempTurnString != null)
                    {
                        if (turnIntersect == null)
                        {
                            turnIntersect = tempTurnString.ToArray();
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
            bool showErrorclampTrials = checkBox_DescriptiveStatistic1_ShowErrorclampTrials.Checked;
            bool showErrorclampTrialsExclusivly = checkBox_DescriptiveStatistic1_ShowErrorclampTrialsExclusivly.Checked;

            IEnumerable<string> szenarioTrialNames = _manipAnalysisFunctions.GetTrialsOfSzenario(study, szenario,
                showCatchTrials,
                showCatchTrialsExclusivly,
                showErrorclampTrials,
                showErrorclampTrialsExclusivly);

            if (szenarioTrialNames != null)
            {
                listBox_DescriptiveStatistic1_Trials.Items.AddRange(szenarioTrialNames.ToArray());
                listBox_DescriptiveStatistic1_Trials.SelectedIndex = 0;
            }
        }

        private void button_StatisticPlots_AddSelected_Click(object sender, EventArgs e)
        {
            if (listBox_DescriptiveStatistic1_Trials.SelectedItems.Count > 0)
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
                            if (_manipAnalysisFunctions.GetTurns(study, group, szenario, subject) != null)
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
            int pdTime = -1;
            if (comboBox_DescriptiveStatistic1_DataTypeSelect.SelectedItem.ToString() == "Perpendicular distance ?ms - Abs" || 
                comboBox_DescriptiveStatistic1_DataTypeSelect.SelectedItem.ToString() == "Perpendicular distance ?ms - Sign")
            {
                var inputForm = new PerpendicularDisplacementTimeInputForm();
                if (inputForm.ShowDialog(this) == DialogResult.OK)
                {
                    pdTime = inputForm.getMilliseconds();
                    inputForm.Dispose();
                }
            }

            _manipAnalysisFunctions.PlotDescriptiveStatistic1(
                listBox_DescriptiveStatistic1_SelectedTrials.Items.Cast<StatisticPlotContainer>(),
                comboBox_DescriptiveStatistic1_DataTypeSelect.SelectedItem.ToString(),
                textBox_DescriptiveStatistic1_FitEquation.Text,
                pdTime,
                checkBox_DescriptiveStatistic1_PlotFit.Checked,
                checkBox_DescriptiveStatistic1_PlotErrorbars.Checked);
        }

        private void button_StatisticPlots_AddAll_Click(object sender, EventArgs e)
        {
            if (listBox_DescriptiveStatistic1_Trials.SelectedItems.Count > 0)
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
                            if (_manipAnalysisFunctions.GetTurns(study, group, szenario, subject) != null)
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
            IEnumerable<object[]> faultyTrialInfo = _manipAnalysisFunctions.GetFaultyTrialInformation();

            if (faultyTrialInfo != null)
            {
                List<object[]> faultyTrialInfoList = faultyTrialInfo.ToList();

                if (faultyTrialInfoList.Any())
                {
                    List<string[]> cache = faultyTrialInfoList.Select(t => new[]
                    {
                        Convert.ToString(t[0]), Convert.ToString(t[1]), Convert.ToString(t[2]),
                        Convert.ToString(t[3]), Convert.ToString(t[4]), Convert.ToString(t[5]),
                        Convert.ToString(Convert.ToDateTime(t[6])), Convert.ToString(Convert.ToInt32(t[7]))
                    }).ToList();

                    string output =
                        cache.OrderBy(t => t[4])
                            .Select(
                                t =>
                                    " TrialID " + t[0] + " - FileID " + t[1] + " -" + t[2] + " - " + t[3] +
                                    " - SubjectID " + t[4] + " - " + t[5] + " - " + t[6] + " - Trial " + t[7])
                            .ToArray()
                            .Aggregate(
                                "\n------------------------------------------------------- Faulty trial list -------------------------------------------------------\n",
                                (current, line) => current + (line + "\n"));
                    output +=
                        "-----------------------------------------------------------------------------------------------------------------------------------";

                    WriteToLogBox(output);
                }
            }
            else
            {
                WriteToLogBox("No faulty Trials!");
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

            IEnumerable<string> studyNames = _manipAnalysisFunctions.GetStudys();
            if (studyNames != null)
            {
                comboBox_DescriptiveStatistic1_Study.Items.AddRange(studyNames.ToArray());
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

            IEnumerable<string> groupNames =
                _manipAnalysisFunctions.GetGroups(comboBox_DescriptiveStatistic2_Study.SelectedItem.ToString());

            if (groupNames != null)
            {
                listBox_DescriptiveStatistic2_Groups.Items.AddRange(groupNames.ToArray());
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
                IEnumerable<string> szenarioIntersect = _manipAnalysisFunctions.GetSzenarios(study, groups[0]);

                if (szenarioIntersect != null)
                {
                    for (int i = 1; i < groups.Length; i++)
                    {
                        szenarioIntersect =
                            szenarioIntersect.Intersect(_manipAnalysisFunctions.GetSzenarios(study, groups[i]));
                    }

                    comboBox_DescriptiveStatistic2_Szenario.Items.AddRange(szenarioIntersect.ToArray());
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
                IEnumerable<SubjectInformationContainer> tempSubjects = _manipAnalysisFunctions.GetSubjects(study,
                    groups[i],
                    szenario);
                if (tempSubjects != null)
                {
                    listBox_DescriptiveStatistic2_Subjects.Items.AddRange(tempSubjects.ToArray());
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
                    IEnumerable<string> tempTurnString = _manipAnalysisFunctions.GetTurns(study, groups[i], szenario,
                        subjects[j]);

                    if (tempTurnString != null)
                    {
                        if (turnIntersect == null)
                        {
                            turnIntersect = tempTurnString.ToArray();
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
            bool showErrorclampTrials = checkBox_DescriptiveStatistic2_ShowErrorclampTrials.Checked;
            bool showErrorclampTrialsExclusivly = checkBox_DescriptiveStatistic2_ShowErrorclampTrialsExclusivly.Checked;

            IEnumerable<string> szenarioTrialNames = _manipAnalysisFunctions.GetTrialsOfSzenario(study, szenario,
                showCatchTrials,
                showCatchTrialsExclusivly,
                showErrorclampTrials,
                showErrorclampTrialsExclusivly);

            if (szenarioTrialNames != null)
            {
                listBox_DescriptiveStatistic2_Trials.Items.AddRange(szenarioTrialNames.ToArray());
                listBox_DescriptiveStatistic2_Trials.SelectedIndex = 0;
            }
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

            IEnumerable<string> studyNames = _manipAnalysisFunctions.GetStudys();

            if (studyNames != null)
            {
                comboBox_DescriptiveStatistic2_Study.Items.AddRange(studyNames.ToArray());
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
                    _manipAnalysisFunctions.ExportDescriptiveStatistic2Data(
                        listBox_DescriptiveStatistic2_SelectedTrials.Items.Cast<StatisticPlotContainer>(),
                        comboBox_DescriptiveStatistic2_DataTypeSelect.SelectedItem.ToString(), saveFileDialog.FileName);
                }
            }
            WriteProgressInfo("Ready");
            SetProgressBarValue(0);
        }

        private void button_DescriptiveStatistic1_ExportData_Click(object sender, EventArgs e)
        {
            int pdTime = -1;
            string statisticType = comboBox_DescriptiveStatistic1_DataTypeSelect.SelectedItem.ToString();
            if (comboBox_DescriptiveStatistic1_DataTypeSelect.SelectedItem.ToString() == "Perpendicular distance ?ms - Abs" ||
                comboBox_DescriptiveStatistic1_DataTypeSelect.SelectedItem.ToString() == "Perpendicular distance ?ms - Sign")
            {
                var inputForm = new PerpendicularDisplacementTimeInputForm();
                if (inputForm.ShowDialog(this) == DialogResult.OK)
                {
                    pdTime = inputForm.getMilliseconds();
                    statisticType = comboBox_DescriptiveStatistic1_DataTypeSelect.SelectedItem.ToString().Replace("?", pdTime.ToString());
                    inputForm.Dispose();
                }
            }

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
                                      + statisticType
                                      + "-data";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (listBox_DescriptiveStatistic1_SelectedTrials.Items.Count > 0)
                {
                    _manipAnalysisFunctions.ExportDescriptiveStatistic1Data(
                        listBox_DescriptiveStatistic1_SelectedTrials.Items.Cast<StatisticPlotContainer>(),
                        comboBox_DescriptiveStatistic1_DataTypeSelect.SelectedItem.ToString(),
                        pdTime,
                        saveFileDialog.FileName);
                }
            }
        }

        private void button_Debug_ShowMatlabFiles_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\MatlabFiles");
            }
            catch (Exception ex)
            {
                WriteToLogBox(ex.ToString());
            }
        }

        private void checkBox_Cancel_CheckedChanged(object sender, EventArgs e)
        {
            TaskManager.Cancel = checkBox_Cancel.Checked;
            if (checkBox_Cancel.Checked)
            {
                checkBox_Cancel.BackColor = Color.Red;
                checkBox_Cancel.ForeColor = Color.White;
            }
            else
            {
                checkBox_Cancel.BackColor = SystemColors.Control;
                checkBox_Cancel.ForeColor = Color.Black;
            }
        }

        private void checkBox_PauseThread_CheckedChanged(object sender, EventArgs e)
        {
            TaskManager.Pause = checkBox_PauseThread.Checked;
            if (checkBox_PauseThread.Checked)
            {
                checkBox_PauseThread.BackColor = Color.Red;
                checkBox_PauseThread.ForeColor = Color.White;
            }
            else
            {
                checkBox_PauseThread.BackColor = SystemColors.Control;
                checkBox_PauseThread.ForeColor = Color.Black;
            }
        }

        private void button_DataManipulation_UpdateGroupID_Click(object sender, EventArgs e)
        {
            _manipAnalysisFunctions.ChangeGroupId(
                Convert.ToInt32(textBox_DataManipulation_OldGroupID.Text),
                Convert.ToInt32(textBox_DataManipulation_NewGroupID.Text)
                );
        }

        private void button_DataManipulation_UpdateSubjectID_Click(object sender, EventArgs e)
        {
            _manipAnalysisFunctions.ChangeSubjectId(
                Convert.ToInt32(textBox_DataManipulation_OldSubjectID.Text),
                Convert.ToInt32(textBox_DataManipulation_NewSubjectID.Text)
                );
        }

        private void button_DataManipulation_UpdateGroupName_Click(object sender, EventArgs e)
        {
            _manipAnalysisFunctions.ChangeGroupGroupName(
                Convert.ToInt32(textBox_DataManipulation_GroupID.Text),
                textBox_DataManipulation_NewGroupName.Text
                );
        }

        private void button_DataManipulation_UpdateSubjectName_Click(object sender, EventArgs e)
        {
            _manipAnalysisFunctions.ChangeSubjectSubjectName(
                Convert.ToInt32(textBox_DataManipulation_SubjectID.Text),
                textBox_DataManipulation_NewSubjectName.Text
                );
        }

        private void button_DataManipulation_UpdateSubjectSubjectID_Click(object sender, EventArgs e)
        {
            _manipAnalysisFunctions.ChangeSubjectSubjectId(
                Convert.ToInt32(textBox_DataManipulation_SubjectIdId.Text),
                textBox_DataManipulation_NewSubjectSubjectID.Text
                );
        }

        private void button_ImportMeasureFiles_Click(object sender, EventArgs e)
        {
            _manipAnalysisFunctions.ImportMeasureFiles(listBox_Import_SelectedMeasureFiles.Items.Cast<string>(),
                Convert.ToInt32(textBox_Import_SamplesPerSec.Text),
                Convert.ToInt32(textBox_Import_FilterOrder.Text),
                Convert.ToInt32(textBox_Import_CutoffFreqPosition.Text),
                Convert.ToInt32(textBox_Import_CutoffFreqForce.Text),
                Convert.ToInt32(textBox_Import_PercentPeakVelocity.Text),
                Convert.ToInt32(textBox_Import_NewSampleCount.Text));
        }

        private void button_CalculateStatistics_Click(object sender, EventArgs e)
        {
            _manipAnalysisFunctions.CalculateStatistics();
        }

        private void button_FixBrokenTrials_Click(object sender, EventArgs e)
        {
            _manipAnalysisFunctions.FixBrokenTrials();
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

            IEnumerable<string> studyNames = _manipAnalysisFunctions.GetStudys();
            if (studyNames != null)
            {
                comboBox_TrajectoryVelocity_Study.Items.AddRange(studyNames.ToArray());
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

            IEnumerable<string> groupNames =
                _manipAnalysisFunctions.GetGroups(comboBox_TrajectoryVelocity_Study.SelectedItem.ToString());
            if (groupNames != null)
            {
                listBox_TrajectoryVelocity_Groups.Items.AddRange(groupNames.ToArray());
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

                IEnumerable<string> szenarioIntersect = _manipAnalysisFunctions.GetSzenarios(study, groups[0]);
                if (szenarioIntersect != null)
                {
                    for (int i = 1; i < groups.Length; i++)
                    {
                        szenarioIntersect =
                            szenarioIntersect.Intersect(_manipAnalysisFunctions.GetSzenarios(study, groups[i]));
                    }

                    comboBox_TrajectoryVelocity_Szenario.Items.AddRange(szenarioIntersect.ToArray());
                    comboBox_TrajectoryVelocity_Szenario.SelectedIndex = 0;
                }
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
                IEnumerable<SubjectInformationContainer> subjects = _manipAnalysisFunctions.GetSubjects(study, groups[i],
                    szenario);
                if (subjects != null)
                {
                    listBox_TrajectoryVelocity_Subjects.Items.AddRange(subjects.ToArray());
                }
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
                    IEnumerable<string> tempTurnString = _manipAnalysisFunctions.GetTurns(study, groups[i], szenario,
                        subjects[j]);

                    if (tempTurnString != null)
                    {
                        if (turnIntersect == null)
                        {
                            turnIntersect = tempTurnString.ToArray();
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

            IEnumerable<string> targets = _manipAnalysisFunctions.GetTargets(study, szenario);
            IEnumerable<string> trials = _manipAnalysisFunctions.GetTrials(study, szenario);

            if (targets != null)
            {
                listBox_TrajectoryVelocity_Targets.Items.AddRange(targets.OrderBy(t => t).ToArray());
                listBox_TrajectoryVelocity_Targets.SelectedIndex = 0;
            }

            if (trials != null)
            {
                listBox_TrajectoryVelocity_Trials.Items.AddRange(trials.OrderBy(t => t).ToArray());
                listBox_TrajectoryVelocity_Trials.SelectedIndex = 0;
            }
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
                                if (_manipAnalysisFunctions.GetTurns(study, group, szenario, subject) != null)
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
                                if (_manipAnalysisFunctions.GetTurns(study, group, szenario, subject) != null)
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
                switch (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString())
                {
                    case "Velocity":
                        _manipAnalysisFunctions.PlotVelocity(
                            listBox_TrajectoryVelocity_SelectedTrials.Items.Cast<TrajectoryVelocityPlotContainer>(),
                            comboBox_TrajectoryVelocity_IndividualMean.SelectedItem.ToString(),
                            checkBox_TrajectoryVelocity_ShowCatchTrials.Checked,
                            checkBox_TrajectoryVelocity_ShowCatchTrialsExclusivly.Checked,
                            checkBox_TrajectoryVelocity_ShowErrorclampTrials.Checked,
                            checkBox_TrajectoryVelocity_ShowErrorclampTrialsExclusivly.Checked);
                        break;

                    case "Trajectory":
                        _manipAnalysisFunctions.PlotTrajectory(
                            listBox_TrajectoryVelocity_SelectedTrials.Items.Cast<TrajectoryVelocityPlotContainer>(),
                            comboBox_TrajectoryVelocity_IndividualMean.SelectedItem.ToString(),
                            checkBox_TrajectoryVelocity_ShowCatchTrials.Checked,
                            checkBox_TrajectoryVelocity_ShowCatchTrialsExclusivly.Checked,
                            checkBox_TrajectoryVelocity_ShowErrorclampTrials.Checked,
                            checkBox_TrajectoryVelocity_ShowErrorclampTrialsExclusivly.Checked,
                            checkBox_TrajectoryVelocity_ShowForceVectors.Checked,
                            checkBox_TrajectoryVelocity_ShowPDForceVectors.Checked);
                        break;
                }
            }
            else
            {
                WriteToLogBox("Please add data to plot!");
            }
        }

        private void tabPage_Others_Enter(object sender, EventArgs e)
        {
            comboBox_Others_Study.Items.Clear();
            comboBox_Others_Group.Items.Clear();
            comboBox_Others_Szenario.Items.Clear();
            comboBox_Others_Subject.Items.Clear();
            comboBox_Others_Turn.Items.Clear();

            IEnumerable<string> studyNames = _manipAnalysisFunctions.GetStudys();
            if (studyNames != null)
            {
                comboBox_Others_Study.Items.AddRange(studyNames.ToArray());
                comboBox_Others_Study.SelectedIndex = 0;
            }
        }

        private void comboBox_Others_Study_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox_Others_Group.Items.Clear();
            comboBox_Others_Szenario.Items.Clear();
            comboBox_Others_Subject.Items.Clear();
            comboBox_Others_Turn.Items.Clear();

            IEnumerable<string> groupNames =
                _manipAnalysisFunctions.GetGroups(comboBox_Others_Study.SelectedItem.ToString());
            if (groupNames != null)
            {
                comboBox_Others_Group.Items.AddRange(groupNames.ToArray());
                comboBox_Others_Group.SelectedIndex = 0;
            }
        }

        private void comboBox_Others_Group_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox_Others_Szenario.Items.Clear();
            comboBox_Others_Subject.Items.Clear();
            comboBox_Others_Turn.Items.Clear();

            string study = comboBox_Others_Study.SelectedItem.ToString();
            string group = comboBox_Others_Group.SelectedItem.ToString();

            IEnumerable<string> szenarioNames = _manipAnalysisFunctions.GetSzenarios(study, group);
            if (szenarioNames != null)
            {
                comboBox_Others_Szenario.Items.AddRange(szenarioNames.ToArray());
                comboBox_Others_Szenario.SelectedIndex = 0;
            }
        }

        private void comboBox_Others_Szenario_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox_Others_Subject.Items.Clear();
            comboBox_Others_Turn.Items.Clear();

            string study = comboBox_Others_Study.SelectedItem.ToString();
            string group = comboBox_Others_Group.SelectedItem.ToString();
            string szenario = comboBox_Others_Szenario.SelectedItem.ToString();

            IEnumerable<SubjectInformationContainer> subjectNames = _manipAnalysisFunctions.GetSubjects(study, group,
                szenario);
            if (subjectNames != null)
            {
                comboBox_Others_Subject.Items.AddRange(subjectNames.ToArray());
                comboBox_Others_Subject.SelectedIndex = 0;
            }
        }

        private void comboBox_Others_Subject_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox_Others_Turn.Items.Clear();

            string study = comboBox_Others_Study.SelectedItem.ToString();
            string group = comboBox_Others_Group.SelectedItem.ToString();
            string szenario = comboBox_Others_Szenario.SelectedItem.ToString();
            var subject = (SubjectInformationContainer) comboBox_Others_Subject.SelectedItem;

            IEnumerable<string> turnNames = _manipAnalysisFunctions.GetTurns(study, group, szenario, subject);
            if (turnNames != null)
            {
                comboBox_Others_Turn.Items.AddRange(turnNames.ToArray());
                comboBox_Others_Turn.SelectedIndex = 0;
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
                    switch (comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString())
                    {
                        case "Velocity":
                            _manipAnalysisFunctions.ExportVelocityData(
                                listBox_TrajectoryVelocity_SelectedTrials.Items.Cast<TrajectoryVelocityPlotContainer>(),
                                comboBox_TrajectoryVelocity_IndividualMean.SelectedItem.ToString(),
                                saveFileDialog.FileName
                                );
                            break;

                        case "Trajectory":
                            _manipAnalysisFunctions.ExportTrajectoryData(
                                listBox_TrajectoryVelocity_SelectedTrials.Items.Cast<TrajectoryVelocityPlotContainer>(),
                                comboBox_TrajectoryVelocity_IndividualMean.SelectedItem.ToString(),
                                saveFileDialog.FileName);
                            break;
                    }
                }
                else
                {
                    WriteToLogBox("Please add data to export!");
                }
            }
            WriteProgressInfo("Ready");
            SetProgressBarValue(0);
        }

        private void button_Others_ExportBaseline_Click(object sender, EventArgs e)
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
                                      + comboBox_Others_Subject.SelectedItem
                                      + "-TrajectoryBaselineData";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                _manipAnalysisFunctions.ExportTrajectoryBaseline(
                    comboBox_Others_Study.SelectedItem.ToString(),
                    comboBox_Others_Group.SelectedItem.ToString(),
                    comboBox_Others_Szenario.SelectedItem.ToString(),
                    (SubjectInformationContainer)
                        comboBox_Others_Subject.SelectedItem,
                    saveFileDialog.FileName);
            }
        }

        private void button_Others_ExportSzenarioMeanTimes_Click(object sender, EventArgs e)
        {
            saveFileDialog = new SaveFileDialog();
            saveFileDialog.Reset();
            saveFileDialog.Title = @"Save mean-time file";
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
                                      + comboBox_Others_Subject.SelectedItem
                                      + "-SzenarioMeanTimeData";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                _manipAnalysisFunctions.ExportSzenarioMeanTimes(
                    comboBox_Others_Study.SelectedItem.ToString(),
                    comboBox_Others_Group.SelectedItem.ToString(),
                    comboBox_Others_Szenario.SelectedItem.ToString(),
                    (SubjectInformationContainer)
                        comboBox_Others_Subject.SelectedItem,
                    Convert.ToInt32(
                        comboBox_Others_Turn.SelectedItem.ToString()
                            .Substring("Turn".Length)),
                    saveFileDialog.FileName);
            }
        }

        private void button_Import_ClearMeasureFileList_Click(object sender, EventArgs e)
        {
            listBox_Import_SelectedMeasureFiles.Items.Clear();
        }

        private void checkBox_DescriptiveStatistic1_ShowCatchTrialsExclusivly_CheckedChanged(object sender, EventArgs e)
        {
            checkBox_DescriptiveStatistic1_ShowErrorclampTrials.Checked = false;
            checkBox_DescriptiveStatistic1_ShowErrorclampTrialsExclusivly.Checked = false;

            checkBox_DescriptiveStatistic1_ShowErrorclampTrials.Enabled =
                !checkBox_DescriptiveStatistic1_ShowCatchTrialsExclusivly.Checked;
            checkBox_DescriptiveStatistic1_ShowErrorclampTrialsExclusivly.Enabled = false;

            checkBox_DescriptiveStatistic1_ShowCatchTrials.Enabled =
                !checkBox_DescriptiveStatistic1_ShowCatchTrialsExclusivly.Checked;

            listBox_DescriptiveStatistic1_Turns_SelectedIndexChanged(this, new EventArgs());
        }

        private void checkBox_DescriptiveStatistic2_ShowCatchTrialsExclusivly_CheckedChanged(object sender, EventArgs e)
        {
            checkBox_DescriptiveStatistic2_ShowErrorclampTrials.Checked = false;
            checkBox_DescriptiveStatistic2_ShowErrorclampTrialsExclusivly.Checked = false;

            checkBox_DescriptiveStatistic2_ShowErrorclampTrials.Enabled =
                !checkBox_DescriptiveStatistic2_ShowCatchTrialsExclusivly.Checked;
            checkBox_DescriptiveStatistic2_ShowErrorclampTrialsExclusivly.Enabled = false;

            checkBox_DescriptiveStatistic2_ShowCatchTrials.Enabled =
                !checkBox_DescriptiveStatistic2_ShowCatchTrialsExclusivly.Checked;

            listBox_DescriptiveStatistic2_Turns_SelectedIndexChanged(this, new EventArgs());
        }

        private void button_DataManipulation_DeleteMeasureFile_Click(object sender, EventArgs e)
        {
            WriteProgressInfo("Deleting measure file...");
            _manipAnalysisFunctions.DeleteMeasureFile(Convert.ToInt32(textBox_DataManipulation_MeasureFileID.Text));
            WriteProgressInfo("Ready");
        }

        private void button_Start_SelectDatabase_Click(object sender, EventArgs e)
        {
            _manipAnalysisFunctions.SetSqlDatabase(comboBox_Start_Database.SelectedItem.ToString());

            if (!tabControl.TabPages.Contains(tabPage_VisualizationExport))
            {
                tabControl.TabPages.Remove(tabPage_Impressum);
                tabControl.TabPages.Add(tabPage_VisualizationExport);
                tabControl.TabPages.Add(tabPage_Impressum);
            }
            checkBox_Start_ManualMode.Enabled = true;
        }

        private void button_Start_ConnectToSQlServer_Click(object sender, EventArgs e)
        {
            WriteProgressInfo("Connecting to server...");

            comboBox_Start_Database.Items.Clear();

            if (_manipAnalysisFunctions.ConnectToSqlServer(textBox_Start_SqlServer.Text))
            {
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

        public void SetSqlDatabases(IEnumerable<string> databases)
        {
            if (databases != null)
            {
                comboBox_Start_Database.Items.AddRange(databases.ToArray());
            }
        }

        private void button_Others_PlotSzenarioMeanTimes_Click(object sender, EventArgs e)
        {
            _manipAnalysisFunctions.PlotSzenarioMeanTimes(comboBox_Others_Study.SelectedItem.ToString(),
                comboBox_Others_Group.SelectedItem.ToString(),
                comboBox_Others_Szenario.SelectedItem.ToString(),
                (SubjectInformationContainer)
                    comboBox_Others_Subject.SelectedItem,
                Convert.ToInt32(
                    comboBox_Others_Turn.SelectedItem.ToString()
                        .Substring("Turn".Length)));
        }

        private void button_Others_ExportVelocityBaseline_Click(object sender, EventArgs e)
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
                                      + comboBox_Others_Subject.SelectedItem
                                      + "-VelocityBaselineData";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                _manipAnalysisFunctions.ExportVelocityBaseline(
                    comboBox_Others_Study.SelectedItem.ToString(),
                    comboBox_Others_Group.SelectedItem.ToString(),
                    comboBox_Others_Szenario.SelectedItem.ToString(),
                    (SubjectInformationContainer)
                        comboBox_Others_Subject.SelectedItem,
                    saveFileDialog.FileName);
            }
        }

        private void button_Others_PlotVelocityBaseline_Click(object sender, EventArgs e)
        {
            _manipAnalysisFunctions.PlotVelocityBaselines(comboBox_Others_Study.SelectedItem.ToString(),
                comboBox_Others_Group.SelectedItem.ToString(),
                comboBox_Others_Szenario.SelectedItem.ToString(),
                (SubjectInformationContainer)
                    comboBox_Others_Subject.SelectedItem);
        }

        private void tabPage_Debug_BaselineRecalculation_Enter(object sender, EventArgs e)
        {
            comboBox_BaselineRecalculation_Study.Items.Clear();
            comboBox_BaselineRecalculation_Group.Items.Clear();
            comboBox_BaselineRecalculation_Szenario.Items.Clear();
            comboBox_BaselineRecalculation_Subject.Items.Clear();
            listBox_BaselineRecalculation_Targets.Items.Clear();
            listBox_BaselineRecalculation_Trials.Items.Clear();

            IEnumerable<string> studyNames = _manipAnalysisFunctions.GetStudys();
            if (studyNames != null)
            {
                comboBox_BaselineRecalculation_Study.Items.AddRange(studyNames.ToArray());
                comboBox_BaselineRecalculation_Study.SelectedIndex = 0;
            }
        }

        private void comboBox_BaselineRecalculation_Study_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox_BaselineRecalculation_Group.Items.Clear();
            comboBox_BaselineRecalculation_Szenario.Items.Clear();
            comboBox_BaselineRecalculation_Subject.Items.Clear();
            listBox_BaselineRecalculation_Targets.Items.Clear();
            listBox_BaselineRecalculation_Trials.Items.Clear();

            IEnumerable<string> groupNames =
                _manipAnalysisFunctions.GetGroups(comboBox_BaselineRecalculation_Study.SelectedItem.ToString());

            if (groupNames != null)
            {
                comboBox_BaselineRecalculation_Group.Items.AddRange(groupNames.ToArray());
                comboBox_BaselineRecalculation_Group.SelectedIndex = 0;
            }
        }

        private void comboBox_BaselineRecalculation_Group_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox_BaselineRecalculation_Szenario.Items.Clear();
            comboBox_BaselineRecalculation_Subject.Items.Clear();
            listBox_BaselineRecalculation_Targets.Items.Clear();
            listBox_BaselineRecalculation_Trials.Items.Clear();

            string study = comboBox_BaselineRecalculation_Study.SelectedItem.ToString();
            string group = comboBox_BaselineRecalculation_Group.SelectedItem.ToString();

            IEnumerable<string> szenarioNames = _manipAnalysisFunctions.GetSzenarios(study, group);
            if (szenarioNames != null)
            {
                comboBox_BaselineRecalculation_Szenario.Items.AddRange(szenarioNames.ToArray());
                comboBox_BaselineRecalculation_Szenario.SelectedIndex = 0;
            }
        }

        private void comboBox_BaselineRecalculation_Szenario_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox_BaselineRecalculation_Subject.Items.Clear();
            listBox_BaselineRecalculation_Targets.Items.Clear();
            listBox_BaselineRecalculation_Trials.Items.Clear();

            string study = comboBox_BaselineRecalculation_Study.SelectedItem.ToString();
            string group = comboBox_BaselineRecalculation_Group.SelectedItem.ToString();
            string szenario = comboBox_BaselineRecalculation_Szenario.SelectedItem.ToString();

            IEnumerable<SubjectInformationContainer> subjectNames = _manipAnalysisFunctions.GetSubjects(study, group,
                szenario);
            if (subjectNames != null)
            {
                comboBox_BaselineRecalculation_Subject.Items.AddRange(subjectNames.ToArray());
                comboBox_BaselineRecalculation_Subject.SelectedIndex = 0;
            }
        }

        private void comboBox_BaselineRecalculation_Subject_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox_BaselineRecalculation_Targets.Items.Clear();
            listBox_BaselineRecalculation_Trials.Items.Clear();

            string study = comboBox_BaselineRecalculation_Study.SelectedItem.ToString();
            string szenario = comboBox_BaselineRecalculation_Szenario.SelectedItem.ToString();

            IEnumerable<string> targets = _manipAnalysisFunctions.GetTargets(study, szenario);
            IEnumerable<string> trials = _manipAnalysisFunctions.GetTrials(study, szenario);

            if (targets != null)
            {
                listBox_BaselineRecalculation_Targets.Items.AddRange(targets.OrderBy(t => t).ToArray());
                listBox_BaselineRecalculation_Targets.SelectedIndex = 0;
            }

            if (trials != null)
            {
                listBox_BaselineRecalculation_Trials.Items.AddRange(trials.OrderBy(t => t).ToArray());
                listBox_BaselineRecalculation_Trials.SelectedIndex = 0;
            }
        }

        private void button_BaselineRecalculation_AddSelected_Click(object sender, EventArgs e)
        {
            if (comboBox_BaselineRecalculation_Study.SelectedItem != null)
            {
                const string turn = "Turn 1";

                string study = comboBox_BaselineRecalculation_Study.SelectedItem.ToString();
                string group = comboBox_BaselineRecalculation_Group.SelectedItem.ToString();
                string szenario = comboBox_BaselineRecalculation_Szenario.SelectedItem.ToString();
                var subject = (SubjectInformationContainer) comboBox_BaselineRecalculation_Subject.SelectedItem;
                string[] targets = listBox_BaselineRecalculation_Targets.SelectedItems.Cast<string>().ToArray();
                string[] trials = listBox_BaselineRecalculation_Trials.SelectedItems.Cast<string>().ToArray();

                foreach (string target in targets)
                {
                    if (_manipAnalysisFunctions.GetTurns(study, group, szenario, subject) != null)
                    {
                        if (listBox_BaselineRecalculation_Trials.Items.Count > 0)
                        {
                            bool canBeUpdated = false;
                            foreach (
                                TrajectoryVelocityPlotContainer temp in
                                    listBox_BaselineRecalculation_SelectedTrials.Items)
                            {
                                if (temp.UpdateTrajectoryVelocityPlotContainer(study, group, szenario,
                                    subject, turn, target, trials))
                                {
                                    typeof (ListBox).InvokeMember("RefreshItems",
                                        BindingFlags.NonPublic |
                                        BindingFlags.Instance |
                                        BindingFlags.InvokeMethod,
                                        null,
                                        listBox_BaselineRecalculation_SelectedTrials,
                                        new object[] {});
                                    canBeUpdated = true;
                                }
                            }

                            if (!canBeUpdated)
                            {
                                listBox_BaselineRecalculation_SelectedTrials.Items.Add(
                                    new TrajectoryVelocityPlotContainer(study, group, szenario, subject,
                                        turn, target, trials));
                            }
                        }
                        else
                        {
                            listBox_BaselineRecalculation_SelectedTrials.Items.Add(
                                new TrajectoryVelocityPlotContainer(study, group, szenario, subject, turn,
                                    target, trials));
                        }
                    }
                }
            }
        }

        private void button_BaselineRecalculation_AddAll_Click(object sender, EventArgs e)
        {
            if (comboBox_BaselineRecalculation_Study.SelectedItem != null)
            {
                const string turn = "Turn 1";

                string study = comboBox_BaselineRecalculation_Study.SelectedItem.ToString();
                string group = comboBox_BaselineRecalculation_Group.SelectedItem.ToString();
                string szenario = comboBox_BaselineRecalculation_Szenario.SelectedItem.ToString();
                var subject = (SubjectInformationContainer) comboBox_BaselineRecalculation_Subject.SelectedItem;
                string[] targets = listBox_BaselineRecalculation_Targets.SelectedItems.Cast<string>().ToArray();
                string[] trials = listBox_BaselineRecalculation_Trials.Items.Cast<string>().ToArray();

                foreach (string target in targets)
                {
                    if (_manipAnalysisFunctions.GetTurns(study, group, szenario, subject) != null)
                    {
                        if (listBox_BaselineRecalculation_Trials.Items.Count > 0)
                        {
                            bool canBeUpdated = false;
                            foreach (
                                TrajectoryVelocityPlotContainer temp in
                                    listBox_BaselineRecalculation_SelectedTrials.Items)
                            {
                                if (temp.UpdateTrajectoryVelocityPlotContainer(study, group, szenario,
                                    subject, turn, target, trials))
                                {
                                    typeof (ListBox).InvokeMember("RefreshItems",
                                        BindingFlags.NonPublic |
                                        BindingFlags.Instance |
                                        BindingFlags.InvokeMethod,
                                        null,
                                        listBox_BaselineRecalculation_SelectedTrials,
                                        new object[] {});
                                    canBeUpdated = true;
                                }
                            }

                            if (!canBeUpdated)
                            {
                                listBox_BaselineRecalculation_SelectedTrials.Items.Add(
                                    new TrajectoryVelocityPlotContainer(study, group, szenario, subject,
                                        turn, target, trials));
                            }
                        }
                        else
                        {
                            listBox_BaselineRecalculation_SelectedTrials.Items.Add(
                                new TrajectoryVelocityPlotContainer(study, group, szenario, subject, turn,
                                    target, trials));
                        }
                    }
                }
            }
        }

        private void button_BaselineRecalculation_ClearSelected_Click(object sender, EventArgs e)
        {
            while (listBox_BaselineRecalculation_SelectedTrials.SelectedItems.Count > 0)
            {
                listBox_BaselineRecalculation_SelectedTrials.Items.Remove(
                    listBox_BaselineRecalculation_SelectedTrials.SelectedItem);
            }
        }

        private void button_BaselineRecalculation_ClearAll_Click(object sender, EventArgs e)
        {
            listBox_BaselineRecalculation_SelectedTrials.Items.Clear();
        }

        private void button_BaselineRecalculation_RecalculateBaseline_Click(object sender, EventArgs e)
        {
            if (listBox_BaselineRecalculation_SelectedTrials.Items.Count ==
                listBox_BaselineRecalculation_Targets.Items.Count)
            {
                if (
                    listBox_BaselineRecalculation_SelectedTrials.Items.Cast<TrajectoryVelocityPlotContainer>()
                        .Select(t => t.Study)
                        .Distinct()
                        .Count() == 1)
                {
                    if (
                        listBox_BaselineRecalculation_SelectedTrials.Items.Cast<TrajectoryVelocityPlotContainer>()
                            .Select(t => t.Group)
                            .Distinct()
                            .Count() == 1)
                    {
                        if (
                            listBox_BaselineRecalculation_SelectedTrials.Items.Cast<TrajectoryVelocityPlotContainer>()
                                .Select(t => t.Subject)
                                .Distinct()
                                .Count() == 1)
                        {
                            if (
                                listBox_BaselineRecalculation_SelectedTrials.Items.Cast<TrajectoryVelocityPlotContainer>
                                    ().Select(t => t.Szenario).Distinct().Count() == 1)
                            {
                                _manipAnalysisFunctions.RecalculateBaselines(
                                    listBox_BaselineRecalculation_SelectedTrials.Items
                                        .Cast<TrajectoryVelocityPlotContainer>());
                            }
                            else
                            {
                                WriteToLogBox("Please add only one Szenario!");
                            }
                        }
                        else
                        {
                            WriteToLogBox("Please add only one Subject!");
                        }
                    }
                    else
                    {
                        WriteToLogBox("Please add only one Group!");
                    }
                }
                else
                {
                    WriteToLogBox("Please add only one Study!");
                }
            }
            else
            {
                WriteToLogBox("Please add data for all Targets!");
            }
        }

        private void button_Others_PlotGroupLi_Click(object sender, EventArgs e)
        {
            IEnumerable<SubjectInformationContainer> subjects;

            if (checkBox_Others_GroupAverage.Checked)
            {
                subjects = comboBox_Others_Subject.Items.Cast<SubjectInformationContainer>();
            }
            else
            {
                subjects = new List<SubjectInformationContainer>
                {
                    (SubjectInformationContainer) comboBox_Others_Subject.SelectedItem
                };
            }

            _manipAnalysisFunctions.PlotLearningIndex(comboBox_Others_Study.SelectedItem.ToString(),
                comboBox_Others_Group.SelectedItem.ToString(),
                comboBox_Others_Szenario.SelectedItem.ToString(),
                subjects,
                Convert.ToInt32(
                    comboBox_Others_Turn.SelectedItem.ToString()
                        .Substring("Turn".Length)));
        }

        private void button_Others_ExportGroupLi_Click(object sender, EventArgs e)
        {
            saveFileDialog = new SaveFileDialog();
            saveFileDialog.Reset();
            saveFileDialog.Title = @"Save learning-index file";
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
                                      + comboBox_Others_Study.SelectedItem
                                      + "-"
                                      + comboBox_Others_Group.SelectedItem
                                      + "-"
                                      + comboBox_Others_Szenario.SelectedItem
                                      + "-LearningIndex";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                IEnumerable<SubjectInformationContainer> subjects;

                if (checkBox_Others_GroupAverage.Checked)
                {
                    subjects = comboBox_Others_Subject.Items.Cast<SubjectInformationContainer>();
                }
                else
                {
                    subjects = new List<SubjectInformationContainer>
                    {
                        (SubjectInformationContainer) comboBox_Others_Subject.SelectedItem
                    };
                }

                _manipAnalysisFunctions.ExportLearningIndex(saveFileDialog.FileName,
                    comboBox_Others_Study.SelectedItem.ToString(),
                    comboBox_Others_Group.SelectedItem.ToString(),
                    comboBox_Others_Szenario.SelectedItem.ToString(),
                    subjects,
                    Convert.ToInt32(
                        comboBox_Others_Turn.SelectedItem.ToString()
                            .Substring("Turn".Length)));
            }
        }

        private void checkBox_DescriptiveStatistic2_ShowErrorclampTrials_CheckedChanged(object sender, EventArgs e)
        {
            listBox_DescriptiveStatistic2_Turns_SelectedIndexChanged(this, new EventArgs());
            checkBox_DescriptiveStatistic2_ShowErrorclampTrialsExclusivly.Enabled =
                checkBox_DescriptiveStatistic2_ShowErrorclampTrials.Checked;
        }

        private void checkBox_DescriptiveStatistic1_ShowErrorclampTrials_CheckedChanged(object sender, EventArgs e)
        {
            listBox_DescriptiveStatistic1_Turns_SelectedIndexChanged(this, new EventArgs());
            checkBox_DescriptiveStatistic1_ShowErrorclampTrialsExclusivly.Enabled =
                checkBox_DescriptiveStatistic1_ShowErrorclampTrials.Checked;
        }

        private void checkBox_DescriptiveStatistic1_ShowErrorclampTrialsExclusivly_CheckedChanged(object sender,
            EventArgs e)
        {
            checkBox_DescriptiveStatistic1_ShowCatchTrials.Checked = false;
            checkBox_DescriptiveStatistic1_ShowCatchTrialsExclusivly.Checked = false;

            checkBox_DescriptiveStatistic1_ShowCatchTrials.Enabled =
                !checkBox_DescriptiveStatistic1_ShowErrorclampTrialsExclusivly.Checked;
            checkBox_DescriptiveStatistic1_ShowCatchTrialsExclusivly.Enabled = false;

            checkBox_DescriptiveStatistic1_ShowErrorclampTrials.Enabled =
                !checkBox_DescriptiveStatistic1_ShowErrorclampTrialsExclusivly.Checked;

            listBox_DescriptiveStatistic1_Turns_SelectedIndexChanged(this, new EventArgs());
        }

        private void checkBox_DescriptiveStatistic2_ShowErrorclampTrialsExclusivly_CheckedChanged(object sender,
            EventArgs e)
        {
            checkBox_DescriptiveStatistic2_ShowCatchTrials.Checked = false;
            checkBox_DescriptiveStatistic2_ShowCatchTrialsExclusivly.Checked = false;

            checkBox_DescriptiveStatistic2_ShowCatchTrials.Enabled =
                !checkBox_DescriptiveStatistic2_ShowErrorclampTrialsExclusivly.Checked;
            checkBox_DescriptiveStatistic2_ShowCatchTrialsExclusivly.Enabled = false;

            checkBox_DescriptiveStatistic2_ShowErrorclampTrials.Enabled =
                !checkBox_DescriptiveStatistic2_ShowErrorclampTrialsExclusivly.Checked;

            listBox_DescriptiveStatistic2_Turns_SelectedIndexChanged(this, new EventArgs());
        }

        private void checkBox_TrajectoryVelocity_ShowCatchTrials_CheckedChanged(object sender, EventArgs e)
        {
            checkBox_TrajectoryVelocity_ShowCatchTrialsExclusivly.Enabled =
                checkBox_TrajectoryVelocity_ShowCatchTrials.Checked;
        }

        private void checkBox_TrajectoryVelocity_ShowErrorclampTrials_CheckedChanged(object sender, EventArgs e)
        {
            checkBox_TrajectoryVelocity_ShowErrorclampTrialsExclusivly.Enabled =
                checkBox_TrajectoryVelocity_ShowErrorclampTrials.Checked;
        }

        private void checkBox_TrajectoryVelocity_ShowCatchTrialsExclusivly_CheckedChanged(object sender, EventArgs e)
        {
            checkBox_TrajectoryVelocity_ShowErrorclampTrials.Checked = false;
            checkBox_TrajectoryVelocity_ShowErrorclampTrialsExclusivly.Checked = false;

            checkBox_TrajectoryVelocity_ShowErrorclampTrials.Enabled =
                !checkBox_TrajectoryVelocity_ShowCatchTrialsExclusivly.Checked;
            checkBox_TrajectoryVelocity_ShowErrorclampTrialsExclusivly.Enabled = false;

            checkBox_TrajectoryVelocity_ShowCatchTrials.Enabled =
                !checkBox_TrajectoryVelocity_ShowCatchTrialsExclusivly.Checked;
        }

        private void checkBox_TrajectoryVelocity_ShowErrorclampTrialsExclusivly_CheckedChanged(object sender,
            EventArgs e)
        {
            checkBox_TrajectoryVelocity_ShowCatchTrials.Checked = false;
            checkBox_TrajectoryVelocity_ShowCatchTrialsExclusivly.Checked = false;

            checkBox_TrajectoryVelocity_ShowCatchTrials.Enabled =
                !checkBox_TrajectoryVelocity_ShowErrorclampTrialsExclusivly.Checked;
            checkBox_TrajectoryVelocity_ShowCatchTrialsExclusivly.Enabled = false;

            checkBox_TrajectoryVelocity_ShowErrorclampTrials.Enabled =
                !checkBox_TrajectoryVelocity_ShowErrorclampTrialsExclusivly.Checked;
        }

        private void button_Others_ForcefieldCompensationFactor_Click(object sender, EventArgs e)
        {
            _manipAnalysisFunctions.ForcefieldCompensationFactor(comboBox_Others_Study.SelectedItem.ToString(),
                comboBox_Others_Group.SelectedItem.ToString(),
                comboBox_Others_Szenario.SelectedItem.ToString(),
                (SubjectInformationContainer)
                    comboBox_Others_Subject.SelectedItem,
                Convert.ToInt32(
                    comboBox_Others_Turn.SelectedItem.ToString()
                        .Substring("Turn".Length)),
                        Convert.ToInt32(textBox_Others_PlotErrorclampForces_MsIndex.Text));
        }

        private delegate void LogBoxCallbackAddString(string text);

        private delegate void LogBoxCallbackClearItems();

        private delegate string[] LogBoxCallbackGetText();

        private delegate void ProgressBarCallback(double value);

        private delegate void ProgressLabelCallback(string text);

        private delegate void TabControlCallback(bool enable);
    }
}