using System.Windows.Forms;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDbTest.Entities;

namespace MongoDbTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            var connectionString = "mongodb://localhost";
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            var database = server.GetDatabase("StudyTest");
            var collection = database.GetCollection<Trial>("Trials");

            var trial = new Trial();

            collection.Insert(trial);
            var id = trial.Id;

            var query = Query<Trial>.EQ(e => e.Id, id);
            trial = collection.FindOne(query);

            //entity.Name = "Dick";
            //collection.Save(entity);

            //var update = Update<Entity>.Set(e => e.Name, "Harry");
            //collection.Update(query, update);

            //collection.Remove(query);
            //server.Shutdown();
        }
    }
}
