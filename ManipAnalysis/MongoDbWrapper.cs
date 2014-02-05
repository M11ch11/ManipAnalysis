﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using ManipAnalysis_v2.Container;
using ManipAnalysis_v2.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;
using MongoDB.Driver.Wrappers;

namespace ManipAnalysis_v2
{
    class MongoDbWrapper
    {
        private string _connectionString;
        private MongoClient _mongoClient;
        private MongoServer _mongoServer;
        private MongoDatabase _mongoDatabase;
        private MongoCollection<Trial> _trialCollection;
        private MongoCollection<Baseline> _baselineCollection;
        private MongoCollection<SzenarioMeanTime> _szenarioMeanTimeCollection;
        private ManipAnalysisGui _myManipAnalysisGui;

        private string _mongoDbDatabaseString;
        private string _mongoDbPasswordString;
        private string _mongoDbServerString;
        private string _mongoDbUsernameString;

        public MongoDbWrapper(ManipAnalysisGui myManipAnalysisGui)
        {
            _myManipAnalysisGui = myManipAnalysisGui;
            _mongoDbDatabaseString = "local";
            _mongoDbPasswordString = "!sport12";
            _mongoDbServerString = "localhost";
            _mongoDbUsernameString = "DataAccess";
        }

        public IEnumerable<string> GetDatabases()
        {
            List<string> databasesList = _mongoServer.GetDatabaseNames().ToList();
            databasesList.Remove("admin");
            databasesList.Remove("local");
            return databasesList;
        }

        public void SetDatabaseServer(string serverUri)
        {
            SetConnectionString(serverUri, _mongoDbDatabaseString, _mongoDbUsernameString, _mongoDbPasswordString);
        }

        private void SetConnectionString(string serverUri, string database, string username, string password)
        {
            try
            {
                _mongoDbServerString = serverUri;
                _mongoDbDatabaseString = database;
                _mongoDbUsernameString = username;
                _mongoDbPasswordString = password;
                _connectionString = "mongodb://" + _mongoDbUsernameString + ":" + _mongoDbPasswordString + "@" +
                                    _mongoDbServerString + "/" +
                                    _mongoDbDatabaseString;
                _mongoClient = new MongoClient(_connectionString);
                _mongoServer = _mongoClient.GetServer();
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox(ex.ToString());
            }
        }

        public void SetDatabase(string database)
        {
            try
            {
                _mongoDatabase = _mongoServer.GetDatabase(database);
                _trialCollection = _mongoDatabase.GetCollection<Trial>("Trial");
                _baselineCollection = _mongoDatabase.GetCollection<Baseline>("Baseline");
                _szenarioMeanTimeCollection = _mongoDatabase.GetCollection<SzenarioMeanTime>("SzenarioMeanTime");
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox(ex.ToString());
            }
        }

        public void EnsureIndexes()
        {
            try
            {
                _trialCollection.EnsureIndex(new IndexKeysBuilder<Trial>().Ascending(t => t.Study));
                _trialCollection.EnsureIndex(new IndexKeysBuilder<Trial>().Ascending(t => t.Group));
                _trialCollection.EnsureIndex(new IndexKeysBuilder<Trial>().Ascending(t => t.Subject));
                _trialCollection.EnsureIndex(new IndexKeysBuilder<Trial>().Ascending(t => t.Subject.PId));
                _trialCollection.EnsureIndex(new IndexKeysBuilder<Trial>().Ascending(t => t.Szenario));
                _trialCollection.EnsureIndex(new IndexKeysBuilder<Trial>().Ascending(t => t.Target));
                _trialCollection.EnsureIndex(new IndexKeysBuilder<Trial>().Ascending(t => t.Target.Number));
                _trialCollection.EnsureIndex(new IndexKeysBuilder<Trial>().Ascending(t => t.TrialNumberInSzenario));
                _trialCollection.EnsureIndex(new IndexKeysBuilder<Trial>().Ascending(t => t.TargetTrialNumberInSzenario));
                _trialCollection.EnsureIndex(new IndexKeysBuilder<Trial>().Ascending(t => t.MeasureFile));
                _trialCollection.EnsureIndex(new IndexKeysBuilder<Trial>().Ascending(t => t.MeasureFile.CreationTime));
                _trialCollection.EnsureIndex(new IndexKeysBuilder<Trial>().Ascending(t => t.MeasureFile.FileHash));

                _baselineCollection.EnsureIndex(new IndexKeysBuilder<Baseline>().Ascending(t => t.Study));
                _baselineCollection.EnsureIndex(new IndexKeysBuilder<Baseline>().Ascending(t => t.Group));
                _baselineCollection.EnsureIndex(new IndexKeysBuilder<Baseline>().Ascending(t => t.Subject));
                _baselineCollection.EnsureIndex(new IndexKeysBuilder<Baseline>().Ascending(t => t.Subject.PId));
                _baselineCollection.EnsureIndex(new IndexKeysBuilder<Baseline>().Ascending(t => t.Szenario));
                _baselineCollection.EnsureIndex(new IndexKeysBuilder<Baseline>().Ascending(t => t.Target));
                _baselineCollection.EnsureIndex(new IndexKeysBuilder<Baseline>().Ascending(t => t.Target.Number));
                _baselineCollection.EnsureIndex(new IndexKeysBuilder<SzenarioMeanTime>().Ascending(t => t.MeasureFile));
                _baselineCollection.EnsureIndex(new IndexKeysBuilder<SzenarioMeanTime>().Ascending(t => t.MeasureFile.CreationTime));
                _baselineCollection.EnsureIndex(new IndexKeysBuilder<SzenarioMeanTime>().Ascending(t => t.MeasureFile.FileHash));

                _szenarioMeanTimeCollection.EnsureIndex(new IndexKeysBuilder<SzenarioMeanTime>().Ascending(t => t.Study));
                _szenarioMeanTimeCollection.EnsureIndex(new IndexKeysBuilder<SzenarioMeanTime>().Ascending(t => t.Group));
                _szenarioMeanTimeCollection.EnsureIndex(new IndexKeysBuilder<SzenarioMeanTime>().Ascending(t => t.Subject));
                _szenarioMeanTimeCollection.EnsureIndex(new IndexKeysBuilder<SzenarioMeanTime>().Ascending(t => t.Subject.PId));
                _szenarioMeanTimeCollection.EnsureIndex(new IndexKeysBuilder<SzenarioMeanTime>().Ascending(t => t.Szenario));
                _szenarioMeanTimeCollection.EnsureIndex(new IndexKeysBuilder<SzenarioMeanTime>().Ascending(t => t.Target));
                _szenarioMeanTimeCollection.EnsureIndex(new IndexKeysBuilder<SzenarioMeanTime>().Ascending(t => t.Target.Number));
                _szenarioMeanTimeCollection.EnsureIndex(new IndexKeysBuilder<SzenarioMeanTime>().Ascending(t => t.MeasureFile));
                _szenarioMeanTimeCollection.EnsureIndex(
                    new IndexKeysBuilder<SzenarioMeanTime>().Ascending(t => t.MeasureFile.CreationTime));
                _szenarioMeanTimeCollection.EnsureIndex(
                    new IndexKeysBuilder<SzenarioMeanTime>().Ascending(t => t.MeasureFile.FileHash));
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox(ex.ToString());
            }
        }

        public void RebuildIndexes()
        {
            try
            {
                _trialCollection.ReIndex();
                _baselineCollection.ReIndex();
                _szenarioMeanTimeCollection.ReIndex();
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox(ex.ToString());
            }
        }

        public void DropAllIndexes()
        {
            try
            {
                _trialCollection.DropAllIndexes();
                _baselineCollection.DropAllIndexes();
                _szenarioMeanTimeCollection.DropAllIndexes();
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox(ex.ToString());
            }
        }
        
        public void CompactDatabase()
        {
            try
            {
                _mongoDatabase.RunCommand(new CommandDocument("compact", "Trial"));
                _mongoDatabase.RunCommand(new CommandDocument("compact", "Baseline"));
                _mongoDatabase.RunCommand(new CommandDocument("compact", "SzenarioMeanTime"));
            }

            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox(ex.ToString());
            }
        }

        public void DropDatabase()
        {
            try
            {
                _mongoDatabase.DropCollection("SzenarioMeanTime");
                _mongoDatabase.DropCollection("Baseline");
                _mongoDatabase.DropCollection("Trial");
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox(ex.ToString());
            }
        }

        public IEnumerable<string> GetStudys()
        {
            try
            {
                //TicToc.Tic();
                //var studys = _trialCollection.Distinct<string>("Study");
                //var studys = _trialCollection.AsQueryable().Select(t => t.Study).Distinct();
                //_myManipAnalysisGui.WriteToLogBox(TicToc.Toc() + "ms");
                return _trialCollection.AsQueryable().Select(t => t.Study).Distinct();
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox("MongoDbwrapper::GetStudys: " + ex);
                return new List<string>();
            }
        }

        public IEnumerable<string> GetGroups(string studyName)
        {
            try
            {
                return _trialCollection.AsQueryable().Where(t => t.Study == studyName).Select(t => t.Group).Distinct();
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox("MongoDbwrapper::GetGroups: " + ex);
                return new List<string>();
            }
        }

        public IEnumerable<string> GetSzenarios(string studyName, string groupName)
        {
            try
            {
                return _trialCollection.AsQueryable().Where(t => t.Study == studyName && t.Group == groupName).Select(t => t.Szenario).Distinct();
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox("MongoDbwrapper::GetSzenarios: " + ex);
                return new List<string>();
            }
        }

        public IEnumerable<SubjectContainer> GetSubjects(string studyName, string groupName, string szenarioName)
        {
            try
            {
                return _trialCollection.AsQueryable().Where(t => t.Study == studyName && t.Group == groupName && t.Szenario == szenarioName).Select(t => t.Subject).Distinct();
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox("MongoDbwrapper::GetSubjects: " + ex);
                return new List<SubjectContainer>();
            }
        }

        public IEnumerable<DateTime> GetTurns(string studyName, string groupName, string szenarioName, SubjectContainer subject)
        {
            try
            {
                return _trialCollection.AsQueryable().Where(t => t.Study == studyName && t.Group == groupName && t.Szenario == szenarioName && t.Subject == subject).Select(t => t.MeasureFile.CreationTime).Distinct();
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox("MongoDbwrapper::GetTurns: " + ex);
                return new List<DateTime>();
            }
        }

        public IEnumerable<int> GetTargets(string studyName, string szenarioName)
        {
            try
            {
                return _trialCollection.AsQueryable().Where(t => t.Study == studyName && t.Szenario == szenarioName).Select(t => t.Target.Number).Distinct();
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox("MongoDbwrapper::GetTargets: " + ex);
                return new List<int>();
            }
        }

        public IEnumerable<int> GetTargetTrials(string studyName, string szenarioName)
        {
            try
            {
                return _trialCollection.AsQueryable().Where(t => t.Study == studyName && t.Szenario == szenarioName).Select(t => t.TargetTrialNumberInSzenario).Distinct();
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox("MongoDbwrapper::GetTargetTrials: " + ex);
                return new List<int>();
            }
        }

        public IEnumerable<int> GetSzenarioTrials(string studyName, string szenarioName, bool showNormalTrials, bool showCatchTrials, bool showErrorclampTrials)
        {
            IEnumerable<int> retVal;
            try
            {
                if (showNormalTrials && showCatchTrials && showErrorclampTrials)
                {
                    retVal = _trialCollection.AsQueryable().Where(t => t.Study == studyName && t.Szenario == szenarioName).Select(t => t.TrialNumberInSzenario).Distinct();
                }
                else if (showNormalTrials && !showCatchTrials && showErrorclampTrials)
                {
                    retVal = _trialCollection.AsQueryable().Where(t => t.Study == studyName && t.Szenario == szenarioName && !t.CatchTrial).Select(t => t.TrialNumberInSzenario).Distinct();
                }
                else if (showNormalTrials && showCatchTrials && !showErrorclampTrials)
                {
                    retVal = _trialCollection.AsQueryable().Where(t => t.Study == studyName && t.Szenario == szenarioName && !t.ErrorClampTrial).Select(t => t.TrialNumberInSzenario).Distinct();
                }
                else if (showNormalTrials && !showCatchTrials && !showErrorclampTrials)
                {
                    retVal = _trialCollection.AsQueryable().Where(t => t.Study == studyName && t.Szenario == szenarioName && !t.CatchTrial && !t.ErrorClampTrial).Select(t => t.TrialNumberInSzenario).Distinct();
                }
                else if (!showNormalTrials && showCatchTrials && showErrorclampTrials)
                {
                    retVal = _trialCollection.AsQueryable().Where(t => t.Study == studyName && t.Szenario == szenarioName && (t.CatchTrial || t.ErrorClampTrial)).Select(t => t.TrialNumberInSzenario).Distinct();
                }
                else if (!showNormalTrials && !showCatchTrials && showErrorclampTrials)
                {
                    retVal = _trialCollection.AsQueryable().Where(t => t.Study == studyName && t.Szenario == szenarioName && !t.CatchTrial && t.ErrorClampTrial).Select(t => t.TrialNumberInSzenario).Distinct();
                }
                else if (!showNormalTrials && showCatchTrials && !showErrorclampTrials)
                {
                    retVal = _trialCollection.AsQueryable().Where(t => t.Study == studyName && t.Szenario == szenarioName && t.CatchTrial && !t.ErrorClampTrial).Select(t => t.TrialNumberInSzenario).Distinct();
                }
                else
                {
                    retVal = new List<int>();
                }
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox("MongoDbwrapper::GetSzenarioTrials: " + ex);
                return new List<int>();
            }

            return retVal;
        }

        public Trial GetTrial(string studyName, string groupName, string szenarioName,
            SubjectContainer subject, DateTime turn, int target, int trial, bool showNormalTrials, bool showCatchTrials, bool showErrorclampTrials, FieldsBuilder<Trial> fields)
        {
            Trial retVal;
            try
            {
                if (showNormalTrials && showCatchTrials && showErrorclampTrials)
                {
                    retVal = _trialCollection.FindAs<Trial>(Query<Trial>.Where(t =>
                        t.Study == studyName && t.Group == groupName && t.Szenario == szenarioName &&
                        t.Subject == subject && t.MeasureFile.CreationTime == turn && t.Target.Number == target &&
                        t.TargetTrialNumberInSzenario == trial))
                        .SetFields(fields).SetLimit(1).First();
                }
                else if (showNormalTrials && !showCatchTrials && showErrorclampTrials)
                {
                    retVal = _trialCollection.FindAs<Trial>(Query<Trial>.Where(t =>
                        t.Study == studyName && t.Group == groupName && t.Szenario == szenarioName &&
                        t.Subject == subject && t.MeasureFile.CreationTime == turn && t.Target.Number == target &&
                        t.TargetTrialNumberInSzenario == trial && !t.CatchTrial))
                        .SetFields(fields).SetLimit(1).First();
                }
                else if (showNormalTrials && showCatchTrials && !showErrorclampTrials)
                {
                    retVal = _trialCollection.FindAs<Trial>(Query<Trial>.Where(t =>
                        t.Study == studyName && t.Group == groupName && t.Szenario == szenarioName &&
                        t.Subject == subject && t.MeasureFile.CreationTime == turn && t.Target.Number == target &&
                        t.TargetTrialNumberInSzenario == trial && !t.ErrorClampTrial))
                        .SetFields(fields).SetLimit(1).First();
                }
                else if (showNormalTrials && !showCatchTrials && !showErrorclampTrials)
                {
                    retVal = _trialCollection.FindAs<Trial>(Query<Trial>.Where(t =>
                        t.Study == studyName && t.Group == groupName && t.Szenario == szenarioName &&
                        t.Subject == subject && t.MeasureFile.CreationTime == turn && t.Target.Number == target &&
                        t.TargetTrialNumberInSzenario == trial && !t.CatchTrial && !t.ErrorClampTrial))
                        .SetFields(fields).SetLimit(1).First();
                }
                else if (!showNormalTrials && showCatchTrials && showErrorclampTrials)
                {
                    retVal = _trialCollection.FindAs<Trial>(Query<Trial>.Where(t =>
                        t.Study == studyName && t.Group == groupName && t.Szenario == szenarioName &&
                        t.Subject == subject && t.MeasureFile.CreationTime == turn && t.Target.Number == target &&
                        t.TargetTrialNumberInSzenario == trial && (t.CatchTrial || t.ErrorClampTrial)))
                        .SetFields(fields).SetLimit(1).First();
                }
                else if (!showNormalTrials && !showCatchTrials && showErrorclampTrials)
                {
                    retVal = _trialCollection.FindAs<Trial>(Query<Trial>.Where(t =>
                        t.Study == studyName && t.Group == groupName && t.Szenario == szenarioName &&
                        t.Subject == subject && t.MeasureFile.CreationTime == turn && t.Target.Number == target &&
                        t.TargetTrialNumberInSzenario == trial && !t.CatchTrial && t.ErrorClampTrial))
                        .SetFields(fields).SetLimit(1).First();
                }
                else if (!showNormalTrials && showCatchTrials && !showErrorclampTrials)
                {
                    retVal = _trialCollection.FindAs<Trial>(Query<Trial>.Where(t =>
                        t.Study == studyName && t.Group == groupName && t.Szenario == szenarioName &&
                        t.Subject == subject && t.MeasureFile.CreationTime == turn && t.Target.Number == target &&
                        t.TargetTrialNumberInSzenario == trial && t.CatchTrial && !t.ErrorClampTrial))
                        .SetFields(fields).SetLimit(1).First();
                }
                else
                {
                    retVal = null;
                }
            }
            catch (Exception)
            {
                retVal = null;
            }

            return retVal;
        }

        public Trial GetTrial(string studyName, string groupName, string szenarioName, SubjectContainer subject, DateTime turn, int szenarioTrial, FieldsBuilder<Trial> fields)
        {
            Trial retVal;
            try
            {
                retVal = _trialCollection.FindAs<Trial>(Query<Trial>.Where(t =>
                    t.Study == studyName && t.Group == groupName && t.Szenario == szenarioName &&
                    t.Subject == subject && t.MeasureFile.CreationTime == turn && t.TrialNumberInSzenario == szenarioTrial))
                    .SetFields(fields).SetLimit(1).First();
            }
            catch (Exception)
            {
                retVal = null;
            }

            return retVal;
        }

        public WriteConcernResult UpdateTrialStatisticsAndBaselineId(Trial trial)
        {
            var query = Query<Trial>.EQ(t => t.Id, trial.Id);
            var update = Update<Trial>.Set(t => t.Statistics, trial.Statistics).Set(t => t.BaselineObjectId, trial.BaselineObjectId);
            return _trialCollection.Update(query, update);
        }

        public IEnumerable<Trial> GetTrialsWithoutStatistics(FieldsBuilder<Trial> statisticFields)
        {
            return _trialCollection.FindAs<Trial>(Query<Trial>.Where(t => t.Statistics == null)).SetFields(statisticFields);
        }

        public Baseline GetBaseline(string study, string group, SubjectContainer subject, int targetNumber, FieldsBuilder<Baseline> baselineFields)
        {
            try
            {
                return _baselineCollection.FindAs<Baseline>(Query<Trial>.Where(t => t.Study == study && t.Group == group && t.Subject == subject && t.Target.Number == targetNumber)).SetFields(baselineFields).SetLimit(1).First();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Baseline[] GetBaseline(string study, string group, SubjectContainer subject, FieldsBuilder<Baseline> baselineFields)
        {
            try
            {
                return _baselineCollection.FindAs<Baseline>(Query<Trial>.Where(t => t.Study == study && t.Group == group && t.Subject == subject)).SetFields(baselineFields).ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Baseline GetBaseline(ObjectId objectId, FieldsBuilder<Baseline> baselineFields)
        {
            try
            {
                return _baselineCollection.FindAs<Baseline>(Query<Trial>.Where(t => t.Id == objectId)).SetFields(baselineFields).SetLimit(1).First();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public SzenarioMeanTime[] GetSzenarioMeanTime(string study, string group, string szenario, SubjectContainer subject, DateTime turn)
        {
            try
            {
                return _szenarioMeanTimeCollection.FindAs<SzenarioMeanTime>(Query<Trial>.Where(t => t.Study == study && t.Group == group && t.Szenario == szenario && t.Subject == subject && t.MeasureFile.CreationTime == turn)).ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool CheckIfMeasureFileHashExists(string measureFileHash)
        {
            try
            {
                return _trialCollection.AsQueryable<Trial>().Any(t => t.MeasureFile.FileHash == measureFileHash);
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                return false;
            }
        }

        public void Insert(IEnumerable<Trial> trials)
        {
            // No try-catch-Block since ManipAnalysisFunctions is handling this
            _trialCollection.InsertBatch(trials);
        }

        public void Insert(IEnumerable<SzenarioMeanTime> szenarioMeanTimes)
        {
            // No try-catch-Block since ManipAnalysisFunctions is handling this
            _szenarioMeanTimeCollection.InsertBatch(szenarioMeanTimes);
        }

        public void Insert(IEnumerable<Baseline> baselines)
        {
            // No try-catch-Block since ManipAnalysisFunctions is handling this
            _baselineCollection.InsertBatch(baselines);
        }

        public void RemoveMeasureFile(MeasureFileContainer measureFile)
        {
            try
            {
                _trialCollection.Remove(Query<Trial>.EQ(t => t.MeasureFile, measureFile));
                _szenarioMeanTimeCollection.Remove(Query<Trial>.EQ(t => t.MeasureFile, measureFile));
                _baselineCollection.Remove(Query<Trial>.EQ(t => t.MeasureFile, measureFile));
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox(ex.ToString());
            }
        }
    }
}
