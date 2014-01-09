using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManipAnalysis.MongoDb
{
    class MeasureFileContainer
    {
        public string FileName { get; set; }
        public string FileHash { get; set; }
        private DateTime _creationTime;
        /// <summary>
        /// Convert to UTC First!!!
        /// </summary>
        public DateTime CreationTime
        {
            get { return _creationTime.ToLocalTime(); }
            set { _creationTime = value.ToUniversalTime(); }
        }
    }
}
