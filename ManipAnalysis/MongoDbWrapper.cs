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
                var options = new CreateIndexOptions { Unique = true, Sparse = true, Name = "1" };
                _trialCollection.Indexes.CreateOne(trialCollectionKeys, options);

                /*
                 * GetStudys
                 */
                trialCollectionKeys = Builders<Trial>.IndexKeys
                    .Ascending(t => t.Study);
                options = new CreateIndexOptions { Unique = false, Sparse = true, Name = "2" };
                _trialCollection.Indexes.CreateOne(trialCollectionKeys, options);

                /*
                 * GetGroups
                 */
                trialCollectionKeys = Builders<Trial>.IndexKeys
                    .Ascending(t => t.Study)
                    .Ascending(t => t.Group);
                options = new CreateIndexOptions { Unique = false, Sparse = true, Name = "3" };
                _trialCollection.Indexes.CreateOne(trialCollectionKeys, options);

                /*
                 * GetSzenarios
                 */
                trialCollectionKeys = Builders<Trial>.IndexKeys
                    .Ascending(t => t.Study)
                    .Ascending(t => t.Group)
                    .Ascending(t => t.Szenario);
                options = new CreateIndexOptions { Unique = false, Sparse = true, Name = "4" };
                _trialCollection.Indexes.CreateOne(trialCollectionKeys, options);

                /*
                 * GetSubjects
                 */
                trialCollectionKeys = Builders<Trial>.IndexKeys
                    .Ascending(t => t.Study)
                    .Ascending(t => t.Group)
                    .Ascending(t => t.Szenario)
                    .Ascending(t => t.Subject);
                options = new CreateIndexOptions { Unique = false, Sparse = true, Name = "5" };
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
                options = new CreateIndexOptions { Unique = false, Sparse = true, Name = "6" };
                _trialCollection.Indexes.CreateOne(trialCollectionKeys, options);

                /*
                 * GetTargets
                 */
                trialCollectionKeys = Builders<Trial>.IndexKeys
                    .Ascending(t => t.Study)
                    .Ascending(t => t.Szenario)
                    .Ascending(t => t.Target.Number);
                options = new CreateIndexOptions { Unique = false, Sparse = true, Name = "7" };
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
                options = new CreateIndexOptions { Unique = false, Sparse = true, Name = "8" };
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
                options = new CreateIndexOptions { Unique = false, Sparse = true, Name = "9" };
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
                options = new CreateIndexOptions { Unique = false, Sparse = true, Name = "10" };
                _trialCollection.Indexes.CreateOne(trialCollectionKeys, options);

                // MeasureFileHash
                trialCollectionKeys = Builders<Trial>.IndexKeys
                    .Ascending(t => t.MeasureFile.FileHash);
                options = new CreateIndexOptions { Unique = false, Sparse = false, Name = "11" };
                _trialCollection.Indexes.CreateOne(trialCollectionKeys, options);

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
                options = new CreateIndexOptions { Unique = false, Sparse = true, Name = "GetSzenarioMeanTime" };
                _szenarioMeanTimeCollection.Indexes.CreateOne(szenarioMeanTimeCollectionKeys, options);

                // MeasureFileHash
                szenarioMeanTimeCollectionKeys = Builders<SzenarioMeanTime>.IndexKeys
                    .Ascending(t => t.MeasureFile.FileHash);
                options = new CreateIndexOptions { Unique = false, Sparse = false, Name = "MeasureFileHash" };
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
                        .Group(t => t.Group, u => new { u.Key })
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
                        .Group(t => t.Szenario, u => new { u.Key })
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
                        .Group(t => t.Szenario, u => new { u.Key })
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

        /// <summary>
        /// Gives an Enumerable of subjects that belong to the same study, same group and same SzenarioName
        /// </summary>
        /// <param name="studyName">Study to select from</param>
        /// <param name="groupName">group to select from</param>
        /// <param name="szenarioName">szenario to select from</param>
        /// <returns></returns>
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
                        .Group(t => t.Subject, u => new { u.Key })
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
                        .Group(t => t.Subject, u => new { u.Key })
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
                        .Group(t => t.MeasureFile.CreationTime, u => new { u.Key })
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
                        .Group(t => t.MeasureFile.CreationTime, u => new { u.Key })
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
        //TODO: Try to improve the runtime of this function, so it does not take 5 mins to run!
        //We could filter more by listing from the selectedTrials and checking...
        //Alternatively we probably have to hardcode the targets as a targetlist or smth...
        //Or maybe we could add the coordinates of the target in the GUI, as the GUI pulls from the TargetContainers anyways.
        //And then parse the GUI Information to the drawCircles function...
        //
        //We should use MyManipAnalysisFunctions.GetTargets I guess?!
        //Also check what happens, if we try to plot data that comes from different study, group, szenario, subjects!!

        /* Not being used right now...
        public List<TargetContainer> getTargetContainers(string studyName)
        {
            var filter = Builders<Trial>.Filter.And(Builders<Trial>.Filter.Eq(t => t.Study, studyName));
            var retVal =
                    _trialCollection.Aggregate()
                        .Match(filter)
                        .Group(t => t.Target, u => new { u.Key })
                        .ToList()
                        .Distinct()
                        .Select(t => t.Key);
            return retVal.ToList();
        }
        */

        public List<TargetContainer> getTargetContainers(string studyName, string groupName, string szenarioName, SubjectContainer subject)
        {
            var filter = Builders<Trial>.Filter.And(Builders<Trial>.Filter.Eq(t => t.Study, studyName),
                    Builders<Trial>.Filter.Eq(t => t.Group, groupName),
                    Builders<Trial>.Filter.Eq(t => t.Szenario, szenarioName),
                    Builders<Trial>.Filter.Eq(t => t.Subject, subject));
            var retVal =
                    _trialCollection.Aggregate()
                        .Match(filter)
                        .Group(t => t.Target, u => new { u.Key })
                        .ToList()
                        .Distinct()
                        .Select(t => t.Key);
            return retVal.ToList();
        }

        public List<TargetContainer> getTargetContainersFromTrajectoryVelocityPlotContainer(Container.TrajectoryVelocityPlotContainer container)
        {
            return getTargetContainers(container.Study, container.Group, container.Szenario, container.Subject);
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
                        .Group(t => t.Target.Number, u => new { u.Key })
                        .ToList()
                        .Select(t => t.Key);
                //_myManipAnalysisGui.WriteToLogBox(TicToc.Toc() + "ms \t" + "GetTargets(string studyName, string groupName, string szenarioName, SubjectContainer subject)");
                return retVal;
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox("MongoDbwrapper::GetTargets: " + ex);
                //return new List<TargetContainer>();
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
                        .Group(t => t.TargetTrialNumberInSzenario, u => new { u.Key })
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
                        .Group(t => t.TrialNumberInSzenario, u => new { u.Key })
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

        // TODO replace w/ commented section
        public IEnumerable<int> GetSzenarioTrials(string studyName, string groupName, string szenarioName, int target,
        SubjectContainer subject, IEnumerable<Trial.TrialTypeEnum> trialTypes,
        IEnumerable<Trial.ForceFieldTypeEnum> forceFields, IEnumerable<Trial.HandednessEnum> handedness)
        {
            IEnumerable<int> retVal;
            try
            {
                var filter = Builders<Trial>.Filter.And(Builders<Trial>.Filter.Eq(t => t.Study, studyName),
                    Builders<Trial>.Filter.Eq(t => t.Group, groupName),
                    Builders<Trial>.Filter.Eq(t => t.Szenario, szenarioName),
                    Builders<Trial>.Filter.Eq(t => t.Target.Number, target),
                    Builders<Trial>.Filter.Eq(t => t.Subject, subject),
                    Builders<Trial>.Filter.In(t => t.TrialType, trialTypes),
                    Builders<Trial>.Filter.In(t => t.ForceFieldType, forceFields),
                    Builders<Trial>.Filter.In(t => t.Handedness, handedness));

                //TicToc.Tic();
                retVal =
                    _trialCollection.Aggregate()
                        .Match(filter)
                        .Group(t => t.TrialNumberInSzenario, u => new { u.Key })
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


        public IEnumerable<int> GetSzenarioTrialsTrialNumber(string studyName, string groupName, string szenarioName, int target,
 SubjectContainer subject, int trialNumberInSzenario)
        {
            IEnumerable<int> retVal;
            try
            {
                List<int> temp_trNumList = new List<int>() { trialNumberInSzenario };
                var filter = Builders<Trial>.Filter.And(Builders<Trial>.Filter.Eq(t => t.Study, studyName),
                    Builders<Trial>.Filter.Eq(t => t.Group, groupName),
                    Builders<Trial>.Filter.Eq(t => t.Szenario, szenarioName),
                    //Builders<Trial>.Filter.Eq(t => t.Target.Number, target),
                    Builders<Trial>.Filter.Eq(t => t.Subject, subject),
                    Builders<Trial>.Filter.In(t => t.TrialNumberInSzenario, temp_trNumList)
                    );

                //TicToc.Tic();
                retVal =
                    _trialCollection.Aggregate()
                        .Match(filter)
                        .Group(t => t.TargetTrialNumberInSzenario, u => new { u.Key })
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
        /// <summary>
        /// Returns all trials, that belong to the same study, group, szenario, subject, turnDateTime and target and whose 
        /// trialNumberinSzenario is within targetTrials
        /// </summary>
        /// <param name="studyName"></param>
        /// <param name="groupName"></param>
        /// <param name="szenarioName"></param>
        /// <param name="subject"></param>
        /// <param name="turn"></param>
        /// <param name="target"></param>
        /// <param name="targetTrials"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
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
            SubjectContainer subject, DateTime turn, ProjectionDefinition<Trial> fields)
        {
            IEnumerable<Trial> retVal;
            try
            {
                fields = fields.Include(t => t.TargetTrialNumberInSzenario);
                retVal = _trialCollection
                    .Find(
                        t =>
                            t.Study == studyName && t.Group == groupName && t.Szenario == szenarioName &&
                            t.Subject == subject && t.MeasureFile.CreationTime == turn)
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

        public void UpdateTrialStatistics(Trial trial)
        {
            var filter = Builders<Trial>.Filter.Eq(t => t.Id, trial.Id);
            var update =
                Builders<Trial>.Update.Set(t => t.ZippedStatistics, trial.ZippedStatistics);
            _trialCollection.FindOneAndUpdate(filter, update);
        }

        /// <summary>
        /// Returns all trials from the currently selected study in the database, that do not have a their statistical parameters calculated yet.
        /// </summary>
        /// <param name="statisticFields">Projection, that selects, which attributes of the trials without statistics are to be selected</param>
        /// <returns>All trials that have no statistical parameters with the attributes that are selected via statisticFields.</returns>
        public IEnumerable<Trial> GetTrialsWithoutStatistics(ProjectionDefinition<Trial> statisticFields)
        {
            return _trialCollection
                .Find(t => t.ZippedStatistics == null)
                .Project<Trial>(statisticFields)
                .ToList();
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

        public void RemoveMeasureFile(MeasureFileContainer measureFile)
        {
            try
            {
                _trialCollection.DeleteMany(Builders<Trial>.Filter.Eq(t => t.MeasureFile, measureFile));
                _szenarioMeanTimeCollection.DeleteMany(Builders<SzenarioMeanTime>.Filter.Eq(t => t.MeasureFile,
                    measureFile));
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
    }
}