using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace ManipAnalysis.MongoDb
{
    class Target
    {
        public ObjectId Id { get; set; }
        public int Number { get; set; }
        public int XPos { get; set; }
        public int YPos { get; set; }
        public int ZPos { get; set; }
    }
}
