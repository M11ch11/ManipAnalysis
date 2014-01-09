using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManipAnalysis.MongoDb
{
    class PositionContainer
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        private DateTime _timeStamp;
        /// <summary>
        /// Convert to UTC First!!!
        /// </summary>
        public DateTime TimeStamp
        {
            get { return _timeStamp.ToLocalTime(); }
            set { _timeStamp = value.ToUniversalTime(); }
        }
        public int PositionStatus { get; set; }
    }
}
