using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest.Entities
{
    class Szenario
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
    }
}
