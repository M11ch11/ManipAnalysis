using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManipAnalysis.MongoDb
{
    class Szenario
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
    }
}
