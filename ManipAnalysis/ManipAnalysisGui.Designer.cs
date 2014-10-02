using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ManipAnalysis_v2
{
    partial class ManipAnalysisGui
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManipAnalysisGui));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage_Start = new System.Windows.Forms.TabPage();
            this.comboBox_Start_DatabaseServer = new System.Windows.Forms.ComboBox();
            this.label_Start_ServerIP = new System.Windows.Forms.Label();
            this.button_Start_SelectDatabase = new System.Windows.Forms.Button();
            this.comboBox_Start_Database = new System.Windows.Forms.ComboBox();
            this.label_Start_Database = new System.Windows.Forms.Label();
            this.button_Start_ConnectToSQlServer = new System.Windows.Forms.Button();
            this.label_Start_ManipAnalysis = new System.Windows.Forms.Label();
            this.tabPage_VisualizationExport = new System.Windows.Forms.TabPage();
            this.tabControl_VisualizationExport = new System.Windows.Forms.TabControl();
            this.tabPage_TrajectoryVelocity = new System.Windows.Forms.TabPage();
            this.label_TrajectoryVelocity_Handedness = new System.Windows.Forms.Label();
            this.label_TrajectoryVelocity_ForceField = new System.Windows.Forms.Label();
            this.label_TrajectoryVelocity_TrialType = new System.Windows.Forms.Label();
            this.listBox_TrajectoryVelocity_Handedness = new System.Windows.Forms.ListBox();
            this.listBox_TrajectoryVelocity_ForceField = new System.Windows.Forms.ListBox();
            this.listBox_TrajectoryVelocity_TrialType = new System.Windows.Forms.ListBox();
            this.checkBox_TrajectoryVelocity_ShowPDForceVectors = new System.Windows.Forms.CheckBox();
            this.checkBox_TrajectoryVelocity_ShowForceVectors = new System.Windows.Forms.CheckBox();
            this.button_TrajectoryVelocity_Export = new System.Windows.Forms.Button();
            this.comboBox_TrajectoryVelocity_IndividualMean = new System.Windows.Forms.ComboBox();
            this.comboBox_TrajectoryVelocity_TrajectoryVelocity = new System.Windows.Forms.ComboBox();
            this.label_TrajectoryVelocity_Targets = new System.Windows.Forms.Label();
            this.listBox_TrajectoryVelocity_Targets = new System.Windows.Forms.ListBox();
            this.listBox_TrajectoryVelocity_Turns = new System.Windows.Forms.ListBox();
            this.label_TrajectoryVelocity_Turns = new System.Windows.Forms.Label();
            this.button_TrajectoryVelocity_AddAll = new System.Windows.Forms.Button();
            this.button_TrajectoryVelocity_Plot = new System.Windows.Forms.Button();
            this.button_TrajectoryVelocity_ClearAll = new System.Windows.Forms.Button();
            this.button_TrajectoryVelocity_ClearSelected = new System.Windows.Forms.Button();
            this.listBox_TrajectoryVelocity_SelectedTrials = new System.Windows.Forms.ListBox();
            this.button_TrajectoryVelocity_AddSelected = new System.Windows.Forms.Button();
            this.label_TrajectoryVelocity_Trials = new System.Windows.Forms.Label();
            this.label_TrajectoryVelocity_Subjects = new System.Windows.Forms.Label();
            this.label_TrajectoryVelocity_Szenario = new System.Windows.Forms.Label();
            this.label_TrajectoryVelocity_Groups = new System.Windows.Forms.Label();
            this.label_TrajectoryVelocity_Study = new System.Windows.Forms.Label();
            this.listBox_TrajectoryVelocity_Trials = new System.Windows.Forms.ListBox();
            this.listBox_TrajectoryVelocity_Subjects = new System.Windows.Forms.ListBox();
            this.comboBox_TrajectoryVelocity_Szenario = new System.Windows.Forms.ComboBox();
            this.comboBox_TrajectoryVelocity_Study = new System.Windows.Forms.ComboBox();
            this.listBox_TrajectoryVelocity_Groups = new System.Windows.Forms.ListBox();
            this.tabPage_DescriptiveStatistic1 = new System.Windows.Forms.TabPage();
            this.label_DescriptiveStatistic1_Handedness = new System.Windows.Forms.Label();
            this.label_DescriptiveStatistic1_ForceField = new System.Windows.Forms.Label();
            this.label_DescriptiveStatistic1_TrialType = new System.Windows.Forms.Label();
            this.listBox_DescriptiveStatistic1_Handedness = new System.Windows.Forms.ListBox();
            this.listBox_DescriptiveStatistic1_ForceField = new System.Windows.Forms.ListBox();
            this.listBox_DescriptiveStatistic1_TrialType = new System.Windows.Forms.ListBox();
            this.checkBox_DescriptiveStatistic1_PlotFit = new System.Windows.Forms.CheckBox();
            this.textBox_DescriptiveStatistic1_FitEquation = new System.Windows.Forms.TextBox();
            this.button_DescriptiveStatistic1_ExportData = new System.Windows.Forms.Button();
            this.listBox_DescriptiveStatistic1_Turns = new System.Windows.Forms.ListBox();
            this.label_DescriptiveStatistic1_Turns = new System.Windows.Forms.Label();
            this.checkBox_DescriptiveStatistic1_PlotErrorbars = new System.Windows.Forms.CheckBox();
            this.button_DescriptiveStatistic1_AddAll = new System.Windows.Forms.Button();
            this.comboBox_DescriptiveStatistic1_DataTypeSelect = new System.Windows.Forms.ComboBox();
            this.button_DescriptiveStatistic1_PlotMeanStd = new System.Windows.Forms.Button();
            this.button_DescriptiveStatistic1_ClearAll = new System.Windows.Forms.Button();
            this.button_DescriptiveStatistic1_ClearSelected = new System.Windows.Forms.Button();
            this.listBox_DescriptiveStatistic1_SelectedTrials = new System.Windows.Forms.ListBox();
            this.button_DescriptiveStatistic1_AddSelected = new System.Windows.Forms.Button();
            this.label_DescriptiveStatistic1_Trials = new System.Windows.Forms.Label();
            this.label_DescriptiveStatistic1_Subject = new System.Windows.Forms.Label();
            this.label_DescriptiveStatistic1_Szenario = new System.Windows.Forms.Label();
            this.label_DescriptiveStatistic1_Groups = new System.Windows.Forms.Label();
            this.label_DescriptiveStatistic1_Study = new System.Windows.Forms.Label();
            this.listBox_DescriptiveStatistic1_Trials = new System.Windows.Forms.ListBox();
            this.listBox_DescriptiveStatistic1_Subjects = new System.Windows.Forms.ListBox();
            this.comboBox_DescriptiveStatistic1_Szenario = new System.Windows.Forms.ComboBox();
            this.comboBox_DescriptiveStatistic1_Study = new System.Windows.Forms.ComboBox();
            this.listBox_DescriptiveStatistic1_Groups = new System.Windows.Forms.ListBox();
            this.tabPage_DescriptiveStatistic2 = new System.Windows.Forms.TabPage();
            this.label_DescriptiveStatistic2_Handedness = new System.Windows.Forms.Label();
            this.label_DescriptiveStatistic2_ForceField = new System.Windows.Forms.Label();
            this.label_DescriptiveStatistic2_TrialType = new System.Windows.Forms.Label();
            this.listBox_DescriptiveStatistic2_Handedness = new System.Windows.Forms.ListBox();
            this.listBox_DescriptiveStatistic2_ForceField = new System.Windows.Forms.ListBox();
            this.listBox_DescriptiveStatistic2_TrialType = new System.Windows.Forms.ListBox();
            this.button_DescriptiveStatistic2_CalculateMeanValues = new System.Windows.Forms.Button();
            this.listBox_DescriptiveStatistic2_Turns = new System.Windows.Forms.ListBox();
            this.label_DescriptiveStatistic2_Turns = new System.Windows.Forms.Label();
            this.button_DescriptiveStatistic2_AddAll = new System.Windows.Forms.Button();
            this.comboBox_DescriptiveStatistic2_DataTypeSelect = new System.Windows.Forms.ComboBox();
            this.button_DescriptiveStatistic2_ClearAll = new System.Windows.Forms.Button();
            this.button_DescriptiveStatistic2_ClearSelected = new System.Windows.Forms.Button();
            this.listBox_DescriptiveStatistic2_SelectedTrials = new System.Windows.Forms.ListBox();
            this.button_DescriptiveStatistic2_AddSelected = new System.Windows.Forms.Button();
            this.label_DescriptiveStatistic2_Trials = new System.Windows.Forms.Label();
            this.label_DescriptiveStatistic2_Subject = new System.Windows.Forms.Label();
            this.label_DescriptiveStatistic2_Szenario = new System.Windows.Forms.Label();
            this.label_DescriptiveStatistic2_Groups = new System.Windows.Forms.Label();
            this.label_DescriptiveStatistic2_Study = new System.Windows.Forms.Label();
            this.listBox_DescriptiveStatistic2_Trials = new System.Windows.Forms.ListBox();
            this.listBox_DescriptiveStatistic2_Subjects = new System.Windows.Forms.ListBox();
            this.comboBox_DescriptiveStatistic2_Szenario = new System.Windows.Forms.ComboBox();
            this.comboBox_DescriptiveStatistic2_Study = new System.Windows.Forms.ComboBox();
            this.listBox_DescriptiveStatistic2_Groups = new System.Windows.Forms.ListBox();
            this.tabPage_Others = new System.Windows.Forms.TabPage();
            this.label_Others_PlotErrorclampForces_At = new System.Windows.Forms.Label();
            this.label_Others_PlotErrorclampForces_Ms = new System.Windows.Forms.Label();
            this.textBox_Others_PlotErrorclampForces_MsIndex = new System.Windows.Forms.TextBox();
            this.button_Others_PlotForcefieldCompensationFactor = new System.Windows.Forms.Button();
            this.checkBox_Others_GroupAverage = new System.Windows.Forms.CheckBox();
            this.button_Others_ExportGroupLi = new System.Windows.Forms.Button();
            this.button_Others_PlotGroupLi = new System.Windows.Forms.Button();
            this.button_Others_ExportVelocityBaseline = new System.Windows.Forms.Button();
            this.button_Others_PlotVelocityBaseline = new System.Windows.Forms.Button();
            this.button_Others_ExportTrajectoryBaseline = new System.Windows.Forms.Button();
            this.button_Others_ExportSzenarioMeanTimes = new System.Windows.Forms.Button();
            this.label_Others_Turn = new System.Windows.Forms.Label();
            this.comboBox_Others_Turn = new System.Windows.Forms.ComboBox();
            this.label_Others_Group = new System.Windows.Forms.Label();
            this.comboBox_Others_Group = new System.Windows.Forms.ComboBox();
            this.label_Others_Szenario = new System.Windows.Forms.Label();
            this.label_Others_Subject = new System.Windows.Forms.Label();
            this.label_Others_Study = new System.Windows.Forms.Label();
            this.comboBox_Others_Subject = new System.Windows.Forms.ComboBox();
            this.comboBox_Others_Study = new System.Windows.Forms.ComboBox();
            this.comboBox_Others_Szenario = new System.Windows.Forms.ComboBox();
            this.button_Others_PlotTrajectoryBaseline = new System.Windows.Forms.Button();
            this.button_Others_PlotSzenarioMeanTimes = new System.Windows.Forms.Button();
            this.tabPage_ImportCalculations = new System.Windows.Forms.TabPage();
            this.groupBox_Import_VelocityCropping = new System.Windows.Forms.GroupBox();
            this.label_Import_PercentPeakVelocity = new System.Windows.Forms.Label();
            this.textBox_Import_PercentPeakVelocity = new System.Windows.Forms.TextBox();
            this.button_Import_ClearMeasureFileList = new System.Windows.Forms.Button();
            this.groupBox_Import_CalculationsImport = new System.Windows.Forms.GroupBox();
            this.button_Import_AutoImport = new System.Windows.Forms.Button();
            this.button_Import_FixBrokenTrials = new System.Windows.Forms.Button();
            this.button_Import_ImportMeasureFiles = new System.Windows.Forms.Button();
            this.button_Import_CalculateStatistics = new System.Windows.Forms.Button();
            this.groupBox_Import_TimeNormalization = new System.Windows.Forms.GroupBox();
            this.label_Import_NewSampleCountText = new System.Windows.Forms.Label();
            this.textBox_Import_NewSampleCount = new System.Windows.Forms.TextBox();
            this.groupBox_Import_ButterworthFilter = new System.Windows.Forms.GroupBox();
            this.label_Import_CutoffFreqForceForce = new System.Windows.Forms.Label();
            this.label_Import_CutoffFreqPositionPosition = new System.Windows.Forms.Label();
            this.textBox_Import_CutoffFreqForce = new System.Windows.Forms.TextBox();
            this.label_Import_CutoffFreqForce = new System.Windows.Forms.Label();
            this.label_Import_SamplesPerSec = new System.Windows.Forms.Label();
            this.textBox_Import_CutoffFreqPosition = new System.Windows.Forms.TextBox();
            this.label_Import_CutoffFreqPosition = new System.Windows.Forms.Label();
            this.textBox_Import_SamplesPerSec = new System.Windows.Forms.TextBox();
            this.textBox_Import_FilterOrder = new System.Windows.Forms.TextBox();
            this.label_Import_FilterOrder = new System.Windows.Forms.Label();
            this.button_Import_SelectMeasureFileFolder = new System.Windows.Forms.Button();
            this.listBox_Import_SelectedMeasureFiles = new System.Windows.Forms.ListBox();
            this.button_Import_SelectMeasureFiles = new System.Windows.Forms.Button();
            this.tabPage_Debug = new System.Windows.Forms.TabPage();
            this.tabControl_Debug = new System.Windows.Forms.TabControl();
            this.tabPage_Debug_MatlabAndLogs = new System.Windows.Forms.TabPage();
            this.button_Debug_ShowMatlabFiles = new System.Windows.Forms.Button();
            this.button_Debug_ShowMatlabWindow = new System.Windows.Forms.Button();
            this.button_Debug_ShowMatlabWorkspace = new System.Windows.Forms.Button();
            this.button_Debug_showFaultyTrials = new System.Windows.Forms.Button();
            this.button_Debug_SaveLogToFile = new System.Windows.Forms.Button();
            this.tabPage_Debug_DatabaseManipulation = new System.Windows.Forms.TabPage();
            this.button_DataManipulation_DropStatistics = new System.Windows.Forms.Button();
            this.button_DataManipulation_CompactDatabase = new System.Windows.Forms.Button();
            this.button_DataManipulation_DropIndexes = new System.Windows.Forms.Button();
            this.button_DataManipulation_RebuildIndexes = new System.Windows.Forms.Button();
            this.button_DataManipulation_EnsureIndexes = new System.Windows.Forms.Button();
            this.button_Debug_DropDatabase = new System.Windows.Forms.Button();
            this.tabPage_Debug_BaselineRecalculation = new System.Windows.Forms.TabPage();
            this.button_BaselineRecalculation_RecalculateBaseline = new System.Windows.Forms.Button();
            this.label_BaselineRecalculation_Targets = new System.Windows.Forms.Label();
            this.listBox_BaselineRecalculation_Targets = new System.Windows.Forms.ListBox();
            this.button_BaselineRecalculation_AddAll = new System.Windows.Forms.Button();
            this.button_BaselineRecalculation_ClearAll = new System.Windows.Forms.Button();
            this.button_BaselineRecalculation_ClearSelected = new System.Windows.Forms.Button();
            this.listBox_BaselineRecalculation_SelectedTrials = new System.Windows.Forms.ListBox();
            this.button_BaselineRecalculation_AddSelected = new System.Windows.Forms.Button();
            this.label_BaselineRecalculation_Trials = new System.Windows.Forms.Label();
            this.listBox_BaselineRecalculation_Trials = new System.Windows.Forms.ListBox();
            this.label_BaselineRecalculation_Group = new System.Windows.Forms.Label();
            this.comboBox_BaselineRecalculation_Group = new System.Windows.Forms.ComboBox();
            this.label_BaselineRecalculation_Szenario = new System.Windows.Forms.Label();
            this.label_BaselineRecalculation_Subject = new System.Windows.Forms.Label();
            this.label_BaselineRecalculation_Study = new System.Windows.Forms.Label();
            this.comboBox_BaselineRecalculation_Subject = new System.Windows.Forms.ComboBox();
            this.comboBox_BaselineRecalculation_Study = new System.Windows.Forms.ComboBox();
            this.comboBox_BaselineRecalculation_Szenario = new System.Windows.Forms.ComboBox();
            this.tabPage_Impressum = new System.Windows.Forms.TabPage();
            this.label_Impressum_Text = new System.Windows.Forms.Label();
            this.pictureBox_Impressum_KITLogo = new System.Windows.Forms.PictureBox();
            this.checkBox_Start_ManualMode = new System.Windows.Forms.CheckBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.checkBox_PauseThread = new System.Windows.Forms.CheckBox();
            this.label_Log = new System.Windows.Forms.Label();
            this.button_ClearLog = new System.Windows.Forms.Button();
            this.listBox_LogBox = new System.Windows.Forms.ListBox();
            this.checkBox_Cancel = new System.Windows.Forms.CheckBox();
            this.tabControl.SuspendLayout();
            this.tabPage_Start.SuspendLayout();
            this.tabPage_VisualizationExport.SuspendLayout();
            this.tabControl_VisualizationExport.SuspendLayout();
            this.tabPage_TrajectoryVelocity.SuspendLayout();
            this.tabPage_DescriptiveStatistic1.SuspendLayout();
            this.tabPage_DescriptiveStatistic2.SuspendLayout();
            this.tabPage_Others.SuspendLayout();
            this.tabPage_ImportCalculations.SuspendLayout();
            this.groupBox_Import_VelocityCropping.SuspendLayout();
            this.groupBox_Import_CalculationsImport.SuspendLayout();
            this.groupBox_Import_TimeNormalization.SuspendLayout();
            this.groupBox_Import_ButterworthFilter.SuspendLayout();
            this.tabPage_Debug.SuspendLayout();
            this.tabControl_Debug.SuspendLayout();
            this.tabPage_Debug_MatlabAndLogs.SuspendLayout();
            this.tabPage_Debug_DatabaseManipulation.SuspendLayout();
            this.tabPage_Debug_BaselineRecalculation.SuspendLayout();
            this.tabPage_Impressum.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Impressum_KITLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage_Start);
            this.tabControl.Controls.Add(this.tabPage_VisualizationExport);
            this.tabControl.Controls.Add(this.tabPage_ImportCalculations);
            this.tabControl.Controls.Add(this.tabPage_Debug);
            this.tabControl.Controls.Add(this.tabPage_Impressum);
            this.tabControl.Location = new System.Drawing.Point(1, 3);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(741, 663);
            this.tabControl.TabIndex = 0;
            // 
            // tabPage_Start
            // 
            this.tabPage_Start.Controls.Add(this.comboBox_Start_DatabaseServer);
            this.tabPage_Start.Controls.Add(this.label_Start_ServerIP);
            this.tabPage_Start.Controls.Add(this.button_Start_SelectDatabase);
            this.tabPage_Start.Controls.Add(this.comboBox_Start_Database);
            this.tabPage_Start.Controls.Add(this.label_Start_Database);
            this.tabPage_Start.Controls.Add(this.button_Start_ConnectToSQlServer);
            this.tabPage_Start.Controls.Add(this.label_Start_ManipAnalysis);
            this.tabPage_Start.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Start.Name = "tabPage_Start";
            this.tabPage_Start.Size = new System.Drawing.Size(733, 637);
            this.tabPage_Start.TabIndex = 9;
            this.tabPage_Start.Text = "Start";
            this.tabPage_Start.UseVisualStyleBackColor = true;
            // 
            // comboBox_Start_DatabaseServer
            // 
            this.comboBox_Start_DatabaseServer.FormattingEnabled = true;
            this.comboBox_Start_DatabaseServer.Items.AddRange(new object[] {
            "IFS96",
            "localhost",
            "7.16.150.60"});
            this.comboBox_Start_DatabaseServer.Location = new System.Drawing.Point(14, 130);
            this.comboBox_Start_DatabaseServer.Name = "comboBox_Start_DatabaseServer";
            this.comboBox_Start_DatabaseServer.Size = new System.Drawing.Size(281, 21);
            this.comboBox_Start_DatabaseServer.TabIndex = 24;
            this.comboBox_Start_DatabaseServer.Text = "IFS96";
            // 
            // label_Start_ServerIP
            // 
            this.label_Start_ServerIP.AutoSize = true;
            this.label_Start_ServerIP.Location = new System.Drawing.Point(11, 113);
            this.label_Start_ServerIP.Name = "label_Start_ServerIP";
            this.label_Start_ServerIP.Size = new System.Drawing.Size(95, 13);
            this.label_Start_ServerIP.TabIndex = 23;
            this.label_Start_ServerIP.Text = "Server name or IP:";
            // 
            // button_Start_SelectDatabase
            // 
            this.button_Start_SelectDatabase.Enabled = false;
            this.button_Start_SelectDatabase.Location = new System.Drawing.Point(301, 178);
            this.button_Start_SelectDatabase.Name = "button_Start_SelectDatabase";
            this.button_Start_SelectDatabase.Size = new System.Drawing.Size(90, 23);
            this.button_Start_SelectDatabase.TabIndex = 21;
            this.button_Start_SelectDatabase.Text = "Select";
            this.button_Start_SelectDatabase.UseVisualStyleBackColor = true;
            this.button_Start_SelectDatabase.Click += new System.EventHandler(this.button_Start_SelectDatabase_Click);
            // 
            // comboBox_Start_Database
            // 
            this.comboBox_Start_Database.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Start_Database.Enabled = false;
            this.comboBox_Start_Database.FormattingEnabled = true;
            this.comboBox_Start_Database.Items.AddRange(new object[] {
            "172.22.190.96",
            "7.188.190.190",
            "localhost"});
            this.comboBox_Start_Database.Location = new System.Drawing.Point(14, 178);
            this.comboBox_Start_Database.Name = "comboBox_Start_Database";
            this.comboBox_Start_Database.Size = new System.Drawing.Size(281, 21);
            this.comboBox_Start_Database.TabIndex = 20;
            // 
            // label_Start_Database
            // 
            this.label_Start_Database.AutoSize = true;
            this.label_Start_Database.Location = new System.Drawing.Point(11, 162);
            this.label_Start_Database.Name = "label_Start_Database";
            this.label_Start_Database.Size = new System.Drawing.Size(56, 13);
            this.label_Start_Database.TabIndex = 19;
            this.label_Start_Database.Text = "Database:";
            // 
            // button_Start_ConnectToSQlServer
            // 
            this.button_Start_ConnectToSQlServer.Location = new System.Drawing.Point(301, 128);
            this.button_Start_ConnectToSQlServer.Name = "button_Start_ConnectToSQlServer";
            this.button_Start_ConnectToSQlServer.Size = new System.Drawing.Size(90, 23);
            this.button_Start_ConnectToSQlServer.TabIndex = 16;
            this.button_Start_ConnectToSQlServer.Text = "Connect";
            this.button_Start_ConnectToSQlServer.UseVisualStyleBackColor = true;
            this.button_Start_ConnectToSQlServer.Click += new System.EventHandler(this.button_Start_ConnectToSQlServer_Click);
            // 
            // label_Start_ManipAnalysis
            // 
            this.label_Start_ManipAnalysis.AutoSize = true;
            this.label_Start_ManipAnalysis.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Start_ManipAnalysis.Location = new System.Drawing.Point(7, 30);
            this.label_Start_ManipAnalysis.Name = "label_Start_ManipAnalysis";
            this.label_Start_ManipAnalysis.Size = new System.Drawing.Size(304, 39);
            this.label_Start_ManipAnalysis.TabIndex = 0;
            this.label_Start_ManipAnalysis.Text = "ManipAnalysis v2";
            // 
            // tabPage_VisualizationExport
            // 
            this.tabPage_VisualizationExport.Controls.Add(this.tabControl_VisualizationExport);
            this.tabPage_VisualizationExport.Location = new System.Drawing.Point(4, 22);
            this.tabPage_VisualizationExport.Name = "tabPage_VisualizationExport";
            this.tabPage_VisualizationExport.Size = new System.Drawing.Size(733, 637);
            this.tabPage_VisualizationExport.TabIndex = 10;
            this.tabPage_VisualizationExport.Text = "Visualization & Export";
            this.tabPage_VisualizationExport.UseVisualStyleBackColor = true;
            this.tabPage_VisualizationExport.Enter += new System.EventHandler(this.tabPage_VisualizationExport_Enter);
            // 
            // tabControl_VisualizationExport
            // 
            this.tabControl_VisualizationExport.Controls.Add(this.tabPage_TrajectoryVelocity);
            this.tabControl_VisualizationExport.Controls.Add(this.tabPage_DescriptiveStatistic1);
            this.tabControl_VisualizationExport.Controls.Add(this.tabPage_DescriptiveStatistic2);
            this.tabControl_VisualizationExport.Controls.Add(this.tabPage_Others);
            this.tabControl_VisualizationExport.Location = new System.Drawing.Point(3, 3);
            this.tabControl_VisualizationExport.Name = "tabControl_VisualizationExport";
            this.tabControl_VisualizationExport.SelectedIndex = 0;
            this.tabControl_VisualizationExport.Size = new System.Drawing.Size(727, 628);
            this.tabControl_VisualizationExport.TabIndex = 0;
            // 
            // tabPage_TrajectoryVelocity
            // 
            this.tabPage_TrajectoryVelocity.Controls.Add(this.label_TrajectoryVelocity_Handedness);
            this.tabPage_TrajectoryVelocity.Controls.Add(this.label_TrajectoryVelocity_ForceField);
            this.tabPage_TrajectoryVelocity.Controls.Add(this.label_TrajectoryVelocity_TrialType);
            this.tabPage_TrajectoryVelocity.Controls.Add(this.listBox_TrajectoryVelocity_Handedness);
            this.tabPage_TrajectoryVelocity.Controls.Add(this.listBox_TrajectoryVelocity_ForceField);
            this.tabPage_TrajectoryVelocity.Controls.Add(this.listBox_TrajectoryVelocity_TrialType);
            this.tabPage_TrajectoryVelocity.Controls.Add(this.checkBox_TrajectoryVelocity_ShowPDForceVectors);
            this.tabPage_TrajectoryVelocity.Controls.Add(this.checkBox_TrajectoryVelocity_ShowForceVectors);
            this.tabPage_TrajectoryVelocity.Controls.Add(this.button_TrajectoryVelocity_Export);
            this.tabPage_TrajectoryVelocity.Controls.Add(this.comboBox_TrajectoryVelocity_IndividualMean);
            this.tabPage_TrajectoryVelocity.Controls.Add(this.comboBox_TrajectoryVelocity_TrajectoryVelocity);
            this.tabPage_TrajectoryVelocity.Controls.Add(this.label_TrajectoryVelocity_Targets);
            this.tabPage_TrajectoryVelocity.Controls.Add(this.listBox_TrajectoryVelocity_Targets);
            this.tabPage_TrajectoryVelocity.Controls.Add(this.listBox_TrajectoryVelocity_Turns);
            this.tabPage_TrajectoryVelocity.Controls.Add(this.label_TrajectoryVelocity_Turns);
            this.tabPage_TrajectoryVelocity.Controls.Add(this.button_TrajectoryVelocity_AddAll);
            this.tabPage_TrajectoryVelocity.Controls.Add(this.button_TrajectoryVelocity_Plot);
            this.tabPage_TrajectoryVelocity.Controls.Add(this.button_TrajectoryVelocity_ClearAll);
            this.tabPage_TrajectoryVelocity.Controls.Add(this.button_TrajectoryVelocity_ClearSelected);
            this.tabPage_TrajectoryVelocity.Controls.Add(this.listBox_TrajectoryVelocity_SelectedTrials);
            this.tabPage_TrajectoryVelocity.Controls.Add(this.button_TrajectoryVelocity_AddSelected);
            this.tabPage_TrajectoryVelocity.Controls.Add(this.label_TrajectoryVelocity_Trials);
            this.tabPage_TrajectoryVelocity.Controls.Add(this.label_TrajectoryVelocity_Subjects);
            this.tabPage_TrajectoryVelocity.Controls.Add(this.label_TrajectoryVelocity_Szenario);
            this.tabPage_TrajectoryVelocity.Controls.Add(this.label_TrajectoryVelocity_Groups);
            this.tabPage_TrajectoryVelocity.Controls.Add(this.label_TrajectoryVelocity_Study);
            this.tabPage_TrajectoryVelocity.Controls.Add(this.listBox_TrajectoryVelocity_Trials);
            this.tabPage_TrajectoryVelocity.Controls.Add(this.listBox_TrajectoryVelocity_Subjects);
            this.tabPage_TrajectoryVelocity.Controls.Add(this.comboBox_TrajectoryVelocity_Szenario);
            this.tabPage_TrajectoryVelocity.Controls.Add(this.comboBox_TrajectoryVelocity_Study);
            this.tabPage_TrajectoryVelocity.Controls.Add(this.listBox_TrajectoryVelocity_Groups);
            this.tabPage_TrajectoryVelocity.Location = new System.Drawing.Point(4, 22);
            this.tabPage_TrajectoryVelocity.Name = "tabPage_TrajectoryVelocity";
            this.tabPage_TrajectoryVelocity.Size = new System.Drawing.Size(719, 602);
            this.tabPage_TrajectoryVelocity.TabIndex = 4;
            this.tabPage_TrajectoryVelocity.Text = "Trajectory / Velocity";
            this.tabPage_TrajectoryVelocity.UseVisualStyleBackColor = true;
            this.tabPage_TrajectoryVelocity.Enter += new System.EventHandler(this.tabPage_TrajectoryVelocity_Enter);
            // 
            // label_TrajectoryVelocity_Handedness
            // 
            this.label_TrajectoryVelocity_Handedness.AutoSize = true;
            this.label_TrajectoryVelocity_Handedness.Location = new System.Drawing.Point(552, 452);
            this.label_TrajectoryVelocity_Handedness.Name = "label_TrajectoryVelocity_Handedness";
            this.label_TrajectoryVelocity_Handedness.Size = new System.Drawing.Size(70, 13);
            this.label_TrajectoryVelocity_Handedness.TabIndex = 64;
            this.label_TrajectoryVelocity_Handedness.Text = "Handedness:";
            // 
            // label_TrajectoryVelocity_ForceField
            // 
            this.label_TrajectoryVelocity_ForceField.AutoSize = true;
            this.label_TrajectoryVelocity_ForceField.Location = new System.Drawing.Point(388, 452);
            this.label_TrajectoryVelocity_ForceField.Name = "label_TrajectoryVelocity_ForceField";
            this.label_TrajectoryVelocity_ForceField.Size = new System.Drawing.Size(59, 13);
            this.label_TrajectoryVelocity_ForceField.TabIndex = 63;
            this.label_TrajectoryVelocity_ForceField.Text = "Force field:";
            // 
            // label_TrajectoryVelocity_TrialType
            // 
            this.label_TrajectoryVelocity_TrialType.AutoSize = true;
            this.label_TrajectoryVelocity_TrialType.Location = new System.Drawing.Point(227, 452);
            this.label_TrajectoryVelocity_TrialType.Name = "label_TrajectoryVelocity_TrialType";
            this.label_TrajectoryVelocity_TrialType.Size = new System.Drawing.Size(53, 13);
            this.label_TrajectoryVelocity_TrialType.TabIndex = 62;
            this.label_TrajectoryVelocity_TrialType.Text = "Trial type:";
            // 
            // listBox_TrajectoryVelocity_Handedness
            // 
            this.listBox_TrajectoryVelocity_Handedness.FormattingEnabled = true;
            this.listBox_TrajectoryVelocity_Handedness.Location = new System.Drawing.Point(555, 470);
            this.listBox_TrajectoryVelocity_Handedness.Name = "listBox_TrajectoryVelocity_Handedness";
            this.listBox_TrajectoryVelocity_Handedness.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox_TrajectoryVelocity_Handedness.Size = new System.Drawing.Size(158, 69);
            this.listBox_TrajectoryVelocity_Handedness.TabIndex = 61;
            // 
            // listBox_TrajectoryVelocity_ForceField
            // 
            this.listBox_TrajectoryVelocity_ForceField.FormattingEnabled = true;
            this.listBox_TrajectoryVelocity_ForceField.Location = new System.Drawing.Point(391, 470);
            this.listBox_TrajectoryVelocity_ForceField.Name = "listBox_TrajectoryVelocity_ForceField";
            this.listBox_TrajectoryVelocity_ForceField.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox_TrajectoryVelocity_ForceField.Size = new System.Drawing.Size(158, 69);
            this.listBox_TrajectoryVelocity_ForceField.TabIndex = 60;
            // 
            // listBox_TrajectoryVelocity_TrialType
            // 
            this.listBox_TrajectoryVelocity_TrialType.FormattingEnabled = true;
            this.listBox_TrajectoryVelocity_TrialType.Location = new System.Drawing.Point(227, 470);
            this.listBox_TrajectoryVelocity_TrialType.Name = "listBox_TrajectoryVelocity_TrialType";
            this.listBox_TrajectoryVelocity_TrialType.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox_TrajectoryVelocity_TrialType.Size = new System.Drawing.Size(158, 69);
            this.listBox_TrajectoryVelocity_TrialType.TabIndex = 59;
            // 
            // checkBox_TrajectoryVelocity_ShowPDForceVectors
            // 
            this.checkBox_TrajectoryVelocity_ShowPDForceVectors.AutoSize = true;
            this.checkBox_TrajectoryVelocity_ShowPDForceVectors.Location = new System.Drawing.Point(71, 569);
            this.checkBox_TrajectoryVelocity_ShowPDForceVectors.Name = "checkBox_TrajectoryVelocity_ShowPDForceVectors";
            this.checkBox_TrajectoryVelocity_ShowPDForceVectors.Size = new System.Drawing.Size(73, 17);
            this.checkBox_TrajectoryVelocity_ShowPDForceVectors.TabIndex = 57;
            this.checkBox_TrajectoryVelocity_ShowPDForceVectors.Text = "PD forces";
            this.checkBox_TrajectoryVelocity_ShowPDForceVectors.UseVisualStyleBackColor = true;
            // 
            // checkBox_TrajectoryVelocity_ShowForceVectors
            // 
            this.checkBox_TrajectoryVelocity_ShowForceVectors.AutoSize = true;
            this.checkBox_TrajectoryVelocity_ShowForceVectors.Location = new System.Drawing.Point(71, 546);
            this.checkBox_TrajectoryVelocity_ShowForceVectors.Name = "checkBox_TrajectoryVelocity_ShowForceVectors";
            this.checkBox_TrajectoryVelocity_ShowForceVectors.Size = new System.Drawing.Size(91, 17);
            this.checkBox_TrajectoryVelocity_ShowForceVectors.TabIndex = 56;
            this.checkBox_TrajectoryVelocity_ShowForceVectors.Text = "Force vectors";
            this.checkBox_TrajectoryVelocity_ShowForceVectors.UseVisualStyleBackColor = true;
            // 
            // button_TrajectoryVelocity_Export
            // 
            this.button_TrajectoryVelocity_Export.Location = new System.Drawing.Point(631, 571);
            this.button_TrajectoryVelocity_Export.Name = "button_TrajectoryVelocity_Export";
            this.button_TrajectoryVelocity_Export.Size = new System.Drawing.Size(85, 23);
            this.button_TrajectoryVelocity_Export.TabIndex = 51;
            this.button_TrajectoryVelocity_Export.Text = "Export";
            this.button_TrajectoryVelocity_Export.UseVisualStyleBackColor = true;
            this.button_TrajectoryVelocity_Export.Click += new System.EventHandler(this.button_TrajectoryVelocity_Export_Click);
            // 
            // comboBox_TrajectoryVelocity_IndividualMean
            // 
            this.comboBox_TrajectoryVelocity_IndividualMean.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_TrajectoryVelocity_IndividualMean.FormattingEnabled = true;
            this.comboBox_TrajectoryVelocity_IndividualMean.Items.AddRange(new object[] {
            "Individual",
            "Mean"});
            this.comboBox_TrajectoryVelocity_IndividualMean.Location = new System.Drawing.Point(409, 542);
            this.comboBox_TrajectoryVelocity_IndividualMean.Name = "comboBox_TrajectoryVelocity_IndividualMean";
            this.comboBox_TrajectoryVelocity_IndividualMean.Size = new System.Drawing.Size(216, 21);
            this.comboBox_TrajectoryVelocity_IndividualMean.TabIndex = 50;
            // 
            // comboBox_TrajectoryVelocity_TrajectoryVelocity
            // 
            this.comboBox_TrajectoryVelocity_TrajectoryVelocity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_TrajectoryVelocity_TrajectoryVelocity.FormattingEnabled = true;
            this.comboBox_TrajectoryVelocity_TrajectoryVelocity.Items.AddRange(new object[] {
            "Trajectory - Normalized",
            "Velocity - Normalized",
            "Force - Normalized",
            "Trajectory - Filtered",
            "Velocity - Filtered",
            "Force - Filtered",
            "Trajectory - Raw",
            "Force - Raw"});
            this.comboBox_TrajectoryVelocity_TrajectoryVelocity.Location = new System.Drawing.Point(409, 571);
            this.comboBox_TrajectoryVelocity_TrajectoryVelocity.Name = "comboBox_TrajectoryVelocity_TrajectoryVelocity";
            this.comboBox_TrajectoryVelocity_TrajectoryVelocity.Size = new System.Drawing.Size(216, 21);
            this.comboBox_TrajectoryVelocity_TrajectoryVelocity.TabIndex = 49;
            // 
            // label_TrajectoryVelocity_Targets
            // 
            this.label_TrajectoryVelocity_Targets.AutoSize = true;
            this.label_TrajectoryVelocity_Targets.Location = new System.Drawing.Point(13, 358);
            this.label_TrajectoryVelocity_Targets.Name = "label_TrajectoryVelocity_Targets";
            this.label_TrajectoryVelocity_Targets.Size = new System.Drawing.Size(52, 13);
            this.label_TrajectoryVelocity_Targets.TabIndex = 46;
            this.label_TrajectoryVelocity_Targets.Text = "Target(s):";
            // 
            // listBox_TrajectoryVelocity_Targets
            // 
            this.listBox_TrajectoryVelocity_Targets.FormattingEnabled = true;
            this.listBox_TrajectoryVelocity_Targets.Location = new System.Drawing.Point(71, 317);
            this.listBox_TrajectoryVelocity_Targets.Name = "listBox_TrajectoryVelocity_Targets";
            this.listBox_TrajectoryVelocity_Targets.ScrollAlwaysVisible = true;
            this.listBox_TrajectoryVelocity_Targets.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox_TrajectoryVelocity_Targets.Size = new System.Drawing.Size(150, 108);
            this.listBox_TrajectoryVelocity_Targets.Sorted = true;
            this.listBox_TrajectoryVelocity_Targets.TabIndex = 45;
            // 
            // listBox_TrajectoryVelocity_Turns
            // 
            this.listBox_TrajectoryVelocity_Turns.FormattingEnabled = true;
            this.listBox_TrajectoryVelocity_Turns.Location = new System.Drawing.Point(71, 268);
            this.listBox_TrajectoryVelocity_Turns.Name = "listBox_TrajectoryVelocity_Turns";
            this.listBox_TrajectoryVelocity_Turns.ScrollAlwaysVisible = true;
            this.listBox_TrajectoryVelocity_Turns.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox_TrajectoryVelocity_Turns.Size = new System.Drawing.Size(150, 43);
            this.listBox_TrajectoryVelocity_Turns.Sorted = true;
            this.listBox_TrajectoryVelocity_Turns.TabIndex = 43;
            this.listBox_TrajectoryVelocity_Turns.SelectedIndexChanged += new System.EventHandler(this.listBox_TrajectoryVelocity_Turns_SelectedIndexChanged);
            // 
            // label_TrajectoryVelocity_Turns
            // 
            this.label_TrajectoryVelocity_Turns.AutoSize = true;
            this.label_TrajectoryVelocity_Turns.Location = new System.Drawing.Point(22, 278);
            this.label_TrajectoryVelocity_Turns.Name = "label_TrajectoryVelocity_Turns";
            this.label_TrajectoryVelocity_Turns.Size = new System.Drawing.Size(43, 13);
            this.label_TrajectoryVelocity_Turns.TabIndex = 41;
            this.label_TrajectoryVelocity_Turns.Text = "Turn(s):";
            // 
            // button_TrajectoryVelocity_AddAll
            // 
            this.button_TrajectoryVelocity_AddAll.Location = new System.Drawing.Point(227, 571);
            this.button_TrajectoryVelocity_AddAll.Name = "button_TrajectoryVelocity_AddAll";
            this.button_TrajectoryVelocity_AddAll.Size = new System.Drawing.Size(85, 23);
            this.button_TrajectoryVelocity_AddAll.TabIndex = 39;
            this.button_TrajectoryVelocity_AddAll.Text = "Add all";
            this.button_TrajectoryVelocity_AddAll.UseVisualStyleBackColor = true;
            this.button_TrajectoryVelocity_AddAll.Click += new System.EventHandler(this.button_TrajectoryVelocity_AddAll_Click);
            // 
            // button_TrajectoryVelocity_Plot
            // 
            this.button_TrajectoryVelocity_Plot.Location = new System.Drawing.Point(631, 542);
            this.button_TrajectoryVelocity_Plot.Name = "button_TrajectoryVelocity_Plot";
            this.button_TrajectoryVelocity_Plot.Size = new System.Drawing.Size(85, 23);
            this.button_TrajectoryVelocity_Plot.TabIndex = 37;
            this.button_TrajectoryVelocity_Plot.Text = "Plot";
            this.button_TrajectoryVelocity_Plot.UseVisualStyleBackColor = true;
            this.button_TrajectoryVelocity_Plot.Click += new System.EventHandler(this.button_TrajectoryVelocity_Plot_Click);
            // 
            // button_TrajectoryVelocity_ClearAll
            // 
            this.button_TrajectoryVelocity_ClearAll.Location = new System.Drawing.Point(318, 571);
            this.button_TrajectoryVelocity_ClearAll.Name = "button_TrajectoryVelocity_ClearAll";
            this.button_TrajectoryVelocity_ClearAll.Size = new System.Drawing.Size(85, 23);
            this.button_TrajectoryVelocity_ClearAll.TabIndex = 36;
            this.button_TrajectoryVelocity_ClearAll.Text = "Clear all";
            this.button_TrajectoryVelocity_ClearAll.UseVisualStyleBackColor = true;
            this.button_TrajectoryVelocity_ClearAll.Click += new System.EventHandler(this.button_TrajectoryVelocity_ClearAll_Click);
            // 
            // button_TrajectoryVelocity_ClearSelected
            // 
            this.button_TrajectoryVelocity_ClearSelected.Location = new System.Drawing.Point(318, 542);
            this.button_TrajectoryVelocity_ClearSelected.Name = "button_TrajectoryVelocity_ClearSelected";
            this.button_TrajectoryVelocity_ClearSelected.Size = new System.Drawing.Size(85, 23);
            this.button_TrajectoryVelocity_ClearSelected.TabIndex = 35;
            this.button_TrajectoryVelocity_ClearSelected.Text = "Clear selected";
            this.button_TrajectoryVelocity_ClearSelected.UseVisualStyleBackColor = true;
            this.button_TrajectoryVelocity_ClearSelected.Click += new System.EventHandler(this.button_TrajectoryVelocity_ClearSelected_Click);
            // 
            // listBox_TrajectoryVelocity_SelectedTrials
            // 
            this.listBox_TrajectoryVelocity_SelectedTrials.FormattingEnabled = true;
            this.listBox_TrajectoryVelocity_SelectedTrials.Location = new System.Drawing.Point(227, 12);
            this.listBox_TrajectoryVelocity_SelectedTrials.Name = "listBox_TrajectoryVelocity_SelectedTrials";
            this.listBox_TrajectoryVelocity_SelectedTrials.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox_TrajectoryVelocity_SelectedTrials.Size = new System.Drawing.Size(489, 433);
            this.listBox_TrajectoryVelocity_SelectedTrials.Sorted = true;
            this.listBox_TrajectoryVelocity_SelectedTrials.TabIndex = 34;
            // 
            // button_TrajectoryVelocity_AddSelected
            // 
            this.button_TrajectoryVelocity_AddSelected.Location = new System.Drawing.Point(227, 542);
            this.button_TrajectoryVelocity_AddSelected.Name = "button_TrajectoryVelocity_AddSelected";
            this.button_TrajectoryVelocity_AddSelected.Size = new System.Drawing.Size(85, 23);
            this.button_TrajectoryVelocity_AddSelected.TabIndex = 33;
            this.button_TrajectoryVelocity_AddSelected.Text = "Add selected";
            this.button_TrajectoryVelocity_AddSelected.UseVisualStyleBackColor = true;
            this.button_TrajectoryVelocity_AddSelected.Click += new System.EventHandler(this.button_TrajectoryVelocity_AddSelected_Click);
            // 
            // label_TrajectoryVelocity_Trials
            // 
            this.label_TrajectoryVelocity_Trials.AutoSize = true;
            this.label_TrajectoryVelocity_Trials.Location = new System.Drawing.Point(24, 464);
            this.label_TrajectoryVelocity_Trials.Name = "label_TrajectoryVelocity_Trials";
            this.label_TrajectoryVelocity_Trials.Size = new System.Drawing.Size(41, 13);
            this.label_TrajectoryVelocity_Trials.TabIndex = 32;
            this.label_TrajectoryVelocity_Trials.Text = "Trial(s):";
            // 
            // label_TrajectoryVelocity_Subjects
            // 
            this.label_TrajectoryVelocity_Subjects.AutoSize = true;
            this.label_TrajectoryVelocity_Subjects.Location = new System.Drawing.Point(8, 207);
            this.label_TrajectoryVelocity_Subjects.Name = "label_TrajectoryVelocity_Subjects";
            this.label_TrajectoryVelocity_Subjects.Size = new System.Drawing.Size(57, 13);
            this.label_TrajectoryVelocity_Subjects.TabIndex = 31;
            this.label_TrajectoryVelocity_Subjects.Text = "Subject(s):";
            // 
            // label_TrajectoryVelocity_Szenario
            // 
            this.label_TrajectoryVelocity_Szenario.AutoSize = true;
            this.label_TrajectoryVelocity_Szenario.Location = new System.Drawing.Point(14, 143);
            this.label_TrajectoryVelocity_Szenario.Name = "label_TrajectoryVelocity_Szenario";
            this.label_TrajectoryVelocity_Szenario.Size = new System.Drawing.Size(51, 13);
            this.label_TrajectoryVelocity_Szenario.TabIndex = 30;
            this.label_TrajectoryVelocity_Szenario.Text = "Szenario:";
            // 
            // label_TrajectoryVelocity_Groups
            // 
            this.label_TrajectoryVelocity_Groups.AutoSize = true;
            this.label_TrajectoryVelocity_Groups.Location = new System.Drawing.Point(15, 75);
            this.label_TrajectoryVelocity_Groups.Name = "label_TrajectoryVelocity_Groups";
            this.label_TrajectoryVelocity_Groups.Size = new System.Drawing.Size(50, 13);
            this.label_TrajectoryVelocity_Groups.TabIndex = 29;
            this.label_TrajectoryVelocity_Groups.Text = "Group(s):";
            // 
            // label_TrajectoryVelocity_Study
            // 
            this.label_TrajectoryVelocity_Study.AutoSize = true;
            this.label_TrajectoryVelocity_Study.Location = new System.Drawing.Point(28, 15);
            this.label_TrajectoryVelocity_Study.Name = "label_TrajectoryVelocity_Study";
            this.label_TrajectoryVelocity_Study.Size = new System.Drawing.Size(37, 13);
            this.label_TrajectoryVelocity_Study.TabIndex = 28;
            this.label_TrajectoryVelocity_Study.Text = "Study:";
            // 
            // listBox_TrajectoryVelocity_Trials
            // 
            this.listBox_TrajectoryVelocity_Trials.FormattingEnabled = true;
            this.listBox_TrajectoryVelocity_Trials.Location = new System.Drawing.Point(71, 431);
            this.listBox_TrajectoryVelocity_Trials.Name = "listBox_TrajectoryVelocity_Trials";
            this.listBox_TrajectoryVelocity_Trials.ScrollAlwaysVisible = true;
            this.listBox_TrajectoryVelocity_Trials.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox_TrajectoryVelocity_Trials.Size = new System.Drawing.Size(150, 108);
            this.listBox_TrajectoryVelocity_Trials.Sorted = true;
            this.listBox_TrajectoryVelocity_Trials.TabIndex = 27;
            // 
            // listBox_TrajectoryVelocity_Subjects
            // 
            this.listBox_TrajectoryVelocity_Subjects.FormattingEnabled = true;
            this.listBox_TrajectoryVelocity_Subjects.Location = new System.Drawing.Point(71, 167);
            this.listBox_TrajectoryVelocity_Subjects.Name = "listBox_TrajectoryVelocity_Subjects";
            this.listBox_TrajectoryVelocity_Subjects.ScrollAlwaysVisible = true;
            this.listBox_TrajectoryVelocity_Subjects.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox_TrajectoryVelocity_Subjects.Size = new System.Drawing.Size(150, 95);
            this.listBox_TrajectoryVelocity_Subjects.Sorted = true;
            this.listBox_TrajectoryVelocity_Subjects.TabIndex = 26;
            this.listBox_TrajectoryVelocity_Subjects.SelectedIndexChanged += new System.EventHandler(this.listBox_TrajectoryVelocity_Subjects_SelectedIndexChanged);
            // 
            // comboBox_TrajectoryVelocity_Szenario
            // 
            this.comboBox_TrajectoryVelocity_Szenario.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_TrajectoryVelocity_Szenario.FormattingEnabled = true;
            this.comboBox_TrajectoryVelocity_Szenario.Location = new System.Drawing.Point(71, 140);
            this.comboBox_TrajectoryVelocity_Szenario.Name = "comboBox_TrajectoryVelocity_Szenario";
            this.comboBox_TrajectoryVelocity_Szenario.Size = new System.Drawing.Size(150, 21);
            this.comboBox_TrajectoryVelocity_Szenario.Sorted = true;
            this.comboBox_TrajectoryVelocity_Szenario.TabIndex = 25;
            this.comboBox_TrajectoryVelocity_Szenario.SelectedIndexChanged += new System.EventHandler(this.comboBox_TrajectoryVelocity_Szenario_SelectedIndexChanged);
            // 
            // comboBox_TrajectoryVelocity_Study
            // 
            this.comboBox_TrajectoryVelocity_Study.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_TrajectoryVelocity_Study.FormattingEnabled = true;
            this.comboBox_TrajectoryVelocity_Study.Location = new System.Drawing.Point(71, 12);
            this.comboBox_TrajectoryVelocity_Study.Name = "comboBox_TrajectoryVelocity_Study";
            this.comboBox_TrajectoryVelocity_Study.Size = new System.Drawing.Size(150, 21);
            this.comboBox_TrajectoryVelocity_Study.Sorted = true;
            this.comboBox_TrajectoryVelocity_Study.TabIndex = 24;
            this.comboBox_TrajectoryVelocity_Study.SelectedIndexChanged += new System.EventHandler(this.comboBox_TrajectoryVelocity_Study_SelectedIndexChanged);
            // 
            // listBox_TrajectoryVelocity_Groups
            // 
            this.listBox_TrajectoryVelocity_Groups.FormattingEnabled = true;
            this.listBox_TrajectoryVelocity_Groups.Location = new System.Drawing.Point(71, 39);
            this.listBox_TrajectoryVelocity_Groups.Name = "listBox_TrajectoryVelocity_Groups";
            this.listBox_TrajectoryVelocity_Groups.ScrollAlwaysVisible = true;
            this.listBox_TrajectoryVelocity_Groups.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox_TrajectoryVelocity_Groups.Size = new System.Drawing.Size(150, 95);
            this.listBox_TrajectoryVelocity_Groups.Sorted = true;
            this.listBox_TrajectoryVelocity_Groups.TabIndex = 23;
            this.listBox_TrajectoryVelocity_Groups.SelectedIndexChanged += new System.EventHandler(this.listBox_TrajectoryVelocity_Groups_SelectedIndexChanged);
            // 
            // tabPage_DescriptiveStatistic1
            // 
            this.tabPage_DescriptiveStatistic1.Controls.Add(this.label_DescriptiveStatistic1_Handedness);
            this.tabPage_DescriptiveStatistic1.Controls.Add(this.label_DescriptiveStatistic1_ForceField);
            this.tabPage_DescriptiveStatistic1.Controls.Add(this.label_DescriptiveStatistic1_TrialType);
            this.tabPage_DescriptiveStatistic1.Controls.Add(this.listBox_DescriptiveStatistic1_Handedness);
            this.tabPage_DescriptiveStatistic1.Controls.Add(this.listBox_DescriptiveStatistic1_ForceField);
            this.tabPage_DescriptiveStatistic1.Controls.Add(this.listBox_DescriptiveStatistic1_TrialType);
            this.tabPage_DescriptiveStatistic1.Controls.Add(this.checkBox_DescriptiveStatistic1_PlotFit);
            this.tabPage_DescriptiveStatistic1.Controls.Add(this.textBox_DescriptiveStatistic1_FitEquation);
            this.tabPage_DescriptiveStatistic1.Controls.Add(this.button_DescriptiveStatistic1_ExportData);
            this.tabPage_DescriptiveStatistic1.Controls.Add(this.listBox_DescriptiveStatistic1_Turns);
            this.tabPage_DescriptiveStatistic1.Controls.Add(this.label_DescriptiveStatistic1_Turns);
            this.tabPage_DescriptiveStatistic1.Controls.Add(this.checkBox_DescriptiveStatistic1_PlotErrorbars);
            this.tabPage_DescriptiveStatistic1.Controls.Add(this.button_DescriptiveStatistic1_AddAll);
            this.tabPage_DescriptiveStatistic1.Controls.Add(this.comboBox_DescriptiveStatistic1_DataTypeSelect);
            this.tabPage_DescriptiveStatistic1.Controls.Add(this.button_DescriptiveStatistic1_PlotMeanStd);
            this.tabPage_DescriptiveStatistic1.Controls.Add(this.button_DescriptiveStatistic1_ClearAll);
            this.tabPage_DescriptiveStatistic1.Controls.Add(this.button_DescriptiveStatistic1_ClearSelected);
            this.tabPage_DescriptiveStatistic1.Controls.Add(this.listBox_DescriptiveStatistic1_SelectedTrials);
            this.tabPage_DescriptiveStatistic1.Controls.Add(this.button_DescriptiveStatistic1_AddSelected);
            this.tabPage_DescriptiveStatistic1.Controls.Add(this.label_DescriptiveStatistic1_Trials);
            this.tabPage_DescriptiveStatistic1.Controls.Add(this.label_DescriptiveStatistic1_Subject);
            this.tabPage_DescriptiveStatistic1.Controls.Add(this.label_DescriptiveStatistic1_Szenario);
            this.tabPage_DescriptiveStatistic1.Controls.Add(this.label_DescriptiveStatistic1_Groups);
            this.tabPage_DescriptiveStatistic1.Controls.Add(this.label_DescriptiveStatistic1_Study);
            this.tabPage_DescriptiveStatistic1.Controls.Add(this.listBox_DescriptiveStatistic1_Trials);
            this.tabPage_DescriptiveStatistic1.Controls.Add(this.listBox_DescriptiveStatistic1_Subjects);
            this.tabPage_DescriptiveStatistic1.Controls.Add(this.comboBox_DescriptiveStatistic1_Szenario);
            this.tabPage_DescriptiveStatistic1.Controls.Add(this.comboBox_DescriptiveStatistic1_Study);
            this.tabPage_DescriptiveStatistic1.Controls.Add(this.listBox_DescriptiveStatistic1_Groups);
            this.tabPage_DescriptiveStatistic1.Location = new System.Drawing.Point(4, 22);
            this.tabPage_DescriptiveStatistic1.Name = "tabPage_DescriptiveStatistic1";
            this.tabPage_DescriptiveStatistic1.Size = new System.Drawing.Size(719, 602);
            this.tabPage_DescriptiveStatistic1.TabIndex = 2;
            this.tabPage_DescriptiveStatistic1.Text = "Descriptive Statistic 1";
            this.tabPage_DescriptiveStatistic1.UseVisualStyleBackColor = true;
            this.tabPage_DescriptiveStatistic1.Enter += new System.EventHandler(this.tabPage_DescriptiveStatistic1_Enter);
            // 
            // label_DescriptiveStatistic1_Handedness
            // 
            this.label_DescriptiveStatistic1_Handedness.AutoSize = true;
            this.label_DescriptiveStatistic1_Handedness.Location = new System.Drawing.Point(552, 449);
            this.label_DescriptiveStatistic1_Handedness.Name = "label_DescriptiveStatistic1_Handedness";
            this.label_DescriptiveStatistic1_Handedness.Size = new System.Drawing.Size(70, 13);
            this.label_DescriptiveStatistic1_Handedness.TabIndex = 70;
            this.label_DescriptiveStatistic1_Handedness.Text = "Handedness:";
            // 
            // label_DescriptiveStatistic1_ForceField
            // 
            this.label_DescriptiveStatistic1_ForceField.AutoSize = true;
            this.label_DescriptiveStatistic1_ForceField.Location = new System.Drawing.Point(388, 449);
            this.label_DescriptiveStatistic1_ForceField.Name = "label_DescriptiveStatistic1_ForceField";
            this.label_DescriptiveStatistic1_ForceField.Size = new System.Drawing.Size(59, 13);
            this.label_DescriptiveStatistic1_ForceField.TabIndex = 69;
            this.label_DescriptiveStatistic1_ForceField.Text = "Force field:";
            // 
            // label_DescriptiveStatistic1_TrialType
            // 
            this.label_DescriptiveStatistic1_TrialType.AutoSize = true;
            this.label_DescriptiveStatistic1_TrialType.Location = new System.Drawing.Point(227, 449);
            this.label_DescriptiveStatistic1_TrialType.Name = "label_DescriptiveStatistic1_TrialType";
            this.label_DescriptiveStatistic1_TrialType.Size = new System.Drawing.Size(53, 13);
            this.label_DescriptiveStatistic1_TrialType.TabIndex = 68;
            this.label_DescriptiveStatistic1_TrialType.Text = "Trial type:";
            // 
            // listBox_DescriptiveStatistic1_Handedness
            // 
            this.listBox_DescriptiveStatistic1_Handedness.FormattingEnabled = true;
            this.listBox_DescriptiveStatistic1_Handedness.Location = new System.Drawing.Point(555, 467);
            this.listBox_DescriptiveStatistic1_Handedness.Name = "listBox_DescriptiveStatistic1_Handedness";
            this.listBox_DescriptiveStatistic1_Handedness.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox_DescriptiveStatistic1_Handedness.Size = new System.Drawing.Size(158, 69);
            this.listBox_DescriptiveStatistic1_Handedness.TabIndex = 67;
            // 
            // listBox_DescriptiveStatistic1_ForceField
            // 
            this.listBox_DescriptiveStatistic1_ForceField.FormattingEnabled = true;
            this.listBox_DescriptiveStatistic1_ForceField.Location = new System.Drawing.Point(391, 467);
            this.listBox_DescriptiveStatistic1_ForceField.Name = "listBox_DescriptiveStatistic1_ForceField";
            this.listBox_DescriptiveStatistic1_ForceField.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox_DescriptiveStatistic1_ForceField.Size = new System.Drawing.Size(158, 69);
            this.listBox_DescriptiveStatistic1_ForceField.TabIndex = 66;
            // 
            // listBox_DescriptiveStatistic1_TrialType
            // 
            this.listBox_DescriptiveStatistic1_TrialType.FormattingEnabled = true;
            this.listBox_DescriptiveStatistic1_TrialType.Location = new System.Drawing.Point(227, 467);
            this.listBox_DescriptiveStatistic1_TrialType.Name = "listBox_DescriptiveStatistic1_TrialType";
            this.listBox_DescriptiveStatistic1_TrialType.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox_DescriptiveStatistic1_TrialType.Size = new System.Drawing.Size(158, 69);
            this.listBox_DescriptiveStatistic1_TrialType.TabIndex = 65;
            // 
            // checkBox_DescriptiveStatistic1_PlotFit
            // 
            this.checkBox_DescriptiveStatistic1_PlotFit.AutoSize = true;
            this.checkBox_DescriptiveStatistic1_PlotFit.Location = new System.Drawing.Point(611, 568);
            this.checkBox_DescriptiveStatistic1_PlotFit.Name = "checkBox_DescriptiveStatistic1_PlotFit";
            this.checkBox_DescriptiveStatistic1_PlotFit.Size = new System.Drawing.Size(37, 17);
            this.checkBox_DescriptiveStatistic1_PlotFit.TabIndex = 24;
            this.checkBox_DescriptiveStatistic1_PlotFit.Text = "Fit";
            this.checkBox_DescriptiveStatistic1_PlotFit.UseVisualStyleBackColor = true;
            // 
            // textBox_DescriptiveStatistic1_FitEquation
            // 
            this.textBox_DescriptiveStatistic1_FitEquation.Location = new System.Drawing.Point(611, 542);
            this.textBox_DescriptiveStatistic1_FitEquation.Name = "textBox_DescriptiveStatistic1_FitEquation";
            this.textBox_DescriptiveStatistic1_FitEquation.Size = new System.Drawing.Size(105, 20);
            this.textBox_DescriptiveStatistic1_FitEquation.TabIndex = 23;
            this.textBox_DescriptiveStatistic1_FitEquation.Text = "exp2";
            // 
            // button_DescriptiveStatistic1_ExportData
            // 
            this.button_DescriptiveStatistic1_ExportData.Location = new System.Drawing.Point(500, 571);
            this.button_DescriptiveStatistic1_ExportData.Name = "button_DescriptiveStatistic1_ExportData";
            this.button_DescriptiveStatistic1_ExportData.Size = new System.Drawing.Size(85, 23);
            this.button_DescriptiveStatistic1_ExportData.TabIndex = 22;
            this.button_DescriptiveStatistic1_ExportData.Text = "Export data";
            this.button_DescriptiveStatistic1_ExportData.UseVisualStyleBackColor = true;
            this.button_DescriptiveStatistic1_ExportData.Click += new System.EventHandler(this.button_DescriptiveStatistic1_ExportData_Click);
            // 
            // listBox_DescriptiveStatistic1_Turns
            // 
            this.listBox_DescriptiveStatistic1_Turns.FormattingEnabled = true;
            this.listBox_DescriptiveStatistic1_Turns.Location = new System.Drawing.Point(71, 268);
            this.listBox_DescriptiveStatistic1_Turns.Name = "listBox_DescriptiveStatistic1_Turns";
            this.listBox_DescriptiveStatistic1_Turns.ScrollAlwaysVisible = true;
            this.listBox_DescriptiveStatistic1_Turns.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox_DescriptiveStatistic1_Turns.Size = new System.Drawing.Size(150, 43);
            this.listBox_DescriptiveStatistic1_Turns.Sorted = true;
            this.listBox_DescriptiveStatistic1_Turns.TabIndex = 21;
            this.listBox_DescriptiveStatistic1_Turns.SelectedIndexChanged += new System.EventHandler(this.listBox_DescriptiveStatistic1_Turns_SelectedIndexChanged);
            // 
            // label_DescriptiveStatistic1_Turns
            // 
            this.label_DescriptiveStatistic1_Turns.AutoSize = true;
            this.label_DescriptiveStatistic1_Turns.Location = new System.Drawing.Point(22, 278);
            this.label_DescriptiveStatistic1_Turns.Name = "label_DescriptiveStatistic1_Turns";
            this.label_DescriptiveStatistic1_Turns.Size = new System.Drawing.Size(43, 13);
            this.label_DescriptiveStatistic1_Turns.TabIndex = 19;
            this.label_DescriptiveStatistic1_Turns.Text = "Turn(s):";
            // 
            // checkBox_DescriptiveStatistic1_PlotErrorbars
            // 
            this.checkBox_DescriptiveStatistic1_PlotErrorbars.AutoSize = true;
            this.checkBox_DescriptiveStatistic1_PlotErrorbars.Location = new System.Drawing.Point(611, 584);
            this.checkBox_DescriptiveStatistic1_PlotErrorbars.Name = "checkBox_DescriptiveStatistic1_PlotErrorbars";
            this.checkBox_DescriptiveStatistic1_PlotErrorbars.Size = new System.Drawing.Size(68, 17);
            this.checkBox_DescriptiveStatistic1_PlotErrorbars.TabIndex = 17;
            this.checkBox_DescriptiveStatistic1_PlotErrorbars.Text = "Errorbars";
            this.checkBox_DescriptiveStatistic1_PlotErrorbars.UseVisualStyleBackColor = true;
            // 
            // button_DescriptiveStatistic1_AddAll
            // 
            this.button_DescriptiveStatistic1_AddAll.Location = new System.Drawing.Point(227, 571);
            this.button_DescriptiveStatistic1_AddAll.Name = "button_DescriptiveStatistic1_AddAll";
            this.button_DescriptiveStatistic1_AddAll.Size = new System.Drawing.Size(85, 23);
            this.button_DescriptiveStatistic1_AddAll.TabIndex = 16;
            this.button_DescriptiveStatistic1_AddAll.Text = "Add all";
            this.button_DescriptiveStatistic1_AddAll.UseVisualStyleBackColor = true;
            this.button_DescriptiveStatistic1_AddAll.Click += new System.EventHandler(this.button_StatisticPlots_AddAll_Click);
            // 
            // comboBox_DescriptiveStatistic1_DataTypeSelect
            // 
            this.comboBox_DescriptiveStatistic1_DataTypeSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_DescriptiveStatistic1_DataTypeSelect.FormattingEnabled = true;
            this.comboBox_DescriptiveStatistic1_DataTypeSelect.Items.AddRange(new object[] {
            "Enclosed area",
            "PD - Abs",
            "PD - Sign",
            "PDmax - Abs",
            "PDmax - Sign",
            "PDmean - Abs",
            "PDVmax - Abs",
            "PDVmax - Sign",
            "RMSE",
            "Trajectory length abs",
            "Trajectory length ratio",
            "Vector correlation",
            "Vector correlation fisher-z",
            "Vector correlation fisher-z to r-values"});
            this.comboBox_DescriptiveStatistic1_DataTypeSelect.Location = new System.Drawing.Point(409, 542);
            this.comboBox_DescriptiveStatistic1_DataTypeSelect.Name = "comboBox_DescriptiveStatistic1_DataTypeSelect";
            this.comboBox_DescriptiveStatistic1_DataTypeSelect.Size = new System.Drawing.Size(196, 21);
            this.comboBox_DescriptiveStatistic1_DataTypeSelect.Sorted = true;
            this.comboBox_DescriptiveStatistic1_DataTypeSelect.TabIndex = 15;
            // 
            // button_DescriptiveStatistic1_PlotMeanStd
            // 
            this.button_DescriptiveStatistic1_PlotMeanStd.Location = new System.Drawing.Point(409, 571);
            this.button_DescriptiveStatistic1_PlotMeanStd.Name = "button_DescriptiveStatistic1_PlotMeanStd";
            this.button_DescriptiveStatistic1_PlotMeanStd.Size = new System.Drawing.Size(85, 23);
            this.button_DescriptiveStatistic1_PlotMeanStd.TabIndex = 14;
            this.button_DescriptiveStatistic1_PlotMeanStd.Text = "Plot data";
            this.button_DescriptiveStatistic1_PlotMeanStd.UseVisualStyleBackColor = true;
            this.button_DescriptiveStatistic1_PlotMeanStd.Click += new System.EventHandler(this.button_StatisticPlots_PlotMeanStd_Click);
            // 
            // button_DescriptiveStatistic1_ClearAll
            // 
            this.button_DescriptiveStatistic1_ClearAll.Location = new System.Drawing.Point(318, 571);
            this.button_DescriptiveStatistic1_ClearAll.Name = "button_DescriptiveStatistic1_ClearAll";
            this.button_DescriptiveStatistic1_ClearAll.Size = new System.Drawing.Size(85, 23);
            this.button_DescriptiveStatistic1_ClearAll.TabIndex = 13;
            this.button_DescriptiveStatistic1_ClearAll.Text = "Clear all";
            this.button_DescriptiveStatistic1_ClearAll.UseVisualStyleBackColor = true;
            this.button_DescriptiveStatistic1_ClearAll.Click += new System.EventHandler(this.button_StatisticPlots_ClearAll_Click);
            // 
            // button_DescriptiveStatistic1_ClearSelected
            // 
            this.button_DescriptiveStatistic1_ClearSelected.Location = new System.Drawing.Point(318, 542);
            this.button_DescriptiveStatistic1_ClearSelected.Name = "button_DescriptiveStatistic1_ClearSelected";
            this.button_DescriptiveStatistic1_ClearSelected.Size = new System.Drawing.Size(85, 23);
            this.button_DescriptiveStatistic1_ClearSelected.TabIndex = 12;
            this.button_DescriptiveStatistic1_ClearSelected.Text = "Clear selected";
            this.button_DescriptiveStatistic1_ClearSelected.UseVisualStyleBackColor = true;
            this.button_DescriptiveStatistic1_ClearSelected.Click += new System.EventHandler(this.button_StatisticPlots_ClearSelected_Click);
            // 
            // listBox_DescriptiveStatistic1_SelectedTrials
            // 
            this.listBox_DescriptiveStatistic1_SelectedTrials.FormattingEnabled = true;
            this.listBox_DescriptiveStatistic1_SelectedTrials.Location = new System.Drawing.Point(227, 12);
            this.listBox_DescriptiveStatistic1_SelectedTrials.Name = "listBox_DescriptiveStatistic1_SelectedTrials";
            this.listBox_DescriptiveStatistic1_SelectedTrials.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox_DescriptiveStatistic1_SelectedTrials.Size = new System.Drawing.Size(489, 433);
            this.listBox_DescriptiveStatistic1_SelectedTrials.Sorted = true;
            this.listBox_DescriptiveStatistic1_SelectedTrials.TabIndex = 11;
            // 
            // button_DescriptiveStatistic1_AddSelected
            // 
            this.button_DescriptiveStatistic1_AddSelected.Location = new System.Drawing.Point(227, 542);
            this.button_DescriptiveStatistic1_AddSelected.Name = "button_DescriptiveStatistic1_AddSelected";
            this.button_DescriptiveStatistic1_AddSelected.Size = new System.Drawing.Size(85, 23);
            this.button_DescriptiveStatistic1_AddSelected.TabIndex = 10;
            this.button_DescriptiveStatistic1_AddSelected.Text = "Add selected";
            this.button_DescriptiveStatistic1_AddSelected.UseVisualStyleBackColor = true;
            this.button_DescriptiveStatistic1_AddSelected.Click += new System.EventHandler(this.button_StatisticPlots_AddSelected_Click);
            // 
            // label_DescriptiveStatistic1_Trials
            // 
            this.label_DescriptiveStatistic1_Trials.AutoSize = true;
            this.label_DescriptiveStatistic1_Trials.Location = new System.Drawing.Point(28, 449);
            this.label_DescriptiveStatistic1_Trials.Name = "label_DescriptiveStatistic1_Trials";
            this.label_DescriptiveStatistic1_Trials.Size = new System.Drawing.Size(41, 13);
            this.label_DescriptiveStatistic1_Trials.TabIndex = 9;
            this.label_DescriptiveStatistic1_Trials.Text = "Trial(s):";
            // 
            // label_DescriptiveStatistic1_Subject
            // 
            this.label_DescriptiveStatistic1_Subject.AutoSize = true;
            this.label_DescriptiveStatistic1_Subject.Location = new System.Drawing.Point(8, 207);
            this.label_DescriptiveStatistic1_Subject.Name = "label_DescriptiveStatistic1_Subject";
            this.label_DescriptiveStatistic1_Subject.Size = new System.Drawing.Size(57, 13);
            this.label_DescriptiveStatistic1_Subject.TabIndex = 8;
            this.label_DescriptiveStatistic1_Subject.Text = "Subject(s):";
            // 
            // label_DescriptiveStatistic1_Szenario
            // 
            this.label_DescriptiveStatistic1_Szenario.AutoSize = true;
            this.label_DescriptiveStatistic1_Szenario.Location = new System.Drawing.Point(14, 143);
            this.label_DescriptiveStatistic1_Szenario.Name = "label_DescriptiveStatistic1_Szenario";
            this.label_DescriptiveStatistic1_Szenario.Size = new System.Drawing.Size(51, 13);
            this.label_DescriptiveStatistic1_Szenario.TabIndex = 7;
            this.label_DescriptiveStatistic1_Szenario.Text = "Szenario:";
            // 
            // label_DescriptiveStatistic1_Groups
            // 
            this.label_DescriptiveStatistic1_Groups.AutoSize = true;
            this.label_DescriptiveStatistic1_Groups.Location = new System.Drawing.Point(15, 75);
            this.label_DescriptiveStatistic1_Groups.Name = "label_DescriptiveStatistic1_Groups";
            this.label_DescriptiveStatistic1_Groups.Size = new System.Drawing.Size(50, 13);
            this.label_DescriptiveStatistic1_Groups.TabIndex = 6;
            this.label_DescriptiveStatistic1_Groups.Text = "Group(s):";
            // 
            // label_DescriptiveStatistic1_Study
            // 
            this.label_DescriptiveStatistic1_Study.AutoSize = true;
            this.label_DescriptiveStatistic1_Study.Location = new System.Drawing.Point(28, 15);
            this.label_DescriptiveStatistic1_Study.Name = "label_DescriptiveStatistic1_Study";
            this.label_DescriptiveStatistic1_Study.Size = new System.Drawing.Size(37, 13);
            this.label_DescriptiveStatistic1_Study.TabIndex = 5;
            this.label_DescriptiveStatistic1_Study.Text = "Study:";
            // 
            // listBox_DescriptiveStatistic1_Trials
            // 
            this.listBox_DescriptiveStatistic1_Trials.FormattingEnabled = true;
            this.listBox_DescriptiveStatistic1_Trials.Location = new System.Drawing.Point(71, 317);
            this.listBox_DescriptiveStatistic1_Trials.Name = "listBox_DescriptiveStatistic1_Trials";
            this.listBox_DescriptiveStatistic1_Trials.ScrollAlwaysVisible = true;
            this.listBox_DescriptiveStatistic1_Trials.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox_DescriptiveStatistic1_Trials.Size = new System.Drawing.Size(150, 277);
            this.listBox_DescriptiveStatistic1_Trials.Sorted = true;
            this.listBox_DescriptiveStatistic1_Trials.TabIndex = 4;
            // 
            // listBox_DescriptiveStatistic1_Subjects
            // 
            this.listBox_DescriptiveStatistic1_Subjects.FormattingEnabled = true;
            this.listBox_DescriptiveStatistic1_Subjects.Location = new System.Drawing.Point(71, 167);
            this.listBox_DescriptiveStatistic1_Subjects.Name = "listBox_DescriptiveStatistic1_Subjects";
            this.listBox_DescriptiveStatistic1_Subjects.ScrollAlwaysVisible = true;
            this.listBox_DescriptiveStatistic1_Subjects.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox_DescriptiveStatistic1_Subjects.Size = new System.Drawing.Size(150, 95);
            this.listBox_DescriptiveStatistic1_Subjects.Sorted = true;
            this.listBox_DescriptiveStatistic1_Subjects.TabIndex = 3;
            this.listBox_DescriptiveStatistic1_Subjects.SelectedIndexChanged += new System.EventHandler(this.listBox_StatisticPlots_Subject_SelectedIndexChanged);
            // 
            // comboBox_DescriptiveStatistic1_Szenario
            // 
            this.comboBox_DescriptiveStatistic1_Szenario.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_DescriptiveStatistic1_Szenario.FormattingEnabled = true;
            this.comboBox_DescriptiveStatistic1_Szenario.Location = new System.Drawing.Point(71, 140);
            this.comboBox_DescriptiveStatistic1_Szenario.Name = "comboBox_DescriptiveStatistic1_Szenario";
            this.comboBox_DescriptiveStatistic1_Szenario.Size = new System.Drawing.Size(150, 21);
            this.comboBox_DescriptiveStatistic1_Szenario.Sorted = true;
            this.comboBox_DescriptiveStatistic1_Szenario.TabIndex = 2;
            this.comboBox_DescriptiveStatistic1_Szenario.SelectedIndexChanged += new System.EventHandler(this.comboBox_StatisticPlots_Szenario_SelectedIndexChanged);
            // 
            // comboBox_DescriptiveStatistic1_Study
            // 
            this.comboBox_DescriptiveStatistic1_Study.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_DescriptiveStatistic1_Study.FormattingEnabled = true;
            this.comboBox_DescriptiveStatistic1_Study.Location = new System.Drawing.Point(71, 12);
            this.comboBox_DescriptiveStatistic1_Study.Name = "comboBox_DescriptiveStatistic1_Study";
            this.comboBox_DescriptiveStatistic1_Study.Size = new System.Drawing.Size(150, 21);
            this.comboBox_DescriptiveStatistic1_Study.Sorted = true;
            this.comboBox_DescriptiveStatistic1_Study.TabIndex = 1;
            this.comboBox_DescriptiveStatistic1_Study.SelectedIndexChanged += new System.EventHandler(this.comboBox_StatisticPlots_Study_SelectedIndexChanged);
            // 
            // listBox_DescriptiveStatistic1_Groups
            // 
            this.listBox_DescriptiveStatistic1_Groups.FormattingEnabled = true;
            this.listBox_DescriptiveStatistic1_Groups.Location = new System.Drawing.Point(71, 39);
            this.listBox_DescriptiveStatistic1_Groups.Name = "listBox_DescriptiveStatistic1_Groups";
            this.listBox_DescriptiveStatistic1_Groups.ScrollAlwaysVisible = true;
            this.listBox_DescriptiveStatistic1_Groups.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox_DescriptiveStatistic1_Groups.Size = new System.Drawing.Size(150, 95);
            this.listBox_DescriptiveStatistic1_Groups.Sorted = true;
            this.listBox_DescriptiveStatistic1_Groups.TabIndex = 0;
            this.listBox_DescriptiveStatistic1_Groups.SelectedIndexChanged += new System.EventHandler(this.listBox_StatisticPlots_Group_SelectedIndexChanged);
            // 
            // tabPage_DescriptiveStatistic2
            // 
            this.tabPage_DescriptiveStatistic2.Controls.Add(this.label_DescriptiveStatistic2_Handedness);
            this.tabPage_DescriptiveStatistic2.Controls.Add(this.label_DescriptiveStatistic2_ForceField);
            this.tabPage_DescriptiveStatistic2.Controls.Add(this.label_DescriptiveStatistic2_TrialType);
            this.tabPage_DescriptiveStatistic2.Controls.Add(this.listBox_DescriptiveStatistic2_Handedness);
            this.tabPage_DescriptiveStatistic2.Controls.Add(this.listBox_DescriptiveStatistic2_ForceField);
            this.tabPage_DescriptiveStatistic2.Controls.Add(this.listBox_DescriptiveStatistic2_TrialType);
            this.tabPage_DescriptiveStatistic2.Controls.Add(this.button_DescriptiveStatistic2_CalculateMeanValues);
            this.tabPage_DescriptiveStatistic2.Controls.Add(this.listBox_DescriptiveStatistic2_Turns);
            this.tabPage_DescriptiveStatistic2.Controls.Add(this.label_DescriptiveStatistic2_Turns);
            this.tabPage_DescriptiveStatistic2.Controls.Add(this.button_DescriptiveStatistic2_AddAll);
            this.tabPage_DescriptiveStatistic2.Controls.Add(this.comboBox_DescriptiveStatistic2_DataTypeSelect);
            this.tabPage_DescriptiveStatistic2.Controls.Add(this.button_DescriptiveStatistic2_ClearAll);
            this.tabPage_DescriptiveStatistic2.Controls.Add(this.button_DescriptiveStatistic2_ClearSelected);
            this.tabPage_DescriptiveStatistic2.Controls.Add(this.listBox_DescriptiveStatistic2_SelectedTrials);
            this.tabPage_DescriptiveStatistic2.Controls.Add(this.button_DescriptiveStatistic2_AddSelected);
            this.tabPage_DescriptiveStatistic2.Controls.Add(this.label_DescriptiveStatistic2_Trials);
            this.tabPage_DescriptiveStatistic2.Controls.Add(this.label_DescriptiveStatistic2_Subject);
            this.tabPage_DescriptiveStatistic2.Controls.Add(this.label_DescriptiveStatistic2_Szenario);
            this.tabPage_DescriptiveStatistic2.Controls.Add(this.label_DescriptiveStatistic2_Groups);
            this.tabPage_DescriptiveStatistic2.Controls.Add(this.label_DescriptiveStatistic2_Study);
            this.tabPage_DescriptiveStatistic2.Controls.Add(this.listBox_DescriptiveStatistic2_Trials);
            this.tabPage_DescriptiveStatistic2.Controls.Add(this.listBox_DescriptiveStatistic2_Subjects);
            this.tabPage_DescriptiveStatistic2.Controls.Add(this.comboBox_DescriptiveStatistic2_Szenario);
            this.tabPage_DescriptiveStatistic2.Controls.Add(this.comboBox_DescriptiveStatistic2_Study);
            this.tabPage_DescriptiveStatistic2.Controls.Add(this.listBox_DescriptiveStatistic2_Groups);
            this.tabPage_DescriptiveStatistic2.Location = new System.Drawing.Point(4, 22);
            this.tabPage_DescriptiveStatistic2.Name = "tabPage_DescriptiveStatistic2";
            this.tabPage_DescriptiveStatistic2.Size = new System.Drawing.Size(719, 602);
            this.tabPage_DescriptiveStatistic2.TabIndex = 3;
            this.tabPage_DescriptiveStatistic2.Text = "Descriptive Statistic 2";
            this.tabPage_DescriptiveStatistic2.UseVisualStyleBackColor = true;
            this.tabPage_DescriptiveStatistic2.Enter += new System.EventHandler(this.tabPage_DescriptiveStatistic2_Enter);
            // 
            // label_DescriptiveStatistic2_Handedness
            // 
            this.label_DescriptiveStatistic2_Handedness.AutoSize = true;
            this.label_DescriptiveStatistic2_Handedness.Location = new System.Drawing.Point(552, 449);
            this.label_DescriptiveStatistic2_Handedness.Name = "label_DescriptiveStatistic2_Handedness";
            this.label_DescriptiveStatistic2_Handedness.Size = new System.Drawing.Size(70, 13);
            this.label_DescriptiveStatistic2_Handedness.TabIndex = 70;
            this.label_DescriptiveStatistic2_Handedness.Text = "Handedness:";
            // 
            // label_DescriptiveStatistic2_ForceField
            // 
            this.label_DescriptiveStatistic2_ForceField.AutoSize = true;
            this.label_DescriptiveStatistic2_ForceField.Location = new System.Drawing.Point(388, 449);
            this.label_DescriptiveStatistic2_ForceField.Name = "label_DescriptiveStatistic2_ForceField";
            this.label_DescriptiveStatistic2_ForceField.Size = new System.Drawing.Size(59, 13);
            this.label_DescriptiveStatistic2_ForceField.TabIndex = 69;
            this.label_DescriptiveStatistic2_ForceField.Text = "Force field:";
            // 
            // label_DescriptiveStatistic2_TrialType
            // 
            this.label_DescriptiveStatistic2_TrialType.AutoSize = true;
            this.label_DescriptiveStatistic2_TrialType.Location = new System.Drawing.Point(227, 449);
            this.label_DescriptiveStatistic2_TrialType.Name = "label_DescriptiveStatistic2_TrialType";
            this.label_DescriptiveStatistic2_TrialType.Size = new System.Drawing.Size(53, 13);
            this.label_DescriptiveStatistic2_TrialType.TabIndex = 68;
            this.label_DescriptiveStatistic2_TrialType.Text = "Trial type:";
            // 
            // listBox_DescriptiveStatistic2_Handedness
            // 
            this.listBox_DescriptiveStatistic2_Handedness.FormattingEnabled = true;
            this.listBox_DescriptiveStatistic2_Handedness.Location = new System.Drawing.Point(555, 467);
            this.listBox_DescriptiveStatistic2_Handedness.Name = "listBox_DescriptiveStatistic2_Handedness";
            this.listBox_DescriptiveStatistic2_Handedness.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox_DescriptiveStatistic2_Handedness.Size = new System.Drawing.Size(158, 69);
            this.listBox_DescriptiveStatistic2_Handedness.TabIndex = 67;
            // 
            // listBox_DescriptiveStatistic2_ForceField
            // 
            this.listBox_DescriptiveStatistic2_ForceField.FormattingEnabled = true;
            this.listBox_DescriptiveStatistic2_ForceField.Location = new System.Drawing.Point(391, 467);
            this.listBox_DescriptiveStatistic2_ForceField.Name = "listBox_DescriptiveStatistic2_ForceField";
            this.listBox_DescriptiveStatistic2_ForceField.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox_DescriptiveStatistic2_ForceField.Size = new System.Drawing.Size(158, 69);
            this.listBox_DescriptiveStatistic2_ForceField.TabIndex = 66;
            // 
            // listBox_DescriptiveStatistic2_TrialType
            // 
            this.listBox_DescriptiveStatistic2_TrialType.FormattingEnabled = true;
            this.listBox_DescriptiveStatistic2_TrialType.Location = new System.Drawing.Point(227, 467);
            this.listBox_DescriptiveStatistic2_TrialType.Name = "listBox_DescriptiveStatistic2_TrialType";
            this.listBox_DescriptiveStatistic2_TrialType.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox_DescriptiveStatistic2_TrialType.Size = new System.Drawing.Size(158, 69);
            this.listBox_DescriptiveStatistic2_TrialType.TabIndex = 65;
            // 
            // button_DescriptiveStatistic2_CalculateMeanValues
            // 
            this.button_DescriptiveStatistic2_CalculateMeanValues.Location = new System.Drawing.Point(409, 571);
            this.button_DescriptiveStatistic2_CalculateMeanValues.Name = "button_DescriptiveStatistic2_CalculateMeanValues";
            this.button_DescriptiveStatistic2_CalculateMeanValues.Size = new System.Drawing.Size(196, 23);
            this.button_DescriptiveStatistic2_CalculateMeanValues.TabIndex = 43;
            this.button_DescriptiveStatistic2_CalculateMeanValues.Text = "Calculate/export mean values";
            this.button_DescriptiveStatistic2_CalculateMeanValues.UseVisualStyleBackColor = true;
            this.button_DescriptiveStatistic2_CalculateMeanValues.Click += new System.EventHandler(this.button_DescriptiveStatistic2_CalculateMeanValues_Click);
            // 
            // listBox_DescriptiveStatistic2_Turns
            // 
            this.listBox_DescriptiveStatistic2_Turns.FormattingEnabled = true;
            this.listBox_DescriptiveStatistic2_Turns.Location = new System.Drawing.Point(71, 268);
            this.listBox_DescriptiveStatistic2_Turns.Name = "listBox_DescriptiveStatistic2_Turns";
            this.listBox_DescriptiveStatistic2_Turns.ScrollAlwaysVisible = true;
            this.listBox_DescriptiveStatistic2_Turns.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox_DescriptiveStatistic2_Turns.Size = new System.Drawing.Size(150, 43);
            this.listBox_DescriptiveStatistic2_Turns.Sorted = true;
            this.listBox_DescriptiveStatistic2_Turns.TabIndex = 42;
            this.listBox_DescriptiveStatistic2_Turns.SelectedIndexChanged += new System.EventHandler(this.listBox_DescriptiveStatistic2_Turns_SelectedIndexChanged);
            // 
            // label_DescriptiveStatistic2_Turns
            // 
            this.label_DescriptiveStatistic2_Turns.AutoSize = true;
            this.label_DescriptiveStatistic2_Turns.Location = new System.Drawing.Point(22, 278);
            this.label_DescriptiveStatistic2_Turns.Name = "label_DescriptiveStatistic2_Turns";
            this.label_DescriptiveStatistic2_Turns.Size = new System.Drawing.Size(43, 13);
            this.label_DescriptiveStatistic2_Turns.TabIndex = 40;
            this.label_DescriptiveStatistic2_Turns.Text = "Turn(s):";
            // 
            // button_DescriptiveStatistic2_AddAll
            // 
            this.button_DescriptiveStatistic2_AddAll.Location = new System.Drawing.Point(227, 571);
            this.button_DescriptiveStatistic2_AddAll.Name = "button_DescriptiveStatistic2_AddAll";
            this.button_DescriptiveStatistic2_AddAll.Size = new System.Drawing.Size(85, 23);
            this.button_DescriptiveStatistic2_AddAll.TabIndex = 38;
            this.button_DescriptiveStatistic2_AddAll.Text = "Add all";
            this.button_DescriptiveStatistic2_AddAll.UseVisualStyleBackColor = true;
            this.button_DescriptiveStatistic2_AddAll.Click += new System.EventHandler(this.button_DescriptiveStatistic2_AddAll_Click);
            // 
            // comboBox_DescriptiveStatistic2_DataTypeSelect
            // 
            this.comboBox_DescriptiveStatistic2_DataTypeSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_DescriptiveStatistic2_DataTypeSelect.FormattingEnabled = true;
            this.comboBox_DescriptiveStatistic2_DataTypeSelect.Items.AddRange(new object[] {
            "Enclosed area",
            "Max perpendicular distance - Abs",
            "Max perpendicular distance - Sign",
            "Mean perpendicular distance - Abs",
            "Perpendicular distance 300ms - Abs",
            "Perpendicular distance 300ms - Sign",
            "RMSE",
            "Trajectory length abs",
            "Trajectory length ratio",
            "Vector correlation",
            "Vector correlation fisher-z",
            "Vector correlation fisher-z to r-values"});
            this.comboBox_DescriptiveStatistic2_DataTypeSelect.Location = new System.Drawing.Point(409, 542);
            this.comboBox_DescriptiveStatistic2_DataTypeSelect.Name = "comboBox_DescriptiveStatistic2_DataTypeSelect";
            this.comboBox_DescriptiveStatistic2_DataTypeSelect.Size = new System.Drawing.Size(196, 21);
            this.comboBox_DescriptiveStatistic2_DataTypeSelect.Sorted = true;
            this.comboBox_DescriptiveStatistic2_DataTypeSelect.TabIndex = 37;
            // 
            // button_DescriptiveStatistic2_ClearAll
            // 
            this.button_DescriptiveStatistic2_ClearAll.Location = new System.Drawing.Point(318, 571);
            this.button_DescriptiveStatistic2_ClearAll.Name = "button_DescriptiveStatistic2_ClearAll";
            this.button_DescriptiveStatistic2_ClearAll.Size = new System.Drawing.Size(85, 23);
            this.button_DescriptiveStatistic2_ClearAll.TabIndex = 35;
            this.button_DescriptiveStatistic2_ClearAll.Text = "Clear all";
            this.button_DescriptiveStatistic2_ClearAll.UseVisualStyleBackColor = true;
            this.button_DescriptiveStatistic2_ClearAll.Click += new System.EventHandler(this.button_DescriptiveStatistic2_ClearAll_Click);
            // 
            // button_DescriptiveStatistic2_ClearSelected
            // 
            this.button_DescriptiveStatistic2_ClearSelected.Location = new System.Drawing.Point(318, 542);
            this.button_DescriptiveStatistic2_ClearSelected.Name = "button_DescriptiveStatistic2_ClearSelected";
            this.button_DescriptiveStatistic2_ClearSelected.Size = new System.Drawing.Size(85, 23);
            this.button_DescriptiveStatistic2_ClearSelected.TabIndex = 34;
            this.button_DescriptiveStatistic2_ClearSelected.Text = "Clear selected";
            this.button_DescriptiveStatistic2_ClearSelected.UseVisualStyleBackColor = true;
            this.button_DescriptiveStatistic2_ClearSelected.Click += new System.EventHandler(this.button_DescriptiveStatistic2_ClearSelected_Click);
            // 
            // listBox_DescriptiveStatistic2_SelectedTrials
            // 
            this.listBox_DescriptiveStatistic2_SelectedTrials.FormattingEnabled = true;
            this.listBox_DescriptiveStatistic2_SelectedTrials.Location = new System.Drawing.Point(227, 12);
            this.listBox_DescriptiveStatistic2_SelectedTrials.Name = "listBox_DescriptiveStatistic2_SelectedTrials";
            this.listBox_DescriptiveStatistic2_SelectedTrials.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox_DescriptiveStatistic2_SelectedTrials.Size = new System.Drawing.Size(489, 433);
            this.listBox_DescriptiveStatistic2_SelectedTrials.Sorted = true;
            this.listBox_DescriptiveStatistic2_SelectedTrials.TabIndex = 33;
            // 
            // button_DescriptiveStatistic2_AddSelected
            // 
            this.button_DescriptiveStatistic2_AddSelected.Location = new System.Drawing.Point(227, 542);
            this.button_DescriptiveStatistic2_AddSelected.Name = "button_DescriptiveStatistic2_AddSelected";
            this.button_DescriptiveStatistic2_AddSelected.Size = new System.Drawing.Size(85, 23);
            this.button_DescriptiveStatistic2_AddSelected.TabIndex = 32;
            this.button_DescriptiveStatistic2_AddSelected.Text = "Add selected";
            this.button_DescriptiveStatistic2_AddSelected.UseVisualStyleBackColor = true;
            this.button_DescriptiveStatistic2_AddSelected.Click += new System.EventHandler(this.button_DescriptiveStatistic2_AddSelected_Click);
            // 
            // label_DescriptiveStatistic2_Trials
            // 
            this.label_DescriptiveStatistic2_Trials.AutoSize = true;
            this.label_DescriptiveStatistic2_Trials.Location = new System.Drawing.Point(28, 449);
            this.label_DescriptiveStatistic2_Trials.Name = "label_DescriptiveStatistic2_Trials";
            this.label_DescriptiveStatistic2_Trials.Size = new System.Drawing.Size(41, 13);
            this.label_DescriptiveStatistic2_Trials.TabIndex = 31;
            this.label_DescriptiveStatistic2_Trials.Text = "Trial(s):";
            // 
            // label_DescriptiveStatistic2_Subject
            // 
            this.label_DescriptiveStatistic2_Subject.AutoSize = true;
            this.label_DescriptiveStatistic2_Subject.Location = new System.Drawing.Point(8, 207);
            this.label_DescriptiveStatistic2_Subject.Name = "label_DescriptiveStatistic2_Subject";
            this.label_DescriptiveStatistic2_Subject.Size = new System.Drawing.Size(57, 13);
            this.label_DescriptiveStatistic2_Subject.TabIndex = 30;
            this.label_DescriptiveStatistic2_Subject.Text = "Subject(s):";
            // 
            // label_DescriptiveStatistic2_Szenario
            // 
            this.label_DescriptiveStatistic2_Szenario.AutoSize = true;
            this.label_DescriptiveStatistic2_Szenario.Location = new System.Drawing.Point(14, 143);
            this.label_DescriptiveStatistic2_Szenario.Name = "label_DescriptiveStatistic2_Szenario";
            this.label_DescriptiveStatistic2_Szenario.Size = new System.Drawing.Size(51, 13);
            this.label_DescriptiveStatistic2_Szenario.TabIndex = 29;
            this.label_DescriptiveStatistic2_Szenario.Text = "Szenario:";
            // 
            // label_DescriptiveStatistic2_Groups
            // 
            this.label_DescriptiveStatistic2_Groups.AutoSize = true;
            this.label_DescriptiveStatistic2_Groups.Location = new System.Drawing.Point(15, 75);
            this.label_DescriptiveStatistic2_Groups.Name = "label_DescriptiveStatistic2_Groups";
            this.label_DescriptiveStatistic2_Groups.Size = new System.Drawing.Size(50, 13);
            this.label_DescriptiveStatistic2_Groups.TabIndex = 28;
            this.label_DescriptiveStatistic2_Groups.Text = "Group(s):";
            // 
            // label_DescriptiveStatistic2_Study
            // 
            this.label_DescriptiveStatistic2_Study.AutoSize = true;
            this.label_DescriptiveStatistic2_Study.Location = new System.Drawing.Point(28, 15);
            this.label_DescriptiveStatistic2_Study.Name = "label_DescriptiveStatistic2_Study";
            this.label_DescriptiveStatistic2_Study.Size = new System.Drawing.Size(37, 13);
            this.label_DescriptiveStatistic2_Study.TabIndex = 27;
            this.label_DescriptiveStatistic2_Study.Text = "Study:";
            // 
            // listBox_DescriptiveStatistic2_Trials
            // 
            this.listBox_DescriptiveStatistic2_Trials.FormattingEnabled = true;
            this.listBox_DescriptiveStatistic2_Trials.Location = new System.Drawing.Point(71, 317);
            this.listBox_DescriptiveStatistic2_Trials.Name = "listBox_DescriptiveStatistic2_Trials";
            this.listBox_DescriptiveStatistic2_Trials.ScrollAlwaysVisible = true;
            this.listBox_DescriptiveStatistic2_Trials.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox_DescriptiveStatistic2_Trials.Size = new System.Drawing.Size(150, 277);
            this.listBox_DescriptiveStatistic2_Trials.Sorted = true;
            this.listBox_DescriptiveStatistic2_Trials.TabIndex = 26;
            // 
            // listBox_DescriptiveStatistic2_Subjects
            // 
            this.listBox_DescriptiveStatistic2_Subjects.FormattingEnabled = true;
            this.listBox_DescriptiveStatistic2_Subjects.Location = new System.Drawing.Point(71, 167);
            this.listBox_DescriptiveStatistic2_Subjects.Name = "listBox_DescriptiveStatistic2_Subjects";
            this.listBox_DescriptiveStatistic2_Subjects.ScrollAlwaysVisible = true;
            this.listBox_DescriptiveStatistic2_Subjects.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox_DescriptiveStatistic2_Subjects.Size = new System.Drawing.Size(150, 95);
            this.listBox_DescriptiveStatistic2_Subjects.Sorted = true;
            this.listBox_DescriptiveStatistic2_Subjects.TabIndex = 25;
            this.listBox_DescriptiveStatistic2_Subjects.SelectedIndexChanged += new System.EventHandler(this.listBox_DescriptiveStatistic2_Subject_SelectedIndexChanged);
            // 
            // comboBox_DescriptiveStatistic2_Szenario
            // 
            this.comboBox_DescriptiveStatistic2_Szenario.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_DescriptiveStatistic2_Szenario.FormattingEnabled = true;
            this.comboBox_DescriptiveStatistic2_Szenario.Location = new System.Drawing.Point(71, 140);
            this.comboBox_DescriptiveStatistic2_Szenario.Name = "comboBox_DescriptiveStatistic2_Szenario";
            this.comboBox_DescriptiveStatistic2_Szenario.Size = new System.Drawing.Size(150, 21);
            this.comboBox_DescriptiveStatistic2_Szenario.Sorted = true;
            this.comboBox_DescriptiveStatistic2_Szenario.TabIndex = 24;
            this.comboBox_DescriptiveStatistic2_Szenario.SelectedIndexChanged += new System.EventHandler(this.comboBox_DescriptiveStatistic2_Szenario_SelectedIndexChanged);
            // 
            // comboBox_DescriptiveStatistic2_Study
            // 
            this.comboBox_DescriptiveStatistic2_Study.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_DescriptiveStatistic2_Study.FormattingEnabled = true;
            this.comboBox_DescriptiveStatistic2_Study.Location = new System.Drawing.Point(71, 12);
            this.comboBox_DescriptiveStatistic2_Study.Name = "comboBox_DescriptiveStatistic2_Study";
            this.comboBox_DescriptiveStatistic2_Study.Size = new System.Drawing.Size(150, 21);
            this.comboBox_DescriptiveStatistic2_Study.Sorted = true;
            this.comboBox_DescriptiveStatistic2_Study.TabIndex = 23;
            this.comboBox_DescriptiveStatistic2_Study.SelectedIndexChanged += new System.EventHandler(this.comboBox_DescriptiveStatistic2_Study_SelectedIndexChanged);
            // 
            // listBox_DescriptiveStatistic2_Groups
            // 
            this.listBox_DescriptiveStatistic2_Groups.FormattingEnabled = true;
            this.listBox_DescriptiveStatistic2_Groups.Location = new System.Drawing.Point(71, 39);
            this.listBox_DescriptiveStatistic2_Groups.Name = "listBox_DescriptiveStatistic2_Groups";
            this.listBox_DescriptiveStatistic2_Groups.ScrollAlwaysVisible = true;
            this.listBox_DescriptiveStatistic2_Groups.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox_DescriptiveStatistic2_Groups.Size = new System.Drawing.Size(150, 95);
            this.listBox_DescriptiveStatistic2_Groups.Sorted = true;
            this.listBox_DescriptiveStatistic2_Groups.TabIndex = 22;
            this.listBox_DescriptiveStatistic2_Groups.SelectedIndexChanged += new System.EventHandler(this.listBox_DescriptiveStatistic2_Groups_SelectedIndexChanged);
            // 
            // tabPage_Others
            // 
            this.tabPage_Others.Controls.Add(this.label_Others_PlotErrorclampForces_At);
            this.tabPage_Others.Controls.Add(this.label_Others_PlotErrorclampForces_Ms);
            this.tabPage_Others.Controls.Add(this.textBox_Others_PlotErrorclampForces_MsIndex);
            this.tabPage_Others.Controls.Add(this.button_Others_PlotForcefieldCompensationFactor);
            this.tabPage_Others.Controls.Add(this.checkBox_Others_GroupAverage);
            this.tabPage_Others.Controls.Add(this.button_Others_ExportGroupLi);
            this.tabPage_Others.Controls.Add(this.button_Others_PlotGroupLi);
            this.tabPage_Others.Controls.Add(this.button_Others_ExportVelocityBaseline);
            this.tabPage_Others.Controls.Add(this.button_Others_PlotVelocityBaseline);
            this.tabPage_Others.Controls.Add(this.button_Others_ExportTrajectoryBaseline);
            this.tabPage_Others.Controls.Add(this.button_Others_ExportSzenarioMeanTimes);
            this.tabPage_Others.Controls.Add(this.label_Others_Turn);
            this.tabPage_Others.Controls.Add(this.comboBox_Others_Turn);
            this.tabPage_Others.Controls.Add(this.label_Others_Group);
            this.tabPage_Others.Controls.Add(this.comboBox_Others_Group);
            this.tabPage_Others.Controls.Add(this.label_Others_Szenario);
            this.tabPage_Others.Controls.Add(this.label_Others_Subject);
            this.tabPage_Others.Controls.Add(this.label_Others_Study);
            this.tabPage_Others.Controls.Add(this.comboBox_Others_Subject);
            this.tabPage_Others.Controls.Add(this.comboBox_Others_Study);
            this.tabPage_Others.Controls.Add(this.comboBox_Others_Szenario);
            this.tabPage_Others.Controls.Add(this.button_Others_PlotTrajectoryBaseline);
            this.tabPage_Others.Controls.Add(this.button_Others_PlotSzenarioMeanTimes);
            this.tabPage_Others.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Others.Name = "tabPage_Others";
            this.tabPage_Others.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Others.Size = new System.Drawing.Size(719, 602);
            this.tabPage_Others.TabIndex = 0;
            this.tabPage_Others.Text = "Others";
            this.tabPage_Others.UseVisualStyleBackColor = true;
            this.tabPage_Others.Enter += new System.EventHandler(this.tabPage_Others_Enter);
            // 
            // label_Others_PlotErrorclampForces_At
            // 
            this.label_Others_PlotErrorclampForces_At.AutoSize = true;
            this.label_Others_PlotErrorclampForces_At.Location = new System.Drawing.Point(192, 268);
            this.label_Others_PlotErrorclampForces_At.Name = "label_Others_PlotErrorclampForces_At";
            this.label_Others_PlotErrorclampForces_At.Size = new System.Drawing.Size(16, 13);
            this.label_Others_PlotErrorclampForces_At.TabIndex = 56;
            this.label_Others_PlotErrorclampForces_At.Text = "at";
            // 
            // label_Others_PlotErrorclampForces_Ms
            // 
            this.label_Others_PlotErrorclampForces_Ms.AutoSize = true;
            this.label_Others_PlotErrorclampForces_Ms.Location = new System.Drawing.Point(300, 268);
            this.label_Others_PlotErrorclampForces_Ms.Name = "label_Others_PlotErrorclampForces_Ms";
            this.label_Others_PlotErrorclampForces_Ms.Size = new System.Drawing.Size(20, 13);
            this.label_Others_PlotErrorclampForces_Ms.TabIndex = 55;
            this.label_Others_PlotErrorclampForces_Ms.Text = "ms";
            // 
            // textBox_Others_PlotErrorclampForces_MsIndex
            // 
            this.textBox_Others_PlotErrorclampForces_MsIndex.Location = new System.Drawing.Point(208, 265);
            this.textBox_Others_PlotErrorclampForces_MsIndex.Name = "textBox_Others_PlotErrorclampForces_MsIndex";
            this.textBox_Others_PlotErrorclampForces_MsIndex.Size = new System.Drawing.Size(86, 20);
            this.textBox_Others_PlotErrorclampForces_MsIndex.TabIndex = 54;
            this.textBox_Others_PlotErrorclampForces_MsIndex.Text = "300";
            // 
            // button_Others_PlotForcefieldCompensationFactor
            // 
            this.button_Others_PlotForcefieldCompensationFactor.Location = new System.Drawing.Point(15, 263);
            this.button_Others_PlotForcefieldCompensationFactor.Name = "button_Others_PlotForcefieldCompensationFactor";
            this.button_Others_PlotForcefieldCompensationFactor.Size = new System.Drawing.Size(171, 23);
            this.button_Others_PlotForcefieldCompensationFactor.TabIndex = 53;
            this.button_Others_PlotForcefieldCompensationFactor.Text = "Forcefield compensation factor";
            this.button_Others_PlotForcefieldCompensationFactor.UseVisualStyleBackColor = true;
            this.button_Others_PlotForcefieldCompensationFactor.Click += new System.EventHandler(this.button_Others_ForcefieldCompensationFactor_Click);
            // 
            // checkBox_Others_GroupAverage
            // 
            this.checkBox_Others_GroupAverage.AutoSize = true;
            this.checkBox_Others_GroupAverage.Location = new System.Drawing.Point(326, 238);
            this.checkBox_Others_GroupAverage.Name = "checkBox_Others_GroupAverage";
            this.checkBox_Others_GroupAverage.Size = new System.Drawing.Size(97, 17);
            this.checkBox_Others_GroupAverage.TabIndex = 52;
            this.checkBox_Others_GroupAverage.Text = "Group average";
            this.checkBox_Others_GroupAverage.UseVisualStyleBackColor = true;
            // 
            // button_Others_ExportGroupLi
            // 
            this.button_Others_ExportGroupLi.Location = new System.Drawing.Point(170, 234);
            this.button_Others_ExportGroupLi.Name = "button_Others_ExportGroupLi";
            this.button_Others_ExportGroupLi.Size = new System.Drawing.Size(150, 23);
            this.button_Others_ExportGroupLi.TabIndex = 47;
            this.button_Others_ExportGroupLi.Text = "Export learning index";
            this.button_Others_ExportGroupLi.UseVisualStyleBackColor = true;
            this.button_Others_ExportGroupLi.Click += new System.EventHandler(this.button_Others_ExportGroupLi_Click);
            // 
            // button_Others_PlotGroupLi
            // 
            this.button_Others_PlotGroupLi.Location = new System.Drawing.Point(15, 234);
            this.button_Others_PlotGroupLi.Name = "button_Others_PlotGroupLi";
            this.button_Others_PlotGroupLi.Size = new System.Drawing.Size(150, 23);
            this.button_Others_PlotGroupLi.TabIndex = 46;
            this.button_Others_PlotGroupLi.Text = "Plot learning index";
            this.button_Others_PlotGroupLi.UseVisualStyleBackColor = true;
            this.button_Others_PlotGroupLi.Click += new System.EventHandler(this.button_Others_PlotGroupLi_Click);
            // 
            // button_Others_ExportVelocityBaseline
            // 
            this.button_Others_ExportVelocityBaseline.Location = new System.Drawing.Point(170, 176);
            this.button_Others_ExportVelocityBaseline.Name = "button_Others_ExportVelocityBaseline";
            this.button_Others_ExportVelocityBaseline.Size = new System.Drawing.Size(150, 23);
            this.button_Others_ExportVelocityBaseline.TabIndex = 45;
            this.button_Others_ExportVelocityBaseline.Text = "Export velocity baselines";
            this.button_Others_ExportVelocityBaseline.UseVisualStyleBackColor = true;
            this.button_Others_ExportVelocityBaseline.Click += new System.EventHandler(this.button_Others_ExportVelocityBaseline_Click);
            // 
            // button_Others_PlotVelocityBaseline
            // 
            this.button_Others_PlotVelocityBaseline.Location = new System.Drawing.Point(15, 176);
            this.button_Others_PlotVelocityBaseline.Name = "button_Others_PlotVelocityBaseline";
            this.button_Others_PlotVelocityBaseline.Size = new System.Drawing.Size(150, 23);
            this.button_Others_PlotVelocityBaseline.TabIndex = 44;
            this.button_Others_PlotVelocityBaseline.Text = "Plot velocity baselines";
            this.button_Others_PlotVelocityBaseline.UseVisualStyleBackColor = true;
            this.button_Others_PlotVelocityBaseline.Click += new System.EventHandler(this.button_Others_PlotVelocityBaseline_Click);
            // 
            // button_Others_ExportTrajectoryBaseline
            // 
            this.button_Others_ExportTrajectoryBaseline.Location = new System.Drawing.Point(170, 147);
            this.button_Others_ExportTrajectoryBaseline.Name = "button_Others_ExportTrajectoryBaseline";
            this.button_Others_ExportTrajectoryBaseline.Size = new System.Drawing.Size(150, 23);
            this.button_Others_ExportTrajectoryBaseline.TabIndex = 43;
            this.button_Others_ExportTrajectoryBaseline.Text = "Export trajectory baselines";
            this.button_Others_ExportTrajectoryBaseline.UseVisualStyleBackColor = true;
            this.button_Others_ExportTrajectoryBaseline.Click += new System.EventHandler(this.button_Others_ExportBaseline_Click);
            // 
            // button_Others_ExportSzenarioMeanTimes
            // 
            this.button_Others_ExportSzenarioMeanTimes.Location = new System.Drawing.Point(170, 205);
            this.button_Others_ExportSzenarioMeanTimes.Name = "button_Others_ExportSzenarioMeanTimes";
            this.button_Others_ExportSzenarioMeanTimes.Size = new System.Drawing.Size(150, 23);
            this.button_Others_ExportSzenarioMeanTimes.TabIndex = 42;
            this.button_Others_ExportSzenarioMeanTimes.Text = "Export mean times";
            this.button_Others_ExportSzenarioMeanTimes.UseVisualStyleBackColor = true;
            this.button_Others_ExportSzenarioMeanTimes.Click += new System.EventHandler(this.button_Others_ExportSzenarioMeanTimes_Click);
            // 
            // label_Others_Turn
            // 
            this.label_Others_Turn.AutoSize = true;
            this.label_Others_Turn.Location = new System.Drawing.Point(31, 123);
            this.label_Others_Turn.Name = "label_Others_Turn";
            this.label_Others_Turn.Size = new System.Drawing.Size(32, 13);
            this.label_Others_Turn.TabIndex = 41;
            this.label_Others_Turn.Text = "Turn:";
            // 
            // comboBox_Others_Turn
            // 
            this.comboBox_Others_Turn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Others_Turn.FormattingEnabled = true;
            this.comboBox_Others_Turn.Location = new System.Drawing.Point(66, 120);
            this.comboBox_Others_Turn.Name = "comboBox_Others_Turn";
            this.comboBox_Others_Turn.Size = new System.Drawing.Size(247, 21);
            this.comboBox_Others_Turn.Sorted = true;
            this.comboBox_Others_Turn.TabIndex = 40;
            // 
            // label_Others_Group
            // 
            this.label_Others_Group.AutoSize = true;
            this.label_Others_Group.Location = new System.Drawing.Point(24, 42);
            this.label_Others_Group.Name = "label_Others_Group";
            this.label_Others_Group.Size = new System.Drawing.Size(39, 13);
            this.label_Others_Group.TabIndex = 39;
            this.label_Others_Group.Text = "Group:";
            // 
            // comboBox_Others_Group
            // 
            this.comboBox_Others_Group.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Others_Group.FormattingEnabled = true;
            this.comboBox_Others_Group.Location = new System.Drawing.Point(66, 39);
            this.comboBox_Others_Group.Name = "comboBox_Others_Group";
            this.comboBox_Others_Group.Size = new System.Drawing.Size(247, 21);
            this.comboBox_Others_Group.Sorted = true;
            this.comboBox_Others_Group.TabIndex = 38;
            this.comboBox_Others_Group.SelectedIndexChanged += new System.EventHandler(this.comboBox_Others_Group_SelectedIndexChanged);
            // 
            // label_Others_Szenario
            // 
            this.label_Others_Szenario.AutoSize = true;
            this.label_Others_Szenario.Location = new System.Drawing.Point(12, 69);
            this.label_Others_Szenario.Name = "label_Others_Szenario";
            this.label_Others_Szenario.Size = new System.Drawing.Size(51, 13);
            this.label_Others_Szenario.TabIndex = 37;
            this.label_Others_Szenario.Text = "Szenario:";
            // 
            // label_Others_Subject
            // 
            this.label_Others_Subject.AutoSize = true;
            this.label_Others_Subject.Location = new System.Drawing.Point(17, 96);
            this.label_Others_Subject.Name = "label_Others_Subject";
            this.label_Others_Subject.Size = new System.Drawing.Size(46, 13);
            this.label_Others_Subject.TabIndex = 36;
            this.label_Others_Subject.Text = "Subject:";
            // 
            // label_Others_Study
            // 
            this.label_Others_Study.AutoSize = true;
            this.label_Others_Study.Location = new System.Drawing.Point(26, 15);
            this.label_Others_Study.Name = "label_Others_Study";
            this.label_Others_Study.Size = new System.Drawing.Size(37, 13);
            this.label_Others_Study.TabIndex = 35;
            this.label_Others_Study.Text = "Study:";
            // 
            // comboBox_Others_Subject
            // 
            this.comboBox_Others_Subject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Others_Subject.FormattingEnabled = true;
            this.comboBox_Others_Subject.Location = new System.Drawing.Point(66, 93);
            this.comboBox_Others_Subject.Name = "comboBox_Others_Subject";
            this.comboBox_Others_Subject.Size = new System.Drawing.Size(247, 21);
            this.comboBox_Others_Subject.Sorted = true;
            this.comboBox_Others_Subject.TabIndex = 34;
            this.comboBox_Others_Subject.SelectedIndexChanged += new System.EventHandler(this.comboBox_Others_Subject_SelectedIndexChanged);
            // 
            // comboBox_Others_Study
            // 
            this.comboBox_Others_Study.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Others_Study.FormattingEnabled = true;
            this.comboBox_Others_Study.Location = new System.Drawing.Point(66, 12);
            this.comboBox_Others_Study.Name = "comboBox_Others_Study";
            this.comboBox_Others_Study.Size = new System.Drawing.Size(247, 21);
            this.comboBox_Others_Study.Sorted = true;
            this.comboBox_Others_Study.TabIndex = 33;
            this.comboBox_Others_Study.SelectedIndexChanged += new System.EventHandler(this.comboBox_Others_Study_SelectedIndexChanged);
            // 
            // comboBox_Others_Szenario
            // 
            this.comboBox_Others_Szenario.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Others_Szenario.FormattingEnabled = true;
            this.comboBox_Others_Szenario.Location = new System.Drawing.Point(66, 66);
            this.comboBox_Others_Szenario.Name = "comboBox_Others_Szenario";
            this.comboBox_Others_Szenario.Size = new System.Drawing.Size(247, 21);
            this.comboBox_Others_Szenario.Sorted = true;
            this.comboBox_Others_Szenario.TabIndex = 32;
            this.comboBox_Others_Szenario.SelectedIndexChanged += new System.EventHandler(this.comboBox_Others_Szenario_SelectedIndexChanged);
            // 
            // button_Others_PlotTrajectoryBaseline
            // 
            this.button_Others_PlotTrajectoryBaseline.Location = new System.Drawing.Point(15, 147);
            this.button_Others_PlotTrajectoryBaseline.Name = "button_Others_PlotTrajectoryBaseline";
            this.button_Others_PlotTrajectoryBaseline.Size = new System.Drawing.Size(150, 23);
            this.button_Others_PlotTrajectoryBaseline.TabIndex = 31;
            this.button_Others_PlotTrajectoryBaseline.Text = "Plot trajectory baselines";
            this.button_Others_PlotTrajectoryBaseline.UseVisualStyleBackColor = true;
            this.button_Others_PlotTrajectoryBaseline.Click += new System.EventHandler(this.button_PlotBaseline_Click);
            // 
            // button_Others_PlotSzenarioMeanTimes
            // 
            this.button_Others_PlotSzenarioMeanTimes.Location = new System.Drawing.Point(15, 205);
            this.button_Others_PlotSzenarioMeanTimes.Name = "button_Others_PlotSzenarioMeanTimes";
            this.button_Others_PlotSzenarioMeanTimes.Size = new System.Drawing.Size(150, 23);
            this.button_Others_PlotSzenarioMeanTimes.TabIndex = 22;
            this.button_Others_PlotSzenarioMeanTimes.Text = "Plot mean times";
            this.button_Others_PlotSzenarioMeanTimes.UseVisualStyleBackColor = true;
            this.button_Others_PlotSzenarioMeanTimes.Click += new System.EventHandler(this.button_Others_PlotSzenarioMeanTimes_Click);
            // 
            // tabPage_ImportCalculations
            // 
            this.tabPage_ImportCalculations.Controls.Add(this.groupBox_Import_VelocityCropping);
            this.tabPage_ImportCalculations.Controls.Add(this.button_Import_ClearMeasureFileList);
            this.tabPage_ImportCalculations.Controls.Add(this.groupBox_Import_CalculationsImport);
            this.tabPage_ImportCalculations.Controls.Add(this.groupBox_Import_TimeNormalization);
            this.tabPage_ImportCalculations.Controls.Add(this.groupBox_Import_ButterworthFilter);
            this.tabPage_ImportCalculations.Controls.Add(this.button_Import_SelectMeasureFileFolder);
            this.tabPage_ImportCalculations.Controls.Add(this.listBox_Import_SelectedMeasureFiles);
            this.tabPage_ImportCalculations.Controls.Add(this.button_Import_SelectMeasureFiles);
            this.tabPage_ImportCalculations.Location = new System.Drawing.Point(4, 22);
            this.tabPage_ImportCalculations.Name = "tabPage_ImportCalculations";
            this.tabPage_ImportCalculations.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_ImportCalculations.Size = new System.Drawing.Size(733, 637);
            this.tabPage_ImportCalculations.TabIndex = 0;
            this.tabPage_ImportCalculations.Text = "Import / Calculations";
            this.tabPage_ImportCalculations.UseVisualStyleBackColor = true;
            // 
            // groupBox_Import_VelocityCropping
            // 
            this.groupBox_Import_VelocityCropping.Controls.Add(this.label_Import_PercentPeakVelocity);
            this.groupBox_Import_VelocityCropping.Controls.Add(this.textBox_Import_PercentPeakVelocity);
            this.groupBox_Import_VelocityCropping.Location = new System.Drawing.Point(566, 317);
            this.groupBox_Import_VelocityCropping.Name = "groupBox_Import_VelocityCropping";
            this.groupBox_Import_VelocityCropping.Size = new System.Drawing.Size(161, 51);
            this.groupBox_Import_VelocityCropping.TabIndex = 35;
            this.groupBox_Import_VelocityCropping.TabStop = false;
            this.groupBox_Import_VelocityCropping.Text = "Velocity cropping";
            // 
            // label_Import_PercentPeakVelocity
            // 
            this.label_Import_PercentPeakVelocity.AutoSize = true;
            this.label_Import_PercentPeakVelocity.Location = new System.Drawing.Point(32, 22);
            this.label_Import_PercentPeakVelocity.Name = "label_Import_PercentPeakVelocity";
            this.label_Import_PercentPeakVelocity.Size = new System.Drawing.Size(81, 13);
            this.label_Import_PercentPeakVelocity.TabIndex = 11;
            this.label_Import_PercentPeakVelocity.Text = "% peak velocity";
            // 
            // textBox_Import_PercentPeakVelocity
            // 
            this.textBox_Import_PercentPeakVelocity.Location = new System.Drawing.Point(9, 19);
            this.textBox_Import_PercentPeakVelocity.Name = "textBox_Import_PercentPeakVelocity";
            this.textBox_Import_PercentPeakVelocity.Size = new System.Drawing.Size(22, 20);
            this.textBox_Import_PercentPeakVelocity.TabIndex = 10;
            this.textBox_Import_PercentPeakVelocity.Text = "10";
            this.textBox_Import_PercentPeakVelocity.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // button_Import_ClearMeasureFileList
            // 
            this.button_Import_ClearMeasureFileList.Location = new System.Drawing.Point(566, 67);
            this.button_Import_ClearMeasureFileList.Name = "button_Import_ClearMeasureFileList";
            this.button_Import_ClearMeasureFileList.Size = new System.Drawing.Size(161, 23);
            this.button_Import_ClearMeasureFileList.TabIndex = 37;
            this.button_Import_ClearMeasureFileList.Text = "Clear measure-file-list";
            this.button_Import_ClearMeasureFileList.UseVisualStyleBackColor = true;
            this.button_Import_ClearMeasureFileList.Click += new System.EventHandler(this.button_Import_ClearMeasureFileList_Click);
            // 
            // groupBox_Import_CalculationsImport
            // 
            this.groupBox_Import_CalculationsImport.Controls.Add(this.button_Import_AutoImport);
            this.groupBox_Import_CalculationsImport.Controls.Add(this.button_Import_FixBrokenTrials);
            this.groupBox_Import_CalculationsImport.Controls.Add(this.button_Import_ImportMeasureFiles);
            this.groupBox_Import_CalculationsImport.Controls.Add(this.button_Import_CalculateStatistics);
            this.groupBox_Import_CalculationsImport.Location = new System.Drawing.Point(566, 431);
            this.groupBox_Import_CalculationsImport.Name = "groupBox_Import_CalculationsImport";
            this.groupBox_Import_CalculationsImport.Size = new System.Drawing.Size(161, 154);
            this.groupBox_Import_CalculationsImport.TabIndex = 36;
            this.groupBox_Import_CalculationsImport.TabStop = false;
            this.groupBox_Import_CalculationsImport.Text = "Calculations / Import";
            // 
            // button_Import_AutoImport
            // 
            this.button_Import_AutoImport.Location = new System.Drawing.Point(6, 19);
            this.button_Import_AutoImport.Name = "button_Import_AutoImport";
            this.button_Import_AutoImport.Size = new System.Drawing.Size(149, 41);
            this.button_Import_AutoImport.TabIndex = 36;
            this.button_Import_AutoImport.Text = "Automatic import and calculation";
            this.button_Import_AutoImport.UseVisualStyleBackColor = true;
            this.button_Import_AutoImport.Click += new System.EventHandler(this.button_Auto_Click);
            // 
            // button_Import_FixBrokenTrials
            // 
            this.button_Import_FixBrokenTrials.Location = new System.Drawing.Point(6, 124);
            this.button_Import_FixBrokenTrials.Name = "button_Import_FixBrokenTrials";
            this.button_Import_FixBrokenTrials.Size = new System.Drawing.Size(149, 23);
            this.button_Import_FixBrokenTrials.TabIndex = 19;
            this.button_Import_FixBrokenTrials.Text = "Fix broken Trials";
            this.button_Import_FixBrokenTrials.UseVisualStyleBackColor = true;
            this.button_Import_FixBrokenTrials.Click += new System.EventHandler(this.button_FixBrokenTrials_Click);
            // 
            // button_Import_ImportMeasureFiles
            // 
            this.button_Import_ImportMeasureFiles.Location = new System.Drawing.Point(6, 66);
            this.button_Import_ImportMeasureFiles.Name = "button_Import_ImportMeasureFiles";
            this.button_Import_ImportMeasureFiles.Size = new System.Drawing.Size(149, 23);
            this.button_Import_ImportMeasureFiles.TabIndex = 35;
            this.button_Import_ImportMeasureFiles.Text = "Import measure-files";
            this.button_Import_ImportMeasureFiles.UseVisualStyleBackColor = true;
            this.button_Import_ImportMeasureFiles.Click += new System.EventHandler(this.button_ImportMeasureFiles_Click);
            // 
            // button_Import_CalculateStatistics
            // 
            this.button_Import_CalculateStatistics.Location = new System.Drawing.Point(6, 95);
            this.button_Import_CalculateStatistics.Name = "button_Import_CalculateStatistics";
            this.button_Import_CalculateStatistics.Size = new System.Drawing.Size(149, 23);
            this.button_Import_CalculateStatistics.TabIndex = 0;
            this.button_Import_CalculateStatistics.Text = "Calculate statistics";
            this.button_Import_CalculateStatistics.UseVisualStyleBackColor = true;
            this.button_Import_CalculateStatistics.Click += new System.EventHandler(this.button_CalculateStatistics_Click);
            // 
            // groupBox_Import_TimeNormalization
            // 
            this.groupBox_Import_TimeNormalization.Controls.Add(this.label_Import_NewSampleCountText);
            this.groupBox_Import_TimeNormalization.Controls.Add(this.textBox_Import_NewSampleCount);
            this.groupBox_Import_TimeNormalization.Location = new System.Drawing.Point(566, 374);
            this.groupBox_Import_TimeNormalization.Name = "groupBox_Import_TimeNormalization";
            this.groupBox_Import_TimeNormalization.Size = new System.Drawing.Size(161, 51);
            this.groupBox_Import_TimeNormalization.TabIndex = 34;
            this.groupBox_Import_TimeNormalization.TabStop = false;
            this.groupBox_Import_TimeNormalization.Text = "Time normalization";
            // 
            // label_Import_NewSampleCountText
            // 
            this.label_Import_NewSampleCountText.AutoSize = true;
            this.label_Import_NewSampleCountText.Location = new System.Drawing.Point(43, 22);
            this.label_Import_NewSampleCountText.Name = "label_Import_NewSampleCountText";
            this.label_Import_NewSampleCountText.Size = new System.Drawing.Size(93, 13);
            this.label_Import_NewSampleCountText.TabIndex = 11;
            this.label_Import_NewSampleCountText.Text = "Samples / second";
            // 
            // textBox_Import_NewSampleCount
            // 
            this.textBox_Import_NewSampleCount.Location = new System.Drawing.Point(9, 19);
            this.textBox_Import_NewSampleCount.Name = "textBox_Import_NewSampleCount";
            this.textBox_Import_NewSampleCount.Size = new System.Drawing.Size(34, 20);
            this.textBox_Import_NewSampleCount.TabIndex = 10;
            this.textBox_Import_NewSampleCount.Text = "101";
            // 
            // groupBox_Import_ButterworthFilter
            // 
            this.groupBox_Import_ButterworthFilter.Controls.Add(this.label_Import_CutoffFreqForceForce);
            this.groupBox_Import_ButterworthFilter.Controls.Add(this.label_Import_CutoffFreqPositionPosition);
            this.groupBox_Import_ButterworthFilter.Controls.Add(this.textBox_Import_CutoffFreqForce);
            this.groupBox_Import_ButterworthFilter.Controls.Add(this.label_Import_CutoffFreqForce);
            this.groupBox_Import_ButterworthFilter.Controls.Add(this.label_Import_SamplesPerSec);
            this.groupBox_Import_ButterworthFilter.Controls.Add(this.textBox_Import_CutoffFreqPosition);
            this.groupBox_Import_ButterworthFilter.Controls.Add(this.label_Import_CutoffFreqPosition);
            this.groupBox_Import_ButterworthFilter.Controls.Add(this.textBox_Import_SamplesPerSec);
            this.groupBox_Import_ButterworthFilter.Controls.Add(this.textBox_Import_FilterOrder);
            this.groupBox_Import_ButterworthFilter.Controls.Add(this.label_Import_FilterOrder);
            this.groupBox_Import_ButterworthFilter.Location = new System.Drawing.Point(566, 183);
            this.groupBox_Import_ButterworthFilter.Name = "groupBox_Import_ButterworthFilter";
            this.groupBox_Import_ButterworthFilter.Size = new System.Drawing.Size(161, 128);
            this.groupBox_Import_ButterworthFilter.TabIndex = 33;
            this.groupBox_Import_ButterworthFilter.TabStop = false;
            this.groupBox_Import_ButterworthFilter.Text = "Butterworth filter settings";
            // 
            // label_Import_CutoffFreqForceForce
            // 
            this.label_Import_CutoffFreqForceForce.AutoSize = true;
            this.label_Import_CutoffFreqForceForce.Location = new System.Drawing.Point(113, 103);
            this.label_Import_CutoffFreqForceForce.Name = "label_Import_CutoffFreqForceForce";
            this.label_Import_CutoffFreqForceForce.Size = new System.Drawing.Size(34, 13);
            this.label_Import_CutoffFreqForceForce.TabIndex = 10;
            this.label_Import_CutoffFreqForceForce.Text = "Force";
            // 
            // label_Import_CutoffFreqPositionPosition
            // 
            this.label_Import_CutoffFreqPositionPosition.AutoSize = true;
            this.label_Import_CutoffFreqPositionPosition.Location = new System.Drawing.Point(113, 77);
            this.label_Import_CutoffFreqPositionPosition.Name = "label_Import_CutoffFreqPositionPosition";
            this.label_Import_CutoffFreqPositionPosition.Size = new System.Drawing.Size(44, 13);
            this.label_Import_CutoffFreqPositionPosition.TabIndex = 9;
            this.label_Import_CutoffFreqPositionPosition.Text = "Position";
            // 
            // textBox_Import_CutoffFreqForce
            // 
            this.textBox_Import_CutoffFreqForce.Location = new System.Drawing.Point(71, 100);
            this.textBox_Import_CutoffFreqForce.Name = "textBox_Import_CutoffFreqForce";
            this.textBox_Import_CutoffFreqForce.Size = new System.Drawing.Size(36, 20);
            this.textBox_Import_CutoffFreqForce.TabIndex = 8;
            this.textBox_Import_CutoffFreqForce.Text = "10";
            // 
            // label_Import_CutoffFreqForce
            // 
            this.label_Import_CutoffFreqForce.AutoSize = true;
            this.label_Import_CutoffFreqForce.Location = new System.Drawing.Point(6, 103);
            this.label_Import_CutoffFreqForce.Name = "label_Import_CutoffFreqForce";
            this.label_Import_CutoffFreqForce.Size = new System.Drawing.Size(62, 13);
            this.label_Import_CutoffFreqForce.TabIndex = 7;
            this.label_Import_CutoffFreqForce.Text = "Cutoff-Freq:";
            // 
            // label_Import_SamplesPerSec
            // 
            this.label_Import_SamplesPerSec.AutoSize = true;
            this.label_Import_SamplesPerSec.Location = new System.Drawing.Point(5, 25);
            this.label_Import_SamplesPerSec.Name = "label_Import_SamplesPerSec";
            this.label_Import_SamplesPerSec.Size = new System.Drawing.Size(60, 13);
            this.label_Import_SamplesPerSec.TabIndex = 1;
            this.label_Import_SamplesPerSec.Text = "Samples/s:";
            // 
            // textBox_Import_CutoffFreqPosition
            // 
            this.textBox_Import_CutoffFreqPosition.Location = new System.Drawing.Point(71, 74);
            this.textBox_Import_CutoffFreqPosition.Name = "textBox_Import_CutoffFreqPosition";
            this.textBox_Import_CutoffFreqPosition.Size = new System.Drawing.Size(36, 20);
            this.textBox_Import_CutoffFreqPosition.TabIndex = 6;
            this.textBox_Import_CutoffFreqPosition.Text = "6";
            // 
            // label_Import_CutoffFreqPosition
            // 
            this.label_Import_CutoffFreqPosition.AutoSize = true;
            this.label_Import_CutoffFreqPosition.Location = new System.Drawing.Point(6, 77);
            this.label_Import_CutoffFreqPosition.Name = "label_Import_CutoffFreqPosition";
            this.label_Import_CutoffFreqPosition.Size = new System.Drawing.Size(62, 13);
            this.label_Import_CutoffFreqPosition.TabIndex = 5;
            this.label_Import_CutoffFreqPosition.Text = "Cutoff-Freq:";
            // 
            // textBox_Import_SamplesPerSec
            // 
            this.textBox_Import_SamplesPerSec.Location = new System.Drawing.Point(71, 22);
            this.textBox_Import_SamplesPerSec.Name = "textBox_Import_SamplesPerSec";
            this.textBox_Import_SamplesPerSec.Size = new System.Drawing.Size(36, 20);
            this.textBox_Import_SamplesPerSec.TabIndex = 2;
            this.textBox_Import_SamplesPerSec.Text = "1000";
            // 
            // textBox_Import_FilterOrder
            // 
            this.textBox_Import_FilterOrder.Location = new System.Drawing.Point(71, 48);
            this.textBox_Import_FilterOrder.Name = "textBox_Import_FilterOrder";
            this.textBox_Import_FilterOrder.Size = new System.Drawing.Size(36, 20);
            this.textBox_Import_FilterOrder.TabIndex = 4;
            this.textBox_Import_FilterOrder.Text = "2";
            // 
            // label_Import_FilterOrder
            // 
            this.label_Import_FilterOrder.AutoSize = true;
            this.label_Import_FilterOrder.Location = new System.Drawing.Point(6, 51);
            this.label_Import_FilterOrder.Name = "label_Import_FilterOrder";
            this.label_Import_FilterOrder.Size = new System.Drawing.Size(59, 13);
            this.label_Import_FilterOrder.TabIndex = 3;
            this.label_Import_FilterOrder.Text = "Filter order:";
            // 
            // button_Import_SelectMeasureFileFolder
            // 
            this.button_Import_SelectMeasureFileFolder.Location = new System.Drawing.Point(566, 38);
            this.button_Import_SelectMeasureFileFolder.Name = "button_Import_SelectMeasureFileFolder";
            this.button_Import_SelectMeasureFileFolder.Size = new System.Drawing.Size(161, 23);
            this.button_Import_SelectMeasureFileFolder.TabIndex = 32;
            this.button_Import_SelectMeasureFileFolder.Text = "Select measure-file-folder";
            this.button_Import_SelectMeasureFileFolder.UseVisualStyleBackColor = true;
            this.button_Import_SelectMeasureFileFolder.Click += new System.EventHandler(this.button_OpenMeasureFilesFolder_Click);
            // 
            // listBox_Import_SelectedMeasureFiles
            // 
            this.listBox_Import_SelectedMeasureFiles.FormattingEnabled = true;
            this.listBox_Import_SelectedMeasureFiles.HorizontalScrollbar = true;
            this.listBox_Import_SelectedMeasureFiles.Location = new System.Drawing.Point(6, 9);
            this.listBox_Import_SelectedMeasureFiles.Name = "listBox_Import_SelectedMeasureFiles";
            this.listBox_Import_SelectedMeasureFiles.Size = new System.Drawing.Size(554, 576);
            this.listBox_Import_SelectedMeasureFiles.TabIndex = 1;
            // 
            // button_Import_SelectMeasureFiles
            // 
            this.button_Import_SelectMeasureFiles.Location = new System.Drawing.Point(566, 9);
            this.button_Import_SelectMeasureFiles.Name = "button_Import_SelectMeasureFiles";
            this.button_Import_SelectMeasureFiles.Size = new System.Drawing.Size(161, 23);
            this.button_Import_SelectMeasureFiles.TabIndex = 0;
            this.button_Import_SelectMeasureFiles.Text = "Select measure-file(s)";
            this.button_Import_SelectMeasureFiles.UseVisualStyleBackColor = true;
            this.button_Import_SelectMeasureFiles.Click += new System.EventHandler(this.button_OpenMeasureFiles_Click);
            // 
            // tabPage_Debug
            // 
            this.tabPage_Debug.Controls.Add(this.tabControl_Debug);
            this.tabPage_Debug.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Debug.Name = "tabPage_Debug";
            this.tabPage_Debug.Size = new System.Drawing.Size(733, 637);
            this.tabPage_Debug.TabIndex = 5;
            this.tabPage_Debug.Text = "Debug";
            this.tabPage_Debug.UseVisualStyleBackColor = true;
            // 
            // tabControl_Debug
            // 
            this.tabControl_Debug.Controls.Add(this.tabPage_Debug_MatlabAndLogs);
            this.tabControl_Debug.Controls.Add(this.tabPage_Debug_DatabaseManipulation);
            this.tabControl_Debug.Controls.Add(this.tabPage_Debug_BaselineRecalculation);
            this.tabControl_Debug.Location = new System.Drawing.Point(3, 3);
            this.tabControl_Debug.Name = "tabControl_Debug";
            this.tabControl_Debug.SelectedIndex = 0;
            this.tabControl_Debug.Size = new System.Drawing.Size(727, 592);
            this.tabControl_Debug.TabIndex = 61;
            // 
            // tabPage_Debug_MatlabAndLogs
            // 
            this.tabPage_Debug_MatlabAndLogs.Controls.Add(this.button_Debug_ShowMatlabFiles);
            this.tabPage_Debug_MatlabAndLogs.Controls.Add(this.button_Debug_ShowMatlabWindow);
            this.tabPage_Debug_MatlabAndLogs.Controls.Add(this.button_Debug_ShowMatlabWorkspace);
            this.tabPage_Debug_MatlabAndLogs.Controls.Add(this.button_Debug_showFaultyTrials);
            this.tabPage_Debug_MatlabAndLogs.Controls.Add(this.button_Debug_SaveLogToFile);
            this.tabPage_Debug_MatlabAndLogs.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Debug_MatlabAndLogs.Name = "tabPage_Debug_MatlabAndLogs";
            this.tabPage_Debug_MatlabAndLogs.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Debug_MatlabAndLogs.Size = new System.Drawing.Size(719, 566);
            this.tabPage_Debug_MatlabAndLogs.TabIndex = 0;
            this.tabPage_Debug_MatlabAndLogs.Text = "Matlab & Logs";
            this.tabPage_Debug_MatlabAndLogs.UseVisualStyleBackColor = true;
            // 
            // button_Debug_ShowMatlabFiles
            // 
            this.button_Debug_ShowMatlabFiles.Location = new System.Drawing.Point(6, 70);
            this.button_Debug_ShowMatlabFiles.Name = "button_Debug_ShowMatlabFiles";
            this.button_Debug_ShowMatlabFiles.Size = new System.Drawing.Size(178, 23);
            this.button_Debug_ShowMatlabFiles.TabIndex = 9;
            this.button_Debug_ShowMatlabFiles.Text = "Show MATLAB files";
            this.button_Debug_ShowMatlabFiles.UseVisualStyleBackColor = true;
            this.button_Debug_ShowMatlabFiles.Click += new System.EventHandler(this.button_Debug_ShowMatlabFiles_Click);
            // 
            // button_Debug_ShowMatlabWindow
            // 
            this.button_Debug_ShowMatlabWindow.Location = new System.Drawing.Point(6, 12);
            this.button_Debug_ShowMatlabWindow.Name = "button_Debug_ShowMatlabWindow";
            this.button_Debug_ShowMatlabWindow.Size = new System.Drawing.Size(178, 23);
            this.button_Debug_ShowMatlabWindow.TabIndex = 2;
            this.button_Debug_ShowMatlabWindow.Text = "Show MATLAB window";
            this.button_Debug_ShowMatlabWindow.UseVisualStyleBackColor = true;
            this.button_Debug_ShowMatlabWindow.Click += new System.EventHandler(this.button_ShowMatlabWindow_Click);
            // 
            // button_Debug_ShowMatlabWorkspace
            // 
            this.button_Debug_ShowMatlabWorkspace.Location = new System.Drawing.Point(6, 41);
            this.button_Debug_ShowMatlabWorkspace.Name = "button_Debug_ShowMatlabWorkspace";
            this.button_Debug_ShowMatlabWorkspace.Size = new System.Drawing.Size(178, 23);
            this.button_Debug_ShowMatlabWorkspace.TabIndex = 4;
            this.button_Debug_ShowMatlabWorkspace.Text = "Show MATLAB workspace";
            this.button_Debug_ShowMatlabWorkspace.UseVisualStyleBackColor = true;
            this.button_Debug_ShowMatlabWorkspace.Click += new System.EventHandler(this.button_ShowMatlabWorkspace_Click);
            // 
            // button_Debug_showFaultyTrials
            // 
            this.button_Debug_showFaultyTrials.Location = new System.Drawing.Point(6, 149);
            this.button_Debug_showFaultyTrials.Name = "button_Debug_showFaultyTrials";
            this.button_Debug_showFaultyTrials.Size = new System.Drawing.Size(178, 23);
            this.button_Debug_showFaultyTrials.TabIndex = 7;
            this.button_Debug_showFaultyTrials.Text = "Show faulty Trials";
            this.button_Debug_showFaultyTrials.UseVisualStyleBackColor = true;
            this.button_Debug_showFaultyTrials.Click += new System.EventHandler(this.button_showFaultyTrials_Click);
            // 
            // button_Debug_SaveLogToFile
            // 
            this.button_Debug_SaveLogToFile.Location = new System.Drawing.Point(6, 178);
            this.button_Debug_SaveLogToFile.Name = "button_Debug_SaveLogToFile";
            this.button_Debug_SaveLogToFile.Size = new System.Drawing.Size(178, 23);
            this.button_Debug_SaveLogToFile.TabIndex = 8;
            this.button_Debug_SaveLogToFile.Text = "Save log to file";
            this.button_Debug_SaveLogToFile.UseVisualStyleBackColor = true;
            this.button_Debug_SaveLogToFile.Click += new System.EventHandler(this.button_Debug_SaveLogToFile_Click);
            // 
            // tabPage_Debug_DatabaseManipulation
            // 
            this.tabPage_Debug_DatabaseManipulation.Controls.Add(this.button_DataManipulation_DropStatistics);
            this.tabPage_Debug_DatabaseManipulation.Controls.Add(this.button_DataManipulation_CompactDatabase);
            this.tabPage_Debug_DatabaseManipulation.Controls.Add(this.button_DataManipulation_DropIndexes);
            this.tabPage_Debug_DatabaseManipulation.Controls.Add(this.button_DataManipulation_RebuildIndexes);
            this.tabPage_Debug_DatabaseManipulation.Controls.Add(this.button_DataManipulation_EnsureIndexes);
            this.tabPage_Debug_DatabaseManipulation.Controls.Add(this.button_Debug_DropDatabase);
            this.tabPage_Debug_DatabaseManipulation.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Debug_DatabaseManipulation.Name = "tabPage_Debug_DatabaseManipulation";
            this.tabPage_Debug_DatabaseManipulation.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Debug_DatabaseManipulation.Size = new System.Drawing.Size(719, 566);
            this.tabPage_Debug_DatabaseManipulation.TabIndex = 1;
            this.tabPage_Debug_DatabaseManipulation.Text = "Database Manipulation";
            this.tabPage_Debug_DatabaseManipulation.UseVisualStyleBackColor = true;
            // 
            // button_DataManipulation_DropStatistics
            // 
            this.button_DataManipulation_DropStatistics.Location = new System.Drawing.Point(17, 129);
            this.button_DataManipulation_DropStatistics.Name = "button_DataManipulation_DropStatistics";
            this.button_DataManipulation_DropStatistics.Size = new System.Drawing.Size(129, 23);
            this.button_DataManipulation_DropStatistics.TabIndex = 66;
            this.button_DataManipulation_DropStatistics.Text = "Drop Statistics";
            this.button_DataManipulation_DropStatistics.UseVisualStyleBackColor = true;
            this.button_DataManipulation_DropStatistics.Click += new System.EventHandler(this.button_DataManipulation_DropStatistics_Click);
            // 
            // button_DataManipulation_CompactDatabase
            // 
            this.button_DataManipulation_CompactDatabase.Location = new System.Drawing.Point(17, 196);
            this.button_DataManipulation_CompactDatabase.Name = "button_DataManipulation_CompactDatabase";
            this.button_DataManipulation_CompactDatabase.Size = new System.Drawing.Size(129, 23);
            this.button_DataManipulation_CompactDatabase.TabIndex = 65;
            this.button_DataManipulation_CompactDatabase.Text = "Compact database";
            this.button_DataManipulation_CompactDatabase.UseVisualStyleBackColor = true;
            this.button_DataManipulation_CompactDatabase.Click += new System.EventHandler(this.button_DataManipulation_CompactDatabase_Click);
            // 
            // button_DataManipulation_DropIndexes
            // 
            this.button_DataManipulation_DropIndexes.Location = new System.Drawing.Point(17, 75);
            this.button_DataManipulation_DropIndexes.Name = "button_DataManipulation_DropIndexes";
            this.button_DataManipulation_DropIndexes.Size = new System.Drawing.Size(129, 23);
            this.button_DataManipulation_DropIndexes.TabIndex = 64;
            this.button_DataManipulation_DropIndexes.Text = "Drop indexes";
            this.button_DataManipulation_DropIndexes.UseVisualStyleBackColor = true;
            this.button_DataManipulation_DropIndexes.Click += new System.EventHandler(this.button_DataManipulation_DropIndexes_Click);
            // 
            // button_DataManipulation_RebuildIndexes
            // 
            this.button_DataManipulation_RebuildIndexes.Location = new System.Drawing.Point(17, 46);
            this.button_DataManipulation_RebuildIndexes.Name = "button_DataManipulation_RebuildIndexes";
            this.button_DataManipulation_RebuildIndexes.Size = new System.Drawing.Size(129, 23);
            this.button_DataManipulation_RebuildIndexes.TabIndex = 63;
            this.button_DataManipulation_RebuildIndexes.Text = "Rebuild indexes";
            this.button_DataManipulation_RebuildIndexes.UseVisualStyleBackColor = true;
            this.button_DataManipulation_RebuildIndexes.Click += new System.EventHandler(this.button_DataManipulation_RebuildIndexes_Click);
            // 
            // button_DataManipulation_EnsureIndexes
            // 
            this.button_DataManipulation_EnsureIndexes.Location = new System.Drawing.Point(17, 17);
            this.button_DataManipulation_EnsureIndexes.Name = "button_DataManipulation_EnsureIndexes";
            this.button_DataManipulation_EnsureIndexes.Size = new System.Drawing.Size(129, 23);
            this.button_DataManipulation_EnsureIndexes.TabIndex = 62;
            this.button_DataManipulation_EnsureIndexes.Text = "Ensure indexes";
            this.button_DataManipulation_EnsureIndexes.UseVisualStyleBackColor = true;
            this.button_DataManipulation_EnsureIndexes.Click += new System.EventHandler(this.button_DataManipulation_EnsureIndexes_Click);
            // 
            // button_Debug_DropDatabase
            // 
            this.button_Debug_DropDatabase.Location = new System.Drawing.Point(17, 225);
            this.button_Debug_DropDatabase.Name = "button_Debug_DropDatabase";
            this.button_Debug_DropDatabase.Size = new System.Drawing.Size(129, 23);
            this.button_Debug_DropDatabase.TabIndex = 61;
            this.button_Debug_DropDatabase.Text = "Drop database";
            this.button_Debug_DropDatabase.UseVisualStyleBackColor = true;
            this.button_Debug_DropDatabase.Click += new System.EventHandler(this.button_Debug_InitialiseDatabase_Click);
            // 
            // tabPage_Debug_BaselineRecalculation
            // 
            this.tabPage_Debug_BaselineRecalculation.Controls.Add(this.button_BaselineRecalculation_RecalculateBaseline);
            this.tabPage_Debug_BaselineRecalculation.Controls.Add(this.label_BaselineRecalculation_Targets);
            this.tabPage_Debug_BaselineRecalculation.Controls.Add(this.listBox_BaselineRecalculation_Targets);
            this.tabPage_Debug_BaselineRecalculation.Controls.Add(this.button_BaselineRecalculation_AddAll);
            this.tabPage_Debug_BaselineRecalculation.Controls.Add(this.button_BaselineRecalculation_ClearAll);
            this.tabPage_Debug_BaselineRecalculation.Controls.Add(this.button_BaselineRecalculation_ClearSelected);
            this.tabPage_Debug_BaselineRecalculation.Controls.Add(this.listBox_BaselineRecalculation_SelectedTrials);
            this.tabPage_Debug_BaselineRecalculation.Controls.Add(this.button_BaselineRecalculation_AddSelected);
            this.tabPage_Debug_BaselineRecalculation.Controls.Add(this.label_BaselineRecalculation_Trials);
            this.tabPage_Debug_BaselineRecalculation.Controls.Add(this.listBox_BaselineRecalculation_Trials);
            this.tabPage_Debug_BaselineRecalculation.Controls.Add(this.label_BaselineRecalculation_Group);
            this.tabPage_Debug_BaselineRecalculation.Controls.Add(this.comboBox_BaselineRecalculation_Group);
            this.tabPage_Debug_BaselineRecalculation.Controls.Add(this.label_BaselineRecalculation_Szenario);
            this.tabPage_Debug_BaselineRecalculation.Controls.Add(this.label_BaselineRecalculation_Subject);
            this.tabPage_Debug_BaselineRecalculation.Controls.Add(this.label_BaselineRecalculation_Study);
            this.tabPage_Debug_BaselineRecalculation.Controls.Add(this.comboBox_BaselineRecalculation_Subject);
            this.tabPage_Debug_BaselineRecalculation.Controls.Add(this.comboBox_BaselineRecalculation_Study);
            this.tabPage_Debug_BaselineRecalculation.Controls.Add(this.comboBox_BaselineRecalculation_Szenario);
            this.tabPage_Debug_BaselineRecalculation.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Debug_BaselineRecalculation.Name = "tabPage_Debug_BaselineRecalculation";
            this.tabPage_Debug_BaselineRecalculation.Size = new System.Drawing.Size(719, 566);
            this.tabPage_Debug_BaselineRecalculation.TabIndex = 2;
            this.tabPage_Debug_BaselineRecalculation.Text = "Baseline Recalculation";
            this.tabPage_Debug_BaselineRecalculation.UseVisualStyleBackColor = true;
            this.tabPage_Debug_BaselineRecalculation.Enter += new System.EventHandler(this.tabPage_Debug_BaselineRecalculation_Enter);
            // 
            // button_BaselineRecalculation_RecalculateBaseline
            // 
            this.button_BaselineRecalculation_RecalculateBaseline.Location = new System.Drawing.Point(466, 421);
            this.button_BaselineRecalculation_RecalculateBaseline.Name = "button_BaselineRecalculation_RecalculateBaseline";
            this.button_BaselineRecalculation_RecalculateBaseline.Size = new System.Drawing.Size(242, 51);
            this.button_BaselineRecalculation_RecalculateBaseline.TabIndex = 61;
            this.button_BaselineRecalculation_RecalculateBaseline.Text = "Recalcualte Baseline";
            this.button_BaselineRecalculation_RecalculateBaseline.UseVisualStyleBackColor = true;
            this.button_BaselineRecalculation_RecalculateBaseline.Click += new System.EventHandler(this.button_BaselineRecalculation_RecalculateBaseline_Click);
            // 
            // label_BaselineRecalculation_Targets
            // 
            this.label_BaselineRecalculation_Targets.AutoSize = true;
            this.label_BaselineRecalculation_Targets.Location = new System.Drawing.Point(10, 215);
            this.label_BaselineRecalculation_Targets.Name = "label_BaselineRecalculation_Targets";
            this.label_BaselineRecalculation_Targets.Size = new System.Drawing.Size(52, 13);
            this.label_BaselineRecalculation_Targets.TabIndex = 60;
            this.label_BaselineRecalculation_Targets.Text = "Target(s):";
            // 
            // listBox_BaselineRecalculation_Targets
            // 
            this.listBox_BaselineRecalculation_Targets.FormattingEnabled = true;
            this.listBox_BaselineRecalculation_Targets.Location = new System.Drawing.Point(65, 117);
            this.listBox_BaselineRecalculation_Targets.Name = "listBox_BaselineRecalculation_Targets";
            this.listBox_BaselineRecalculation_Targets.ScrollAlwaysVisible = true;
            this.listBox_BaselineRecalculation_Targets.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox_BaselineRecalculation_Targets.Size = new System.Drawing.Size(150, 212);
            this.listBox_BaselineRecalculation_Targets.Sorted = true;
            this.listBox_BaselineRecalculation_Targets.TabIndex = 59;
            // 
            // button_BaselineRecalculation_AddAll
            // 
            this.button_BaselineRecalculation_AddAll.Location = new System.Drawing.Point(222, 449);
            this.button_BaselineRecalculation_AddAll.Name = "button_BaselineRecalculation_AddAll";
            this.button_BaselineRecalculation_AddAll.Size = new System.Drawing.Size(116, 23);
            this.button_BaselineRecalculation_AddAll.TabIndex = 58;
            this.button_BaselineRecalculation_AddAll.Text = "Add all";
            this.button_BaselineRecalculation_AddAll.UseVisualStyleBackColor = true;
            this.button_BaselineRecalculation_AddAll.Click += new System.EventHandler(this.button_BaselineRecalculation_AddAll_Click);
            // 
            // button_BaselineRecalculation_ClearAll
            // 
            this.button_BaselineRecalculation_ClearAll.Location = new System.Drawing.Point(344, 449);
            this.button_BaselineRecalculation_ClearAll.Name = "button_BaselineRecalculation_ClearAll";
            this.button_BaselineRecalculation_ClearAll.Size = new System.Drawing.Size(116, 23);
            this.button_BaselineRecalculation_ClearAll.TabIndex = 57;
            this.button_BaselineRecalculation_ClearAll.Text = "Clear all";
            this.button_BaselineRecalculation_ClearAll.UseVisualStyleBackColor = true;
            this.button_BaselineRecalculation_ClearAll.Click += new System.EventHandler(this.button_BaselineRecalculation_ClearAll_Click);
            // 
            // button_BaselineRecalculation_ClearSelected
            // 
            this.button_BaselineRecalculation_ClearSelected.Location = new System.Drawing.Point(344, 420);
            this.button_BaselineRecalculation_ClearSelected.Name = "button_BaselineRecalculation_ClearSelected";
            this.button_BaselineRecalculation_ClearSelected.Size = new System.Drawing.Size(116, 23);
            this.button_BaselineRecalculation_ClearSelected.TabIndex = 56;
            this.button_BaselineRecalculation_ClearSelected.Text = "Clear selected";
            this.button_BaselineRecalculation_ClearSelected.UseVisualStyleBackColor = true;
            this.button_BaselineRecalculation_ClearSelected.Click += new System.EventHandler(this.button_BaselineRecalculation_ClearSelected_Click);
            // 
            // listBox_BaselineRecalculation_SelectedTrials
            // 
            this.listBox_BaselineRecalculation_SelectedTrials.FormattingEnabled = true;
            this.listBox_BaselineRecalculation_SelectedTrials.Location = new System.Drawing.Point(221, 8);
            this.listBox_BaselineRecalculation_SelectedTrials.Name = "listBox_BaselineRecalculation_SelectedTrials";
            this.listBox_BaselineRecalculation_SelectedTrials.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox_BaselineRecalculation_SelectedTrials.Size = new System.Drawing.Size(487, 407);
            this.listBox_BaselineRecalculation_SelectedTrials.Sorted = true;
            this.listBox_BaselineRecalculation_SelectedTrials.TabIndex = 55;
            // 
            // button_BaselineRecalculation_AddSelected
            // 
            this.button_BaselineRecalculation_AddSelected.Location = new System.Drawing.Point(221, 420);
            this.button_BaselineRecalculation_AddSelected.Name = "button_BaselineRecalculation_AddSelected";
            this.button_BaselineRecalculation_AddSelected.Size = new System.Drawing.Size(116, 23);
            this.button_BaselineRecalculation_AddSelected.TabIndex = 54;
            this.button_BaselineRecalculation_AddSelected.Text = "Add selected";
            this.button_BaselineRecalculation_AddSelected.UseVisualStyleBackColor = true;
            this.button_BaselineRecalculation_AddSelected.Click += new System.EventHandler(this.button_BaselineRecalculation_AddSelected_Click);
            // 
            // label_BaselineRecalculation_Trials
            // 
            this.label_BaselineRecalculation_Trials.AutoSize = true;
            this.label_BaselineRecalculation_Trials.Location = new System.Drawing.Point(21, 366);
            this.label_BaselineRecalculation_Trials.Name = "label_BaselineRecalculation_Trials";
            this.label_BaselineRecalculation_Trials.Size = new System.Drawing.Size(41, 13);
            this.label_BaselineRecalculation_Trials.TabIndex = 53;
            this.label_BaselineRecalculation_Trials.Text = "Trial(s):";
            // 
            // listBox_BaselineRecalculation_Trials
            // 
            this.listBox_BaselineRecalculation_Trials.FormattingEnabled = true;
            this.listBox_BaselineRecalculation_Trials.Location = new System.Drawing.Point(65, 335);
            this.listBox_BaselineRecalculation_Trials.Name = "listBox_BaselineRecalculation_Trials";
            this.listBox_BaselineRecalculation_Trials.ScrollAlwaysVisible = true;
            this.listBox_BaselineRecalculation_Trials.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox_BaselineRecalculation_Trials.Size = new System.Drawing.Size(150, 82);
            this.listBox_BaselineRecalculation_Trials.Sorted = true;
            this.listBox_BaselineRecalculation_Trials.TabIndex = 52;
            // 
            // label_BaselineRecalculation_Group
            // 
            this.label_BaselineRecalculation_Group.AutoSize = true;
            this.label_BaselineRecalculation_Group.Location = new System.Drawing.Point(23, 38);
            this.label_BaselineRecalculation_Group.Name = "label_BaselineRecalculation_Group";
            this.label_BaselineRecalculation_Group.Size = new System.Drawing.Size(39, 13);
            this.label_BaselineRecalculation_Group.TabIndex = 49;
            this.label_BaselineRecalculation_Group.Text = "Group:";
            // 
            // comboBox_BaselineRecalculation_Group
            // 
            this.comboBox_BaselineRecalculation_Group.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_BaselineRecalculation_Group.FormattingEnabled = true;
            this.comboBox_BaselineRecalculation_Group.Location = new System.Drawing.Point(65, 35);
            this.comboBox_BaselineRecalculation_Group.Name = "comboBox_BaselineRecalculation_Group";
            this.comboBox_BaselineRecalculation_Group.Size = new System.Drawing.Size(150, 21);
            this.comboBox_BaselineRecalculation_Group.Sorted = true;
            this.comboBox_BaselineRecalculation_Group.TabIndex = 48;
            this.comboBox_BaselineRecalculation_Group.SelectedIndexChanged += new System.EventHandler(this.comboBox_BaselineRecalculation_Group_SelectedIndexChanged);
            // 
            // label_BaselineRecalculation_Szenario
            // 
            this.label_BaselineRecalculation_Szenario.AutoSize = true;
            this.label_BaselineRecalculation_Szenario.Location = new System.Drawing.Point(11, 65);
            this.label_BaselineRecalculation_Szenario.Name = "label_BaselineRecalculation_Szenario";
            this.label_BaselineRecalculation_Szenario.Size = new System.Drawing.Size(51, 13);
            this.label_BaselineRecalculation_Szenario.TabIndex = 47;
            this.label_BaselineRecalculation_Szenario.Text = "Szenario:";
            // 
            // label_BaselineRecalculation_Subject
            // 
            this.label_BaselineRecalculation_Subject.AutoSize = true;
            this.label_BaselineRecalculation_Subject.Location = new System.Drawing.Point(16, 92);
            this.label_BaselineRecalculation_Subject.Name = "label_BaselineRecalculation_Subject";
            this.label_BaselineRecalculation_Subject.Size = new System.Drawing.Size(46, 13);
            this.label_BaselineRecalculation_Subject.TabIndex = 46;
            this.label_BaselineRecalculation_Subject.Text = "Subject:";
            // 
            // label_BaselineRecalculation_Study
            // 
            this.label_BaselineRecalculation_Study.AutoSize = true;
            this.label_BaselineRecalculation_Study.Location = new System.Drawing.Point(25, 11);
            this.label_BaselineRecalculation_Study.Name = "label_BaselineRecalculation_Study";
            this.label_BaselineRecalculation_Study.Size = new System.Drawing.Size(37, 13);
            this.label_BaselineRecalculation_Study.TabIndex = 45;
            this.label_BaselineRecalculation_Study.Text = "Study:";
            // 
            // comboBox_BaselineRecalculation_Subject
            // 
            this.comboBox_BaselineRecalculation_Subject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_BaselineRecalculation_Subject.FormattingEnabled = true;
            this.comboBox_BaselineRecalculation_Subject.Location = new System.Drawing.Point(65, 89);
            this.comboBox_BaselineRecalculation_Subject.Name = "comboBox_BaselineRecalculation_Subject";
            this.comboBox_BaselineRecalculation_Subject.Size = new System.Drawing.Size(150, 21);
            this.comboBox_BaselineRecalculation_Subject.Sorted = true;
            this.comboBox_BaselineRecalculation_Subject.TabIndex = 44;
            this.comboBox_BaselineRecalculation_Subject.SelectedIndexChanged += new System.EventHandler(this.comboBox_BaselineRecalculation_Subject_SelectedIndexChanged);
            // 
            // comboBox_BaselineRecalculation_Study
            // 
            this.comboBox_BaselineRecalculation_Study.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_BaselineRecalculation_Study.FormattingEnabled = true;
            this.comboBox_BaselineRecalculation_Study.Location = new System.Drawing.Point(65, 8);
            this.comboBox_BaselineRecalculation_Study.Name = "comboBox_BaselineRecalculation_Study";
            this.comboBox_BaselineRecalculation_Study.Size = new System.Drawing.Size(150, 21);
            this.comboBox_BaselineRecalculation_Study.Sorted = true;
            this.comboBox_BaselineRecalculation_Study.TabIndex = 43;
            this.comboBox_BaselineRecalculation_Study.SelectedIndexChanged += new System.EventHandler(this.comboBox_BaselineRecalculation_Study_SelectedIndexChanged);
            // 
            // comboBox_BaselineRecalculation_Szenario
            // 
            this.comboBox_BaselineRecalculation_Szenario.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_BaselineRecalculation_Szenario.FormattingEnabled = true;
            this.comboBox_BaselineRecalculation_Szenario.Location = new System.Drawing.Point(65, 62);
            this.comboBox_BaselineRecalculation_Szenario.Name = "comboBox_BaselineRecalculation_Szenario";
            this.comboBox_BaselineRecalculation_Szenario.Size = new System.Drawing.Size(150, 21);
            this.comboBox_BaselineRecalculation_Szenario.Sorted = true;
            this.comboBox_BaselineRecalculation_Szenario.TabIndex = 42;
            this.comboBox_BaselineRecalculation_Szenario.SelectedIndexChanged += new System.EventHandler(this.comboBox_BaselineRecalculation_Szenario_SelectedIndexChanged);
            // 
            // tabPage_Impressum
            // 
            this.tabPage_Impressum.Controls.Add(this.label_Impressum_Text);
            this.tabPage_Impressum.Controls.Add(this.pictureBox_Impressum_KITLogo);
            this.tabPage_Impressum.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Impressum.Name = "tabPage_Impressum";
            this.tabPage_Impressum.Size = new System.Drawing.Size(733, 637);
            this.tabPage_Impressum.TabIndex = 11;
            this.tabPage_Impressum.Text = "Impressum";
            this.tabPage_Impressum.UseVisualStyleBackColor = true;
            // 
            // label_Impressum_Text
            // 
            this.label_Impressum_Text.AutoSize = true;
            this.label_Impressum_Text.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Impressum_Text.Location = new System.Drawing.Point(100, 272);
            this.label_Impressum_Text.Name = "label_Impressum_Text";
            this.label_Impressum_Text.Size = new System.Drawing.Size(304, 192);
            this.label_Impressum_Text.TabIndex = 1;
            this.label_Impressum_Text.Text = resources.GetString("label_Impressum_Text.Text");
            // 
            // pictureBox_Impressum_KITLogo
            // 
            this.pictureBox_Impressum_KITLogo.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_Impressum_KITLogo.Image")));
            this.pictureBox_Impressum_KITLogo.Location = new System.Drawing.Point(103, 15);
            this.pictureBox_Impressum_KITLogo.Name = "pictureBox_Impressum_KITLogo";
            this.pictureBox_Impressum_KITLogo.Size = new System.Drawing.Size(502, 234);
            this.pictureBox_Impressum_KITLogo.TabIndex = 0;
            this.pictureBox_Impressum_KITLogo.TabStop = false;
            // 
            // checkBox_Start_ManualMode
            // 
            this.checkBox_Start_ManualMode.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_Start_ManualMode.Enabled = false;
            this.checkBox_Start_ManualMode.FlatAppearance.BorderSize = 2;
            this.checkBox_Start_ManualMode.Location = new System.Drawing.Point(1079, 672);
            this.checkBox_Start_ManualMode.Name = "checkBox_Start_ManualMode";
            this.checkBox_Start_ManualMode.Size = new System.Drawing.Size(91, 23);
            this.checkBox_Start_ManualMode.TabIndex = 15;
            this.checkBox_Start_ManualMode.Text = "Extended mode";
            this.checkBox_Start_ManualMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_Start_ManualMode.UseVisualStyleBackColor = true;
            this.checkBox_Start_ManualMode.CheckedChanged += new System.EventHandler(this.checkBox_ManualMode_CheckedChanged);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(1, 672);
            this.progressBar.MarqueeAnimationSpeed = 0;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(741, 52);
            this.progressBar.Step = 1;
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 3;
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            // 
            // checkBox_PauseThread
            // 
            this.checkBox_PauseThread.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_PauseThread.Location = new System.Drawing.Point(748, 672);
            this.checkBox_PauseThread.Name = "checkBox_PauseThread";
            this.checkBox_PauseThread.Size = new System.Drawing.Size(91, 23);
            this.checkBox_PauseThread.TabIndex = 5;
            this.checkBox_PauseThread.Text = "Pause";
            this.checkBox_PauseThread.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_PauseThread.UseVisualStyleBackColor = true;
            this.checkBox_PauseThread.CheckedChanged += new System.EventHandler(this.checkBox_PauseThread_CheckedChanged);
            // 
            // label_Log
            // 
            this.label_Log.AutoSize = true;
            this.label_Log.Location = new System.Drawing.Point(745, 9);
            this.label_Log.Name = "label_Log";
            this.label_Log.Size = new System.Drawing.Size(28, 13);
            this.label_Log.TabIndex = 7;
            this.label_Log.Text = "Log:";
            // 
            // button_ClearLog
            // 
            this.button_ClearLog.Location = new System.Drawing.Point(1079, 701);
            this.button_ClearLog.Name = "button_ClearLog";
            this.button_ClearLog.Size = new System.Drawing.Size(91, 23);
            this.button_ClearLog.TabIndex = 8;
            this.button_ClearLog.Text = "Clear log";
            this.button_ClearLog.UseVisualStyleBackColor = true;
            this.button_ClearLog.Click += new System.EventHandler(this.button_ClearLog_Click);
            // 
            // listBox_LogBox
            // 
            this.listBox_LogBox.FormattingEnabled = true;
            this.listBox_LogBox.HorizontalScrollbar = true;
            this.listBox_LogBox.Location = new System.Drawing.Point(748, 25);
            this.listBox_LogBox.Name = "listBox_LogBox";
            this.listBox_LogBox.ScrollAlwaysVisible = true;
            this.listBox_LogBox.Size = new System.Drawing.Size(422, 641);
            this.listBox_LogBox.TabIndex = 9;
            // 
            // checkBox_Cancel
            // 
            this.checkBox_Cancel.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_Cancel.Location = new System.Drawing.Point(748, 701);
            this.checkBox_Cancel.Name = "checkBox_Cancel";
            this.checkBox_Cancel.Size = new System.Drawing.Size(91, 23);
            this.checkBox_Cancel.TabIndex = 16;
            this.checkBox_Cancel.Text = "Cancel";
            this.checkBox_Cancel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_Cancel.UseVisualStyleBackColor = true;
            this.checkBox_Cancel.CheckedChanged += new System.EventHandler(this.checkBox_Cancel_CheckedChanged);
            // 
            // ManipAnalysisGui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1172, 729);
            this.Controls.Add(this.checkBox_Cancel);
            this.Controls.Add(this.listBox_LogBox);
            this.Controls.Add(this.button_ClearLog);
            this.Controls.Add(this.label_Log);
            this.Controls.Add(this.checkBox_PauseThread);
            this.Controls.Add(this.checkBox_Start_ManualMode);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.progressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ManipAnalysisGui";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ManipAnalysis";
            this.tabControl.ResumeLayout(false);
            this.tabPage_Start.ResumeLayout(false);
            this.tabPage_Start.PerformLayout();
            this.tabPage_VisualizationExport.ResumeLayout(false);
            this.tabControl_VisualizationExport.ResumeLayout(false);
            this.tabPage_TrajectoryVelocity.ResumeLayout(false);
            this.tabPage_TrajectoryVelocity.PerformLayout();
            this.tabPage_DescriptiveStatistic1.ResumeLayout(false);
            this.tabPage_DescriptiveStatistic1.PerformLayout();
            this.tabPage_DescriptiveStatistic2.ResumeLayout(false);
            this.tabPage_DescriptiveStatistic2.PerformLayout();
            this.tabPage_Others.ResumeLayout(false);
            this.tabPage_Others.PerformLayout();
            this.tabPage_ImportCalculations.ResumeLayout(false);
            this.groupBox_Import_VelocityCropping.ResumeLayout(false);
            this.groupBox_Import_VelocityCropping.PerformLayout();
            this.groupBox_Import_CalculationsImport.ResumeLayout(false);
            this.groupBox_Import_TimeNormalization.ResumeLayout(false);
            this.groupBox_Import_TimeNormalization.PerformLayout();
            this.groupBox_Import_ButterworthFilter.ResumeLayout(false);
            this.groupBox_Import_ButterworthFilter.PerformLayout();
            this.tabPage_Debug.ResumeLayout(false);
            this.tabControl_Debug.ResumeLayout(false);
            this.tabPage_Debug_MatlabAndLogs.ResumeLayout(false);
            this.tabPage_Debug_DatabaseManipulation.ResumeLayout(false);
            this.tabPage_Debug_BaselineRecalculation.ResumeLayout(false);
            this.tabPage_Debug_BaselineRecalculation.PerformLayout();
            this.tabPage_Impressum.ResumeLayout(false);
            this.tabPage_Impressum.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Impressum_KITLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage_ImportCalculations;
        private System.Windows.Forms.TabPage tabPage_Debug;
        private System.Windows.Forms.Button button_Import_SelectMeasureFiles;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ListBox listBox_Import_SelectedMeasureFiles;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button button_Debug_ShowMatlabWindow;
        private System.Windows.Forms.Button button_Debug_ShowMatlabWorkspace;
        private System.Windows.Forms.Label label_DescriptiveStatistic1_Trials;
        private System.Windows.Forms.Label label_DescriptiveStatistic1_Subject;
        private System.Windows.Forms.Label label_DescriptiveStatistic1_Szenario;
        private System.Windows.Forms.Label label_DescriptiveStatistic1_Groups;
        private System.Windows.Forms.Label label_DescriptiveStatistic1_Study;
        private System.Windows.Forms.ListBox listBox_DescriptiveStatistic1_Trials;
        private System.Windows.Forms.ListBox listBox_DescriptiveStatistic1_Subjects;
        private System.Windows.Forms.ComboBox comboBox_DescriptiveStatistic1_Szenario;
        private System.Windows.Forms.ComboBox comboBox_DescriptiveStatistic1_Study;
        private System.Windows.Forms.ListBox listBox_DescriptiveStatistic1_Groups;
        private System.Windows.Forms.ListBox listBox_DescriptiveStatistic1_SelectedTrials;
        private System.Windows.Forms.Button button_DescriptiveStatistic1_AddSelected;
        private System.Windows.Forms.Button button_DescriptiveStatistic1_ClearAll;
        private System.Windows.Forms.Button button_DescriptiveStatistic1_ClearSelected;
        private System.Windows.Forms.ComboBox comboBox_DescriptiveStatistic1_DataTypeSelect;
        private System.Windows.Forms.Button button_DescriptiveStatistic1_PlotMeanStd;
        private System.Windows.Forms.Button button_DescriptiveStatistic1_AddAll;
        private System.Windows.Forms.CheckBox checkBox_DescriptiveStatistic1_PlotErrorbars;
        private System.Windows.Forms.Button button_Debug_showFaultyTrials;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.TabPage tabPage_Start;
        private System.Windows.Forms.Label label_Start_ManipAnalysis;
        private System.Windows.Forms.TabPage tabPage_VisualizationExport;
        private System.Windows.Forms.Button button_Start_ConnectToSQlServer;
        private System.Windows.Forms.CheckBox checkBox_Start_ManualMode;
        private System.Windows.Forms.TabControl tabControl_VisualizationExport;
        private System.Windows.Forms.TabPage tabPage_DescriptiveStatistic1;
        private System.Windows.Forms.TabPage tabPage_DescriptiveStatistic2;
        private System.Windows.Forms.TabPage tabPage_Others;
        private System.Windows.Forms.Label label_Others_Group;
        private System.Windows.Forms.ComboBox comboBox_Others_Group;
        private System.Windows.Forms.Label label_Others_Szenario;
        private System.Windows.Forms.Label label_Others_Subject;
        private System.Windows.Forms.Label label_Others_Study;
        private System.Windows.Forms.ComboBox comboBox_Others_Subject;
        private System.Windows.Forms.ComboBox comboBox_Others_Study;
        private System.Windows.Forms.ComboBox comboBox_Others_Szenario;
        private System.Windows.Forms.Button button_Others_PlotTrajectoryBaseline;
        private System.Windows.Forms.Button button_Others_PlotSzenarioMeanTimes;
        private System.Windows.Forms.Label label_DescriptiveStatistic1_Turns;
        private System.Windows.Forms.ListBox listBox_DescriptiveStatistic1_Turns;
        private System.Windows.Forms.ListBox listBox_DescriptiveStatistic2_Turns;
        private System.Windows.Forms.Label label_DescriptiveStatistic2_Turns;
        private System.Windows.Forms.Button button_DescriptiveStatistic2_AddAll;
        private System.Windows.Forms.ComboBox comboBox_DescriptiveStatistic2_DataTypeSelect;
        private System.Windows.Forms.Button button_DescriptiveStatistic2_ClearAll;
        private System.Windows.Forms.Button button_DescriptiveStatistic2_ClearSelected;
        private System.Windows.Forms.ListBox listBox_DescriptiveStatistic2_SelectedTrials;
        private System.Windows.Forms.Button button_DescriptiveStatistic2_AddSelected;
        private System.Windows.Forms.Label label_DescriptiveStatistic2_Trials;
        private System.Windows.Forms.Label label_DescriptiveStatistic2_Subject;
        private System.Windows.Forms.Label label_DescriptiveStatistic2_Szenario;
        private System.Windows.Forms.Label label_DescriptiveStatistic2_Groups;
        private System.Windows.Forms.Label label_DescriptiveStatistic2_Study;
        private System.Windows.Forms.ListBox listBox_DescriptiveStatistic2_Trials;
        private System.Windows.Forms.ListBox listBox_DescriptiveStatistic2_Subjects;
        private System.Windows.Forms.ComboBox comboBox_DescriptiveStatistic2_Szenario;
        private System.Windows.Forms.ComboBox comboBox_DescriptiveStatistic2_Study;
        private System.Windows.Forms.ListBox listBox_DescriptiveStatistic2_Groups;
        private System.Windows.Forms.Button button_DescriptiveStatistic2_CalculateMeanValues;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Button button_DescriptiveStatistic1_ExportData;
        private System.Windows.Forms.Button button_Import_SelectMeasureFileFolder;
        private System.Windows.Forms.CheckBox checkBox_PauseThread;
        private System.Windows.Forms.Button button_Import_ImportMeasureFiles;
        private System.Windows.Forms.GroupBox groupBox_Import_TimeNormalization;
        private System.Windows.Forms.Label label_Import_NewSampleCountText;
        private System.Windows.Forms.TextBox textBox_Import_NewSampleCount;
        private System.Windows.Forms.GroupBox groupBox_Import_ButterworthFilter;
        private System.Windows.Forms.Label label_Import_SamplesPerSec;
        private System.Windows.Forms.TextBox textBox_Import_CutoffFreqPosition;
        private System.Windows.Forms.Label label_Import_CutoffFreqPosition;
        private System.Windows.Forms.TextBox textBox_Import_SamplesPerSec;
        private System.Windows.Forms.TextBox textBox_Import_FilterOrder;
        private System.Windows.Forms.Label label_Import_FilterOrder;
        private System.Windows.Forms.GroupBox groupBox_Import_CalculationsImport;
        private System.Windows.Forms.Button button_Import_FixBrokenTrials;
        private System.Windows.Forms.Button button_Import_CalculateStatistics;
        private System.Windows.Forms.Button button_Debug_SaveLogToFile;
        private System.Windows.Forms.Label label_Log;
        private System.Windows.Forms.Button button_Import_AutoImport;
        private System.Windows.Forms.Button button_ClearLog;
        private System.Windows.Forms.TabPage tabPage_TrajectoryVelocity;
        private System.Windows.Forms.ListBox listBox_TrajectoryVelocity_Turns;
        private System.Windows.Forms.Label label_TrajectoryVelocity_Turns;
        private System.Windows.Forms.Button button_TrajectoryVelocity_AddAll;
        private System.Windows.Forms.Button button_TrajectoryVelocity_Plot;
        private System.Windows.Forms.Button button_TrajectoryVelocity_ClearAll;
        private System.Windows.Forms.Button button_TrajectoryVelocity_ClearSelected;
        private System.Windows.Forms.ListBox listBox_TrajectoryVelocity_SelectedTrials;
        private System.Windows.Forms.Button button_TrajectoryVelocity_AddSelected;
        private System.Windows.Forms.Label label_TrajectoryVelocity_Trials;
        private System.Windows.Forms.Label label_TrajectoryVelocity_Subjects;
        private System.Windows.Forms.Label label_TrajectoryVelocity_Szenario;
        private System.Windows.Forms.Label label_TrajectoryVelocity_Groups;
        private System.Windows.Forms.Label label_TrajectoryVelocity_Study;
        private System.Windows.Forms.ListBox listBox_TrajectoryVelocity_Trials;
        private System.Windows.Forms.ListBox listBox_TrajectoryVelocity_Subjects;
        private System.Windows.Forms.ComboBox comboBox_TrajectoryVelocity_Szenario;
        private System.Windows.Forms.ComboBox comboBox_TrajectoryVelocity_Study;
        private System.Windows.Forms.ListBox listBox_TrajectoryVelocity_Groups;
        private System.Windows.Forms.Label label_TrajectoryVelocity_Targets;
        private System.Windows.Forms.ListBox listBox_TrajectoryVelocity_Targets;
        private System.Windows.Forms.ComboBox comboBox_TrajectoryVelocity_IndividualMean;
        private System.Windows.Forms.ComboBox comboBox_TrajectoryVelocity_TrajectoryVelocity;
        private System.Windows.Forms.Button button_TrajectoryVelocity_Export;
        private System.Windows.Forms.Button button_Others_ExportTrajectoryBaseline;
        private System.Windows.Forms.Button button_Others_ExportSzenarioMeanTimes;
        private System.Windows.Forms.Label label_Others_Turn;
        private System.Windows.Forms.ComboBox comboBox_Others_Turn;
        private System.Windows.Forms.CheckBox checkBox_DescriptiveStatistic1_PlotFit;
        private System.Windows.Forms.TextBox textBox_DescriptiveStatistic1_FitEquation;
        private System.Windows.Forms.TabPage tabPage_Impressum;
        private System.Windows.Forms.PictureBox pictureBox_Impressum_KITLogo;
        private System.Windows.Forms.Label label_Impressum_Text;
        private System.Windows.Forms.Button button_Import_ClearMeasureFileList;
        private System.Windows.Forms.GroupBox groupBox_Import_VelocityCropping;
        private System.Windows.Forms.Label label_Import_PercentPeakVelocity;
        private System.Windows.Forms.TextBox textBox_Import_PercentPeakVelocity;
        private System.Windows.Forms.Button button_Start_SelectDatabase;
        private System.Windows.Forms.ComboBox comboBox_Start_Database;
        private System.Windows.Forms.Label label_Start_Database;
        private System.Windows.Forms.Button button_Others_ExportVelocityBaseline;
        private System.Windows.Forms.Button button_Others_PlotVelocityBaseline;
        private System.Windows.Forms.ListBox listBox_LogBox;

        private static void GetSubDirectories(ref List<DirectoryInfo> directoriesList, DirectoryInfo rootDir)
        {
            directoriesList.Add(rootDir);
            DirectoryInfo[] dirs = rootDir.GetDirectories();

            directoriesList.AddRange(dirs);

            for (int i = 0; i < dirs.Count(); i++)
            {
                DirectoryInfo di = dirs[i];
                GetSubDirectories(ref directoriesList, di);
            }
        }
        private System.Windows.Forms.TabControl tabControl_Debug;
        private System.Windows.Forms.TabPage tabPage_Debug_MatlabAndLogs;
        private System.Windows.Forms.TabPage tabPage_Debug_DatabaseManipulation;
        private System.Windows.Forms.TabPage tabPage_Debug_BaselineRecalculation;
        private System.Windows.Forms.Button button_Debug_DropDatabase;
        private System.Windows.Forms.Label label_BaselineRecalculation_Targets;
        private System.Windows.Forms.ListBox listBox_BaselineRecalculation_Targets;
        private System.Windows.Forms.Button button_BaselineRecalculation_AddAll;
        private System.Windows.Forms.Button button_BaselineRecalculation_ClearAll;
        private System.Windows.Forms.Button button_BaselineRecalculation_ClearSelected;
        private System.Windows.Forms.ListBox listBox_BaselineRecalculation_SelectedTrials;
        private System.Windows.Forms.Button button_BaselineRecalculation_AddSelected;
        private System.Windows.Forms.Label label_BaselineRecalculation_Trials;
        private System.Windows.Forms.ListBox listBox_BaselineRecalculation_Trials;
        private System.Windows.Forms.Label label_BaselineRecalculation_Group;
        private System.Windows.Forms.ComboBox comboBox_BaselineRecalculation_Group;
        private System.Windows.Forms.Label label_BaselineRecalculation_Szenario;
        private System.Windows.Forms.Label label_BaselineRecalculation_Subject;
        private System.Windows.Forms.Label label_BaselineRecalculation_Study;
        private System.Windows.Forms.ComboBox comboBox_BaselineRecalculation_Subject;
        private System.Windows.Forms.ComboBox comboBox_BaselineRecalculation_Study;
        private System.Windows.Forms.ComboBox comboBox_BaselineRecalculation_Szenario;
        private System.Windows.Forms.Button button_BaselineRecalculation_RecalculateBaseline;
        private System.Windows.Forms.Button button_Others_ExportGroupLi;
        private System.Windows.Forms.Button button_Others_PlotGroupLi;
        private System.Windows.Forms.CheckBox checkBox_Others_GroupAverage;
        private System.Windows.Forms.CheckBox checkBox_TrajectoryVelocity_ShowForceVectors;
        private System.Windows.Forms.CheckBox checkBox_TrajectoryVelocity_ShowPDForceVectors;
        private System.Windows.Forms.Button button_Others_PlotForcefieldCompensationFactor;
        private System.Windows.Forms.Label label_Import_CutoffFreqForceForce;
        private System.Windows.Forms.Label label_Import_CutoffFreqPositionPosition;
        private System.Windows.Forms.TextBox textBox_Import_CutoffFreqForce;
        private System.Windows.Forms.Label label_Import_CutoffFreqForce;
        private System.Windows.Forms.Label label_Others_PlotErrorclampForces_At;
        private System.Windows.Forms.Label label_Others_PlotErrorclampForces_Ms;
        private System.Windows.Forms.TextBox textBox_Others_PlotErrorclampForces_MsIndex;
        private System.Windows.Forms.Button button_Debug_ShowMatlabFiles;
        private System.Windows.Forms.CheckBox checkBox_Cancel;
        private System.Windows.Forms.Label label_Start_ServerIP;
        private System.Windows.Forms.ComboBox comboBox_Start_DatabaseServer;
        private System.Windows.Forms.Button button_DataManipulation_EnsureIndexes;
        private System.Windows.Forms.Button button_DataManipulation_RebuildIndexes;
        private System.Windows.Forms.Button button_DataManipulation_DropIndexes;
        private System.Windows.Forms.Button button_DataManipulation_CompactDatabase;
        private System.Windows.Forms.Button button_DataManipulation_DropStatistics;
        private System.Windows.Forms.Label label_TrajectoryVelocity_Handedness;
        private System.Windows.Forms.Label label_TrajectoryVelocity_ForceField;
        private System.Windows.Forms.Label label_TrajectoryVelocity_TrialType;
        private System.Windows.Forms.ListBox listBox_TrajectoryVelocity_Handedness;
        private System.Windows.Forms.ListBox listBox_TrajectoryVelocity_ForceField;
        private System.Windows.Forms.ListBox listBox_TrajectoryVelocity_TrialType;
        private System.Windows.Forms.Label label_DescriptiveStatistic1_Handedness;
        private System.Windows.Forms.Label label_DescriptiveStatistic1_ForceField;
        private System.Windows.Forms.Label label_DescriptiveStatistic1_TrialType;
        private System.Windows.Forms.ListBox listBox_DescriptiveStatistic1_Handedness;
        private System.Windows.Forms.ListBox listBox_DescriptiveStatistic1_ForceField;
        private System.Windows.Forms.ListBox listBox_DescriptiveStatistic1_TrialType;
        private System.Windows.Forms.Label label_DescriptiveStatistic2_Handedness;
        private System.Windows.Forms.Label label_DescriptiveStatistic2_ForceField;
        private System.Windows.Forms.Label label_DescriptiveStatistic2_TrialType;
        private System.Windows.Forms.ListBox listBox_DescriptiveStatistic2_Handedness;
        private System.Windows.Forms.ListBox listBox_DescriptiveStatistic2_ForceField;
        private System.Windows.Forms.ListBox listBox_DescriptiveStatistic2_TrialType;
    }
}

