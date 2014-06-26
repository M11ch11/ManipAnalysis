using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ManipAnalysis_v2.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace ManipAnalysis_v2
{
    internal class MongoDbWrapper
    {
        private readonly ManipAnalysisGui _myManipAnalysisGui;
        private MongoCollection<Baseline> _baselineCollection;
        private string _connectionString;
        private MongoClient _mongoClient;
        private MongoDatabase _mongoDatabase;

        private string _mongoDbDatabaseString;
        private string _mongoDbPasswordString;
        private string _mongoDbServerString;
        private string _mongoDbUsernameString;
        private MongoServer _mongoServer;
        private MongoCollection<SzenarioMeanTime> _szenarioMeanTimeCollection;
        private MongoCollection<Trial> _trialCollection;

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
                #region TrialCollection indexes
                /*
                 * GetTrial(string studyName, string groupName, string szenarioName,
                 *           SubjectContainer subject, DateTime turn, int target, 
                 *          int trial, bool showNormalTrials, bool showCatchTrials,
                 *          bool showErrorclampTrials)
                 *          
                 * GetTrial(string studyName, string groupName, string szenarioName, 
                 *          SubjectContainer subject, DateTime turn, int szenarioTrial)
                 */
                var trialCollectionKeys = new IndexKeysBuilder<Trial>();
                trialCollectionKeys.Ascending(t => t.Study);
                trialCollectionKeys.Ascending(t => t.Group);
                trialCollectionKeys.Ascending(t => t.Szenario);
                trialCollectionKeys.Ascending(t => t.Subject);
                trialCollectionKeys.Ascending(t => t.MeasureFile.CreationTime);
                trialCollectionKeys.Ascending(t => t.TrialNumberInSzenario);
                trialCollectionKeys.Ascending(t => t.Target.Number);
                trialCollectionKeys.Ascending(t => t.TargetTrialNumberInSzenario);
                trialCollectionKeys.Ascending(t => t.TrialType);
                trialCollectionKeys.Ascending(t => t.ForceFieldType);
                var options = new IndexOptionsBuilder();
                options.SetSparse(true);
                options.SetUnique(true);
                options.SetName("GetTrial");
                _trialCollection.EnsureIndex(trialCollectionKeys, options);

                /*
                 * GetStudys
                 */
                trialCollectionKeys = new IndexKeysBuilder<Trial>();
                trialCollectionKeys.Ascending(t => t.Study);
                options = new IndexOptionsBuilder();
                options.SetSparse(true);
                options.SetUnique(false);
                options.SetName("GetStudys");
                _trialCollection.EnsureIndex(trialCollectionKeys, options);

                /*
                 * GetGroups
                 */
                trialCollectionKeys = new IndexKeysBuilder<Trial>();
                trialCollectionKeys.Ascending(t => t.Study);
                trialCollectionKeys.Ascending(t => t.Group);
                options = new IndexOptionsBuilder();
                options.SetSparse(true);
                options.SetUnique(false);
                options.SetName("GetGroups");
                _trialCollection.EnsureIndex(trialCollectionKeys, options);

                /*
                 * GetSzenarios
                 */
                trialCollectionKeys = new IndexKeysBuilder<Trial>();
                trialCollectionKeys.Ascending(t => t.Study);
                trialCollectionKeys.Ascending(t => t.Group);
                trialCollectionKeys.Ascending(t => t.Szenario);
                options = new IndexOptionsBuilder();
                options.SetSparse(true);
                options.SetUnique(false);
                options.SetName("GetSzenarios");
                _trialCollection.EnsureIndex(trialCollectionKeys, options);

                /*
                 * GetSubjects
                 */
                trialCollectionKeys = new IndexKeysBuilder<Trial>();
                trialCollectionKeys.Ascending(t => t.Study);
                trialCollectionKeys.Ascending(t => t.Group);
                trialCollectionKeys.Ascending(t => t.Szenario);
                trialCollectionKeys.Ascending(t => t.Subject);
                options = new IndexOptionsBuilder();
                options.SetSparse(true);
                options.SetUnique(false);
                options.SetName("GetSubjects");
                _trialCollection.EnsureIndex(trialCollectionKeys, options);

                /*
                 * GetTurns
                 */
                trialCollectionKeys = new IndexKeysBuilder<Trial>();
                trialCollectionKeys.Ascending(t => t.Study);
                trialCollectionKeys.Ascending(t => t.Group);
                trialCollectionKeys.Ascending(t => t.Szenario);
                trialCollectionKeys.Ascending(t => t.Subject);
                trialCollectionKeys.Ascending(t => t.MeasureFile.CreationTime);
                options = new IndexOptionsBuilder();
                options.SetSparse(true);
                options.SetUnique(false);
                options.SetName("GetTurns");
                _trialCollection.EnsureIndex(trialCollectionKeys, options);

                /*
                 * GetSzenario-/Target/TargetTrials/CatchTrial/ErrorClampTrial
                 */
                trialCollectionKeys = new IndexKeysBuilder<Trial>();
                trialCollectionKeys.Ascending(t => t.Study);
                trialCollectionKeys.Ascending(t => t.Szenario);
                trialCollectionKeys.Ascending(t => t.Target.Number);
                trialCollectionKeys.Ascending(t => t.TargetTrialNumberInSzenario);
                trialCollectionKeys.Ascending(t => t.ForceFieldType);
                trialCollectionKeys.Ascending(t => t.TrialType);
                trialCollectionKeys.Ascending(t => t.Handedness);
                options = new IndexOptionsBuilder();
                options.SetSparse(true);
                options.SetUnique(false);
                options.SetName("GetSzenario-/Target/TargetTrials/CatchTrial/ErrorClampTrial");
                _trialCollection.EnsureIndex(trialCollectionKeys, options);

                // MeasureFileHash
                trialCollectionKeys = new IndexKeysBuilder<Trial>();
                trialCollectionKeys.Ascending(t => t.MeasureFile.FileHash);
                options = new IndexOptionsBuilder();
                options.SetSparse(false);
                options.SetUnique(false);
                options.SetName("MeasureFileHash");
                _trialCollection.EnsureIndex(trialCollectionKeys, options);
                #endregion

                #region BaselineCollection indexes
                /*
                 * GetBaseline
                 */
                var baselineCollectionKeys = new IndexKeysBuilder<Baseline>();
                baselineCollectionKeys.Ascending(t => t.Study);
                baselineCollectionKeys.Ascending(t => t.Group);
                baselineCollectionKeys.Ascending(t => t.Subject);
                baselineCollectionKeys.Ascending(t => t.Target.Number);
                options = new IndexOptionsBuilder();
                options.SetSparse(true);
                options.SetUnique(false);
                options.SetName("GetBaseline");
                _baselineCollection.EnsureIndex(baselineCollectionKeys, options);

                // MeasureFileHash
                baselineCollectionKeys = new IndexKeysBuilder<Baseline>();
                baselineCollectionKeys.Ascending(t => t.MeasureFile.FileHash);
                options = new IndexOptionsBuilder();
                options.SetSparse(false);
                options.SetUnique(false);
                options.SetName("MeasureFileHash");
                _baselineCollection.EnsureIndex(trialCollectionKeys, options);
                #endregion

                #region SzenarioMeanTimeCollection indexes
                /*
                 * GetSzenarioMeanTime
                 */
                var szenarioMeanTimeCollectionKeys = new IndexKeysBuilder<SzenarioMeanTime>();
                szenarioMeanTimeCollectionKeys.Ascending(t => t.Study);
                szenarioMeanTimeCollectionKeys.Ascending(t => t.Group);
                szenarioMeanTimeCollectionKeys.Ascending(t => t.Szenario);
                szenarioMeanTimeCollectionKeys.Ascending(t => t.Subject);
                szenarioMeanTimeCollectionKeys.Ascending(t => t.MeasureFile.CreationTime);
                options = new IndexOptionsBuilder();
                options.SetSparse(true);
                options.SetUnique(false);
                options.SetName("GetSzenarioMeanTime");
                _szenarioMeanTimeCollection.EnsureIndex(szenarioMeanTimeCollectionKeys, options);

                // MeasureFileHash
                szenarioMeanTimeCollectionKeys = new IndexKeysBuilder<SzenarioMeanTime>();
                szenarioMeanTimeCollectionKeys.Ascending(t => t.MeasureFile.FileHash);
                options = new IndexOptionsBuilder();
                options.SetSparse(false);
                options.SetUnique(false);
                options.SetName("MeasureFileHash");
                _szenarioMeanTimeCollection.EnsureIndex(trialCollectionKeys, options);
                #endregion
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
                return
                    _trialCollection.AsQueryable()
                        .Where(t => t.Study == studyName && t.Group == groupName)
                        .Select(t => t.Szenario)
                        .Distinct();
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
                return
                    _trialCollection.AsQueryable()
                        .Where(t => t.Study == studyName && t.Group == groupName && t.Szenario == szenarioName)
                        .Select(t => t.Subject)
                        .Distinct();
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
                return
                    _trialCollection.AsQueryable()
                        .Where(
                            t =>
                                t.Study == studyName && t.Group == groupName && t.Szenario == szenarioName && t.Subject == subject)
                        .Select(t => t.MeasureFile.CreationTime)
                        .Distinct();
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
                return
                    _trialCollection.AsQueryable()
                        .Where(t => t.Study == studyName && t.Szenario == szenarioName)
                        .Select(t => t.Target.Number)
                        .Distinct();
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
                return
                    _trialCollection.AsQueryable()
                        .Where(t => t.Study == studyName && t.Szenario == szenarioName)
                        .Select(t => t.TargetTrialNumberInSzenario)
                        .Distinct();
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox("MongoDbwrapper::GetTargetTrials: " + ex);
                return new List<int>();
            }
        }

        public IEnumerable<int> GetSzenarioTrials(string studyName, string szenarioName, IEnumerable<MongoDb.Trial.TrialTypeEnum> trialTypes, IEnumerable<MongoDb.Trial.ForceFieldTypeEnum> forceFields, IEnumerable<MongoDb.Trial.HandednessEnum> handedness)
        {
            IEnumerable<int> retVal;
            try
            {
                retVal =
                        _trialCollection.AsQueryable()
                            .Where(t => t.Study == studyName && t.Szenario == szenarioName && trialTypes.Contains(t.TrialType) && forceFields.Contains(t.ForceFieldType) && handedness.Contains(t.Handedness))
                            .Select(t => t.TrialNumberInSzenario)
                            .Distinct();
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox("MongoDbwrapper::GetSzenarioTrials: " + ex);
                return new List<int>();
            }

            return retVal;
        }

        public IEnumerable<Trial> GetTrial(string studyName, string groupName, string szenarioName,
            SubjectContainer subject, DateTime turn, int target, IEnumerable<int> targetTrials, IEnumerable<MongoDb.Trial.TrialTypeEnum> trialTypes, 
            IEnumerable<MongoDb.Trial.ForceFieldTypeEnum> forceFields, IEnumerable<MongoDb.Trial.HandednessEnum> handedness, FieldsBuilder<Trial> fields)
        {
            IEnumerable<Trial> retVal;
            try
            {
                retVal = _trialCollection.FindAs<Trial>(Query<Trial>.Where(t =>
                        t.Study == studyName && t.Group == groupName && t.Szenario == szenarioName &&
                        t.Subject == subject && t.MeasureFile.CreationTime == turn && t.Target.Number == target &&
                        targetTrials.Contains(t.TargetTrialNumberInSzenario) && trialTypes.Contains(t.TrialType) && 
                        forceFields.Contains(t.ForceFieldType) && handedness.Contains(t.Handedness)))
                        .SetFields(fields).OrderBy(t => t.TargetTrialNumberInSzenario);
            }
            catch (Exception)
            {
                retVal = null;
            }

            return retVal;
        }

        public IEnumerable<Trial> GetTrials(string studyName, string groupName, string szenarioName, SubjectContainer subject, DateTime turn, IEnumerable<int> szenarioTrials, FieldsBuilder<Trial> fields)
        {
            IEnumerable<Trial> retVal;
            try
            {
                retVal = _trialCollection.FindAs<Trial>(Query<Trial>.Where(t =>
                    t.Study == studyName && t.Group == groupName && t.Szenario == szenarioName &&
                    t.Subject == subject && t.MeasureFile.CreationTime == turn && szenarioTrials.Contains(t.TrialNumberInSzenario)))
                    .SetFields(fields).OrderBy(t => t.TrialNumberInSzenario);
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                retVal = null;
            }

            return retVal;
        }

        public IEnumerable<Trial> GetTrials(string studyName, string groupName, string szenarioName, SubjectContainer subject, DateTime turn, IEnumerable<int> szenarioTrials, IEnumerable<MongoDb.Trial.TrialTypeEnum> trialTypes,
            IEnumerable<MongoDb.Trial.ForceFieldTypeEnum> forceFields, IEnumerable<MongoDb.Trial.HandednessEnum> handedness, FieldsBuilder<Trial> fields)
        {
            IEnumerable<Trial> retVal;
            try
            {
                retVal = _trialCollection.FindAs<Trial>(Query<Trial>.Where(t =>
                    t.Study == studyName && t.Group == groupName && t.Szenario == szenarioName &&
                    t.Subject == subject && t.MeasureFile.CreationTime == turn && szenarioTrials.Contains(t.TrialNumberInSzenario) && trialTypes.Contains(t.TrialType) &&
                        forceFields.Contains(t.ForceFieldType) && handedness.Contains(t.Handedness)))
                    .SetFields(fields).OrderBy(t => t.TrialNumberInSzenario);
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                retVal = null;
            }

            return retVal;
        }

        public WriteConcernResult UpdateTrialStatisticsAndBaselineId(Trial trial)
        {
            IMongoQuery query = Query<Trial>.EQ(t => t.Id, trial.Id);
            UpdateBuilder<Trial> update = Update<Trial>.Set(t => t.ZippedStatistics, trial.ZippedStatistics)
                .Set(t => t.BaselineObjectId, trial.BaselineObjectId);
            return _trialCollection.Update(query, update);
        }

        public IEnumerable<Trial> GetTrialsWithoutStatistics(FieldsBuilder<Trial> statisticFields)
        {
            return _trialCollection.FindAs<Trial>(Query<Trial>.Where(t => t.ZippedStatistics == null)).SetFields(statisticFields);
        }

        public Baseline GetBaseline(string study, string group, SubjectContainer subject, int targetNumber,
            FieldsBuilder<Baseline> baselineFields)
        {
            try
            {
                return
                    _baselineCollection.FindAs<Baseline>(
                        Query<Trial>.Where(
                            t => t.Study == study && t.Group == group && t.Subject == subject && t.Target.Number == targetNumber))
                        .SetFields(baselineFields)
                        .SetLimit(1)
                        .First();
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
                return
                    _baselineCollection.FindAs<Baseline>(
                        Query<Trial>.Where(t => t.Study == study && t.Group == group && t.Subject == subject))
                        .SetFields(baselineFields)
                        .ToArray();
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
                return
                    _baselineCollection.FindAs<Baseline>(Query<Trial>.Where(t => t.Id == objectId))
                        .SetFields(baselineFields)
                        .SetLimit(1)
                        .First();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public SzenarioMeanTime[] GetSzenarioMeanTime(string study, string group, string szenario, SubjectContainer subject,
            DateTime turn)
        {
            try
            {
                return
                    _szenarioMeanTimeCollection.FindAs<SzenarioMeanTime>(
                        Query<Trial>.Where(
                            t =>
                                t.Study == study && t.Group == group && t.Szenario == szenario && t.Subject == subject &&
                                t.MeasureFile.CreationTime == turn)).ToArray();
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
                return _trialCollection.AsQueryable().Any(t => t.MeasureFile.FileHash == measureFileHash);
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

        public void DropStatistics()
        {
            IMongoQuery query = Query<Trial>.NE(t => t.ZippedStatistics, null);
            UpdateBuilder<Trial> update = Update<Trial>.Set(t => t.ZippedStatistics, null);
            _trialCollection.Update(query, update, UpdateFlags.Multi);
        }
    }
}