using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManipAnalysis.MongoDb
{
    class MeasureFile
    {
        public ObjectId Id { get; set; }
        public string FileName { get; set; }
        public int FileHash { get; set; }

        /// <summary>
        /// Convert to UTC First!!!
        /// </summary>
        public DateTime CreationTime { get; set; }
    }
}
