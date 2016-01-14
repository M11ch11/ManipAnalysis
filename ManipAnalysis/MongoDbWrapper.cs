using System;
using System.Collections.Generic;
using System.Linq;
using ManipAnalysis_v2.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ManipAnalysis_v2
{
    internal class MongoDbWrapper
    {
        private readonly ManipAnalysisGui _myManipAnalysisGui;

        private IMongoCollection<Baseline> _baselineCollection;

        private string _connectionString;

        private MongoClient _mongoClient;

        private IMongoDatabase _mongoDatabase;

        private string _mongoDbDatabaseString;

        private string _mongoDbPasswordString;

        private string _mongoDbServerString;

        private string _mongoDbUsernameString;

        private IMongoCollection<SzenarioMeanTime> _szenarioMeanTimeCollection;

        private IMongoCollection<Trial> _trialCollection;

        public MongoDbWrapper(ManipAnalysisGui myManipAnalysisGui)
        {
            _myManipAnalysisGui = myManipAnalysisGui;
            _mongoDbDatabaseString = "admin";
            _mongoDbPasswordString = "!sport12";
            _mongoDbServerString = "localhost";
            _mongoDbUsernameString = "DataAccess";
        }

        public IEnumerable<string> GetDatabases()
        {
            List<string> databasesList;
            using (var cursor = _mongoClient.ListDatabases())
            {
                databasesList = cursor.ToList().Select(t => t["name"].AsString).ToList();
            }
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
                                    _mongoDbServerString + "/" + _mongoDbDatabaseString;
                _mongoClient = new MongoClient(_connectionString);
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
                _mongoDatabase = _mongoClient.GetDatabase(database);
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
                var trialCollectionKeys = Builders<Trial>.IndexKeys
                    .Ascending(t => t.Study)
                    .Ascending(t => t.Group)
                    .Ascending(t => t.Szenario)
                    .Ascending(t => t.Subject)
                    .Ascending(t => t.MeasureFile.CreationTime)
                    .Ascending(t => t.TrialNumberInSzenario)
                    .Ascending(t => t.Target.Number)
                    .Ascending(t => t.TargetTrialNumberInSzenario)
                    .Ascending(t => t.TrialType)
                    .Ascending(t => t.ForceFieldType);
                var options = new CreateIndexOptions {Unique = true, Sparse = true, Name = "1"};
                _trialCollection.Indexes.CreateOne(trialCollectionKeys, options);

                /*
                 * GetStudys
                 */
                trialCollectionKeys = Builders<Trial>.IndexKeys
                    .Ascending(t => t.Study);
                options = new CreateIndexOptions {Unique = false, Sparse = true, Name = "2"};
                _trialCollection.Indexes.CreateOne(trialCollectionKeys, options);

                /*
                 * GetGroups
                 */
                trialCollectionKeys = Builders<Trial>.IndexKeys
                    .Ascending(t => t.Study)
                    .Ascending(t => t.Group);
                options = new CreateIndexOptions {Unique = false, Sparse = true, Name = "3"};
                _trialCollection.Indexes.CreateOne(trialCollectionKeys, options);

                /*
                 * GetSzenarios
                 */
                trialCollectionKeys = Builders<Trial>.IndexKeys
                    .Ascending(t => t.Study)
                    .Ascending(t => t.Group)
                    .Ascending(t => t.Szenario);
                options = new CreateIndexOptions {Unique = false, Sparse = true, Name = "4"};
                _trialCollection.Indexes.CreateOne(trialCollectionKeys, options);

                /*
                 * GetSubjects
                 */
                trialCollectionKeys = Builders<Trial>.IndexKeys
                    .Ascending(t => t.Study)
                    .Ascending(t => t.Group)
                    .Ascending(t => t.Szenario)
                    .Ascending(t => t.Subject);
                options = new CreateIndexOptions {Unique = false, Sparse = true, Name = "5"};
                _trialCollection.Indexes.CreateOne(trialCollectionKeys, options);

                /*
                 * GetTurns
                 */
                trialCollectionKeys = Builders<Trial>.IndexKeys
                    .Ascending(t => t.Study)
                    .Ascending(t => t.Group)
                    .Ascending(t => t.Szenario)
                    .Ascending(t => t.Subject)
                    .Ascending(t => t.MeasureFile.CreationTime);
                options = new CreateIndexOptions {Unique = false, Sparse = true, Name = "6"};
                _trialCollection.Indexes.CreateOne(trialCollectionKeys, options);

                /*
                 * GetTargets
                 */
                trialCollectionKeys = Builders<Trial>.IndexKeys
                    .Ascending(t => t.Study)
                    .Ascending(t => t.Szenario)
                    .Ascending(t => t.Target.Number);
                options = new CreateIndexOptions {Unique = false, Sparse = true, Name = "7"};
                _trialCollection.Indexes.CreateOne(trialCollectionKeys, options);

                /*
                 * GetSzenario-/Target/TargetTrials/CatchTrial/ErrorClampTrial
                 */
                trialCollectionKeys = Builders<Trial>.IndexKeys
                    .Ascending(t => t.Study)
                    .Ascending(t => t.Szenario)
                    .Ascending(t => t.Target.Number)
                    .Ascending(t => t.TargetTrialNumberInSzenario)
                    .Ascending(t => t.ForceFieldType)
                    .Ascending(t => t.TrialType)
                    .Ascending(t => t.Handedness);
                options = new CreateIndexOptions {Unique = false, Sparse = true, Name = "8"};
                _trialCollection.Indexes.CreateOne(trialCollectionKeys, options);

                /*
                 * 
                 */
                trialCollectionKeys = Builders<Trial>.IndexKeys
                    .Ascending(t => t.Study)
                    .Ascending(t => t.Szenario)
                    .Ascending(t => t.ForceFieldType)
                    .Ascending(t => t.TrialType)
                    .Ascending(t => t.Handedness)
                    .Ascending(t => t.TargetTrialNumberInSzenario);
                options = new CreateIndexOptions {Unique = false, Sparse = true, Name = "9"};
                _trialCollection.Indexes.CreateOne(trialCollectionKeys, options);

                /*
                 * 
                 */
                trialCollectionKeys = Builders<Trial>.IndexKeys
                    .Ascending(t => t.Study)
                    .Ascending(t => t.Szenario)
                    .Ascending(t => t.Subject)
                    .Ascending(t => t.ForceFieldType)
                    .Ascending(t => t.TrialType)
                    .Ascending(t => t.Handedness)
                    .Ascending(t => t.TrialNumberInSzenario);
                options = new CreateIndexOptions {Unique = false, Sparse = true, Name = "10"};
                _trialCollection.Indexes.CreateOne(trialCollectionKeys, options);

                // MeasureFileHash
                trialCollectionKeys = Builders<Trial>.IndexKeys
                    .Ascending(t => t.MeasureFile.FileHash);
                options = new CreateIndexOptions {Unique = false, Sparse = false, Name = "11"};
                _trialCollection.Indexes.CreateOne(trialCollectionKeys, options);

                #endregion

                #region BaselineCollection indexes

                /*
                 * GetBaseline
                 */
                var baselineCollectionKeys = Builders<Baseline>.IndexKeys
                    .Ascending(t => t.Study)
                    .Ascending(t => t.Group)
                    .Ascending(t => t.Subject)
                    .Ascending(t => t.Target.Number);
                options = new CreateIndexOptions {Unique = false, Sparse = true, Name = "GetBaseline"};
                _baselineCollection.Indexes.CreateOne(baselineCollectionKeys, options);

                // MeasureFileHash
                baselineCollectionKeys = Builders<Baseline>.IndexKeys.Ascending(t => t.MeasureFile.FileHash);
                options = new CreateIndexOptions {Unique = false, Sparse = false, Name = "MeasureFileHash"};
                _baselineCollection.Indexes.CreateOne(baselineCollectionKeys, options);

                #endregion

                #region SzenarioMeanTimeCollection indexes

                /*
                 * GetSzenarioMeanTime
                 */
                var szenarioMeanTimeCollectionKeys = Builders<SzenarioMeanTime>.IndexKeys
                    .Ascending(t => t.Study)
                    .Ascending(t => t.Group)
                    .Ascending(t => t.Szenario)
                    .Ascending(t => t.Subject)
                    .Ascending(t => t.MeasureFile.CreationTime);
                options = new CreateIndexOptions {Unique = false, Sparse = true, Name = "GetSzenarioMeanTime"};
                _szenarioMeanTimeCollection.Indexes.CreateOne(szenarioMeanTimeCollectionKeys, options);

                // MeasureFileHash
                szenarioMeanTimeCollectionKeys = Builders<SzenarioMeanTime>.IndexKeys
                    .Ascending(t => t.MeasureFile.FileHash);
                options = new CreateIndexOptions {Unique = false, Sparse = false, Name = "MeasureFileHash"};
                _szenarioMeanTimeCollection.Indexes.CreateOne(szenarioMeanTimeCollectionKeys, options);

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
                DropAllIndexes();
                EnsureIndexes();
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
                _trialCollection.Indexes.DropAll();
                _baselineCollection.Indexes.DropAll();
                _szenarioMeanTimeCollection.Indexes.DropAll();
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
                _myManipAnalysisGui.WriteToLogBox("Unimplemented");
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

        public void DropBaselines()
        {
            try
            {
                _mongoDatabase.DropCollection("Baseline");
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
                var retVal = _trialCollection.Distinct(t => t.Study, FilterDefinition<Trial>.Empty).ToList();
                //_myManipAnalysisGui.WriteToLogBox(TicToc.Toc() + "ms \t" + "GetStudys()");

                return retVal;
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
                var filter = Builders<Trial>.Filter.Eq(t => t.Study, studyName);

                //TicToc.Tic();
                var retVal =
                    _trialCollection.Aggregate()
                        .Match(filter)
                        .Group(t => t.Group, u => new {u.Key})
                        .ToList()
                        .Select(t => t.Key);
                //_myManipAnalysisGui.WriteToLogBox(TicToc.Toc() + "ms \t" + "GetGroups(string studyName)");

                return retVal;
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
                var filter = Builders<Trial>.Filter.And(Builders<Trial>.Filter.Eq(t => t.Study, studyName),
                    Builders<Trial>.Filter.Eq(t => t.Group, groupName));

                //TicToc.Tic();
                var retVal =
                    _trialCollection.Aggregate()
                        .Match(filter)
                        .Group(t => t.Szenario, u => new {u.Key})
                        .ToList()
                        .Select(t => t.Key);
                //_myManipAnalysisGui.WriteToLogBox(TicToc.Toc() + "ms \t" + "GetSzenarios(string studyName, string groupName)");

                return retVal;
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox("MongoDbwrapper::GetSzenarios: " + ex);
                return new List<string>();
            }
        }

        public IEnumerable<string> GetSzenarios(string studyName, string groupName, SubjectContainer subject)
        {
            try
            {
                var filter = Builders<Trial>.Filter.And(Builders<Trial>.Filter.Eq(t => t.Study, studyName),
                    Builders<Trial>.Filter.Eq(t => t.Group, groupName),
                    Builders<Trial>.Filter.Eq(t => t.Subject, subject));

                //TicToc.Tic();
                var retVal =
                    _trialCollection.Aggregate()
                        .Match(filter)
                        .Group(t => t.Szenario, u => new {u.Key})
                        .ToList()
                        .Select(t => t.Key);
                //_myManipAnalysisGui.WriteToLogBox(TicToc.Toc() + "ms \t" + "GetSzenarios(string studyName, string groupName, SubjectContainer subject)");

                return retVal;
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
                var filter = Builders<Trial>.Filter.And(Builders<Trial>.Filter.Eq(t => t.Study, studyName),
                    Builders<Trial>.Filter.Eq(t => t.Group, groupName),
                    Builders<Trial>.Filter.Eq(t => t.Szenario, szenarioName));

                //TicToc.Tic();
                var retVal =
                    _trialCollection.Aggregate()
                        .Match(filter)
                        .Group(t => t.Subject, u => new {u.Key})
                        .ToList()
                        .Select(t => t.Key);
                //_myManipAnalysisGui.WriteToLogBox(TicToc.Toc() + "ms \t" + "GetSubjects(string studyName, string groupName, string szenarioName)");

                return retVal;
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox("MongoDbwrapper::GetSubjects: " + ex);
                return new List<SubjectContainer>();
            }
        }

        public IEnumerable<SubjectContainer> GetSubjects(string studyName, string groupName)
        {
            try
            {
                var filter = Builders<Trial>.Filter.And(Builders<Trial>.Filter.Eq(t => t.Study, studyName),
                    Builders<Trial>.Filter.Eq(t => t.Group, groupName));

                //TicToc.Tic();
                var retVal =
                    _trialCollection.Aggregate()
                        .Match(filter)
                        .Group(t => t.Subject, u => new {u.Key})
                        .ToList()
                        .Select(t => t.Key);
                //_myManipAnalysisGui.WriteToLogBox(TicToc.Toc() + "ms \t" + "GetSubjects(string studyName, string groupName)");

                return retVal;
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox("MongoDbwrapper::GetSubjects: " + ex);
                return new List<SubjectContainer>();
            }
        }

        public IEnumerable<DateTime> GetTurns(string studyName, string groupName, string szenarioName,
            SubjectContainer subject)
        {
            try
            {
                var filter = Builders<Trial>.Filter.And(Builders<Trial>.Filter.Eq(t => t.Study, studyName),
                    Builders<Trial>.Filter.Eq(t => t.Group, groupName),
                    Builders<Trial>.Filter.Eq(t => t.Szenario, szenarioName),
                    Builders<Trial>.Filter.Eq(t => t.Subject, subject));

                //TicToc.Tic();
                var retVal =
                    _trialCollection.Aggregate()
                        .Match(filter)
                        .Group(t => t.MeasureFile.CreationTime, u => new {u.Key})
                        .ToList()
                        .Select(t => t.Key);
                //_myManipAnalysisGui.WriteToLogBox(TicToc.Toc() + "ms \t" + "GetTurns(string studyName, string groupName, string szenarioName, SubjectContainer subject)");

                return retVal;
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox("MongoDbwrapper::GetTurns: " + ex);
                return new List<DateTime>();
            }
        }

        public IEnumerable<DateTime> GetTurns(string studyName, string[] groupNames, string szenarioName,
            SubjectContainer subject)
        {
            try
            {
                var filter = Builders<Trial>.Filter.And(Builders<Trial>.Filter.Eq(t => t.Study, studyName),
                    Builders<Trial>.Filter.In(t => t.Group, groupNames),
                    Builders<Trial>.Filter.Eq(t => t.Szenario, szenarioName),
                    Builders<Trial>.Filter.Eq(t => t.Subject, subject));

                //TicToc.Tic();
                var retVal =
                    _trialCollection.Aggregate()
                        .Match(filter)
                        .Group(t => t.MeasureFile.CreationTime, u => new {u.Key})
                        .ToList()
                        .Select(t => t.Key);
                //_myManipAnalysisGui.WriteToLogBox(TicToc.Toc() + "ms \t" + "GetTurns(string studyName, string[] groupNames, string szenarioName, SubjectContainer subject)");

                return retVal;
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox("MongoDbwrapper::GetTurns: " + ex);
                return new List<DateTime>();
            }
        }

        public IEnumerable<int> GetTargets(string studyName, string groupName, string szenarioName,
            SubjectContainer subject)
        {
            try
            {
                var filter = Builders<Trial>.Filter.And(Builders<Trial>.Filter.Eq(t => t.Study, studyName),
                    Builders<Trial>.Filter.Eq(t => t.Group, groupName),
                    Builders<Trial>.Filter.Eq(t => t.Szenario, szenarioName),
                    Builders<Trial>.Filter.Eq(t => t.Subject, subject));

                //TicToc.Tic();
                var retVal =
                    _trialCollection.Aggregate()
                        .Match(filter)
                        .Group(t => t.Target.Number, u => new {u.Key})
                        .ToList()
                        .Select(t => t.Key);
                //_myManipAnalysisGui.WriteToLogBox(TicToc.Toc() + "ms \t" + "GetTargets(string studyName, string groupName, string szenarioName, SubjectContainer subject)");

                return retVal;
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox("MongoDbwrapper::GetTargets: " + ex);
                return new List<int>();
            }
        }

        public IEnumerable<int> GetTargetTrials(string studyName, string groupName, string szenarioName,
            SubjectContainer subject)
        {
            try
            {
                var filter = Builders<Trial>.Filter.And(Builders<Trial>.Filter.Eq(t => t.Study, studyName),
                    Builders<Trial>.Filter.Eq(t => t.Group, groupName),
                    Builders<Trial>.Filter.Eq(t => t.Szenario, szenarioName),
                    Builders<Trial>.Filter.Eq(t => t.Subject, subject));

                //TicToc.Tic();
                var retVal =
                    _trialCollection.Aggregate()
                        .Match(filter)
                        .Group(t => t.TargetTrialNumberInSzenario, u => new {u.Key})
                        .ToList()
                        .Select(t => t.Key);
                //_myManipAnalysisGui.WriteToLogBox(TicToc.Toc() + "ms \t" + "GetTargetTrials(string studyName, string groupName, string szenarioName, SubjectContainer subject)");

                return retVal;
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox("MongoDbwrapper::GetTargetTrials: " + ex);
                return new List<int>();
            }
        }

        public IEnumerable<int> GetSzenarioTrials(string studyName, string groupName, string szenarioName,
            SubjectContainer subject, IEnumerable<Trial.TrialTypeEnum> trialTypes,
            IEnumerable<Trial.ForceFieldTypeEnum> forceFields, IEnumerable<Trial.HandednessEnum> handedness)
        {
            IEnumerable<int> retVal;
            try
            {
                var filter = Builders<Trial>.Filter.And(Builders<Trial>.Filter.Eq(t => t.Study, studyName),
                    Builders<Trial>.Filter.Eq(t => t.Group, groupName),
                    Builders<Trial>.Filter.Eq(t => t.Szenario, szenarioName),
                    Builders<Trial>.Filter.Eq(t => t.Subject, subject),
                    Builders<Trial>.Filter.In(t => t.TrialType, trialTypes),
                    Builders<Trial>.Filter.In(t => t.ForceFieldType, forceFields),
                    Builders<Trial>.Filter.In(t => t.Handedness, handedness));

                //TicToc.Tic();
                retVal =
                    _trialCollection.Aggregate()
                        .Match(filter)
                        .Group(t => t.TrialNumberInSzenario, u => new {u.Key})
                        .ToList()
                        .Select(t => t.Key);
                //_myManipAnalysisGui.WriteToLogBox(TicToc.Toc() + "ms \t" + "GetSzenarioTrials(string studyName, string szenarioName, IEnumerable<Trial.TrialTypeEnum> trialTypes, IEnumerable<Trial.ForceFieldTypeEnum> forceFields, IEnumerable<Trial.HandednessEnum> handedness)");
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox("MongoDbwrapper::GetSzenarioTrials: " + ex);
                return new List<int>();
            }

            return retVal;
        }

        public IEnumerable<Trial> GetTrials(string studyName, string groupName, string szenarioName,
            SubjectContainer subject, DateTime turn, int target, IEnumerable<int> targetTrials,
            IEnumerable<Trial.TrialTypeEnum> trialTypes, IEnumerable<Trial.ForceFieldTypeEnum> forceFields,
            IEnumerable<Trial.HandednessEnum> handedness, ProjectionDefinition<Trial> fields)
        {
            IEnumerable<Trial> retVal;
            try
            {
                fields = fields.Include(t => t.TargetTrialNumberInSzenario);
                retVal = _trialCollection
                    .Find(
                        t =>
                            t.Study == studyName && t.Group == groupName && t.Szenario == szenarioName &&
                            t.Subject == subject && t.MeasureFile.CreationTime == turn && t.Target.Number == target &&
                            targetTrials.Contains(t.TargetTrialNumberInSzenario) && trialTypes.Contains(t.TrialType) &&
                            forceFields.Contains(t.ForceFieldType) && handedness.Contains(t.Handedness))
                    .Project<Trial>(fields)
                    .ToList()
                    .OrderBy(t => t.TargetTrialNumberInSzenario);
            }
            catch (Exception)
            {
                retVal = null;
            }

            return retVal;
        }

        public IEnumerable<Trial> GetTrials(string studyName, string groupName, string szenarioName,
            SubjectContainer subject, DateTime turn, int target, IEnumerable<int> targetTrials,
            ProjectionDefinition<Trial> fields)
        {
            IEnumerable<Trial> retVal;
            try
            {
                fields = fields.Include(t => t.TargetTrialNumberInSzenario);
                retVal = _trialCollection
                    .Find(
                        t =>
                            t.Study == studyName && t.Group == groupName && t.Szenario == szenarioName &&
                            t.Subject == subject && t.MeasureFile.CreationTime == turn && t.Target.Number == target &&
                            targetTrials.Contains(t.TargetTrialNumberInSzenario))
                    .Project<Trial>(fields)
                    .ToList()
                    .OrderBy(t => t.TargetTrialNumberInSzenario);
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                retVal = null;
            }

            return retVal;
        }

        public IEnumerable<Trial> GetTrials(string studyName, string groupName, string szenarioName,
            SubjectContainer subject, DateTime turn, IEnumerable<int> szenarioTrials, ProjectionDefinition<Trial> fields)
        {
            IEnumerable<Trial> retVal;
            try
            {
                fields = fields.Include(t => t.TargetTrialNumberInSzenario);
                retVal = _trialCollection
                    .Find(
                        t =>
                            t.Study == studyName && t.Group == groupName && t.Szenario == szenarioName &&
                            t.Subject == subject && t.MeasureFile.CreationTime == turn &&
                            szenarioTrials.Contains(t.TrialNumberInSzenario))
                    .Project<Trial>(fields)
                    .ToList()
                    .OrderBy(t => t.TargetTrialNumberInSzenario);
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                retVal = null;
            }

            return retVal;
        }

        public IEnumerable<Trial> GetTrials(string studyName, string groupName, string szenarioName,
            SubjectContainer subject, DateTime turn, IEnumerable<int> szenarioTrials,
            IEnumerable<Trial.TrialTypeEnum> trialTypes, IEnumerable<Trial.ForceFieldTypeEnum> forceFields,
            IEnumerable<Trial.HandednessEnum> handedness, ProjectionDefinition<Trial> fields)
        {
            IEnumerable<Trial> retVal;
            try
            {
                fields = fields.Include(t => t.TargetTrialNumberInSzenario);
                retVal = _trialCollection
                    .Find(
                        t =>
                            t.Study == studyName && t.Group == groupName && t.Szenario == szenarioName &&
                            t.Subject == subject && t.MeasureFile.CreationTime == turn &&
                            szenarioTrials.Contains(t.TrialNumberInSzenario) && trialTypes.Contains(t.TrialType) &&
                            forceFields.Contains(t.ForceFieldType) && handedness.Contains(t.Handedness))
                    .Project<Trial>(fields)
                    .ToList()
                    .OrderBy(t => t.TargetTrialNumberInSzenario);
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                retVal = null;
            }

            return retVal;
        }

        public void UpdateTrialStatisticsAndBaselineId(Trial trial)
        {
            var filter = Builders<Trial>.Filter.Eq(t => t.Id, trial.Id);
            var update =
                Builders<Trial>.Update.Set(t => t.ZippedStatistics, trial.ZippedStatistics)
                    .Set(t => t.BaselineObjectId, trial.BaselineObjectId);
            _trialCollection.FindOneAndUpdate(filter, update);
        }

        public void UpdateTrialBaselineId(Trial trial)
        {
            var filter = Builders<Trial>.Filter.Eq(t => t.Id, trial.Id);
            var update = Builders<Trial>.Update.Set(t => t.BaselineObjectId, trial.BaselineObjectId);
            _trialCollection.FindOneAndUpdate(filter, update);
        }

        public void UpdateBaseline(Baseline baseline)
        {
            var filter = Builders<Baseline>.Filter.Eq(t => t.Id, baseline.Id);
            var update =
                Builders<Baseline>.Update.Set(t => t.ZippedMeasuredForces, baseline.ZippedMeasuredForces)
                    .Set(t => t.ZippedMomentForces, baseline.ZippedMomentForces)
                    .Set(t => t.ZippedNominalForces, baseline.ZippedNominalForces)
                    .Set(t => t.ZippedPosition, baseline.ZippedPosition)
                    .Set(t => t.ZippedVelocity, baseline.ZippedVelocity);
            _baselineCollection.FindOneAndUpdate(filter, update);
        }

        public IEnumerable<Trial> GetTrialsWithoutStatistics(ProjectionDefinition<Trial> statisticFields)
        {
            return _trialCollection
                .Find(t => t.ZippedStatistics == null)
                .Project<Trial>(statisticFields)
                .ToList();
        }

        public Baseline GetBaseline(string study, string group, SubjectContainer subject, int targetNumber,
            Trial.TrialTypeEnum trialType, Trial.ForceFieldTypeEnum forceField, Trial.HandednessEnum handedness)
        {
            try
            {
                return _baselineCollection
                    .Find(
                        t =>
                            t.Study == study && t.Group == group && t.Subject == subject &&
                            t.Target.Number == targetNumber && t.TrialType == trialType &&
                            t.ForceFieldType == forceField && t.Handedness == handedness)
                    .First<Baseline>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IEnumerable<Baseline> GetBaseline(string study, string group, SubjectContainer subject, int[] targets,
            IEnumerable<Trial.TrialTypeEnum> trialTypes, IEnumerable<Trial.ForceFieldTypeEnum> forceFields,
            IEnumerable<Trial.HandednessEnum> handedness, ProjectionDefinition<Baseline> baselineFields)
        {
            try
            {
                return _baselineCollection
                    .Find(
                        t =>
                            t.Study == study && t.Group == group && t.Subject == subject &&
                            targets.Contains(t.Target.Number) && trialTypes.Contains(t.TrialType) &&
                            forceFields.Contains(t.ForceFieldType) && handedness.Contains(t.Handedness))
                    .Project<Baseline>(baselineFields)
                    .ToList();
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
                return _baselineCollection.Find(t => t.Id == objectId).First<Baseline>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IEnumerable<SzenarioMeanTime> GetSzenarioMeanTime(string study, string group, string szenario,
            SubjectContainer subject, DateTime turn)
        {
            try
            {
                return _szenarioMeanTimeCollection
                    .Find(
                        t =>
                            t.Study == study && t.Group == group && t.Szenario == szenario && t.Subject == subject &&
                            t.MeasureFile.CreationTime == turn)
                    .ToList();
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
            catch (Exception
                ex)
            {
                _myManipAnalysisGui.WriteToLogBox(ex.ToString());
                return false;
            }
        }

        public void Insert(IEnumerable<Trial> trials)
        {
            // No try-catch-Block since ManipAnalysisFunctions is handling this
            _trialCollection.InsertMany(trials);
        }

        public void Insert(IEnumerable<SzenarioMeanTime> szenarioMeanTimes)
        {
            // No try-catch-Block since ManipAnalysisFunctions is handling this
            _szenarioMeanTimeCollection.InsertMany(szenarioMeanTimes);
        }

        public void Insert(IEnumerable<Baseline> baselines)
        {
            // No try-catch-Block since ManipAnalysisFunctions is handling this
            _baselineCollection.InsertMany(baselines);
        }

        public void RemoveMeasureFile(MeasureFileContainer measureFile)
        {
            try
            {
                _trialCollection.DeleteMany(Builders<Trial>.Filter.Eq(t => t.MeasureFile, measureFile));
                _szenarioMeanTimeCollection.DeleteMany(Builders<SzenarioMeanTime>.Filter.Eq(t => t.MeasureFile,
                    measureFile));
                _baselineCollection.DeleteMany(Builders<Baseline>.Filter.Eq(t => t.MeasureFile, measureFile));
            }
            catch (Exception
                ex)
            {
                _myManipAnalysisGui.WriteToLogBox(ex.ToString());
            }
        }

        public void DropStatistics()
        {
            var filter = Builders<Trial>.Filter.Ne(t => t.ZippedStatistics, null);
            var update = Builders<Trial>.Update.Set(t => t.ZippedStatistics, null);
            _trialCollection.UpdateMany(filter, update);
        }

        public void DropStatistics(Baseline baseline)
        {
            var filter = Builders<Trial>.Filter.Eq(t => t.BaselineObjectId, baseline.Id);
            var update = Builders<Trial>.Update.Set(t => t.ZippedStatistics, null);
            _trialCollection.UpdateMany(filter, update);
        }
    }
}