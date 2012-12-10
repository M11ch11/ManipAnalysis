using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Net;

namespace ManipAnalysis
{
    public class SqlWrapper
    {
        SqlConnection sqlCon;
        SqlCommand sqlCmd;
        ManipAnalysis mainWindow;

        string SQL_server;
        string SQL_database;
        string SQL_username;
        string SQL_password;

        public SqlWrapper(ManipAnalysis _mainWindow)
        {
            mainWindow = _mainWindow;

            SQL_server = "localhost";
            SQL_database = "master";
            SQL_username = "DataAccess";
            SQL_password = "!sport12";           
        }

        private void setConnectionString(string _serverURI, string _database, string _username, string _password)
        {
            SQL_server = _serverURI;
            SQL_database = _database;
            SQL_username = _username;
            SQL_password = _password;            

            sqlCon = new SqlConnection(@"Data Source=" + SQL_server + ";Initial Catalog=" + SQL_database + ";User Id=" + SQL_username + ";Password=" + SQL_password);

            sqlCmd = new SqlCommand();
            sqlCmd.Connection = sqlCon;
            sqlCmd.CommandTimeout = 600;
        }

        public void setSqlServer(string _serverURI)
        {
            closeSqlConnection();
            setConnectionString(_serverURI, SQL_database, SQL_username, SQL_password);
        }

        public void setDatabase(string _database)
        {
            openSqlConnection();
            sqlCon.ChangeDatabase(_database);
        }

        public bool openSqlConnection()
        {
            bool _isOpen = true;

            if (sqlCon.State == ConnectionState.Closed)
            {
                _isOpen = false;

                mainWindow.writeProgressInfo("Opening SQL-Connection...");

                try
                {
                    sqlCon.Open();
                    _isOpen = true;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    _isOpen = false;
                }


                if (_isOpen)
                {
                    mainWindow.writeProgressInfo("Ready.");
                }
                else
                {
                    mainWindow.writeProgressInfo("SQL-Connection failed!");
                }
            }
            return _isOpen;
        }

        public void closeSqlConnection()
        {
            if ( (sqlCon != null ) && (sqlCon.State == ConnectionState.Open) )
            {
                try
                {
                    sqlCon.Close();
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                }
            }
        }

        public string[] getDatabases()
        {
            string[] retVal = null;

            sqlCmd.Parameters.Clear();
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = "SELECT name FROM master..sysdatabases WHERE name LIKE 'ManipData%';";

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    SqlDataReader SqlRdr = sqlCmd.ExecuteReader();

                    if (SqlRdr.HasRows)
                    {
                        List<string> rows = new List<string>();
                        while (SqlRdr.Read())
                        {
                            rows.Add(SqlRdr.GetString(0));
                        }
                        SqlRdr.Close();
                        retVal = rows.ToArray();
                    }
                    else
                    {
                        SqlRdr.Close();
                    }
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public void executeSqlFile(string filename)
        {
            sqlCmd.Parameters.Clear();
            FileInfo file = new FileInfo(filename);
            string script = file.OpenText().ReadToEnd();
            sqlCmd.CommandText = script;

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }
        }

        public void cleanOrphanedEntries()
        {
            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlCmd.CommandText = "cleanOrphanedEntries";

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }
        }

        public void deleteMeasureFile(int _measureFileID)
        {
            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlCmd.CommandText = "deleteMeasureFile";
            sqlCmd.Parameters.Add(new SqlParameter("@measureFileID", _measureFileID));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }
        }

        public void changeGroupID(int _oldGroupID, int _newGroupID)
        {
            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlCmd.CommandText = "changeGroupID";
            sqlCmd.Parameters.Add(new SqlParameter("@oldGroupID", _oldGroupID));
            sqlCmd.Parameters.Add(new SqlParameter("@newGroupID", _newGroupID));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }
        }

        public void changeSubjectID(int _oldSubjectID, int _newSubjectID)
        {
            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlCmd.CommandText = "changeSubjectID";
            sqlCmd.Parameters.Add(new SqlParameter("@oldSubjectID", _oldSubjectID));
            sqlCmd.Parameters.Add(new SqlParameter("@newSubjectID", _newSubjectID));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }
        }

        public void changeSubjectName(int _subjectID, string _newSubjectName)
        {
            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlCmd.CommandText = "changeSubjectName";
            sqlCmd.Parameters.Add(new SqlParameter("@subjectID", _subjectID));
            sqlCmd.Parameters.Add(new SqlParameter("@newSubjectName", _newSubjectName));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }
        }

        public void changeGroupName(int _groupID, string _newGrouptName)
        {
            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlCmd.CommandText = "changeGroupName";
            sqlCmd.Parameters.Add(new SqlParameter("@groupID", _groupID));
            sqlCmd.Parameters.Add(new SqlParameter("@newGroupName", _newGrouptName));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }
        }

        public List<int[]> getStatisticCalculationInformation()
        {
            List<int[]> retVal = null;

            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.Text;

            sqlCmd.CommandText = "SELECT * FROM getStatisticCalculationInformation()";

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    SqlDataReader SqlRdr = sqlCmd.ExecuteReader();

                    if (SqlRdr.HasRows)
                    {
                        retVal = new List<int[]>();
                        
                        while (SqlRdr.Read())
                        {
                            bool hasNullValues = false;
                            for(int i = 0; i < 6; i++)
                            {
                                if (SqlRdr.IsDBNull(i))
                                {
                                    if (i > 0)
                                    {
                                        Logger.writeToLog("Trial " + SqlRdr.GetInt32(0) + " has NULL values as id.");
                                    }
                                    else
                                    {
                                        Logger.writeToLog("There are NULL values.");
                                    }
                                    hasNullValues = true;
                                }
                            }

                            if (!hasNullValues)
                            {
                                retVal.Add(new int[]  {
                                                    SqlRdr.GetInt32(0), //trial_id
                                                    SqlRdr.GetInt32(1), //subject_id
                                                    SqlRdr.GetInt32(2), //study_id
                                                    SqlRdr.GetInt32(3), //group_id
                                                    SqlRdr.GetInt32(4),  //target_id
                                                    SqlRdr.GetInt32(5)  //target_number
                                                   });
                            }
                        }
                        SqlRdr.Close();
                    }
                    else
                    {
                        SqlRdr.Close();
                    }
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    sqlCmd.Cancel();
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;                            
                        }
                    }
                }
            }

            return retVal;
        }

        public List<object[]> getFaultyTrialInformation()
        {
            List<object[]> retVal = null;

            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.Text;

            sqlCmd.CommandText = "SELECT * FROM getFaultyTrialInformation()";

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    SqlDataReader SqlRdr = sqlCmd.ExecuteReader();

                    if (SqlRdr.HasRows)
                    {
                        retVal = new List<object[]>();
                        while (SqlRdr.Read())
                        {
                            retVal.Add(new object[] {
                                                        SqlRdr.GetInt32(0),     //trial_id
                                                        SqlRdr.GetInt32(1),     //measure_file_id
                                                        SqlRdr.GetString(2),    //study_name
                                                        SqlRdr.GetString(3),    //group_name
                                                        SqlRdr.GetInt32(4),    //subject_id
                                                        SqlRdr.GetString(5),    //szenario_name
                                                        SqlRdr.GetDateTime(6),  //measure_file_creation_time
                                                        SqlRdr.GetInt32(7)      //szenario_trial_number
                                                    });
                        }
                        SqlRdr.Close();
                    }
                    else
                    {
                        SqlRdr.Close();
                    }
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int[] getFaultyTrialFixInformation(int _measureFileID, int _szenarioTrialNumber)
        {
            int[] retVal = null;

            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlCmd.CommandText = "getFaultyTrialFixInformation";

            sqlCmd.Parameters.Add("@upperTrialID", SqlDbType.Int);
            sqlCmd.Parameters.Add("@lowerTrialID", SqlDbType.Int);
            sqlCmd.Parameters.Add(new SqlParameter("@measureFileID", _measureFileID));
            sqlCmd.Parameters.Add(new SqlParameter("@szenarioTrialNumber", _szenarioTrialNumber));

            sqlCmd.Parameters["@upperTrialID"].Direction = ParameterDirection.Output;
            sqlCmd.Parameters["@lowerTrialID"].Direction = ParameterDirection.Output;

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    retVal = new int[]  {
                                            Convert.ToInt32(sqlCmd.Parameters["@upperTrialID"].Value),
                                            Convert.ToInt32(sqlCmd.Parameters["@lowerTrialID"].Value)
                                        };
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;

        }

        public DateTime getTurnDateTime(string _study, string _group, string _szenario, int _subjectID, int _turn)
        {
            DateTime retVal = new DateTime();

            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlCmd.CommandText = "getTurnDateTime";

            sqlCmd.Parameters.Add("@turnDateTime", SqlDbType.DateTime2);
            sqlCmd.Parameters.Add(new SqlParameter("@studyName", _study));
            sqlCmd.Parameters.Add(new SqlParameter("@groupName", _group));
            sqlCmd.Parameters.Add(new SqlParameter("@szenarioName", _szenario));
            sqlCmd.Parameters.Add(new SqlParameter("@subjectID", _subjectID));
            sqlCmd.Parameters.Add(new SqlParameter("@turn", _turn));

            sqlCmd.Parameters["@turnDateTime"].Direction = ParameterDirection.Output;

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToDateTime(sqlCmd.Parameters["@turnDateTime"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public DataSet getMeasureDataNormalizedDataSet(int _trialID)
        {
            DataSet retVal = null;

            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.Text;

            sqlCmd.CommandText = "SELECT * FROM getMeasureDataNormalizedData(@trialID) ORDER BY time_stamp;";
            sqlCmd.Parameters.Add(new SqlParameter("@trialID", _trialID));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    SqlDataAdapter sqlAdapt = new SqlDataAdapter(sqlCmd);
                    retVal = new DataSet();
                    sqlAdapt.Fill(retVal);
                    sqlAdapt.Dispose();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int getTrailID(string _study, string _group, string _szenario, int _subjectID, DateTime _turnDateTime, int _target, int _trial)
        {
            int retVal = -1;

            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlCmd.CommandText = "getTrialID";

            sqlCmd.Parameters.Add(new SqlParameter("@studyName", _study));
            sqlCmd.Parameters.Add(new SqlParameter("@groupName", _group));
            sqlCmd.Parameters.Add(new SqlParameter("@szenarioName", _szenario));
            sqlCmd.Parameters.Add(new SqlParameter("@subjectID", _subjectID));
            sqlCmd.Parameters.Add(new SqlParameter("@turnDateTime", _turnDateTime));
            sqlCmd.Parameters.Add(new SqlParameter("@target", _target));
            sqlCmd.Parameters.Add(new SqlParameter("@trial", _trial));
            sqlCmd.Parameters.Add("@id", SqlDbType.Int);

            sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public DataSet getVelocityDataNormalizedDataSet(int _trialID)
        {
            DataSet retVal = null;

            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.Text;

            sqlCmd.CommandText = "SELECT * FROM getVelocityDataNormalizedData(@trialID) ORDER BY time_stamp;";
            sqlCmd.Parameters.Add(new SqlParameter("@trialID", _trialID));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    SqlDataAdapter sqlAdapt = new SqlDataAdapter(sqlCmd);
                    retVal = new DataSet();
                    sqlAdapt.Fill(retVal);
                    sqlAdapt.Dispose();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public DataSet getStatisticDataSet(int _trialID)
        {
            DataSet retVal = null;

            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.Text;

            sqlCmd.CommandText = "SELECT * FROM getStatisticData(@trialID);";
            sqlCmd.Parameters.Add(new SqlParameter("@trialID", _trialID));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    SqlDataAdapter sqlAdapt = new SqlDataAdapter(sqlCmd);
                    retVal = new DataSet();
                    sqlAdapt.Fill(retVal);
                    sqlAdapt.Dispose();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public DataSet getStatisticDataSet(string _studyName, string _groupName, string _szenarioName, int _subjectID, DateTime _turn)
        {
            DataSet retVal = null;

            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.Text;

            sqlCmd.CommandText = "SELECT * FROM getStatisticData2(@studyName,@groupName,@szenarioName,@subjectID,@turnDateTime);";
            sqlCmd.Parameters.Add(new SqlParameter("@studyName", _studyName));
            sqlCmd.Parameters.Add(new SqlParameter("@groupName", _groupName));
            sqlCmd.Parameters.Add(new SqlParameter("@szenarioName", _szenarioName));
            sqlCmd.Parameters.Add(new SqlParameter("@subjectID", _subjectID));
            sqlCmd.Parameters.Add(new SqlParameter("@turnDateTime", _turn));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    SqlDataAdapter sqlAdapt = new SqlDataAdapter(sqlCmd);
                    retVal = new DataSet();
                    sqlAdapt.Fill(retVal);
                    sqlAdapt.Dispose();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public DataSet getBaselineDataSet(int _subjectID, int _studyID, int _groupID, int _targetID)
        {
            DataSet retVal = null;

            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.Text;

            sqlCmd.CommandText = "SELECT * FROM getBaseLineData(@subjectID,@studyID,@groupID,@targetID) ORDER BY pseudo_time_stamp;";
            sqlCmd.Parameters.Add(new SqlParameter("@subjectID", _subjectID));
            sqlCmd.Parameters.Add(new SqlParameter("@studyID", _studyID));
            sqlCmd.Parameters.Add(new SqlParameter("@groupID", _groupID));
            sqlCmd.Parameters.Add(new SqlParameter("@targetID", _targetID));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    SqlDataAdapter sqlAdapt = new SqlDataAdapter(sqlCmd);
                    retVal = new DataSet();
                    sqlAdapt.Fill(retVal);
                    sqlAdapt.Dispose();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public DataSet getBaselineDataSet(string _studyName, string _groupName, string _szenarioName, int _subjectID)
        {
            DataSet retVal = null;

            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.Text;

            sqlCmd.CommandText = "SELECT * FROM getBaseLineData2(@studyName,@groupName,@szenarioName,@subjectID) ORDER BY pseudo_time_stamp;";
            sqlCmd.Parameters.Add(new SqlParameter("@studyName", _studyName));
            sqlCmd.Parameters.Add(new SqlParameter("@groupName", _groupName));
            sqlCmd.Parameters.Add(new SqlParameter("@szenarioName", _szenarioName));
            sqlCmd.Parameters.Add(new SqlParameter("@subjectID", _subjectID));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    SqlDataAdapter sqlAdapt = new SqlDataAdapter(sqlCmd);
                    retVal = new DataSet();
                    sqlAdapt.Fill(retVal);
                    sqlAdapt.Dispose();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public DataSet getMeanTimeDataSet(string _studyName, string _groupName, string _szenarioName, int _subjectID, DateTime _turnDateTime)
        {
            DataSet retVal = null;

            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.Text;

            sqlCmd.CommandText = "SELECT * FROM getSzenarioMeanTimeData(@studyName,@groupName,@szenarioName,@subjectID,@turnDateTime);";
            sqlCmd.Parameters.Add(new SqlParameter("@studyName", _studyName));
            sqlCmd.Parameters.Add(new SqlParameter("@groupName", _groupName));
            sqlCmd.Parameters.Add(new SqlParameter("@szenarioName", _szenarioName));
            sqlCmd.Parameters.Add(new SqlParameter("@subjectID", _subjectID));
            sqlCmd.Parameters.Add(new SqlParameter("@turnDateTime", _turnDateTime));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    SqlDataAdapter sqlAdapt = new SqlDataAdapter(sqlCmd);
                    retVal = new DataSet();
                    sqlAdapt.Fill(retVal);
                    sqlAdapt.Dispose();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int insertMeasureFile(DateTime _creationTime, string _measureFileHash)
        {
            int retVal = -1;

            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlCmd.CommandText = "insertMeasureFile";
            sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            sqlCmd.Parameters.Add(new SqlParameter("@creationTime", _creationTime));
            sqlCmd.Parameters.Add(new SqlParameter("@fileHash", _measureFileHash));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int insertStudy(string _studyName)
        {
            int retVal = -1;

            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlCmd.CommandText = "insertStudy";
            sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            sqlCmd.Parameters.Add(new SqlParameter("@studyName", _studyName));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int insertSzenario(string _szenarioName)
        {
            int retVal = -1;

            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlCmd.CommandText = "insertSzenario";
            sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            sqlCmd.Parameters.Add(new SqlParameter("@szenarioName", _szenarioName));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int insertGroup(string _groupName)
        {
            int retVal = -1;

            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlCmd.CommandText = "insertGroup";
            sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            sqlCmd.Parameters.Add(new SqlParameter("@groupName", _groupName));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int insertSubject(string _subjectName, string _subjectID)
        {
            int retVal = -1;

            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlCmd.CommandText = "insertSubject";
            sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            sqlCmd.Parameters.Add(new SqlParameter("@subjectName", _subjectName));
            sqlCmd.Parameters.Add(new SqlParameter("@subjectID", _subjectID));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int insertTarget(int _targetNumber)
        {
            int retVal = -1;

            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlCmd.CommandText = "insertTarget";
            sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            sqlCmd.Parameters.Add(new SqlParameter("@targetNumber", _targetNumber));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int insertTargetTrialNumber(int _targetTrialNumber)
        {
            int retVal = -1;

            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlCmd.CommandText = "insertTargetTrialNumber";
            sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            sqlCmd.Parameters.Add(new SqlParameter("@targetTrialNumber", _targetTrialNumber));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int insertSzenarioTrialNumber(int _szenarioTrialNumber)
        {
            int retVal = -1;

            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlCmd.CommandText = "insertSzenarioTrialNumber";
            sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            sqlCmd.Parameters.Add(new SqlParameter("@szenarioTrialNumber", _szenarioTrialNumber));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int insertIsCatchTrial(bool _isCatchTrial)
        {
            int retVal = -1;

            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlCmd.CommandText = "insertIsCatchTrial";
            sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            sqlCmd.Parameters.Add(new SqlParameter("@isCatchTrial", _isCatchTrial));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int insertTrialInformation(bool _faultyTrial, int _butterworthFilterOrder, int _butterworthFilterFreq, int _velocityTrimThreshold)
        {
            int retVal = -1;

            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlCmd.CommandText = "insertTrialInformation";
            sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            sqlCmd.Parameters.Add(new SqlParameter("@faultyTrial", _faultyTrial));
            sqlCmd.Parameters.Add(new SqlParameter("@butterworthFilterOrder", _butterworthFilterOrder));
            sqlCmd.Parameters.Add(new SqlParameter("@butterworthCutOffFreq", _butterworthFilterFreq));
            sqlCmd.Parameters.Add(new SqlParameter("@velocityTrimThreshold", _velocityTrimThreshold));


            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int insertMeasureDataRaw(
                                            int _measureDataRawID,
                                            DateTime _timeStamp,
                                            double _forceActualX,
                                            double _forceActualY,
                                            double _forceActualZ,
                                            double _forceNominalX,
                                            double _forceNominalY,
                                            double _forceNominalZ,
                                            double _forceMomentX,
                                            double _forceMomentY,
                                            double _forceMomentZ,
                                            double _positionCartesianX,
                                            double _positionCartesianY,
                                            double _positionCartesianZ,
                                            int _position_status
                                        )
        {
            int retVal = -1;

            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlCmd.CommandText = "insertMeasureDataRaw";
            sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            sqlCmd.Parameters.Add(new SqlParameter("@measureDataRawID", _measureDataRawID));
            sqlCmd.Parameters.Add(new SqlParameter("@timeStamp", _timeStamp));
            sqlCmd.Parameters.Add(new SqlParameter("@forceActualX", _forceActualX));
            sqlCmd.Parameters.Add(new SqlParameter("@forceActualY", _forceActualY));
            sqlCmd.Parameters.Add(new SqlParameter("@forceActualZ", _forceActualZ));
            sqlCmd.Parameters.Add(new SqlParameter("@forceNominalX", _forceNominalX));
            sqlCmd.Parameters.Add(new SqlParameter("@forceNominalY", _forceNominalY));
            sqlCmd.Parameters.Add(new SqlParameter("@forceNominalZ", _forceNominalZ));
            sqlCmd.Parameters.Add(new SqlParameter("@forceMomentX", _forceMomentX));
            sqlCmd.Parameters.Add(new SqlParameter("@forceMomentY", _forceMomentY));
            sqlCmd.Parameters.Add(new SqlParameter("@forceMomentZ", _forceMomentZ));
            sqlCmd.Parameters.Add(new SqlParameter("@positionCartesianX", _positionCartesianX));
            sqlCmd.Parameters.Add(new SqlParameter("@positionCartesianY", _positionCartesianY));
            sqlCmd.Parameters.Add(new SqlParameter("@positionCartesianZ", _positionCartesianZ));
            sqlCmd.Parameters.Add(new SqlParameter("@positionStatus", _position_status));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int insertMeasureDataFiltered(
                                            int _measureDataFilteredID,
                                            DateTime _timeStamp,
                                            double _forceActualX,
                                            double _forceActualY,
                                            double _forceActualZ,
                                            double _forceNominalX,
                                            double _forceNominalY,
                                            double _forceNominalZ,
                                            double _forceMomentX,
                                            double _forceMomentY,
                                            double _forceMomentZ,
                                            double _positionCartesianX,
                                            double _positionCartesianY,
                                            double _positionCartesianZ,
                                            int _positionStatus
                                        )
        {
            int retVal = -1;

            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlCmd.CommandText = "insertMeasureDataFiltered";
            sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            sqlCmd.Parameters.Add(new SqlParameter("@measureDataFilteredID", _measureDataFilteredID));
            sqlCmd.Parameters.Add(new SqlParameter("@timeStamp", _timeStamp));
            sqlCmd.Parameters.Add(new SqlParameter("@forceActualX", _forceActualX));
            sqlCmd.Parameters.Add(new SqlParameter("@forceActualY", _forceActualY));
            sqlCmd.Parameters.Add(new SqlParameter("@forceActualZ", _forceActualZ));
            sqlCmd.Parameters.Add(new SqlParameter("@forceNominalX", _forceNominalX));
            sqlCmd.Parameters.Add(new SqlParameter("@forceNominalY", _forceNominalY));
            sqlCmd.Parameters.Add(new SqlParameter("@forceNominalZ", _forceNominalZ));
            sqlCmd.Parameters.Add(new SqlParameter("@forceMomentX", _forceMomentX));
            sqlCmd.Parameters.Add(new SqlParameter("@forceMomentY", _forceMomentY));
            sqlCmd.Parameters.Add(new SqlParameter("@forceMomentZ", _forceMomentZ));
            sqlCmd.Parameters.Add(new SqlParameter("@positionCartesianX", _positionCartesianX));
            sqlCmd.Parameters.Add(new SqlParameter("@positionCartesianY", _positionCartesianY));
            sqlCmd.Parameters.Add(new SqlParameter("@positionCartesianZ", _positionCartesianZ));
            sqlCmd.Parameters.Add(new SqlParameter("@positionStatus", _positionStatus));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int insertMeasureDataNormalized(
                                            int _measureDataNormalizedID,
                                            DateTime _timeStamp,
                                            double _forceActualX,
                                            double _forceActualY,
                                            double _forceActualZ,
                                            double _forceNominalX,
                                            double _forceNominalY,
                                            double _forceNominalZ,
                                            double _forceMomentX,
                                            double _forceMomentY,
                                            double _forceMomentZ,
                                            double _positionCartesianX,
                                            double _positionCartesianY,
                                            double _positionCartesianZ,
                                            int _positionStatus
                                        )
        {
            int retVal = -1;

            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlCmd.CommandText = "insertMeasureDataNormalized";
            sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            sqlCmd.Parameters.Add(new SqlParameter("@measureDataNormalizedID", _measureDataNormalizedID));
            sqlCmd.Parameters.Add(new SqlParameter("@timeStamp", _timeStamp));
            sqlCmd.Parameters.Add(new SqlParameter("@forceActualX", _forceActualX));
            sqlCmd.Parameters.Add(new SqlParameter("@forceActualY", _forceActualY));
            sqlCmd.Parameters.Add(new SqlParameter("@forceActualZ", _forceActualZ));
            sqlCmd.Parameters.Add(new SqlParameter("@forceNominalX", _forceNominalX));
            sqlCmd.Parameters.Add(new SqlParameter("@forceNominalY", _forceNominalY));
            sqlCmd.Parameters.Add(new SqlParameter("@forceNominalZ", _forceNominalZ));
            sqlCmd.Parameters.Add(new SqlParameter("@forceMomentX", _forceMomentX));
            sqlCmd.Parameters.Add(new SqlParameter("@forceMomentY", _forceMomentY));
            sqlCmd.Parameters.Add(new SqlParameter("@forceMomentZ", _forceMomentZ));
            sqlCmd.Parameters.Add(new SqlParameter("@positionCartesianX", _positionCartesianX));
            sqlCmd.Parameters.Add(new SqlParameter("@positionCartesianY", _positionCartesianY));
            sqlCmd.Parameters.Add(new SqlParameter("@positionCartesianZ", _positionCartesianZ));
            sqlCmd.Parameters.Add(new SqlParameter("@positionStatus", _positionStatus));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int insertVelocityDataFiltered(
                                            int _velocityDataFilteredID,
                                            DateTime _timeStamp,
                                            double _velocityX,
                                            double _velocityY,
                                            double _velocityZ
                                        )
        {
            int retVal = -1;

            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlCmd.CommandText = "insertVelocityDataFiltered";
            sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            sqlCmd.Parameters.Add(new SqlParameter("@velocityDataFilteredID", _velocityDataFilteredID));
            sqlCmd.Parameters.Add(new SqlParameter("@timeStamp", _timeStamp));
            sqlCmd.Parameters.Add(new SqlParameter("@velocityX", _velocityX));
            sqlCmd.Parameters.Add(new SqlParameter("@velocityY", _velocityY));
            sqlCmd.Parameters.Add(new SqlParameter("@velocityZ", _velocityZ)); ;

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int insertVelocityDataNormalized(
                                            int _velocityDataNormalizedID,
                                            DateTime _timeStamp,
                                            double _velocityX,
                                            double _velocityY,
                                            double _velocityZ
                                        )
        {
            int retVal = -1;

            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlCmd.CommandText = "insertVelocityDataNormalized";
            sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            sqlCmd.Parameters.Add(new SqlParameter("@velocityDataNormalizedID", _velocityDataNormalizedID));
            sqlCmd.Parameters.Add(new SqlParameter("@timeStamp", _timeStamp));
            sqlCmd.Parameters.Add(new SqlParameter("@velocityX", _velocityX));
            sqlCmd.Parameters.Add(new SqlParameter("@velocityY", _velocityY));
            sqlCmd.Parameters.Add(new SqlParameter("@velocityZ", _velocityZ)); ;

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int insertStatisticData(
                                            int _trialID,
                                            double _velocityVectorCorrelation,
                                            double _trajectoryLengthAbs,
                                            double _trajectoryLengthRatio,
                                            double _perpendicularDisplacement300msAbs,
                                            double _maximalPerpendicularDisplacementAbs,
                                            double _meanPerpendicularDisplacementAbs,
                                            double _perpendicularDisplacement300msSign,
                                            double _maximalPerpendicularDisplacementSign,
                                            double _enclosedArea,
                                            double _rmse
                                       )
        {
            int retVal = -1;

            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlCmd.CommandText = "insertStatisticData";
            sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            sqlCmd.Parameters.Add(new SqlParameter("@trialID", _trialID));
            sqlCmd.Parameters.Add(new SqlParameter("@velocityVectorCorrelation", _velocityVectorCorrelation));
            sqlCmd.Parameters.Add(new SqlParameter("@trajectoryLengthAbs", _trajectoryLengthAbs));
            sqlCmd.Parameters.Add(new SqlParameter("@trajectoryLengthRatioBaseline", _trajectoryLengthRatio));
            sqlCmd.Parameters.Add(new SqlParameter("@perpendicularDisplacement300msAbs", _perpendicularDisplacement300msAbs));
            sqlCmd.Parameters.Add(new SqlParameter("@maximalPerpendicularDisplacementAbs", _maximalPerpendicularDisplacementAbs));
            sqlCmd.Parameters.Add(new SqlParameter("@meanPerpendicularDisplacementAbs", _meanPerpendicularDisplacementAbs));
            sqlCmd.Parameters.Add(new SqlParameter("@perpendicularDisplacement300msSign", _perpendicularDisplacement300msSign));
            sqlCmd.Parameters.Add(new SqlParameter("@maximalPerpendicularDisplacementSign", _maximalPerpendicularDisplacementSign));
            sqlCmd.Parameters.Add(new SqlParameter("@enclosedArea", _enclosedArea));
            sqlCmd.Parameters.Add(new SqlParameter("@rmse", _rmse));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int insertTrial(
                                            int _subjectID,
                                            int _studyID,
                                            int _groupID,
                                            int _isCatchTrialID,
                                            int _szenarioID,
                                            int _targetID,
                                            int _targetTrialNumberID,
                                            int _szenarioTrialNumberID,
                                            int _measureFileID,
                                            int _trialInformationID
                                        )
        {
            int retVal = -1;

            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlCmd.CommandText = "insertTrial";
            sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            sqlCmd.Parameters.Add(new SqlParameter("@subjectID", _subjectID));
            sqlCmd.Parameters.Add(new SqlParameter("@studyID", _studyID));
            sqlCmd.Parameters.Add(new SqlParameter("@groupID", _groupID));
            sqlCmd.Parameters.Add(new SqlParameter("@isCatchTrialID", _isCatchTrialID));
            sqlCmd.Parameters.Add(new SqlParameter("@szenarioID", _szenarioID));
            sqlCmd.Parameters.Add(new SqlParameter("@targetID", _targetID));
            sqlCmd.Parameters.Add(new SqlParameter("@targetTrialNumberID", _targetTrialNumberID));
            sqlCmd.Parameters.Add(new SqlParameter("@szenarioTrialNumberID", _szenarioTrialNumberID));
            sqlCmd.Parameters.Add(new SqlParameter("@measureFileID", _measureFileID));
            sqlCmd.Parameters.Add(new SqlParameter("@trialInformationID", _trialInformationID));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int insertSzenarioMeanTimeData(int _szenarioMeanTimeID, TimeSpan _szenarioMeanTime, TimeSpan _szenarioMeanTimeStd)
        {
            int retVal = -1;

            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlCmd.CommandText = "insertSzenarioMeanTimeData";
            sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            sqlCmd.Parameters.Add(new SqlParameter("@szenarioMeanTimeID", _szenarioMeanTimeID));
            sqlCmd.Parameters.Add(new SqlParameter("@szenarioMeanTime", _szenarioMeanTime));
            sqlCmd.Parameters.Add(new SqlParameter("@szenarioMeanTimeStd", _szenarioMeanTimeStd));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int insertSzenarioMeanTime(
                                            int _subjectID,
                                            int _studyID,
                                            int _groupID,
                                            int _targetID,
                                            int _szenarioID,
                                            int _measureFileID
                                        )
        {
            int retVal = -1;

            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlCmd.CommandText = "insertSzenarioMeanTime";
            sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            sqlCmd.Parameters.Add(new SqlParameter("@subjectID", _subjectID));
            sqlCmd.Parameters.Add(new SqlParameter("@studyID", _studyID));
            sqlCmd.Parameters.Add(new SqlParameter("@groupID", _groupID));
            sqlCmd.Parameters.Add(new SqlParameter("@targetID", _targetID));
            sqlCmd.Parameters.Add(new SqlParameter("@szenarioID", _szenarioID));
            sqlCmd.Parameters.Add(new SqlParameter("@measureFileID", _measureFileID));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int insertBaselineData(
                                            int _baselineID,
                                            DateTime _pseudoTimeStamp,
                                            double _baselinePositionCartesianX,
                                            double _baselinePositionCartesianY,
                                            double _baselinePositionCartesianZ,
                                            double _baselineVelocityX,
                                            double _baselineVelocityY,
                                            double _baselineVelocityZ
                                        )
        {
            int retVal = -1;

            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlCmd.CommandText = "insertBaselineData";
            sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            sqlCmd.Parameters.Add(new SqlParameter("@baselineID", _baselineID));
            sqlCmd.Parameters.Add(new SqlParameter("@pseudoTimeStamp", _pseudoTimeStamp));
            sqlCmd.Parameters.Add(new SqlParameter("@baselinePositionCartesianX", _baselinePositionCartesianX));
            sqlCmd.Parameters.Add(new SqlParameter("baselinePositionCartesianY", _baselinePositionCartesianY));
            sqlCmd.Parameters.Add(new SqlParameter("baselinePositionCartesianZ", _baselinePositionCartesianZ));
            sqlCmd.Parameters.Add(new SqlParameter("baselineVelocityX", _baselineVelocityX));
            sqlCmd.Parameters.Add(new SqlParameter("baselineVelocityY", _baselineVelocityY));
            sqlCmd.Parameters.Add(new SqlParameter("baselineVelocityZ", _baselineVelocityZ));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public int insertBaseline(
                                            int _subjectID,
                                            int _studyID,
                                            int _groupID,
                                            int _targetID,
                                            int _szenarioID,
                                            int _measureFileID
                                        )
        {
            int retVal = -1;

            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlCmd.CommandText = "insertBaseline";
            sqlCmd.Parameters.Add("@id", SqlDbType.Int);
            sqlCmd.Parameters["@id"].Direction = ParameterDirection.Output;

            sqlCmd.Parameters.Add(new SqlParameter("@subjectID", _subjectID));
            sqlCmd.Parameters.Add(new SqlParameter("@studyID", _studyID));
            sqlCmd.Parameters.Add(new SqlParameter("@groupID", _groupID));
            sqlCmd.Parameters.Add(new SqlParameter("@targetID", _targetID));
            sqlCmd.Parameters.Add(new SqlParameter("@szenarioID", _szenarioID));
            sqlCmd.Parameters.Add(new SqlParameter("@measureFileID", _measureFileID));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToInt32(sqlCmd.Parameters["@id"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public void bulkInsertMeasureDataRaw(string _filename)
        {
            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.Text;

            sqlCmd.CommandText = "BULK INSERT dbo._measure_data_raw FROM '" + @_filename + "' WITH  ( FIELDTERMINATOR =',', ROWTERMINATOR ='\r\n' )";

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }
        }

        public void bulkInsertMeasureDataFiltered(string _filename)
        {
            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.Text;

            sqlCmd.CommandText = "BULK INSERT dbo._measure_data_filtered FROM '" + @_filename + "' WITH  ( FIELDTERMINATOR =',', ROWTERMINATOR ='\r\n' )";

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }
        }

        public void bulkInsertMeasureDataNormalized(string _filename)
        {
            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.Text;

            sqlCmd.CommandText = "BULK INSERT dbo._measure_data_normalized FROM '" + @_filename + "' WITH  ( FIELDTERMINATOR =',', ROWTERMINATOR ='\r\n' )";

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }
        }

        public void bulkInsertVelocityDataFiltered(string _filename)
        {
            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.Text;

            sqlCmd.CommandText = "BULK INSERT dbo._velocity_data_filtered FROM '" + @_filename + "' WITH  ( FIELDTERMINATOR =',', ROWTERMINATOR ='\r\n' )";

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }
        }

        public void bulkInsertVelocityDataNormalized(string _filename)
        {
            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.Text;

            sqlCmd.CommandText = "BULK INSERT dbo._velocity_data_normalized FROM '" + @_filename + "' WITH  ( FIELDTERMINATOR =',', ROWTERMINATOR ='\r\n' )";

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }
        }

        public bool checkIfMeasureFileHashExists(string _measureFileHash)
        {
            bool retVal = false;

            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlCmd.CommandText = "checkMeasureFileHash";
            sqlCmd.Parameters.Add("@hashExists", SqlDbType.Bit);
            sqlCmd.Parameters["@hashExists"].Direction = ParameterDirection.Output;

            sqlCmd.Parameters.Add(new SqlParameter("@measureFileHash", _measureFileHash));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    sqlCmd.ExecuteNonQuery();
                    retVal = Convert.ToBoolean(sqlCmd.Parameters["@hashExists"].Value);
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public string[] getStudyNames()
        {
            string[] retVal = null;

            sqlCmd.Parameters.Clear();
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = "SELECT * FROM getStudyNames();";

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    SqlDataReader SqlRdr = sqlCmd.ExecuteReader();

                    if (SqlRdr.HasRows)
                    {
                        List<string> rows = new List<string>();
                        while (SqlRdr.Read())
                        {
                            rows.Add(SqlRdr.GetString(0));
                        }
                        SqlRdr.Close();
                        retVal = rows.ToArray();
                    }
                    else
                    {
                        SqlRdr.Close();
                    }
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public string[] getGroupNames(string _studyName)
        {
            string[] retVal = null;

            sqlCmd.Parameters.Clear();
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = "SELECT * FROM getGroupNames(@studyName);";
            sqlCmd.Parameters.Add(new SqlParameter("@studyName", _studyName));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    SqlDataReader SqlRdr = sqlCmd.ExecuteReader();

                    if (SqlRdr.HasRows)
                    {
                        List<string> rows = new List<string>();
                        while (SqlRdr.Read())
                        {
                            rows.Add(SqlRdr.GetString(0));
                        }
                        SqlRdr.Close();
                        retVal = rows.ToArray();
                    }
                    else
                    {
                        SqlRdr.Close();
                    }
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public string[] getSzenarioNames(string _studyName, string _groupName)
        {
            string[] retVal = null;

            sqlCmd.Parameters.Clear();
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = "SELECT * FROM getSzenarioNames(@studyName,@groupName);";
            sqlCmd.Parameters.Add(new SqlParameter("@studyName", _studyName));
            sqlCmd.Parameters.Add(new SqlParameter("@groupName", _groupName));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    SqlDataReader SqlRdr = sqlCmd.ExecuteReader();

                    if (SqlRdr.HasRows)
                    {
                        List<string> rows = new List<string>();
                        while (SqlRdr.Read())
                        {
                            rows.Add(SqlRdr.GetString(0));
                        }
                        SqlRdr.Close();
                        retVal = rows.ToArray();
                    }
                    else
                    {
                        SqlRdr.Close();
                    }
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public SubjectInformationContainer[] getSubjectInformations(string _studyName, string _groupName, string _szenarioName)
        {
            SubjectInformationContainer[] retVal = null;

            sqlCmd.Parameters.Clear();
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = "SELECT * FROM getSubjectInformations(@studyName,@groupName,@szenarioName);";
            sqlCmd.Parameters.Add(new SqlParameter("@studyName", _studyName));
            sqlCmd.Parameters.Add(new SqlParameter("@groupName", _groupName));
            sqlCmd.Parameters.Add(new SqlParameter("@szenarioName", _szenarioName));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    SqlDataReader SqlRdr = sqlCmd.ExecuteReader();

                    if (SqlRdr.HasRows)
                    {
                        List<SubjectInformationContainer> rows = new List<SubjectInformationContainer>();
                        while (SqlRdr.Read())
                        {
                            rows.Add(new SubjectInformationContainer(SqlRdr.GetInt32(0), SqlRdr.GetString(1), SqlRdr.GetString(2)));
                        }
                        SqlRdr.Close();
                        retVal = rows.ToArray();
                    }
                    else
                    {
                        SqlRdr.Close();
                    }
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public string[] getTurns(string _studyName, string _groupName, string _szenarioName, int _subjectID)
        {
            string[] retVal = null;

            sqlCmd.Parameters.Clear();
            sqlCmd.CommandType = CommandType.Text;

            sqlCmd.CommandText = "SELECT * FROM getTurns(@studyName,@groupName,@szenarioName,@subjectID)";

            sqlCmd.Parameters.Add(new SqlParameter("@studyName", _studyName));
            sqlCmd.Parameters.Add(new SqlParameter("@groupName", _groupName));
            sqlCmd.Parameters.Add(new SqlParameter("@szenarioName", _szenarioName));
            sqlCmd.Parameters.Add(new SqlParameter("@subjectID", _subjectID));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    SqlDataReader SqlRdr = sqlCmd.ExecuteReader();

                    if (SqlRdr.HasRows)
                    {
                        List<string> rows = new List<string>();
                        while (SqlRdr.Read())
                        {
                            rows.Add(SqlRdr.GetString(0));
                        }
                        SqlRdr.Close();
                        retVal = rows.ToArray();
                    }
                    else
                    {
                        SqlRdr.Close();
                    }
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public string[] getSzenarioTrials(string _studyName, string _szenarioName, bool _showCatchTrials, bool _showCatchTrialsExclusivly)
        {
            string[] retVal = null;

            sqlCmd.Parameters.Clear();
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = "SELECT * FROM getSzenarioTrials(@studyName,@szenarioName,@showCatchTrials,@showCatchTrialsExclusivly)";

            sqlCmd.Parameters.Add(new SqlParameter("@studyName", _studyName));
            sqlCmd.Parameters.Add(new SqlParameter("@szenarioName", _szenarioName));
            sqlCmd.Parameters.Add(new SqlParameter("@showCatchTrials", Convert.ToInt32(_showCatchTrials)));
            sqlCmd.Parameters.Add(new SqlParameter("@showCatchTrialsExclusivly", Convert.ToInt32(_showCatchTrialsExclusivly)));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    SqlDataReader SqlRdr = sqlCmd.ExecuteReader();

                    if (SqlRdr.HasRows)
                    {
                        List<string> rows = new List<string>();
                        while (SqlRdr.Read())
                        {
                            rows.Add(SqlRdr.GetString(0));
                        }
                        SqlRdr.Close();
                        retVal = rows.ToArray();
                    }
                    else
                    {
                        SqlRdr.Close();
                    }
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public string[] getTargets(string _studyName, string _szenarioName)
        {
            string[] retVal = null;

            sqlCmd.Parameters.Clear();
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = "SELECT * FROM getTargets(@studyName,@szenarioName)";

            sqlCmd.Parameters.Add(new SqlParameter("@studyName", _studyName));
            sqlCmd.Parameters.Add(new SqlParameter("@szenarioName", _szenarioName));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    SqlDataReader SqlRdr = sqlCmd.ExecuteReader();

                    if (SqlRdr.HasRows)
                    {
                        List<string> rows = new List<string>();
                        while (SqlRdr.Read())
                        {
                            rows.Add(SqlRdr.GetString(0));
                        }
                        SqlRdr.Close();
                        retVal = rows.ToArray();
                    }
                    else
                    {
                        SqlRdr.Close();
                    }
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

                        if (result == DialogResult.Yes)
                        {
                            executeTryCounter = 5;
                        }
                    }
                }
            }

            return retVal;
        }

        public string[] getTrials(string _studyName, string _szenarioName)
        {
            string[] retVal = null;

            sqlCmd.Parameters.Clear();

            sqlCmd.CommandType = CommandType.Text;

            sqlCmd.CommandText = "SELECT * FROM  getTrials(@studyName,@szenarioName)";

            sqlCmd.Parameters.Add(new SqlParameter("@studyName", _studyName));
            sqlCmd.Parameters.Add(new SqlParameter("@szenarioName", _szenarioName));

            int executeTryCounter = 5;
            while (executeTryCounter > 0)
            {
                try
                {
                    openSqlConnection();
                    SqlDataReader SqlRdr = sqlCmd.ExecuteReader();

                    if (SqlRdr.HasRows)
                    {
                        List<string> rows = new List<string>();
                        while (SqlRdr.Read())
                        {
                            rows.Add(SqlRdr.GetString(0));
                        }
                        SqlRdr.Close();
                        retVal = rows.ToArray();
                    }
                    else
                    {
                        SqlRdr.Close();
                    }
                    executeTryCounter = 0;
                }
                catch (Exception ex)
                {
                    Logger.writeToLog(ex.ToString());
                    executeTryCounter--;
                    if (executeTryCounter == 0)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result;

                        result = MessageBox.Show("Tried to execute SQL command 5 times, try another 5?", "Try again?", buttons);

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