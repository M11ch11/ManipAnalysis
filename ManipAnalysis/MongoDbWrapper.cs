using System;
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
            _mongoDbServerString = serverUri;
            _mongoDbDatabaseString = database;
            _mongoDbUsernameString = username;
            _mongoDbPasswordString = password;
            _connectionString = "mongodb://" + _mongoDbUsernameString + ":" + _mongoDbPasswordString + "@" + _mongoDbServerString + "/" +
                                _mongoDbDatabaseString;
            _mongoClient = new MongoClient(_connectionString);
            _mongoServer = _mongoClient.GetServer();
        }

        public void SetDatabase(string database)
        {
            _mongoDatabase = _mongoServer.GetDatabase(database);
            _trialCollection = _mongoDatabase.GetCollection<Trial>("Trial");
            _baselineCollection = _mongoDatabase.GetCollection<Baseline>("Baseline");
            _szenarioMeanTimeCollection = _mongoDatabase.GetCollection<SzenarioMeanTime>("SzenarioMeanTime");
        }

        public void EnsureIndexes()
        {
            //IndexKeysBuilder<Trial> trialIndex = new IndexKeysBuilder<Trial>();
            //trialIndex.Ascending(t1 => t1.Group, t2 => t2.Subject, t3 => t3.Study, t4 => t4.Subject.PId, t5 => t5.Szenario, t6 => t6.Target.Number);
            //_trialCollection.EnsureIndex(trialIndex);

            //IndexKeysBuilder<Baseline> baselineIndex = new IndexKeysBuilder<Baseline>();
            //baselineIndex.Ascending(t1 => t1.Group, t2 => t2.Subject, t3 => t3.Study, t4 => t4.Subject.PId, t5 => t5.Szenario, t6 => t6.Target.Number);
            //_baselineCollection.EnsureIndex(baselineIndex);

            //IndexKeysBuilder<SzenarioMeanTime> szenarioMeanTimeIndex = new IndexKeysBuilder<SzenarioMeanTime>();
            //szenarioMeanTimeIndex.Ascending(t1 => t1.Group, t2 => t2.Subject, t3 => t3.Study, t4 => t4.Subject.PId, t5 => t5.Szenario, t6 => t6.Target.Number);
            //_szenarioMeanTimeCollection.EnsureIndex(szenarioMeanTimeIndex);

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
            _szenarioMeanTimeCollection.EnsureIndex(new IndexKeysBuilder<SzenarioMeanTime>().Ascending(t => t.MeasureFile.CreationTime));
            _szenarioMeanTimeCollection.EnsureIndex(new IndexKeysBuilder<SzenarioMeanTime>().Ascending(t => t.MeasureFile.FileHash));
        }

        public void RebuildIndexes()
        {
            _trialCollection.ReIndex();
            _baselineCollection.ReIndex();
            _szenarioMeanTimeCollection.ReIndex();
        }

        public void DropAllIndexes()
        {
            _trialCollection.DropAllIndexes();
            _baselineCollection.DropAllIndexes();
            _szenarioMeanTimeCollection.DropAllIndexes();
        }
        
        public void CompactDatabase()
        {
            _mongoDatabase.RunCommand(new CommandDocument("compact", "Trial"));
            _mongoDatabase.RunCommand(new CommandDocument("compact", "Baseline"));
            _mongoDatabase.RunCommand(new CommandDocument("compact", "SzenarioMeanTime"));
        }

        public void DropDatabase()
        {
            _mongoDatabase.DropCollection("SzenarioMeanTime");
            _mongoDatabase.DropCollection("Baseline");
            _mongoDatabase.DropCollection("Trial");
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

        public IEnumerable<Trial> GetTrialsWithoutStatistics()
        {
            FieldsBuilder<Trial> statisticFields = new FieldsBuilder<Trial>();
            statisticFields.Include(t1 => t1.VelocityNormalized, t2 => t2.PositionNormalized, t3 => t3.Subject, t4 => t4.Study, t5 => t5.Group, t6 => t6.Target);
            return _trialCollection.FindAs<Trial>(Query<Trial>.Where(t => t.Statistics == null)).SetFields(statisticFields);
        }

        public Baseline GetBaseline(string study, string group, SubjectContainer subject, int targetNumber)
        {
            try
            {
                return _baselineCollection.FindAs<Baseline>(Query<Trial>.Where(t => t.Study == study && t.Group == group && t.Subject == subject && t.Target.Number == targetNumber)).SetLimit(1).First();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Baseline[] GetBaseline(string study, string group, SubjectContainer subject)
        {
            try
            {
                return _baselineCollection.FindAs<Baseline>(Query<Trial>.Where(t => t.Study == study && t.Group == group && t.Subject == subject)).ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Baseline GetBaseline(ObjectId objectId)
        {
            try
            {
                return _baselineCollection.FindAs<Baseline>(Query<Trial>.Where(t => t.Id == objectId)).SetLimit(1).First();
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
            return _trialCollection.AsQueryable<Trial>().Any(t => t.MeasureFile.FileHash == measureFileHash);
        }

        public void Insert(IEnumerable<Trial> trials)
        {
            _trialCollection.InsertBatch(trials);
        }

        public void Insert(IEnumerable<SzenarioMeanTime> szenarioMeanTimes)
        {
            _szenarioMeanTimeCollection.InsertBatch(szenarioMeanTimes);
        }

        public void Insert(IEnumerable<Baseline> baselines)
        {
            _baselineCollection.InsertBatch(baselines);
        }

        internal void DeleteMeasureFile(int measureFileId)
        {
            throw new NotImplementedException();
        }
    }
}
