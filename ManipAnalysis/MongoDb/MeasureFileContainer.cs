using System;

namespace ManipAnalysis_v2.MongoDb
{
    internal class MeasureFileContainer
    {
        private DateTime _creationTime;

        public string FileName { get; set; }

        public string FileHash { get; set; }

        /// <summary>
        ///     Convert to UTC First!!!
        /// </summary>
        public DateTime CreationTime
        {
            get { return _creationTime.ToLocalTime(); }
            set { _creationTime = value.ToUniversalTime(); }
        }
    }
}