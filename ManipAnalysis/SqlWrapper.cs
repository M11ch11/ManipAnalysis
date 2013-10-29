using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using ManipAnalysis.Container;

namespace ManipAnalysis
{
    public class SqlWrapper
    {
        private readonly ManipAnalysisGui _myManipAnalysisGui;
        private SqlCommand _sqlCmd;
        private SqlConnection _sqlCon;

        private string _sqlDatabase;
        private string _sqlPassword;
        private string _sqlServer;
        private string _sqlUsername;

        public SqlWrapper(ManipAnalysisGui myManipAnalysisGui)
        {
            _myManipAnalysisGui = myManipAnalysisGui;

            _sqlServer = "localhost";
            _sqlDatabase = "master";
            _sqlUsername = "DataAccess";
            _sqlPassword = "!sport12";
        }

        ~SqlWrapper()
        {
            CloseSqlConnection();
        }

        private void SetConnectionString(string serverUri, string database, string username, string password)
        {
            _sqlServer = serverUri;
            _sqlDatabase = database;
            _sqlUsername = username;
            _sqlPassword = password;

            _sqlCon =
                new SqlConnection(@"Data Source=" + _sqlServer + ";Initial Catalog=" + _sqlDatabase + ";User Id=" +
                                  _sqlUsername + ";Password=" + _sqlPassword) {FireInfoMessageEventOnUserErrors = true};

            _sqlCon.InfoMessage += OnSqlConInfoMessage;

            _sqlCmd = new SqlCommand {Connection = _sqlCon, CommandTimeout = 600};
        }

        private void OnSqlConInfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            _myManipAnalysisGui.WriteToLogBox(e.ToString());
        }

        public void SetSqlServer(string serverUri)
        {
            CloseSqlConnection();
            SetConnectionString(serverUri, _sqlDatabase, _sqlUsername, _sqlPassword);
        }

        public void SetDatabase(string database)
        {
            OpenSqlConnection();
            _sqlCon.ChangeDatabase(database);
        }

        private void OpenSqlConnection()
        {
            if (_sqlCon.State == ConnectionState.Closed)
            {
                _myManipAnalysisGui.WriteProgressInfo("Opening SQL-Connection...");

                bool isOpen;
                try
                {
                    _sqlCon.Open();
                    isOpen = true;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    isOpen = false;
                }


                if (isOpen)
                {
                    _myManipAnalysisGui.WriteProgressInfo("Ready.");
                }
                else
                {
                    _myManipAnalysisGui.WriteProgressInfo("SQL-Connection failed!");
                }
            }
        }

        public void CloseSqlConnection()
        {
            if ((_sqlCon != null) && (_sqlCon.State == ConnectionState.Open))
            {
                try
                {
                    _sqlCon.Close();
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                }
            }
        }

        public string[] GetDatabases()
        {
            string[] retVal = null;

            _sqlCmd.Parameters.Clear();
            _sqlCmd.CommandType = CommandType.Text;
            _sqlCmd.CommandText = "SELECT name FROM master..sysdatabases WHERE name LIKE 'Study%';";

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    SqlDataReader sqlRdr = _sqlCmd.ExecuteReader();

                    if (sqlRdr.HasRows)
                    {
                        var rows = new List<string>();
                        while (sqlRdr.Read())
                        {
                            rows.Add(sqlRdr.GetString(0));
                        }
                        sqlRdr.Close();
                        retVal = rows.ToArray();
                    }
                    else
                    {
                        sqlRdr.Close();
                    }
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        private void ExecuteSqlFile(string filename)
        {
            _sqlCmd.Parameters.Clear();
            _sqlCmd.CommandType = CommandType.Text;
            var cmds = new List<string>();

            if (File.Exists(filename))
            {
                TextReader tr = new StreamReader(filename);
                string line;
                string cmd = "";

                while ((line = tr.ReadLine()) != null)
                {
                    if (line.Trim().ToUpper() == "GO")
                    {
                        cmds.Add(cmd);
                        cmd = "";
                    }
                    else
                    {
                        cmd += line + "\r\n";
                    }
                }
                if (cmd.Length > 0)
                {
                    cmds.Add(cmd);
                }
                tr.Close();
            }

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();

                    if (cmds.Count > 0)
                    {
                        for (int i = 0; i < cmds.Count; i++)
                        {
                            _sqlCmd.CommandText = cmds[i];
                            _sqlCmd.ExecuteNonQuery();
                        }
                    }

                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }
        }

        public void InitializeDatabase()
        {
            ExecuteSqlFile("SQL-Files\\SQL-Tables.sql");
            ExecuteSqlFile("SQL-Files\\SQL-FunctionsProcedures.sql");
        }

        public void DeleteMeasureFile(int measureFileID)
        {
            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.StoredProcedure;

            _sqlCmd.CommandText = "deleteMeasureFile";
            _sqlCmd.Parameters.Add(new SqlParameter("@measureFileID", measureFileID));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }
        }

        public void DeleteSubjectStatistics(SubjectInformationContainer subject)
        {
            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.StoredProcedure;

            _sqlCmd.CommandText = "deleteSubjectStatistics";
            _sqlCmd.Parameters.Add(new SqlParameter("@subjectID", subject.ID));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }
        }

        public void DeleteBaselineData(int baselineID)
        {
            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.StoredProcedure;

            _sqlCmd.CommandText = "deleteBaselineData";
            _sqlCmd.Parameters.Add(new SqlParameter("@baselineID", baselineID));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }
        }


        public void ChangeGroupID(int oldGroupID, int newGroupID)
        {
            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.StoredProcedure;

            _sqlCmd.CommandText = "changeGroupID";
            _sqlCmd.Parameters.Add(new SqlParameter("@oldGroupID", oldGroupID));
            _sqlCmd.Parameters.Add(new SqlParameter("@newGroupID", newGroupID));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }
        }

        public void ChangeSubjectID(int oldSubjectID, int newSubjectID)
        {
            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.StoredProcedure;

            _sqlCmd.CommandText = "changeSubjectID";
            _sqlCmd.Parameters.Add(new SqlParameter("@oldSubjectID", oldSubjectID));
            _sqlCmd.Parameters.Add(new SqlParameter("@newSubjectID", newSubjectID));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }
        }

        public void ChangeSubjectName(int subjectID, string newSubjectName)
        {
            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.StoredProcedure;

            _sqlCmd.CommandText = "changeSubjectName";
            _sqlCmd.Parameters.Add(new SqlParameter("@subjectID", subjectID));
            _sqlCmd.Parameters.Add(new SqlParameter("@newSubjectName", newSubjectName));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }
        }

        public void ChangeSubjectSubjectID(int subjectID, string newSubjectSubjectID)
        {
            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.StoredProcedure;

            _sqlCmd.CommandText = "changeSubjectSubjectID";
            _sqlCmd.Parameters.Add(new SqlParameter("@subjectID", subjectID));
            _sqlCmd.Parameters.Add(new SqlParameter("@newSubjectSubjectID", newSubjectSubjectID));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }
        }

        public void ChangeGroupName(int groupID, string newGrouptName)
        {
            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.StoredProcedure;

            _sqlCmd.CommandText = "changeGroupName";
            _sqlCmd.Parameters.Add(new SqlParameter("@groupID", groupID));
            _sqlCmd.Parameters.Add(new SqlParameter("@newGroupName", newGrouptName));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }
        }

        public List<int[]> GetStatisticCalculationInformation()
        {
            List<int[]> retVal = null;

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.Text;

            _sqlCmd.CommandText = "SELECT * FROM getStatisticCalculationInformation()";

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    SqlDataReader sqlRdr = _sqlCmd.ExecuteReader();

                    if (sqlRdr.HasRows)
                    {
                        retVal = new List<int[]>();

                        while (sqlRdr.Read())
                        {
                            bool hasNullValues = false;
                            for (int i = 0; i < 6; i++)
                            {
                                if (sqlRdr.IsDBNull(i))
                                {
                                    if (i > 0)
                                    {
                                        _myManipAnalysisGui.WriteToLogBox("Trial " + sqlRdr.GetInt32(0) +
                                                                          " has NULL values as id.");
                                    }
                                    else
                                    {
                                        _myManipAnalysisGui.WriteToLogBox("There are NULL values.");
                                    }
                                    hasNullValues = true;
                                }
                            }

                            if (!hasNullValues)
                            {
                                retVal.Add(new int[]
                                    {
                                        sqlRdr.GetInt32(0), //trial_id
                                        sqlRdr.GetInt32(1), //subject_id
                                        sqlRdr.GetInt32(2), //study_id
                                        sqlRdr.GetInt32(3), //group_id
                                        sqlRdr.GetInt32(4), //target_id
                                        sqlRdr.GetInt32(5) //target_number
                                    });
                            }
                        }
                        sqlRdr.Close();
                    }
                    else
                    {
                        sqlRdr.Close();
                    }
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    _sqlCmd.Cancel();
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public List<object[]> GetFaultyTrialInformation()
        {
            List<object[]> retVal = null;

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.Text;

            _sqlCmd.CommandText = "SELECT * FROM getFaultyTrialInformation()";

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    SqlDataReader sqlRdr = _sqlCmd.ExecuteReader();

                    if (sqlRdr.HasRows)
                    {
                        retVal = new List<object[]>();
                        while (sqlRdr.Read())
                        {
                            retVal.Add(new object[]
                                {
                                    sqlRdr.GetInt32(0), //trial_id
                                    sqlRdr.GetInt32(1), //measure_file_id
                                    sqlRdr.GetString(2), //study_name
                                    sqlRdr.GetString(3), //group_name
                                    sqlRdr.GetInt32(4), //subject_id
                                    sqlRdr.GetString(5), //szenario_name
                                    sqlRdr.GetDateTime(6), //measure_file_creation_time
                                    sqlRdr.GetInt32(7) //szenario_trial_number
                                });
                        }
                        sqlRdr.Close();
                    }
                    else
                    {
                        sqlRdr.Close();
                    }
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int[] GetFaultyTrialFixInformation(int measureFileID, int szenarioTrialNumber)
        {
            int[] retVal = null;

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.StoredProcedure;

            _sqlCmd.CommandText = "getFaultyTrialFixInformation";

            _sqlCmd.Parameters.Add("@upperTrialID", SqlDbType.Int);
            _sqlCmd.Parameters.Add("@lowerTrialID", SqlDbType.Int);
            _sqlCmd.Parameters.Add(new SqlParameter("@measureFileID", measureFileID));
            _sqlCmd.Parameters.Add(new SqlParameter("@szenarioTrialNumber", szenarioTrialNumber));

            _sqlCmd.Parameters["@upperTrialID"].Direction = ParameterDirection.Output;
            _sqlCmd.Parameters["@lowerTrialID"].Direction = ParameterDirection.Output;

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    retVal = new int[]
                        {
                            Convert.ToInt32(_sqlCmd.Parameters["@upperTrialID"].Value),
                            Convert.ToInt32(_sqlCmd.Parameters["@lowerTrialID"].Value)
                        };
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public DateTime GetTurnDateTime(string study, string group, string szenario, SubjectInformationContainer subject,
                                        int turn)
        {
            var retVal = new DateTime();

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.StoredProcedure;

            _sqlCmd.CommandText = "getTurnDateTime";

            _sqlCmd.Parameters.Add("@turnDateTime", SqlDbType.DateTime2);
            _sqlCmd.Parameters.Add(new SqlParameter("@studyName", study));
            _sqlCmd.Parameters.Add(new SqlParameter("@groupName", group));
            _sqlCmd.Parameters.Add(new SqlParameter("@szenarioName", szenario));
            _sqlCmd.Parameters.Add(new SqlParameter("@subjectID", subject.ID));
            _sqlCmd.Parameters.Add(new SqlParameter("@turn", turn));

            _sqlCmd.Parameters["@turnDateTime"].Direction = ParameterDirection.Output;

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToDateTime(_sqlCmd.Parameters["@turnDateTime"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public DataSet GetMeasureDataNormalizedDataSet(int trialID)
        {
            DataSet retVal = null;

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.Text;

            _sqlCmd.CommandText = "SELECT * FROM getMeasureDataNormalizedData(@trialID) ORDER BY time_stamp;";
            _sqlCmd.Parameters.Add(new SqlParameter("@trialID", trialID));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    var sqlAdapt = new SqlDataAdapter(_sqlCmd);
                    retVal = new DataSet();
                    sqlAdapt.Fill(retVal);
                    sqlAdapt.Dispose();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int GetTrailID(string study, string group, string szenario, SubjectInformationContainer subject,
                              DateTime turnDateTime,
                              int target, int trial)
        {
            int retVal = -1;

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.StoredProcedure;

            _sqlCmd.CommandText = "getTrialID";

            _sqlCmd.Parameters.Add(new SqlParameter("@studyName", study));
            _sqlCmd.Parameters.Add(new SqlParameter("@groupName", group));
            _sqlCmd.Parameters.Add(new SqlParameter("@szenarioName", szenario));
            _sqlCmd.Parameters.Add(new SqlParameter("@subjectID", subject.ID));
            _sqlCmd.Parameters.Add(new SqlParameter("@turnDateTime", turnDateTime));
            _sqlCmd.Parameters.Add(new SqlParameter("@target", target));
            _sqlCmd.Parameters.Add(new SqlParameter("@trial", trial));
            _sqlCmd.Parameters.Add("@id", SqlDbType.Int);

            _sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(_sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int GetTrailID(string study, string group, string szenario, SubjectInformationContainer subject,
                              DateTime turnDateTime, int szenarioTrialNumber)
        {
            int retVal = -1;

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.StoredProcedure;

            _sqlCmd.CommandText = "getTrialID2";

            _sqlCmd.Parameters.Add(new SqlParameter("@studyName", study));
            _sqlCmd.Parameters.Add(new SqlParameter("@groupName", group));
            _sqlCmd.Parameters.Add(new SqlParameter("@szenarioName", szenario));
            _sqlCmd.Parameters.Add(new SqlParameter("@subjectID", subject.ID));
            _sqlCmd.Parameters.Add(new SqlParameter("@turnDateTime", turnDateTime));
            _sqlCmd.Parameters.Add(new SqlParameter("@szenarioTrialNumber", szenarioTrialNumber));
            _sqlCmd.Parameters.Add("@id", SqlDbType.Int);

            _sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(_sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int GetBaselineID(string study, string group, string szenario, SubjectInformationContainer subject,
                                 int target)
        {
            int retVal = -1;

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.StoredProcedure;

            _sqlCmd.CommandText = "getBaselineID";

            _sqlCmd.Parameters.Add(new SqlParameter("@studyName", study));
            _sqlCmd.Parameters.Add(new SqlParameter("@groupName", group));
            _sqlCmd.Parameters.Add(new SqlParameter("@szenarioName", szenario));
            _sqlCmd.Parameters.Add(new SqlParameter("@subjectID", subject.ID));
            _sqlCmd.Parameters.Add(new SqlParameter("@target", target));
            _sqlCmd.Parameters.Add("@id", SqlDbType.Int);

            _sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(_sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public DataSet GetVelocityDataNormalizedDataSet(int trialID)
        {
            DataSet retVal = null;

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.Text;

            _sqlCmd.CommandText = "SELECT * FROM getVelocityDataNormalizedData(@trialID) ORDER BY time_stamp;";
            _sqlCmd.Parameters.Add(new SqlParameter("@trialID", trialID));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    var sqlAdapt = new SqlDataAdapter(_sqlCmd);
                    retVal = new DataSet();
                    sqlAdapt.Fill(retVal);
                    sqlAdapt.Dispose();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public DataSet GetStatisticDataSet(int trialID)
        {
            DataSet retVal = null;

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.Text;

            _sqlCmd.CommandText = "SELECT * FROM getStatisticData(@trialID);";
            _sqlCmd.Parameters.Add(new SqlParameter("@trialID", trialID));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    var sqlAdapt = new SqlDataAdapter(_sqlCmd);
                    retVal = new DataSet();
                    sqlAdapt.Fill(retVal);
                    sqlAdapt.Dispose();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public DataSet GetStatisticDataSet(string studyName, string groupName, string szenarioName,
                                           SubjectInformationContainer subject,
                                           DateTime turn)
        {
            DataSet retVal = null;

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.Text;

            _sqlCmd.CommandText =
                "SELECT * FROM getStatisticData2(@studyName,@groupName,@szenarioName,@subjectID,@turnDateTime);";
            _sqlCmd.Parameters.Add(new SqlParameter("@studyName", studyName));
            _sqlCmd.Parameters.Add(new SqlParameter("@groupName", groupName));
            _sqlCmd.Parameters.Add(new SqlParameter("@szenarioName", szenarioName));
            _sqlCmd.Parameters.Add(new SqlParameter("@subjectID", subject.ID));
            _sqlCmd.Parameters.Add(new SqlParameter("@turnDateTime", turn));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    var sqlAdapt = new SqlDataAdapter(_sqlCmd);
                    retVal = new DataSet();
                    sqlAdapt.Fill(retVal);
                    sqlAdapt.Dispose();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public DataSet GetBaselineDataSet(int subjectID, int studyID, int groupID, int targetID)
        {
            DataSet retVal = null;

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.Text;

            _sqlCmd.CommandText =
                "SELECT * FROM getBaseLineData(@subjectID,@studyID,@groupID,@targetID) ORDER BY pseudo_time_stamp;";
            _sqlCmd.Parameters.Add(new SqlParameter("@subjectID", subjectID));
            _sqlCmd.Parameters.Add(new SqlParameter("@studyID", studyID));
            _sqlCmd.Parameters.Add(new SqlParameter("@groupID", groupID));
            _sqlCmd.Parameters.Add(new SqlParameter("@targetID", targetID));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    var sqlAdapt = new SqlDataAdapter(_sqlCmd);
                    retVal = new DataSet();
                    sqlAdapt.Fill(retVal);
                    sqlAdapt.Dispose();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public DataSet GetBaselineDataSet(string studyName, string groupName, string szenarioName,
                                          SubjectInformationContainer subject)
        {
            DataSet retVal = null;

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.Text;

            _sqlCmd.CommandText =
                "SELECT * FROM getBaseLineData2(@studyName,@groupName,@szenarioName,@subjectID) ORDER BY pseudo_time_stamp;";
            _sqlCmd.Parameters.Add(new SqlParameter("@studyName", studyName));
            _sqlCmd.Parameters.Add(new SqlParameter("@groupName", groupName));
            _sqlCmd.Parameters.Add(new SqlParameter("@szenarioName", szenarioName));
            _sqlCmd.Parameters.Add(new SqlParameter("@subjectID", subject.ID));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    var sqlAdapt = new SqlDataAdapter(_sqlCmd);
                    retVal = new DataSet();
                    sqlAdapt.Fill(retVal);
                    sqlAdapt.Dispose();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public DataSet GetMeanTimeDataSet(string studyName, string groupName, string szenarioName,
                                          SubjectInformationContainer subject,
                                          DateTime turnDateTime)
        {
            DataSet retVal = null;

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.Text;

            _sqlCmd.CommandText =
                "SELECT * FROM getSzenarioMeanTimeData(@studyName,@groupName,@szenarioName,@subjectID,@turnDateTime);";
            _sqlCmd.Parameters.Add(new SqlParameter("@studyName", studyName));
            _sqlCmd.Parameters.Add(new SqlParameter("@groupName", groupName));
            _sqlCmd.Parameters.Add(new SqlParameter("@szenarioName", szenarioName));
            _sqlCmd.Parameters.Add(new SqlParameter("@subjectID", subject.ID));
            _sqlCmd.Parameters.Add(new SqlParameter("@turnDateTime", turnDateTime));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    var sqlAdapt = new SqlDataAdapter(_sqlCmd);
                    retVal = new DataSet();
                    sqlAdapt.Fill(retVal);
                    sqlAdapt.Dispose();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int InsertMeasureFile(DateTime creationTime, string measureFileHash)
        {
            int retVal = -1;

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.StoredProcedure;

            _sqlCmd.CommandText = "insertMeasureFile";
            _sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            _sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            _sqlCmd.Parameters.Add(new SqlParameter("@creationTime", creationTime));
            _sqlCmd.Parameters.Add(new SqlParameter("@fileHash", measureFileHash));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(_sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int InsertStudy(string studyName)
        {
            int retVal = -1;

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.StoredProcedure;

            _sqlCmd.CommandText = "insertStudy";
            _sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            _sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            _sqlCmd.Parameters.Add(new SqlParameter("@studyName", studyName));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(_sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int InsertSzenario(string szenarioName)
        {
            int retVal = -1;

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.StoredProcedure;

            _sqlCmd.CommandText = "insertSzenario";
            _sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            _sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            _sqlCmd.Parameters.Add(new SqlParameter("@szenarioName", szenarioName));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(_sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int InsertGroup(string groupName)
        {
            int retVal = -1;

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.StoredProcedure;

            _sqlCmd.CommandText = "insertGroup";
            _sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            _sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            _sqlCmd.Parameters.Add(new SqlParameter("@groupName", groupName));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(_sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int InsertSubject(string subjectName, string subjectID)
        {
            int retVal = -1;

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.StoredProcedure;

            _sqlCmd.CommandText = "insertSubject";
            _sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            _sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            _sqlCmd.Parameters.Add(new SqlParameter("@subjectName", subjectName));
            _sqlCmd.Parameters.Add(new SqlParameter("@subjectID", subjectID));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(_sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int InsertTarget(int targetNumber)
        {
            int retVal = -1;

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.StoredProcedure;

            _sqlCmd.CommandText = "insertTarget";
            _sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            _sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            _sqlCmd.Parameters.Add(new SqlParameter("@targetNumber", targetNumber));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(_sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int InsertTargetTrialNumber(int targetTrialNumber)
        {
            int retVal = -1;

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.StoredProcedure;

            _sqlCmd.CommandText = "insertTargetTrialNumber";
            _sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            _sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            _sqlCmd.Parameters.Add(new SqlParameter("@targetTrialNumber", targetTrialNumber));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(_sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;


                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int InsertSzenarioTrialNumber(int szenarioTrialNumber)
        {
            int retVal = -1;

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.StoredProcedure;

            _sqlCmd.CommandText = "insertSzenarioTrialNumber";
            _sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            _sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            _sqlCmd.Parameters.Add(new SqlParameter("@szenarioTrialNumber", szenarioTrialNumber));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(_sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;


                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int InsertIsCatchTrial(bool isCatchTrial)
        {
            int retVal = -1;

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.StoredProcedure;

            _sqlCmd.CommandText = "insertIsCatchTrial";
            _sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            _sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            _sqlCmd.Parameters.Add(new SqlParameter("@isCatchTrial", isCatchTrial));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(_sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int InsertTrialInformation(bool faultyTrial, bool isCatchTrial, bool isErrorclampTrial,
                                          int butterworthFilterOrder, int butterworthFilterFreqPosition, int butterworthFilterFreqForce, int velocityTrimThreshold)
        {
            int retVal = -1;

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.StoredProcedure;

            _sqlCmd.CommandText = "insertTrialInformation";
            _sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            _sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            _sqlCmd.Parameters.Add(new SqlParameter("@faultyTrial", faultyTrial));
            _sqlCmd.Parameters.Add(new SqlParameter("@isCatchTrial", isCatchTrial));
            _sqlCmd.Parameters.Add(new SqlParameter("@isErrorclampTrial", isErrorclampTrial));
            _sqlCmd.Parameters.Add(new SqlParameter("@butterworthFilterOrder", butterworthFilterOrder));
            _sqlCmd.Parameters.Add(new SqlParameter("@butterworthCutOffFreqPosition", butterworthFilterFreqPosition));
            _sqlCmd.Parameters.Add(new SqlParameter("@butterworthCutOffFreqForce", butterworthFilterFreqForce));
            _sqlCmd.Parameters.Add(new SqlParameter("@velocityTrimThreshold", velocityTrimThreshold));


            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(_sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;


                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        /*
        public int InsertMeasureDataRaw(
            int measureDataRawID,
            DateTime timeStamp,
            double forceActualX,
            double forceActualY,
            double forceActualZ,
            double forceNominalX,
            double forceNominalY,
            double forceNominalZ,
            double forceMomentX,
            double forceMomentY,
            double forceMomentZ,
            double positionCartesianX,
            double positionCartesianY,
            double positionCartesianZ,
            int positionStatus
            )
        {
            int retVal = -1;

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.StoredProcedure;

            _sqlCmd.CommandText = "insertMeasureDataRaw";
            _sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            _sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            _sqlCmd.Parameters.Add(new SqlParameter("@measureDataRawID", measureDataRawID));
            _sqlCmd.Parameters.Add(new SqlParameter("@timeStamp", timeStamp));
            _sqlCmd.Parameters.Add(new SqlParameter("@forceActualX", forceActualX));
            _sqlCmd.Parameters.Add(new SqlParameter("@forceActualY", forceActualY));
            _sqlCmd.Parameters.Add(new SqlParameter("@forceActualZ", forceActualZ));
            _sqlCmd.Parameters.Add(new SqlParameter("@forceNominalX", forceNominalX));
            _sqlCmd.Parameters.Add(new SqlParameter("@forceNominalY", forceNominalY));
            _sqlCmd.Parameters.Add(new SqlParameter("@forceNominalZ", forceNominalZ));
            _sqlCmd.Parameters.Add(new SqlParameter("@forceMomentX", forceMomentX));
            _sqlCmd.Parameters.Add(new SqlParameter("@forceMomentY", forceMomentY));
            _sqlCmd.Parameters.Add(new SqlParameter("@forceMomentZ", forceMomentZ));
            _sqlCmd.Parameters.Add(new SqlParameter("@positionCartesianX", positionCartesianX));
            _sqlCmd.Parameters.Add(new SqlParameter("@positionCartesianY", positionCartesianY));
            _sqlCmd.Parameters.Add(new SqlParameter("@positionCartesianZ", positionCartesianZ));
            _sqlCmd.Parameters.Add(new SqlParameter("@positionStatus", positionStatus));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(_sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;


                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }
        */
        /*
        public int InsertMeasureDataFiltered(
            int measureDataFilteredID,
            DateTime timeStamp,
            double forceActualX,
            double forceActualY,
            double forceActualZ,
            double forceNominalX,
            double forceNominalY,
            double forceNominalZ,
            double forceMomentX,
            double forceMomentY,
            double forceMomentZ,
            double positionCartesianX,
            double positionCartesianY,
            double positionCartesianZ,
            int positionStatus
            )
        {
            int retVal = -1;

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.StoredProcedure;

            _sqlCmd.CommandText = "insertMeasureDataFiltered";
            _sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            _sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            _sqlCmd.Parameters.Add(new SqlParameter("@measureDataFilteredID", measureDataFilteredID));
            _sqlCmd.Parameters.Add(new SqlParameter("@timeStamp", timeStamp));
            _sqlCmd.Parameters.Add(new SqlParameter("@forceActualX", forceActualX));
            _sqlCmd.Parameters.Add(new SqlParameter("@forceActualY", forceActualY));
            _sqlCmd.Parameters.Add(new SqlParameter("@forceActualZ", forceActualZ));
            _sqlCmd.Parameters.Add(new SqlParameter("@forceNominalX", forceNominalX));
            _sqlCmd.Parameters.Add(new SqlParameter("@forceNominalY", forceNominalY));
            _sqlCmd.Parameters.Add(new SqlParameter("@forceNominalZ", forceNominalZ));
            _sqlCmd.Parameters.Add(new SqlParameter("@forceMomentX", forceMomentX));
            _sqlCmd.Parameters.Add(new SqlParameter("@forceMomentY", forceMomentY));
            _sqlCmd.Parameters.Add(new SqlParameter("@forceMomentZ", forceMomentZ));
            _sqlCmd.Parameters.Add(new SqlParameter("@positionCartesianX", positionCartesianX));
            _sqlCmd.Parameters.Add(new SqlParameter("@positionCartesianY", positionCartesianY));
            _sqlCmd.Parameters.Add(new SqlParameter("@positionCartesianZ", positionCartesianZ));
            _sqlCmd.Parameters.Add(new SqlParameter("@positionStatus", positionStatus));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(_sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;


                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }
        */
        /*
        public int InsertMeasureDataNormalized(
            int measureDataNormalizedID,
            DateTime timeStamp,
            double forceActualX,
            double forceActualY,
            double forceActualZ,
            double forceNominalX,
            double forceNominalY,
            double forceNominalZ,
            double forceMomentX,
            double forceMomentY,
            double forceMomentZ,
            double positionCartesianX,
            double positionCartesianY,
            double positionCartesianZ,
            int positionStatus
            )
        {
            int retVal = -1;

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.StoredProcedure;

            _sqlCmd.CommandText = "insertMeasureDataNormalized";
            _sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            _sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            _sqlCmd.Parameters.Add(new SqlParameter("@measureDataNormalizedID", measureDataNormalizedID));
            _sqlCmd.Parameters.Add(new SqlParameter("@timeStamp", timeStamp));
            _sqlCmd.Parameters.Add(new SqlParameter("@forceActualX", forceActualX));
            _sqlCmd.Parameters.Add(new SqlParameter("@forceActualY", forceActualY));
            _sqlCmd.Parameters.Add(new SqlParameter("@forceActualZ", forceActualZ));
            _sqlCmd.Parameters.Add(new SqlParameter("@forceNominalX", forceNominalX));
            _sqlCmd.Parameters.Add(new SqlParameter("@forceNominalY", forceNominalY));
            _sqlCmd.Parameters.Add(new SqlParameter("@forceNominalZ", forceNominalZ));
            _sqlCmd.Parameters.Add(new SqlParameter("@forceMomentX", forceMomentX));
            _sqlCmd.Parameters.Add(new SqlParameter("@forceMomentY", forceMomentY));
            _sqlCmd.Parameters.Add(new SqlParameter("@forceMomentZ", forceMomentZ));
            _sqlCmd.Parameters.Add(new SqlParameter("@positionCartesianX", positionCartesianX));
            _sqlCmd.Parameters.Add(new SqlParameter("@positionCartesianY", positionCartesianY));
            _sqlCmd.Parameters.Add(new SqlParameter("@positionCartesianZ", positionCartesianZ));
            _sqlCmd.Parameters.Add(new SqlParameter("@positionStatus", positionStatus));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(_sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;


                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }
        */
        /*
        public int InsertVelocityDataFiltered(
            int velocityDataFilteredID,
            DateTime timeStamp,
            double velocityX,
            double velocityY,
            double velocityZ
            )
        {
            int retVal = -1;

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.StoredProcedure;

            _sqlCmd.CommandText = "insertVelocityDataFiltered";
            _sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            _sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            _sqlCmd.Parameters.Add(new SqlParameter("@velocityDataFilteredID", velocityDataFilteredID));
            _sqlCmd.Parameters.Add(new SqlParameter("@timeStamp", timeStamp));
            _sqlCmd.Parameters.Add(new SqlParameter("@velocityX", velocityX));
            _sqlCmd.Parameters.Add(new SqlParameter("@velocityY", velocityY));
            _sqlCmd.Parameters.Add(new SqlParameter("@velocityZ", velocityZ));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(_sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;


                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }
        */
        /*
        public int InsertVelocityDataNormalized(
            int velocityDataNormalizedID,
            DateTime timeStamp,
            double velocityX,
            double velocityY,
            double velocityZ
            )
        {
            int retVal = -1;

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.StoredProcedure;

            _sqlCmd.CommandText = "insertVelocityDataNormalized";
            _sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            _sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            _sqlCmd.Parameters.Add(new SqlParameter("@velocityDataNormalizedID", velocityDataNormalizedID));
            _sqlCmd.Parameters.Add(new SqlParameter("@timeStamp", timeStamp));
            _sqlCmd.Parameters.Add(new SqlParameter("@velocityX", velocityX));
            _sqlCmd.Parameters.Add(new SqlParameter("@velocityY", velocityY));
            _sqlCmd.Parameters.Add(new SqlParameter("@velocityZ", velocityZ));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(_sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;


                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }
        */

        public int InsertStatisticData(
            int trialID,
            double velocityVectorCorrelation,
            double velocityVectorCorrelationFisherZ,
            double trajectoryLengthAbs,
            double trajectoryLengthRatio,
            double perpendicularDisplacement300MsAbs,
            double maximalPerpendicularDisplacementAbs,
            double meanPerpendicularDisplacementAbs,
            double perpendicularDisplacement300MsSign,
            double maximalPerpendicularDisplacementSign,
            double enclosedArea,
            double rmse
            )
        {
            int retVal = -1;

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.StoredProcedure;

            _sqlCmd.CommandText = "insertStatisticData";
            _sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            _sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            _sqlCmd.Parameters.Add(new SqlParameter("@trialID", trialID));
            _sqlCmd.Parameters.Add(new SqlParameter("@velocityVectorCorrelation", velocityVectorCorrelation));
            _sqlCmd.Parameters.Add(new SqlParameter("@velocityVectorCorrelationFisherZ", velocityVectorCorrelationFisherZ));
            _sqlCmd.Parameters.Add(new SqlParameter("@trajectoryLengthAbs", trajectoryLengthAbs));
            _sqlCmd.Parameters.Add(new SqlParameter("@trajectoryLengthRatioBaseline", trajectoryLengthRatio));
            _sqlCmd.Parameters.Add(new SqlParameter("@perpendicularDisplacement300msAbs",
                                                    perpendicularDisplacement300MsAbs));
            _sqlCmd.Parameters.Add(new SqlParameter("@maximalPerpendicularDisplacementAbs",
                                                    maximalPerpendicularDisplacementAbs));
            _sqlCmd.Parameters.Add(new SqlParameter("@meanPerpendicularDisplacementAbs",
                                                    meanPerpendicularDisplacementAbs));
            _sqlCmd.Parameters.Add(new SqlParameter("@perpendicularDisplacement300msSign",
                                                    perpendicularDisplacement300MsSign));
            _sqlCmd.Parameters.Add(new SqlParameter("@maximalPerpendicularDisplacementSign",
                                                    maximalPerpendicularDisplacementSign));
            _sqlCmd.Parameters.Add(new SqlParameter("@enclosedArea", enclosedArea));
            _sqlCmd.Parameters.Add(new SqlParameter("@rmse", rmse));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(_sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;


                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int InsertTrial(
            int subjectID,
            int studyID,
            int groupID,
            int szenarioID,
            int targetID,
            int targetTrialNumberID,
            int szenarioTrialNumberID,
            int measureFileID,
            int trialInformationID
            )
        {
            int retVal = -1;

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.StoredProcedure;

            _sqlCmd.CommandText = "insertTrial";
            _sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            _sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            _sqlCmd.Parameters.Add(new SqlParameter("@subjectID", subjectID));
            _sqlCmd.Parameters.Add(new SqlParameter("@studyID", studyID));
            _sqlCmd.Parameters.Add(new SqlParameter("@groupID", groupID));
            _sqlCmd.Parameters.Add(new SqlParameter("@szenarioID", szenarioID));
            _sqlCmd.Parameters.Add(new SqlParameter("@targetID", targetID));
            _sqlCmd.Parameters.Add(new SqlParameter("@targetTrialNumberID", targetTrialNumberID));
            _sqlCmd.Parameters.Add(new SqlParameter("@szenarioTrialNumberID", szenarioTrialNumberID));
            _sqlCmd.Parameters.Add(new SqlParameter("@measureFileID", measureFileID));
            _sqlCmd.Parameters.Add(new SqlParameter("@trialInformationID", trialInformationID));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(_sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;


                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int InsertSzenarioMeanTimeData(int szenarioMeanTimeID, TimeSpan szenarioMeanTime,
                                              TimeSpan szenarioMeanTimeStd)
        {
            int retVal = -1;

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.StoredProcedure;

            _sqlCmd.CommandText = "insertSzenarioMeanTimeData";
            _sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            _sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            _sqlCmd.Parameters.Add(new SqlParameter("@szenarioMeanTimeID", szenarioMeanTimeID));
            _sqlCmd.Parameters.Add(new SqlParameter("@szenarioMeanTime", szenarioMeanTime));
            _sqlCmd.Parameters.Add(new SqlParameter("@szenarioMeanTimeStd", szenarioMeanTimeStd));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(_sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;


                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int InsertSzenarioMeanTime(
            int subjectID,
            int studyID,
            int groupID,
            int targetID,
            int szenarioID,
            int measureFileID
            )
        {
            int retVal = -1;

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.StoredProcedure;

            _sqlCmd.CommandText = "insertSzenarioMeanTime";
            _sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            _sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            _sqlCmd.Parameters.Add(new SqlParameter("@subjectID", subjectID));
            _sqlCmd.Parameters.Add(new SqlParameter("@studyID", studyID));
            _sqlCmd.Parameters.Add(new SqlParameter("@groupID", groupID));
            _sqlCmd.Parameters.Add(new SqlParameter("@targetID", targetID));
            _sqlCmd.Parameters.Add(new SqlParameter("@szenarioID", szenarioID));
            _sqlCmd.Parameters.Add(new SqlParameter("@measureFileID", measureFileID));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(_sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;


                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int InsertBaselineData(
            int baselineID,
            DateTime pseudoTimeStamp,
            double baselinePositionCartesianX,
            double baselinePositionCartesianY,
            double baselinePositionCartesianZ,
            double baselineVelocityX,
            double baselineVelocityY,
            double baselineVelocityZ
            )
        {
            int retVal = -1;

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.StoredProcedure;

            _sqlCmd.CommandText = "insertBaselineData";
            _sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            _sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            _sqlCmd.Parameters.Add(new SqlParameter("@baselineID", baselineID));
            _sqlCmd.Parameters.Add(new SqlParameter("@pseudoTimeStamp", pseudoTimeStamp));
            _sqlCmd.Parameters.Add(new SqlParameter("@baselinePositionCartesianX", baselinePositionCartesianX));
            _sqlCmd.Parameters.Add(new SqlParameter("baselinePositionCartesianY", baselinePositionCartesianY));
            _sqlCmd.Parameters.Add(new SqlParameter("baselinePositionCartesianZ", baselinePositionCartesianZ));
            _sqlCmd.Parameters.Add(new SqlParameter("baselineVelocityX", baselineVelocityX));
            _sqlCmd.Parameters.Add(new SqlParameter("baselineVelocityY", baselineVelocityY));
            _sqlCmd.Parameters.Add(new SqlParameter("baselineVelocityZ", baselineVelocityZ));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(_sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;


                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int InsertBaseline(
            int subjectID,
            int studyID,
            int groupID,
            int targetID,
            int szenarioID,
            int measureFileID
            )
        {
            int retVal = -1;

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.StoredProcedure;

            _sqlCmd.CommandText = "insertBaseline";
            _sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            _sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            _sqlCmd.Parameters.Add(new SqlParameter("@subjectID", subjectID));
            _sqlCmd.Parameters.Add(new SqlParameter("@studyID", studyID));
            _sqlCmd.Parameters.Add(new SqlParameter("@groupID", groupID));
            _sqlCmd.Parameters.Add(new SqlParameter("@targetID", targetID));
            _sqlCmd.Parameters.Add(new SqlParameter("@szenarioID", szenarioID));
            _sqlCmd.Parameters.Add(new SqlParameter("@measureFileID", measureFileID));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(_sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;


                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public void BulkInsertMeasureDataRaw(string filename)
        {
            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.Text;

            _sqlCmd.CommandText = "BULK INSERT dbo._measure_data_raw FROM '" + filename +
                                  "' WITH  ( FIELDTERMINATOR =',', ROWTERMINATOR ='\r\n' )";

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;


                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }
        }

        public void BulkInsertMeasureDataFiltered(string filename)
        {
            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.Text;

            _sqlCmd.CommandText = "BULK INSERT dbo._measure_data_filtered FROM '" + filename +
                                  "' WITH  ( FIELDTERMINATOR =',', ROWTERMINATOR ='\r\n' )";

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;


                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }
        }

        public void BulkInsertMeasureDataNormalized(string filename)
        {
            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.Text;

            _sqlCmd.CommandText = "BULK INSERT dbo._measure_data_normalized FROM '" + filename +
                                  "' WITH  ( FIELDTERMINATOR =',', ROWTERMINATOR ='\r\n' )";

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;


                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }
        }

        public void BulkInsertVelocityDataFiltered(string filename)
        {
            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.Text;

            _sqlCmd.CommandText = "BULK INSERT dbo._velocity_data_filtered FROM '" + filename +
                                  "' WITH  ( FIELDTERMINATOR =',', ROWTERMINATOR ='\r\n' )";

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;


                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }
        }

        public void BulkInsertVelocityDataNormalized(string filename)
        {
            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.Text;

            _sqlCmd.CommandText = "BULK INSERT dbo._velocity_data_normalized FROM '" + filename +
                                  "' WITH  ( FIELDTERMINATOR =',', ROWTERMINATOR ='\r\n' )";

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;


                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }
        }

        public bool CheckIfMeasureFileHashExists(string measureFileHash)
        {
            bool retVal = false;

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.StoredProcedure;

            _sqlCmd.CommandText = "checkMeasureFileHash";
            _sqlCmd.Parameters.Add("@hashExists", SqlDbType.Bit);
            _sqlCmd.Parameters["@hashExists"].Direction = ParameterDirection.Output;

            _sqlCmd.Parameters.Add(new SqlParameter("@measureFileHash", measureFileHash));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    _sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToBoolean(_sqlCmd.Parameters["@hashExists"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;


                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public string[] GetStudyNames()
        {
            string[] retVal = null;

            _sqlCmd.Parameters.Clear();
            _sqlCmd.CommandType = CommandType.Text;
            _sqlCmd.CommandText = "SELECT * FROM getStudyNames();";

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    SqlDataReader sqlRdr = _sqlCmd.ExecuteReader();

                    if (sqlRdr.HasRows)
                    {
                        var rows = new List<string>();
                        while (sqlRdr.Read())
                        {
                            rows.Add(sqlRdr.GetString(0));
                        }
                        sqlRdr.Close();
                        retVal = rows.ToArray();
                    }
                    else
                    {
                        sqlRdr.Close();
                    }
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;


                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public string[] GetGroupNames(string studyName)
        {
            string[] retVal = null;

            _sqlCmd.Parameters.Clear();
            _sqlCmd.CommandType = CommandType.Text;
            _sqlCmd.CommandText = "SELECT * FROM getGroupNames(@studyName);";
            _sqlCmd.Parameters.Add(new SqlParameter("@studyName", studyName));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    SqlDataReader sqlRdr = _sqlCmd.ExecuteReader();

                    if (sqlRdr.HasRows)
                    {
                        var rows = new List<string>();
                        while (sqlRdr.Read())
                        {
                            rows.Add(sqlRdr.GetString(0));
                        }
                        sqlRdr.Close();
                        retVal = rows.ToArray();
                    }
                    else
                    {
                        sqlRdr.Close();
                    }
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;


                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public string[] GetSzenarioNames(string studyName, string groupName)
        {
            string[] retVal = null;

            _sqlCmd.Parameters.Clear();
            _sqlCmd.CommandType = CommandType.Text;
            _sqlCmd.CommandText = "SELECT * FROM getSzenarioNames(@studyName,@groupName);";
            _sqlCmd.Parameters.Add(new SqlParameter("@studyName", studyName));
            _sqlCmd.Parameters.Add(new SqlParameter("@groupName", groupName));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    SqlDataReader sqlRdr = _sqlCmd.ExecuteReader();

                    if (sqlRdr.HasRows)
                    {
                        var rows = new List<string>();
                        while (sqlRdr.Read())
                        {
                            rows.Add(sqlRdr.GetString(0));
                        }
                        sqlRdr.Close();
                        retVal = rows.ToArray();
                    }
                    else
                    {
                        sqlRdr.Close();
                    }
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;


                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public SubjectInformationContainer[] GetSubjectInformations(string studyName, string groupName,
                                                                    string szenarioName)
        {
            SubjectInformationContainer[] retVal = null;

            _sqlCmd.Parameters.Clear();
            _sqlCmd.CommandType = CommandType.Text;
            _sqlCmd.CommandText = "SELECT * FROM getSubjectInformations(@studyName,@groupName,@szenarioName);";
            _sqlCmd.Parameters.Add(new SqlParameter("@studyName", studyName));
            _sqlCmd.Parameters.Add(new SqlParameter("@groupName", groupName));
            _sqlCmd.Parameters.Add(new SqlParameter("@szenarioName", szenarioName));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    SqlDataReader sqlRdr = _sqlCmd.ExecuteReader();

                    if (sqlRdr.HasRows)
                    {
                        var rows = new List<SubjectInformationContainer>();
                        while (sqlRdr.Read())
                        {
                            rows.Add(new SubjectInformationContainer(sqlRdr.GetInt32(0), sqlRdr.GetString(1),
                                                                     sqlRdr.GetString(2)));
                        }
                        sqlRdr.Close();
                        retVal = rows.ToArray();
                    }
                    else
                    {
                        sqlRdr.Close();
                    }
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;


                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public string[] GetTurns(string studyName, string groupName, string szenarioName,
                                 SubjectInformationContainer subject)
        {
            string[] retVal = null;

            _sqlCmd.Parameters.Clear();
            _sqlCmd.CommandType = CommandType.Text;

            _sqlCmd.CommandText = "SELECT * FROM getTurns(@studyName,@groupName,@szenarioName,@subjectID)";

            _sqlCmd.Parameters.Add(new SqlParameter("@studyName", studyName));
            _sqlCmd.Parameters.Add(new SqlParameter("@groupName", groupName));
            _sqlCmd.Parameters.Add(new SqlParameter("@szenarioName", szenarioName));
            _sqlCmd.Parameters.Add(new SqlParameter("@subjectID", subject.ID));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    SqlDataReader sqlRdr = _sqlCmd.ExecuteReader();

                    if (sqlRdr.HasRows)
                    {
                        var rows = new List<string>();
                        while (sqlRdr.Read())
                        {
                            rows.Add(sqlRdr.GetString(0));
                        }
                        sqlRdr.Close();
                        retVal = rows.ToArray();
                    }
                    else
                    {
                        sqlRdr.Close();
                    }
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;


                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public string[] GetSzenarioTrials(string studyName, string szenarioName, bool showCatchTrials,
                                          bool showCatchTrialsExclusivly, bool showErrorclampTrials,
                                          bool showErrorclampTrialsExclusivly)
        {
            string[] retVal = null;

            _sqlCmd.Parameters.Clear();
            _sqlCmd.CommandType = CommandType.Text;
            _sqlCmd.CommandText =
                "SELECT * FROM getSzenarioTrials(@studyName,@szenarioName,@showCatchTrials,@showCatchTrialsExclusivly,@showErrorclampTrials,@showErrorclampTrialsExclusivly)";

            _sqlCmd.Parameters.Add(new SqlParameter("@studyName", studyName));
            _sqlCmd.Parameters.Add(new SqlParameter("@szenarioName", szenarioName));
            _sqlCmd.Parameters.Add(new SqlParameter("@showCatchTrials", showCatchTrials));
            _sqlCmd.Parameters.Add(new SqlParameter("@showCatchTrialsExclusivly", showCatchTrialsExclusivly));
            _sqlCmd.Parameters.Add(new SqlParameter("@showErrorclampTrials", showErrorclampTrials));
            _sqlCmd.Parameters.Add(new SqlParameter("@showErrorclampTrialsExclusivly", showErrorclampTrialsExclusivly));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    SqlDataReader sqlRdr = _sqlCmd.ExecuteReader();

                    if (sqlRdr.HasRows)
                    {
                        var rows = new List<string>();
                        while (sqlRdr.Read())
                        {
                            rows.Add(sqlRdr.GetString(0));
                        }
                        sqlRdr.Close();
                        retVal = rows.ToArray();
                    }
                    else
                    {
                        sqlRdr.Close();
                    }
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;


                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public IEnumerable<string> GetTargets(string studyName, string szenarioName)
        {
            string[] retVal = null;

            _sqlCmd.Parameters.Clear();
            _sqlCmd.CommandType = CommandType.Text;
            _sqlCmd.CommandText = "SELECT * FROM getTargets(@studyName,@szenarioName)";

            _sqlCmd.Parameters.Add(new SqlParameter("@studyName", studyName));
            _sqlCmd.Parameters.Add(new SqlParameter("@szenarioName", szenarioName));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    SqlDataReader sqlRdr;
                    using (sqlRdr = _sqlCmd.ExecuteReader())
                    {
                        if (sqlRdr.HasRows)
                        {
                            var rows = new List<string>();
                            while (sqlRdr.Read())
                            {
                                rows.Add(sqlRdr.GetString(0));
                            }
                            retVal = rows.ToArray();
                        }
                    }
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;


                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public IEnumerable<string> GetTrials(string studyName, string szenarioName)
        {
            string[] retVal = null;

            _sqlCmd.Parameters.Clear();

            _sqlCmd.CommandType = CommandType.Text;

            _sqlCmd.CommandText = "SELECT * FROM  getTrials(@studyName,@szenarioName)";

            _sqlCmd.Parameters.Add(new SqlParameter("@studyName", studyName));
            _sqlCmd.Parameters.Add(new SqlParameter("@szenarioName", szenarioName));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    OpenSqlConnection();
                    SqlDataReader sqlRdr = _sqlCmd.ExecuteReader();

                    if (sqlRdr.HasRows)
                    {
                        var rows = new List<string>();
                        while (sqlRdr.Read())
                        {
                            rows.Add(sqlRdr.GetString(0));
                        }
                        sqlRdr.Close();
                        retVal = rows.ToArray();
                    }
                    else
                    {
                        sqlRdr.Close();
                    }
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        const MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                        DialogResult result = MessageBox.Show(@"Tried to execute SQL command 5 times, try another 5?",
                                                              @"Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }
    }
}