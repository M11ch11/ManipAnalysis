using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace ManipAnalysis.MongoDb
{
    class Study
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
    }
}
