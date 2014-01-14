using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var query = _trialCollection.AsQueryable<Trial>().Select(t => t.Study).Distinct();
            return query.AsEnumerable();
        }

        public IEnumerable<string> GetGroupNames(string studyName)
        {
            var query = _trialCollection.AsQueryable<Trial>().Where(t => t.Study == studyName).Select(t => t.Group).Distinct();
            return query.AsEnumerable();
        }
    }
}
