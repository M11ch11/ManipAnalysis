using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using ManipAnalysis.Container;
using ManipAnalysis.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;

namespace ManipAnalysis
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

        private string _mongoDbDatabaseString;
        private string _mongoDbPasswordString;
        private string _mongoDbServerString;
        private string _mongoDbUsernameString;

        public MongoDbWrapper()
        {
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

        public IEnumerable<string> GetStudyNames()
        {
            return _trialCollection.AsQueryable().Select(t => t.Study).Distinct();
        }

        public IEnumerable<string> GetGroupNames(string studyName)
        {
            return _trialCollection.AsQueryable().Where(t => t.Study == studyName).Select(t => t.Group).Distinct();
        }

        public IEnumerable<string> GetSzenarioNames(string studyName, string groupName)
        {
            return _trialCollection.AsQueryable().Where(t => t.Study == studyName && t.Group == groupName).Select(t => t.Szenario).Distinct();
        }

        public IEnumerable<SubjectContainer> GetSubjectInformations(string studyName, string groupName, string szenarioName)
        {
            return _trialCollection.AsQueryable().Where(t => t.Study == studyName && t.Group == groupName && t.Szenario == szenarioName).Select(t => t.Subject).Distinct();
        }

        public IEnumerable<DateTime> GetTurns(string studyName, string groupName, string szenarioName, SubjectContainer subject)
        {
            return _trialCollection.AsQueryable().Where(t => t.Study == studyName && t.Group == groupName && t.Szenario == szenarioName && t.Subject == subject).Select(t => t.MeasureFile.CreationTime).Distinct();
        }

        public IEnumerable<int> GetTargetNumbers(string studyName, string szenarioName)
        {
            return _trialCollection.AsQueryable().Where(t => t.Study == studyName && t.Szenario == szenarioName).Select(t => t.Target.Number).Distinct();
        }

        public IEnumerable<int> GetTrialNumbers(string studyName, string szenarioName)
        {
            return _trialCollection.AsQueryable().Where(t => t.Study == studyName && t.Szenario == szenarioName).Select(t => t.TargetTrialNumberInSzenario).Distinct();
        }

        public List<VelocityContainer> GetNormalizedVelocity(string studyName, string groupName, string szenarioName,
            SubjectContainer subject, DateTime turn, int target, int trial, bool showNormalTrials, bool showCatchTrials, bool showErrorclampTrials)
        {
            if(showNormalTrials && showCatchTrials && showErrorclampTrials)
            {
                return _trialCollection.AsQueryable().SingleOrDefault(t => t.Study == studyName && t.Group == groupName && t.Szenario == szenarioName && t.Subject == subject && t.MeasureFile.CreationTime == turn && t.Target.Number == target && t.TargetTrialNumberInSzenario == trial).VelocityNormalized;
            }
            else if (showNormalTrials && !showCatchTrials && showErrorclampTrials)
            {
                return _trialCollection.AsQueryable().SingleOrDefault(t => t.Study == studyName && t.Group == groupName && t.Szenario == szenarioName && t.Subject == subject && t.MeasureFile.CreationTime == turn && t.Target.Number == target && t.TargetTrialNumberInSzenario == trial && !t.CatchTrial).VelocityNormalized;
            }
            else if (showNormalTrials && showCatchTrials && !showErrorclampTrials)
            {
                return _trialCollection.AsQueryable().SingleOrDefault(t => t.Study == studyName && t.Group == groupName && t.Szenario == szenarioName && t.Subject == subject && t.MeasureFile.CreationTime == turn && t.Target.Number == target && t.TargetTrialNumberInSzenario == trial && !t.ErrorClampTrial).VelocityNormalized;
            }
            else if (showNormalTrials && !showCatchTrials && !showErrorclampTrials)
            {
                return _trialCollection.AsQueryable().SingleOrDefault(t => t.Study == studyName && t.Group == groupName && t.Szenario == szenarioName && t.Subject == subject && t.MeasureFile.CreationTime == turn && t.Target.Number == target && t.TargetTrialNumberInSzenario == trial && !t.CatchTrial && !t.ErrorClampTrial).VelocityNormalized;
            }
            else if (!showNormalTrials && showCatchTrials && showErrorclampTrials)
            {
                return _trialCollection.AsQueryable().SingleOrDefault(t => t.Study == studyName && t.Group == groupName && t.Szenario == szenarioName && t.Subject == subject && t.MeasureFile.CreationTime == turn && t.Target.Number == target && t.TargetTrialNumberInSzenario == trial && t.CatchTrial && t.ErrorClampTrial).VelocityNormalized;
            }
            else if (!showNormalTrials && !showCatchTrials && showErrorclampTrials)
            {
                return _trialCollection.AsQueryable().SingleOrDefault(t => t.Study == studyName && t.Group == groupName && t.Szenario == szenarioName && t.Subject == subject && t.MeasureFile.CreationTime == turn && t.Target.Number == target && t.TargetTrialNumberInSzenario == trial && !t.CatchTrial && t.ErrorClampTrial).VelocityNormalized;
            }
            else if (!showNormalTrials && showCatchTrials && !showErrorclampTrials)
            {
                return _trialCollection.AsQueryable().SingleOrDefault(t => t.Study == studyName && t.Group == groupName && t.Szenario == szenarioName && t.Subject == subject && t.MeasureFile.CreationTime == turn && t.Target.Number == target && t.TargetTrialNumberInSzenario == trial && t.CatchTrial && !t.ErrorClampTrial).VelocityNormalized;
            }
            else
            {
                return null;
            }
        }

        public List<PositionContainer> GetNormalizedPosition(string studyName, string groupName, string szenarioName,
            SubjectContainer subject, DateTime turn, int target, int trial, bool showNormalTrials, bool showCatchTrials, bool showErrorclampTrials)
        {
            if (showNormalTrials && showCatchTrials && showErrorclampTrials)
            {
                return _trialCollection.AsQueryable().SingleOrDefault(t => t.Study == studyName && t.Group == groupName && t.Szenario == szenarioName && t.Subject == subject && t.MeasureFile.CreationTime == turn && t.Target.Number == target && t.TargetTrialNumberInSzenario == trial).PositionNormalized;
            }
            else if (showNormalTrials && !showCatchTrials && showErrorclampTrials)
            {
                return _trialCollection.AsQueryable().SingleOrDefault(t => t.Study == studyName && t.Group == groupName && t.Szenario == szenarioName && t.Subject == subject && t.MeasureFile.CreationTime == turn && t.Target.Number == target && t.TargetTrialNumberInSzenario == trial && !t.CatchTrial).PositionNormalized;
            }
            else if (showNormalTrials && showCatchTrials && !showErrorclampTrials)
            {
                return _trialCollection.AsQueryable().SingleOrDefault(t => t.Study == studyName && t.Group == groupName && t.Szenario == szenarioName && t.Subject == subject && t.MeasureFile.CreationTime == turn && t.Target.Number == target && t.TargetTrialNumberInSzenario == trial && !t.ErrorClampTrial).PositionNormalized;
            }
            else if (showNormalTrials && !showCatchTrials && !showErrorclampTrials)
            {
                return _trialCollection.AsQueryable().SingleOrDefault(t => t.Study == studyName && t.Group == groupName && t.Szenario == szenarioName && t.Subject == subject && t.MeasureFile.CreationTime == turn && t.Target.Number == target && t.TargetTrialNumberInSzenario == trial && !t.CatchTrial && !t.ErrorClampTrial).PositionNormalized;
            }
            else if (!showNormalTrials && showCatchTrials && showErrorclampTrials)
            {
                return _trialCollection.AsQueryable().SingleOrDefault(t => t.Study == studyName && t.Group == groupName && t.Szenario == szenarioName && t.Subject == subject && t.MeasureFile.CreationTime == turn && t.Target.Number == target && t.TargetTrialNumberInSzenario == trial && t.CatchTrial && t.ErrorClampTrial).PositionNormalized;
            }
            else if (!showNormalTrials && !showCatchTrials && showErrorclampTrials)
            {
                return _trialCollection.AsQueryable().SingleOrDefault(t => t.Study == studyName && t.Group == groupName && t.Szenario == szenarioName && t.Subject == subject && t.MeasureFile.CreationTime == turn && t.Target.Number == target && t.TargetTrialNumberInSzenario == trial && !t.CatchTrial && t.ErrorClampTrial).PositionNormalized;
            }
            else if (!showNormalTrials && showCatchTrials && !showErrorclampTrials)
            {
                return _trialCollection.AsQueryable().SingleOrDefault(t => t.Study == studyName && t.Group == groupName && t.Szenario == szenarioName && t.Subject == subject && t.MeasureFile.CreationTime == turn && t.Target.Number == target && t.TargetTrialNumberInSzenario == trial && t.CatchTrial && !t.ErrorClampTrial).PositionNormalized;
            }
            else
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
    }
}
