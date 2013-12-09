using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private string fileName = null;
        private SqlCommand _sqlCmd;
        private SqlConnection _sqlCon;

        private StreamWriter filteredDataFileWriter;
        private StreamWriter normalizedDataFileWriter;

        private string _sqlDatabase;
        private string _sqlPassword;
        private string _sqlServer;
        private string _sqlUsername;
        MatlabWrapper _myMatlabWrapper;
        TrialContainer tc;
        List<TrialIdContainer> trialIDs;
        private delegate void setProgressCallback(int progress);

        // Create a NumberFormatInfo object and set some of its properties.
        NumberFormatInfo provider;

        public Form1()
        {
            InitializeComponent();

            provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";

            _myMatlabWrapper = new MatlabWrapper();
            tc = new TrialContainer();            

            _sqlServer = "localhost";
            _sqlDatabase = "master";
            _sqlUsername = "DataAccess";
            _sqlPassword = "!sport12";

            _sqlCon =
                new SqlConnection(@"Data Source=" + _sqlServer + ";Initial Catalog=" + _sqlDatabase + ";User Id=" +
                                  _sqlUsername + ";Password=" + _sqlPassword);
            _sqlCmd = new SqlCommand {Connection = _sqlCon, CommandTimeout = 600};
            
        }

        ~Form1()
        {
            CloseSQL();
        }

        private void OpenSQL()
        {
            _sqlCon.Open();
            //_sqlCon.ChangeDatabase("Study 01 - Catch Trial and Amount of Practice");
            _sqlCon.ChangeDatabase("Study 02 - Catch Trial Rate");
            //_sqlCon.ChangeDatabase("Study 03 - EEG MA Benny");
            //_sqlCon.ChangeDatabase("Study 04 - Generalization and Seat Position");
            //_sqlCon.ChangeDatabase("Study 05 - MPI & BioMotion Center");
        }

        private void CloseSQL()
        {
            if ((_sqlCon != null) && (_sqlCon.State == ConnectionState.Open))
            {
                _sqlCon.Close();
            }
        }

        private void setProgress(int progress)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new setProgressCallback(setProgress), progress);
            }
            else
            {
                progressBar1.Value = progress;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime tStart = DateTime.Now;
            OpenSQL();

            var filteredDataFileStream = new FileStream("C:\\measureDataFiltered.dat", FileMode.Create,
                     FileAccess.Write);

            var normalizedDataFileStream = new FileStream("C:\\measureDataNormalized.dat", FileMode.Create,
                FileAccess.Write);

            filteredDataFileWriter = new StreamWriter(filteredDataFileStream);
            normalizedDataFileWriter = new StreamWriter(normalizedDataFileStream);

            trialIDs = getTrailIds().OrderBy(t => t.trialId).ToList();
            
            
            var fileStream = new FileStream("C:\\Study2raw.csv", FileMode.Open, FileAccess.Read);
            var fileStreamReader = new StreamReader(fileStream);
            int tempTrialId = 1;
            int currentTrialId = 0;
            string[] splittedLine;

            progressBar1.Maximum = trialIDs.Count;
            listBox1.Items.Add("Started...");

            while(!fileStreamReader.EndOfStream)
            {
                splittedLine = fileStreamReader.ReadLine().Split('\t');
                currentTrialId = Convert.ToInt32(splittedLine[1]);

                if (currentTrialId > tempTrialId)
                {
                    try
                    {
                        if (progressBar1.Maximum > progressBar1.Value)
                        {
                            progressBar1.Value++;
                        }

                        TrialContainer tcFiltered = butterWorthFilter(tc, Convert.ToDouble(textBox_filterOrder.Text), Convert.ToDouble(textBox_PosCutoff.Text), Convert.ToDouble(textBox_ForceCutoff.Text), Convert.ToDouble(textBox_SampleRate.Text));

                        List<string> filteredDataFileCache =
                                            tcFiltered.data.Select(
                                                t =>
                                                    "," + t.trialId + "," + t.time_stamp.ToString("yyyy-MM-dd HH:mm:ss.fffffff") +
                                                    "," + DoubleConverter.ToExactString(t.force_actual_x) + "," +
                                                    DoubleConverter.ToExactString(t.force_actual_y) + "," +
                                                    DoubleConverter.ToExactString(t.force_actual_z) + "," +
                                                    DoubleConverter.ToExactString(t.force_nominal_x) + "," +
                                                    DoubleConverter.ToExactString(t.force_nominal_y) + "," +
                                                    DoubleConverter.ToExactString(t.force_nominal_z) + "," +
                                                    DoubleConverter.ToExactString(t.force_moment_x) + "," +
                                                    DoubleConverter.ToExactString(t.force_moment_y) + "," +
                                                    DoubleConverter.ToExactString(t.force_moment_z) + "," +
                                                    DoubleConverter.ToExactString(t.position_cartesian_x) + "," +
                                                    DoubleConverter.ToExactString(t.position_cartesian_y) + "," +
                                                    DoubleConverter.ToExactString(t.position_cartesian_z) + "," +
                                                    t.position_status).ToList();


                        tcFiltered.data.RemoveAll(t1 => !trialIDs.Select(t2 => t2.trialId).Contains(t1.trialId));
                        tcFiltered.data.RemoveAll(t => t.time_stamp < trialIDs.Where(t2 => t2.trialId == tc.data[0].trialId).ElementAt(0).startTime);
                        tcFiltered.data.RemoveAll(t => t.time_stamp > trialIDs.Where(t2 => t2.trialId == tc.data[0].trialId).ElementAt(0).stopTime);


                        TrialContainer tcNormalized = timeNormalization(tcFiltered, Convert.ToDouble(textBox_NewSampleRate.Text));

                        List<string> normalizedDataFileCache =
                                            tcNormalized.data.Select(
                                                t =>
                                                    "," + t.trialId + "," + t.time_stamp.ToString("yyyy-MM-dd HH:mm:ss.fffffff") +
                                                    "," + DoubleConverter.ToExactString(t.force_actual_x) + "," +
                                                    DoubleConverter.ToExactString(t.force_actual_y) + "," +
                                                    DoubleConverter.ToExactString(t.force_actual_z) + "," +
                                                    DoubleConverter.ToExactString(t.force_nominal_x) + "," +
                                                    DoubleConverter.ToExactString(t.force_nominal_y) + "," +
                                                    DoubleConverter.ToExactString(t.force_nominal_z) + "," +
                                                    DoubleConverter.ToExactString(t.force_moment_x) + "," +
                                                    DoubleConverter.ToExactString(t.force_moment_y) + "," +
                                                    DoubleConverter.ToExactString(t.force_moment_z) + "," +
                                                    DoubleConverter.ToExactString(t.position_cartesian_x) + "," +
                                                    DoubleConverter.ToExactString(t.position_cartesian_y) + "," +
                                                    DoubleConverter.ToExactString(t.position_cartesian_z) + "," +
                                                    t.position_status).ToList();

                        for (int cacheWriter = 0; cacheWriter < filteredDataFileCache.Count(); cacheWriter++)
                        {
                            filteredDataFileWriter.WriteLine(filteredDataFileCache[cacheWriter]);
                        }
                        for (int cacheWriter = 0; cacheWriter < normalizedDataFileCache.Count(); cacheWriter++)
                        {
                            normalizedDataFileWriter.WriteLine(normalizedDataFileCache[cacheWriter]);
                        }
                    }
                    catch
                    {
                        listBox1.Items.Add("Error in Trial " + currentTrialId);

                    }
                    tc = new TrialContainer();
                   
                    tempTrialId = currentTrialId;
                }
                else if (currentTrialId == tempTrialId)
                {
                    TrialContainerNode tempNode = new TrialContainerNode();
                    tempNode.trialId = currentTrialId;
                    tempNode.time_stamp = Convert.ToDateTime(splittedLine[2]);
                    tempNode.force_actual_x = Convert.ToDouble(splittedLine[3], provider);
                    tempNode.force_actual_y = Convert.ToDouble(splittedLine[4], provider);
                    tempNode.force_actual_z = Convert.ToDouble(splittedLine[5], provider);
                    tempNode.force_nominal_x = Convert.ToDouble(splittedLine[6], provider);
                    tempNode.force_nominal_y = Convert.ToDouble(splittedLine[7], provider);
                    tempNode.force_nominal_z = Convert.ToDouble(splittedLine[8], provider);
                    tempNode.force_moment_x = Convert.ToDouble(splittedLine[9], provider);
                    tempNode.force_moment_y = Convert.ToDouble(splittedLine[10], provider);
                    tempNode.force_moment_z = Convert.ToDouble(splittedLine[11], provider);
                    tempNode.position_cartesian_x = Convert.ToDouble(splittedLine[12], provider);
                    tempNode.position_cartesian_y = Convert.ToDouble(splittedLine[13], provider);
                    tempNode.position_cartesian_z = Convert.ToDouble(splittedLine[14], provider);
                    tempNode.position_status = Convert.ToInt32(splittedLine[15]);
                    tc.data.Add(tempNode);
                }
                else
                {
                    MessageBox.Show("Error");
                }
            }
            fileStream.Close();
           
            filteredDataFileWriter.Close();
            normalizedDataFileWriter.Close();
        }

        private List<TrialIdContainer> getTrailIds()
        {
            List<TrialIdContainer> trialIds = new List<TrialIdContainer>();

            _sqlCmd.Parameters.Clear();
            _sqlCmd.CommandType = CommandType.Text;
            _sqlCmd.CommandText = "SELECT trial_id, MIN(time_stamp), MAX(time_stamp) FROM _measure_data_normalized GROUP BY trial_id;";

            try
            {
                SqlDataReader sqlRdr = _sqlCmd.ExecuteReader();

                if (sqlRdr.HasRows)
                {
                    while (sqlRdr.Read())
                    {
                        TrialIdContainer tempContainer = new TrialIdContainer();
                        tempContainer.trialId = sqlRdr.GetInt32(0);
                        tempContainer.startTime = sqlRdr.GetDateTime(1);
                        tempContainer.stopTime = sqlRdr.GetDateTime(2);
                        trialIds.Add(tempContainer);
                    }
                    sqlRdr.Close();
                }
                else
                {
                    sqlRdr.Close();
                }
            }
            catch (Exception ex)
            {
            }

            return trialIds;
        }


        private TrialContainer getRawData(int trialID)
        {
            TrialContainer trialContainer = new TrialContainer();

            _sqlCmd.Parameters.Clear();
            _sqlCmd.CommandType = CommandType.Text;
            _sqlCmd.CommandText = "SELECT trial_id, time_stamp, force_actual_x, force_actual_y, force_actual_z, force_nominal_x, force_nominal_y, force_nominal_z, force_moment_x, force_moment_y, force_moment_z, position_cartesian_x, position_cartesian_y, position_cartesian_z, position_status FROM _measure_data_raw WHERE trial_id = " + trialID + ";";
            
            try
            {
                SqlDataReader sqlRdr = _sqlCmd.ExecuteReader();

                if (sqlRdr.HasRows)
                {
                    while (sqlRdr.Read())
                    {
                        TrialContainerNode tempNode = new TrialContainerNode();
                        tempNode.trialId = sqlRdr.GetInt32(0);
                        tempNode.time_stamp = sqlRdr.GetDateTime(1);
                        tempNode.force_actual_x = sqlRdr.GetDouble(2);
                        tempNode.force_actual_y = sqlRdr.GetDouble(3);
                        tempNode.force_actual_z = sqlRdr.GetDouble(4);
                        tempNode.force_nominal_x = sqlRdr.GetDouble(5);
                        tempNode.force_nominal_y = sqlRdr.GetDouble(6);
                        tempNode.force_nominal_z = sqlRdr.GetDouble(7);
                        tempNode.force_moment_x = sqlRdr.GetDouble(8);
                        tempNode.force_moment_y = sqlRdr.GetDouble(9);
                        tempNode.force_moment_z = sqlRdr.GetDouble(10);
                        tempNode.position_cartesian_x = sqlRdr.GetDouble(11);
                        tempNode.position_cartesian_y = sqlRdr.GetDouble(12);
                        tempNode.position_cartesian_z = sqlRdr.GetDouble(13);
                        tempNode.position_status = sqlRdr.GetInt32(14);
                        trialContainer.data.Add(tempNode);
                    }
                    sqlRdr.Close();
                }
                else
                {
                    sqlRdr.Close();
                }
            }
            catch (Exception ex)
            {
            }

            return trialContainer;
        }

        TrialContainer butterWorthFilter(TrialContainer tc, double filterOrder, double cutoffPos, double cutoffForce, double samplesPerSec)
        {
            _myMatlabWrapper.ClearWorkspace();
            TrialContainer newTc = new TrialContainer();

            _myMatlabWrapper.SetWorkspaceData("filterOrder", filterOrder);
            _myMatlabWrapper.SetWorkspaceData("cutoffFreqPosition", cutoffPos);
            _myMatlabWrapper.SetWorkspaceData("cutoffFreqForce", cutoffForce);
            _myMatlabWrapper.SetWorkspaceData("samplesPerSecond", samplesPerSec);
            _myMatlabWrapper.Execute("[bPosition,aPosition] = butter(filterOrder,(cutoffFreqPosition/(samplesPerSecond/2)));");
            _myMatlabWrapper.Execute("[bForce,aForce] = butter(filterOrder,(cutoffFreqForce/(samplesPerSecond/2)));");

            _myMatlabWrapper.SetWorkspaceData(
                "force_actual_x", tc.data.Select(t => t.force_actual_x).ToArray());
            _myMatlabWrapper.SetWorkspaceData(
                "force_actual_y", tc.data.Select(t => t.force_actual_y).ToArray());
            _myMatlabWrapper.SetWorkspaceData(
                "force_actual_z", tc.data.Select(t => t.force_actual_z).ToArray());

            _myMatlabWrapper.SetWorkspaceData(
                "force_nominal_x", tc.data.Select(t => t.force_nominal_x).ToArray());
            _myMatlabWrapper.SetWorkspaceData(
                "force_nominal_y", tc.data.Select(t => t.force_nominal_y).ToArray());
            _myMatlabWrapper.SetWorkspaceData(
                "force_nominal_z", tc.data.Select(t => t.force_nominal_z).ToArray());

            _myMatlabWrapper.SetWorkspaceData(
                "force_moment_x", tc.data.Select(t => t.force_moment_x).ToArray());
            _myMatlabWrapper.SetWorkspaceData(
                "force_moment_y", tc.data.Select(t => t.force_moment_y).ToArray());
            _myMatlabWrapper.SetWorkspaceData(
                "force_moment_z", tc.data.Select(t => t.force_moment_z).ToArray());

            _myMatlabWrapper.SetWorkspaceData(
                "position_cartesian_x", tc.data.Select(t => t.position_cartesian_x).ToArray());
            _myMatlabWrapper.SetWorkspaceData(
                "position_cartesian_y", tc.data.Select(t => t.position_cartesian_y).ToArray());
            _myMatlabWrapper.SetWorkspaceData(
                "position_cartesian_z", tc.data.Select(t => t.position_cartesian_z).ToArray());

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

            for (int j = 0; j < tc.data.Count; j++)
            {
                TrialContainerNode tempNode = new TrialContainerNode();
                tempNode.trialId = tc.data[j].trialId;
                tempNode.time_stamp = tc.data[j].time_stamp;
                tempNode.force_actual_x = forceActualX[0, j];
                tempNode.force_actual_y = forceActualY[0, j];
                tempNode.force_actual_z = forceActualZ[0, j];
                tempNode.force_nominal_x = forceNominalX[0, j];
                tempNode.force_nominal_y = forceNominalY[0, j];
                tempNode.force_nominal_z = forceNominalZ[0, j];
                tempNode.force_moment_x = forceMomentX[0, j];
                tempNode.force_moment_y = forceMomentY[0, j];
                tempNode.force_moment_z = forceMomentZ[0, j];
                tempNode.position_cartesian_x = positionCartesianX[0, j];
                tempNode.position_cartesian_y = positionCartesianY[0, j];
                tempNode.position_cartesian_z = positionCartesianZ[0, j];
                tempNode.position_status = tc.data[j].position_status;
                newTc.data.Add(tempNode);
            }
            
            _myMatlabWrapper.ClearWorkspace();
            return newTc;
        }

        TrialContainer timeNormalization(TrialContainer tc, double newSampleRate)
        {
            _myMatlabWrapper.ClearWorkspace();
            TrialContainer newTc = new TrialContainer();

            _myMatlabWrapper.SetWorkspaceData("newSampleCount", newSampleRate);

            _myMatlabWrapper.SetWorkspaceData("measure_data_time", tc.data.Select(t => Convert.ToDouble(t.time_stamp.Ticks)).ToArray());
            _myMatlabWrapper.SetWorkspaceData("forceActualX", tc.data.Select(t => t.force_actual_x).ToArray());
            _myMatlabWrapper.SetWorkspaceData("forceActualY", tc.data.Select(t => t.force_actual_y).ToArray());
            _myMatlabWrapper.SetWorkspaceData("forceActualZ", tc.data.Select(t => t.force_actual_z).ToArray());
            _myMatlabWrapper.SetWorkspaceData("forceNominalX", tc.data.Select(t => t.force_nominal_x).ToArray());
            _myMatlabWrapper.SetWorkspaceData("forceNominalY", tc.data.Select(t => t.force_nominal_y).ToArray());
            _myMatlabWrapper.SetWorkspaceData("forceNominalZ", tc.data.Select(t => t.force_nominal_z).ToArray());
            _myMatlabWrapper.SetWorkspaceData("forceMomentX", tc.data.Select(t => t.force_moment_x).ToArray());
            _myMatlabWrapper.SetWorkspaceData("forceMomentY", tc.data.Select(t => t.force_moment_y).ToArray());
            _myMatlabWrapper.SetWorkspaceData("forceMomentZ", tc.data.Select(t => t.force_moment_z).ToArray());
            _myMatlabWrapper.SetWorkspaceData("positionCartesianX", tc.data.Select(t => t.position_cartesian_x).ToArray());
            _myMatlabWrapper.SetWorkspaceData("positionCartesianY", tc.data.Select(t => t.position_cartesian_y).ToArray());
            _myMatlabWrapper.SetWorkspaceData("positionCartesianZ", tc.data.Select(t => t.position_cartesian_z).ToArray());
            _myMatlabWrapper.SetWorkspaceData("positionStatus", tc.data.Select(t => Convert.ToDouble(t.position_status)).ToArray());

            //-----

            _myMatlabWrapper.Execute("[errorvar1, forceActualX,newMeasureTime] = timeNorm(forceActualX,measure_data_time,newSampleCount);");
            _myMatlabWrapper.Execute("[errorvar2, forceActualY,newMeasureTime] = timeNorm(forceActualY,measure_data_time,newSampleCount);");
            _myMatlabWrapper.Execute("[errorvar3, forceActualZ,newMeasureTime] = timeNorm(forceActualZ,measure_data_time,newSampleCount);");

            _myMatlabWrapper.Execute("[errorvar4, forceNominalX,newMeasureTime] = timeNorm(forceNominalX,measure_data_time,newSampleCount);");
            _myMatlabWrapper.Execute("[errorvar5, forceNominalY,newMeasureTime] = timeNorm(forceNominalY,measure_data_time,newSampleCount);");
            _myMatlabWrapper.Execute("[errorvar6, forceNominalZ,newMeasureTime] = timeNorm(forceNominalZ,measure_data_time,newSampleCount);");

            _myMatlabWrapper.Execute("[errorvar7, forceMomentX,newMeasureTime] = timeNorm(forceMomentX,measure_data_time,newSampleCount);");
            _myMatlabWrapper.Execute("[errorvar8, forceMomentY,newMeasureTime] = timeNorm(forceMomentY,measure_data_time,newSampleCount);");
            _myMatlabWrapper.Execute("[errorvar9, forceMomentZ,newMeasureTime] = timeNorm(forceMomentZ,measure_data_time,newSampleCount);");

            _myMatlabWrapper.Execute("[errorvar10, positionCartesianX,newMeasureTime] = timeNorm(positionCartesianX,measure_data_time,newSampleCount);");
            _myMatlabWrapper.Execute("[errorvar11, positionCartesianY,newMeasureTime] = timeNorm(positionCartesianY,measure_data_time,newSampleCount);");
            _myMatlabWrapper.Execute("[errorvar12, positionCartesianZ,newMeasureTime] = timeNorm(positionCartesianZ,measure_data_time,newSampleCount);");

            _myMatlabWrapper.Execute("[errorvar13, positionStatus,newMeasureTime] = timeNorm(positionStatus,measure_data_time,newSampleCount);");

            double[,] timeStamp =
                _myMatlabWrapper.GetWorkspaceData("newMeasureTime");

            double[,] forceActualX =
                _myMatlabWrapper.GetWorkspaceData("forceActualX");
            double[,] forceActualY =
                _myMatlabWrapper.GetWorkspaceData("forceActualY");
            double[,] forceActualZ =
                _myMatlabWrapper.GetWorkspaceData("forceActualZ");

            double[,] forceNominalX =
                _myMatlabWrapper.GetWorkspaceData("forceNominalX");
            double[,] forceNominalY =
                _myMatlabWrapper.GetWorkspaceData("forceNominalY");
            double[,] forceNominalZ =
                _myMatlabWrapper.GetWorkspaceData("forceNominalZ");

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

            int tempTrialId = tc.data[0].trialId;
            for (int j = 0; j < timeStamp.Length; j++)
            {
                TrialContainerNode tempNode = new TrialContainerNode();
                tempNode.trialId = tempTrialId;
                tempNode.time_stamp = new DateTime(Convert.ToInt64(timeStamp[j, 0]));
                tempNode.force_actual_x = forceActualX[j, 0];
                tempNode.force_actual_y = forceActualY[j, 0];
                tempNode.force_actual_z = forceActualZ[j, 0];
                tempNode.force_nominal_x = forceNominalX[j, 0];
                tempNode.force_nominal_y = forceNominalY[j, 0];
                tempNode.force_nominal_z = forceNominalZ[j, 0];
                tempNode.force_moment_x = forceMomentX[j, 0];
                tempNode.force_moment_y = forceMomentY[j, 0];
                tempNode.force_moment_z = forceMomentZ[j, 0];
                tempNode.position_cartesian_x = positionCartesianX[j, 0];
                tempNode.position_cartesian_y = positionCartesianY[j, 0];
                tempNode.position_cartesian_z = positionCartesianZ[j, 0];
                tempNode.position_status = Convert.ToInt32(positionStatus[j, 0]);
                newTc.data.Add(tempNode);
            }

            _myMatlabWrapper.ClearWorkspace();

            return newTc;
        }
    }
}
