using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using ManipAnalysis_v2.Container;
using ManipAnalysis_v2.MongoDb;

namespace ManipAnalysis_v2
{
    partial class ManipAnalysisGui : Form
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

        public void SetManipAnalysisModel(ManipAnalysisFunctions manipAnalysisFunctions)
        {
            _manipAnalysisFunctions = manipAnalysisFunctions;
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

                for (int i = 0; i < textArray.Length; i
                                                          ++)
                {
                    if (i == 0)
                    {
                        listBox_LogBox.Items.Add("[" + DateTime.Now + "] " + textArray[i]);
                    }
                    else
                    {
                        listBox_LogBox.Items.Add(textArray[i]);
                    }

                    if (listBox_LogBox.HorizontalExtent < TextRenderer.MeasureText(listBox_LogBox.Items[listBox_LogBox.Items.Count - 1].ToString(), listBox_LogBox.Font, listBox_LogBox.ClientSize, TextFormatFlags.NoPrefix).Width)
                    {
                        listBox_LogBox.HorizontalExtent = TextRenderer.MeasureText(listBox_LogBox.Items[listBox_LogBox.Items.Count - 1].ToString(), listBox_LogBox.Font, listBox_LogBox.ClientSize, TextFormatFlags.NoPrefix).Width;
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
            openFileDialog.Reset();
            openFileDialog.Multiselect = true;
            openFileDialog.Title = @"Select measure-file(s)";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog.Filter = @"KINARM-files (*.zip)|*.zip|BioMotionBot-files (*.csv)|*.csv";
            openFileDialog.ShowDialog();

            var filesList = new List<FileInfo>(openFileDialog.FileNames.Select(t => new FileInfo(t)));

            for (int filesCounter = 0; filesCounter < filesList.Count; filesCounter
                                                                           ++)
            {
                if (_manipAnalysisFunctions.IsValidMeasureDataFile(filesList[filesCounter].FullName))
                {
                    if (!listBox_Import_SelectedMeasureFiles.Items.Contains(filesList[filesCounter].FullName))
                    {
                        listBox_Import_SelectedMeasureFiles.Items.Add(filesList[filesCounter].FullName);
                    }
                }
                else
                {
                    WriteToLogBox("File is invalid or already imported: " + filesList[filesCounter].FullName);
                }
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

                for (int i = 0; i < directoriesList.Count; i
                                                               ++)
                {
                    DirectoryInfo di = directoriesList[i];
                    filesList.AddRange(di.GetFiles("*.csv"));
                    filesList.AddRange(di.GetFiles("*.zip"));
                }
                directoriesList.Clear();

                for (int filesCounter = 0; filesCounter < filesList.Count; filesCounter
                                                                               ++)
                {
                    if (_manipAnalysisFunctions.IsValidMeasureDataFile(filesList[filesCounter].FullName))
                    {
                        if (!listBox_Import_SelectedMeasureFiles.Items.Contains(filesList[filesCounter].FullName))
                        {
                            listBox_Import_SelectedMeasureFiles.Items.Add(filesList[filesCounter].FullName);
                        }
                    }
                    else
                    {
                        WriteToLogBox("File is invalid or already imported: " + filesList[filesCounter].FullName);
                    }
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
                /*
                if (Environment.MachineName != textBox_Start_SqlServer.Text)
                {
                    MessageBox.Show(@"Import and Calculations only possible when running on ManipServer!");
                }
                else
                {
                    tabControl.TabPages.Add(tabPage_ImportCalculations);
                }*/
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
            if (MessageBox.Show(@"Are you really sure you want to initialise the Database?", @"Really?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _manipAnalysisFunctions.DropDatabase();
            }
        }

        private void button_PlotBaseline_Click(object sender, EventArgs e)
        {
            var trialTypes = new List<Trial.TrialTypeEnum>();
            var forceFields = new List<Trial.ForceFieldTypeEnum>();
            var handedness = new List<Trial.HandednessEnum>();
            int[] targets = listBox_OtherStatistics_Targets.SelectedItems.Cast<string>().Select(t => Convert.ToInt32(t.Substring(7, 2))).ToArray();

            foreach (string
                item
                in
                listBox_OtherStatistics_TrialType.SelectedItems)
            {
                trialTypes.Add((Trial.TrialTypeEnum) Enum.Parse(typeof (Trial.TrialTypeEnum), item));
            }

            foreach (string
                item
                in
                listBox_OtherStatistics_ForceField.SelectedItems)
            {
                forceFields.Add((Trial.ForceFieldTypeEnum) Enum.Parse(typeof (Trial.ForceFieldTypeEnum), item));
            }

            foreach (string
                item
                in
                listBox_OtherStatistics_Handedness.SelectedItems)
            {
                handedness.Add((Trial.HandednessEnum) Enum.Parse(typeof (Trial.HandednessEnum), item));
            }

            _manipAnalysisFunctions.PlotTrajectoryBaseline(comboBox_Others_Study.SelectedItem.ToString(), comboBox_Others_Group.SelectedItem.ToString(), (SubjectContainer) comboBox_Others_Subject.SelectedItem, targets, trialTypes, forceFields, handedness);
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
                if (value <= progressBar.Maximum && value >= progressBar.Minimum)
                {
                    progressBar.Value = Convert.ToInt32(value);
                }
                using (Graphics gr = progressBar.CreateGraphics())
                {
                    progressBar.Refresh();
                    gr.DrawString(progressBar.Text, SystemFonts.DefaultFont, Brushes.Black, new PointF(progressBar.Width/2 - (gr.MeasureString(progressBar.Text, SystemFonts.DefaultFont).Width/2.0F), progressBar.Height/2 - (gr.MeasureString(progressBar.Text, SystemFonts.DefaultFont).Height/2.0F)));
                }
            }
        }

        public void WriteProgressInfo(string text)
        {
            if (progressBar.InvokeRequired)
            {
                ProgressLabelCallback writeProgressInfo = WriteProgressInfo;
                progressBar.Invoke(writeProgressInfo, new object[] {text});
            }
            else
            {
                using (Graphics gr = progressBar.CreateGraphics())
                {
                    progressBar.Text = text;
                    progressBar.Refresh();
                    gr.DrawString(progressBar.Text, SystemFonts.DefaultFont, Brushes.Black, new PointF(progressBar.Width/2 - (gr.MeasureString(progressBar.Text, SystemFonts.DefaultFont).Width/2.0F), progressBar.Height/2 - (gr.MeasureString(progressBar.Text, SystemFonts.DefaultFont).Height/2.0F)));
                }
            }
        }

        private void comboBox_StatisticPlots_Study_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox_DescriptiveStatistic1_Groups.Items.Clear();
            comboBox_DescriptiveStatistic1_Szenario.Items.Clear();
            listBox_DescriptiveStatistic1_Subjects.Items.Clear();
            listBox_DescriptiveStatistic1_Turns.Items.Clear();
            listBox_DescriptiveStatistic1_Trials.Items.Clear();

            IEnumerable<string> groupNames = _manipAnalysisFunctions.GetGroups(comboBox_DescriptiveStatistic1_Study.SelectedItem.ToString());
            if (groupNames != null)
            {
                listBox_DescriptiveStatistic1_Groups.Items.AddRange(groupNames.ToArray());
                if (listBox_DescriptiveStatistic1_Groups.Items.Count > 0)
                {
                    listBox_DescriptiveStatistic1_Groups.SelectedIndex = 0;
                }
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
                    for (int i = 1; i < groups.Length; i
                                                           ++)
                    {
                        szenarioIntersect = szenarioIntersect.Intersect(_manipAnalysisFunctions.GetSzenarios(study, groups[i])).ToArray();
                    }

                    comboBox_DescriptiveStatistic1_Szenario.Items.AddRange(szenarioIntersect.ToArray());
                    if (comboBox_DescriptiveStatistic1_Szenario.Items.Count > 0)
                    {
                        comboBox_DescriptiveStatistic1_Szenario.SelectedIndex = 0;
                    }
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

            for (int i = 0; i < groups.Length; i
                                                   ++)
            {
                IEnumerable<SubjectContainer> tempSubjects = _manipAnalysisFunctions.GetSubjects(study, groups[i], szenario);
                if (tempSubjects != null)
                {
                    listBox_DescriptiveStatistic1_Subjects.Items.AddRange(tempSubjects.ToArray());
                }
            }
            if (listBox_DescriptiveStatistic1_Subjects.Items.Count > 0)
            {
                listBox_DescriptiveStatistic1_Subjects.SelectedIndex = 0;
            }
        }

        private void listBox_StatisticPlots_Subject_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox_DescriptiveStatistic1_Turns.Items.Clear();
            listBox_DescriptiveStatistic1_Trials.Items.Clear();

            string study = comboBox_DescriptiveStatistic1_Study.SelectedItem.ToString();
            string szenario = comboBox_DescriptiveStatistic1_Szenario.SelectedItem.ToString();
            SubjectContainer[] subjects = listBox_DescriptiveStatistic1_Subjects.SelectedItems.Cast<SubjectContainer>().ToArray();

            string[] turnIntersect = null;
            for (int j = 0; j < subjects.Length; j++)
            {
                IEnumerable<string> tempTurnString = _manipAnalysisFunctions.GetTurns(study, szenario, subjects[j]);

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

            if (turnIntersect != null)
            {
                listBox_DescriptiveStatistic1_Turns.Items.AddRange(turnIntersect);
            }

            if (listBox_DescriptiveStatistic1_Turns.Items.Count > 0)
            {
                listBox_DescriptiveStatistic1_Turns.SelectedIndex = 0;
            }
        }

        private void listBox_DescriptiveStatistic1_Turns_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox_DescriptiveStatistic1_Trials.Items.Clear();

            string study = comboBox_DescriptiveStatistic1_Study.SelectedItem.ToString();
            string szenario = comboBox_DescriptiveStatistic1_Szenario.SelectedItem.ToString();
            SubjectContainer[] subjects = listBox_DescriptiveStatistic1_Subjects.SelectedItems.Cast<SubjectContainer>().ToArray();
            string[] turns = listBox_DescriptiveStatistic1_Turns.SelectedItems.Cast<string>().ToArray();

            var trialTypes = new List<Trial.TrialTypeEnum>();
            var forceFields = new List<Trial.ForceFieldTypeEnum>();
            var handedness = new List<Trial.HandednessEnum>();

            foreach (string item in listBox_DescriptiveStatistic1_TrialType.SelectedItems)
            {
                trialTypes.Add((Trial.TrialTypeEnum) Enum.Parse(typeof (Trial.TrialTypeEnum), item));
            }

            foreach (string item in listBox_DescriptiveStatistic1_ForceField.SelectedItems)
            {
                forceFields.Add((Trial.ForceFieldTypeEnum) Enum.Parse(typeof (Trial.ForceFieldTypeEnum), item));
            }

            foreach (string item in listBox_DescriptiveStatistic1_Handedness.SelectedItems)
            {
                handedness.Add((Trial.HandednessEnum) Enum.Parse(typeof (Trial.HandednessEnum), item));
            }

            string[] szenarioTrialNamesIntersect = null;
            
                for (int i = 0; i < subjects.Length; i++)
                {
                    IEnumerable<string> szenarioTrialNames = _manipAnalysisFunctions.GetTrialsOfSzenario(study, szenario, subjects[i], trialTypes, forceFields, handedness);

                    if (szenarioTrialNames != null)
                    {
                        if (szenarioTrialNamesIntersect == null)
                        {
                            szenarioTrialNamesIntersect = szenarioTrialNames.ToArray();
                        }
                        else
                        {
                            szenarioTrialNamesIntersect = szenarioTrialNamesIntersect.Intersect(szenarioTrialNames).ToArray();
                        }
                    }
                }
            
            //IEnumerable<string> szenarioTrialNames = _manipAnalysisFunctions.GetTrialsOfSzenario(study, groups, szenario, subjects, trialTypes, forceFields, handedness);
            
            if (szenarioTrialNamesIntersect.Any())
            {
                listBox_DescriptiveStatistic1_Trials.Items.AddRange(szenarioTrialNamesIntersect);
                if (listBox_DescriptiveStatistic1_Trials.Items.Count > 0)
                {
                    listBox_DescriptiveStatistic1_Trials.SelectedIndex = 0;
                }
            }
        }

        private void button_StatisticPlots_AddSelected_Click(object sender, EventArgs e)
        {
            if (listBox_DescriptiveStatistic1_Trials.SelectedItems.Count > 0)
            {
                string study = comboBox_DescriptiveStatistic1_Study.SelectedItem.ToString();
                string[] groups = listBox_DescriptiveStatistic1_Groups.SelectedItems.Cast<string>().ToArray();
                string szenario = comboBox_DescriptiveStatistic1_Szenario.SelectedItem.ToString();
                SubjectContainer[] subjects = listBox_DescriptiveStatistic1_Subjects.SelectedItems.Cast<SubjectContainer>().ToArray();
                string[] turns = listBox_DescriptiveStatistic1_Turns.SelectedItems.Cast<string>().ToArray();
                string[] trials = listBox_DescriptiveStatistic1_Trials.SelectedItems.Cast<string>().ToArray();

                foreach (string
                    group
                    in
                    groups)
                {
                    foreach (SubjectContainer
                        subject
                        in
                        subjects.Where(t => _manipAnalysisFunctions.GetSubjects(study, group, szenario).Select(u => u.PId).Contains(t.PId)))
                    {
                        foreach (string
                            turn
                            in
                            turns)
                        {
                            if (_manipAnalysisFunctions.GetTurns(study, group, szenario, subject) != null)
                            {
                                if (listBox_DescriptiveStatistic1_SelectedTrials.Items.Count > 0)
                                {
                                    bool canBeUpdated = false;
                                    foreach (StatisticPlotContainer
                                        temp
                                        in
                                        listBox_DescriptiveStatistic1_SelectedTrials.Items)
                                    {
                                        if (temp.UpdateStatisticPlotContainer(study, group, szenario, subject, turn, trials))
                                        {
                                            typeof (ListBox).InvokeMember("RefreshItems", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, listBox_DescriptiveStatistic1_SelectedTrials, new object[] {});
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
            int pdTime = 300;
            if (comboBox_DescriptiveStatistic1_DataTypeSelect.SelectedItem.ToString() == "PD - Abs" || comboBox_DescriptiveStatistic1_DataTypeSelect.SelectedItem.ToString() == "PD - Sign")
            {
                var inputForm = new PerpendicularDisplacementTimeInputForm();
                if (inputForm.ShowDialog(this) == DialogResult.OK)
                {
                    pdTime = inputForm.getMilliseconds();
                    inputForm.Dispose();
                }
            }

            if (pdTime < 0)
            {
                WriteToLogBox("Negative PD-Times are not allowed!");
            }
            else
            {
                _manipAnalysisFunctions.PlotExportDescriptiveStatistic1(listBox_DescriptiveStatistic1_SelectedTrials.Items.Cast<StatisticPlotContainer>(), comboBox_DescriptiveStatistic1_DataTypeSelect.SelectedItem.ToString(), textBox_DescriptiveStatistic1_FitEquation.Text, pdTime, checkBox_DescriptiveStatistic1_PlotFit.Checked, checkBox_DescriptiveStatistic1_PlotErrorbars.Checked, null);
            }
        }

        private void button_StatisticPlots_AddAll_Click(object sender, EventArgs e)
        {
            if (listBox_DescriptiveStatistic1_Trials.SelectedItems.Count > 0)
            {
                string study = comboBox_DescriptiveStatistic1_Study.SelectedItem.ToString();
                string[] groups = listBox_DescriptiveStatistic1_Groups.SelectedItems.Cast<string>().ToArray();
                string szenario = comboBox_DescriptiveStatistic1_Szenario.SelectedItem.ToString();
                SubjectContainer[] subjects = listBox_DescriptiveStatistic1_Subjects.SelectedItems.Cast<SubjectContainer>().ToArray();
                string[] turns = listBox_DescriptiveStatistic1_Turns.SelectedItems.Cast<string>().ToArray();
                string[] trials = listBox_DescriptiveStatistic1_Trials.Items.Cast<string>().ToArray();

                foreach (string
                    group
                    in
                    groups)
                {
                    foreach (SubjectContainer
                        subject
                        in
                        subjects.Where(t => _manipAnalysisFunctions.GetSubjects(study, group, szenario).Select(u => u.PId).Contains(t.PId)))
                    {
                        foreach (string
                            turn
                            in
                            turns)
                        {
                            if (_manipAnalysisFunctions.GetTurns(study, group, szenario, subject) != null)
                            {
                                if (listBox_DescriptiveStatistic1_SelectedTrials.Items.Count > 0)
                                {
                                    bool canBeUpdated = false;
                                    foreach (StatisticPlotContainer
                                        temp
                                        in
                                        listBox_DescriptiveStatistic1_SelectedTrials.Items)
                                    {
                                        if (temp.UpdateStatisticPlotContainer(study, group, szenario, subject, turn, trials))
                                        {
                                            typeof (ListBox).InvokeMember("RefreshItems", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, listBox_DescriptiveStatistic1_SelectedTrials, new object[] {});
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

        private void tabPage_DescriptiveStatistic1_Enter(object sender, EventArgs e)
        {
            listBox_DescriptiveStatistic1_TrialType.Items.Clear();
            listBox_DescriptiveStatistic1_ForceField.Items.Clear();
            listBox_DescriptiveStatistic1_Handedness.Items.Clear();

            listBox_DescriptiveStatistic1_TrialType.Items.AddRange(Enum.GetNames(typeof (Trial.TrialTypeEnum)));
            listBox_DescriptiveStatistic1_ForceField.Items.AddRange(Enum.GetNames(typeof (Trial.ForceFieldTypeEnum)));
            listBox_DescriptiveStatistic1_Handedness.Items.AddRange(Enum.GetNames(typeof (Trial.HandednessEnum)));

            for (int listboxIndex = 0; listboxIndex < listBox_DescriptiveStatistic1_TrialType.Items.Count; listboxIndex
                                                                                                               ++)
            {
                listBox_DescriptiveStatistic1_TrialType.SetSelected(listboxIndex, true);
            }

            for (int listboxIndex = 0; listboxIndex < listBox_DescriptiveStatistic1_ForceField.Items.Count; listboxIndex
                                                                                                                ++)
            {
                listBox_DescriptiveStatistic1_ForceField.SetSelected(listboxIndex, true);
            }

            for (int listboxIndex = 0; listboxIndex < listBox_DescriptiveStatistic1_Handedness.Items.Count; listboxIndex
                                                                                                                ++)
            {
                listBox_DescriptiveStatistic1_Handedness.SetSelected(listboxIndex, true);
            }

            listBox_DescriptiveStatistic1_TrialType.SelectedIndexChanged += listBox_DescriptiveStatistic1_Turns_SelectedIndexChanged;
            listBox_DescriptiveStatistic1_ForceField.SelectedIndexChanged += listBox_DescriptiveStatistic1_Turns_SelectedIndexChanged;
            listBox_DescriptiveStatistic1_Handedness.SelectedIndexChanged += listBox_DescriptiveStatistic1_Turns_SelectedIndexChanged;

            comboBox_DescriptiveStatistic1_DataTypeSelect.SelectedIndex = 0;

            comboBox_DescriptiveStatistic1_Study.Items.Clear();
            listBox_DescriptiveStatistic1_Groups.Items.Clear();
            comboBox_DescriptiveStatistic1_Szenario.Items.Clear();
            listBox_DescriptiveStatistic1_Subjects.Items.Clear();
            listBox_DescriptiveStatistic1_Turns.Items.Clear();
            listBox_DescriptiveStatistic1_Trials.Items.Clear();

            IEnumerable<string> studyNames = _manipAnalysisFunctions.GetStudys();
            if (studyNames.Any())
            {
                comboBox_DescriptiveStatistic1_Study.Items.AddRange(studyNames.ToArray());
                if (comboBox_DescriptiveStatistic1_Study.Items.Count > 0)
                {
                    comboBox_DescriptiveStatistic1_Study.SelectedIndex = 0;
                }
            }
        }

        private void comboBox_DescriptiveStatistic2_Study_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox_DescriptiveStatistic2_Groups.Items.Clear();
            comboBox_DescriptiveStatistic2_Szenario.Items.Clear();
            listBox_DescriptiveStatistic2_Subjects.Items.Clear();
            listBox_DescriptiveStatistic2_Turns.Items.Clear();
            listBox_DescriptiveStatistic2_Trials.Items.Clear();

            IEnumerable<string> groupNames = _manipAnalysisFunctions.GetGroups(comboBox_DescriptiveStatistic2_Study.SelectedItem.ToString());

            if (groupNames.Any())
            {
                listBox_DescriptiveStatistic2_Groups.Items.AddRange(groupNames.ToArray());
                if (listBox_DescriptiveStatistic2_Groups.Items.Count > 0)
                {
                    listBox_DescriptiveStatistic2_Groups.SelectedIndex = 0;
                }
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

                if (szenarioIntersect.Any())
                {
                    for (int i = 1; i < groups.Length; i++)
                    {
                        szenarioIntersect = szenarioIntersect.Intersect(_manipAnalysisFunctions.GetSzenarios(study, groups[i]));
                    }

                    comboBox_DescriptiveStatistic2_Szenario.Items.AddRange(szenarioIntersect.ToArray());
                }
                if (comboBox_DescriptiveStatistic2_Szenario.Items.Count > 0)
                {
                    comboBox_DescriptiveStatistic2_Szenario.SelectedIndex = 0;
                }
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

            for (int i = 0; i < groups.Length; i
                                                   ++)
            {
                IEnumerable<SubjectContainer> tempSubjects = _manipAnalysisFunctions.GetSubjects(study, groups[i], szenario);
                if (tempSubjects.Any())
                {
                    listBox_DescriptiveStatistic2_Subjects.Items.AddRange(tempSubjects.ToArray());
                }
            }
            if (listBox_DescriptiveStatistic2_Subjects.Items.Count > 0)
            {
                listBox_DescriptiveStatistic2_Subjects.SelectedIndex = 0;
            }
        }

        private void listBox_DescriptiveStatistic2_Subject_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox_DescriptiveStatistic2_Turns.Items.Clear();
            listBox_DescriptiveStatistic2_Trials.Items.Clear();

            string study = comboBox_DescriptiveStatistic2_Study.SelectedItem.ToString();
            string szenario = comboBox_DescriptiveStatistic2_Szenario.SelectedItem.ToString();
            SubjectContainer[] subjects = listBox_DescriptiveStatistic2_Subjects.SelectedItems.Cast<SubjectContainer>().ToArray();

            string[] turnIntersect = null;
            for (int j = 0; j < subjects.Length; j
                                                     ++)
            {
                IEnumerable<string> tempTurnString = _manipAnalysisFunctions.GetTurns(study, szenario, subjects[j]);

                if (tempTurnString.Any())
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

            if (turnIntersect != null)
            {
                listBox_DescriptiveStatistic2_Turns.Items.AddRange(turnIntersect);
            }
            if (listBox_DescriptiveStatistic2_Turns.Items.Count > 0)
            {
                listBox_DescriptiveStatistic2_Turns.SelectedIndex = 0;
            }
        }

        private void listBox_DescriptiveStatistic2_Turns_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox_DescriptiveStatistic2_Trials.Items.Clear();

            string study = comboBox_DescriptiveStatistic2_Study.SelectedItem.ToString();
            string szenario = comboBox_DescriptiveStatistic2_Szenario.SelectedItem.ToString();
            SubjectContainer[] subjects = listBox_DescriptiveStatistic2_Subjects.SelectedItems.Cast<SubjectContainer>().ToArray();
            string[] turns = listBox_DescriptiveStatistic2_Turns.SelectedItems.Cast<string>().ToArray();

            var trialTypes = new List<Trial.TrialTypeEnum>();
            var forceFields = new List<Trial.ForceFieldTypeEnum>();
            var handedness = new List<Trial.HandednessEnum>();

            foreach (string item in listBox_DescriptiveStatistic2_TrialType.SelectedItems)
            {
                trialTypes.Add((Trial.TrialTypeEnum)Enum.Parse(typeof(Trial.TrialTypeEnum), item));
            }

            foreach (string item in listBox_DescriptiveStatistic2_ForceField.SelectedItems)
            {
                forceFields.Add((Trial.ForceFieldTypeEnum)Enum.Parse(typeof(Trial.ForceFieldTypeEnum), item));
            }

            foreach (string item in listBox_DescriptiveStatistic2_Handedness.SelectedItems)
            {
                handedness.Add((Trial.HandednessEnum)Enum.Parse(typeof(Trial.HandednessEnum), item));
            }

            string[] szenarioTrialNamesIntersect = null;

            for (int i = 0; i < subjects.Length; i++)
            {
                IEnumerable<string> szenarioTrialNames = _manipAnalysisFunctions.GetTrialsOfSzenario(study, szenario, subjects[i], trialTypes, forceFields, handedness);

                if (szenarioTrialNames != null)
                {
                    if (szenarioTrialNamesIntersect == null)
                    {
                        szenarioTrialNamesIntersect = szenarioTrialNames.ToArray();
                    }
                    else
                    {
                        szenarioTrialNamesIntersect = szenarioTrialNamesIntersect.Intersect(szenarioTrialNames).ToArray();
                    }
                }
            }

            if (szenarioTrialNamesIntersect.Any())
            {
                listBox_DescriptiveStatistic2_Trials.Items.AddRange(szenarioTrialNamesIntersect);
                if (listBox_DescriptiveStatistic2_Trials.Items.Count > 0)
                {
                    listBox_DescriptiveStatistic2_Trials.SelectedIndex = 0;
                }
            }
        }

        private void button_DescriptiveStatistic2_AddSelected_Click(object sender, EventArgs e)
        {
            if (comboBox_DescriptiveStatistic2_Study.SelectedItem != null)
            {
                string study = comboBox_DescriptiveStatistic2_Study.SelectedItem.ToString();
                string[] groups = listBox_DescriptiveStatistic2_Groups.SelectedItems.Cast<string>().ToArray();
                string szenario = comboBox_DescriptiveStatistic2_Szenario.SelectedItem.ToString();
                SubjectContainer[] subjects = listBox_DescriptiveStatistic2_Subjects.SelectedItems.Cast<SubjectContainer>().ToArray();
                string[] turns = listBox_DescriptiveStatistic2_Turns.SelectedItems.Cast<string>().ToArray();
                string[] trials = listBox_DescriptiveStatistic2_Trials.SelectedItems.Cast<string>().ToArray();

                foreach (string
                    group
                    in
                    groups)
                {
                    foreach (SubjectContainer
                        subject
                        in
                        subjects.Where(t => _manipAnalysisFunctions.GetSubjects(study, group, szenario).Select(u => u.PId).Contains(t.PId)))
                    {
                        foreach (string
                            turn
                            in
                            turns)
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
                SubjectContainer[] subjects = listBox_DescriptiveStatistic2_Subjects.SelectedItems.Cast<SubjectContainer>().ToArray();
                string[] turns = listBox_DescriptiveStatistic2_Turns.SelectedItems.Cast<string>().ToArray();
                string[] trials = listBox_DescriptiveStatistic2_Trials.Items.Cast<string>().ToArray();

                foreach (string
                    group
                    in
                    groups)
                {
                    foreach (SubjectContainer
                        subject
                        in
                        subjects.Where(t => _manipAnalysisFunctions.GetSubjects(study, group, szenario).Select(u => u.PId).Contains(t.PId)))
                    {
                        foreach (string
                            turn
                            in
                            turns)
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
            listBox_DescriptiveStatistic2_TrialType.Items.Clear();
            listBox_DescriptiveStatistic2_ForceField.Items.Clear();
            listBox_DescriptiveStatistic2_Handedness.Items.Clear();

            listBox_DescriptiveStatistic2_TrialType.Items.AddRange(Enum.GetNames(typeof (Trial.TrialTypeEnum)));
            listBox_DescriptiveStatistic2_ForceField.Items.AddRange(Enum.GetNames(typeof (Trial.ForceFieldTypeEnum)));
            listBox_DescriptiveStatistic2_Handedness.Items.AddRange(Enum.GetNames(typeof (Trial.HandednessEnum)));

            for (int listboxIndex = 0; listboxIndex < listBox_DescriptiveStatistic2_TrialType.Items.Count; listboxIndex
                                                                                                               ++)
            {
                listBox_DescriptiveStatistic2_TrialType.SetSelected(listboxIndex, true);
            }

            for (int listboxIndex = 0; listboxIndex < listBox_DescriptiveStatistic2_ForceField.Items.Count; listboxIndex
                                                                                                                ++)
            {
                listBox_DescriptiveStatistic2_ForceField.SetSelected(listboxIndex, true);
            }

            for (int listboxIndex = 0; listboxIndex < listBox_DescriptiveStatistic2_Handedness.Items.Count; listboxIndex
                                                                                                                ++)
            {
                listBox_DescriptiveStatistic2_Handedness.SetSelected(listboxIndex, true);
            }

            listBox_DescriptiveStatistic2_TrialType.SelectedIndexChanged += listBox_DescriptiveStatistic2_Turns_SelectedIndexChanged;
            listBox_DescriptiveStatistic2_ForceField.SelectedIndexChanged += listBox_DescriptiveStatistic2_Turns_SelectedIndexChanged;
            listBox_DescriptiveStatistic2_Handedness.SelectedIndexChanged += listBox_DescriptiveStatistic2_Turns_SelectedIndexChanged;

            comboBox_DescriptiveStatistic2_DataTypeSelect.SelectedIndex = 0;

            comboBox_DescriptiveStatistic2_Study.Items.Clear();
            listBox_DescriptiveStatistic2_Groups.Items.Clear();
            comboBox_DescriptiveStatistic2_Szenario.Items.Clear();
            listBox_DescriptiveStatistic2_Subjects.Items.Clear();
            listBox_DescriptiveStatistic2_Turns.Items.Clear();
            listBox_DescriptiveStatistic2_Trials.Items.Clear();

            IEnumerable<string> studyNames = _manipAnalysisFunctions.GetStudys();

            if (studyNames.Any())
            {
                comboBox_DescriptiveStatistic2_Study.Items.AddRange(studyNames.ToArray());
                if (comboBox_DescriptiveStatistic2_Study.Items.Count > 0)
                {
                    comboBox_DescriptiveStatistic2_Study.SelectedIndex = 0;
                }
            }
        }

        private void button_DescriptiveStatistic2_CalculateMeanValues_Click(object sender, EventArgs e)
        {
            int pdTime = 300;
            if (comboBox_DescriptiveStatistic2_DataTypeSelect.SelectedItem.ToString() == "PD - Abs" || comboBox_DescriptiveStatistic2_DataTypeSelect.SelectedItem.ToString() == "PD - Sign")
            {
                var inputForm = new PerpendicularDisplacementTimeInputForm();
                if (inputForm.ShowDialog(this) == DialogResult.OK)
                {
                    pdTime = inputForm.getMilliseconds();
                    inputForm.Dispose();
                }
            }

            if (pdTime < 0)
            {
                WriteToLogBox("Negative PD-Times are not allowed!");
            }
            else
            {
                WriteProgressInfo("Getting data...");
                saveFileDialog = new SaveFileDialog();
                saveFileDialog.Reset();
                saveFileDialog.Title = @"Save mean data file";
                saveFileDialog.AddExtension = true;
                saveFileDialog.DefaultExt = ".csv";
                saveFileDialog.Filter = @"DataFiles (*.csv)|.csv";
                saveFileDialog.OverwritePrompt = true;
                saveFileDialog.FileName = DateTime.Now.Year.ToString("0000") + "." + DateTime.Now.Month.ToString("00") + "." + DateTime.Now.Day.ToString("00") + "-" + DateTime.Now.Hour.ToString("00") + "." + DateTime.Now.Minute.ToString("00") + "-mean-" + comboBox_DescriptiveStatistic2_DataTypeSelect.SelectedItem + "-data";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (listBox_DescriptiveStatistic2_SelectedTrials.Items.Count > 0)
                    {
                        _manipAnalysisFunctions.ExportDescriptiveStatistic2Data(listBox_DescriptiveStatistic2_SelectedTrials.Items.Cast<StatisticPlotContainer>(), comboBox_DescriptiveStatistic2_DataTypeSelect.SelectedItem.ToString(), saveFileDialog.FileName, pdTime);
                    }
                }
            }
        }

        private void button_DescriptiveStatistic1_ExportData_Click(object sender, EventArgs e)
        {
            int pdTime = 300;
            if (comboBox_DescriptiveStatistic1_DataTypeSelect.SelectedItem.ToString() == "PD - Abs" || comboBox_DescriptiveStatistic1_DataTypeSelect.SelectedItem.ToString() == "PD - Sign")
            {
                var inputForm = new PerpendicularDisplacementTimeInputForm();
                if (inputForm.ShowDialog(this) == DialogResult.OK)
                {
                    pdTime = inputForm.getMilliseconds();
                    inputForm.Dispose();
                }
            }

            if (pdTime < 0)
            {
                WriteToLogBox("Negative PD-Times are not allowed!");
            }
            else
            {
                WriteProgressInfo("Getting data...");
                saveFileDialog = new SaveFileDialog();
                saveFileDialog.Reset();
                saveFileDialog.Title = @"Save mean data file";
                saveFileDialog.AddExtension = true;
                saveFileDialog.DefaultExt = ".csv";
                saveFileDialog.Filter = @"DataFiles (*.csv)|.csv";
                saveFileDialog.OverwritePrompt = true;
                saveFileDialog.FileName = DateTime.Now.Year.ToString("0000") + "." + DateTime.Now.Month.ToString("00") + "." + DateTime.Now.Day.ToString("00") + "-" + DateTime.Now.Hour.ToString("00") + "." + DateTime.Now.Minute.ToString("00") + "." + DateTime.Now.Second.ToString("00") + "-mean-" + comboBox_DescriptiveStatistic1_DataTypeSelect.SelectedItem.ToString().Replace("-", pdTime.ToString()) + "-data";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (listBox_DescriptiveStatistic1_SelectedTrials.Items.Count > 0)
                    {
                        _manipAnalysisFunctions.PlotExportDescriptiveStatistic1(listBox_DescriptiveStatistic1_SelectedTrials.Items.Cast<StatisticPlotContainer>(), comboBox_DescriptiveStatistic1_DataTypeSelect.SelectedItem.ToString(), null, pdTime, false, false, saveFileDialog.FileName);
                    }
                }
            }
        }

        private void button_Debug_ShowMatlabFiles_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\MatlabFiles");
            }
            catch (Exception
                ex)
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

        private void button_ImportMeasureFiles_Click(object sender, EventArgs e)
        {
            _manipAnalysisFunctions.ImportMeasureFiles(listBox_Import_SelectedMeasureFiles.Items.Cast<string>().ToList(), Convert.ToInt32(textBox_Import_SamplesPerSec.Text), Convert.ToInt32(textBox_Import_FilterOrder.Text), Convert.ToInt32(textBox_Import_CutoffFreqPosition.Text), Convert.ToInt32(textBox_Import_CutoffFreqForce.Text), Convert.ToInt32(textBox_Import_PercentPeakVelocity.Text), Convert.ToInt32(textBox_Import_NewSampleCount.Text));
        }

        private void button_CalculateBaselines_Click(object sender, EventArgs e)
        {
            _manipAnalysisFunctions.CalculateBaselines();
        }

        private void button_CalculateStatistics_Click(object sender, EventArgs e)
        {
            _manipAnalysisFunctions.CalculateStatistics();
        }

        private void button_Auto_Click(object sender, EventArgs e)
        {
            button_DataManipulation_EnsureIndexes_Click(sender, e);
            button_ImportMeasureFiles_Click(sender, e);
            button_CalculateBaselines_Click(sender, e);
            button_CalculateStatistics_Click(sender, e);
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
            saveFileDialog.FileName = DateTime.Now.Year.ToString("0000") + "." + DateTime.Now.Month.ToString("00") + "." + DateTime.Now.Day.ToString("00") + "-" + DateTime.Now.Hour.ToString("00") + "." + DateTime.Now.Minute.ToString("00") + "-LogFile";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var logFileStream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
                var logFileWriter = new StreamWriter(logFileStream);

                string[] logText = GetLogBoxText();
                for (int i = 0; i < logText.Length; i
                                                        ++)
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
            listBox_TrajectoryVelocity_TrialType.Items.Clear();
            listBox_TrajectoryVelocity_ForceField.Items.Clear();
            listBox_TrajectoryVelocity_Handedness.Items.Clear();

            listBox_TrajectoryVelocity_TrialType.Items.AddRange(Enum.GetNames(typeof (Trial.TrialTypeEnum)));
            listBox_TrajectoryVelocity_ForceField.Items.AddRange(Enum.GetNames(typeof (Trial.ForceFieldTypeEnum)));
            listBox_TrajectoryVelocity_Handedness.Items.AddRange(Enum.GetNames(typeof (Trial.HandednessEnum)));

            for (int listboxIndex = 0; listboxIndex < listBox_TrajectoryVelocity_TrialType.Items.Count; listboxIndex
                                                                                                            ++)
            {
                listBox_TrajectoryVelocity_TrialType.SetSelected(listboxIndex, true);
            }

            for (int listboxIndex = 0; listboxIndex < listBox_TrajectoryVelocity_ForceField.Items.Count; listboxIndex
                                                                                                             ++)
            {
                listBox_TrajectoryVelocity_ForceField.SetSelected(listboxIndex, true);
            }

            for (int listboxIndex = 0; listboxIndex < listBox_TrajectoryVelocity_Handedness.Items.Count; listboxIndex
                                                                                                             ++)
            {
                listBox_TrajectoryVelocity_Handedness.SetSelected(listboxIndex, true);
            }

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
            if (studyNames.Any())
            {
                comboBox_TrajectoryVelocity_Study.Items.AddRange(studyNames.ToArray());
                if (comboBox_TrajectoryVelocity_Study.Items.Count > 0)
                {
                    comboBox_TrajectoryVelocity_Study.SelectedIndex = 0;
                }
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

            IEnumerable<string> groupNames = _manipAnalysisFunctions.GetGroups(comboBox_TrajectoryVelocity_Study.SelectedItem.ToString());
            if (groupNames.Any())
            {
                listBox_TrajectoryVelocity_Groups.Items.AddRange(groupNames.ToArray());
                if (listBox_TrajectoryVelocity_Groups.Items.Count > 0)
                {
                    listBox_TrajectoryVelocity_Groups.SelectedIndex = 0;
                }
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
                if (szenarioIntersect.Any())
                {
                    for (int i = 1; i < groups.Length; i
                                                           ++)
                    {
                        szenarioIntersect = szenarioIntersect.Intersect(_manipAnalysisFunctions.GetSzenarios(study, groups[i]));
                    }

                    comboBox_TrajectoryVelocity_Szenario.Items.AddRange(szenarioIntersect.ToArray());
                    if (comboBox_TrajectoryVelocity_Szenario.Items.Count > 0)
                    {
                        comboBox_TrajectoryVelocity_Szenario.SelectedIndex = 0;
                    }
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
                IEnumerable<SubjectContainer> subjects = _manipAnalysisFunctions.GetSubjects(study, groups[i], szenario);
                if (subjects != null)
                {
                    listBox_TrajectoryVelocity_Subjects.Items.AddRange(subjects.ToArray());
                }
            }
            if (listBox_TrajectoryVelocity_Subjects.Items.Count > 0)
            {
                listBox_TrajectoryVelocity_Subjects.SelectedIndex = 0;
            }
        }

        private void listBox_TrajectoryVelocity_Subjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox_TrajectoryVelocity_Turns.Items.Clear();
            listBox_TrajectoryVelocity_Targets.Items.Clear();
            listBox_TrajectoryVelocity_Trials.Items.Clear();

            string study = comboBox_TrajectoryVelocity_Study.SelectedItem.ToString();
            string szenario = comboBox_TrajectoryVelocity_Szenario.SelectedItem.ToString();
            SubjectContainer[] subjects = listBox_TrajectoryVelocity_Subjects.SelectedItems.Cast<SubjectContainer>().ToArray();

            string[] turnIntersect = null;

            for (int j = 0; j < subjects.Length; j
                                                     ++)
            {
                IEnumerable<string> tempTurnString = _manipAnalysisFunctions.GetTurns(study, szenario, subjects[j]);

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


            if (turnIntersect != null)
            {
                listBox_TrajectoryVelocity_Turns.Items.AddRange(turnIntersect);
            }
            if (listBox_TrajectoryVelocity_Turns.Items.Count > 0)
            {
                listBox_TrajectoryVelocity_Turns.SelectedIndex = 0;
            }
        }

        private void listBox_TrajectoryVelocity_Turns_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox_TrajectoryVelocity_Targets.Items.Clear();
            listBox_TrajectoryVelocity_Trials.Items.Clear();

            string study = comboBox_TrajectoryVelocity_Study.SelectedItem.ToString();
            string szenario = comboBox_TrajectoryVelocity_Szenario.SelectedItem.ToString();

            IEnumerable<string> targets = _manipAnalysisFunctions.GetTargets(study, szenario);
            IEnumerable<string> trials = _manipAnalysisFunctions.GetTrials(study, szenario);

            if (targets.Any())
            {
                listBox_TrajectoryVelocity_Targets.Items.AddRange(targets.OrderBy(t => t).ToArray());
                if (listBox_TrajectoryVelocity_Targets.Items.Count > 0)
                {
                    listBox_TrajectoryVelocity_Targets.SelectedIndex = 0;
                }
            }

            if (trials.Any())
            {
                listBox_TrajectoryVelocity_Trials.Items.AddRange(trials.OrderBy(t => t).ToArray());
                if (listBox_TrajectoryVelocity_Trials.Items.Count > 0)
                {
                    listBox_TrajectoryVelocity_Trials.SelectedIndex = 0;
                }
            }
        }

        private void button_TrajectoryVelocity_AddSelected_Click(object sender, EventArgs e)
        {
            if (comboBox_TrajectoryVelocity_Study.SelectedItem != null)
            {
                string study = comboBox_TrajectoryVelocity_Study.SelectedItem.ToString();
                string[] groups = listBox_TrajectoryVelocity_Groups.SelectedItems.Cast<string>().ToArray();
                string szenario = comboBox_TrajectoryVelocity_Szenario.SelectedItem.ToString();
                SubjectContainer[] subjects = listBox_TrajectoryVelocity_Subjects.SelectedItems.Cast<SubjectContainer>().ToArray();
                string[] turns = listBox_TrajectoryVelocity_Turns.SelectedItems.Cast<string>().ToArray();
                string[] targets = listBox_TrajectoryVelocity_Targets.SelectedItems.Cast<string>().ToArray();
                string[] trials = listBox_TrajectoryVelocity_Trials.SelectedItems.Cast<string>().ToArray();

                foreach (string group in groups)
                {
                    foreach (SubjectContainer subject in subjects.Where(t => _manipAnalysisFunctions.GetSubjects(study, group, szenario).Select(u => u.PId).Contains(t.PId)))
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
                                        foreach (TrajectoryVelocityPlotContainer temp in listBox_TrajectoryVelocity_SelectedTrials.Items)
                                        {
                                            if (temp.UpdateTrajectoryVelocityPlotContainer(study, group, szenario, subject, turn, target, trials))
                                            {
                                                typeof (ListBox).InvokeMember("RefreshItems", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, listBox_TrajectoryVelocity_SelectedTrials, new object[] {});
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
                SubjectContainer[] subjects = listBox_TrajectoryVelocity_Subjects.SelectedItems.Cast<SubjectContainer>().ToArray();
                string[] turns = listBox_TrajectoryVelocity_Turns.SelectedItems.Cast<string>().ToArray();
                string[] targets = listBox_TrajectoryVelocity_Targets.SelectedItems.Cast<string>().ToArray();
                string[] trials = listBox_TrajectoryVelocity_Trials.Items.Cast<string>().ToArray();

                foreach (string group in groups)
                {
                    foreach (SubjectContainer subject in subjects.Where(t => _manipAnalysisFunctions.GetSubjects(study, group, szenario).Select(u => u.PId).Contains(t.PId)))
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
                                        foreach (TrajectoryVelocityPlotContainer temp in listBox_TrajectoryVelocity_SelectedTrials.Items)
                                        {
                                            if (temp.UpdateTrajectoryVelocityPlotContainer(study, group, szenario, subject, turn, target, trials))
                                            {
                                                typeof (ListBox).InvokeMember("RefreshItems", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, listBox_TrajectoryVelocity_SelectedTrials, new object[] {});
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
                var trialTypes = new List<Trial.TrialTypeEnum>();
                var forceFields = new List<Trial.ForceFieldTypeEnum>();
                var handedness = new List<Trial.HandednessEnum>();

                foreach (string item in listBox_TrajectoryVelocity_TrialType.SelectedItems)
                {
                    trialTypes.Add((Trial.TrialTypeEnum) Enum.Parse(typeof (Trial.TrialTypeEnum), item));
                }

                foreach (string item in listBox_TrajectoryVelocity_ForceField.SelectedItems)
                {
                    forceFields.Add((Trial.ForceFieldTypeEnum) Enum.Parse(typeof (Trial.ForceFieldTypeEnum), item));
                }

                foreach (string item in listBox_TrajectoryVelocity_Handedness.SelectedItems)
                {
                    handedness.Add((Trial.HandednessEnum) Enum.Parse(typeof (Trial.HandednessEnum), item));
                }

                _manipAnalysisFunctions.PlotTrajectoryVelocityForce(listBox_TrajectoryVelocity_SelectedTrials.Items.Cast<TrajectoryVelocityPlotContainer>(), comboBox_TrajectoryVelocity_IndividualMean.SelectedItem.ToString(), comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString(), trialTypes, forceFields, handedness, checkBox_TrajectoryVelocity_ShowForceVectors.Checked, checkBox_TrajectoryVelocity_ShowPDForceVectors.Checked);
            }
            else
            {
                WriteToLogBox("Please add data to plot!");
            }
        }

        private void tabPage_Others_Enter(object sender, EventArgs e)
        {
            listBox_OtherStatistics_TrialType.Items.Clear();
            listBox_OtherStatistics_ForceField.Items.Clear();
            listBox_OtherStatistics_Handedness.Items.Clear();

            listBox_OtherStatistics_TrialType.Items.AddRange(Enum.GetNames(typeof (Trial.TrialTypeEnum)));
            listBox_OtherStatistics_ForceField.Items.AddRange(Enum.GetNames(typeof (Trial.ForceFieldTypeEnum)));
            listBox_OtherStatistics_Handedness.Items.AddRange(Enum.GetNames(typeof (Trial.HandednessEnum)));

            for (int listboxIndex = 0; listboxIndex < listBox_OtherStatistics_TrialType.Items.Count; listboxIndex++)
            {
                listBox_OtherStatistics_TrialType.SetSelected(listboxIndex, true);
            }

            for (int listboxIndex = 0; listboxIndex < listBox_OtherStatistics_ForceField.Items.Count; listboxIndex++)
            {
                listBox_OtherStatistics_ForceField.SetSelected(listboxIndex, true);
            }

            for (int listboxIndex = 0; listboxIndex < listBox_OtherStatistics_Handedness.Items.Count; listboxIndex++)
            {
                listBox_OtherStatistics_Handedness.SetSelected(listboxIndex, true);
            }

            comboBox_Others_Study.Items.Clear();
            comboBox_Others_Group.Items.Clear();
            comboBox_Others_Szenario.Items.Clear();
            comboBox_Others_Subject.Items.Clear();
            comboBox_Others_Turn.Items.Clear();

            IEnumerable<string> studyNames = _manipAnalysisFunctions.GetStudys();
            if (studyNames.Any())
            {
                comboBox_Others_Study.Items.AddRange(studyNames.ToArray());
                if (comboBox_Others_Study.Items.Count > 0)
                {
                    comboBox_Others_Study.SelectedIndex = 0;
                }
            }
        }

        private void comboBox_Others_Study_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox_Others_Group.Items.Clear();
            comboBox_Others_Szenario.Items.Clear();
            comboBox_Others_Subject.Items.Clear();
            comboBox_Others_Turn.Items.Clear();

            IEnumerable<string> groupNames = _manipAnalysisFunctions.GetGroups(comboBox_Others_Study.SelectedItem.ToString());
            if (groupNames.Any())
            {
                comboBox_Others_Group.Items.AddRange(groupNames.ToArray());
                if (comboBox_Others_Group.Items.Count > 0)
                {
                    comboBox_Others_Group.SelectedIndex = 0;
                }
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
            if (szenarioNames.Any())
            {
                comboBox_Others_Szenario.Items.AddRange(szenarioNames.ToArray());
                if (comboBox_Others_Szenario.Items.Count > 0)
                {
                    comboBox_Others_Szenario.SelectedIndex = 0;
                }
            }
        }

        private void comboBox_Others_Szenario_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox_Others_Subject.Items.Clear();
            comboBox_Others_Turn.Items.Clear();

            string study = comboBox_Others_Study.SelectedItem.ToString();
            string group = comboBox_Others_Group.SelectedItem.ToString();
            string szenario = comboBox_Others_Szenario.SelectedItem.ToString();

            IEnumerable<SubjectContainer> subjectNames = _manipAnalysisFunctions.GetSubjects(study, group, szenario);
            if (subjectNames.Any())
            {
                comboBox_Others_Subject.Items.AddRange(subjectNames.ToArray());
                if (comboBox_Others_Subject.Items.Count > 0)
                {
                    comboBox_Others_Subject.SelectedIndex = 0;
                }
            }
        }

        private void comboBox_Others_Subject_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox_Others_Turn.Items.Clear();

            string study = comboBox_Others_Study.SelectedItem.ToString();
            string group = comboBox_Others_Group.SelectedItem.ToString();
            string szenario = comboBox_Others_Szenario.SelectedItem.ToString();
            var subject = (SubjectContainer) comboBox_Others_Subject.SelectedItem;

            IEnumerable<string> turnNames = _manipAnalysisFunctions.GetTurns(study, group, szenario, subject);
            if (turnNames.Any())
            {
                comboBox_Others_Turn.Items.AddRange(turnNames.ToArray());
                if (comboBox_Others_Turn.Items.Count > 0)
                {
                    comboBox_Others_Turn.SelectedIndex = 0;
                }
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
            saveFileDialog.FileName = DateTime.Now.Year.ToString("0000") + "." + DateTime.Now.Month.ToString("00") + "." + DateTime.Now.Day.ToString("00") + "-" + DateTime.Now.Hour.ToString("00") + "." + DateTime.Now.Minute.ToString("00") + "-" + comboBox_TrajectoryVelocity_IndividualMean.SelectedItem + "-" + comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem + "-data";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (listBox_TrajectoryVelocity_SelectedTrials.Items.Count != 0)
                {
                    var trialTypes = new List<Trial.TrialTypeEnum>();
                    var forceFields = new List<Trial.ForceFieldTypeEnum>();
                    var handedness = new List<Trial.HandednessEnum>();

                    foreach (string item in listBox_TrajectoryVelocity_TrialType.SelectedItems)
                    {
                        trialTypes.Add((Trial.TrialTypeEnum) Enum.Parse(typeof (Trial.TrialTypeEnum), item));
                    }

                    foreach (string item in listBox_TrajectoryVelocity_ForceField.SelectedItems)
                    {
                        forceFields.Add((Trial.ForceFieldTypeEnum) Enum.Parse(typeof (Trial.ForceFieldTypeEnum), item));
                    }

                    foreach (string item in listBox_TrajectoryVelocity_Handedness.SelectedItems)
                    {
                        handedness.Add((Trial.HandednessEnum) Enum.Parse(typeof (Trial.HandednessEnum), item));
                    }

                    _manipAnalysisFunctions.ExportTrajectoryVelocityForce(listBox_TrajectoryVelocity_SelectedTrials.Items.Cast<TrajectoryVelocityPlotContainer>(), comboBox_TrajectoryVelocity_IndividualMean.SelectedItem.ToString(), comboBox_TrajectoryVelocity_TrajectoryVelocity.SelectedItem.ToString(), trialTypes, forceFields, handedness, checkBox_TrajectoryVelocity_ShowForceVectors.Checked, checkBox_TrajectoryVelocity_ShowPDForceVectors.Checked, saveFileDialog.FileName);
                }
                else
                {
                    WriteToLogBox("Please add data to export!");
                }
            }
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
            saveFileDialog.FileName = DateTime.Now.Year.ToString("0000") + "." + DateTime.Now.Month.ToString("00") + "." + DateTime.Now.Day.ToString("00") + "-" + DateTime.Now.Hour.ToString("00") + "." + DateTime.Now.Minute.ToString("00") + "-" + comboBox_Others_Subject.SelectedItem + "-TrajectoryBaselineData";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                _manipAnalysisFunctions.ExportTrajectoryBaseline(comboBox_Others_Study.SelectedItem.ToString(), comboBox_Others_Group.SelectedItem.ToString(), comboBox_Others_Szenario.SelectedItem.ToString(), (SubjectContainer) comboBox_Others_Subject.SelectedItem, saveFileDialog.FileName);
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
            saveFileDialog.FileName = DateTime.Now.Year.ToString("0000") + "." + DateTime.Now.Month.ToString("00") + "." + DateTime.Now.Day.ToString("00") + "-" + DateTime.Now.Hour.ToString("00") + "." + DateTime.Now.Minute.ToString("00") + "-" + comboBox_Others_Subject.SelectedItem + "-SzenarioMeanTimeData";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                _manipAnalysisFunctions.ExportSzenarioMeanTimes(comboBox_Others_Study.SelectedItem.ToString(), comboBox_Others_Group.SelectedItem.ToString(), comboBox_Others_Szenario.SelectedItem.ToString(), (SubjectContainer) comboBox_Others_Subject.SelectedItem, Convert.ToInt32(comboBox_Others_Turn.SelectedItem.ToString().Substring("Turn".Length)), saveFileDialog.FileName);
            }
        }

        private void button_Import_ClearMeasureFileList_Click(object sender, EventArgs e)
        {
            listBox_Import_SelectedMeasureFiles.Items.Clear();
        }

        private void button_Start_SelectDatabase_Click(object sender, EventArgs e)
        {
            _manipAnalysisFunctions.SetDatabase(comboBox_Start_Database.SelectedItem.ToString());

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

            if (_manipAnalysisFunctions.ConnectToDatabaseServer(comboBox_Start_DatabaseServer.Text) && comboBox_Start_Database.Items.Count > 0)
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

            WriteProgressInfo("Ready");
        }

        public void SetSqlDatabases(IEnumerable<string> databases)
        {
            if (databases != null && databases.Any())
            {
                comboBox_Start_Database.Items.AddRange(databases.ToArray());
            }
            else
            {
                WriteToLogBox("No Databases available!");
            }
        }

        private void button_Others_PlotSzenarioMeanTimes_Click(object sender, EventArgs e)
        {
            _manipAnalysisFunctions.PlotSzenarioMeanTimes(comboBox_Others_Study.SelectedItem.ToString(), comboBox_Others_Group.SelectedItem.ToString(), comboBox_Others_Szenario.SelectedItem.ToString(), (SubjectContainer) comboBox_Others_Subject.SelectedItem, Convert.ToInt32(comboBox_Others_Turn.SelectedItem.ToString().Substring("Turn".Length)));
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
            saveFileDialog.FileName = DateTime.Now.Year.ToString("0000") + "." + DateTime.Now.Month.ToString("00") + "." + DateTime.Now.Day.ToString("00") + "-" + DateTime.Now.Hour.ToString("00") + "." + DateTime.Now.Minute.ToString("00") + "-" + comboBox_Others_Subject.SelectedItem + "-VelocityBaselineData";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                _manipAnalysisFunctions.ExportVelocityBaseline(comboBox_Others_Study.SelectedItem.ToString(), comboBox_Others_Group.SelectedItem.ToString(), comboBox_Others_Szenario.SelectedItem.ToString(), (SubjectContainer) comboBox_Others_Subject.SelectedItem, saveFileDialog.FileName);
            }
        }

        private void button_Others_PlotVelocityBaseline_Click(object sender, EventArgs e)
        {
            var trialTypes = new List<Trial.TrialTypeEnum>();
            var forceFields = new List<Trial.ForceFieldTypeEnum>();
            var handedness = new List<Trial.HandednessEnum>();
            int[] targets = listBox_OtherStatistics_Targets.SelectedItems.Cast<string>().Select(t => Convert.ToInt32(t.Substring(7, 2))).ToArray();

            foreach (string
                item
                in
                listBox_OtherStatistics_TrialType.SelectedItems)
            {
                trialTypes.Add((Trial.TrialTypeEnum) Enum.Parse(typeof (Trial.TrialTypeEnum), item));
            }

            foreach (string
                item
                in
                listBox_OtherStatistics_ForceField.SelectedItems)
            {
                forceFields.Add((Trial.ForceFieldTypeEnum) Enum.Parse(typeof (Trial.ForceFieldTypeEnum), item));
            }

            foreach (string
                item
                in
                listBox_OtherStatistics_Handedness.SelectedItems)
            {
                handedness.Add((Trial.HandednessEnum) Enum.Parse(typeof (Trial.HandednessEnum), item));
            }

            _manipAnalysisFunctions.PlotVelocityBaselines(comboBox_Others_Study.SelectedItem.ToString(), comboBox_Others_Group.SelectedItem.ToString(), (SubjectContainer) comboBox_Others_Subject.SelectedItem, targets, trialTypes, forceFields, handedness);
        }

       

        private void button_DataManipulation_EnsureIndexes_Click(object sender, EventArgs e)
        {
            _manipAnalysisFunctions.EnsureIndexes();
        }

        private void button_DataManipulation_RebuildIndexes_Click(object sender, EventArgs e)
        {
            _manipAnalysisFunctions.RebuildIndexes();
        }

        private void button_DataManipulation_DropIndexes_Click(object sender, EventArgs e)
        {
            _manipAnalysisFunctions.DropIndexes();
        }

        private void button_DataManipulation_CompactDatabase_Click(object sender, EventArgs e)
        {
            _manipAnalysisFunctions.CompactDatabase();
        }

        private void button_DataManipulation_DropStatistics_Click(object sender, EventArgs e)
        {
            _manipAnalysisFunctions.DropStatistics();
        }

        private void button_Others_PlotForceBaseline_Click(object sender, EventArgs e)
        {
            var trialTypes = new List<Trial.TrialTypeEnum>();
            var forceFields = new List<Trial.ForceFieldTypeEnum>();
            var handedness = new List<Trial.HandednessEnum>();
            int[] targets = listBox_OtherStatistics_Targets.SelectedItems.Cast<string>().Select(t => Convert.ToInt32(t.Substring(7, 2))).ToArray();

            foreach (string
                item
                in
                listBox_OtherStatistics_TrialType.SelectedItems)
            {
                trialTypes.Add((Trial.TrialTypeEnum) Enum.Parse(typeof (Trial.TrialTypeEnum), item));
            }

            foreach (string
                item
                in
                listBox_OtherStatistics_ForceField.SelectedItems)
            {
                forceFields.Add((Trial.ForceFieldTypeEnum) Enum.Parse(typeof (Trial.ForceFieldTypeEnum), item));
            }

            foreach (string
                item
                in
                listBox_OtherStatistics_Handedness.SelectedItems)
            {
                handedness.Add((Trial.HandednessEnum) Enum.Parse(typeof (Trial.HandednessEnum), item));
            }

            _manipAnalysisFunctions.PlotForceBaselines(comboBox_Others_Study.SelectedItem.ToString(), comboBox_Others_Group.SelectedItem.ToString(), (SubjectContainer) comboBox_Others_Subject.SelectedItem, targets, trialTypes, forceFields, handedness);
        }

        private void comboBox_Others_Turn_SelectedValueChanged(object sender, EventArgs e)
        {
            listBox_OtherStatistics_Targets.Items.Clear();

            string study = comboBox_TrajectoryVelocity_Study.SelectedItem.ToString();
            string szenario = comboBox_TrajectoryVelocity_Szenario.SelectedItem.ToString();

            IEnumerable<string> targets = _manipAnalysisFunctions.GetTargets(study, szenario);

            if (targets.Any())
            {
                listBox_OtherStatistics_Targets.Items.AddRange(targets.OrderBy(t => t).ToArray());

                for (int listboxIndex = 0; listboxIndex < listBox_OtherStatistics_Targets.Items.Count; listboxIndex
                                                                                                           ++)
                {
                    listBox_OtherStatistics_Targets.SetSelected(listboxIndex, true);
                }
            }
        }

        private delegate void LogBoxCallbackAddString(string text);

        private delegate void LogBoxCallbackClearItems();

        private delegate string[] LogBoxCallbackGetText();

        private delegate void ProgressBarCallback(double value);

        private delegate void ProgressLabelCallback(string text);

        private delegate void TabControlCallback(bool enable);

        private void tabPage_Debug_BaselineRecalculation_Enter(object sender, EventArgs e)
        {
            listBox_BaselineRecalculation_TrialType.Items.Clear();
            listBox_BaselineRecalculation_ForceField.Items.Clear();
            listBox_BaselineRecalculation_Handedness.Items.Clear();

            listBox_BaselineRecalculation_TrialType.Items.AddRange(Enum.GetNames(typeof(Trial.TrialTypeEnum)));
            listBox_BaselineRecalculation_ForceField.Items.AddRange(Enum.GetNames(typeof(Trial.ForceFieldTypeEnum)));
            listBox_BaselineRecalculation_Handedness.Items.AddRange(Enum.GetNames(typeof(Trial.HandednessEnum)));

            if (listBox_BaselineRecalculation_TrialType.Items.Count > 0)
            {
                listBox_BaselineRecalculation_TrialType.SelectedIndex = 0;
            }

            if (listBox_BaselineRecalculation_ForceField.Items.Count > 0)
            {
                listBox_BaselineRecalculation_ForceField.SelectedIndex = 0;
            }

            if (listBox_BaselineRecalculation_Handedness.Items.Count > 0)
            {
                listBox_BaselineRecalculation_Handedness.SelectedIndex = 0;
            }

            comboBox_BaselineRecalculation_Study.Items.Clear();
            comboBox_BaselineRecalculation_Group.Items.Clear();
            comboBox_BaselineRecalculation_Subject.Items.Clear();
            comboBox_BaselineRecalculation_Szenario.Items.Clear();
            comboBox_BaselineRecalculation_Turn.Items.Clear();
            comboBox_BaselineRecalculation_Target.Items.Clear();
            listBox_BaselineRecalculation_Trials.Items.Clear();
            
            IEnumerable<string> studyNames = _manipAnalysisFunctions.GetStudys();
            if (studyNames.Any())
            {
                comboBox_BaselineRecalculation_Study.Items.AddRange(studyNames.ToArray());
                if (comboBox_BaselineRecalculation_Study.Items.Count > 0)
                {
                    comboBox_BaselineRecalculation_Study.SelectedIndex = 0;
                }
            }
        }

        private void comboBox_BaselineRecalculation_Study_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox_BaselineRecalculation_Group.Items.Clear();
            comboBox_BaselineRecalculation_Subject.Items.Clear();
            comboBox_BaselineRecalculation_Szenario.Items.Clear();
            comboBox_BaselineRecalculation_Turn.Items.Clear();
            comboBox_BaselineRecalculation_Target.Items.Clear();
            listBox_BaselineRecalculation_Trials.Items.Clear();

            IEnumerable<string> groupNames = _manipAnalysisFunctions.GetGroups(comboBox_BaselineRecalculation_Study.SelectedItem.ToString());
            if (groupNames.Any())
            {
                comboBox_BaselineRecalculation_Group.Items.AddRange(groupNames.ToArray());
                if (comboBox_BaselineRecalculation_Group.Items.Count > 0)
                {
                    comboBox_BaselineRecalculation_Group.SelectedIndex = 0;
                }
            }
        }

        private void comboBox_BaselineRecalculation_Group_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_BaselineRecalculation_Group.Items.Count > 0)
            {
                comboBox_BaselineRecalculation_Subject.Items.Clear();
                comboBox_BaselineRecalculation_Szenario.Items.Clear();
                comboBox_BaselineRecalculation_Turn.Items.Clear();
                comboBox_BaselineRecalculation_Target.Items.Clear();
                listBox_BaselineRecalculation_Trials.Items.Clear();

                string study = comboBox_BaselineRecalculation_Study.SelectedItem.ToString();
                string group = comboBox_BaselineRecalculation_Group.SelectedItem.ToString();

                IEnumerable<SubjectContainer> subjects = _manipAnalysisFunctions.GetSubjects(study, group);
                if (subjects != null)
                {
                    comboBox_BaselineRecalculation_Subject.Items.AddRange(subjects.ToArray());
                    if (comboBox_BaselineRecalculation_Subject.Items.Count > 0)
                    {
                        comboBox_BaselineRecalculation_Subject.SelectedIndex = 0;
                    }
                }
            }
        }

        private void comboBox_BaselineRecalculation_Subject_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_BaselineRecalculation_Subject.Items.Count > 0)
            {
                comboBox_BaselineRecalculation_Szenario.Items.Clear();
                comboBox_BaselineRecalculation_Turn.Items.Clear();
                comboBox_BaselineRecalculation_Target.Items.Clear();
                listBox_BaselineRecalculation_Trials.Items.Clear();

                string study = comboBox_BaselineRecalculation_Study.SelectedItem.ToString();
                string group = comboBox_BaselineRecalculation_Group.SelectedItem.ToString();
                SubjectContainer subject = (SubjectContainer)comboBox_BaselineRecalculation_Subject.SelectedItem;

                IEnumerable<string> szenarios = _manipAnalysisFunctions.GetSzenarios(study, group, subject);
                if (szenarios.Any())
                {
                    comboBox_BaselineRecalculation_Szenario.Items.AddRange(szenarios.ToArray());
                    if (comboBox_BaselineRecalculation_Szenario.Items.Count > 0)
                    {
                        comboBox_BaselineRecalculation_Szenario.SelectedIndex = 0;
                    }
                }
            }
        }

        private void comboBox_BaselineRecalculation_Szenario_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_BaselineRecalculation_Szenario.Items.Count > 0)
            {
                comboBox_BaselineRecalculation_Turn.Items.Clear();
                comboBox_BaselineRecalculation_Target.Items.Clear();
                listBox_BaselineRecalculation_Trials.Items.Clear();

                string study = comboBox_BaselineRecalculation_Study.SelectedItem.ToString();
                string szenario = comboBox_BaselineRecalculation_Szenario.SelectedItem.ToString();
                SubjectContainer subject = (SubjectContainer)comboBox_BaselineRecalculation_Subject.SelectedItem;

                IEnumerable<string> turns = _manipAnalysisFunctions.GetTurns(study, szenario, subject);

                if (turns != null)
                {
                    comboBox_BaselineRecalculation_Turn.Items.AddRange(turns.ToArray());
                    if (comboBox_BaselineRecalculation_Turn.Items.Count > 0)
                    {
                        comboBox_BaselineRecalculation_Turn.SelectedIndex = 0;
                    }
                }
            }
        }

        private void comboBox_BaselineRecalculation_Turn_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox_BaselineRecalculation_Target.Items.Clear();
            listBox_BaselineRecalculation_Trials.Items.Clear();

            string study = comboBox_BaselineRecalculation_Study.SelectedItem.ToString();
            string szenario = comboBox_BaselineRecalculation_Szenario.SelectedItem.ToString();

            IEnumerable<string> targets = _manipAnalysisFunctions.GetTargets(study, szenario);
            IEnumerable<string> trials = _manipAnalysisFunctions.GetTrials(study, szenario);

            if (targets.Any())
            {
                comboBox_BaselineRecalculation_Target.Items.AddRange(targets.OrderBy(t => t).ToArray());
                if (comboBox_BaselineRecalculation_Target.Items.Count > 0)
                {
                    comboBox_BaselineRecalculation_Target.SelectedIndex = 0;
                }
            }

            if (trials.Any())
            {
                listBox_BaselineRecalculation_Trials.Items.AddRange(trials.OrderBy(t => t).ToArray());
                if (listBox_BaselineRecalculation_Trials.Items.Count > 0)
                {
                    listBox_BaselineRecalculation_Trials.SelectedIndex = 0;
                }
            }
        }

        private void button_BaselineRecalculation_AddSelected_Click(object sender, EventArgs e)
        {
            if (comboBox_BaselineRecalculation_Study.SelectedItem != null)
            {
                string study = comboBox_BaselineRecalculation_Study.SelectedItem.ToString();
                string group = comboBox_BaselineRecalculation_Group.SelectedItem.ToString();
                string szenario = comboBox_BaselineRecalculation_Szenario.SelectedItem.ToString();
                SubjectContainer subject = (SubjectContainer) comboBox_BaselineRecalculation_Subject.SelectedItem;
                string turn = comboBox_BaselineRecalculation_Turn.SelectedItem.ToString();
                string target = comboBox_BaselineRecalculation_Target.SelectedItem.ToString();
                string[] trials = listBox_BaselineRecalculation_Trials.SelectedItems.Cast<string>().ToArray();


                if (_manipAnalysisFunctions.GetTurns(study, group, szenario, subject) != null)
                {
                    if (listBox_BaselineRecalculation_SelectedTrials.Items.Count > 0)
                    {
                        bool canBeUpdated = false;
                        foreach (TrajectoryVelocityPlotContainer temp in listBox_BaselineRecalculation_SelectedTrials.Items)
                        {
                            if (temp.UpdateTrajectoryVelocityPlotContainer(study, group, szenario, subject, turn, target, trials))
                            {
                                typeof (ListBox).InvokeMember("RefreshItems", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, listBox_BaselineRecalculation_SelectedTrials, new object[] {});
                                canBeUpdated = true;
                            }
                        }

                        if (!canBeUpdated)
                        {
                            listBox_BaselineRecalculation_SelectedTrials.Items.Add(new TrajectoryVelocityPlotContainer(study, group, szenario, subject, turn, target, trials));
                        }
                    }
                    else
                    {
                        listBox_BaselineRecalculation_SelectedTrials.Items.Add(new TrajectoryVelocityPlotContainer(study, group, szenario, subject, turn, target, trials));
                    }
                }
            }
        }

        private void button_BaselineRecalculation_AddAll_Click(object sender, EventArgs e)
        {
            if (comboBox_BaselineRecalculation_Study.SelectedItem != null)
            {
                string study = comboBox_BaselineRecalculation_Study.SelectedItem.ToString();
                string group = comboBox_BaselineRecalculation_Group.SelectedItem.ToString();
                string szenario = comboBox_BaselineRecalculation_Szenario.SelectedItem.ToString();
                SubjectContainer subject = (SubjectContainer) comboBox_BaselineRecalculation_Subject.SelectedItem;
                string turn = comboBox_BaselineRecalculation_Turn.SelectedItem.ToString();
                string target = comboBox_BaselineRecalculation_Target.SelectedItem.ToString();
                string[] trials = listBox_BaselineRecalculation_Trials.Items.Cast<string>().ToArray();


                if (_manipAnalysisFunctions.GetTurns(study, group, szenario, subject) != null)
                {
                    if (listBox_BaselineRecalculation_SelectedTrials.Items.Count > 0)
                    {
                        bool canBeUpdated = false;
                        foreach (TrajectoryVelocityPlotContainer temp in listBox_BaselineRecalculation_SelectedTrials.Items)
                        {
                            if (temp.UpdateTrajectoryVelocityPlotContainer(study, group, szenario, subject, turn, target, trials))
                            {
                                typeof (ListBox).InvokeMember("RefreshItems", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, listBox_BaselineRecalculation_SelectedTrials, new object[] {});
                                canBeUpdated = true;
                            }
                        }

                        if (!canBeUpdated)
                        {
                            listBox_BaselineRecalculation_SelectedTrials.Items.Add(new TrajectoryVelocityPlotContainer(study, group, szenario, subject, turn, target, trials));
                        }
                    }
                    else
                    {
                        listBox_BaselineRecalculation_SelectedTrials.Items.Add(new TrajectoryVelocityPlotContainer(study, group, szenario, subject, turn, target, trials));
                    }
                }
            }

        }

        private void button_BaselineRecalculation_ClearSelected_Click(object sender, EventArgs e)
        {
            while (listBox_BaselineRecalculation_SelectedTrials.SelectedItems.Count > 0)
            {
                listBox_BaselineRecalculation_SelectedTrials.Items.Remove(listBox_BaselineRecalculation_SelectedTrials.SelectedItem);
            }
        }

        private void button_BaselineRecalculation_ClearAll_Click(object sender, EventArgs e)
        {
            listBox_BaselineRecalculation_SelectedTrials.Items.Clear();
        }

        private void button_BaselineRecalculation_RecalculateBaselines_Click(object sender, EventArgs e)
        {
            if (listBox_BaselineRecalculation_SelectedTrials.Items.Count != 0)
            {
                string study = comboBox_BaselineRecalculation_Study.SelectedItem.ToString();
                string group = comboBox_BaselineRecalculation_Group.SelectedItem.ToString();
                string szenario = comboBox_BaselineRecalculation_Szenario.SelectedItem.ToString();
                SubjectContainer subject = (SubjectContainer) comboBox_BaselineRecalculation_Subject.SelectedItem;
                int turn = Convert.ToInt32(comboBox_BaselineRecalculation_Turn.SelectedItem.ToString().Substring(5, 1));
                int target = Convert.ToInt32(comboBox_BaselineRecalculation_Target.SelectedItem.ToString().Substring(7, 2));

                var trialType = (Trial.TrialTypeEnum) Enum.Parse(typeof (Trial.TrialTypeEnum), listBox_BaselineRecalculation_TrialType.SelectedItem.ToString());
                var forceField = (Trial.ForceFieldTypeEnum)Enum.Parse(typeof(Trial.ForceFieldTypeEnum), listBox_BaselineRecalculation_ForceField.SelectedItem.ToString());
                var handedness = (Trial.HandednessEnum)Enum.Parse(typeof(Trial.HandednessEnum), listBox_BaselineRecalculation_Handedness.SelectedItem.ToString());

                List<TrajectoryVelocityPlotContainer> selectedTrials = listBox_BaselineRecalculation_SelectedTrials.Items.Cast<TrajectoryVelocityPlotContainer>().ToList();

                if (selectedTrials.All(t => t.Study == study && t.Group == group && t.Szenario == szenario && t.Subject == subject && t.Turn == turn))
                {
                    _manipAnalysisFunctions.RecalculateBaselines(selectedTrials, trialType, forceField, handedness);
                }
                else
                {
                    WriteToLogBox("Please only add data for ONE Study / Group / Szenario / Subject / Turn combination!");
                }
            }
            else
            {
                WriteToLogBox("Please add data!");
            }
        }
    }
}